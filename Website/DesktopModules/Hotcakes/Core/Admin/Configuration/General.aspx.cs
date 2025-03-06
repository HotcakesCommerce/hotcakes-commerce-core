#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.IO;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class General : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("StoreNameLogo");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                txtSiteName.Text = HccApp.CurrentStore.Settings.FriendlyName;
                txtLogoText.Text = HccApp.CurrentStore.Settings.LogoText;
                chkUseLogoImage.Checked = HccApp.CurrentStore.Settings.UseLogoImage;
                chkUseSSL.Checked = HccApp.CurrentStore.Settings.ForceAdminSSL;

                UpdateLogoImage();
            }
        }

        private void UpdateLogoImage()
        {
            ucStoreLogo.ImageUrl = HccApp.CurrentStore.Settings.LogoImageFullUrl(HccApp, Page.Request.IsSecureConnection);

            if (string.IsNullOrWhiteSpace(HccApp.CurrentStore.Settings.LogoImage))
            {
                ucStoreLogo.ImageUrl = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/MissingImage.png");
            }

            HccApp.UpdateCurrentStore();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (Save())
                {
                    ucMessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
                }
            }
        }

        private bool Save()
        {
            var result = false;

            HccApp.CurrentStore.Settings.UseLogoImage = chkUseLogoImage.Checked;
            HccApp.CurrentStore.Settings.FriendlyName = txtSiteName.Text.Trim();
            HccApp.CurrentStore.Settings.LogoText = txtLogoText.Text.Trim();

            result = true;

            if (!string.IsNullOrEmpty(ucStoreLogo.FileName))
            {
                var fileName = Path.GetFileNameWithoutExtension(ucStoreLogo.FileName);
                var ext = Path.GetExtension(ucStoreLogo.FileName);

                fileName = Text.CleanFileName(fileName);

                if (DiskStorage.UploadStoreImage(HccApp.CurrentStore, ucStoreLogo.TempImagePath, ucStoreLogo.FileName))
                {
                    HccApp.CurrentStore.Settings.LogoImage = fileName + ext;
                }
            }

            HccApp.CurrentStore.Settings.ForceAdminSSL = chkUseSSL.Checked;

            HccApp.UpdateCurrentStore();

            return result;
        }
    }
}