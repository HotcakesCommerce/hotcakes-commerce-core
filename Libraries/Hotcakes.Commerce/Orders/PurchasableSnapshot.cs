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

using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     Creates a purchasable snapshot with empty identifiers and descriptive values plus zero or false pricing, tax, shipping, and mode defaults.
    /// </summary>
    public class PurchasableSnapshot
    {
        /// <summary>
        ///     Creates a purchasable snapshot with empty identifiers and descriptive values plus zero or false pricing, tax, shipping, and mode defaults.
        /// </summary>
        public PurchasableSnapshot()
        {
            ProductId = string.Empty;
            VariantId = string.Empty;
            Name = string.Empty;
            Sku = string.Empty;
            Description = string.Empty;
            BasePrice = 0m;
            IsTaxExempt = false;
            TaxScheduleId = 0;
            SelectionData = new OptionSelections();
            ShippingDetails = new ShippableItem();
            IsBundle = false;
            IsGiftCard = false;
            IsRecurring = false;
        }

        /// <summary>
        ///     Gets or sets the product id value used by this API.
        /// </summary>
        public string ProductId { get; set; }
        /// <summary>
        ///     Gets or sets the variant id value used by this API.
        /// </summary>
        public string VariantId { get; set; }
        /// <summary>
        ///     Gets or sets the name value used by this API.
        /// </summary>
        public string Name { get; set; }
        /// <summary>
        ///     Gets or sets the sku value used by this API.
        /// </summary>
        public string Sku { get; set; }
        /// <summary>
        ///     Gets or sets the description value used by this API.
        /// </summary>
        public string Description { get; set; }
        /// <summary>
        ///     Gets or sets the base price value used by this API.
        /// </summary>
        public decimal BasePrice { get; set; }
        /// <summary>
        ///     Gets or sets the is tax exempt value used by this API.
        /// </summary>
        public bool IsTaxExempt { get; set; }
        /// <summary>
        ///     Gets or sets the tax schedule id value used by this API.
        /// </summary>
        public long TaxScheduleId { get; set; }
        /// <summary>
        ///     Gets or sets the selection data value used by this API.
        /// </summary>
        public OptionSelections SelectionData { get; set; }
        /// <summary>
        ///     Gets or sets the shipping details value used by this API.
        /// </summary>
        public ShippableItem ShippingDetails { get; set; }
        /// <summary>
        ///     Gets or sets the is bundle value used by this API.
        /// </summary>
        public bool IsBundle { get; set; }
        /// <summary>
        ///     Gets or sets the is gift card value used by this API.
        /// </summary>
        public bool IsGiftCard { get; set; }
        /// <summary>
        ///     Gets or sets the is recurring value used by this API.
        /// </summary>
        public bool IsRecurring { get; set; }
    }
}
