#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    public class UserIs : PromotionQualificationBase
    {
        public UserIs()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdUserIs); }
        }

        public List<string> UserIds()
        {
            var result = new List<string>();
            var all = GetSetting("userids");
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

        private void SaveUserIdsToSettings(List<string> coupons)
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
            SetSetting("userids", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When User Is:<ul>";

            foreach (var userid in UserIds())
            {
                var c = app.MembershipServices.Customers.Find(userid);
                if (c != null)
                {
                    result += "<li>" + c.Email + "</li>";
                }
            }

            result += "</ul>";
            return result;
        }

        public void AddUserId(string uid)
        {
            var _UserIds = UserIds();

            var possible = uid.Trim().ToUpperInvariant();
            if (possible == string.Empty) return;
            if (_UserIds.Contains(possible)) return;
            _UserIds.Add(possible);
            SaveUserIdsToSettings(_UserIds);
        }

        public void RemoveUserId(string uid)
        {
            var _UserIds = UserIds();
            if (_UserIds.Contains(uid.Trim().ToUpperInvariant()))
            {
                _UserIds.Remove(uid.Trim().ToUpperInvariant());
                SaveUserIdsToSettings(_UserIds);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.CurrentCustomer == null) return false;
            if (context.CurrentCustomer.Bvin == string.Empty) return false;

            var currentId = context.CurrentCustomer.Bvin.Trim().ToUpperInvariant();

            foreach (var uid in UserIds())
            {
                if (currentId == uid) return true;
            }

            return false;
        }
    }
}