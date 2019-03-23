using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features
{
    public class FeaturesVectorService
    {
        public Dictionary<string, List<string>> KeyWords { get; set; }
        public ISimilarityFunction SimilarityFunction { get; set; }

        public FeaturesVectorService(Dictionary<string, List<string>> keyWords, ISimilarityFunction similarityFunction)
        {
            KeyWords = keyWords;
            SimilarityFunction = similarityFunction;
        }
        public List<double> GetFeaturesVector(PreprocessedArticle article)
        {
            List<double> features = new List<double>();
            foreach (string tag in KeyWords.Keys)
            {
                double feature = 0;
                foreach (string keyWord in KeyWords[tag])
                {
                    feature += article.Words.Sum(t => SimilarityFunction.CalculateSimilarity(keyWord, t));
                }
                features.Add(feature);
            }

            return features;
        }
    }
}
