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

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class ShippingMethodIs : PromotionQualificationBase
    {
        public ShippingMethodIs()
        {
            ProcessingCost = RelativeProcessingCost.Normal;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdShippingMethodIs); }
        }

        public List<string> ItemIds()
        {
            var result = new List<string>();
            var all = GetSetting("itemids");
            var parts = all.Split(',');
            foreach (var s in parts)
            {
                if (s != string.Empty)
                {
                    result.Add(s.Trim().ToUpperInvariant());
                }
            }
            return result;
        }

        private void SaveItemIdsToSettings(List<string> coupons)
        {
            var all = string.Empty;
            foreach (var s in coupons)
            {
                if (s != string.Empty)
                {
                    all += s.Trim().ToUpperInvariant() + ",";
                }
            }
            all = all.TrimEnd(',');
            SetSetting("itemids", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var methods = app.OrderServices.ShippingMethods.FindAll(app.CurrentStore.Id);

            var result = "When Order Has Shipping Method Of:<ul>";
            foreach (var itemid in ItemIds())
            {
                var displayName = itemid;

                if (methods != null)
                {
                    var m = methods.SingleOrDefault(y => y.Bvin.ToUpperInvariant() == itemid.ToUpperInvariant());
                    if (m != null)
                    {
                        displayName = m.Name;
                    }
                }
                result += "<li>" + displayName + "</li>";
            }
            result += "</ul>";
            return result;
        }

        public void AddItemId(string itemid)
        {
            var _ItemIds = ItemIds();

            var possible = itemid.Trim().ToUpperInvariant();
            if (possible == string.Empty) return;
            if (_ItemIds.Contains(possible)) return;
            _ItemIds.Add(possible);
            SaveItemIdsToSettings(_ItemIds);
        }

        public void RemoveItemId(string itemid)
        {
            var _ItemIds = ItemIds();
            if (_ItemIds.Contains(itemid.Trim().ToUpperInvariant()))
            {
                _ItemIds.Remove(itemid.Trim().ToUpperInvariant());
                SaveItemIdsToSettings(_ItemIds);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;

            var idToTest = context.CurrentShippingMethodId.Trim();
            if (idToTest.Length < 1)
            {
                if (context.Order == null) return false;
                idToTest = context.Order.ShippingMethodId.Trim();
            }
            if (idToTest.Length < 1) return false;

            foreach (var itemid in ItemIds())
            {
                if (idToTest.ToUpperInvariant() == itemid.ToUpperInvariant()) return true;
            }

            return false;
        }
    }
}