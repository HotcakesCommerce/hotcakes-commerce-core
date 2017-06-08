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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class ProductTypeIs : PromotionIdQualificationBase
    {
        public const string TypeIdString = "04065B1E-5217-41EF-9801-7AB5244A3373";

        public bool IsNotMode
        {
            get
            {
                var all = GetSetting("PrTIsNotMode");

                return all == "1";
            }
            set { SetSetting("PrTIsNotMode", value); }
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdString); }
        }

        public override string IdSettingName
        {
            get { return "ProductTypeIds"; }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Product Type is" + (IsNotMode ? " not" : string.Empty) + ":<ul>";
            var ids = CurrentIds();

            foreach (var bvin in ids)
            {
                var c = app.CatalogServices.ProductTypes.Find(bvin);
                if (c != null)
                {
                    result += "<li>" + c.ProductTypeName + "<br />";
                }
            }
            result += "</ul>";

            return result;
        }

        protected override void OnInit()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            var ids = CurrentIds();

            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            if (mode == PromotionQualificationMode.LineItems)
            {
                if (context.CurrentlyProcessingLineItem == null) return false;

                var li = context.CurrentlyProcessingLineItem;

                return MeetLineItem(context, li, ids);
            }
            if (mode == PromotionQualificationMode.Orders)
            {
                var items = context.Order.Items;
                return items.Any(i => MeetLineItem(context, i, ids));
            }

            return false;
        }

        private bool MeetLineItem(PromotionContext context, LineItem li, List<string> ids)
        {
            var prod = context.HccApp.CatalogServices.Products.FindWithCache(li.ProductId);

            if (prod == null)
            {
                return false;
            }

            var type = prod.ProductTypeId == string.Empty ? "00000000-0000-0000-0000-000000000000" : prod.ProductTypeId;
                // additional check for generic.

            if (IsNotMode)
            {
                return !ids.Contains(type);
            }
            return ids.Contains(type);
        }
    }
}