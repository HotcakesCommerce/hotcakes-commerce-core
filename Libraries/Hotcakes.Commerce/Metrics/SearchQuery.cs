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

namespace Hotcakes.Commerce.Metrics
{
    /// <summary>
    /// Represents a shopper's search phrase recorded for a commerce store.
    /// </summary>
    /// <remarks>
    /// The <see cref="ShopperID"/> property casing is part of the model's compatibility-sensitive
    /// public API and is intentionally preserved. A newly constructed instance starts with an empty
    /// identifier and phrase, a zero store identifier, and a UTC <see cref="LastUpdated"/> value.
    /// </remarks>
    public class SearchQuery
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchQuery"/> class with default values.
        /// </summary>
        public SearchQuery()
        {
            clear();
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchQuery"/> class for a shopper and phrase.
        /// </summary>
        /// <param name="userId">The shopper identifier to assign to <see cref="ShopperID"/>.</param>
        /// <param name="searchPhrase">The search phrase to assign to <see cref="QueryPhrase"/>.</param>
        public SearchQuery(string userId, string searchPhrase)
        {
            clear();
            ShopperID = userId;
            QueryPhrase = searchPhrase;
        }

        /// <summary>
        /// Gets or sets the unique identifier of this search-query record.
        /// </summary>
        /// <value>The record identifier, or an empty string before repository creation assigns one.</value>
        public string Bvin { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the store that owns this record.
        /// </summary>
        /// <value>The commerce store identifier; repository creation replaces it with the current store.</value>
        public long StoreId { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when this record was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the shopper identifier associated with the search.
        /// </summary>
        /// <value>The shopper identifier. The public member name is intentionally <c>ShopperID</c>.</value>
        public string ShopperID { get; set; }

        /// <summary>
        /// Gets or sets the phrase entered by the shopper.
        /// </summary>
        public string QueryPhrase { get; set; }

        private void clear()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            ShopperID = string.Empty;
            QueryPhrase = string.Empty;
        }
    }
}
