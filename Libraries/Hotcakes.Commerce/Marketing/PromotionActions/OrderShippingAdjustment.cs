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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class OrderShippingAdjustment : PromotionActionBase
    {
        public const string TypeIdString = "608c118e-cf72-4703-b4cb-dab1d579c53e";

        public OrderShippingAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        public OrderShippingAdjustment(AmountTypes type, decimal amount)
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = type;
            Amount = amount;
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

            var result = (isDiscount ? "Decrease" : "Increase") + " Shipping by ";

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
            if (context == null) return false;
            if (context.Order == null) return false;

            // only apply when applying to shipping areas
            if (context.Mode != PromotionType.OfferForShipping) return false;

            // TODO: It doesn't make sense
            //if (context.AdjustedShippingRate <= 0)
            //{
            //	context.AdjustedShippingRate = context.Order.TotalShippingBeforeDiscounts;
            //}

            // Business logic below should include 2 cases:
            // 1) when discount for current order is calculated
            // 2) when discount is calculated for all possible shipping method in the loop
            // This can be determined by value of CurrentShippingMethodId variable
            decimal baseShippingRate = 0;
            if (!string.IsNullOrWhiteSpace(context.CurrentShippingMethodId))
                baseShippingRate = context.AdjustedShippingRate;
            else
                baseShippingRate = context.Order.TotalShippingBeforeDiscounts;

            decimal adjustment = 0;
            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    adjustment = Money.GetDiscountAmount(baseShippingRate, Amount);
                    break;
                case AmountTypes.Percent:
                    adjustment = Money.GetDiscountAmountByPercent(baseShippingRate, Amount);
                    break;
            }

            if (!string.IsNullOrWhiteSpace(context.CurrentShippingMethodId))
                context.AdjustedShippingRate = context.AdjustedShippingRate + adjustment;
            else
                context.Order.AddShippingDiscount(adjustment, context.CustomerDescription, context.PromotionId, Id);

            return true;
        }
    }
}