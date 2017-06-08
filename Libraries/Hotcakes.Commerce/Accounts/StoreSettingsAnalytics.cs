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

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreSettingsAnalytics
    {
        private readonly StoreSettings parent;

        public StoreSettingsAnalytics(StoreSettings s)
        {
            parent = s;
        }

        public bool UseGoogleAdWords
        {
            get { return parent.GetPropBool("UseGoogleAdWords"); }
            set { parent.SetProp("UseGoogleAdWords", value); }
        }

        public bool UseGoogleEcommerce
        {
            get { return parent.GetPropBool("UseGoogleEcommerce"); }
            set { parent.SetProp("UseGoogleEcommerce", value); }
        }

        public bool UseYahooTracker
        {
            get { return parent.GetPropBool("UseYahooTracker"); }
            set { parent.SetProp("UseYahooTracker", value); }
        }

        public string GoogleAdWordsId
        {
            get { return parent.GetProp("GoogleAdWordsId"); }
            set { parent.SetProp("GoogleAdWordsId", value); }
        }

        public string GoogleAdWordsFormat
        {
            get { return parent.GetProp("GoogleAdWordsFormat"); }
            set { parent.SetProp("GoogleAdWordsFormat", value); }
        }

        public string GoogleAdWordsLabel
        {
            get { return parent.GetProp("GoogleAdWordsLabel"); }
            set { parent.SetProp("GoogleAdWordsLabel", value); }
        }

        public string GoogleAdWordsBgColor
        {
            get { return parent.GetProp("GoogleAdWordsBgColor"); }
            set { parent.SetProp("GoogleAdWordsBgColor", value); }
        }

        public string GoogleEcommerceStoreName
        {
            get { return parent.GetProp("GoogleEcommerceStoreName"); }
            set { parent.SetProp("GoogleEcommerceStoreName", value); }
        }

        public string GoogleEcommerceCategory
        {
            get { return parent.GetProp("GoogleEcommerceCategory"); }
            set { parent.SetProp("GoogleEcommerceCategory", value); }
        }

        public string YahooAccountId
        {
            get { return parent.GetProp("YahooAccountId"); }
            set { parent.SetProp("YahooAccountId", value); }
        }

        public string AdditionalMetaTags
        {
            get
            {
                var prop = parent.GetProp("AdditionalMetaTags");
                return prop;
            }
            set { parent.SetProp("AdditionalMetaTags", value); }
        }

        public string BottomAnalytics
        {
            get
            {
                var prop = parent.GetProp("BottomAnalytics");
                return prop;
            }
            set { parent.SetProp("BottomAnalytics", value); }
        }

        public bool UseShopZillaSurvey
        {
            get { return parent.GetPropBool("UseShopZillaSurvey"); }
            set { parent.SetProp("UseShopZillaSurvey", value); }
        }

        public string ShopZillaId
        {
            get { return parent.GetProp("ShopZillaId"); }
            set { parent.SetProp("ShopZillaId", value); }
        }
    }
}