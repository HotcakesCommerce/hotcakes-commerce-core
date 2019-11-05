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
using Hotcakes.Commerce.Taxes.Providers.Avalara;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Modules.TaxProviders.Avalara
{
    partial class Edit : HccTaxProviderPart
    {
        public override void LoadData()
        {
            var settings = new AvalaraSettings();
            settings.Merge(HccApp.CurrentStore.Settings.TaxProviderSettingsGet(ProviderId));


            txtAccount.Text = settings.Account;
            txtLicenseKey.Text = settings.LicenseKey;
            txtCompanyCode.Text = settings.CompanyCode;
            txtUrl.Text = settings.ServiceUrl;
            chkDebug.Checked = settings.DebugMode;
            txtShippingTaxCode.Text = settings.ShippingTaxCode;
            txtTaxExemptCode.Text = settings.TaxExemptCode;
        }

        public override void SaveData()
        {
            var settings = new AvalaraSettings();

            settings.Merge(HccApp.CurrentStore.Settings.TaxProviderSettingsGet(ProviderId));

            settings.Account = txtAccount.Text.Trim();
            settings.LicenseKey = txtLicenseKey.Text.Trim();
            settings.CompanyCode = txtCompanyCode.Text.Trim();
            settings.ServiceUrl = txtUrl.Text.Trim();
            settings.DebugMode = chkDebug.Checked;
            settings.ShippingTaxCode = txtShippingTaxCode.Text.Trim();
            settings.TaxExemptCode = txtTaxExemptCode.Text.Trim();

            // Save Settings 
            HccApp.CurrentStore.Settings.TaxProviderSettingsSet(ProviderId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }

        protected void lnkTestConnection_Click(object sender, EventArgs e)
        {
            var settings = new AvalaraSettings();

            settings.Merge(HccApp.CurrentStore.Settings.TaxProviderSettingsGet(ProviderId));

            settings.Account = txtAccount.Text.Trim();
            settings.LicenseKey = txtLicenseKey.Text.Trim();
            settings.CompanyCode = txtCompanyCode.Text.Trim();
            settings.ServiceUrl = txtUrl.Text.Trim();
            settings.DebugMode = chkDebug.Checked;
            settings.ShippingTaxCode = txtShippingTaxCode.Text.Trim();
            settings.TaxExemptCode = txtTaxExemptCode.Text.Trim();

            // Save Settings 
            HccApp.CurrentStore.Settings.TaxProviderSettingsSet(ProviderId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);


            var avalaraProvider = Commerce.Taxes.Providers.TaxProviders.Find(ProviderId, HccApp.CurrentStore);

            if (avalaraProvider.TestConnection(HccApp.CurrentRequestContext))
            {
                msg.ShowOk(Localization.GetString("AvataxConnectionSuccess"));
            }
            else
            {
                msg.ShowWarning(Localization.GetString("AvataxConnectionFail"));
            }
        }
    }
}