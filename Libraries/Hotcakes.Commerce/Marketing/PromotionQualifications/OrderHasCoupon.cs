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

namespace Hotcakes.Commerce.Marketing.PromotionQualifications
{
    public class OrderHasCoupon : PromotionQualificationBase
    {
        public OrderHasCoupon()
        {
            ProcessingCost = RelativeProcessingCost.Normal;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdOrderHasCoupon); }
        }

        public List<string> CurrentCoupons()
        {
            var all = GetSetting("coupons") ?? string.Empty;
            if (string.IsNullOrWhiteSpace(all)) return new List<string>();

            return all
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim().ToUpperInvariant())
                .Where(s => s.Length > 0)
                .ToList();
        }

        private void SaveCouponsToSettings(List<string> coupons)
        {
            var list = coupons?.Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpperInvariant())
                .ToList() ?? new List<string>();

            var all = list.Count == 0 ? string.Empty : string.Join(",", list);
            SetSetting("coupons", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "Order Has Coupon Codes:<ul>";
            foreach (var coupon in CurrentCoupons())
            {
                result += "<li>" + coupon + "</li>";
            }
            result += "</ul>";
            return result;
        }

        public void AddCoupon(string coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon)) return;

            var normalized = coupon.Trim().ToUpperInvariant();
            var coupons = CurrentCoupons();
            if (coupons.Contains(normalized)) return;

            coupons.Add(normalized);
            SaveCouponsToSettings(coupons);
        }

        public void RemoveCoupon(string coupon)
        {
            if (string.IsNullOrWhiteSpace(coupon)) return;

            var normalized = coupon.Trim().ToUpperInvariant();
            var coupons = CurrentCoupons();
            if (coupons.Remove(normalized))
            {
                SaveCouponsToSettings(coupons);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Orders && mode != PromotionQualificationMode.LineItems) return false;
            if (context == null) return false;
            if (context.Order == null) return false;

            var coupons = CurrentCoupons();
            if (coupons.Count == 0) return false;

            // Use Order helper to determine existence; assume it handles case appropriately.
            return coupons.Any(c => context.Order.CouponCodeExists(c));
        }
    }
}