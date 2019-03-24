using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.Data;
using ArticlesClassifactionCore.Preprocessing;

namespace ArticlesClassifactionCore
{
    public class PreprocessedArticle
    {
        public string Label { get; set; }
        public List<string> Words { get; set; }

        public PreprocessedArticle(ArticleData data, string tag, List<string> stopList)
        {
            Label = tag;
            PreProcessText(data,stopList);
        }
        private void PreProcessText(ArticleData data,List<string> stopList)
        {
            List<string> words = data.Text.GetWordsFromBody();
            if (words == null)
                return;
            PreprocessingService.RemoveNonLetterCharacters(ref words);
            PreprocessingService.RemoveStopList(ref words, stopList);
            PreprocessingService.StemWords(ref words);
            Words = words;
        }
    }
}
