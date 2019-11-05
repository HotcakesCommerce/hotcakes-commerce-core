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
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.People
{
    partial class Vendors : BaseAdminPage
    {
        #region Fields

        protected int RowCount;

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("VendorManagement");
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

            LocalizationUtils.LocalizeGridView(gvVendors, Localization);

            if (!Page.IsPostBack)
            {
                InitialBindData();
            }
        }

        protected override void OnLoadComplete(EventArgs e)
        {
            base.OnLoadComplete(e);

            LoadData();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            Response.Redirect("Vendors_edit.aspx?id=0");
        }

        protected void gvVendors_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            var vendorId = (string) gvVendors.DataKeys[e.NewEditIndex].Value;
            Response.Redirect(string.Format("Vendors_edit.aspx?id={0}", vendorId));
        }

        protected void gvVendors_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;

            var vendorId = (string) gvVendors.DataKeys[e.RowIndex].Value;
            HccApp.ContactServices.Vendors.Delete(vendorId);

            LoadData();
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
            txtKeywords.Text = SessionManager.AdminVendorKeywords;
            txtKeywords.Focus();
        }

        private void LoadData()
        {
            var items = HccApp.ContactServices.Vendors.FindAllWithFilter(txtKeywords.Text, ucPager.PageNumber,
                ucPager.PageSize, ref RowCount);

            ucPager.SetRowCount(RowCount);

            gvVendors.DataSource = items;
            gvVendors.DataBind();
        }

        #endregion
    }
}