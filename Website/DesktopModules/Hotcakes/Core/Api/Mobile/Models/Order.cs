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
using System.Collections.Generic;

namespace Hotcakes.Modules.Core.Api.Mobile.Models
{
    [Serializable]
    public class OrderItem
    {
        public string ProductName { get; set; }
        public int Quantity { get; set; }
        public decimal PricePerItem { get; set; }
    }

    [Serializable]
    public class HoldTransaction
    {
        public string CardInfo { get; set; }
        public string TransactionId { get; set; }
    }

    [Serializable]
    public class OrderBrief
    {
        public string OrderId { get; set; }
        public string CustomerName { get; set; }
        public string OrderNumber { get; set; }
        public decimal Total { get; set; }
        public string PaymentStatus { get; set; }
        public string ShippingStatus { get; set; }
        public string StatusCode { get; set; }
        public DateTime OrderDate { get; set; }
    }

    [Serializable]
    public class Order : OrderBrief
    {
        public decimal PaymentAmountAuthorized;
        public string CustomerEmail { get; set; }
        public string CustomerPhone { get; set; }
        public decimal ShippingPrice { get; set; }
        public decimal Tax { get; set; }
        public decimal TotalDiscounts { get; set; }
        public List<OrderItem> Items { get; set; }
        public List<HoldTransaction> PendingHolds { get; set; }
        public string SpecialInstructions { get; set; }
        public decimal PaymentAmountCharged { get; set; }
        public decimal PaymentAmountRefunded { get; set; }
        public decimal PaymentAmountDue { get; set; }
    }
}