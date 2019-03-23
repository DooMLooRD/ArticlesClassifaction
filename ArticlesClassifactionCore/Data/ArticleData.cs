using System;
using System.Collections.Generic;
using System.Linq;

namespace ArticlesClassifactionCore.Data
{
    public class ArticleData
    {
        public DateTime? Date { get; set; }
        public Dictionary<string, List<string>> Tags { get; set; } 
        public string Unknown { get; set; }
        public Text Text { get; set; }

        public ArticleData()
        {
            Text = new Text();
            Tags = new Dictionary<string, List<string>>();
        }
    }

    public class Text
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string Dateline { get; set; }
        public List<string> GetWordsFromBody()
        {
            return new string(Body?.Where(c=> !(char.IsPunctuation(c) || char.IsDigit(c) || char.IsSymbol(c))).ToArray()).Split().Select(s=>s.ToLower()).ToList();
        }
    }
}
