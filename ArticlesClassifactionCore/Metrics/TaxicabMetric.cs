using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Metrics
{
    public class TaxicabMetric : IMetric
    {
        public double CalculateDistance(List<double> vector1, List<double> vector2)
        {
            double result = 0;
            for (int i = 0; i < vector1.Count; i++)
            {
                result += Math.Abs(vector1[i] - vector2[i]);
            }

            return result;
        }

        public override string ToString()
        {
            return "Taxicab metric";
        }
    }
}
