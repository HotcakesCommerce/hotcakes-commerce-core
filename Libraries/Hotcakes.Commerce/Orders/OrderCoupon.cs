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
using Hotcakes.CommerceDTO.v1.Orders;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order Coupon.
    /// </summary>
    /// <remarks>The REST API equivalent is OrderCouponDTO.</remarks>
    [Serializable]
    public class OrderCoupon : IEquatable<OrderCoupon>
    {
        public OrderCoupon()
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            OrderBvin = string.Empty;
            CouponCode = string.Empty;
            IsUsed = false;
            UserId = string.Empty;
        }

        /// <summary>
        ///     The unique ID of the order coupon.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the order coupon was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the order that this order coupon belongs to.
        /// </summary>
        public string OrderBvin { get; set; }

        /// <summary>
        ///     The coupon code for the current order coupon.
        /// </summary>
        public string CouponCode { get; set; }

        /// <summary>
        ///     Defines whether or not the current order coupon has been used.
        /// </summary>
        public bool IsUsed { get; set; }

        /// <summary>
        ///     Records the user account ID that is using the coupon.
        /// </summary>
        public string UserId { get; set; }

        /// <summary>
        ///     Compares the given order coupon ID with the current order coupon ID to determine if they are the same coupon.
        /// </summary>
        /// <param name="other">OrderCoupon - a populated instance of the order coupon object.</param>
        /// <returns>Boolean - if true, the order coupon ID's match.</returns>
        bool IEquatable<OrderCoupon>.Equals(OrderCoupon other)
        {
            return Id == other.Id;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current order coupon object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderCouponDTO</returns>
        public OrderCouponDTO ToDto()
        {
            var dto = new OrderCouponDTO
            {
                Id = Id,
                StoreId = StoreId,
                LastUpdatedUtc = LastUpdatedUtc,
                OrderBvin = OrderBvin,
                CouponCode = CouponCode,
                IsUsed = IsUsed,
                UserId = UserId
            };

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current order coupon object using a OrderCouponDTO instance
        /// </summary>
        /// <param name="dto">An instance of the order coupon from the REST API</param>
        public void FromDto(OrderCouponDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            StoreId = dto.StoreId;
            LastUpdatedUtc = dto.LastUpdatedUtc;
            OrderBvin = dto.OrderBvin ?? string.Empty;
            CouponCode = dto.CouponCode ?? string.Empty;
            IsUsed = dto.IsUsed;
            UserId = dto.UserId ?? string.Empty;
        }

        #endregion
    }
}