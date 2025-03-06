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

namespace Hotcakes.Commerce.Content
{
    public class HtmlTemplateTag
    {
        public HtmlTemplateTag()
        {
            Tag = string.Empty;
            Replacement = string.Empty;
            IsObsolete = false;
        }

        public HtmlTemplateTag(string tagValue, string replacementValue, bool isObsoleteValue = false)
        {
            Tag = tagValue;
            Replacement = replacementValue;
            IsObsolete = isObsoleteValue;
        }

        public string Tag { get; set; }
        public string Replacement { get; set; }

        // This flag can be used to hide the Obsolete tokens from the
        // available tokens list during the edit email templates
        public bool IsObsolete { get; set; }

        public string ReplaceTags(string input)
        {
            var result = input;
            result = input.Replace(Tag, Replacement);
            return result;
        }
    }
}