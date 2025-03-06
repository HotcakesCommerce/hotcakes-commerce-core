#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Linq;
using System.Text;

namespace Hotcakes.Web.Search
{
    [Serializable]
    public class TextParser
    {
        public static List<string> ParseText(string fullQuery, string culture)
        {
            return ParseText(fullQuery, culture, true, 10);
        }

        public static List<string> ParseText(string fullQuery, string culture, bool removeStopWords, int limit)
        {
            var result = new List<string>();

            var charsRemoved = ReplaceNonAlphaNumeric(fullQuery);

            var working = charsRemoved.Trim().ToCharArray();

            var parsingword = false;
            var word = string.Empty;
            for (var i = 0; i < working.Length; i++)
            {
                if (parsingword)
                {
                    if (working[i] == ' ')
                    {
                        ProcessWord(result, word, culture, removeStopWords);
                        word = string.Empty;
                        parsingword = false;
                    }
                    else
                    {
                        word += working[i];
                    }
                }
                else
                {
                    if (working[i] != ' ')
                    {
                        parsingword = true;
                        word += working[i];
                    }
                }
            }

            if (word.Length > 0)
            {
                ProcessWord(result, word, culture, removeStopWords);
            }

            if (result.Count > limit)
            {
                result = result.Take(limit).ToList();
            }

            return result;
        }

        private static void ProcessWord(List<string> result, string word, string culture, bool removeStopWords)
        {
            var stemmedWord = WordProcessor.StemSingleWord(word, culture);
            if (removeStopWords)
            {
                if (!WordProcessor.IsStopWord(stemmedWord, culture))
                {
                    result.Add(stemmedWord);
                }
            }
            else
            {
                result.Add(stemmedWord);
            }
        }

        private static string ReplaceNonAlphaNumeric(string input)
        {
            if (input == null)
                return string.Empty;
            var working = input.Trim().ToLowerInvariant().ToCharArray();
            for (var i = 0; i < working.Length; i++)
            {
                if (!char.IsLetterOrDigit(working[i]) && !(working[i] == '-'))
                {
                    working[i] = ' ';
                }
            }

            var sb = new StringBuilder();
            sb.Append(working);
            return sb.ToString();
        }
    }
}