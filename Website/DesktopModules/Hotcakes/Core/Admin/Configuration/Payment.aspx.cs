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
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Payment : BaseAdminPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LoadMethods();
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PaymentMethods");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        private void LoadMethods()
        {
            gvPaymentMethods.DataSource = PaymentMethods.AvailableMethods();
            gvPaymentMethods.DataBind();
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            SaveChanges();
            LoadMethods();
            MessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
        }

        private void SaveChanges()
        {
            var newList = new List<string>();
            for (var i = 0; i <= gvPaymentMethods.Rows.Count - 1; i++)
            {
                var chkEnabled = gvPaymentMethods.Rows[i].FindControl("chkEnabled") as CheckBox;
                if (chkEnabled != null && chkEnabled.Checked)
                {
                    var paymentMethodId = (string) gvPaymentMethods.DataKeys[i].Value;
                    newList.Add(paymentMethodId);
                }
            }

            HccApp.CurrentStore.Settings.PaymentMethodsEnabled = newList;
            HccApp.UpdateCurrentStore();
        }

        protected void gvPaymentMethods_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var method = (PaymentMethod) e.Row.DataItem;

                var chkEnabled = e.Row.FindControl("chkEnabled") as CheckBox;
                var lblMethodName = e.Row.FindControl("lblMethodName") as Label;
                var btnEdit = e.Row.FindControl("btnEdit") as LinkButton;

                var contains = HccApp.CurrentStore.Settings.PaymentMethodsEnabled.Contains(method.MethodId);
                chkEnabled.Checked = contains;

                if (string.IsNullOrEmpty(LocalizationUtils.GetPaymentMethodFriendlyName(method.MethodName)))
                {
                    lblMethodName.Text = method.MethodName;
                }
                else
                {
                    lblMethodName.Text = LocalizationUtils.GetPaymentMethodFriendlyName(method.MethodName);
                }

                var editor = HccPartController.LoadPaymentMethodEditor(method.MethodName, Page) as HccPart;
                var editorExists = editor != null;
                btnEdit.Visible = editorExists;
            }
        }

        protected void gvPaymentMethods_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var paymentMethodId = (string) gvPaymentMethods.DataKeys[e.NewEditIndex].Value;
            Response.Redirect("Payment_Edit.aspx?id=" + paymentMethodId);
        }
    }
}