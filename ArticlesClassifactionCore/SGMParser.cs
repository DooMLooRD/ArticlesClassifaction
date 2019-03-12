using System;
using System.Collections.Generic;
using System.IO;
using System.Collections.Specialized;
using System.ComponentModel.Composition;
using System.ComponentModel.Composition.Hosting;
using System.Linq;
using HtmlAgilityPack;

namespace ArticlesClassifactionCore
{
    public class SgmParser
    {
        public List<DataModel> ReadAllSgmFromDirectory()
        {
            DirectoryInfo d = new DirectoryInfo("../../../Data");
            FileInfo[] files = d.GetFiles("*.sgm");
            List<string> filePaths = new List<string>();
            foreach (FileInfo file in files)
            {
                filePaths.Add(file.FullName);
            }
            List<DataModel> models = new List<DataModel>();
            foreach (string filePath in filePaths)
            {
                models.AddRange(FromSgml(filePath));
            }

            return models;
        }
        public List<DataModel> FromSgml(string path)
        {
            List<DataModel> models = new List<DataModel>();
            HtmlDocument doc = new HtmlDocument();
            doc.Load(path);

            var pages = doc.DocumentNode.Descendants("REUTERS");

            foreach (var sgmlNode in pages)
            {
                DataModel model = new DataModel();

                var categories = sgmlNode.Descendants("D");

                foreach (var category in categories)
                {
                    model.Tags[category.ParentNode.Name] = category.ChildNodes.Select(n => n.InnerText).ToList();
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