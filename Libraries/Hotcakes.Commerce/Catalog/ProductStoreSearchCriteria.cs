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
using System.Collections.ObjectModel;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>Filters and ordering options for a store-scoped product search.</summary>
    [Serializable]
    public class ProductStoreSearchCriteria
    {
        private string _CategoryId = string.Empty;
        private Collection<CustomPropertyValue> _CustomProperties = new Collection<CustomPropertyValue>();

        private string _Keyword = string.Empty;
        private string _ManufacturerId = string.Empty;
        private decimal _MaxPrice = -1m;
        private decimal _MinPrice = -1m;
        private ProductStoreSearchSortBy _sortBy = ProductStoreSearchSortBy.NotSet;
        private ProductSearchCriteriaSortOrder _sortOrder = ProductSearchCriteriaSortOrder.NotSet;
        private string _VendorId = string.Empty;

        /// <summary>Gets or sets the Keyword value.</summary>
        public string Keyword
        {
            get { return _Keyword; }
            set { _Keyword = value; }
        }

        /// <summary>Gets or sets the ManufacturerId value.</summary>
        public string ManufacturerId
        {
            get { return _ManufacturerId; }
            set { _ManufacturerId = value; }
        }

        /// <summary>Gets or sets the VendorId value.</summary>
        public string VendorId
        {
            get { return _VendorId; }
            set { _VendorId = value; }
        }

        /// <summary>Gets or sets the CategoryId value.</summary>
        public string CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }

        /// <summary>Gets or sets the MinPrice value.</summary>
        public decimal MinPrice
        {
            get { return _MinPrice; }
            set { _MinPrice = value; }
        }

        /// <summary>Gets or sets the MaxPrice value.</summary>
        public decimal MaxPrice
        {
            get { return _MaxPrice; }
            set { _MaxPrice = value; }
        }

        /// <summary>Gets or sets the SortOrder value.</summary>
        public ProductSearchCriteriaSortOrder SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        /// <summary>Gets or sets the SortBy value.</summary>
        public ProductStoreSearchSortBy SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; }
        }

        /// <summary>Gets or sets the CustomProperties value.</summary>
        public Collection<CustomPropertyValue> CustomProperties
        {
            get { return _CustomProperties; }
            set { _CustomProperties = value; }
        }

        /// <summary>Creates a copy of the current search criteria, including custom property values.</summary>
        /// <returns>The result of this operation.</returns>
        public ProductStoreSearchCriteria Clone()
        {
            var result = new ProductStoreSearchCriteria();

            result.CategoryId = CategoryId;
            foreach (var cpv in CustomProperties)
            {
                var cp = cpv.Clone();
                result.CustomProperties.Add(cp);
            }
            result.Keyword = Keyword;
            result.ManufacturerId = ManufacturerId;
            result.MaxPrice = MaxPrice;
            result.MinPrice = MinPrice;
            result.SortBy = SortBy;
            result.SortOrder = SortOrder;
            result.VendorId = VendorId;

            return result;
        }

        /// <summary>Provides the catalog CustomPropertyValue API.</summary>
        public class CustomPropertyValue
        {
            private string _PropertyBvin = string.Empty;
            private string _PropertyValue = string.Empty;

            public CustomPropertyValue()
            {
            }

            public CustomPropertyValue(string bvin, string value)
            {
                _PropertyBvin = bvin;
                _PropertyValue = value;
            }

            /// <summary>Gets or sets the PropertyBvin value.</summary>
            public string PropertyBvin
            {
                get { return _PropertyBvin; }
                set { _PropertyBvin = value; }
            }

            /// <summary>Gets or sets the PropertyValue value.</summary>
            public string PropertyValue
            {
                get { return _PropertyValue; }
                set { _PropertyValue = value; }
            }

            /// <summary>Creates a copy of the current search criteria, including custom property values.</summary>
            /// <returns>The result of this operation.</returns>
            public CustomPropertyValue Clone()
            {
                var result = new CustomPropertyValue(PropertyBvin, PropertyValue);
                return result;
            }
        }
    }
}
