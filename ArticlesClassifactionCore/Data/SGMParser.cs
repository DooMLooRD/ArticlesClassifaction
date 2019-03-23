using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using HtmlAgilityPack;

namespace ArticlesClassifactionCore.Data
{
    public class SgmParser
    {
        public List<ArticleData> ReadAllSgmFromDirectory()
        {
            DirectoryInfo d = new DirectoryInfo("../../../Data");
            FileInfo[] files = d.GetFiles("*.sgm");
            List<string> filePaths = new List<string>();
            foreach (FileInfo file in files)
            {
                filePaths.Add(file.FullName);
            }
            List<ArticleData> models = new List<ArticleData>();
            foreach (string filePath in filePaths)
            {
                models.AddRange(FromSgml(filePath));
            }

            return models;
        }
        public List<ArticleData> FromSgml(string path)
        {
            List<ArticleData> models = new List<ArticleData>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            var pages = doc.DocumentNode.Descendants("REUTERS");

            foreach (var sgmlNode in pages)
            {
                ArticleData model = new ArticleData();

                var categories = sgmlNode.Descendants("D").Select(t=>t.ParentNode).Distinct();

                foreach (var category in categories)
                {
                    model.Tags[category.Name] = category.ChildNodes.Select(n => n.InnerText).ToList();
                }

                string date = sgmlNode.Descendants("DATE")?.FirstOrDefault()?.InnerText;
                if (date != null)
                {
                    DateTime.TryParse(date, out DateTime outDate);
                    model.Date = outDate;
                }
                model.Text.Author = sgmlNode.Descendants("AUTHOR").FirstOrDefault()?.InnerText;
                model.Unknown= sgmlNode.Descendants("UNKNOWN").FirstOrDefault()?.InnerText;
                model.Text.Title = sgmlNode.Descendants("TITLE").FirstOrDefault()?.InnerText;
                model.Text.Body = sgmlNode.Descendants("BODY").FirstOrDefault()?.InnerText;
                model.Text.Dateline = sgmlNode.Descendants("DATELINE").FirstOrDefault()?.InnerText;

                models.Add(model);
            }
            return models;
        }
    }
}