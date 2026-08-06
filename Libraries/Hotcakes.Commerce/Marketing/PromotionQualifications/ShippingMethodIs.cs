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
using System.Text;

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
            var all = GetSetting("itemids") ?? string.Empty;

            var parts = all
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.ToUpperInvariant())
                .ToList();

            return parts;
        }

        private void SaveItemIdsToSettings(IEnumerable<string> itemIds)
        {
            if (itemIds == null)
            {
                SetSetting("itemids", string.Empty);
                return;
            }

            var cleaned = itemIds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpperInvariant())
                .Distinct(StringComparer.OrdinalIgnoreCase);

            SetSetting("itemids", string.Join(",", cleaned));
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var sb = new StringBuilder();
            sb.Append("When Order Has Shipping Method Of:<ul>");

            var methods = app?.OrderServices?.ShippingMethods?.FindAll(app.CurrentStore.Id);

            foreach (var itemid in ItemIds())
            {
                var displayName = itemid;

                if (methods != null)
                {
                    var m = methods.SingleOrDefault(y => string.Equals(y.Bvin, itemid, StringComparison.OrdinalIgnoreCase));
                    if (m != null)
                    {
                        displayName = m.Name;
                    }
                }

                sb.Append("<li>");
                sb.Append(displayName);
                sb.Append("</li>");
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        public void AddItemId(string itemid)
        {
            if (string.IsNullOrWhiteSpace(itemid)) return;

            var possible = itemid.Trim().ToUpperInvariant();
            var ids = ItemIds();

            if (ids.Contains(possible)) return;

            ids.Add(possible);
            SaveItemIdsToSettings(ids);
        }

        public void RemoveItemId(string itemid)
        {
            if (string.IsNullOrWhiteSpace(itemid)) return;

            var cleaned = itemid.Trim().ToUpperInvariant();
            var ids = ItemIds();

            var removed = ids.RemoveAll(x => string.Equals(x, cleaned, StringComparison.OrdinalIgnoreCase));
            if (removed > 0)
            {
                SaveItemIdsToSettings(ids);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;

            var idToTest = context.CurrentShippingMethodId?.Trim();
            if (string.IsNullOrEmpty(idToTest))
            {
                idToTest = context.Order?.ShippingMethodId?.Trim();
            }
            if (string.IsNullOrWhiteSpace(idToTest)) return false;

            var idNormalized = idToTest.ToUpperInvariant();
            var idSet = new HashSet<string>(ItemIds(), StringComparer.OrdinalIgnoreCase);

            return idSet.Contains(idNormalized);
        }
    }
}