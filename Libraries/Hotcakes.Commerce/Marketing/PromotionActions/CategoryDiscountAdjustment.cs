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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    /// <summary>Adjusts qualifying line-item totals when a product belongs to a configured category.</summary>
    /// <remarks>Category identifiers are stored under <c>categoryids</c>, normalized to uppercase, and joined by commas.</remarks>
    public class CategoryDiscountAdjustment : PromotionActionBase
    {
        #region Constants

        /// <summary>Stable serialized identifier for the category adjustment action.</summary>
        public const string TypeIdString = "9482acc3-9ef6-4490-9890-cb7787d6b0ee";

        #endregion

        #region Init

        /// <summary>Initializes a category adjustment with a monetary amount of zero.</summary>
        public CategoryDiscountAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        #endregion

        #region Properties

        /// <summary>Gets or sets whether <see cref="Amount"/> is monetary or percentage-based.</summary>
        public AmountTypes AdjustmentType
        {
            get
            {
                var temp = GetSetting("AdjustmentType");
                var result = AmountTypes.MonetaryAmount;
                Enum.TryParse(temp, out result);
                return result;
            }
            set { SetSetting("AdjustmentType", (int) value); }
        }

        /// <summary>Gets or sets the signed adjustment amount; negative values decrease the qualifying total.</summary>
        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        #endregion

        #region Overrides

        /// <summary>Describes the category adjustment and lists configured categories as HTML.</summary>
        /// <param name="app">The application context used to resolve category names and URLs.</param>
        /// <returns>The localized-style HTML description used by the promotion UI.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var isDiscount = Amount < 0;

            var result = (isDiscount ? "Decrease" : "Increase") + " Category ";

            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    result += Math.Abs(Amount).ToString("c");
                    break;
                case AmountTypes.Percent:
                    result += (Math.Abs(Amount)/100m).ToString("p");
                    break;
            }

            result += "<ul>";
            var categories = GetCategories();

            foreach (var bvin in categories)
            {
                var c = app.CatalogServices.Categories.Find(bvin);
                if (c != null)
                {
                    result += "<li>" + c.Name + "<br />";
                    result += "<em>" + c.RewriteUrl + "</em></li>";
                }
            }

            result += "</ul>";

            return result;
        }

        /// <summary>Applies one discount detail to the currently processed qualifying line item.</summary>
        /// <param name="context">The line-item promotion context.</param>
        /// <returns><see langword="false"/> for an invalid or non-line-item context; otherwise <see langword="true"/>.</returns>
        /// <remarks>Only the first 100 category assignments are inspected. A matching promotion is not added twice.</remarks>
        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.Mode != PromotionType.OfferForLineItems) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;
            if (context.CurrentlyProcessingLineItem == null) return false;

            var li = context.CurrentlyProcessingLineItem;

            var hasDiscount = false;

            // Note: this only checks the first 100 categories. You're pretty much insane if you're
            // running a promotion on a product by category and it's in more than 100 categories.
            var assignments
                = context.HccApp.CatalogServices.CategoriesXProducts.FindForProduct(li.ProductId, 1, 100);

            var categories = GetCategories();

            foreach (var cross in assignments)
            {
                var match = cross.CategoryId.Trim().ToUpperInvariant();
                if (categories.Contains(match))
                {
                    hasDiscount = true;
                    break;
                }
            }

            if (hasDiscount)
            {
                var adjustment = 0m;

                switch (AdjustmentType)
                {
                    case AmountTypes.MonetaryAmount:
                        adjustment = Money.GetDiscountAmount(li.LineTotalWithSalesWithoutDiscounts, Amount);
                        break;
                    case AmountTypes.Percent:
                        adjustment = Money.GetDiscountAmountByPercent(li.LineTotalWithSalesWithoutDiscounts, Amount);
                        break;
                }

                // don't try to add it again, if it's already there
                if (li.DiscountDetails.Any(d => d.PromotionId == context.PromotionId)) return true;

                li.DiscountDetails.Add(new DiscountDetail
                {
                    Amount = adjustment,
                    Description = context.CustomerDescription,
                    PromotionId = context.PromotionId,
                    ActionId = Id
                });
            }

            return true;
        }

        /// <summary>Gets the stable serialized action identifier.</summary>
        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        #endregion

        #region Helper Methods

        /// <summary>Gets configured category identifiers, normalized to trimmed uppercase values.</summary>
        /// <returns>A new list of category identifiers; malformed empty entries are omitted.</returns>
        public List<string> GetCategories()
        {
            var all = GetSetting("categoryids");
            if (string.IsNullOrWhiteSpace(all)) return new List<string>();
            return
                all.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().ToUpperInvariant())
                    .ToList();
        }

        /// <summary>Persists category identifiers in the compatibility-sensitive <c>categoryids</c> setting.</summary>
        /// <param name="items">The identifiers to normalize and persist; null is treated as an empty list.</param>
        public void SaveIdsToSettings(List<string> items)
        {
            var safeItems = items ?? new List<string>();
            var all = string.Join(",", safeItems.Select(s => s.Trim().ToUpperInvariant()));

            SetSetting("categoryids", all);
        }

        #endregion

        #region Public Methods

        /// <summary>Adds a category identifier unless it is empty or already present.</summary>
        /// <param name="id">The category identifier to normalize and add.</param>
        public void AddCategoryId(string id)
        {
            var ids = GetCategories();

            var possible = (id ?? string.Empty).Trim().ToUpperInvariant();
            if (string.IsNullOrEmpty(possible)) return;
            if (ids.Contains(possible)) return;
            ids.Add(possible);
            SaveIdsToSettings(ids);
        }

        /// <summary>Removes a matching normalized category identifier, if present.</summary>
        /// <param name="id">The category identifier to normalize and remove.</param>
        public void RemoveCategoryId(string id)
        {
            var ids = GetCategories();

            var normalized = (id ?? string.Empty).Trim().ToUpperInvariant();
            if (ids.Contains(normalized))
            {
                ids.Remove(normalized);
                SaveIdsToSettings(ids);
            }
        }

        #endregion
    }
}