using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassificationView.ViewModels.Base;

namespace ArticlesClassificationView.ViewModels
{
    public class KeyWordVM : BaseViewModel
    {
        public string Tag { get; set; }
        public string Word { get; set; }
    }
}
