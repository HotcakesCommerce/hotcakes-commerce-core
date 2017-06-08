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
using System.Collections.Generic;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Telerik.Web.UI;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class PageConfiguration : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageConfiguration");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            BindUrls();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            try
            {
                SaveUrls();
                HccApp.UpdateCurrentStore();

                msg.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
            catch (Exception ex)
            {
                msg.ShowError(string.Concat(Localization.GetString("ErrorOccured"), "&nbsp; ", ex));
            }

            BindUrls();
        }

        private void BindUrls()
        {
            try
            {
                var showPageUrls = PageUtils.ShowPageUrls();

                var urlControls = new List<Control>
                {
                    divCategoryUrl,
                    divProductsUrl,
                    divCheckoutUrl,
                    divAddressBookUrl,
                    divCartUrl,
                    divOrderHistoryUrl,
                    divProductReviewUrl,
                    divSearchUrl,
                    divWishListUrl
                };
                urlControls.ForEach(c => c.Visible = showPageUrls);

                var sett = HccApp.CurrentStore.Settings.Urls;

                if (showPageUrls)
                {
                    txtCategoryUrl.Text = sett.CategoryUrl;
                    txtProductsUrl.Text = sett.ProductUrl;
                    txtCheckoutUrl.Text = sett.CheckoutUrl;
                    txtAddressBookUrl.Text = sett.AddressBookUrl;
                    txtCartUrl.Text = sett.CartUrl;
                    txtOrderHistoryUrl.Text = sett.OrderHistoryUrl;
                    txtProductReviewUrl.Text = sett.ProductReviewUrl;
                    txtSearchUrl.Text = sett.SearchUrl;
                    txtWishListUrl.Text = sett.WishListUrl;
                }

                var list = TabController.GetTabsBySortOrder(PortalSettings.Current.PortalId,
                    HccApp.CurrentRequestContext.MainContentCulture, true);

                PopulateTabsDropDown(ddlCategoryTab, list, sett.CategoryTabId);
                PopulateTabsDropDown(ddlCheckoutTab, list, sett.CheckoutTabId);
                PopulateTabsDropDown(ddlProductsTab, list, sett.ProductTabId);
                PopulateTabsDropDown(ddlAddressBookTab, list, sett.AddressBookTabId);
                PopulateTabsDropDown(ddlCartTab, list, sett.CartTabId);
                PopulateTabsDropDown(ddlOrderHistoryTab, list, sett.OrderHistoryTabId);
                PopulateTabsDropDown(ddlProductReviewTab, list, sett.ProductReviewTabId);
                PopulateTabsDropDown(ddlSearchTab, list, sett.SearchTabId);
                PopulateTabsDropDown(ddlWishListTab, list, sett.WishListTabId);
            }
            catch (Exception ex)
            {
                msg.ShowError(string.Concat(Localization.GetString("ErrorOccured"), "&nbsp; ", ex));
            }
        }

        private void PopulateTabsDropDown(RadComboBox ddlTabs, List<TabInfo> list, int defaultValue)
        {
            ddlTabs.Items.Clear();
            ddlTabs.ClearSelection();
            ddlTabs.DataSource = list;
            ddlTabs.DataTextField = "IndentedTabName";
            ddlTabs.DataValueField = "TabID";
            ddlTabs.DataBind();

            var li = new RadComboBoxItem
            {
                Value = Convert.ToString(-1),
                Text = Localization.GetString("CreateNewPage")
            };
            ddlTabs.Items.Insert(0, li);
            ddlTabs.SelectedValue = defaultValue.ToString();
        }

        private void SaveUrls()
        {
            var sett = HccApp.CurrentStore.Settings.Urls;

            var showPageUrls = PageUtils.ShowPageUrls();
            if (showPageUrls)
            {
                sett.CategoryUrl = txtCategoryUrl.Text;
                sett.ProductUrl = txtProductsUrl.Text;
                sett.CheckoutUrl = txtCheckoutUrl.Text;
                sett.AddressBookUrl = txtAddressBookUrl.Text;
                sett.CartUrl = txtCartUrl.Text;
                sett.OrderHistoryUrl = txtOrderHistoryUrl.Text;
                sett.ProductReviewUrl = txtProductReviewUrl.Text;
                sett.SearchUrl = txtSearchUrl.Text;
                sett.WishListUrl = txtWishListUrl.Text;
            }

            sett.CategoryTabId = int.Parse(ddlCategoryTab.SelectedValue);
            sett.ProductTabId = int.Parse(ddlProductsTab.SelectedValue);
            sett.CheckoutTabId = int.Parse(ddlCheckoutTab.SelectedValue);
            sett.AddressBookTabId = int.Parse(ddlAddressBookTab.SelectedValue);
            sett.CartTabId = int.Parse(ddlCartTab.SelectedValue);
            sett.OrderHistoryTabId = int.Parse(ddlOrderHistoryTab.SelectedValue);
            sett.ProductReviewTabId = int.Parse(ddlProductReviewTab.SelectedValue);
            sett.SearchTabId = int.Parse(ddlSearchTab.SelectedValue);
            sett.WishListTabId = int.Parse(ddlWishListTab.SelectedValue);

            PageUtils.EnsureTabsExist(sett);
        }
    }
}