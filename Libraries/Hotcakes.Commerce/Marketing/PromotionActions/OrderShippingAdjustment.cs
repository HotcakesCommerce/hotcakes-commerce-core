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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Marketing.PromotionActions
{
    /// <summary>Adjusts the order's shipping charge during shipping promotion evaluation.</summary>
    /// <remarks>The adjustment is applied to the current shipping rate when one is being evaluated; otherwise it is persisted as an order shipping discount.</remarks>
    public class OrderShippingAdjustment : PromotionActionBase
    {
        /// <summary>Stable serialized identifier for the order-shipping adjustment action.</summary>
        public const string TypeIdString = "608c118e-cf72-4703-b4cb-dab1d579c53e";

        private static readonly Guid _typeId = new Guid(TypeIdString);

        /// <summary>Initializes a monetary shipping adjustment with amount zero.</summary>
        public OrderShippingAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        /// <summary>Initializes a shipping adjustment with the specified amount mode and signed amount.</summary>
        /// <param name="type">Whether the amount is monetary or percentage-based.</param>
        /// <param name="amount">The signed adjustment amount.</param>
        public OrderShippingAdjustment(AmountTypes type, decimal amount)
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = type;
            Amount = amount;
        }

        /// <summary>Gets the stable serialized action identifier.</summary>
        public override Guid TypeId
        {
            get { return _typeId; }
        }

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

        /// <summary>Gets or sets the signed shipping adjustment amount.</summary>
        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        /// <summary>Describes the shipping adjustment.</summary>
        /// <param name="app">The application context; it is not used by this implementation.</param>
        /// <returns>The action description.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var amount = Amount;
            var isDiscount = amount < 0m;
            var result = (isDiscount ? "Decrease" : "Increase") + " Shipping by ";
            var adjustmentType = AdjustmentType;

            switch (adjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    result += Math.Abs(amount).ToString("c");
                    break;
                case AmountTypes.Percent:
                    result += (Math.Abs(amount) / 100m).ToString("p");
                    break;
            }
            return result;
        }

        /// <summary>Calculates and applies the shipping adjustment for the current promotion pass.</summary>
        /// <param name="context">The shipping promotion context.</param>
        /// <returns><see langword="false"/> for invalid or non-shipping contexts; otherwise <see langword="true"/>.</returns>
        /// <remarks>This implementation does not perform duplicate suppression or override cancellation.</remarks>
        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.Order == null) return false;

            // only apply when applying to shipping areas
            if (context.Mode != PromotionType.OfferForShipping) return false;

            // Determine whether we're working with a specific shipping method
            var hasCurrentShippingMethod = !string.IsNullOrWhiteSpace(context.CurrentShippingMethodId);

            // Determine base shipping rate once
            decimal baseShippingRate = hasCurrentShippingMethod
                ? context.AdjustedShippingRate
                : context.Order.TotalShippingBeforeDiscounts;

            var adjustmentType = AdjustmentType;
            var amount = Amount;

            decimal adjustment = 0m;
            switch (adjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    adjustment = Money.GetDiscountAmount(baseShippingRate, amount);
                    break;
                case AmountTypes.Percent:
                    adjustment = Money.GetDiscountAmountByPercent(baseShippingRate, amount);
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