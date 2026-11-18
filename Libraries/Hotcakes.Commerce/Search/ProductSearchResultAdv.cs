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

namespace Hotcakes.Commerce.Search
{
    /// <summary>
    /// Contains product identifiers, counts, price bounds, and facet data returned by an advanced search.
    /// </summary>
    /// <remarks>Collection properties are initialized by the constructor and may be empty when no matches exist.</remarks>
    [Serializable]
    public class ProductSearchResultAdv
    {
        private List<FacetItem> _Manufacturers;

        private List<SelectedFacetItem> _SelectedManufacturers;

        /// <summary>
        /// Initializes an empty search result with its facet collections ready for population.
        /// </summary>
        public ProductSearchResultAdv()
        {
            Products = new List<string>();
            Manufacturers = new List<FacetItem>();
            Vendors = new List<FacetItem>();
            Types = new List<FacetItem>();
            Categories = new List<FacetItem>();
            Properties = new List<PropertyFacetItem>();
        }

        /// <summary>
        /// Gets or sets the paged product bvins returned by the search.
        /// </summary>
        public List<string> Products { get; set; }

        /// <summary>
        /// Gets or sets the total number of products after all filters and before paging.
        /// </summary>
        public int TotalCount { get; set; }
        
        /// <summary>
        /// Gets or sets manufacturer facets. The backing field is retained for compatibility.
        /// </summary>
        public List<FacetItem> Manufacturers
        {
            get { return _Manufacturers; }
            set { _Manufacturers = value; }
        }

        /// <summary>
        /// Gets or sets vendor facets.
        /// </summary>
        public List<FacetItem> Vendors { get; set; }

        /// <summary>
        /// Gets or sets product-type facets.
        /// </summary>
        public List<FacetItem> Types { get; set; }

        /// <summary>
        /// Gets or sets category facets.
        /// </summary>
        public List<FacetItem> Categories { get; set; }

        /// <summary>
        /// Gets or sets product-property facets and their values.
        /// </summary>
        public List<PropertyFacetItem> Properties { get; set; }

        /// <summary>
        /// Gets or sets the minimum site price among products before price filtering.
        /// </summary>
        public decimal MinPrice { get; set; }

        /// <summary>
        /// Gets or sets the maximum site price among products before price filtering.
        /// </summary>
        public decimal MaxPrice { get; set; }
        
        /// <summary>
        /// Gets or sets the selected manufacturer values.
        /// </summary>
        public List<SelectedFacetItem> SelectedManufacturers
        {
            get { return _SelectedManufacturers; }
            set { _SelectedManufacturers = value; }
        }

        /// <summary>
        /// Gets or sets the selected vendor values.
        /// </summary>
        public List<SelectedFacetItem> SelectedVendors { get; set; }

        /// <summary>
        /// Gets or sets the selected product-type values.
        /// </summary>
        public List<SelectedFacetItem> SelectedTypes { get; set; }

        /// <summary>
        /// Gets or sets the selected category values.
        /// </summary>
        public List<SelectedFacetItem> SelectedCategories { get; set; }

        /// <summary>
        /// Gets or sets the selected product properties and values.
        /// </summary>
        public List<SelectedPropertyFacetItem> SelectedProperties { get; set; }

        /// <summary>
        /// Gets or sets the effective minimum price selected for the query.
        /// </summary>
        public decimal SelectedMinPrice { get; set; }

        /// <summary>
        /// Gets or sets the effective maximum price selected for the query.
        /// </summary>
        public decimal SelectedMaxPrice { get; set; }
    }
}
