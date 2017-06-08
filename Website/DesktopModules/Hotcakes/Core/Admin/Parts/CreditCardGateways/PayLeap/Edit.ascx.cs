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

using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment.Gateways;

namespace Hotcakes.Modules.Core.Admin.Parts.CreditCardGateways.PayLeap
{
    public partial class Edit : HccCreditCardGatewayPart
    {
        public override void LoadData()
        {
            var settings = new PayLeapSettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            txtUsernameField.Text = settings.Username;

            if (settings.Password.Length > 0)
            {
                txtPasswordField.Text = "************";
            }

            chkTestMode.Checked = settings.TrainingMode;
            chkDeveloperMode.Checked = settings.DeveloperMode;
            chkEnableTracing.Checked = settings.EnableDebugTracing;
        }

        public override void SaveData()
        {
            var settings = new PayLeapSettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            settings.Username = txtUsernameField.Text.Trim();
            if (txtPasswordField.Text != "************")
            {
                settings.Password = txtPasswordField.Text.Trim();
            }
            settings.TrainingMode = chkTestMode.Checked;
            settings.EnableDebugTracing = chkEnableTracing.Checked;
            settings.DeveloperMode = chkDeveloperMode.Checked;

            HccApp.CurrentStore.Settings.PaymentSettingsSet(GatewayId, settings);

            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}