#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
            var result = new List<string>();
            var all = GetSetting("coupons");
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

        private void SaveCouponsToSettings(List<string> coupons)
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
            var _Coupons = CurrentCoupons();

            var possible = coupon.Trim().ToUpperInvariant();
            if (possible == string.Empty) return;
            if (_Coupons.Contains(possible)) return;
            _Coupons.Add(possible);
            SaveCouponsToSettings(_Coupons);
        }

        public void RemoveCoupon(string coupon)
        {
            var _Coupons = CurrentCoupons();
            if (_Coupons.Contains(coupon.Trim().ToUpperInvariant()))
            {
                _Coupons.Remove(coupon.Trim().ToUpperInvariant());
                SaveCouponsToSettings(_Coupons);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (mode != PromotionQualificationMode.Orders && mode != PromotionQualificationMode.LineItems) return false;
            if (context == null) return false;
            if (context.Order == null) return false;
            if (context.Order.Coupons == null) return false;

            foreach (var coupon in CurrentCoupons())
            {
                if (context.Order.CouponCodeExists(coupon)) return true;
            }

            return false;
        }
    }
}