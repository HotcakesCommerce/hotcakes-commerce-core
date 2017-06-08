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
            var bvin = (string) e.Keys[0];
            q.RemoveUserId(bvin);
            UpdatePromotion();
            LoadQualification();
        }

        private void UserPicker1_UserSelected(object sender, UserSelectedEventArgs e)
        {
            var q = TypedQualification;
            q.AddUserId(e.UserAccount.Bvin);
            UpdatePromotion();
            LoadQualification();
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            UserPicker1.UserSelected += UserPicker1_UserSelected;
            UserPicker1.MessageBox = ucMessageBox;
        }

        public override void LoadQualification()
        {
            var displayData = new List<FriendlyBvinDisplay>();

            foreach (var bvin in TypedQualification.UserIds())
            {
                var item = new FriendlyBvinDisplay();
                item.bvin = bvin;
                item.DisplayName = bvin;

                var c = HccApp.MembershipServices.Customers.Find(item.bvin);
                if (c != null)
                {
                    item.DisplayName = c.Email;
                }
                displayData.Add(item);
            }

            gvUserIs.DataSource = displayData;
            gvUserIs.DataBind();
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