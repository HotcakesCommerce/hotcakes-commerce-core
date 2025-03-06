#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     This is used to view the individual review information
    /// </summary>
    [Serializable]
    public class ProductReviewViewModel
    {
        /// <summary>
        ///     set default values
        /// </summary>
        /// <param name="productReview">Product review information object. Mapped to the database table</param>
        /// <remarks>This generally called when needs to copy review on product detail page</remarks>
        public ProductReviewViewModel(ProductReview productReview)
        {
            ProductReview = productReview;
        }

        /// <summary>
        ///     Product review information. More details can be found at <see cref="ProductReview" />
        /// </summary>
        public ProductReview ProductReview { get; set; }

        /// <summary>
        ///     Unique user identifier who has added the review.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        ///     Name of the user who has added the review
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        ///     City of the user who has added the review
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     State of the user who has added the review
        /// </summary>
        public string State { get; set; }
    }
}