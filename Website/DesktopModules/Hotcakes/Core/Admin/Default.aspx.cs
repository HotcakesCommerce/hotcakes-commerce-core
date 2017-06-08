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
using System.Web;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Payment.Methods;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment.Gateways;

namespace Hotcakes.Modules.Core.Admin
{
    /// <summary>
    ///     This is the default page of all of the administration area, where the dashboard is shown to store owners and
    ///     merchants.
    /// </summary>
    partial class Default : BaseAdminPage
    {
        #region Private Members

        private const string URL_TEMPLATE =
            "~/DesktopModules/Hotcakes/Core/Admin/Configuration/Payment_Edit.aspx?id={0}";

        private const string EXIT = "EXIT";

        private const string LOCALIZATION_DASHBOARD = "Dashboard";
        private const string LOCALIZATION_GATEWAY_MESSAGE = "TextGatewayMessage";

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString(LOCALIZATION_DASHBOARD);
            CurrentTab = AdminTabType.Dashboard;

            // TFS 33658
            // validate permissions for multiple roles: catalog, orders, admin, mobile
            if (!HasCurrentUserPermission(SystemPermissions.CatalogView) &&
                !HasCurrentUserPermission(SystemPermissions.OrdersView) &&
                !HasCurrentUserPermission(SystemPermissions.SettingsView) &&
                !HasCurrentUserPermission(SystemPermissions.PluginView))
            {
                Response.Redirect(HccApp.MembershipServices.GetLoginPagePath());
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var settings = HccApp.CurrentStore.Settings;
            if (settings.PaymentMethodsEnabled.Contains(PaymentMethods.CreditCardId)
                && PaymentGateways.CurrentPaymentProcessor(HccApp.CurrentStore).Id == new TestGateway().Id)
            {
                var url = VirtualPathUtility.ToAbsolute(string.Format(URL_TEMPLATE, CreditCard.Id()));
                var message = string.Format(Localization.GetString(LOCALIZATION_GATEWAY_MESSAGE), url);
                ucMessageBox.ShowWarning(message);
            }

            ucStep0Dashboard.EditingComplete += ucStep0Dashboard_EditingComplete;
        }

        private void ucStep0Dashboard_EditingComplete(object sender, HccPartEventArgs e)
        {
            if (string.IsNullOrEmpty(e.Info))
                Response.Redirect(string.Format(WizardPageStepPath, 1));

            if (string.Equals(e.Info, EXIT))
            {
                ucStep0Dashboard.Visible = false;
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var sett = HccApp.CurrentStore.Settings.Urls;
            if (sett.HideSetupWizardWelcome)
            {
                ucStep0Dashboard.Visible = false;
            }
        }

        #endregion
    }
}