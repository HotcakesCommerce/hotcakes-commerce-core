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
    public class UserIsInGroup : PromotionQualificationBase
    {
        public UserIsInGroup()
        {
            ProcessingCost = RelativeProcessingCost.Lower;
        }

        public override Guid TypeId
        {
            get { return new Guid(TypeIdUserIsInGroup); }
        }

        public List<string> CurrentGroupIds()
        {
            var result = new List<string>();
            var all = GetSetting("groupids");
            var parts = all.Split(',');
            foreach (var s in parts)
            {
                if (s != string.Empty)
                {
                    result.Add(s.Trim().ToLowerInvariant());
                }
            }
            return result;
        }

        private void SaveGroupIdsToSettings(List<string> groupids)
        {
            var all = string.Empty;
            foreach (var s in groupids)
            {
                if (s != string.Empty)
                {
                    all += s.Trim().ToLowerInvariant() + ",";
                }
            }
            all = all.TrimEnd(',');
            SetSetting("groupids", all);
        }

        public override string FriendlyDescription(HotcakesApplication app)
        {
            var result = "When Current User Is In Group:<ul>";

            foreach (var gid in CurrentGroupIds())
            {
                var g = app.ContactServices.PriceGroups.Find(gid);
                if (g != null)
                {
                    result += "<li>" + g.Name + "</li>";
                }
            }
            result += "</ul>";
            return result;
        }

        public void AddGroup(string groupid)
        {
            var _Groups = CurrentGroupIds();

            var possible = groupid.Trim().ToLowerInvariant();
            if (possible == string.Empty) return;
            if (_Groups.Contains(possible)) return;
            _Groups.Add(possible);
            SaveGroupIdsToSettings(_Groups);
        }

        public void RemoveGroup(string groupid)
        {
            var _Groups = CurrentGroupIds();
            if (_Groups.Contains(groupid.Trim().ToLowerInvariant()))
            {
                _Groups.Remove(groupid.Trim().ToLowerInvariant());
                SaveGroupIdsToSettings(_Groups);
            }
        }

        public override bool MeetsQualification(PromotionContext context, PromotionQualificationMode mode)
        {
            if (context == null) return false;
            if (context.CurrentCustomer == null) return false;
            if (context.CurrentCustomer.PricingGroupId == string.Empty) return false;

            if (CurrentGroupIds().Contains(context.CurrentCustomer.PricingGroupId.Trim().ToLowerInvariant()))
            {
                return true;
            }

            return false;
        }
    }
}