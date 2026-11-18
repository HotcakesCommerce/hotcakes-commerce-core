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

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Describes one token and the text that replaces it in an HTML template.
    /// </summary>
    public class HtmlTemplateTag
    {
        /// <summary>
        /// Initializes an empty tag with empty <see cref="Tag"/> and <see cref="Replacement"/> values.
        /// </summary>
        public HtmlTemplateTag()
        {
            Tag = string.Empty;
            Replacement = string.Empty;
            IsObsolete = false;
        }

        /// <summary>
        /// Initializes a tag with the specified token, replacement text, and obsolete-token flag.
        /// </summary>
        /// <param name="tagValue">The token to find in template text.</param>
        /// <param name="replacementValue">The text that replaces each occurrence of <paramref name="tagValue"/>.</param>
        /// <param name="isObsoleteValue">Whether the token is obsolete and can be hidden from the available-token list when editing email templates.</param>
        public HtmlTemplateTag(string tagValue, string replacementValue, bool isObsoleteValue = false)
        {
            Tag = tagValue;
            Replacement = replacementValue;
            IsObsolete = isObsoleteValue;
        }

        /// <summary>
        /// Gets or sets the token to find in template text.
        /// </summary>
        public string Tag { get; set; }

        /// <summary>
        /// Gets or sets the text used to replace each occurrence of <see cref="Tag"/>.
        /// </summary>
        public string Replacement { get; set; }

        // This flag can be used to hide the Obsolete tokens from the
        // available tokens list during the edit email templates
        /// <summary>
        /// Gets or sets a value indicating whether this token can be hidden from the available-token list when editing email templates.
        /// </summary>
        public bool IsObsolete { get; set; }

        /// <summary>
        /// Replaces every occurrence of <see cref="Tag"/> in the supplied text with <see cref="Replacement"/>.
        /// </summary>
        /// <param name="input">The template text in which the token is replaced.</param>
        /// <returns>The resulting text after all occurrences are replaced.</returns>
        /// <remarks>
        /// Replacement uses the behavior of <see cref="string.Replace(string, string)"/> and is case-sensitive.
        /// The <see cref="IsObsolete"/> flag does not affect replacement. Null values are not normalized; the underlying
        /// string operation therefore determines the exception behavior for a null input, token, or replacement.
        /// </remarks>
        public string ReplaceTags(string input)
        {
            var result = input;
            result = input.Replace(Tag, Replacement);
            return result;
        }
    }
}
