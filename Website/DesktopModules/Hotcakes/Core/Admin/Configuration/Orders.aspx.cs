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
using System.Globalization;
using System.Threading;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Orders : BaseAdminPage
    {
        private bool UpdateLastOrderNumber { get; set; }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("Orders");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            txtLastOrderNumber.TextChanged += txtLastOrderNumber_TextChanged;
            ucMessageBox.ClearMessage();
        }

        private void txtLastOrderNumber_TextChanged(object sender, EventArgs e)
        {
            UpdateLastOrderNumber = true;
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Save())
                {
                    ucMessageBox.ShowOk(Localization.GetString("SettingsSuccess"));
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            LocalizeView();

            var store = HccApp.CurrentStore;

            chkSwatches.Checked = store.Settings.ProductEnableSwatches;
            txtOrderLimitQuantity.Text = store.Settings.MaxItemsPerOrder.ToString();
            txtOrderLimitWeight.Text = store.Settings.MaxWeightPerOrder.ToString();
            chkZeroDollarOrders.Checked = store.Settings.AllowZeroDollarOrders;
            chkRequirePhoneNumber.Checked = store.Settings.RequirePhoneNumber;
            txtLastOrderNumber.Text =
                HccApp.AccountServices.Stores.GetLastOrderNumber(HccApp.CurrentStore.Id).ToString();
            chkUseChildChoicesAdjustmentsForBundles.Checked = store.Settings.UseChildChoicesAdjustmentsForBundles;
            chkForceSiteTerms.Checked = store.Settings.ForceTermsAgreement;

            ddlShoppingCartStore.SelectedValue = store.Settings.PreserveCartInSession ? "1" : "0";
            chkSendAbandonedCartEmails.Checked = store.Settings.SendAbandonedCartEmails;
            txtSendAbandonedEmailIn.Text = store.Settings.SendAbandonedEmailIn.ToString();

            txtQuickbooksOrderAccount.Text = store.Settings.QuickbooksOrderAccount;
            txtQuickbooksShippingAccount.Text = store.Settings.QuickbooksShippingAccount;
        }

        private bool Save()
        {
            var store = HccApp.CurrentStore;

            store.Settings.ProductEnableSwatches = chkSwatches.Checked;

            decimal weight = 0;
            decimal.TryParse(txtOrderLimitWeight.Text.Trim(), NumberStyles.Float, Thread.CurrentThread.CurrentUICulture,
                out weight);

            store.Settings.MaxItemsPerOrder = int.Parse(txtOrderLimitQuantity.Text.Trim());
            store.Settings.MaxWeightPerOrder = Math.Round(weight, 3);
            store.Settings.AllowZeroDollarOrders = chkZeroDollarOrders.Checked;

            store.Settings.RequirePhoneNumber = chkRequirePhoneNumber.Checked;

            if (UpdateLastOrderNumber)
            {
                var lastOrderNumber = 0;

                if (int.TryParse(txtLastOrderNumber.Text.Trim(), out lastOrderNumber))
                {
                    HccApp.AccountServices.Stores.SetLastOrderNumber(HccApp.CurrentStore.Id, lastOrderNumber);
                }
            }

            store.Settings.UseChildChoicesAdjustmentsForBundles = chkUseChildChoicesAdjustmentsForBundles.Checked;
            store.Settings.ForceTermsAgreement = chkForceSiteTerms.Checked;

            store.Settings.PreserveCartInSession = ddlShoppingCartStore.SelectedValue == "1";
            store.Settings.SendAbandonedCartEmails = chkSendAbandonedCartEmails.Checked;
            store.Settings.SendAbandonedEmailIn = int.Parse(txtSendAbandonedEmailIn.Text.Trim());

            store.Settings.QuickbooksOrderAccount = txtQuickbooksOrderAccount.Text.Trim();
            store.Settings.QuickbooksShippingAccount = txtQuickbooksShippingAccount.Text.Trim();

            return HccApp.UpdateCurrentStore();
        }

        private void LocalizeView()
        {
            if (ddlShoppingCartStore.Items.Count == 0)
            {
                ddlShoppingCartStore.Items.Add(new ListItem(Localization.GetString("BrowserCookies"), "0"));
                ddlShoppingCartStore.Items.Add(new ListItem(Localization.GetString("BrowserSession"), "1"));
            }
        }
    }
}