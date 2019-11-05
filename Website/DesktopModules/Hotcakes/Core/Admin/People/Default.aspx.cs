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
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Default : BaseAdminPage
    {
        private const string AVATAR_FORMAT = "<a href=\"{0}\"><img src=\"{1}\" alt=\"{2}\" /></a>";
        protected int RowCount;

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("CustomerManagement");
            CurrentTab = AdminTabType.People;
            ValidateCurrentUserHasPermission(SystemPermissions.PeopleView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            btnGo.Click += (s, a) => ucPager.ResetPageNumber();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                InitialBindData();
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            LoadUsers();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("Users_Edit.aspx");
        }

        protected void gvCustomers_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            var customerId = (string) gvCustomers.DataKeys[e.NewEditIndex].Value;

            Response.Redirect(GetEditUrl(customerId));
        }

        protected void gvCustomers_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;

            var customerId = (string) gvCustomers.DataKeys[e.RowIndex].Value;
            HccApp.MembershipServices.Customers.Delete(customerId);

            LoadUsers();
        }

        protected void gvCustomers_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Avatar");
                e.Row.Cells[1].Text = Localization.GetString("Name");
                e.Row.Cells[2].Text = Localization.GetString("Email");
            }
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmDelete"));
        }

        #endregion

        #region Implementation

        private void InitialBindData()
        {
            txtKeywords.Text = SessionManager.AdminCustomerKeywords;
            txtKeywords.Focus();
        }

        private void LoadUsers()
        {
            var accounts = HccApp.MembershipServices.Customers.FindByFilter(txtKeywords.Text, ucPager.PageNumber,
                ucPager.PageSize, ref RowCount);

            ucPager.SetRowCount(RowCount);

            gvCustomers.DataSource = accounts;
            gvCustomers.DataBind();
        }

        protected string GetDisplayName(object FirstName, object LastName)
        {
            if (FirstName != null || LastName != null)
            {
                var displayName = string.Empty;
                displayName = FirstName == null ? string.Empty : FirstName.ToString();

                if (!string.IsNullOrEmpty(displayName))
                {
                    displayName += " ";
                }

                displayName += LastName == null ? string.Empty : LastName.ToString();

                if (string.IsNullOrEmpty(displayName))
                {
                    return Localization.GetString("NoDisplayName");
                }

                return displayName;
            }

            return Localization.GetString("NoDisplayName");
        }

        protected string GetAvatar(object CustomerId, object EmailAddress)
        {
            var email = EmailAddress == null ? string.Empty : EmailAddress.ToString();

            var avatar = GravatarHelper.GetGravatarUrlForEmailWithSize(email, 40);

            return string.Format(AVATAR_FORMAT, GetEditUrl(CustomerId.ToString()), avatar, email);
        }

        private string GetEditUrl(string CustomerId)
        {
            return string.Format("Users_Edit.aspx?id={0}", CustomerId);
        }

        #endregion
    }
}