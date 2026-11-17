#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    /// <summary>Provides store page URL, page identifier, integration, and view-path settings.</summary>
    /// <remarks>Settings are owned by the current store through <see cref="StoreSettings"/>. URL values use localized persistence, while setters stage changes in the parent façade for later persistence. The constructor also initializes <see cref="UrlConfigs"/> from the current values.</remarks>
    [Serializable]
    public class StoreSettingsUrls
    {
        /// <summary>The default category page path: <c>/store/category</c>.</summary>
        public const string DefaultCategoryUrl = "/store/category";
        /// <summary>The default product page path: <c>/store/product</c>.</summary>
        public const string DefaultProductUrl = "/store/product";
        /// <summary>The default checkout page path: <c>/store/checkout</c>.</summary>
        public const string DefaultCheckoutUrl = "/store/checkout";
        /// <summary>The default cart page path: <c>/store/cart</c>.</summary>
        public const string DefaultCartUrl = "/store/cart";
        /// <summary>The default product-review page path: <c>/store/productreview</c>.</summary>
        public const string DefaultProductReviewUrl = "/store/productreview";

        /// <summary>The default wish-list page path: <c>/store/wishlist</c>.</summary>
        public const string DefaultWishListUrl = "/store/wishlist";
        /// <summary>The default search page path: <c>/store/search</c>.</summary>
        public const string DefaultSearchUrl = "/store/search";
        /// <summary>The default order-history page path: <c>/store/orderhistory</c>.</summary>
        public const string DefaultOrderHistoryUrl = "/store/orderhistory";
        /// <summary>The default address-book page path: <c>/store/addressbook</c>.</summary>
        public const string DefaultAddressBookUrl = "/store/addressbook";

        /// <summary>The default views virtual path: <c>~/Portals/_default/HotcakesViews/_default</c>.</summary>
        public const string DefaultViewsVirtualPath = "~/Portals/_default/HotcakesViews/_default";

        private readonly StoreSettings parent;

        /// <summary>Initializes URL settings and the URL configuration list for <paramref name="s"/>.</summary>
        /// <param name="s">The owning store settings façade.</param>
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

        /// <summary>Gets the URL configuration entries initialized from this façade's page URLs and tab IDs.</summary>
        /// <value>A mutable list of <see cref="UrlConfigSettings"/> entries.</value>
        public List<UrlConfigSettings> UrlConfigs { get; private set; }

        /// <summary>Gets or sets the category page URL.</summary>
        /// <value>The localized <c>PageCategoryUrl</c> setting; unset values default to an empty string.</value>
        public string CategoryUrl
        {
            get { return parent.GetProp("PageCategoryUrl", "", true); }
            set { parent.SetProp("PageCategoryUrl", value, true); }
        }

        /// <summary>Gets or sets the product page URL.</summary>
        /// <value>The localized <c>PageProductUrl</c> setting; unset values default to an empty string.</value>
        public string ProductUrl
        {
            get { return parent.GetProp("PageProductUrl", "", true); }
            set { parent.SetProp("PageProductUrl", value, true); }
        }

        /// <summary>Gets or sets the product-review page URL.</summary>
        /// <value>The localized <c>PageProductReviewUrl</c> setting; unset values default to an empty string.</value>
        public string ProductReviewUrl
        {
            get { return parent.GetProp("PageProductReviewUrl", "", true); }
            set { parent.SetProp("PageProductReviewUrl", value, true); }
        }

        /// <summary>Gets or sets the cart page URL.</summary>
        /// <value>The localized <c>PageCartUrl</c> setting; unset values default to an empty string.</value>
        public string CartUrl
        {
            get { return parent.GetProp("PageCartUrl", "", true); }
            set { parent.SetProp("PageCartUrl", value, true); }
        }

        /// <summary>Gets or sets the checkout page URL.</summary>
        /// <value>The localized <c>PageCheckoutUrl</c> setting; unset values default to an empty string.</value>
        public string CheckoutUrl
        {
            get { return parent.GetProp("PageCheckoutUrl", "", true); }
            set { parent.SetProp("PageCheckoutUrl", value, true); }
        }

        /// <summary>Gets or sets the wish-list page URL.</summary>
        /// <value>The localized <c>PageWishListUrl</c> setting; unset values default to an empty string.</value>
        public string WishListUrl
        {
            get { return parent.GetProp("PageWishListUrl", "", true); }
            set { parent.SetProp("PageWishListUrl", value, true); }
        }

        /// <summary>Gets or sets the search page URL.</summary>
        /// <value>The localized <c>PageSearchUrl</c> setting; unset values default to an empty string.</value>
        public string SearchUrl
        {
            get { return parent.GetProp("PageSearchUrl", "", true); }
            set { parent.SetProp("PageSearchUrl", value, true); }
        }

        /// <summary>Gets or sets the order-history page URL.</summary>
        /// <value>The localized <c>PageOrderHistoryUrl</c> setting; unset values default to an empty string.</value>
        public string OrderHistoryUrl
        {
            get { return parent.GetProp("PageOrderHistoryUrl", "", true); }
            set { parent.SetProp("PageOrderHistoryUrl", value, true); }
        }

        /// <summary>Gets or sets the address-book page URL.</summary>
        /// <value>The localized <c>PageAddressBookUrl</c> setting; unset values default to an empty string.</value>
        public string AddressBookUrl
        {
            get { return parent.GetProp("PageAddressBookUrl", "", true); }
            set { parent.SetProp("PageAddressBookUrl", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the category page.</summary>
        /// <value>The localized <c>PageCategoryTabId</c> setting; unset values default to <c>0</c>.</value>
        public int CategoryTabId
        {
            get { return parent.GetPropInt("PageCategoryTabId", true); }
            set { parent.SetProp("PageCategoryTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the product page.</summary>
        /// <value>The localized <c>PageProductTabId</c> setting; unset values default to <c>0</c>.</value>
        public int ProductTabId
        {
            get { return parent.GetPropInt("PageProductTabId", true); }
            set { parent.SetProp("PageProductTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the product-review page.</summary>
        /// <value>The localized <c>PageProductReviewTabId</c> setting; unset values default to <c>0</c>.</value>
        public int ProductReviewTabId
        {
            get { return parent.GetPropInt("PageProductReviewTabId", true); }
            set { parent.SetProp("PageProductReviewTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the cart page.</summary>
        /// <value>The localized <c>PageCartTabId</c> setting; unset values default to <c>0</c>.</value>
        public int CartTabId
        {
            get { return parent.GetPropInt("PageCartTabId", true); }
            set { parent.SetProp("PageCartTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the checkout page.</summary>
        /// <value>The localized <c>PageCheckoutTabId</c> setting; unset values default to <c>0</c>.</value>
        public int CheckoutTabId
        {
            get { return parent.GetPropInt("PageCheckoutTabId", true); }
            set { parent.SetProp("PageCheckoutTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the wish-list page.</summary>
        /// <value>The localized <c>PageWishListTabId</c> setting; unset values default to <c>0</c>.</value>
        public int WishListTabId
        {
            get { return parent.GetPropInt("PageWishListTabId", true); }
            set { parent.SetProp("PageWishListTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the search page.</summary>
        /// <value>The localized <c>PageSearchTabId</c> setting; unset values default to <c>0</c>.</value>
        public int SearchTabId
        {
            get { return parent.GetPropInt("PageSearchTabId", true); }
            set { parent.SetProp("PageSearchTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the order-history page.</summary>
        /// <value>The localized <c>PageOrderHistoryTabId</c> setting; unset values default to <c>0</c>.</value>
        public int OrderHistoryTabId
        {
            get { return parent.GetPropInt("PageOrderHistoryTabId", true); }
            set { parent.SetProp("PageOrderHistoryTabId", value, true); }
        }

        /// <summary>Gets or sets the DNN tab identifier for the address-book page.</summary>
        /// <value>The localized <c>PageAddressBookTabId</c> setting; unset values default to <c>0</c>.</value>
        public int AddressBookTabId
        {
            get { return parent.GetPropInt("PageAddressBookTabId", true); }
            set { parent.SetProp("PageAddressBookTabId", value, true); }
        }

        /// <summary>Gets or sets whether the setup wizard welcome is hidden.</summary>
        /// <value>The <c>SkipSetupWizard</c> setting; unset values read as <see langword="false"/>.</value>
        public bool HideSetupWizardWelcome
        {
            get { return parent.GetPropBool("SkipSetupWizard"); }
            set { parent.SetProp("SkipSetupWizard", value); }
        }

        /// <summary>Gets or sets the workflow plugin assembly-qualified type name.</summary>
        /// <value>The <c>WorkflowPluginAssemblyAndType</c> setting.</value>
        public string WorkflowPluginAssemblyAndType
        {
            get { return parent.GetProp("WorkflowPluginAssemblyAndType"); }
            set { parent.SetProp("WorkflowPluginAssemblyAndType", value); }
        }

        /// <summary>Gets or sets the product integration assembly-qualified type name.</summary>
        /// <value>The <c>ProductIntegrationAssemblyAndType</c> setting.</value>
        public string ProductIntegrationAssemblyAndType
        {
            get { return parent.GetProp("ProductIntegrationAssemblyAndType"); }
            set { parent.SetProp("ProductIntegrationAssemblyAndType", value); }
        }

        /// <summary>Gets or sets the cart integration assembly-qualified type name.</summary>
        /// <value>The <c>CartIntegrationAssemblyAndType</c> setting.</value>
        public string CartIntegrationAssemblyAndType
        {
            get { return parent.GetProp("CartIntegrationAssemblyAndType"); }
            set { parent.SetProp("CartIntegrationAssemblyAndType", value); }
        }

        /// <summary>Gets or sets the checkout integration assembly-qualified type name.</summary>
        /// <value>The <c>CheckoutIntegrationAssemblyAndType</c> setting.</value>
        public string CheckoutIntegrationAssemblyAndType
        {
            get { return parent.GetProp("CheckoutIntegrationAssemblyAndType"); }
            set { parent.SetProp("CheckoutIntegrationAssemblyAndType", value); }
        }

        /// <summary>Gets or sets the virtual path from which store views are loaded.</summary>
        /// <value>The <c>ViewsVirtualPath</c> setting; null or empty values read as <see cref="DefaultViewsVirtualPath"/>.</value>
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
