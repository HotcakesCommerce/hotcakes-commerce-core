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
using System.Globalization;
using System.IO;
using System.Web.UI;
using DotNetNuke.Entities.Portals;
using DotNetNuke.Entities.Tabs;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;
using Telerik.Web.UI;

namespace Hotcakes.Modules.Core.Admin.SetupWizard
{
    public partial class Step1General : HccPart
    {
        #region Properties

        protected string AddressBvin
        {
            get { return ViewState["AddressBvin"] as string; }
            set { ViewState["AddressBvin"] = value; }
        }

        protected AddressTypes AddressTypeField
        {
            get { return (AddressTypes) (ViewState["AddressTypeField"] ?? AddressTypes.General); }
            set { ViewState["AddressTypeField"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LocalizeView();

            BindStoreInfo();
            BindAddress();
            BindLocation();
            BindUrls();
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!IsPostBack)
            {
                txtStoreName.Focus();
            }
        }

        protected void ddlCountries_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            PopulateRegions(ddlCountries.SelectedValue);
        }

        protected void ddlCurrencyCulture_SelectedIndexChanged(object sender, RadComboBoxSelectedIndexChangedEventArgs e)
        {
            var currCultureCode = HccApp.CurrentStore.Settings.CurrencyCultureCode;
            var currCultureInfo = CultureInfo.GetCultureInfo(currCultureCode);

            var newCultureCode = ddlCurrencyCulture.SelectedValue;
            var newCultureInfo = CultureInfo.GetCultureInfo(newCultureCode);

            if (newCultureInfo.NumberFormat.CurrencyDecimalDigits < currCultureInfo.NumberFormat.CurrencyDecimalDigits)
                ucMessageBox.ShowWarning(Localization.GetString("CurrencyDecimalWarning"));
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveStoreInfo();
                SaveAddress();
                SaveLocation();
                SaveUrls();

                HccApp.UpdateCurrentStore();

                BindStoreInfo();

                NotifyFinishedEditing();
            }
        }

