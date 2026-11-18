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

namespace Hotcakes.Modules.Core.Api.Mobile.Models
{
    /// <summary>
    /// Describes a product line in a mobile order response.
    /// </summary>
    [Serializable]
    public class OrderItem
    {
        /// <summary>Gets or sets the product name.</summary>
        public string ProductName { get; set; }
        /// <summary>Gets or sets the quantity ordered.</summary>
        public int Quantity { get; set; }
        /// <summary>Gets or sets the base price for one item.</summary>
        public decimal PricePerItem { get; set; }
    }

    /// <summary>
    /// Describes a pending payment hold associated with an order.
    /// </summary>
    [Serializable]
    public class HoldTransaction
    {
        /// <summary>Gets or sets the display information for the payment card and hold amount.</summary>
        public string CardInfo { get; set; }
        /// <summary>Gets or sets the payment transaction identifier.</summary>
        public string TransactionId { get; set; }
    }

    /// <summary>
    /// Contains the summary fields returned for a mobile order listing.
    /// </summary>
    [Serializable]
    public class OrderBrief
    {
        /// <summary>Gets or sets the store-scoped order ID.</summary>
        public string OrderId { get; set; }
        /// <summary>Gets or sets the customer's display name.</summary>
        public string CustomerName { get; set; }
        /// <summary>Gets or sets the human-readable order number.</summary>
        public string OrderNumber { get; set; }
        /// <summary>Gets or sets the order grand total.</summary>
        public decimal Total { get; set; }
        /// <summary>Gets or sets the payment status display value.</summary>
        public string PaymentStatus { get; set; }
        /// <summary>Gets or sets the shipping status display value.</summary>
        public string ShippingStatus { get; set; }
        /// <summary>Gets or sets the order status name.</summary>
        public string StatusCode { get; set; }
        /// <summary>Gets or sets the UTC order date.</summary>
        public DateTime OrderDate { get; set; }
    }

    /// <summary>
    /// Contains the detailed fields returned for a mobile order.
    /// </summary>
    [Serializable]
    public class Order : OrderBrief
    {
        /// <summary>Gets or sets the amount authorized for payment.</summary>
        public decimal PaymentAmountAuthorized;
        public string CustomerEmail { get; set; }
        /// <summary>Gets or sets the customer's email address.</summary>
        /// <summary>Gets or sets the customer's telephone number.</summary>
        public string CustomerPhone { get; set; }
        /// <summary>Gets or sets the shipping price before other order totals.</summary>
        public decimal ShippingPrice { get; set; }
        /// <summary>Gets or sets the order tax total.</summary>
        public decimal Tax { get; set; }
        /// <summary>Gets or sets the total discounts applied to the order.</summary>
        public decimal TotalDiscounts { get; set; }
        /// <summary>Gets or sets the product items in the order.</summary>
        public List<OrderItem> Items { get; set; }
        /// <summary>Gets or sets the pending payment holds.</summary>
        public List<HoldTransaction> PendingHolds { get; set; }
        /// <summary>Gets or sets the customer's special instructions.</summary>
        public string SpecialInstructions { get; set; }
        /// <summary>Gets or sets the amount charged to the payment method.</summary>
        public decimal PaymentAmountCharged { get; set; }
        /// <summary>Gets or sets the amount refunded to the payment method.</summary>
        public decimal PaymentAmountRefunded { get; set; }
        /// <summary>Gets or sets the amount still due for payment.</summary>
        public decimal PaymentAmountDue { get; set; }
    }
}
