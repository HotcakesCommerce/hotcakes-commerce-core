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
using System.Collections.ObjectModel;

namespace Hotcakes.Commerce.Catalog
{
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

        public string Keyword
        {
            get { return _Keyword; }
            set { _Keyword = value; }
        }

        public string ManufacturerId
        {
            get { return _ManufacturerId; }
            set { _ManufacturerId = value; }
        }

        public string VendorId
        {
            get { return _VendorId; }
            set { _VendorId = value; }
        }

        public string CategoryId
        {
            get { return _CategoryId; }
            set { _CategoryId = value; }
        }

        public decimal MinPrice
        {
            get { return _MinPrice; }
            set { _MinPrice = value; }
        }

        public decimal MaxPrice
        {
            get { return _MaxPrice; }
            set { _MaxPrice = value; }
        }

        public ProductSearchCriteriaSortOrder SortOrder
        {
            get { return _sortOrder; }
            set { _sortOrder = value; }
        }

        public ProductStoreSearchSortBy SortBy
        {
            get { return _sortBy; }
            set { _sortBy = value; }
        }

        public Collection<CustomPropertyValue> CustomProperties
        {
            get { return _CustomProperties; }
            set { _CustomProperties = value; }
        }

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

            public string PropertyBvin
            {
                get { return _PropertyBvin; }
                set { _PropertyBvin = value; }
            }

            public string PropertyValue
            {
                get { return _PropertyValue; }
                set { _PropertyValue = value; }
            }

            public CustomPropertyValue Clone()
            {
                var result = new CustomPropertyValue(PropertyBvin, PropertyValue);
                return result;
            }
        }
    }
}