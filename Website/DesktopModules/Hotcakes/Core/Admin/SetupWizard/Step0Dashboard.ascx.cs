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
using DotNetNuke.Common.Utilities;
using DotNetNuke.Entities.Portals;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.SetupWizard
{
    public partial class Step0Dashboard : HccPart
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            BindUrls();
            LoadDisplaySettings();

            if (IsPagesNotInstalled())
            {
                btnSkip.Attributes["href"] = "#SkipDlg";
                btnSkip.CssClass = "hcSecondaryAction hcOpenPopup";
            }
        }

        protected void btnStart_Click(object sender, EventArgs e)
        {
            EnsureStore();

            SaveDisplaySettings();
            HccApp.UpdateCurrentStore();
            NotifyFinishedEditing();
        }

        protected void btnSkip_Click(object sender, EventArgs e)
        {
            SaveDisplaySettings();
            HccApp.UpdateCurrentStore();
            NotifyFinishedEditing("EXIT");
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                EnsureStore();

                CreatePages();

                SaveDisplaySettings();
                HccApp.UpdateCurrentStore();
                NotifyFinishedEditing("EXIT");
            }
        }

        protected void btnClose_Click(object sender, EventArgs e)
        {
            EnsureStore();

            SaveDisplaySettings();
            HccApp.UpdateCurrentStore();
            NotifyFinishedEditing("EXIT");
        }

        #region Implementation

        private bool IsPagesNotInstalled()
        {
            if (HccApp.CurrentStore == null)
                return true;

            var sett = HccApp.CurrentStore.Settings.Urls;
            return sett.CategoryTabId == Null.NullInteger;
        }

        private void LoadDisplaySettings()
        {
            if (HccApp.CurrentStore == null)
                return;

            var sett = HccApp.CurrentStore.Settings.Urls;
            chkDontShowAgain.Checked = sett.HideSetupWizardWelcome;
        }

        private void SaveDisplaySettings()
        {
            var sett = HccApp.CurrentStore.Settings.Urls;
            sett.HideSetupWizardWelcome = chkDontShowAgain.Checked;
        }

        private void BindUrls()
        {
            var showPageUrls = PageUtils.ShowPageUrls();
            pnlPageUrls.Visible = showPageUrls;

            if (showPageUrls)
            {
                if (HccApp.CurrentStore != null)
                {
                    var sett = HccApp.CurrentStore.Settings.Urls;
                    txtCategoryUrl.Text = sett.CategoryUrl;
                    txtProductsUrl.Text = sett.ProductUrl;
                    txtCheckoutUrl.Text = sett.CheckoutUrl;
                }
                else
                {
                    txtCategoryUrl.Text = StoreSettingsUrls.DefaultCategoryUrl;
                    txtProductsUrl.Text = StoreSettingsUrls.DefaultProductUrl;
                    txtCheckoutUrl.Text = StoreSettingsUrls.DefaultCheckoutUrl;
                }
            }
        }

        private void CreatePages()
        {
            var sett = HccApp.CurrentStore.Settings.Urls;

            var showPageUrls = PageUtils.ShowPageUrls();
            if (showPageUrls)
            {
                sett.CategoryUrl = txtCategoryUrl.Text;
                sett.ProductUrl = txtProductsUrl.Text;
                sett.CheckoutUrl = txtCheckoutUrl.Text;
                sett.AddressBookUrl = StoreSettingsUrls.DefaultAddressBookUrl;
                sett.CartUrl = StoreSettingsUrls.DefaultCartUrl;
                sett.OrderHistoryUrl = StoreSettingsUrls.DefaultOrderHistoryUrl;
                sett.ProductReviewUrl = StoreSettingsUrls.DefaultProductReviewUrl;
                sett.SearchUrl = StoreSettingsUrls.DefaultSearchUrl;
                sett.WishListUrl = StoreSettingsUrls.DefaultWishListUrl;
            }

            PageUtils.EnsureTabsExist(sett);
        }

        private void EnsureStore()
        {
            if (HccApp.CurrentStore == null)
            {
                var store = HccApp.AccountServices.CreateAndSetupStore();
                if (store == null)
                    throw new CreateStoreException("Could not create store");

                HccApp.CurrentStore = store;

                HotcakesController.AddAdminPage(PortalSettings.Current.PortalId);

                var systemColumnsFilePath =
                    "~/DesktopModules/Hotcakes/Core/Admin/Parts/ContentBlocks/SystemColumnsData.xml";
                HccApp.ContentServices.Columns.CreateFromTemplateFile(systemColumnsFilePath);

                HccApp.OrderServices.EnsureDefaultZones(store.Id);
            }
        }

        #endregion
    }
}