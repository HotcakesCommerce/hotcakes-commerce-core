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

namespace MyCompany.MyGiftCardGateway
{
    public partial class Edit : HccGiftCardGatewayPart
    {
        public override void LoadData()
        {
            var settings = new MyTestGatewaySettings();
            settings.Merge(HccApp.CurrentStore.Settings.GiftCardSettingsGet(GatewayId));

            txtUserName.Text = settings.UserName;
            txtPassword.Text = settings.Password;
        }

        public override void SaveData()
        {
            var settings = new MyTestGatewaySettings();
            settings.Merge(HccApp.CurrentStore.Settings.GiftCardSettingsGet(GatewayId));

            settings.UserName = txtUserName.Text.Trim();
            settings.Password = txtPassword.Text.Trim();

            HccApp.CurrentStore.Settings.GiftCardSettingsSet(GatewayId, settings);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }
    }
}