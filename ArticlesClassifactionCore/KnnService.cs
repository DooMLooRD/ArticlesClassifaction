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

        public KnnService(FeaturesVectorService featuresVectorService, IMetric metric, int k)
        {
            FeaturesVectorService = featuresVectorService;
            Metric = metric;
            K = k;
        }

        public void InitKnn(List<PreprocessedArticle> articles)
        {
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
                var distinctNeighborsSum = neighborsWithSameCount.GroupBy(t => t.Item1).Select(g => (g.Key, g.Sum(t => t.Item2))).OrderBy(e => e.Item2).ToList();
                knnArticle.PredictedLabel = distinctNeighborsSum[0].Item1;
                ClassifiedArticles.Add(knnArticle);
                return distinctNeighborsSum[0].Item1;
            }

            knnArticle.PredictedLabel = distinctNeighborsCount[0].Item1;
            ClassifiedArticles.Add(knnArticle);
            return distinctNeighborsCount[0].Item1;
        }


    }
}
