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
using System.Text;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class Api : BaseAdminPage
    {
        private const string API_KEY_FORMAT = " <a id=\"remove{0}\" href=\"#\" class=\"removeapikey\">{1}</a>";

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("APISettings");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadApiKeys();
                litTimeLimit.Text = HccApp.CurrentStore.Settings.AllowApiToClearUntil.ToString("u");
            }
        }

        private void LoadApiKeys()
        {
            var keys = HccApp.AccountServices.ApiKeys.FindByStoreId(HccApp.CurrentStore.Id);

            var sb = new StringBuilder();
            sb.Append("<ul class=\"apikeys\">");

            foreach (var key in keys)
            {
                sb.Append("<li><pre>");
                sb.Append(key.Key);
                sb.AppendFormat(API_KEY_FORMAT, key.Id, Localization.GetString("Revoke"));
                sb.Append("</pre></li>");
            }

            sb.Append("</ul>");

            litApiKeys.Text = sb.ToString();
        }

        protected void lnkCreateApiKey_Click(object sender, EventArgs e)
        {
            MessageBox1.ClearMessage();

            var key = new ApiKey();
            key.StoreId = HccApp.CurrentStore.Id;
            var k = Guid.NewGuid().ToString();
            k = key.StoreId + "-" + k;
            key.Key = k;

            if (HccApp.AccountServices.ApiKeys.Create(key))
            {
                MessageBox1.ShowOk(Localization.GetString("APIKeySuccess"));
            }
            else
            {
                MessageBox1.ShowWarning(Localization.GetString("APIKeyFailure"));
            }

            LoadApiKeys();
        }

        protected void btnResetClearTime_Click(object sender, EventArgs e)
        {
            HccApp.CurrentStore.Settings.AllowApiToClearUntil = DateTime.UtcNow.AddHours(1);
            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
            litTimeLimit.Text = HccApp.CurrentStore.Settings.AllowApiToClearUntil.ToString("u");
            LoadApiKeys();
        }
    }
}