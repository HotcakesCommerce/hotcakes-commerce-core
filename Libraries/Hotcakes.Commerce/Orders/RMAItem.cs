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

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    /// Represents the RMAItem order-domain type and preserves the established public compatibility surface.
    /// </summary>
    public class RMAItem
    {
        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
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

        /// <summary>
        /// The store id.
        /// </summary>
        public long StoreId { get; set; }
        /// <summary>
        /// The bvin.
        /// </summary>
        public string Bvin { get; set; }
        /// <summary>
        /// The last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }
        /// <summary>
        /// The rmabvin.
        /// </summary>
        public string RMABvin { get; set; }
        /// <summary>
        /// The line item id.
        /// </summary>
        public long LineItemId { get; set; }
        /// <summary>
        /// The item description.
        /// </summary>
        public string ItemDescription { get; set; }
        /// <summary>
        /// The item name.
        /// </summary>
        public string ItemName { get; set; }
        /// <summary>
        /// The note.
        /// </summary>
        public string Note { get; set; }
        /// <summary>
        /// The reason.
        /// </summary>
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

        /// <summary>
        /// The replace.
        /// </summary>
        public bool Replace { get; set; }
        /// <summary>
        /// The quantity.
        /// </summary>
        public int Quantity { get; set; }
        /// <summary>
        /// The quantity received.
        /// </summary>
        public int QuantityReceived { get; set; }
        /// <summary>
        /// The quantity returned to inventory.
        /// </summary>
        public int QuantityReturnedToInventory { get; set; }
        /// <summary>
        /// The refund amount.
        /// </summary>
        public decimal RefundAmount { get; set; }
        /// <summary>
        /// The refund shipping amount.
        /// </summary>
        public decimal RefundShippingAmount { get; set; }
        /// <summary>
        /// The refund tax amount.
        /// </summary>
        public decimal RefundTaxAmount { get; set; }
        /// <summary>
        /// The refund gift wrap amount.
        /// </summary>
        public decimal RefundGiftWrapAmount { get; set; }
        /// <summary>
        /// The persisted product-class value used by return processing.
        /// </summary>
        public string ProductClass { get; set; }
    }
}
