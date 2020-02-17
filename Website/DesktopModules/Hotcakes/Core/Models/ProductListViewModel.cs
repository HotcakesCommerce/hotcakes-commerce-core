#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     List of product. This is used to show the
    ///     last viewed products
    /// </summary>
    [Serializable]
    public class ProductListViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public ProductListViewModel()
        {
            Title = string.Empty;
            PagerData = new PagerViewModel();
            Items = new List<Product>();
        }

        /// <summary>
        ///     Title shown for this view
        /// </summary>
        public string Title { get; set; }

        /// <summary>
        ///     Paging control for the view. More detail for the pager can be found at <see cref="PagerViewModel" />
        /// </summary>
        public PagerViewModel PagerData { get; set; }

        /// <summary>
        ///     List of products which has been last viewed
        /// </summary>
        public List<Product> Items { get; set; }
    }
}