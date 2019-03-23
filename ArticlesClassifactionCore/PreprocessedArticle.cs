﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ArticlesClassifactionCore.Data;

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
            TextPreProcessing.RemoveNonLetterCharacters(ref words);
            TextPreProcessing.RemoveStopList(ref words, stopList);
            TextPreProcessing.StemWords(ref words);
            Words = words;
        }
    }
}