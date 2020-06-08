#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
    [Serializable]
    public class ProductSearchResultAdv
    {
        private List<FacetItem> _Manufacturers;

        private List<SelectedFacetItem> _SelectedManufacturers;

        public ProductSearchResultAdv()
        {
            Products = new List<string>();
            Manufacturers = new List<FacetItem>();
            Vendors = new List<FacetItem>();
            Types = new List<FacetItem>();
            Categories = new List<FacetItem>();
            Properties = new List<PropertyFacetItem>();
        }

        public List<string> Products { get; set; }

        public int TotalCount { get; set; }
        
        public List<FacetItem> Manufacturers
        {
            get { return _Manufacturers; }
            set { _Manufacturers = value; }
        }

        public List<FacetItem> Vendors { get; set; }

        public List<FacetItem> Types { get; set; }

        public List<FacetItem> Categories { get; set; }

        public List<PropertyFacetItem> Properties { get; set; }

        public decimal MinPrice { get; set; }

        public decimal MaxPrice { get; set; }
        
        public List<SelectedFacetItem> SelectedManufacturers
        {
            get { return _SelectedManufacturers; }
            set { _SelectedManufacturers = value; }
        }

        public List<SelectedFacetItem> SelectedVendors { get; set; }

        public List<SelectedFacetItem> SelectedTypes { get; set; }

        public List<SelectedFacetItem> SelectedCategories { get; set; }

        public List<SelectedPropertyFacetItem> SelectedProperties { get; set; }

        public decimal SelectedMinPrice { get; set; }

        public decimal SelectedMaxPrice { get; set; }
    }
}