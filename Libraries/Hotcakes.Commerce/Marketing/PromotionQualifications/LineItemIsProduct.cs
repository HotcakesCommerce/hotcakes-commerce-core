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
using System.Linq;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class LineItemIsProduct : HasProductsQualificationBase
    {
        public const string TypeIdString = "CCB783E6-9CA3-42FF-A59F-E063D3EFEB99";

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.Order == null) return false;

            if (mode == PromotionQualificationMode.LineItems)
            {
                if (context.CurrentlyProcessingLineItem == null) return false;

                var li = context.CurrentlyProcessingLineItem;

                return MeetLineItem(context, li);
            }
            if (mode == PromotionQualificationMode.Orders)
            {
                var items = context.Order.Items;
                return items.Any(i => MeetLineItem(context, i));
            }

            return false;
        }

        private bool MeetLineItem(PromotionContext context, LineItem li)
        {
            return GetProductIds().Contains(li.ProductId);
        }
    }
}