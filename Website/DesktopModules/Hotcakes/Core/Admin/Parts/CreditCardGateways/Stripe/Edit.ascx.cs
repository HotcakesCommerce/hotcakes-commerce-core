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

namespace Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.Stripe
{
    public partial class Edit : HccCreditCardGatewayPart
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

            var settings = new StripeSettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            txtApiKey.Text = settings.StripeApiKey;
            ddlCurrency.SelectedValue = settings.CurrencyCode;
        }

        public override void SaveData()
        {
            var settings = new StripeSettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            settings.StripeApiKey = txtApiKey.Text.Trim();
            settings.CurrencyCode = ddlCurrency.SelectedValue;

            HccApp.CurrentStore.Settings.PaymentSettingsSet(GatewayId, settings);

            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}