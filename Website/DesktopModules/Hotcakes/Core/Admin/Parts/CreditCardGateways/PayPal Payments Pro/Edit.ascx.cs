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

using System.Web.UI.WebControls;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment.Gateways;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Modules.CreditCardGateways.PayPal_Payments_Pro
{
    partial class Edit : HccCreditCardGatewayPart
    {
        public override void LoadData()
        {
            if (ddlCurrency.Items.Count == 0)
            {
                ddlCurrency.Items.Add(new ListItem(Localization.GetString("CurrencyEmptyValue"), string.Empty));
                ddlCurrency.AppendDataBoundItems = true;
                ddlCurrency.DataTextField = "CurrencyEnglishName";
                ddlCurrency.DataValueField = "ISOCurrencySymbol";
                ddlCurrency.DataSource = Currency.GetCurrencyList();
                ddlCurrency.DataBind();
            }

            var settings = new PayPalPaymentsProSettings();

            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            txtUsername.Text = settings.PayPalUserName;
            if (settings.PayPalPassword.Length > 0)
            {
                txtPassword.Text = "**********";
            }

            ddlCurrency.SelectedValue = settings.Currency;

            txtSignature.Text = settings.PayPalSignature;
            rblMode.SelectedValue = settings.PayPalMode;

            chkDebugMode.Checked = settings.DebugMode;
        }

        public override void SaveData()
        {
            var settings = new PayPalPaymentsProSettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            settings.PayPalUserName = txtUsername.Text.Trim();
            if (txtPassword.Text != "**********")
            {
                settings.PayPalPassword = txtPassword.Text.Trim();
            }
            settings.PayPalSignature = txtSignature.Text.Trim();

            settings.Currency = ddlCurrency.SelectedValue;

            settings.PayPalMode = rblMode.SelectedValue;
            settings.DebugMode = chkDebugMode.Checked;

            HccApp.CurrentStore.Settings.PaymentSettingsSet(GatewayId, settings);

            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}