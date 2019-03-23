using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore
{
    public class KnnArticle
    {
        public string Label { get; set; }
        public string PredictedLabel { get; set; }
        public List<double> FeaturesVector { get; set; }
    }
}
