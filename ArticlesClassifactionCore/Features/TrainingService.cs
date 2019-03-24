using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore.Features
{
    public class TrainingService
    {
        public string Category { get; set; }
        public List<string> Tags { get; set; }
        public List<PreprocessedArticle> Articles { get; set; }
        public Dictionary<string, List<string>> KeyWords { get; set; }

        public TrainingService(string category, List<string> tags, List<PreprocessedArticle> articles)
        {
            Category = category;
            Tags = tags;
            Articles = articles;
            KeyWords = new Dictionary<string, List<string>>();
            foreach (string tag in tags)
            {
                KeyWords.Add(tag, new List<string>());
            }
        }
        public void Train()
        {
            KeyWordsExtractor extractor = new KeyWordsExtractor(Articles);

            foreach (string tag in Tags)
            {
                List<PreprocessedArticle> articles = Articles.Where(t => t.Label == tag).ToList();
                List<(string, double)> tfidf = new List<(string, double)>();
                foreach (PreprocessedArticle article in articles)
                {
                    tfidf.AddRange(extractor.CalculateTFIDF(article).Select(t => (t.Key, t.Value)));
                }
                var keyWords = tfidf.OrderByDescending(n => n.Item2)
                    .ToList();
                KeyWords[tag] = keyWords.Select(n => n.Item1).Distinct().Take(10).ToList();
            }
        }
    }
}
