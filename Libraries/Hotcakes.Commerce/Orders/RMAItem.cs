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

namespace Hotcakes.Commerce.Orders
{
    public class RMAItem
    {
        public RMAItem()
        {
            StoreId = 0;
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            RMABvin = string.Empty;
            LineItemId = 0;
            ItemName = string.Empty;
            ItemDescription = string.Empty;
            Note = string.Empty;
            Reason = string.Empty;
            Replace = false;
            Quantity = 1;
            QuantityReceived = 0;
            QuantityReturnedToInventory = 0;
            RefundAmount = 0;
            RefundTaxAmount = 0;
            RefundShippingAmount = 0;
            RefundGiftWrapAmount = 0;
            ProductClass = "-1";
        }

        public long StoreId { get; set; }
        public string Bvin { get; set; }
        public DateTime LastUpdated { get; set; }
        public string RMABvin { get; set; }
        public long LineItemId { get; set; }
        public string ItemDescription { get; set; }
        public string ItemName { get; set; }
        public string Note { get; set; }
        public string Reason { get; set; }

        public string ReasonDescription
        {
            get
            {
                switch (Reason.ToLower().Trim())
                {
                    case "-1":
                        return "-- Not Selected --";
                    case "1":
                        return "I changed my mind; didn't like";
                    case "2":
                        return "I ordered the wrong item";
                    case "3":
                        return "The item was damaged but NOT due to shipping";
                    case "4":
                        return "The item was damaged during shipping";
                    case "5":
                        return "I received the wrong item; incorrect item was shipped";
                    case "6":
                        return "Other... Please describe in the comment box below";
                }
                return "Unknown";
            }
        }

        public bool Replace { get; set; }
        public int Quantity { get; set; }
        public int QuantityReceived { get; set; }
        public int QuantityReturnedToInventory { get; set; }
        public decimal RefundAmount { get; set; }
        public decimal RefundShippingAmount { get; set; }
        public decimal RefundTaxAmount { get; set; }
        public decimal RefundGiftWrapAmount { get; set; }
        public string ProductClass { get; set; }
    }
}