        protected void btnLater_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing("EXIT");
        }

        #endregion

        #region Implementation / binding

        private void BindAddress()
        {
            PopulateCountries();
            ddlCountries.ClearSelection();
            if (ddlCountries.Items.FindItemByValue(WebAppSettings.ApplicationCountryBvin) != null)
            {
                ddlCountries.Items.FindItemByValue(WebAppSettings.ApplicationCountryBvin).Selected = true;
                PopulateRegions(WebAppSettings.ApplicationCountryBvin);
            }
            else
            {
                if (ddlCountries.Items.Count > 0)
                {
                    ddlCountries.Items[0].Selected = true;
                    PopulateRegions(ddlCountries.Items[0].Value);
                }
            }

            LoadFromAddress(HccApp.ContactServices.Addresses.FindStoreContactAddress());
        }

        private void PopulateCountries()
        {
            ddlCountries.DataSource = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            ddlCountries.DataValueField = "Bvin";
            ddlCountries.DataTextField = "DisplayName";
            ddlCountries.DataBind();
        }

        private void PopulateRegions(string countryCode)
        {
            ddlRegions.Items.Clear();
            ddlRegions.ClearSelection();
            ddlRegions.Text = null;
            var country = HccApp.GlobalizationServices.Countries.Find(countryCode);
            ddlRegions.DataSource = country.Regions;
            ddlRegions.DataTextField = "DisplayName";
            ddlRegions.DataValueField = "Abbreviation";
            ddlRegions.DataBind();

            var li = new RadComboBoxItem(Localization.GetString("SelectState"), string.Empty);

            valRegion.Enabled = ddlRegions.Items.Count > 1;
        }

        private void LoadFromAddress(Address addr)
        {
            if (addr != null)
            {
                AddressBvin = addr.Bvin;
                AddressTypeField = addr.AddressType;
                ddlCountries.SelectedValue = addr.CountryBvin;
                PopulateRegions(addr.CountryBvin);
                ddlRegions.SelectedValue = addr.RegionBvin;

                txtFirstName.Text = addr.FirstName;
                txtLastName.Text = addr.LastName;
                txtCompany.Text = addr.Company;
                txtAddressLine1.Text = addr.Line1;
                txtAddressLine2.Text = addr.Line2;
                txtCity.Text = addr.City;
                txtZip.Text = addr.PostalCode;
                txtPhone.Text = addr.Phone;
            }
        }

        private Address GetAddressObject()
        {
            var a = new Address();

            if (ddlCountries.Items.Count > 0)
            {
                a.CountryBvin = ddlCountries.SelectedValue;
            }
            else
            {
                a.CountryBvin = string.Empty;
            }

            if (ddlRegions.Items.Count > 0 && ddlRegions.SelectedItem != null)
            {
                a.RegionBvin = ddlRegions.SelectedValue;
            }
            else
            {
                a.RegionBvin = string.Empty;
            }

            a.FirstName = txtFirstName.Text;
            a.LastName = txtLastName.Text;
            a.Company = txtCompany.Text;
            a.Line1 = txtAddressLine1.Text;
            a.Line2 = txtAddressLine2.Text;
            a.City = txtCity.Text;
            a.PostalCode = txtZip.Text;
            a.Phone = txtPhone.Text;
            a.Bvin = AddressBvin;
            a.AddressType = AddressTypeField;
            a.StoreId = HccApp.CurrentStore.Id;

            return a;
        }

        private void BindStoreInfo()
        {
            var sett = HccApp.CurrentStore.Settings;

            txtStoreName.Text = sett.FriendlyName;
            chkUseSSL.Checked = sett.ForceAdminSSL;

            ucStoreLogo.ImageUrl = sett.LogoImageFullUrl(HccApp, Request.IsSecureConnection);

            if (string.IsNullOrWhiteSpace(ucStoreLogo.ImageUrl))
            {
                ucStoreLogo.ImageUrl = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/MissingImage.png");
            }
        }

        private void BindUrls()
        {
            var sett = HccApp.CurrentStore.Settings.Urls;

            var showPageUrls = PageUtils.ShowPageUrls();
            var urlControls = new List<Control> {divCategoryUrl, divProductsUrl, divCheckoutUrl};

            urlControls.ForEach(c => c.Visible = showPageUrls);

            if (showPageUrls)
            {
                var categoryUrl = string.IsNullOrEmpty(sett.CategoryUrl)
                    ? StoreSettingsUrls.DefaultCategoryUrl
                    : sett.CategoryUrl;
                var productUrl = string.IsNullOrEmpty(sett.ProductUrl)
                    ? StoreSettingsUrls.DefaultProductUrl
                    : sett.ProductUrl;
                var checkoutUrl = string.IsNullOrEmpty(sett.CheckoutUrl)
                    ? StoreSettingsUrls.DefaultCheckoutUrl
                    : sett.CheckoutUrl;

                txtCategoryUrl.Text = categoryUrl;
                txtProductsUrl.Text = productUrl;
                txtCheckoutUrl.Text = checkoutUrl;
            }

            var list = TabController.GetPortalTabs(PortalSettings.Current.PortalId, 0, false, true, false, true);

            PopulateTabs(ddlCategoryTab, list, sett.CategoryTabId);
            PopulateTabs(ddlCheckoutTab, list, sett.CheckoutTabId);
            PopulateTabs(ddlProductsTab, list, sett.ProductTabId);
        }

        private void BindLocation()
        {
            PopulateTimeZones();
            PopulateCultures();
            FillLocationForm();
        }

        private void PopulateTabs(RadComboBox ddlTabs, List<TabInfo> list, int defaultValue)
        {
            ddlTabs.DataSource = list;
            ddlTabs.DataTextField = "IndentedTabName";
            ddlTabs.DataValueField = "TabID";
            ddlTabs.DataBind();
            ddlTabs.SelectedValue = defaultValue.ToString();

            var li = new RadComboBoxItem
            {
                Value = Convert.ToString(-1),
                Text = Localization.GetString("CreateNewPage")
            };
            ddlTabs.Items.Insert(0, li);
        }

        private void PopulateTimeZones()
        {
            ddlTimeZone.DataSource = TimeZoneInfo.GetSystemTimeZones();
            ddlTimeZone.DataTextField = "DisplayName";
            ddlTimeZone.DataValueField = "Id";
            ddlTimeZone.DataBind();
        }

        private void PopulateCultures()
        {
            var allCountries = HccApp.GlobalizationServices.Countries.FindAllForCurrency();

            ddlCurrencyCulture.DataSource = allCountries;
            ddlCurrencyCulture.DataTextField = "SampleNameAndCurrency";
            ddlCurrencyCulture.DataValueField = "CultureCode";
            ddlCurrencyCulture.DataBind();
        }

        private void FillLocationForm()
        {
            var timeZone = HccApp.CurrentStore.Settings.TimeZone;
            var timeZoneItem = ddlTimeZone.Items.FindItemByValue(timeZone.Id);

            if (timeZoneItem != null)
                timeZoneItem.Selected = true;

            var currencyCode = HccApp.CurrentStore.Settings.CurrencyCultureCode;
            var currencyCodeItem = ddlCurrencyCulture.Items.FindItemByValue(currencyCode);

            if (currencyCodeItem != null)
                currencyCodeItem.Selected = true;
        }

        private void LocalizeView()
        {
            txtFirstName.EmptyMessage = Localization.GetString("txtFirstName.EmptyMessage");
            txtLastName.EmptyMessage = Localization.GetString("txtLastName.EmptyMessage");
            txtCompany.EmptyMessage = Localization.GetString("txtCompany.EmptyMessage");
            txtAddressLine1.EmptyMessage = Localization.GetString("txtAddressLine1.EmptyMessage");
            rfvAddress1.ErrorMessage = Localization.GetString("rfvAddress1.ErrorMessage");
            txtAddressLine2.EmptyMessage = Localization.GetString("txtAddressLine2.EmptyMessage");
            txtCity.EmptyMessage = Localization.GetString("txtCity.EmptyMessage");
            rfvCity.ErrorMessage = Localization.GetString("rfvCity.ErrorMessage");
            txtRegion.EmptyMessage = Localization.GetString("txtRegion.EmptyMessage");
            valRegion.ErrorMessage = Localization.GetString("valRegion.ErrorMessage");
            valRegion.ValueToCompare = Localization.GetString("SelectState");
            txtZip.EmptyMessage = Localization.GetString("txtZip.EmptyMessage");
            rfvZip.ErrorMessage = Localization.GetString("rfvZip.ErrorMessage");
            txtPhone.EmptyMessage = Localization.GetString("txtPhone.EmptyMessage");
            rfvCategoryUrl.ErrorMessage = Localization.GetString("rfvCategoryUrl.ErrorMessage");
            rfvProductsUrl.ErrorMessage = Localization.GetString("rfvProductsUrl.ErrorMessage");
            rfvCheckoutUrl.ErrorMessage = Localization.GetString("rfvCheckoutUrl.ErrorMessage");
        }

        #endregion

        #region Implementation / saving

        private void SaveStoreInfo()
        {
            HccApp.CurrentStore.Settings.FriendlyName = txtStoreName.Text.Trim();
            HccApp.CurrentStore.Settings.UseLogoImage = true;

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
        }

        private void SaveAddress()
        {
            var toUpdate = GetAddressObject();
            toUpdate.AddressType = AddressTypes.StoreContact;

            if (toUpdate.Bvin == string.Empty)
            {
                HccApp.ContactServices.Addresses.Create(toUpdate);
            }
            else
            {
                HccApp.ContactServices.Addresses.Update(toUpdate);
            }
        }

        private void SaveLocation()
        {
            var timeZone = TimeZoneInfo.FindSystemTimeZoneById(ddlTimeZone.SelectedValue);

            HccApp.CurrentStore.Settings.TimeZone = timeZone;
            HccApp.CurrentStore.Settings.CurrencyCultureCode = ddlCurrencyCulture.SelectedValue;
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
                sett.AddressBookUrl = StoreSettingsUrls.DefaultAddressBookUrl;
                sett.CartUrl = StoreSettingsUrls.DefaultCartUrl;
                sett.OrderHistoryUrl = StoreSettingsUrls.DefaultOrderHistoryUrl;
                sett.ProductReviewUrl = StoreSettingsUrls.DefaultProductReviewUrl;
                sett.SearchUrl = StoreSettingsUrls.DefaultSearchUrl;
                sett.WishListUrl = StoreSettingsUrls.DefaultWishListUrl;
            }

            sett.CategoryTabId = int.Parse(ddlCategoryTab.SelectedValue);
            sett.ProductTabId = int.Parse(ddlProductsTab.SelectedValue);
            sett.CheckoutTabId = int.Parse(ddlCheckoutTab.SelectedValue);

            PageUtils.EnsureTabsExist(sett);
        }

        #endregion
    }
}