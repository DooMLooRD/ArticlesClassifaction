﻿using System;
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
            Dictionary<string, List<string>> keyWords = new Dictionary<string, List<string>>();
            foreach (string tag in Tags)
            {

                List<PreprocessedArticle> articles = Articles.Where(t => t.Label == tag).ToList();
                KeyWordsExtractor extractor = new KeyWordsExtractor(articles);

                List<(string, int)> df = new List<(string, int)>();
                df.AddRange(extractor.WordsNumber.Select(t => (t.Key, t.Value)));

                keyWords.Add(tag, df.OrderByDescending(n => n.Item2).Select(t => t.Item1).ToList());
            }

            foreach (string tag in Tags)
            {
                //var notInTag = new List<string>();
                //foreach (string s in Tags)
                //{
                //    if (s != tag)
                //    {
                //        notInTag.AddRange(keyWords[s].Take((int)(keyWords[s].Count*0.2)));
                //    }
                //}
                //var distinctInTag = keyWords[tag].Except(notInTag).ToList();
                KeyWords[tag] = keyWords[tag].Take(30).ToList();
            }
        }
    }
}
