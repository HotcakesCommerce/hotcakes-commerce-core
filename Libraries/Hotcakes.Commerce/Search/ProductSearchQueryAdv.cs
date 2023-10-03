#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
    [Serializable]
    public class ProductSearchQueryAdv
    {
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

        public List<string> Categories { get; set; }
        public List<string> Types { get; set; }
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the Manufacturers property instead. Removing in version 03.04.00 or later.")]
        public List<string> Manufactures
        {
            get { return Manufacturers; }
            set { Manufacturers = value; }
        }
        public List<string> Manufacturers { get; set; }
        public List<string> Vendors { get; set; }
        public Dictionary<long, string[]> Properties { get; set; }

        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }

        public bool IsConsiderSearchable { get; set; }
        public bool IsSearchable { get; set; }

        public CategorySortOrder SortOrder { get; set; }
    }
}