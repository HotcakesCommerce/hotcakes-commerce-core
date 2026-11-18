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
using System.Linq;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Marketing;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>Calculated price information for a product and a customer selection.</summary>
    public class UserSpecificPrice
    {
        public UserSpecificPrice(Product initialProduct, OptionSelections selections, StoreSettings storeSettings)
        {
            if (initialProduct == null) throw new ArgumentNullException("Initial Product can not be null");

            // init
            IsValid = true;
            DiscountDetails = new List<DiscountDetail>();
            VariantId = string.Empty;

            // Load from Product
            Sku = initialProduct.Sku;
            InitialListPrice = initialProduct.ListPrice;
            InitialBasePrice = initialProduct.SitePrice;
            OverrideText = initialProduct.SitePriceOverrideText;

            if (initialProduct.IsUserSuppliedPrice || initialProduct.IsGiftCard)
            {
                OverrideText = " ";
            }

            PriceForSelections(initialProduct, selections, storeSettings);
        }

        private decimal InitialListPrice { get; set; }
        private decimal InitialBasePrice { get; set; }
        private decimal ModifierAdjustments { get; set; }

        /// <summary>Gets or sets the IsValid value.</summary>
        public bool IsValid { get; private set; }
        public string OverrideText { get; set; }

        public string Sku { get; private set; }
        /// <summary>Gets or sets the VariantId value.</summary>
        public string VariantId { get; private set; }
        public List<DiscountDetail> DiscountDetails { get; set; }

        // Calculated Properties
        /// <summary>Gets or sets the ListPrice value.</summary>
        public decimal ListPrice
        {
            get { return InitialListPrice + ModifierAdjustments; }
        }

        /// <summary>Gets or sets the BasePrice value.</summary>
        public decimal BasePrice
        {
            get { return InitialBasePrice + ModifierAdjustments; }
        }

        /// <summary>Gets or sets the Savings value.</summary>
        public decimal Savings
        {
            get
            {
                if (ListPrice <= PriceWithAdjustments()) return 0;
                return Math.Round(ListPrice - PriceWithAdjustments(), 2);
            }
        }

        /// <summary>Gets or sets the SavingsPercent value.</summary>
        public decimal SavingsPercent
        {
            get { return Math.Round(Savings/ListPrice*100, 0); }
        }

        /// <summary>Gets or sets the ListPriceGreaterThanCurrentPrice value.</summary>
        public bool ListPriceGreaterThanCurrentPrice
        {
            get { return ListPrice > BasePrice; }
        }

        private void PriceForSelections(Product p, OptionSelections selections, StoreSettings storeSettings)
        {
            if (selections == null || p == null)
                return;

            IsValid = true;
            VariantId = string.Empty;
            ModifierAdjustments = 0;

            if (!p.IsBundle)
            {
                // Check for Option Price Modifiers
                if (!p.HasOptions())
                    return;
                ModifierAdjustments = selections.OptionSelectionList.GetPriceAdjustmentForSelections(p.Options);

                // Check for Variant Changes
                if (!p.HasVariants())
                    return;
                var v = p.Variants.FindBySelectionData(selections.OptionSelectionList, p.Options);
                if (v == null)
                {
                    IsValid = false;
                    return;
                }

                // Assign Variant Attributes to this price data
                VariantId = v.Bvin;
                if (!string.IsNullOrWhiteSpace(v.Sku))
                    Sku = v.Sku;
                if (v.Price >= 0)
                    InitialBasePrice = v.Price;
            }
            else
            {
                // Check global store option
                foreach (var bundledProductAdv in p.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null || !bundledProduct.HasOptions())
                        continue;

                    var optionSelections = selections.GetSelections(bundledProductAdv.Id);

                    if (storeSettings.UseChildChoicesAdjustmentsForBundles)
                    {
                        var optionsAdjustments = optionSelections.GetPriceAdjustmentForSelections(bundledProduct.Options);
                        ModifierAdjustments += optionsAdjustments*bundledProductAdv.Quantity;
                    }

                    if (!bundledProduct.HasVariants())
                        continue;
                    var v = bundledProduct.Variants.FindBySelectionData(optionSelections, bundledProduct.Options);
                    if (v == null)
                        IsValid = false;
                }
            }
        }

        /// <summary>Formats the calculated price, optionally including adjustments and currency formatting.</summary>
        /// <returns>The result of this operation.</returns>
        public string DisplayPrice()
        {
            return DisplayPrice(true);
        }

        /// <summary>Formats the calculated price, optionally including adjustments and currency formatting.</summary>
        /// <param name="includeAdjustments">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public string DisplayPrice(bool includeAdjustments)
        {
            return DisplayPrice(includeAdjustments, true);
        }

        /// <summary>Formats the calculated price, optionally including adjustments and currency formatting.</summary>
        /// <param name="includeAdjustments">Input used by this operation.</param>
        /// <param name="noFormatting">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public string DisplayPrice(bool includeAdjustments, bool noFormatting)
        {
            if (OverrideText.Length > 0)
            {
                return OverrideText;
            }
            if (includeAdjustments)
            {
                var pricewith = PriceWithAdjustments();
                if (pricewith < BasePrice)
                {
                    return noFormatting
                        ? pricewith.ToString("F")
                        : string.Format("<strike>{0}</strike> {1}", BasePrice.ToString("C"), pricewith.ToString("C"));
                }
                return noFormatting ? pricewith.ToString("F") : pricewith.ToString("C");
            }
            return noFormatting ? BasePrice.ToString("F") : BasePrice.ToString("C");
        }

        /// <summary>Adds a discount detail to the calculated price.</summary>
        /// <param name="amount">Input used by this operation.</param>
        /// <param name="description">Input used by this operation.</param>
        /// <param name="promotionId">Input used by this operation.</param>
        /// <param name="actionId">Input used by this operation.</param>
        public void AddAdjustment(decimal amount, string description, long promotionId = -1, long actionId = -1)
        {
            AddAdjustment(new DiscountDetail {Amount = amount, Description = description});
        }

        /// <summary>Adds a discount detail to the calculated price.</summary>
        /// <param name="discount">Input used by this operation.</param>
        public void AddAdjustment(DiscountDetail discount)
        {
            DiscountDetails.Add(discount);
        }

        /// <summary>Removes all discount details from the calculated price.</summary>
        public void ClearAllDiscounts()
        {
            DiscountDetails.Clear();
        }

        /// <summary>Returns the base price plus all discount-detail amounts.</summary>
        /// <returns>The result of this operation.</returns>
        public decimal PriceWithAdjustments()
        {
            var result = BasePrice;
            result += DiscountDetails.Sum(y => y.Amount);
            return result;
        }
    }
}
