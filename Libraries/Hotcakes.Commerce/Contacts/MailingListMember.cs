#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Content;

namespace Hotcakes.Commerce.Contacts
{
    public class MailingListMember : IReplaceable
    {
        public MailingListMember()
        {
            Id = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            ListId = 0;
            EmailAddress = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            StoreId = 0;
        }

        public long Id { get; set; }
        public long ListId { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string EmailAddress { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public long StoreId { get; set; }

        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();

            result.Add(new HtmlTemplateTag("[[MailingListMember.EmailAddress]]", EmailAddress));
            result.Add(new HtmlTemplateTag("[[MailingListMember.FirstName]]", FirstName));
            result.Add(new HtmlTemplateTag("[[MailingListMember.LastName]]", LastName));

            return result;
        }
    }
}