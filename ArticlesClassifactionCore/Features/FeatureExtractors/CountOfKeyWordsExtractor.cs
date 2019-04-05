using System.Collections.Generic;
using System.Linq;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features.FeatureExtractors
{
    public class CountOfKeyWordsExtractor : IFeatureExtractor
    {
        public Dictionary<string, List<string>> KeyWords { get; set; }
        public double PercentOfData { get; set; }
        public CountOfKeyWordsExtractor(Dictionary<string, List<string>> keyWords, bool countAll)
        {
            if (countAll)
                PercentOfData = 1;
            else
                PercentOfData = 0.3;
            Features = new List<Feature>();

            KeyWords = keyWords;
            ExtractFeatures(keyWords.Keys);
        }

        public List<Feature> Features { get; set; }

        private void ExtractFeatures(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                Features.Add(new Feature() { Name = tag });
            }
        }



        public List<double> GetFeatures(PreprocessedArticle article)
        {
            List<double> features = new List<double>();
            List<string> enabledFeatures = Features.Select(t => t.Name).ToList();
            foreach (string tag in enabledFeatures)
            {
                double feature = 0;

                foreach (string keyWord in KeyWords[tag])
                {
                    feature += article.Words.Take((int)(article.Words.Count * PercentOfData)).Distinct().Contains(keyWord) ? 1 : 0;
                }

                features.Add(feature);
            }

            double max = features.Max();
            if (!max.Equals(0))
            {
                for (int i = 0; i < features.Count; i++)
                {
                    features[i] /= max;
                }
            }
            return features;
        }

        public override string ToString()
        {
            if (PercentOfData.Equals(1))
                return "Count of distinct key words";
            return "Count of distinct key words at first 30% of text";
        }
    }
}
