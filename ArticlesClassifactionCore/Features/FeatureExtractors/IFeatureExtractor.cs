using System.Collections.Generic;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features.FeatureExtractors
{
    public interface IFeatureExtractor
    {
        List<Feature> Features { get; set; }
        List<double> GetFeatures(PreprocessedArticle article);
    }
}
