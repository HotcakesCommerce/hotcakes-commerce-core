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
using System.Web;
using System.Web.UI.WebControls;
using DotNetNuke.Entities.Users;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class ViewsManager : BaseAdminPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            Load += ViewsManager_Load;
            btnSaveChanges.Click += btnSaveChanges_Click;
            btnCopyDlgSaveChanges.Click += btnCopyDlgSaveChanges_Click;
            btnUploadDlgSaveChanges.Click += btnUploadDlgSaveChanges_Click;
            PreRender += ViewsManager_PreRender;
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ViewsManager");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        private void ViewsManager_Load(object sender, EventArgs e)
        {
            LocalizeView();
        }

        private void btnSaveChanges_Click(object sender, EventArgs e)
        {
            var urlStoreSettings = HccApp.CurrentStore.Settings.Urls;

            urlStoreSettings.ViewsVirtualPath = ddlViewSets.SelectedValue;

            HccApp.UpdateCurrentStore();

            msg.ShowOk(Localization.GetString("SettingsSuccessful"));
        }

        private void btnCopyDlgSaveChanges_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var user = UserController.Instance.GetCurrentUserInfo();

                string baseVirtualPath;
                if (user.IsSuperUser && chbCopyToHost.Checked)
                    baseVirtualPath = PathHelper.ViewsVirtualPath;
                else
                    baseVirtualPath = DnnPathHelper.GetViewsVirtualPath();

                var virtualPath = VirtualPathUtility.Combine(baseVirtualPath, txtViewSetName.Text);

                var sourceDirectory = new DirectoryInfo(Server.MapPath(ddlViewSets.SelectedValue));
                var targetDirectory = new DirectoryInfo(Server.MapPath(virtualPath));

                if (targetDirectory.Exists)
                {
                    msg.ShowError(Localization.GetString("ExistingViewset"));
                }
                else
                {
                    CopyFilesRecursively(sourceDirectory, targetDirectory);
                    msg.ShowOk(string.Format(Localization.GetString("ViewsetCreated"), targetDirectory.FullName));
                }
            }
        }

        private void btnUploadDlgSaveChanges_Click(object sender, EventArgs e)
        {
            try
            {
                if (fuViewSet.HasFile)
                {
                    var tempFile = Path.Combine(Path.GetTempPath(), Path.GetTempFileName());
                    fuViewSet.PostedFile.SaveAs(tempFile);

                    var user = UserController.Instance.GetCurrentUserInfo();
                    string baseVirtualPath;
                    if (user.IsSuperUser && chbUploadToHost.Checked)
                        baseVirtualPath = PathHelper.ViewsVirtualPath;
                    else
                        baseVirtualPath = DnnPathHelper.GetViewsVirtualPath();

                    var setVirtualPath = VirtualPathUtility.Combine(baseVirtualPath,
                        Path.GetFileNameWithoutExtension(fuViewSet.FileName));
                    var setPath = Server.MapPath(setVirtualPath);
                    ZipUtils.UnZipFiles(tempFile, setPath);

                    msg.ShowOk(Localization.GetString("ViewsetInstalled"));
                }
                else
                    msg.ShowWarning(Localization.GetString("rfvUploadViewset.ErrorMessage"));
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                msg.ShowError(Localization.GetString("FailedToUpload"));
            }
        }

        private void ViewsManager_PreRender(object sender, EventArgs e)
        {
            var urlStoreSettings = HccApp.CurrentStore.Settings.Urls;

            FillViewSets();

            ddlViewSets.SelectedValue = urlStoreSettings.ViewsVirtualPath;
            if (urlStoreSettings.ViewsVirtualPath == StoreSettingsUrls.DefaultViewsVirtualPath)
                lblLocation.Text = Server.MapPath(urlStoreSettings.ViewsVirtualPath);
            else
                lblLocation.Text = Localization.GetString("ReadOnly");
        }

        private void FillViewSets()
        {
            ddlViewSets.Items.Clear();

            var hostViewsDir = new DirectoryInfo(Server.MapPath(PathHelper.ViewsVirtualPath));
            var hostViewSets = hostViewsDir.GetDirectories();

            foreach (var hostViewSet in hostViewSets)
            {
                var item = new ListItem();
                item.Text = string.Format(Localization.GetString("Host"), hostViewSet.Name);
                item.Value = VirtualPathUtility.Combine(PathHelper.ViewsVirtualPath, hostViewSet.Name);
                ddlViewSets.Items.Add(item);
            }

            var siteViewsPath = DnnPathHelper.GetViewsVirtualPath();
            var siteViewsDir = new DirectoryInfo(Server.MapPath(siteViewsPath));
            if (siteViewsDir.Exists)
            {
                var siteViewSets = siteViewsDir.GetDirectories();

                foreach (var siteViewSet in siteViewSets)
                {
                    var item = new ListItem();
                    item.Text = string.Format(Localization.GetString("Site"), siteViewSet.Name);
                    item.Value = VirtualPathUtility.Combine(siteViewsPath, siteViewSet.Name);
                    ddlViewSets.Items.Add(item);
                }
            }
        }

        private static void CopyFilesRecursively(DirectoryInfo source, DirectoryInfo target)
        {
            foreach (var dir in source.GetDirectories())
                CopyFilesRecursively(dir, target.CreateSubdirectory(dir.Name));
            foreach (var file in source.GetFiles())
                file.CopyTo(Path.Combine(target.FullName, file.Name), true);
        }

        private void LocalizeView()
        {
            rfvViewsetName.ErrorMessage = Localization.GetString("rfvViewsetName.ErrorMessage");
            rfvUploadViewset.ErrorMessage = Localization.GetString("rfvUploadViewset.ErrorMessage");
        }
    }
}