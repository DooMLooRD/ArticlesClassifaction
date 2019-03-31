using System.Collections.Generic;
using System.Linq;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features.FeatureExtractors
{
    public class ArticleExtractor : IFeatureExtractor
    {
        public ArticleExtractor()
        {
            Features = new List<Feature>() { new Feature() { Name = "Article word count", IsChecked = true }, new Feature() { Name = "Average word length", IsChecked = true } };
        }
        public List<Feature> Features { get; set; }
        public List<double> GetFeatures(PreprocessedArticle article, ISimilarityFunction function)
        {
            return new List<double>() { article.Words.Count, article.Words.Average(c => c.Length) };
        }

        public override string ToString()
        {
            return "Article extractor";
        }
    }
}
