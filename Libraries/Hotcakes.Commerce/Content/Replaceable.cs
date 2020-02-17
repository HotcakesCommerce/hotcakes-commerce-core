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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Content
{
    /// <summary>
    ///     Holds a temporary list of replacement tags
    ///     Allows you to generate "one off" tags or lists as needed
    ///     Used by the "new password" email messages
    /// </summary>
    public class Replaceable : IReplaceable
    {
        public Replaceable()
        {
            Tags = new List<HtmlTemplateTag>();
        }

        public Replaceable(string tagName, string replacement)
        {
            Tags = new List<HtmlTemplateTag>();
            var t = new HtmlTemplateTag {Replacement = replacement, Tag = tagName};
            Tags.Add(t);
        }

        public List<HtmlTemplateTag> Tags { get; set; }

        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            return Tags;
        }
    }
}