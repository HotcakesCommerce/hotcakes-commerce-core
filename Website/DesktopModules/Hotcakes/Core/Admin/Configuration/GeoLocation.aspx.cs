#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class GeoLocation : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("GeoLocation");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                PopulateTimeZones();
                PopulateCultures();
                FillForm();
            }
        }

        protected void lstCulture_SelectedIndexChanged(object sender, EventArgs e)
        {
            var currCultureCode = HccApp.CurrentStore.Settings.CurrencyCultureCode;
            var currCultureInfo = CultureInfo.GetCultureInfo(currCultureCode);

            var newCultureCode = lstCulture.SelectedValue;
            var newCultureInfo = CultureInfo.GetCultureInfo(newCultureCode);

            if (newCultureInfo.NumberFormat.CurrencyDecimalDigits < currCultureInfo.NumberFormat.CurrencyDecimalDigits)
                ucMessageBox.ShowWarning(Localization.GetString("DecimalWarning.Text"));
        }

        private void PopulateTimeZones()
        {
            lstTimeZone.DataSource = TimeZoneInfo.GetSystemTimeZones();
            lstTimeZone.DataTextField = "DisplayName";
            lstTimeZone.DataValueField = "Id";
            lstTimeZone.DataBind();
        }

        private void PopulateCultures()
        {
            var allCountries = HccApp.GlobalizationServices.Countries.FindAllForCurrency();

            lstCulture.DataSource = allCountries;
            lstCulture.DataTextField = "SampleNameAndCurrency";
            lstCulture.DataValueField = "CultureCode";
            lstCulture.DataBind();
        }

        private void FillForm()
        {
            var timeZone = HccApp.CurrentStore.Settings.TimeZone;
            var timeZoneItem = lstTimeZone.Items.FindByValue(timeZone.Id);
            if (timeZoneItem != null)
                timeZoneItem.Selected = true;

            var currencyCode = HccApp.CurrentStore.Settings.CurrencyCultureCode;
            var currencyCodeItem = lstCulture.Items.FindByValue(currencyCode);
            if (currencyCodeItem != null)
                currencyCodeItem.Selected = true;
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var timeZone = TimeZoneInfo.FindSystemTimeZoneById(lstTimeZone.SelectedValue);

                HccApp.CurrentStore.Settings.TimeZone = timeZone;
                HccApp.CurrentStore.Settings.CurrencyCultureCode = lstCulture.SelectedValue;

                HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);

                ucMessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
        }
    }
}