using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore;

namespace ConsoleForTests
{
    class Program
    {
        static void Main(string[] args)
        {
            List<string> places=new List<string>(){ "west-germany", "usa", "france", "uk", "canada", "japan" };
            SgmParser parser = new SgmParser();
            List<string> stopList = File.ReadAllLines("../../../Data/stopList.txt").ToList();
            List<ArticleData> models = parser.ReadAllSgmFromDirectory().Where(n => n.Tags.ContainsKey("places")).ToList();
            List<ArticleData> modelsWithOnePlace = models.Where(n => n.Tags["places"].Count == 1 && places.Contains(n.Tags["places"][0])).ToList();
            List<KnnArticle> articles = modelsWithOnePlace.Select(t => new KnnArticle(t, t.Tags["places"][0], stopList)).Take(8000)
                .ToList();
            FeaturesExtractor extractor=new FeaturesExtractor(articles);
            List<(string,string)> words05andhigher=new List<(string,string)>();
            foreach (KnnArticle knnArticle in articles)
            {
               var test= extractor.CalculateTFIDF(knnArticle);//.OrderByDescending(t=>t.Value);
                words05andhigher.AddRange(test.Where(e => e.Value > 1).Select(t => (t.Key,knnArticle.Label)).ToList());
            }

            var hehe = words05andhigher.Distinct().ToList();

        }
    }
}
