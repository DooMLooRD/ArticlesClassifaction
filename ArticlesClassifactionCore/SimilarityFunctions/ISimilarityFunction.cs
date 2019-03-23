using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.SimilarityFunctions
{
    public interface ISimilarityFunction
    {
        double CalculateSimilarity(string word1, string word2);
    }
}
