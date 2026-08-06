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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class OrderHasProducts : PromotionIdQualificationBase
    {
        public OrderHasProducts()
        {
            ProcessingCost = RelativeProcessingCost.Lowest;
            // Only supporting "Has At Least" mode for now
            HasMode = QualificationHasMode.HasAtLeast;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdOrderHasProducts); }
        }


        public QualificationHasMode HasMode
        {
            get
            {
                var result = QualificationHasMode.HasAtLeast;
                var temp = GetSettingAsInt("HasMode");
                if (temp < 0) temp = 0;
                result = (QualificationHasMode) temp;
                return result;
            }
            set { SetSetting("HasMode", (int) value); }
        }

        public QualificationSetMode SetMode
        {
            get
            {
                var result = QualificationSetMode.AnyOfTheseItems;
                var temp = GetSettingAsInt("SetMode");
                if (temp < 0) temp = 0;
                result = (QualificationSetMode) temp;
                return result;
            }
            set { SetSetting("SetMode", (int) value); }
        }

        public int Quantity
        {
            get { return GetSettingAsInt("Quantity"); }
            set { SetSetting("Quantity", value); }
        }

        public override string IdSettingName
        {
            get { return "ProductIds"; }
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "";

            switch (HasMode)
            {
                case QualificationHasMode.HasAtLeast:
                    result += "When order has AT LEAST ";
                    break;
            }
            result += Quantity.ToString();
            switch (SetMode)
            {
                case QualificationSetMode.AllOfTheseItems:
                    result += " of ALL of these products";
                    break;
                case QualificationSetMode.AnyOfTheseItems:
                    result += " of ANY of these products";
                    break;
            }
            result += ":<ul>";

            foreach (var bvin in CurrentIds())
            {
                var p = app.CatalogServices.Products.FindWithCache(bvin);
                if (p != null)
                {
                    result += "<li>[" + p.Sku + "] " + p.ProductName + "</li>";
                }
            }
            result += "</ul>";
            return result;
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Orders &&
                mode != PromotionQualificationMode.LineItems) return false;
            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Items == null) return false;

            var productIds = CurrentIds();
            if (productIds.Count == 0) return false;
            if (Quantity <= 0) return true;

            switch (SetMode)
            {
                case QualificationSetMode.AnyOfTheseItems:
                    return MatchAny(Quantity, context.Order.Items, productIds);
                case QualificationSetMode.AllOfTheseItems:
                    return MatchAll(Quantity, context.Order.Items, productIds);
            }

            return false;
        }

        private bool MatchAny(int qty, List<LineItem> items, List<string> productIds)
        {
            if (qty <= 0) return true;
            if (items == null || items.Count == 0) return false;

            var matchedQuantity = items
                .Where(li => !string.IsNullOrWhiteSpace(li.ProductId) &&
                             productIds.Contains(li.ProductId.Trim().ToLowerInvariant()))
                .Sum(li => li.Quantity);

            return matchedQuantity >= qty;
        }

        private bool MatchAll(int qty, List<LineItem> items, List<string> productIds)
        {
            if (qty <= 0) return true;
            if (productIds == null || productIds.Count == 0) return false;
            if (items == null || items.Count == 0) return false;

            // Build map of productId -> total quantity present in order
            var itemQuantities = items
                .Where(li => !string.IsNullOrWhiteSpace(li.ProductId))
                .GroupBy(li => li.ProductId.Trim().ToLowerInvariant())
                .ToDictionary(g => g.Key, g => g.Sum(li => li.Quantity));

            // Ensure each required productId has at least 'qty' quantity present
            foreach (var required in productIds)
            {
                itemQuantities.TryGetValue(required, out var foundQty);
                if (foundQty < qty) return false;
            }

            return true;
        }
    }
}