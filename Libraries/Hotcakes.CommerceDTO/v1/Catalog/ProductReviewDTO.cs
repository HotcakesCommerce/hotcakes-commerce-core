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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Reviews in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is ProductReview.</remarks>
    [DataContract]
    [Serializable]
    public class ProductReviewDTO
    {
        public ProductReviewDTO()
        {
            Bvin = string.Empty;
            UserID = string.Empty;
            ProductBvin = string.Empty;
            ReviewDateUtc = DateTime.UtcNow;
            Rating = ProductReviewRatingDTO.ThreeStars;
            Karma = 0;
            Description = string.Empty;
            Approved = false;
            ProductName = string.Empty;
        }

        /// <summary>
        ///     This is the ID of the review.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The unique ID of the user account that created the review.
        /// </summary>
        [DataMember]
        public string UserID { get; set; }

        /// <summary>
        ///     The unique ID of the product that this review is written for.
        /// </summary>
        [DataMember]
        public string ProductBvin { get; set; }

        /// <summary>
        ///     The UTC version of the date/time stamp reflecting when the review was submitted.
        /// </summary>
        [DataMember]
        public DateTime ReviewDateUtc { get; set; }

        /// <summary>
        ///     A star rating from 1-5 to reflect how good or bad the purchased product and/or service was.
        /// </summary>
        [DataMember]
        public ProductReviewRatingDTO Rating { get; set; }

        /// <summary>
        ///     Karma is used by merchants to adjust the overall rating of a review.
        /// </summary>
        /// <remarks>This property is currently not being used by the application.</remarks>
        [DataMember]
        public int Karma { get; set; }

        /// <summary>
        ///     Gets or sets the comments from the reviewer that describe their feelings/experience with the product and/or
        ///     service.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     When moderation is enabled, this value will determine if the review will be shown to site visitors.
        /// </summary>
        [DataMember]
        public bool Approved { get; set; }

        /// <summary>
        ///     Gets or sets the name of the product that this review was created for.
        /// </summary>
        /// <remarks>This property is currently not saved in the data source.</remarks>
        [DataMember]
        public string ProductName { get; set; }
    }
}