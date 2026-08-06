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
            var all = GetSetting("userids") ?? string.Empty;

            var parts = all
                .Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries)
                .Select(s => s.Trim())
                .Where(s => s.Length > 0)
                .Select(s => s.ToUpperInvariant())
                .ToList();

            return parts;
        }

        private void SaveUserIdsToSettings(IEnumerable<string> userIds)
        {
            if (userIds == null)
            {
                SetSetting("userids", string.Empty);
                return;
            }

            var cleaned = userIds
                .Where(s => !string.IsNullOrWhiteSpace(s))
                .Select(s => s.Trim().ToUpperInvariant());

            SetSetting("userids", string.Join(",", cleaned));
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var sb = new StringBuilder();
            sb.Append("When User Is:<ul>");

            foreach (var userid in UserIds())
            {
                var c = app.MembershipServices.Customers.Find(userid);
                if (c != null)
                {
                    sb.Append("<li>");
                    sb.Append(c.Email);
                    sb.Append("</li>");
                }
            }

            sb.Append("</ul>");
            return sb.ToString();
        }

        public void AddUserId(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid)) return;

            var candidate = uid.Trim().ToUpperInvariant();
            var ids = UserIds();

            if (ids.Contains(candidate)) return;

            ids.Add(candidate);
            SaveUserIdsToSettings(ids);
        }

        public void RemoveUserId(string uid)
        {
            if (string.IsNullOrWhiteSpace(uid)) return;

            var candidate = uid.Trim().ToUpperInvariant();
            var ids = UserIds();

            if (!ids.Contains(candidate)) return;

            ids.RemoveAll(x => string.Equals(x, candidate, StringComparison.OrdinalIgnoreCase));
            SaveUserIdsToSettings(ids);
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