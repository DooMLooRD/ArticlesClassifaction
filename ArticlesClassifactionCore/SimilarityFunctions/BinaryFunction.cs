using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.SimilarityFunctions
{
    public class BinaryFunction : ISimilarityFunction
    {
        public double CalculateSimilarity(string word1, string word2)
        {
            if (word1 == word2)
                return 1;
            return 0;
        }

        public override string ToString()
        {
            return "Binary Function";
        }
    }
}
