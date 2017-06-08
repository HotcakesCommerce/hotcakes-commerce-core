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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Analytics : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                // Loading
                chkGoogleAdwords.Checked = HccApp.CurrentStore.Settings.Analytics.UseGoogleAdWords;
                GoogleAdwordsConversionIdField.Text = HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsId;
                ddlAdwordsFormat.SelectedValue = HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsFormat;
                GoogleAdwordsLabelField.Text = HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsLabel;
                GoogleAdwordsBackgroundColorField.Text = HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsBgColor;

                chkGoogleEcommerce.Checked = HccApp.CurrentStore.Settings.Analytics.UseGoogleEcommerce;
                GoogleEcommerceCategoryNameField.Text = HccApp.CurrentStore.Settings.Analytics.GoogleEcommerceCategory;
                GoogleEcommerceStoreNameField.Text = HccApp.CurrentStore.Settings.Analytics.GoogleEcommerceStoreName;

                var googleSettings = HccApp.AccountServices.GetGoogleAnalyticsSettings();
                chkGoogleTracker.Checked = googleSettings.UseTracker;
                GoogleTrackingIdField.Text = googleSettings.TrackerId;

                chkYahoo.Checked = HccApp.CurrentStore.Settings.Analytics.UseYahooTracker;
                YahooAccountIdField.Text = HccApp.CurrentStore.Settings.Analytics.YahooAccountId;

                AdditionalMetaTagsField.Text = HccApp.CurrentStore.Settings.Analytics.AdditionalMetaTags;
                BottomAnalyticsField.Text = HccApp.CurrentStore.Settings.Analytics.BottomAnalytics;

                chkUseShopZillaSurvey.Checked = HccApp.CurrentStore.Settings.Analytics.UseShopZillaSurvey;
                ShopZillaIdField.Text = HccApp.CurrentStore.Settings.Analytics.ShopZillaId;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            HccApp.CurrentStore.Settings.Analytics.UseGoogleAdWords = chkGoogleAdwords.Checked;
            HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsId = GoogleAdwordsConversionIdField.Text;
            HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsFormat = ddlAdwordsFormat.SelectedValue;
            HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsLabel = GoogleAdwordsLabelField.Text;
            HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsBgColor = GoogleAdwordsBackgroundColorField.Text;

            HccApp.CurrentStore.Settings.Analytics.UseGoogleEcommerce = chkGoogleEcommerce.Checked;
            HccApp.CurrentStore.Settings.Analytics.GoogleEcommerceCategory = GoogleEcommerceCategoryNameField.Text;
            HccApp.CurrentStore.Settings.Analytics.GoogleEcommerceStoreName = GoogleEcommerceStoreNameField.Text;

            var googleSettings = new GoogleAnalyticsSettings();
            googleSettings.UseTracker = chkGoogleTracker.Checked;
            googleSettings.TrackerId = GoogleTrackingIdField.Text;
            HccApp.AccountServices.SetGoogleAnalyticsSettings(googleSettings);

            HccApp.CurrentStore.Settings.Analytics.UseYahooTracker = chkYahoo.Checked;
            HccApp.CurrentStore.Settings.Analytics.YahooAccountId = YahooAccountIdField.Text;

            HccApp.CurrentStore.Settings.Analytics.AdditionalMetaTags = AdditionalMetaTagsField.Text;
            HccApp.CurrentStore.Settings.Analytics.BottomAnalytics = BottomAnalyticsField.Text;

            HccApp.CurrentStore.Settings.Analytics.UseShopZillaSurvey = chkUseShopZillaSurvey.Checked;
            HccApp.CurrentStore.Settings.Analytics.ShopZillaId = ShopZillaIdField.Text.Trim();

            HccApp.UpdateCurrentStore();

            ucMessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
        }
    }
}