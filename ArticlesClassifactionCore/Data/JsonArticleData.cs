using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Data
{
    public class JsonArticleData
    {
        public List<JsonArticle> Articles { get; set; }
    }

    public class JsonArticle
    {
        public string Tag { get; set; }
        public string Text { get; set; }
    }
}
