using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.SimilarityFunctions
{
    public class NGramFunction : ISimilarityFunction
    {
        public int N { get; set; }

        public NGramFunction(int n)
        {
            N = n;
        }
        public double CalculateSimilarity(string word1, string word2)
        {
            if (word1.Length > word2.Length)
            {
                string tmp = word1;
                word1 = word2;
                word2 = tmp;
            }
            int smallestLetterNumber = word1.Length;
            int n = N;
            if (smallestLetterNumber < N)
                n = smallestLetterNumber;
            int substringsNumber = word2.Length - n + 1;
            int occur = 0;
            for (int i = 0; i < substringsNumber; i++)
            {
                string ngram = word2.Substring(i, n);
                if (word1.Contains(ngram))
                    occur++;
            }
            return (1.0 / substringsNumber) * occur;
        }

        public override string ToString()
        {
            return "N-Gram Function";
        }
    }
}
