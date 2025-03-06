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

using System.Collections.Generic;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     This is used to show the order information on
    ///     order receipt and customer account history page
    /// </summary>
    public class OrderViewModel
    {
        /// <summary>
        ///     Set default value with passed order information
        /// </summary>
        /// <param name="o"></param>
        public OrderViewModel(Order o)
        {
            LocalOrder = o;
            Items = o.Items;
            OrderNumber = o.OrderNumber;
            FullOrderStatusDescription = o.FullOrderStatusDescription();
            BillingAddressAsHtml = o.BillingAddress.ToHtmlString();
            HasShippingItems = o.HasShippingItems;
            ShippingAddressAsHtml = o.ShippingAddress.ToHtmlString();
            Instructions = o.Instructions;
            Coupons = o.Coupons;
            TotalsAsTable = o.TotalsAsTable();
            IsRecurring = o.IsRecurring;
        }

        /// <summary>
        ///     Order information. More detail information about Order can be read at <see cref="Order" />
        /// </summary>
        public Order LocalOrder { get; set; }

        /// <summary>
        ///     Items purchased under this order. More detail about individual line item can be read at  <see cref="LineItem" />
        /// </summary>
        public List<LineItem> Items { get; set; }

        /// <summary>
        ///     Unique identifier for the order
        /// </summary>
        public string OrderNumber { get; set; }

        /// <summary>
        ///     Order status whether paid, unpaid, shipped, partial paid etc.
        /// </summary>
        public string FullOrderStatusDescription { get; set; }

        /// <summary>
        ///     Billing address information in html format to be viewed on the screen
        /// </summary>
        public string BillingAddressAsHtml { get; set; }

        /// <summary>
        ///     Indicates whether any of the items purchased on this order are eligible for shipping or not.
        ///     There are many products in store which are not shippable.
        /// </summary>
        public bool HasShippingItems { get; set; }

        /// <summary>
        ///     Shipping address information in html format to be viewed along with order detail.
        /// </summary>
        public string ShippingAddressAsHtml { get; set; }

        /// <summary>
        ///     Any specicial instruction for the order from the customer to seller
        /// </summary>
        public string Instructions { get; set; }

        /// <summary>
        ///     Coupons applied for the specific order. More detail about the coupon can be read at  <see cref="OrderCoupon" />
        /// </summary>
        public List<OrderCoupon> Coupons { get; set; }

        /// <summary>
        ///     string which represents the total information on the screen. This includs total of tax, total of shipping, sub
        ///     total, net amount, amount ex vat
        /// </summary>
        public string TotalsAsTable { get; set; }

        /// <summary>
        ///     Indicates whether its recurring order or one time order.
        /// </summary>
        public bool IsRecurring { get; set; }
    }
}