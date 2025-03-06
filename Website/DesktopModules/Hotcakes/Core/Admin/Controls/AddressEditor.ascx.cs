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
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class AddressEditor : HccUserControl
    {
        public AddressEditor()
        {
            RequirePhone = false;
            RequireCompany = false;
            ShowCounty = false;
            ShowAddressLine2 = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LocalizeView();

                InitializeAddress();

                lstCountry.TabIndex = (short) (_TabOrderOffSet + 0);
                firstNameField.TabIndex = (short) (_TabOrderOffSet + 2);
                lastNameField.TabIndex = (short) (_TabOrderOffSet + 4);
                CompanyField.TabIndex = (short) (_TabOrderOffSet + 5);
                address1Field.TabIndex = (short) (_TabOrderOffSet + 6);
                address2Field.TabIndex = (short) (_TabOrderOffSet + 7);
                address3Field.TabIndex = (short) (_TabOrderOffSet + 7);
                cityField.TabIndex = (short) (_TabOrderOffSet + 8);
                lstRegions.TabIndex = (short) (_TabOrderOffSet + 9);
                regionField.TabIndex = (short) (_TabOrderOffSet + 10);
                postalCodeField.TabIndex = (short) (_TabOrderOffSet + 11);
                PhoneNumberField.TabIndex = (short) (_TabOrderOffSet + 13);
            }
            UpdateVisibleRows();
        }

        public bool Validate()
        {
            var result = true;

            if (_RequireAddress)
            {
                valAddress.Validate();
                if (valAddress.IsValid == false)
                {
                    result = false;
                }
            }

            if (_RequireFirstName)
            {
                valFirstName.Validate();
                if (valFirstName.IsValid == false)
                {
                    result = false;
                }
            }

            if (_RequireLastName)
            {
                valLastName.Validate();
                if (valLastName.IsValid == false)
                {
                    result = false;
                }
            }

            if (_RequireCity)
            {
                valCity.Validate();
                if (valCity.IsValid == false)
                {
                    result = false;
                }
            }

            if (_RequirePostalCode)
            {
                valPostalCode.Validate();
                if (valPostalCode.IsValid == false)
                {
                    result = false;
                }
            }


            if (RequireCompany)
            {
                valCompany.Validate();
                if (valCompany.IsValid == false)
                {
                    result = false;
                }
            }

            if (RequirePhone)
            {
                valPhone.Validate();
                if (valPhone.IsValid == false)
                {
                    result = false;
                }
                if (PhoneNumberField.Text.Trim().Length < 7)
                {
                    result = false;
                    valPhone.IsValid = false;
                }
            }

            return result;
        }

        public void LoadFromAddress(Address a)
        {
            InitializeAddress();
            if (a != null)
            {
                AddressBvin.Value = a.Bvin;
                AddressTypeField.Value = ((int) a.AddressType).ToString();
                lstCountry.ClearSelection();
                if (lstCountry.Items.FindByValue(a.CountryBvin) != null)
                {
                    lstCountry.Items.FindByValue(a.CountryBvin).Selected = true;
                }
                PopulateRegions(lstCountry.SelectedValue);
                if (lstRegions.Items.Count > 0)
                {
                    lstRegions.ClearSelection();
                    if (lstRegions.Items.FindByValue(a.RegionBvin) != null)
                    {
                        lstRegions.Items.FindByValue(a.RegionBvin).Selected = true;
                    }
                }

                State = a.RegionBvin;
                FirstName = a.FirstName;
                LastName = a.LastName;
                CompanyName = a.Company;
                StreetLine1 = a.Line1;
                StreetLine2 = a.Line2;
                StreetLine3 = a.Line3;
                City = a.City;
                PostalCode = a.PostalCode;
                PhoneNumber = a.Phone;

                StoreId.Value = a.StoreId.ToString();
            }
        }

        public Address GetAsAddress()
        {
            var a = new Address();
            if (lstCountry.Items.Count > 0)
            {
                a.CountryBvin = lstCountry.SelectedValue;
            }

            a.RegionBvin = State;
            a.FirstName = FirstName;
            a.LastName = LastName;
            a.Company = CompanyName;
            a.Line1 = StreetLine1;
            a.Line2 = StreetLine2;
            a.Line3 = StreetLine3;
            a.City = City;
            a.PostalCode = PostalCode;
            a.Phone = PhoneNumber;
            if (AddressBvin.Value != string.Empty)
            {
                a.Bvin = AddressBvin.Value;
            }
            var type = 0;
            if (int.TryParse(AddressTypeField.Value, out type))
            {
                a.AddressType = (AddressTypes) type;
            }

            long storeId = 0;
            if (long.TryParse(StoreId.Value, out storeId)) a.StoreId = storeId;

            return a;
        }

        private void UpdateVisibleRows()
        {
            valFirstName.Enabled = _RequireFirstName;
            valLastName.Enabled = _RequireLastName;
            valAddress.Enabled = _RequireAddress;
            valCity.Enabled = _RequireCity;
            valPostalCode.Enabled = _RequirePostalCode;
            valRegion.Enabled = _RequireRegion && (lstRegions.Items.Count > 2);

            if (_ShowCompanyName)
            {
                CompanyNameRow.Visible = true;
                valCompany.Enabled = RequireCompany;
            }
            else
            {
                valCompany.Enabled = false;
                CompanyNameRow.Visible = false;
            }

            if (_ShowPhoneNumber)
            {
                PhoneRow.Visible = true;
                valPhone.Enabled = RequirePhone;
            }
            else
            {
                valPhone.Enabled = false;
                PhoneRow.Visible = false;
            }

            divAddressLine2.Visible = ShowAddressLine2;
            divAddressLine3.Visible = ShowAddressLine3;
        }

        private void InitializeAddress()
        {
            if (_Initialized == false)
            {
                AddressTypeField.Value = "0";

                PopulateCountries();
                lstCountry.ClearSelection();
                if (lstCountry.Items.FindByValue(WebAppSettings.ApplicationCountryBvin) != null)
                {
                    lstCountry.Items.FindByValue(WebAppSettings.ApplicationCountryBvin).Selected = true;
                    PopulateRegions(WebAppSettings.ApplicationCountryBvin);
                }
                else
                {
                    if (lstCountry.Items.Count > 0)
                    {
                        lstCountry.Items[0].Selected = true;
                        PopulateRegions(lstCountry.Items[0].Value);
                    }
                }

                _Initialized = true;
            }
        }

        private void PopulateCountries()
        {
            lstCountry.DataSource = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            lstCountry.DataValueField = "Bvin";
            lstCountry.DataTextField = "DisplayName";
            lstCountry.DataBind();
        }

        private void PopulateRegions(string countryCode)
        {
            lstRegions.Items.Clear();
            var country = HccApp.GlobalizationServices.Countries.Find(countryCode);
            lstRegions.DataSource = country.Regions;
            lstRegions.DataTextField = "DisplayName";
            lstRegions.DataValueField = "Abbreviation";
            lstRegions.DataBind();

            var li = new ListItem(Localization.GetString("NotSelected"), string.Empty);
            lstRegions.Items.Insert(0, li);

            valRegion.Enabled = _RequireRegion && (lstRegions.Items.Count > 2);
        }

        protected void lstCountry_SelectedIndexChanged(object Sender, EventArgs e)
        {
            PopulateRegions(lstCountry.SelectedItem.Value);
        }

        private void LocalizeView()
        {
            valFirstName.ErrorMessage = Localization.GetString("valFirstName.ErrorMessage");
            valLastName.ErrorMessage = Localization.GetString("valLastName.ErrorMessage");
            valCompany.ErrorMessage = Localization.GetString("valCompany.ErrorMessage");
            valAddress.ErrorMessage = Localization.GetString("valAddress.ErrorMessage");
            valCity.ErrorMessage = Localization.GetString("valCity.ErrorMessage");
            valRegion.ErrorMessage = Localization.GetString("valRegion.ErrorMessage");
            valRegion.InitialValue = Localization.GetString("NotSelected");
            valPostalCode.ErrorMessage = Localization.GetString("valPostalCode.ErrorMessage");
            valPhone.ErrorMessage = Localization.GetString("valPhone.ErrorMessage");
        }

        #region Properties

        private bool _ShowCompanyName = true;
        private bool _ShowPhoneNumber = true;

        private bool _RequireFirstName = true;
        private bool _RequireLastName = true;

        private bool _RequireAddress = true;
        private bool _RequireCity = true;
        private bool _RequirePostalCode = true;
        private bool _RequireRegion = true;

        private int _TabOrderOffSet = 100;

        public bool ShowAddressLine2 { get; set; }
        public bool ShowAddressLine3 { get; set; }

        public bool CreateValidationInputs { get; set; }
        public string FormSelector { get; set; }
        public string ErrorMessageSelector { get; set; }

        public bool ShowCompanyName
        {
            get { return _ShowCompanyName; }
            set { _ShowCompanyName = value; }
        }

        public bool ShowPhoneNumber
        {
            get { return _ShowPhoneNumber; }
            set { _ShowPhoneNumber = value; }
        }

        public bool ShowCounty { get; set; }

        public bool RequireFirstName
        {
            get { return _RequireFirstName; }
            set { _RequireFirstName = value; }
        }

        public bool RequireLastName
        {
            get { return _RequireLastName; }
            set { _RequireLastName = value; }
        }

        public bool RequireCompany { get; set; }

        public bool RequirePhone { get; set; }

        public bool RequireAddress
        {
            get { return _RequireAddress; }
            set { _RequireAddress = value; }
        }

        public bool RequireCity
        {
            get { return _RequireCity; }
            set { _RequireCity = value; }
        }

        public bool RequirePostalCode
        {
            get { return _RequirePostalCode; }
            set { _RequirePostalCode = value; }
        }

        public bool RequireRegion
        {
            get { return _RequireRegion; }
            set { _RequireRegion = value; }
        }

        public string CountryName
        {
            get { return lstCountry.SelectedItem.Text; }
        }

        public string CountryCode
        {
            get { return lstCountry.SelectedValue; }
            set
            {
                if (lstCountry.Items.FindByValue(value) != null)
                {
                    lstCountry.ClearSelection();
                    lstCountry.Items.FindByValue(value).Selected = true;
                }
                PopulateRegions(lstCountry.SelectedValue);
            }
        }

        public string FirstName
        {
            get { return firstNameField.Text.Trim(); }
            set { firstNameField.Text = value; }
        }

        public string LastName
        {
            get { return lastNameField.Text.Trim(); }
            set { lastNameField.Text = value; }
        }

        public string CompanyName
        {
            get { return CompanyField.Text.Trim(); }
            set { CompanyField.Text = value; }
        }

        public string StreetLine1
        {
            get { return address1Field.Text.Trim(); }
            set { address1Field.Text = value; }
        }

        public string StreetLine2
        {
            get { return address2Field.Text.Trim(); }
            set { address2Field.Text = value; }
        }

        public string StreetLine3
        {
            get { return address3Field.Text.Trim(); }
            set { address3Field.Text = value; }
        }

        public string City
        {
            get { return cityField.Text.Trim(); }
            set { cityField.Text = value; }
        }

        public string State
        {
            get
            {
                if (lstRegions.Items.Count > 0)
                {
                    return lstRegions.SelectedValue;
                }
                return regionField.Text.Trim();
            }
            set
            {
                if (lstRegions.Items.FindByValue(value) != null)
                {
                    lstRegions.ClearSelection();
                    lstRegions.Items.FindByValue(value).Selected = true;
                }
                regionField.Text = value;
            }
        }

        public string PostalCode
        {
            get { return postalCodeField.Text.Trim(); }
            set { postalCodeField.Text = value; }
        }

        public string PhoneNumber
        {
            get { return PhoneNumberField.Text.Trim(); }
            set { PhoneNumberField.Text = value; }
        }

        public int TabOrderOffSet
        {
            get { return _TabOrderOffSet; }
            set { _TabOrderOffSet = value; }
        }

        private bool _Initialized;

        #endregion
    }
}