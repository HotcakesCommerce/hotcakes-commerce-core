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
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Reviews in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is ProductReviewDTO.</remarks>
    [Serializable]
    public class ProductReview
    {
        public ProductReview()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            UserID = string.Empty;
            ProductBvin = string.Empty;
            ReviewDateUtc = DateTime.UtcNow;
            Rating = ProductReviewRating.ThreeStars;
            Karma = 0;
            Description = string.Empty;
            Approved = false;
            ProductName = string.Empty;
        }

        /// <summary>
        ///     This is the ID of the review.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the review was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The unique ID of the user account that created the review.
        /// </summary>
        public string UserID { get; set; }

        /// <summary>
        ///     The unique ID of the product that this review is written for.
        /// </summary>
        public string ProductBvin { get; set; }

        /// <summary>
        ///     The UTC version of the date/time stamp reflecting when the review was submitted.
        /// </summary>
        public DateTime ReviewDateUtc { get; set; }

        /// <summary>
        ///     A star rating from 1-5 to reflect how good or bad the purchased product and/or service was.
        /// </summary>
        public ProductReviewRating Rating { get; set; }

        /// <summary>
        ///     A conversion of Rating to a numeric value, used for mathematical functions.
        /// </summary>
        public int RatingAsInteger
        {
            get { return (int) Rating; }
        }

        /// <summary>
        ///     Karma is used by merchants to adjust the overall rating of a review.
        /// </summary>
        /// <remarks>This property is currently not being used by the application.</remarks>
        public int Karma { get; set; }

        /// <summary>
        ///     Gets or sets the comments from the reviewer that describe their feelings/experience with the product and/or
        ///     service.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     When moderation is enabled, this value will determine if the review will be shown to site visitors.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        ///     Gets or sets the name of the product that this review was created for.
        /// </summary>
        /// <remarks>This property is currently not saved in the data source.</remarks>
        public string ProductName { get; set; }

        /// <summary>
        ///     A time zone adjusted date/time stamp of the review date in the local time of the current user.
        /// </summary>
        /// <param name="tz">TimeZoneInfo - an object describing the timezone of the user.</param>
        /// <returns>DateTime - an adjusted date/time stamp for the current user in their time zone.</returns>
        public DateTime ReviewDateForTimeZone(TimeZoneInfo tz)
        {
            return TimeZoneInfo.ConvertTimeFromUtc(ReviewDateUtc, tz);
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product review object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductReviewDTO</returns>
        public ProductReviewDTO ToDto()
        {
            var dto = new ProductReviewDTO();

            dto.Approved = Approved;
            dto.Bvin = Bvin;
            dto.Description = Description;
            dto.Karma = Karma;
            dto.ProductBvin = ProductBvin;
            dto.ProductName = ProductName;
            dto.Rating = (ProductReviewRatingDTO) (int) Rating;
            dto.ReviewDateUtc = ReviewDateUtc;
            dto.UserID = UserID;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product review object using a ProductReviewDTO instance
        /// </summary>
        /// <param name="dto">An instance of the product review from the REST API</param>
        public void FromDto(ProductReviewDTO dto)
        {
            if (dto == null) return;

            Approved = dto.Approved;
            Bvin = dto.Bvin;
            Description = dto.Description;
            Karma = dto.Karma;
            ProductBvin = dto.ProductBvin;
            ProductName = dto.ProductName;
            Rating = (ProductReviewRating) (int) dto.Rating;
            ReviewDateUtc = dto.ReviewDateUtc;
            UserID = dto.UserID;
        }

        #endregion
    }
}