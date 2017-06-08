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
using System.Collections.Generic;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Product review information
    /// </summary>
    [Serializable]
    public class ProductReviewsViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public ProductReviewsViewModel()
        {
            ProductView = new SingleProductViewModel();
            Reviews = new List<ProductReviewViewModel>();
        }

        /// <summary>
        ///     Single review information detail. Detailed information for single review can be found at
        ///     <see cref="SingleProductViewModel" />
        /// </summary>
        public SingleProductViewModel ProductView { get; set; }

        /// <summary>
        ///     List of reviews for the product. Detailed information for single review can be found at
        ///     <see cref="SingleProductViewModel" />
        /// </summary>
        public List<ProductReviewViewModel> Reviews { get; set; }
    }
}