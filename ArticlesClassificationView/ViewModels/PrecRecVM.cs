using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassificationView.ViewModels
{
    public class PrecRecVM
    {
        public string Tag { get; set; }
        public double Precision { get; set; }
        public double Recall { get; set; }
    }
}
