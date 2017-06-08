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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order Coupon in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is OrderCoupon.</remarks>
    [DataContract]
    [Serializable]
    public class OrderCouponDTO
    {
        public OrderCouponDTO()
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
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the order coupon was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the order that this order coupon belongs to.
        /// </summary>
        [DataMember]
        public string OrderBvin { get; set; }

        /// <summary>
        ///     The coupon code for the current order coupon.
        /// </summary>
        [DataMember]
        public string CouponCode { get; set; }

        /// <summary>
        ///     Defines whether or not the current order coupon has been used.
        /// </summary>
        [DataMember]
        public bool IsUsed { get; set; }

        /// <summary>
        ///     Records the user account ID that is using the coupon.
        /// </summary>
        [DataMember]
        public string UserId { get; set; }
    }
}