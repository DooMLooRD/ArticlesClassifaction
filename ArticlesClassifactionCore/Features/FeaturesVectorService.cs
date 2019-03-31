using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.Features.FeatureExtractors;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features
{
    public class FeaturesVectorService
    {
        public Dictionary<string, List<string>> KeyWords { get; set; }
        public ISimilarityFunction SimilarityFunction { get; set; }
        public List<IFeatureExtractor> FeatureExtractors { get; set; }

        public FeaturesVectorService(Dictionary<string, List<string>> keyWords, ISimilarityFunction similarityFunction, List<IFeatureExtractor> featureExtractors)
        {
            KeyWords = keyWords;
            SimilarityFunction = similarityFunction;
            FeatureExtractors = featureExtractors;

        }
        public List<double> GetFeaturesVector(PreprocessedArticle article)
        {
            List<double> features = new List<double>();
            foreach (IFeatureExtractor featureExtractor in FeatureExtractors)
            {
                features.AddRange(featureExtractor.GetFeatures(article, SimilarityFunction));
            }

            return features;
        }
    }
}
