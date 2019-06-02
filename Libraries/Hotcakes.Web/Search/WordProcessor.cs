#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// 
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software 
// and associated documentation files (the "Software"), to deal in the Software without restriction, 
// including without limitation the rights to use, copy, modify, merge, publish, distribute, 
// sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is 
// furnished to do so, subject to the following conditions:
// 
// The above copyright notice and this permission notice shall be included in all copies or 
// substantial portions of the Software.
// 
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Collections.Generic;
using System.Globalization;

namespace Hotcakes.Web.Search
{
    [Serializable]
    public static class WordProcessor
    {
        static WordProcessor()
        {
            EnglishStopWords = new List<string>
            {
                "i",
                "a",
                "about",
                "an",
                "and",
                "are",
                "as",
                "at",
                "be",
                "by",
                "com",
                "de",
                "en",
                "for",
                "from",
                "how",
                "in",
                "is",
                "it",
                "la",
                "of",
                "on",
                "or",
                "that",
                "the",
                "thi",
                "to",
                "was",
                "what",
                "when",
                "where",
                "who",
                "will",
                "with",
                "und",
                "www"
            };
        }

        private static List<string> EnglishStopWords { get; set; }

        public static bool IsEnglish(string culture)
        {
            var cultureInfo = new CultureInfo(culture);
            return cultureInfo.TwoLetterISOLanguageName == "en";
        }

        public static string StemSingleWord(string word, string culture)
        {
            if (IsEnglish(culture))
            {
                var stemmer = new PorterStemmer();
                var parts = word.Trim().ToLowerInvariant().ToCharArray();
                stemmer.add(parts, parts.Length);
                stemmer.stem();
                return stemmer.ToString();
            }
            return word;
        }

        public static bool IsStopWord(string stemmedWord, string culture)
        {
            if (IsEnglish(culture))
            {
                return EnglishStopWords.IndexOf(stemmedWord) > -1;
            }
            return false;
        }
    }
}