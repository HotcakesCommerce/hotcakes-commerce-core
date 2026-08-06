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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Controls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class UserIdEditor : BaseQualificationControl
    {
        private UserIs TypedQualification
        {
            get { return Qualification as UserIs; }
        }

        protected void gvUserIs_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            if (q == null) return;
            if (e?.Keys == null || e.Keys.Count == 0) return;

            var keyObj = e.Keys[0];
            if (keyObj == null) return;

            var bvin = keyObj as string ?? keyObj.ToString();
            if (string.IsNullOrWhiteSpace(bvin)) return;

            q.RemoveUserId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        private void UserPicker1_UserSelected(object sender, UserSelectedEventArgs e)
        {
            if (TypedQualification == null) return;
            if (e == null || e.UserAccount == null) return;
            if (string.IsNullOrWhiteSpace(e.UserAccount.Bvin)) return;

            TypedQualification.AddUserId(e.UserAccount.Bvin.Trim());
            UpdatePromotion();
            LoadQualification();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (UserPicker1 != null)
            {
                UserPicker1.UserSelected += UserPicker1_UserSelected;
                UserPicker1.MessageBox = ucMessageBox;
            }
        }

        public override void LoadQualification()
        {
            var q = TypedQualification;
            if (q == null)
            {
                if (gvUserIs != null) gvUserIs.DataSource = new List<FriendlyBvinDisplay>();
                if (gvUserIs != null) gvUserIs.DataBind();
                return;
            }

            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in q.UserIds())
            {
                if (string.IsNullOrWhiteSpace(bvin)) continue;

                var item = new FriendlyBvinDisplay
                {
                    bvin = bvin,
                    DisplayName = bvin
                };

                var c = HccApp?.MembershipServices?.Customers?.Find(item.bvin);
                if (c != null)
                {
                    item.DisplayName = c.Email ?? item.DisplayName;
                }
                displayData.Add(item);
            }

            if (gvUserIs != null)
            {
                gvUserIs.DataSource = displayData;
                gvUserIs.DataBind();
            }
        }

        public override bool SaveQualification()
        {
            return UpdatePromotion();
        }

        protected void btnDeleteUserIs_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}