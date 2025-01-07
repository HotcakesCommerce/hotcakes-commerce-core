#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Text.RegularExpressions;
using System.Web;

namespace Hotcakes.Web
{
    [Serializable]
    public class Text
    {
        public static string PlaceholderText()
        {
            return PlaceholderText(1024);
        }

        public static string PlaceholderText(int maxLength)
        {
            return
                TrimToLength(
                    "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    maxLength);
        }

        public static string ForceAlphaNumericOnly(string input)
        {
            return Regex.Replace(input, @"[^a-zA-Z0-9]", string.Empty);
        }

        /// <summary>
        ///     Pads a given string to a specific length with a given character
        /// </summary>
        /// <param name="sourceString">String to pad</param>
        /// <param name="maxLength">Maximum length of padded string</param>
        /// <param name="padCharacter">Character to append to string less than the max length.</param>
        /// <returns></returns>
        public static string PadString(string sourceString, int maxLength, string padCharacter)
        {
            var result = string.Empty;

            if (sourceString.Length > maxLength)
            {
                result = sourceString.Substring(0, maxLength);
            }
            else
            {
                result = sourceString;
                while (result.Length < maxLength)
                {
                    result += padCharacter;
                }
            }

            return result;
        }

        public static string PadStringLeft(string sourceString, int maxLength, string padCharacter)
        {
            var result = string.Empty;

            if (sourceString.Length > maxLength)
            {
                result = sourceString.Substring(0, maxLength);
            }
            else
            {
                result = sourceString;
                while (result.Length < maxLength)
                {
                    result = padCharacter + result;
                }
            }

            return result;
        }

        public static string TrimToLength(string input, int maxLength)
        {
            if (input == null)
                return input;

            if (input.Length > maxLength)
            {
                return input.Substring(0, maxLength);
            }
            return input;
        }

        public static string ConvertLinefeedToBrTag(string input)
        {
            var result = input;

            result = result.Replace(Environment.NewLine, "<br />");
            result = result.Replace(Convert.ToChar(13).ToString(), "<br />");
            result = result.Replace(Convert.ToChar(10).ToString(), "<br />");

            return result;
        }

        public static string CleanFileName(string input)
        {
            var result = input;
            result = result.Replace(" ", "-");
            result = result.Replace("\"", string.Empty);
            result = result.Replace("&", "and");
            result = result.Replace("?", string.Empty);
            result = result.Replace("=", string.Empty);
            result = result.Replace("/", string.Empty);
            result = result.Replace("\\", string.Empty);
            result = result.Replace("%", string.Empty);
            result = result.Replace("#", string.Empty);
            result = result.Replace("*", string.Empty);
            result = result.Replace("!", string.Empty);
            result = result.Replace("$", string.Empty);
            result = result.Replace("+", "-plus-");
            result = result.Replace(",", "-");
            result = result.Replace("@", "-at-");
            result = result.Replace(":", "-");
            result = result.Replace(";", "-");
            result = result.Replace(">", string.Empty);
            result = result.Replace("<", string.Empty);
            result = result.Replace("{", string.Empty);
            result = result.Replace("}", string.Empty);
            result = result.Replace("~", string.Empty);
            result = result.Replace("|", "-");
            result = result.Replace("^", string.Empty);
            result = result.Replace("[", string.Empty);
            result = result.Replace("]", string.Empty);
            result = result.Replace("`", string.Empty);
            result = result.Replace("'", string.Empty);
            result = result.Replace("©", string.Empty);
            result = result.Replace("™", string.Empty);
            result = result.Replace("®", string.Empty);

            return result;
        }

        public static string Slugify(string input)
        {
            return Slugify(input, true);
        }

        public static string Slugify(string input, bool urlEncode)
        {
            return Slugify(input, urlEncode, false);
        }

        public static string Slugify(string input, bool urlEncode, bool allowSlashesAndPeriods)
        {
            var result = input.Trim().ToLower().Replace(' ', '-');

            result = result.Replace(" ", "-");
            result = result.Replace("\"", string.Empty);
            result = result.Replace("&", "and");
            result = result.Replace("?", string.Empty);
            result = result.Replace("=", string.Empty);
            result = result.Replace("(", string.Empty);
            result = result.Replace(")", string.Empty);
            if (!allowSlashesAndPeriods)
            {
                result = result.Replace("/", string.Empty);
                result = result.Replace(".", string.Empty);
            }
            result = result.Replace("\\", string.Empty);
            result = result.Replace("%", string.Empty);
            result = result.Replace("#", string.Empty);
            result = result.Replace("*", string.Empty);
            result = result.Replace("!", string.Empty);
            result = result.Replace("$", string.Empty);
            result = result.Replace("+", "-plus-");
            result = result.Replace(",", "-");
            result = result.Replace("@", "-at-");
            result = result.Replace(":", "-");
            result = result.Replace(";", "-");
            result = result.Replace(">", string.Empty);
            result = result.Replace("<", string.Empty);
            result = result.Replace("{", string.Empty);
            result = result.Replace("}", string.Empty);
            result = result.Replace("~", string.Empty);
            result = result.Replace("|", "-");
            result = result.Replace("^", string.Empty);
            result = result.Replace("[", string.Empty);
            result = result.Replace("]", string.Empty);
            result = result.Replace("`", string.Empty);
            result = result.Replace("'", string.Empty);
            result = result.Replace("©", string.Empty);
            result = result.Replace("™", string.Empty);
            result = result.Replace("®", string.Empty);

            if (urlEncode)
            {
                result = HttpUtility.UrlEncode(result);
            }

            if (allowSlashesAndPeriods)
            {
                result = result.Replace("%2f", "/");
                result = result.Replace("%252f", "/");
            }

            return result;
        }

        public static string RemoveHtmlTags(string input)
        {
            return RemoveHtmlTags(input, string.Empty);
        }

        public static string RemoveHtmlTags(string input, string replacementString)
        {
            if (input == null)
            {
                return string.Empty;
            }

            var output = Regex.Replace(input, @"<(.|\n)*?>", replacementString);

            return output;
        }
    }
}