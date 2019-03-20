using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Iveonik.Stemmers;

namespace ArticlesClassifactionCore
{
    public class TextPreProcessing
    {
        public static void RemoveStopList(ref List<string> words, List<string> stopList)
        {
            words = words.Where(e => !stopList.Contains(e)).ToList();
        }

        public static void RemoveNonLetterCharacters(ref List<string> words)
        {
            words = words.Where(e => !string.IsNullOrEmpty(e)).ToList();
        }

        public static void StemWords(ref List<string> words)
        {
            EnglishStemmer stemmer = new EnglishStemmer();
            words = words.Select(stemmer.Stem).ToList();
        }
       
    }


}
