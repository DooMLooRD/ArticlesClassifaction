using System.Collections.Generic;
using System.Linq;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features.FeatureExtractors
{
    public class SumOfSimilarityArticleKeyWordsExtractor : IFeatureExtractor
    {
        public Dictionary<string, List<string>> KeyWords { get; set; }
        public SumOfSimilarityArticleKeyWordsExtractor(Dictionary<string, List<string>> keyWords)
        {
            Features = new List<Feature>();
            KeyWords = keyWords;
            ExtractFeatures(keyWords.Keys);
        }

        private void ExtractFeatures(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                Features.Add(new Feature() { Name = tag, IsChecked = true });
            }
        }

        public List<Feature> Features { get; set; }

        public List<double> GetFeatures(PreprocessedArticle article, ISimilarityFunction function)
        {
            List<double> features = new List<double>();
            List<string> enabledFeatures = Features.Where(c => c.IsChecked).Select(t => t.Name).ToList();
            foreach (string tag in enabledFeatures)
            {
                double feature = 0;

                foreach (string keyWord in KeyWords[tag])
                {
                    feature += article.Words.Sum(t => function.CalculateSimilarity(keyWord, t));
                }

                if (feature.Equals(0))
                    features.Add(0);
                else
                    features.Add(feature / article.Words.Count);
            }

            return features;
        }

        public override string ToString()
        {
            return "Sum of Similarity Article-KeyWords";
        }
    }
}
