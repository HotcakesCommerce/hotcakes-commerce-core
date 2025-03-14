﻿#region License

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
using System.Linq;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class LineItemAdjustment : PromotionActionBase
    {
        public const string TypeIdString = "adf5b289-3f11-4e90-a8c7-fe62a0274205";

        public LineItemAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

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

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var isDiscount = Amount < 0;

            var result = (isDiscount ? "Decrease" : "Increase") + " Qualifying Item Price by ";

            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    result += Math.Abs(Amount).ToString("c");
                    break;
                case AmountTypes.Percent:
                    result += (Math.Abs(Amount)/100m).ToString("p");
                    break;
            }
            return result;
        }

        public override bool ApplyAction(PromotionContext context)
        {
            try
            {
                if (context == null) return false;
                if (context.Mode != PromotionType.OfferForLineItems) return false;

                if (context.Order == null) return false;
                if (context.Order.Items == null) return false;
                if (context.CurrentlyProcessingLineItem == null) return false;

                var li = context.CurrentlyProcessingLineItem;
                var promo = context.HccApp.MarketingServices.Promotions.Find(context.PromotionId);

                // Check if lineitem already has this promotion. Dont apply it again in that case.
                if (li.DiscountDetails.Any(d => d.PromotionId == context.PromotionId))
                {
                    return true;
                }
                //if (promo.DoNotCombine && context.Order.HasAnyNonSaleDiscounts) return false;

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


                li.DiscountDetails.Add(new DiscountDetail
                {
                    Amount = adjustment,
                    Description = context.CustomerDescription,
                    PromotionId = context.PromotionId,
                    ActionId = Id,
                    DiscountType = PromotionType.OfferForLineItems
                });

                return true;
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return false;
            }
        }
    }
}