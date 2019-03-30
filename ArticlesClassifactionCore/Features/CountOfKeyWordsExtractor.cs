using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features
{
    public class CountOfKeyWordsExtractor : IFeatureExtractor
    {
        public Dictionary<string, List<string>> KeyWords { get; set; }
        public CountOfKeyWordsExtractor(Dictionary<string, List<string>> keyWords)
        {
            Features = new List<Feature>();

            KeyWords = keyWords;
            ExtractFeatures(keyWords.Keys);
        }

        public List<Feature> Features { get; set; }

        private void ExtractFeatures(IEnumerable<string> tags)
        {
            foreach (string tag in tags)
            {
                Features.Add(new Feature() { Name = tag, IsChecked = true });
            }
        }



        public List<double> GetFeatures(PreprocessedArticle article, ISimilarityFunction function)
        {
            List<double> features = new List<double>();
            List<string> enabledFeatures = Features.Where(c => c.IsChecked).Select(t => t.Name).ToList();
            foreach (string tag in enabledFeatures)
            {
                double feature = 0;

                foreach (string keyWord in KeyWords[tag])
                {
                    feature += article.Words.Distinct().Contains(keyWord) ? 1 : 0;
                }

                features.Add(feature/KeyWords[tag].Count);
            }

            return features;
        }

        public override string ToString()
        {
            return "Count of distinct key words";
        }
    }
}
