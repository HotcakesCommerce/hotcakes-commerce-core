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
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Describes the filters, price range, visibility options, and sort order for an advanced product search.
    /// </summary>
    /// <remarks>
    /// Collection properties are initialized by the constructor. The obsolete <see cref="Manufactures"/>
    /// alias remains mapped to <see cref="Manufacturers"/> for serialized and source compatibility.
    /// </remarks>
    [Serializable]
    public class ProductSearchQueryAdv
    {
        /// <summary>
        /// Initializes a query with empty facet collections and searchable-product filtering enabled.
        /// </summary>
        public ProductSearchQueryAdv()
        {
            Types = new List<string>();
            Manufacturers = new List<string>();
            Vendors = new List<string>();
            Categories = new List<string>();

            Properties = new Dictionary<long, string[]>();

            IsConsiderSearchable = true;
            IsSearchable = true;
        }

        /// <summary>
        /// Gets or sets category bvins that every result must belong to.
        /// </summary>
        public List<string> Categories { get; set; }
        /// <summary>
        /// Gets or sets product-type bvins used to filter results.
        /// </summary>
        public List<string> Types { get; set; }
        /// <summary>
        /// Gets or sets manufacturer bvins through the misspelled legacy property name.
        /// </summary>
        /// <remarks>This alias reads and writes <see cref="Manufacturers"/>.</remarks>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<string> Manufactures
        {
            get { return Manufacturers; }
            set { Manufacturers = value; }
        }
        /// <summary>
        /// Gets or sets manufacturer bvins used to filter results.
        /// </summary>
        public List<string> Manufacturers { get; set; }
        /// <summary>
        /// Gets or sets vendor bvins used to filter results.
        /// </summary>
        public List<string> Vendors { get; set; }
        /// <summary>
        /// Gets or sets product-property filters keyed by property identifier and containing accepted values.
        /// </summary>
        public Dictionary<long, string[]> Properties { get; set; }

        /// <summary>
        /// Gets or sets the inclusive minimum site price, when specified.
        /// </summary>
        public decimal? MinPrice { get; set; }
        /// <summary>
        /// Gets or sets the inclusive maximum site price, when specified.
        /// </summary>
        public decimal? MaxPrice { get; set; }

        /// <summary>
        /// Gets or sets whether <see cref="IsSearchable"/> is applied as a product filter.
        /// </summary>
        public bool IsConsiderSearchable { get; set; }
        /// <summary>
        /// Gets or sets the searchable flag required when <see cref="IsConsiderSearchable"/> is true.
        /// </summary>
        public bool IsSearchable { get; set; }

        /// <summary>
        /// Gets or sets the ordering applied to the filtered product identifiers.
        /// </summary>
        public CategorySortOrder SortOrder { get; set; }
    }
}
