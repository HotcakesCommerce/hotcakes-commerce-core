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

using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment.Gateways;

namespace Hotcakes.Modules.Core.Modules.CreditCardGateways.Test_Gateway
{
    partial class Edit : HccCreditCardGatewayPart
    {
        public override void LoadData()
        {
            var settings = new TestGatewaySettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            chkAuthorizeFails.Checked = !settings.ResponseForHold;
            chkCaptureFails.Checked = !settings.ResponseForCapture;
            chkChargeFails.Checked = !settings.ResponseForCharge;
            chkRefundFails.Checked = !settings.ResponseForRefund;
            chkVoidFails.Checked = !settings.ResponseForVoid;
        }

        public override void SaveData()
        {
            var settings = new TestGatewaySettings();
            settings.Merge(HccApp.CurrentStore.Settings.PaymentSettingsGet(GatewayId));

            settings.ResponseForCapture = !chkCaptureFails.Checked;
            settings.ResponseForCharge = !chkChargeFails.Checked;
            settings.ResponseForHold = !chkAuthorizeFails.Checked;
            settings.ResponseForRefund = !chkRefundFails.Checked;
            settings.ResponseForVoid = !chkVoidFails.Checked;

            HccApp.CurrentStore.Settings.PaymentSettingsSet(GatewayId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}