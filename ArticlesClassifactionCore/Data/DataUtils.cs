using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Data
{
    public class DataUtils
    {
        public static List<string> GetCategories(List<ArticleData> articles)
        {
            return articles.SelectMany(t => t.Tags.Keys).Distinct().ToList();
        }


        public static List<string> GetTags(List<ArticleData> articles, string category)
        {
            return articles.Where(t => t.Tags.ContainsKey(category)).SelectMany(a => a.Tags[category]).Distinct().ToList();
        }

        public static List<ArticleData> Filter(List<ArticleData> articles, string category, List<string> tags)
        {
            return articles.Where(n => n.Tags.ContainsKey(category) && n.Tags[category].Count == 1 && tags.Contains(n.Tags[category][0])).ToList();
        }

    }
}
