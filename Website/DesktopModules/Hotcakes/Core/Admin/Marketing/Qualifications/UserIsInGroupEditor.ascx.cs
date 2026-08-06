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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class UserIsInGroupEditor : BaseQualificationControl
    {
        private UserIsInGroup TypedQualification
        {
            get { return Qualification as UserIsInGroup; }
        }

        protected void btnAddUserIsInGroup_Click(object sender, EventArgs e)
        {
            var q = TypedQualification;
            if (q == null || lstUserIsInGroup == null) return;

            var selected = lstUserIsInGroup.SelectedValue;
            if (string.IsNullOrWhiteSpace(selected)) return;

            q.AddGroup(selected.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvUserIsInGroup_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null) return;
            if (e.Keys == null || e.Keys.Count == 0) return;

            var keyObj = e.Keys[0];
            if (keyObj == null) return;

            var bvin = keyObj as string ?? keyObj.ToString();
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveGroup(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            var q = TypedQualification;
            if (q == null) return;

            var allGroups = HccApp?.ContactServices?.PriceGroups?.FindAll() ?? new List<PriceGroup>();

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in q.CurrentGroupIds() ?? Enumerable.Empty<string>())
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                // Match by trimmed ordinal ignore-case
                var trimmed = bvin.Trim();
                var t = allGroups.FirstOrDefault(y => string.Equals((y.Bvin ?? string.Empty).Trim(), trimmed, StringComparison.OrdinalIgnoreCase));
                if (t != null)
                {
                    item.DisplayName = t.Name ?? trimmed;
                    // remove matched to avoid duplicates in selection list
                    allGroups.Remove(t);
                }

                displayData.Add(item);
            }

            if (lstUserIsInGroup != null)
            {
                lstUserIsInGroup.DataSource = allGroups;
                lstUserIsInGroup.DataValueField = "Bvin";
                lstUserIsInGroup.DataTextField = "Name";
                lstUserIsInGroup.DataBind();
            }

            if (gvUserIsInGroup != null)
            {
                gvUserIsInGroup.DataSource = displayData;
                gvUserIsInGroup.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            return UpdatePromotion();
        }

        protected void btnDeleteUserIsInGroup_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}