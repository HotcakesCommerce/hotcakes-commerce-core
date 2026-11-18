#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>
    /// Provides helpers for generating, normalizing, and formatting text.
    /// </summary>
    [Serializable]
    public class Text
    {
        /// <summary>
        /// Creates placeholder text using the default maximum length of 1024 characters.
        /// </summary>
        /// <returns>Placeholder text truncated to the default maximum length.</returns>
        /// <see cref="PlaceholderText(int)" />
        public static string PlaceholderText()
        {
            return PlaceholderText(1024);
        }

        /// <summary>
        /// Creates placeholder text truncated to the specified maximum length.
        /// </summary>
        /// <param name="maxLength">The maximum number of characters to return.</param>
        /// <returns>Placeholder text no longer than <paramref name="maxLength" /> characters.</returns>
        /// <see cref="TrimToLength(string, int)" />
        public static string PlaceholderText(int maxLength)
        {
            return
                TrimToLength(
                    "Lorem ipsum dolor sit amet, consectetur adipisicing elit, sed do eiusmod tempor incididunt ut labore et dolore magna aliqua. Ut enim ad minim veniam, quis nostrud exercitation ullamco laboris nisi ut aliquip ex ea commodo consequat. Duis aute irure dolor in reprehenderit in voluptate velit esse cillum dolore eu fugiat nulla pariatur. Excepteur sint occaecat cupidatat non proident, sunt in culpa qui officia deserunt mollit anim id est laborum.",
                    maxLength);
        }

        /// <summary>
        /// Removes every character from a string except ASCII letters and digits.
        /// </summary>
        /// <param name="input">The text to filter.</param>
        /// <returns>The input containing only ASCII letters and digits.</returns>
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
        /// <returns>The padded or truncated string.</returns>
        /// <see cref="PadStringLeft(string, int, string)" />
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

        /// <summary>
        /// Pads a string on the left until it reaches the specified maximum length, or truncates it when it is longer.
        /// </summary>
        /// <param name="sourceString">The string to pad or truncate.</param>
        /// <param name="maxLength">The maximum length of the result.</param>
        /// <param name="padCharacter">The string to prepend while padding.</param>
        /// <returns>The padded or truncated string.</returns>
        /// <see cref="PadString(string, int, string)" />
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

        /// <summary>
        /// Truncates text to a maximum length without changing shorter text.
        /// </summary>
        /// <param name="input">The text to truncate.</param>
        /// <param name="maxLength">The maximum number of characters.</param>
        /// <returns>The original text, or its leading characters up to <paramref name="maxLength" />.</returns>
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

        /// <summary>
        /// Replaces line-feed and carriage-return characters with HTML line-break elements.
        /// </summary>
        /// <param name="input">The text containing line breaks.</param>
        /// <returns>The text with line breaks represented by <c>&lt;br /&gt;</c>.</returns>
        public static string ConvertLinefeedToBrTag(string input)
        {
            var result = input;

            result = result.Replace(Environment.NewLine, "<br />");
            result = result.Replace(Convert.ToChar(13).ToString(), "<br />");
            result = result.Replace(Convert.ToChar(10).ToString(), "<br />");

            return result;
        }

        /// <summary>
        /// Removes or substitutes characters that are unsuitable for a file name.
        /// </summary>
        /// <param name="input">The file name text to clean.</param>
        /// <returns>A cleaned file name string.</returns>
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

        /// <summary>
        /// Creates a URL slug using URL encoding and excluding slashes and periods.
        /// </summary>
        /// <param name="input">The text to convert.</param>
        /// <returns>A URL-friendly slug.</returns>
        /// <see cref="Slugify(string, bool)" />
        public static string Slugify(string input)
        {
            return Slugify(input, true);
        }

        /// <summary>
        /// Creates a URL slug with configurable URL encoding and excluding slashes and periods.
        /// </summary>
        /// <param name="input">The text to convert.</param>
        /// <param name="urlEncode"><see langword="true" /> to URL-encode the result; otherwise, <see langword="false" />.</param>
        /// <returns>A URL-friendly slug.</returns>
        /// <see cref="Slugify(string, bool, bool)" />
        public static string Slugify(string input, bool urlEncode)
        {
            return Slugify(input, urlEncode, false);
        }

        /// <summary>
        /// Creates a URL slug with configurable URL encoding and preservation of slashes and periods.
        /// </summary>
        /// <param name="input">The text to convert.</param>
        /// <param name="urlEncode"><see langword="true" /> to URL-encode the result; otherwise, <see langword="false" />.</param>
        /// <param name="allowSlashesAndPeriods"><see langword="true" /> to preserve slashes and periods; otherwise, <see langword="false" />.</param>
        /// <returns>A URL-friendly slug.</returns>
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

        /// <summary>
        /// Removes HTML tags from text without inserting replacement text.
        /// </summary>
        /// <param name="input">The text from which to remove tags.</param>
        /// <returns>The text with HTML tags removed.</returns>
        /// <see cref="RemoveHtmlTags(string, string)" />
        public static string RemoveHtmlTags(string input)
        {
            return RemoveHtmlTags(input, string.Empty);
        }

        /// <summary>
        /// Removes HTML tags from text and replaces each tag with the specified string.
        /// </summary>
        /// <param name="input">The text from which to remove tags.</param>
        /// <param name="replacementString">The text to insert for each removed tag.</param>
        /// <returns>The text with HTML tags replaced.</returns>
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
