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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Payment.Methods;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Modules.PaymentMethods.Ogone
{
    partial class Edit : HccPaymentMethodPart
    {
        public override void LoadData()
        {
            if (ddlHashAlgorithm.Items.Count == 0)
            {
                var values = Enum.GetNames(typeof (OgoneHashAlgorithm));
                foreach (var value in values)
                {
                    var text = Localization.GetString("HashAlgorithmValue" + value);
                    var listItem = new ListItem(text, value);
                    ddlHashAlgorithm.Items.Add(listItem);
                }
            }
            var settings = new OgoneSettings();
            settings.Merge(HccApp.CurrentStore.Settings.MethodSettingsGet(MethodId));

            txtPaymentServiceProviderId.Text = settings.PaymentServiceProviderId;

            ddlHashAlgorithm.SelectedValue = settings.HashAlgorithm.ToString();
            txtShaInPassPhrase.Text = settings.ShaInPassPhrase;
            txtShaOutPassPhrase.Text = settings.ShaOutPassPhrase;
            txtTemplatePage.Text = settings.TemplatePage;
            chkDebugMode.Checked = settings.DebugMode;
            chkDeveloperMode.Checked = settings.DeveloperMode;
        }

        public override void SaveData()
        {
            var settings = new OgoneSettings();
            settings.Merge(HccApp.CurrentStore.Settings.MethodSettingsGet(MethodId));

            settings.PaymentServiceProviderId = txtPaymentServiceProviderId.Text.Trim();
            settings.ShaInPassPhrase = txtShaInPassPhrase.Text.Trim();
            settings.ShaOutPassPhrase = txtShaOutPassPhrase.Text.Trim();
            settings.HashAlgorithm =
                (OgoneHashAlgorithm) Enum.Parse(typeof (OgoneHashAlgorithm), ddlHashAlgorithm.SelectedValue);
            settings.TemplatePage = txtTemplatePage.Text.Trim();
            settings.DebugMode = chkDebugMode.Checked;
            settings.DeveloperMode = chkDeveloperMode.Checked;

            HccApp.CurrentStore.Settings.MethodSettingsSet(MethodId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}