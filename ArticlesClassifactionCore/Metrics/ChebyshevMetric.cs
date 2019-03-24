using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Metrics
{
    public class ChebyshevMetric : IMetric
    {
        public double CalculateDistance(List<double> vector1, List<double> vector2)
        {
            List<double> values = new List<double>();
            for (int i = 0; i < vector1.Count; i++)
            {
                values.Add(Math.Abs(vector1[i] - vector2[i]));
            }
            return values.Max();
        }

        public override string ToString()
        {
            return "Chebyshev Metric";
        }
    }
}
