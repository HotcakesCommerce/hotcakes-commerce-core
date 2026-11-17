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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Represents the lightweight list metadata returned by paged list queries.</summary>
    public class MailingListSnapShot
    {
        /// <summary>Initializes a snapshot with default identifiers, timestamp, and visibility.</summary>
        public MailingListSnapShot()
        {
            Id = 0;
            Name = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            IsPrivate = false;
            StoreId = 0;
        }

        /// <summary>Gets or sets the list identifier.</summary>
        public long Id { get; set; }
        /// <summary>Gets or sets the list display name.</summary>
        public string Name { get; set; }
        /// <summary>Gets or sets the UTC timestamp of the last update.</summary>
        public DateTime LastUpdatedUtc { get; set; }
        /// <summary>Gets or sets whether the represented list is private.</summary>
        public bool IsPrivate { get; set; }
        /// <summary>Gets or sets the identifier of the owning store.</summary>
        public long StoreId { get; set; }
    }
}
