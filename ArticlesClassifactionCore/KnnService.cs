using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.Features;
using ArticlesClassifactionCore.Metrics;

namespace ArticlesClassifactionCore
{
    public class KnnService
    {
        public FeaturesVectorService FeaturesVectorService { get; set; }
        public IMetric Metric { get; set; }
        public List<KnnArticle> ClassifiedArticles { get; set; }
        public int K { get; set; }
        private int coldStartNumber;

        public KnnService(FeaturesVectorService featuresVectorService, IMetric metric, int k)
        {
            FeaturesVectorService = featuresVectorService;
            Metric = metric;
            K = k;
        }

        public void InitKnn(List<PreprocessedArticle> articles)
        {
            coldStartNumber = articles.Count;
            ClassifiedArticles = articles.Select(t => new KnnArticle() { Label = t.Label, PredictedLabel = t.Label, FeaturesVector = FeaturesVectorService.GetFeaturesVector(t) }).ToList();
        }

        public string ClassifyArticle(PreprocessedArticle article)
        {
            KnnArticle knnArticle = new KnnArticle { Label = article.Label, FeaturesVector = FeaturesVectorService.GetFeaturesVector(article) };
            List<(string, double)> neighbors = new List<(string, double)>();
            foreach (KnnArticle classifiedArticle in ClassifiedArticles)
            {
                neighbors.Add((classifiedArticle.PredictedLabel, Metric.CalculateDistance(classifiedArticle.FeaturesVector, knnArticle.FeaturesVector)));
            }

            var orderedNeighbors = neighbors.OrderBy(t => t.Item2).Take(K).ToList();
            var distinctNeighborsCount = orderedNeighbors.GroupBy(t => t.Item1).Select(g => (g.Key, g.Count())).OrderByDescending(e => e.Item2).ToList();
            var neighborsWithSameCount =
                distinctNeighborsCount.Where(t => t.Item2 == distinctNeighborsCount[0].Item2).ToList();
            if (neighborsWithSameCount.Count != 1)
            {
                var orderedNeighborsWithSameCount =
                    orderedNeighbors.Where(c => neighborsWithSameCount.Select(d => d.Item1).Contains(c.Item1));
                var distinctNeighborsSum = orderedNeighborsWithSameCount.GroupBy(t => t.Item1).Select(g => (g.Key, g.Sum(t => t.Item2))).OrderBy(e => e.Item2).ToList();
                knnArticle.PredictedLabel = distinctNeighborsSum[0].Item1;
                ClassifiedArticles.Add(knnArticle);
                return distinctNeighborsSum[0].Item1;
            }

            knnArticle.PredictedLabel = distinctNeighborsCount[0].Item1;
            ClassifiedArticles.Add(knnArticle);
            return distinctNeighborsCount[0].Item1;
        }

        public Dictionary<string, Dictionary<string, int>> CalculateConfusionMatrix(List<string> tags)
        {
            Dictionary<string, Dictionary<string, int>> matrix = new Dictionary<string, Dictionary<string, int>>();
            foreach (var tag in tags)
            {
                Dictionary<string, int> row = new Dictionary<string, int>();
                foreach (var tag1 in tags)
                {
                    row.Add(tag1, 0);
                }
                matrix.Add(tag, row);
            }
            foreach (KnnArticle classifiedArticle in ClassifiedArticles.Skip(coldStartNumber))
            {
                matrix[classifiedArticle.Label][classifiedArticle.PredictedLabel]++;
            }

            return matrix;
        }

        public Dictionary<string, (double, double, double, double)> CalculateTpFpTnFn(
            Dictionary<string, Dictionary<string, int>> matrix)
        {
            Dictionary<string, (double, double, double, double)> dic = new Dictionary<string, (double, double, double, double)>();
            foreach (var row in matrix)
            {
                dic.Add(row.Key, (0, 0, 0, 0));
            }

            foreach (var row in matrix)
            {
                double tp = row.Value[row.Key];
                double fp = 0;
                double fn = 0;
                double tn = 0;
                foreach (var row1 in matrix)
                {
                    fp += row1.Value[row.Key];
                    fn += row.Value[row1.Key];
                }
                foreach (var row1 in matrix)
                {
                    foreach (var row2 in matrix)
                    {
                        tn += row1.Value[row2.Key];
                    }
                }

                fp -= tp;
                fn -= tp;
                tn += -tp - fp - fn;
                dic[row.Key] = (tp, fp, tn, fn);
            }

            return dic;
        }

        public Dictionary<string, (double, double)> CalculatePrecRec(
            Dictionary<string, (double, double, double, double)> TpFpTnFn)
        {
            Dictionary<string, (double, double)> dic = new Dictionary<string, (double, double)>();
            foreach (var row in TpFpTnFn)
            {
                double tp = row.Value.Item1;
                double fp = row.Value.Item2;
                double tn = row.Value.Item3;
                double fn = row.Value.Item4;

                dic.Add(row.Key, (tp / (tp + fp), tp / (tp + fn)));
            }

            return dic;
        }

        public string ConfusionMatrixAsText(Dictionary<string, Dictionary<string, int>> matrix)
        {
            int maxLetters = matrix.Keys.Max(c => c.Length) + 2;
            List<int> lettersLength = matrix.Keys.Select(c => c.Length + 2).ToList();
            StringBuilder builder = new StringBuilder();
            builder.Append("".PadLeft(maxLetters));
            for (int i = 0; i < matrix.Keys.Count; i++)
            {
                builder.Append(matrix.ElementAt(i).Key.PadLeft(lettersLength[i]));
            }
            builder.AppendLine();
            foreach (var row in matrix)
            {
                builder.Append(row.Key.PadLeft(maxLetters));
                for (int i = 0; i < row.Value.Count; i++)
                {
                    builder.Append(row.Value.ElementAt(i).Value.ToString().PadLeft(lettersLength[i]));
                }

                builder.AppendLine();
            }
            return builder.ToString();
        }

    }
}
