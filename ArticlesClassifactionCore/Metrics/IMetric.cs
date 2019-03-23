using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Metrics
{
    public interface IMetric
    {
        double CalculateDistance(List<double> vector1, List<double> vector2);
    }
}
