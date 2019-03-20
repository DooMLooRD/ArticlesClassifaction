using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ArticlesClassifactionCore
{
    public class FeaturesExtractor
    {
        public List<KnnArticle> Articles { get; set; }
        public Dictionary<string,int> DocumentFrequency { get; set; }

        public FeaturesExtractor(List<KnnArticle> articles)
        {
            Articles = articles;
            CalculateDocumentFrequency();
        }

        public Dictionary<string, double> CalculateTFIDF(KnnArticle article)
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
        private Dictionary<string, double> TermFrequency(KnnArticle article)
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

            foreach (KnnArticle knnArticle in Articles)
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
