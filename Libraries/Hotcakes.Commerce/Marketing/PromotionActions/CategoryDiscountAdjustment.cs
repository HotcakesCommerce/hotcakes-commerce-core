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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class CategoryDiscountAdjustment : PromotionActionBase
    {
        #region Constants

        public const string TypeIdString = "9482acc3-9ef6-4490-9890-cb7787d6b0ee";

        #endregion

        #region Init

        public CategoryDiscountAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        #endregion

        #region Properties

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

        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        #endregion

        #region Overrides

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

        public override bool ApplyAction(PromotionContext context)
        {
            if (context.Mode != PromotionType.OfferForLineItems) return false;

            if (context == null) return false;
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

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        #endregion

        #region Helper Methods

        public List<string> GetCategories()
        {
            var result = new List<string>();
            var all = GetSetting("categoryids");
            return
                all.Split(new[] {','}, StringSplitOptions.RemoveEmptyEntries)
                    .Select(s => s.Trim().ToUpperInvariant())
                    .ToList();
        }

        public void SaveIdsToSettings(List<string> items)
        {
            var all = string.Join(",", items.Select(s => s.Trim().ToUpperInvariant()));

            SetSetting("categoryids", all);
        }

        #endregion

        #region Public Methods

        public void AddCategoryId(string id)
        {
            var _Ids = GetCategories();

            var possible = id.Trim().ToLowerInvariant();
            if (possible == string.Empty) return;
            if (_Ids.Contains(possible)) return;
            _Ids.Add(possible);
            SaveIdsToSettings(_Ids);
        }

        public void RemoveCategoryId(string id)
        {
            var _Ids = GetCategories();

            if (_Ids.Contains(id))
            {
                _Ids.Remove(id);
                SaveIdsToSettings(_Ids);
            }
        }

        #endregion
    }
}