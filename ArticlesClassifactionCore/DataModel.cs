using System;
using System.Collections.Generic;

namespace ArticlesClassifactionCore
{
    public class DataModel
    {
        public DateTime? Date { get; set; }
        public Dictionary<string, List<string>> Tags { get; set; }=new Dictionary<string, List<string>>();
        public string Unknown { get; set; }
        public Text Text { get; set; }=new Text();
    }

    public class Text
    {
        public string Title { get; set; }
        public string Body { get; set; }
        public string Author { get; set; }
        public string Dateline { get; set; }
    }
}
