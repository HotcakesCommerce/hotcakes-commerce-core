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
    /// <summary>Adjusts the price of the currently qualifying line item.</summary>
    /// <remarks>The signed amount is converted through <see cref="Money"/> and recorded as a line-item discount detail.</remarks>
    public class LineItemAdjustment : PromotionActionBase
    {
        /// <summary>Stable serialized identifier for the line-item adjustment action.</summary>
        public const string TypeIdString = "adf5b289-3f11-4e90-a8c7-fe62a0274205";
        private static readonly Guid _typeId = new Guid(TypeIdString);

        /// <summary>Initializes a line-item adjustment with a monetary amount of zero.</summary>
        public LineItemAdjustment()
        {
            Id = 0;
            Settings = new Dictionary<string, string>();
            AdjustmentType = AmountTypes.MonetaryAmount;
            Amount = 0m;
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
                if (string.IsNullOrWhiteSpace(temp)) return AmountTypes.MonetaryAmount;
                AmountTypes result;
                Enum.TryParse(temp, true, out result);
                return result;
            }
            set { SetSetting("AdjustmentType", ((int) value).ToString()); }
        }

        /// <summary>Gets or sets the signed adjustment amount; negative values decrease the item price.</summary>
        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        /// <summary>Describes the qualifying item price adjustment.</summary>
        /// <param name="app">The application context; it is not used by this implementation.</param>
        /// <returns>The action description.</returns>
        public override string FriendlyDescription(HotcakesApplication app)
        {
            var isDiscount = Amount < 0m;
            var action = isDiscount ? "Decrease" : "Increase";
            var absAmount = Math.Abs(Amount);
            string amountText;
            switch (AdjustmentType)
            {
                case AmountTypes.MonetaryAmount:
                    amountText = absAmount.ToString("c");
                    break;
                case AmountTypes.Percent:
                    amountText = (absAmount / 100m).ToString("p");
                    break;
                default:
                    amountText = absAmount.ToString();
                    break;
            }
            return action + " Qualifying Item Price by " + amountText;
        }

        /// <summary>Applies the adjustment to the currently processed line item.</summary>
        /// <param name="context">The line-item promotion context.</param>
        /// <returns><see langword="false"/> for invalid contexts or an exception; otherwise <see langword="true"/>.</returns>
        /// <remarks>The action skips insertion when the line item already contains this promotion.</remarks>
        public override bool ApplyAction(PromotionContext context)
        {
            try
            {
                if (context == null) return false;
                if (context.Mode != PromotionType.OfferForLineItems) return false;

                var order = context.Order;
                if (order == null || order.Items == null) return false;
                if (context.CurrentlyProcessingLineItem == null) return false;

                var li = context.CurrentlyProcessingLineItem;

                // Check if lineitem already has this promotion. Don't apply it again in that case.
                if (li.DiscountDetails.Any(d => d.PromotionId == context.PromotionId))
                {
                    return true;
                }

                var adjustment = 0m;
                switch (AdjustmentType)
                {
                    case AmountTypes.MonetaryAmount:
                        adjustment = Money.GetDiscountAmount(li.LineTotalWithSalesWithoutDiscounts, Amount);
                        break;
                    case AmountTypes.Percent:
                        adjustment = Money.GetDiscountAmountByPercent(li.LineTotalWithSalesWithoutDiscounts, Amount);
                        break;
                    default:
                        adjustment = 0m;
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