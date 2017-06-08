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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    public class OrderTotalAdjustment : PromotionActionBase
    {
        public const string TypeIdString = "6574b0b9-9c65-4968-8605-eecf6f7a0407";

        public OrderTotalAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        public OrderTotalAdjustment(AmountTypes type, decimal amount)
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

            var result = (isDiscount ? "Decrease" : "Increase") + " Order Total ";

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

            // only apply when applying to sub total
            if (context.Mode != PromotionType.OfferForOrder) return false;

            var adjustment = 0m;

            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    adjustment = Money.GetDiscountAmount(context.Order.GetTotal(false, false, false), Amount);
                    break;
                case AmountTypes.Percent:
                    adjustment = Money.GetDiscountAmountByPercent(context.Order.GetTotal(false, false, false), Amount);
                    break;
            }

            context.Order.AddOrderDiscount(adjustment, context.CustomerDescription, context.PromotionId, Id);

            return true;
        }
    }
}