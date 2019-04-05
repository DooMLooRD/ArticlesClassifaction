using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassificationView.ViewModels.Base;

namespace ArticlesClassificationView.ViewModels
{
    public class ResultVM : BaseViewModel
    {
        public string Tag { get; set; }
        public int All { get; set; }
        public int Tp { get; set; }
        public int Fp { get; set; }
    }
}
