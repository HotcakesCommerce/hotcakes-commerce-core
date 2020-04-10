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
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Modules.PaymentMethods.PaypalExpress
{
    partial class Edit : HccPaymentMethodPart
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

            lstMode.SelectedValue = HccApp.CurrentStore.Settings.PayPal.Mode;

            txtUsername.Text = HccApp.CurrentStore.Settings.PayPal.UserName;
            txtPassword.Text = HccApp.CurrentStore.Settings.PayPal.Password;
            txtSignature.Text = HccApp.CurrentStore.Settings.PayPal.Signature;
            txtPayPalFastSignupEmail.Text = HccApp.CurrentStore.Settings.PayPal.FastSignupEmail;

            var authorizeOnly = HccApp.CurrentStore.Settings.PayPal.ExpressAuthorizeOnly;
            lstCaptureMode.SelectedValue = authorizeOnly ? "1" : "0";

            chkUnconfirmedAddress.Checked = HccApp.CurrentStore.Settings.PayPal.AllowUnconfirmedAddresses;
            chkRequirePayPalAccount.Checked = HccApp.CurrentStore.Settings.PayPal.RequirePayPalAccount;
            ddlCurrency.SelectedValue = HccApp.CurrentStore.Settings.PayPal.Currency;
        }

        public override void SaveData()
        {
            HccApp.CurrentStore.Settings.PayPal.Mode = lstMode.SelectedValue;

            HccApp.CurrentStore.Settings.PayPal.UserName = txtUsername.Text.Trim();
            HccApp.CurrentStore.Settings.PayPal.Password = txtPassword.Text.Trim();
            HccApp.CurrentStore.Settings.PayPal.Signature = txtSignature.Text.Trim();
            HccApp.CurrentStore.Settings.PayPal.FastSignupEmail = txtPayPalFastSignupEmail.Text.Trim();

            var authorizeOnly = lstCaptureMode.SelectedValue == "1";
            HccApp.CurrentStore.Settings.PayPal.ExpressAuthorizeOnly = authorizeOnly;
            HccApp.CurrentStore.Settings.PayPal.AllowUnconfirmedAddresses = chkUnconfirmedAddress.Checked;
            HccApp.CurrentStore.Settings.PayPal.RequirePayPalAccount = chkRequirePayPalAccount.Checked;
            HccApp.CurrentStore.Settings.PayPal.Currency = ddlCurrency.SelectedValue;

            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}