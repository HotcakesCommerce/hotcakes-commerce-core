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

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Represents an object tracked by the commerce search index.
    /// </summary>
    [Serializable]
    public class SearchObject
    {
        /// <summary>
        /// Initializes a search object with compatibility defaults, including a 1900-01-01 UTC index timestamp.
        /// </summary>
        public SearchObject()
        {
            Id = 0;
            ObjectType = 0;
            ObjectId = Guid.Empty;
            SiteId = 0;
            Title = string.Empty;
            LastIndexUtc = new DateTime(1900, 1, 1);
        }

        /// <summary>
        /// Gets or sets the persistent search-object identifier.
        /// </summary>
        public long Id { get; set; }
        /// <summary>
        /// Gets or sets the application-defined object type value.
        /// </summary>
        public int ObjectType { get; set; }
        /// <summary>
        /// Gets or sets the identifier of the indexed object.
        /// </summary>
        public Guid ObjectId { get; set; }
        /// <summary>
        /// Gets or sets the site identifier that owns the indexed object.
        /// </summary>
        public long SiteId { get; set; }
        /// <summary>
        /// Gets or sets the title stored for the indexed object.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the UTC date and time when the object was last indexed.
        /// </summary>
        public DateTime LastIndexUtc { get; set; }
    }
}
