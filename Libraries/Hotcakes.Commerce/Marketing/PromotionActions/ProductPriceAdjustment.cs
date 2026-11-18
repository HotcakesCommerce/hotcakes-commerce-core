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
    /// <summary>Adjusts a product's user price while a sale promotion is evaluated.</summary>
    /// <remarks>The signed adjustment is recorded on the context's <see cref="PromotionContext.UserPrice"/>.</remarks>
    public class ProductPriceAdjustment : PromotionActionBase
    {
        /// <summary>Stable serialized identifier for the product-price adjustment action.</summary>
        public const string TypeIdString = "a07aff02-ba28-42e0-b334-324de467b2d7";
        private static readonly Guid TypeIdValue = Guid.Parse(TypeIdString);

        /// <summary>Initializes a monetary product-price adjustment with amount zero.</summary>
        public ProductPriceAdjustment()
            : this(AmountTypes.MonetaryAmount, 0m)
        {
        }

        /// <summary>Initializes a product-price adjustment with the specified amount mode and signed amount.</summary>
        /// <param name="type">Whether the amount is monetary or percentage-based.</param>
        /// <param name="amount">The signed adjustment amount.</param>
        public ProductPriceAdjustment(AmountTypes type, decimal amount)
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = type;
            Amount = amount;
        }

        /// <summary>Gets the stable serialized action identifier.</summary>
        public override Guid TypeId => TypeIdValue;

        /// <summary>Gets or sets whether <see cref="Amount"/> is monetary or percentage-based.</summary>
        public AmountTypes AdjustmentType
        {
            get
            {
                var temp = GetSetting("AdjustmentType");
                if (string.IsNullOrWhiteSpace(temp)) return AmountTypes.MonetaryAmount;

                if (Enum.TryParse<AmountTypes>(temp, true, out var result))
                {
                    return result;
                }

                return AmountTypes.MonetaryAmount;
            }
            set { SetSetting("AdjustmentType", (int)value); }
        }

        /// <summary>Gets or sets the signed product-price adjustment amount.</summary>
        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        /// <summary>Describes the product-price adjustment.</summary>
        /// <param name="app">The application context; it is not used by this implementation.</param>
        /// <returns>The action description.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var isDiscount = Amount < 0;
            var result = (isDiscount ? "Decrease" : "Increase") + " Product Price by ";

            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    result += Math.Abs(Amount).ToString("c");
                    break;
                case AmountTypes.Percent:
                    result += (Math.Abs(Amount) / 100m).ToString("p");
                    break;
                default:
                    result += Math.Abs(Amount).ToString();
                    break;
            }

            return result;
        }

        /// <summary>Calculates and records an adjustment against the current user price.</summary>
        /// <param name="context">The sale promotion context.</param>
        /// <returns><see langword="false"/> for missing target data or a non-sale context; otherwise <see langword="true"/>.</returns>
        /// <remarks>Each invocation adds an adjustment; this implementation does not suppress duplicates.</remarks>
        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null || context.Product == null || context.UserPrice == null) return false;

            // only apply when applying to product price
            if (context.Mode != PromotionType.Sale) return false;

            var adjustment = AdjustmentType switch
            {
                AmountTypes.MonetaryAmount => Money.GetDiscountAmount(context.UserPrice.BasePrice, Amount),
                AmountTypes.Percent => Money.GetDiscountAmountByPercent(context.UserPrice.BasePrice, Amount),
                _ => 0m
            };

            context.UserPrice.AddAdjustment(adjustment, context.CustomerDescription, context.PromotionId, Id);

            return true;
        }
    }
}