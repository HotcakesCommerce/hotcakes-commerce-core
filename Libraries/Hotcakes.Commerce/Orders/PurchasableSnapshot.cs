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

using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Commerce.Orders
{
    public class PurchasableSnapshot
    {
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

        public string ProductId { get; set; }
        public string VariantId { get; set; }
        public string Name { get; set; }
        public string Sku { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }
        public bool IsTaxExempt { get; set; }
        public long TaxScheduleId { get; set; }
        public OptionSelections SelectionData { get; set; }
        public ShippableItem ShippingDetails { get; set; }
        public bool IsBundle { get; set; }
        public bool IsGiftCard { get; set; }
        public bool IsRecurring { get; set; }
    }
}