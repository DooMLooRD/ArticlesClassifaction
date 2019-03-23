using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassificationView.ViewModels.Base;

namespace ArticlesClassificationView.ViewModels
{
    public class TagVM : BaseViewModel
    {
        public string Name { get; set; }
        public bool IsChecked { get; set; }
    }
}
