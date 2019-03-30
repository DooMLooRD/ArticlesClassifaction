using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.Features;
using ArticlesClassificationView.ViewModels.Base;

namespace ArticlesClassificationView.ViewModels
{
    public class ExtractorVM : BaseViewModel
    {
        public IFeatureExtractor FeatureExtractor { get; set; }
        public bool IsChecked { get; set; }
        public List<CheckBoxVM> Features { get; set; }
    }
}
