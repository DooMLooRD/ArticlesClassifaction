using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace ArticlesClassifactionCore.Data
{
    public class JsonParser
    {
        public List<ArticleData> FromJson(string path)
        {
            using (StreamReader file = File.OpenText(path))
            {
                JsonSerializer serializer = new JsonSerializer();
                JsonArticleData articleData = (JsonArticleData)serializer.Deserialize(file, typeof(JsonArticleData));
                List<ArticleData> articles = new List<ArticleData>();
                foreach (JsonArticle articleDataArticle in articleData.Articles)
                {
                    articles.Add(new ArticleData() { Tags = new Dictionary<string, List<string>>() { { "league", new List<string>() { articleDataArticle.Tag } } }, Text = new Text() { Body = articleDataArticle.Text } });
                }

                return articles;
            }
        }
    }
}
