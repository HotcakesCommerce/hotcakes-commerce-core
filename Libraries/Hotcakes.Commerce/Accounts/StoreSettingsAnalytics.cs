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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Provides analytics and tracking settings owned by a <see cref="StoreSettings"/> instance.</summary>
    /// <remarks>Settings are scoped to the current store. Setters stage values in the parent façade; database persistence is performed by the caller's normal store-settings save operation.</remarks>
    [Serializable]
    public class StoreSettingsAnalytics
    {
        private readonly StoreSettings parent;

        /// <summary>Initializes a new analytics façade for <paramref name="s"/>.</summary>
        /// <param name="s">The store settings façade that owns the settings.</param>
        public StoreSettingsAnalytics(StoreSettings s)
        {
            parent = s;
        }

        /// <summary>Gets or sets whether Google AdWords tracking is enabled.</summary>
        /// <value>The value persisted under <c>UseGoogleAdWords</c>; an unset value is read as <see langword="false"/>.</value>
        public bool UseGoogleAdWords
        {
            get { return parent.GetPropBool("UseGoogleAdWords"); }
            set { parent.SetProp("UseGoogleAdWords", value); }
        }

        /// <summary>Gets or sets whether Google Ecommerce tracking is enabled.</summary>
        /// <value>The value persisted under <c>UseGoogleEcommerce</c>; an unset value is read as <see langword="false"/>.</value>
        public bool UseGoogleEcommerce
        {
            get { return parent.GetPropBool("UseGoogleEcommerce"); }
            set { parent.SetProp("UseGoogleEcommerce", value); }
        }

        /// <summary>Gets or sets whether the Yahoo tracker is enabled.</summary>
        /// <value>The value persisted under <c>UseYahooTracker</c>; an unset value is read as <see langword="false"/>.</value>
        public bool UseYahooTracker
        {
            get { return parent.GetPropBool("UseYahooTracker"); }
            set { parent.SetProp("UseYahooTracker", value); }
        }

        /// <summary>Gets or sets the Google AdWords identifier.</summary>
        /// <value>The value persisted under <c>GoogleAdWordsId</c>.</value>
        public string GoogleAdWordsId
        {
            get { return parent.GetProp("GoogleAdWordsId"); }
            set { parent.SetProp("GoogleAdWordsId", value); }
        }

        /// <summary>Gets or sets the Google AdWords tracking format.</summary>
        /// <value>The value persisted under <c>GoogleAdWordsFormat</c>.</value>
        public string GoogleAdWordsFormat
        {
            get { return parent.GetProp("GoogleAdWordsFormat"); }
            set { parent.SetProp("GoogleAdWordsFormat", value); }
        }

        /// <summary>Gets or sets the Google AdWords conversion label.</summary>
        /// <value>The value persisted under <c>GoogleAdWordsLabel</c>.</value>
        public string GoogleAdWordsLabel
        {
            get { return parent.GetProp("GoogleAdWordsLabel"); }
            set { parent.SetProp("GoogleAdWordsLabel", value); }
        }

        /// <summary>Gets or sets the Google AdWords background color.</summary>
        /// <value>The value persisted under <c>GoogleAdWordsBgColor</c>.</value>
        public string GoogleAdWordsBgColor
        {
            get { return parent.GetProp("GoogleAdWordsBgColor"); }
            set { parent.SetProp("GoogleAdWordsBgColor", value); }
        }

        /// <summary>Gets or sets the Google Ecommerce store name.</summary>
        /// <value>The value persisted under <c>GoogleEcommerceStoreName</c>.</value>
        public string GoogleEcommerceStoreName
        {
            get { return parent.GetProp("GoogleEcommerceStoreName"); }
            set { parent.SetProp("GoogleEcommerceStoreName", value); }
        }

        /// <summary>Gets or sets the Google Ecommerce category.</summary>
        /// <value>The value persisted under <c>GoogleEcommerceCategory</c>.</value>
        public string GoogleEcommerceCategory
        {
            get { return parent.GetProp("GoogleEcommerceCategory"); }
            set { parent.SetProp("GoogleEcommerceCategory", value); }
        }

        /// <summary>Gets or sets the Yahoo tracking account identifier.</summary>
        /// <value>The value persisted under <c>YahooAccountId</c>.</value>
        public string YahooAccountId
        {
            get { return parent.GetProp("YahooAccountId"); }
            set { parent.SetProp("YahooAccountId", value); }
        }

        /// <summary>Gets or sets additional HTML meta tags emitted by the store.</summary>
        /// <value>The value persisted under <c>AdditionalMetaTags</c>.</value>
        public string AdditionalMetaTags
        {
            get
            {
                var prop = parent.GetProp("AdditionalMetaTags");
                return prop;
            }
            set { parent.SetProp("AdditionalMetaTags", value); }
        }

        /// <summary>Gets or sets analytics markup emitted at the bottom of the store page.</summary>
        /// <value>The value persisted under <c>BottomAnalytics</c>.</value>
        public string BottomAnalytics
        {
            get
            {
                var prop = parent.GetProp("BottomAnalytics");
                return prop;
            }
            set { parent.SetProp("BottomAnalytics", value); }
        }

        /// <summary>Gets or sets whether the ShopZilla survey is enabled.</summary>
        /// <value>The value persisted under <c>UseShopZillaSurvey</c>; an unset value is read as <see langword="false"/>.</value>
        public bool UseShopZillaSurvey
        {
            get { return parent.GetPropBool("UseShopZillaSurvey"); }
            set { parent.SetProp("UseShopZillaSurvey", value); }
        }

        /// <summary>Gets or sets the ShopZilla identifier.</summary>
        /// <value>The value persisted under <c>ShopZillaId</c>.</value>
        public string ShopZillaId
        {
            get { return parent.GetProp("ShopZillaId"); }
            set { parent.SetProp("ShopZillaId", value); }
        }
    }
}
