using System;
using System.Collections.Generic;
using System.Linq;

namespace ArticlesClassifactionCore.Features
{
    public class KeyWordsExtractor
    {
        public List<PreprocessedArticle> Articles { get; set; }
        public Dictionary<string, int> DocumentFrequency { get; set; }
        public Dictionary<string, int> WordsNumber { get; set; }

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


        public double CalculateTFIDF(string word)
        {
            double result = 0;
            foreach (PreprocessedArticle preprocessedArticle in Articles)
            {
                result += TermFrequency(preprocessedArticle, word) * IDF(word);
            }
            return result;
        }

        private double IDF(string word)
        {
            return Math.Log(Articles.Count / (double)DocumentFrequency[word]);
        }
        private static double TermFrequency(PreprocessedArticle article, string word)
        {
            return article.Words.Where(c => c == word).ToList().Count / (double)article.Words.Count;
        }

        private void CalculateDocumentFrequency()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            List<string> distinctWords = Articles.SelectMany(t => t.Words).Distinct().ToList();
            foreach (string word in distinctWords)
            {
                dictionary.Add(word, 0);
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
