using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Metrics
{
    public class EuclideanMetric : IMetric
    {
        public double CalculateDistance(List<double> vector1, List<double> vector2)
        {
            double result = 0;
            if (vector1.Count != vector2.Count)
                return 0;
            for (int i = 0; i < vector1.Count; i++)
            {
                result += Math.Pow(vector1[i] - vector2[i], 2);
            }

            return Math.Sqrt(result);
        }
    }
}
