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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    /// Holds a temporary list of replacement tags for one-off template substitutions.
    /// </summary>
    /// <remarks>
    /// This implementation is used for ad hoc tokens, including tokens supplied by new-password email messages.
    /// The <see cref="Tags"/> list is returned directly by <see cref="GetReplaceableTags(HccRequestContext)"/>.
    /// </remarks>
    public class Replaceable : IReplaceable
    {
        /// <summary>
        /// Initializes an instance with an empty list of replacement tags.
        /// </summary>
        public Replaceable()
        {
            Tags = new List<HtmlTemplateTag>();
        }

        /// <summary>
        /// Initializes an instance containing one replacement tag.
        /// </summary>
        /// <param name="tagName">The token to find in template text.</param>
        /// <param name="replacement">The text that replaces the token.</param>
        public Replaceable(string tagName, string replacement)
        {
            Tags = new List<HtmlTemplateTag>();
            var t = new HtmlTemplateTag {Replacement = replacement, Tag = tagName};
            Tags.Add(t);
        }

        /// <summary>
        /// Gets or sets the replacement tags supplied by this instance.
        /// </summary>
        /// <remarks>
        /// The list is mutable and is returned as-is by <see cref="GetReplaceableTags(HccRequestContext)"/>.
        /// </remarks>
        public List<HtmlTemplateTag> Tags { get; set; }

        /// <summary>
        /// Gets the replacement tags held by this instance.
        /// </summary>
        /// <param name="context">The request context for the email operation. This implementation does not read the parameter.</param>
        /// <returns>The current <see cref="Tags"/> list, not a copy.</returns>
        /// <remarks>
        /// The context parameter is accepted to implement <see cref="IReplaceable"/> but is ignored by this class.
        /// </remarks>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            return Tags;
        }
    }
}
