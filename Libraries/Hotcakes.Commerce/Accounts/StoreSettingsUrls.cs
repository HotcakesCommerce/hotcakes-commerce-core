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
using System.Collections.Generic;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreSettingsUrls
    {
        public const string DefaultCategoryUrl = "/store/category";
        public const string DefaultProductUrl = "/store/product";
        public const string DefaultCheckoutUrl = "/store/checkout";
        public const string DefaultCartUrl = "/store/cart";
        public const string DefaultProductReviewUrl = "/store/productreview";

        public const string DefaultWishListUrl = "/store/wishlist";
        public const string DefaultSearchUrl = "/store/search";
        public const string DefaultOrderHistoryUrl = "/store/orderhistory";
        public const string DefaultAddressBookUrl = "/store/addressbook";

        public const string DefaultViewsVirtualPath = "~/Portals/_default/HotcakesViews/_default";

        private readonly StoreSettings parent;

        public StoreSettingsUrls(StoreSettings s)
        {
            parent = s;

            UrlConfigs = new List<UrlConfigSettings>();
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = CategoryUrl, TabId = CategoryTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = ProductUrl, TabId = ProductTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = ProductReviewUrl, TabId = ProductReviewTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = CartUrl, TabId = CartTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = CheckoutUrl, TabId = CheckoutTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = WishListUrl, TabId = WishListTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = SearchUrl, TabId = SearchTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = OrderHistoryUrl, TabId = OrderHistoryTabId});
            UrlConfigs.Add(new UrlConfigSettings {CustomUrl = AddressBookUrl, TabId = AddressBookTabId});
        }

        public List<UrlConfigSettings> UrlConfigs { get; private set; }

        public string CategoryUrl
        {
            get { return parent.GetProp("PageCategoryUrl", "", true); }
            set { parent.SetProp("PageCategoryUrl", value, true); }
        }

        public string ProductUrl
        {
            get { return parent.GetProp("PageProductUrl", "", true); }
            set { parent.SetProp("PageProductUrl", value, true); }
        }

        public string ProductReviewUrl
        {
            get { return parent.GetProp("PageProductReviewUrl", "", true); }
            set { parent.SetProp("PageProductReviewUrl", value, true); }
        }

        public string CartUrl
        {
            get { return parent.GetProp("PageCartUrl", "", true); }
            set { parent.SetProp("PageCartUrl", value, true); }
        }

        public string CheckoutUrl
        {
            get { return parent.GetProp("PageCheckoutUrl", "", true); }
            set { parent.SetProp("PageCheckoutUrl", value, true); }
        }

        public string WishListUrl
        {
            get { return parent.GetProp("PageWishListUrl", "", true); }
            set { parent.SetProp("PageWishListUrl", value, true); }
        }

        public string SearchUrl
        {
            get { return parent.GetProp("PageSearchUrl", "", true); }
            set { parent.SetProp("PageSearchUrl", value, true); }
        }

        public string OrderHistoryUrl
        {
            get { return parent.GetProp("PageOrderHistoryUrl", "", true); }
            set { parent.SetProp("PageOrderHistoryUrl", value, true); }
        }

        public string AddressBookUrl
        {
            get { return parent.GetProp("PageAddressBookUrl", "", true); }
            set { parent.SetProp("PageAddressBookUrl", value, true); }
        }

        public int CategoryTabId
        {
            get { return parent.GetPropInt("PageCategoryTabId", true); }
            set { parent.SetProp("PageCategoryTabId", value, true); }
        }

        public int ProductTabId
        {
            get { return parent.GetPropInt("PageProductTabId", true); }
            set { parent.SetProp("PageProductTabId", value, true); }
        }

        public int ProductReviewTabId
        {
            get { return parent.GetPropInt("PageProductReviewTabId", true); }
            set { parent.SetProp("PageProductReviewTabId", value, true); }
        }

        public int CartTabId
        {
            get { return parent.GetPropInt("PageCartTabId", true); }
            set { parent.SetProp("PageCartTabId", value, true); }
        }

        public int CheckoutTabId
        {
            get { return parent.GetPropInt("PageCheckoutTabId", true); }
            set { parent.SetProp("PageCheckoutTabId", value, true); }
        }

        public int WishListTabId
        {
            get { return parent.GetPropInt("PageWishListTabId", true); }
            set { parent.SetProp("PageWishListTabId", value, true); }
        }

        public int SearchTabId
        {
            get { return parent.GetPropInt("PageSearchTabId", true); }
            set { parent.SetProp("PageSearchTabId", value, true); }
        }

        public int OrderHistoryTabId
        {
            get { return parent.GetPropInt("PageOrderHistoryTabId", true); }
            set { parent.SetProp("PageOrderHistoryTabId", value, true); }
        }

        public int AddressBookTabId
        {
            get { return parent.GetPropInt("PageAddressBookTabId", true); }
            set { parent.SetProp("PageAddressBookTabId", value, true); }
        }

        public bool HideSetupWizardWelcome
        {
            get { return parent.GetPropBool("SkipSetupWizard"); }
            set { parent.SetProp("SkipSetupWizard", value); }
        }

        public string WorkflowPluginAssemblyAndType
        {
            get { return parent.GetProp("WorkflowPluginAssemblyAndType"); }
            set { parent.SetProp("WorkflowPluginAssemblyAndType", value); }
        }

        public string ProductIntegrationAssemblyAndType
        {
            get { return parent.GetProp("ProductIntegrationAssemblyAndType"); }
            set { parent.SetProp("ProductIntegrationAssemblyAndType", value); }
        }

        public string CartIntegrationAssemblyAndType
        {
            get { return parent.GetProp("CartIntegrationAssemblyAndType"); }
            set { parent.SetProp("CartIntegrationAssemblyAndType", value); }
        }

        public string CheckoutIntegrationAssemblyAndType
        {
            get { return parent.GetProp("CheckoutIntegrationAssemblyAndType"); }
            set { parent.SetProp("CheckoutIntegrationAssemblyAndType", value); }
        }

        public string ViewsVirtualPath
        {
            get
            {
                var settingValue = parent.GetProp("ViewsVirtualPath");
                if (string.IsNullOrEmpty(settingValue))
                    settingValue = DefaultViewsVirtualPath;
                return settingValue;
            }
            set { parent.SetProp("ViewsVirtualPath", value); }
        }
    }
}