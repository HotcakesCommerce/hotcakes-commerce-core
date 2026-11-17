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
    /// <summary>Adjusts the order total during order-level promotion evaluation.</summary>
    /// <remarks>The computed adjustment is persisted through the order's discount collection.</remarks>
    public class OrderTotalAdjustment : PromotionActionBase
    {
        /// <summary>Stable serialized identifier for the order-total adjustment action.</summary>
        public const string TypeIdString = "6574b0b9-9c65-4968-8605-eecf6f7a0407";
            
        private static readonly Guid _typeId = new Guid(TypeIdString);

        /// <summary>Initializes a monetary order-total adjustment with amount zero.</summary>
        public OrderTotalAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
        }

        /// <summary>Initializes an order-total adjustment with the specified amount mode and signed amount.</summary>
        /// <param name="type">Whether the amount is monetary or percentage-based.</param>
        /// <param name="amount">The signed adjustment amount.</param>
        public OrderTotalAdjustment(AmountTypes type, decimal amount)
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

        /// <summary>Gets or sets the signed order-total adjustment amount.</summary>
        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        /// <summary>Describes the order-total adjustment.</summary>
        /// <param name="app">The application context; it is not used by this implementation.</param>
        /// <returns>The action description.</returns>
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

        /// <summary>Calculates and records an adjustment against the order total.</summary>
        /// <param name="context">The order promotion context.</param>
        /// <returns><see langword="false"/> for invalid or non-order contexts; otherwise <see langword="true"/>.</returns>
        /// <remarks>The current order total is calculated once, then persisted through the order discount API.</remarks>
        public override bool ApplyAction(PromotionContext context)
        {
            if (context == null) return false;
            if (context.Order == null) return false;

            // only apply when applying to sub total
            if (context.Mode != PromotionType.OfferForOrder) return false;

            // Compute order total once instead of calling GetTotal repeatedly.
            var orderTotal = context.Order.GetTotal(false, false, false);

            decimal adjustment = 0;

            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    adjustment = Money.GetDiscountAmount(orderTotal, Amount);
                    break;
                case AmountTypes.Percent:
                    adjustment = Money.GetDiscountAmountByPercent(orderTotal, Amount);
                    break;
            }

            context.Order.AddOrderDiscount(adjustment, context.CustomerDescription, context.PromotionId, Id);

            return true;
        }
    }
}