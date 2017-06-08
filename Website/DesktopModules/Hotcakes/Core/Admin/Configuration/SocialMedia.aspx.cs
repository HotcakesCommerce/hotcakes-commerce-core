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
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class SocialMedia : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("SocialMediaSettings");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                var currentStore = HccApp.CurrentStore;

                // Loading
                chkUseFaceBook.Checked = currentStore.Settings.FaceBook.UseFaceBook;
                FaceBookAdminsField.Text = currentStore.Settings.FaceBook.Admins;
                FaceBookAppIdField.Text = currentStore.Settings.FaceBook.AppId;

                chkUseTwitter.Checked = currentStore.Settings.Twitter.UseTwitter;
                TwitterHandleField.Text = currentStore.Settings.Twitter.TwitterHandle;
                DefaultTweetTextField.Text = currentStore.Settings.Twitter.DefaultTweetText;

                chkUseGooglePlus.Checked = currentStore.Settings.GooglePlus.UseGooglePlus;

                chkUsePinterest.Checked = currentStore.Settings.Pinterest.UsePinterest;
            }
        }

        protected void btnSave_OnClick(object sender, EventArgs e)
        {
            HccApp.CurrentStore.Settings.FaceBook.UseFaceBook = chkUseFaceBook.Checked;
            HccApp.CurrentStore.Settings.FaceBook.Admins = FaceBookAdminsField.Text;
            HccApp.CurrentStore.Settings.FaceBook.AppId = FaceBookAppIdField.Text;

            HccApp.CurrentStore.Settings.Twitter.UseTwitter = chkUseTwitter.Checked;
            HccApp.CurrentStore.Settings.Twitter.TwitterHandle = TwitterHandleField.Text.Trim()
                .Replace("@", string.Empty);
            HccApp.CurrentStore.Settings.Twitter.DefaultTweetText = DefaultTweetTextField.Text.Trim();

            HccApp.CurrentStore.Settings.GooglePlus.UseGooglePlus = chkUseGooglePlus.Checked;

            HccApp.CurrentStore.Settings.Pinterest.UsePinterest = chkUsePinterest.Checked;

            HccApp.UpdateCurrentStore();

            msg.ShowOk(Localization.GetString("SettingsSuccessful"));
        }
    }
}