using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.SimilarityFunctions;

namespace ArticlesClassifactionCore.Features
{
    public interface IFeatureExtractor
    {
        List<Feature> Features { get; set; }
        List<double> GetFeatures(PreprocessedArticle article,ISimilarityFunction function);
    }
}
