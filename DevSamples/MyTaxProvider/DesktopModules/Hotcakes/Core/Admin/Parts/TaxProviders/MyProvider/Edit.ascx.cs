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
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace MyCompany.MyTaxProvider
{
    public partial class Edit : HccTaxProviderPart
    {
        public override void LoadData()
        {
            var settings = new MyTaxProviderSettings();
            settings.Merge(HccApp.CurrentStore.Settings.TaxProviderSettingsGet(ProviderId));

            txtProviderProp1.Text = settings.TaxProviderProp1;
            txtProviderProp2.Text = settings.TaxProviderProp2;
        }

        public override void SaveData()
        {
            var settings = new MyTaxProviderSettings();

            settings.Merge(HccApp.CurrentStore.Settings.TaxProviderSettingsGet(ProviderId));

            settings.TaxProviderProp1 = txtProviderProp1.Text.Trim();
            settings.TaxProviderProp2 = txtProviderProp2.Text.Trim();

            // Save Settings 
            HccApp.CurrentStore.Settings.TaxProviderSettingsSet(ProviderId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }

        protected void lnkTestConnection_Click(object sender, EventArgs e)
        {
            var settings = new MyTaxProviderSettings();

            settings.Merge(HccApp.CurrentStore.Settings.TaxProviderSettingsGet(ProviderId));

            settings.TaxProviderProp1 = txtProviderProp1.Text.Trim();
            settings.TaxProviderProp2 = txtProviderProp2.Text.Trim();

            // Save Settings 
            HccApp.CurrentStore.Settings.TaxProviderSettingsSet(ProviderId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);

            var myProvider = TaxProviders.Find(ProviderId, HccApp.CurrentStore);

            if (myProvider.TestConnection(HccApp.CurrentRequestContext))
            {
                msg.ShowOk(Localization.GetString("TaxProviderConnectionSuccess"));
            }
            else
            {
                msg.ShowWarning(Localization.GetString("TaxProviderConnectionFail"));
            }
        }
    }
}