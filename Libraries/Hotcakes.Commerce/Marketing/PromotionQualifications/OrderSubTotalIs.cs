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
using System.Linq;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class OrderSubTotalIs : PromotionQualificationBase
    {
        public OrderSubTotalIs()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdOrderSubTotalIs); }
        }

        public decimal Amount
        {
            get { return GetSettingAsDecimal("Amount"); }
            set { SetSetting("Amount", value); }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Order Total >= " + Amount.ToString("C");
            return result;
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.LineItems &&
                mode != PromotionQualificationMode.Orders) return false;
            if (context == null) return false;
            if (context.Order == null) return false;

            var adjustedSubtotal = context.Order.TotalOrderBeforeDiscounts - RemoveDiscountedAmount(context);
            return adjustedSubtotal >= Amount;
        }

        #region Private Methods

        private decimal RemoveDiscountedAmount(PromotionContext context)
        {
            if (context?.Order?.Items == null) return 0m;

            // Sum free item value and explicit discount amounts converted to negative contributions.
            var freeItemsTotal = context.Order.Items
                .Where(i => i.IsFreeItem)
                .Sum(i => i.BasePricePerItem * i.FreeQuantity);

            var discountDetailsTotal = context.Order.Items
                .Where(i => i.DiscountDetails != null && i.DiscountDetails.Any())
                .SelectMany(i => i.DiscountDetails)
                .Where(d => d != null && d.Amount > 0)
                .Sum(d => d.Amount * -1m);

            return freeItemsTotal + discountDetailsTotal;
        }

        #endregion
    }
}