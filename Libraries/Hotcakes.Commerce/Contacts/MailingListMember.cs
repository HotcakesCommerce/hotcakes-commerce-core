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
using System.Collections.Generic;
using Hotcakes.Commerce.Content;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Represents a member of a mailing list and its template replacement values.</summary>
    public class MailingListMember : IReplaceable
    {
        /// <summary>Initializes a member with empty text values and default identifiers.</summary>
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

        /// <summary>Gets or sets the member identifier.</summary>
        public long Id { get; set; }
        /// <summary>Gets or sets the mailing-list identifier.</summary>
        public long ListId { get; set; }
        /// <summary>Gets or sets the UTC timestamp of the last update.</summary>
        public DateTime LastUpdatedUtc { get; set; }
        /// <summary>Gets or sets the member's email address.</summary>
        public string EmailAddress { get; set; }
        /// <summary>Gets or sets the member's first name.</summary>
        public string FirstName { get; set; }
        /// <summary>Gets or sets the member's last name.</summary>
        public string LastName { get; set; }
        /// <summary>Gets or sets the identifier of the store that owns the member.</summary>
        public long StoreId { get; set; }

        /// <summary>
        /// Gets the template tags for the member's email address, first name, and last name.
        /// </summary>
        /// <param name="context">The request context; retained for the replacement contract.</param>
        /// <returns>The member-specific template tags.</returns>
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
