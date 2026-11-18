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
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Areas.ContentBlocks.Models
{
    /// <summary>
    /// Represents the data displayed by the featured-products content block.
    /// </summary>
    [Serializable]
    public class FeaturedProductsViewModel
    {
        /// <summary>
        /// Represents the data displayed by the featured-products content block.
        /// </summary>
        public FeaturedProductsViewModel()
        {
            Title = string.Empty;
            PagerData = new PagerViewModel();
            Items = new List<SingleProductViewModel>();
        }

        /// <summary>
        /// Gets or sets the content block title.
        /// </summary>
        public string Title { get; set; }
        /// <summary>
        /// Gets or sets the paging data for the content block.
        /// See <see cref="PagerViewModel"/> for the paging contract.
        /// </summary>
        public PagerViewModel PagerData { get; set; }
        /// <summary>
        /// Gets or sets the products displayed by the content block.
        /// Each item is a <see cref="SingleProductViewModel"/>.
        /// </summary>
        public List<SingleProductViewModel> Items { get; set; }
    }
}
