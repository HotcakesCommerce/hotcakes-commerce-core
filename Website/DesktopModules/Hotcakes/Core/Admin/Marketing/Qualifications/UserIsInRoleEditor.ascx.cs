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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Dnn.Marketing.Qualifications;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class UserIsInRoleEditor : BaseQualificationControl
    {
        private UserIsInRole TypedQualification
        {
            get { return Qualification as UserIsInRole; }
        }

        protected void btnAddRole_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(lstRoles.SelectedValue))
            {
                var q = TypedQualification;
                q.AddRole(lstRoles.SelectedValue);
            }
            UpdatePromotion();
            LoadQualification();
        }

        protected void gvRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var q = TypedQualification;
            var bvin = (int) e.Keys[0];
            q.RemoveRole(bvin.ToString());
            UpdatePromotion();
            LoadQualification();
        }

        public override void LoadQualification()
        {
            var roles = TypedQualification.GetAllRoles();
            var selRoles = TypedQualification.GetRoles();

            foreach (var sb in selRoles)
            {
                var rb = roles.FirstOrDefault(b => b.RoleID == sb.RoleID);

                if (rb != null)
                {
                    roles.Remove(rb);
                }
            }

            if (roles.Count == 0)
                btnAddRole.Enabled = false;
            else
                btnAddRole.Enabled = true;

            lstRoles.DataSource = roles;
            lstRoles.DataValueField = "RoleID";
            lstRoles.DataTextField = "RoleName";
            lstRoles.DataBind();

            gvRoles.DataSource = selRoles;
            gvRoles.DataBind();
        }

        public override bool SaveQualification()
        {
            return UpdatePromotion();
        }

        protected void btnDeleteRole_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}