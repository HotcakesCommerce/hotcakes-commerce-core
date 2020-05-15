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
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    public partial class Users_Roles : BaseCustomerPage
    {
        #region Properties

        protected List<string> SelectedRoles
        {
            get { return ViewState["SelectedRoles"] as List<string> ?? new List<string>(); }
            set { ViewState["SelectedRoles"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            PageTitle = Localization.GetString("UserProductRoles");

            InitNavMenu(ucNavMenu);

            gvRoles.RowDeleting += gvRoles_RowDeleting;
            btnAdd.Click += btnAdd_Click;
            btnSendNotification.Click += btnSendNotification_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadRoles();
            }
        }

        private void btnSendNotification_Click(object sender, EventArgs e)
        {
            var roleNames = gvRoles.Rows.OfType<GridViewRow>().Where(r =>
            {
                var cbSelect = r.FindControl("cbSelect") as CheckBox;
                return cbSelect.Checked;
            })
                .Select(r => gvRoles.DataKeys[r.RowIndex].Value as string)
                .ToArray();

            if (roleNames.Length > 0)
            {
                HccApp.ContactServices.SendNewRolesAssignment(Customer, roleNames);

                ucMessageBox.ShowOk(Localization.GetString("NotificationSuccessful"));

                SelectedRoles = null;
                LoadRoles();
            }
            else
            {
                ucMessageBox.ShowWarning(Localization.GetString("SelectRolesError"));
            }
        }

        private void btnAdd_Click(object sender, EventArgs e)
        {
            var roleName = ddlRoles.SelectedValue;
            var userId = Convert.ToInt32(CustomerId);

            DnnUserController.Instance.AddUserRole(DnnGlobal.Instance.GetPortalId(), userId, roleName);

            var roles = SelectedRoles;

            roles.Add(roleName);
            SelectedRoles = roles;

            LoadRoles();
        }

        private void gvRoles_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var roleName = e.Keys[0] as string;
            var userId = Convert.ToInt32(CustomerId);

            DnnUserController.Instance.RemoveUserRole(DnnGlobal.Instance.GetCurrentPortalSettings(), userId, roleName);

            LoadRoles();
        }

        protected void gvRoles_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = Localization.GetString("Role");
            }
        }

        #endregion

        #region Implementation

        protected bool IsChecked(IDataItemContainer cont)
        {
            var roleName = DataBinder.Eval(cont.DataItem, "RoleName") as string;

            return SelectedRoles.Contains(roleName);
        }

        private void LoadRoles()
        {
            var userId = Convert.ToInt32(CustomerId);
            var user = DnnUserController.Instance.GetUser(DnnGlobal.Instance.GetPortalId(), userId);

            var allRoles = HccApp.CatalogServices.CatalogRoles.FindAllRoleNames();

            ddlRoles.DataTextField = "RoleName";
            ddlRoles.DataValueField = "RoleName";

            ddlRoles.DataSource = DnnUserController.Instance.GetRoles()
                .Where(r => allRoles.Any(sr => sr == r.RoleName))
                .Where(r => !user.Roles.Any(sr => sr == r.RoleName));

            ddlRoles.DataBind();

            var roles = user.Roles.Where(r => allRoles.Any(sr => sr == r)).Select(r => new {RoleName = r});

            gvRoles.DataSource = roles;
            gvRoles.DataBind();

            btnSendNotification.Visible = roles.Any();

            ddlRoles.Enabled = (ddlRoles.Items != null && ddlRoles.Items.Count > 0);
            btnAdd.Enabled = (ddlRoles.Items != null && ddlRoles.Items.Count > 0);
        }

        #endregion
    }
}