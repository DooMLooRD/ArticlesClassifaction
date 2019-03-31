using System;
using System.Collections.Generic;
using System.Linq;

namespace ArticlesClassifactionCore.Features
{
    public class KeyWordsExtractor
    {
        public List<PreprocessedArticle> Articles { get; set; }
        public Dictionary<string,int> DocumentFrequency { get; set; }
        public Dictionary<string,int> WordsNumber { get; set; }

        public KeyWordsExtractor(List<PreprocessedArticle> articles)
        {
            Articles = articles;
            CalculateWordsNumber();
            CalculateDocumentFrequency();
        }

        private void CalculateWordsNumber()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            List<string> distinctWords = Articles.SelectMany(t => t.Words).Distinct().ToList();
            foreach (string word in distinctWords)
            {
                dictionary.Add(word, 0);
            }

            foreach (PreprocessedArticle knnArticle in Articles)
            {
                foreach (string s in knnArticle.Words)
                {
                    dictionary[s]++;
                }
            }
            WordsNumber = dictionary;
        }


        public Dictionary<string, double> CalculateTFIDF(PreprocessedArticle article)
        {
            Dictionary<string,double> dictionary=new Dictionary<string, double>();
            
            Dictionary<string, double> termFrequencies = TermFrequency(article);
            foreach (string s in article.Words.Distinct())
            {
                dictionary.Add(s,termFrequencies[s]*IDF(s));
            }

            return dictionary;
        }

        private double IDF(string word)
        {
            return Math.Log(Articles.Count / (double)DocumentFrequency[word]);
        }
        private static Dictionary<string, double> TermFrequency(PreprocessedArticle article)
        {
            Dictionary<string, double> distinctWords = new Dictionary<string, double>();
            foreach (string word in article.Words)
            {
                if (distinctWords.ContainsKey(word))
                    distinctWords[word]++;
                else
                    distinctWords.Add(word, 1);
            }

            foreach (string s in distinctWords.Keys.ToList())
            {
                distinctWords[s] /= article.Words.Count;
            }
            return distinctWords;
        }

        private void CalculateDocumentFrequency()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            List<string> distinctWords = Articles.SelectMany(t => t.Words).Distinct().ToList();
            foreach (string word in distinctWords)
            {
                dictionary.Add(word,0);
            }

            foreach (PreprocessedArticle knnArticle in Articles)
            {
                foreach (string s in knnArticle.Words.Distinct())
                {
                    dictionary[s]++;
                }
            }
            DocumentFrequency = dictionary;
        }
    }
}
