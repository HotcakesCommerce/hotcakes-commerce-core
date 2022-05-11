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
using System.Linq;
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;
using Hotcakes.Shipping.UpsFreight;

namespace Hotcakes.Modules.Core.Admin.Parts.Shipping.UPSFreight
{

    partial class Edit : HccShippingPart
    {
        #region Private Members

        private bool IsNew
        {
            get
            {
                return (string.IsNullOrEmpty(ShippingMethod.Name));
            }
        }

        #endregion

        #region Events
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LocalizeView();

            AddHighlightColors(lstHighlights);
            LoadZones();
            LoadServiceCodes();
            LoadData();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing("Canceled");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            NotifyFinishedEditing(NameField.Text.Trim());
        }

        protected void cvAdjustmentTextBox_ServerValidate(object source, ServerValidateEventArgs args)
        {
            var val = 0m;

            if (decimal.TryParse(AdjustmentTextBox.Text, NumberStyles.Currency, CultureInfo.CurrentCulture, out val))
            {
                args.IsValid = true;
            }
            else
            {
                args.IsValid = false;
            }
        }
        #endregion

        #region Helper Methods
        private void LoadZones()
        {
            lstZones.Items.Clear();

            lstZones.DataSource = HccApp.OrderServices.ShippingZones.FindForStore(HccApp.CurrentStore.Id);
            lstZones.DataTextField = "Name";
            lstZones.DataValueField = "id";
            lstZones.DataBind();
        }

        private void LoadServiceCodes()
        {
            ShippingTypesRadioButtonList.Items.Clear();

            var ups = AvailableServices.FindById(ShippingMethod.ShippingProviderId, HccApp.CurrentStore);
            ShippingTypesRadioButtonList.DataSource = ups.ListAllServiceCodes();
            ShippingTypesRadioButtonList.DataTextField = "DisplayName";
            ShippingTypesRadioButtonList.DataValueField = "Code";
            ShippingTypesRadioButtonList.DataBind();

            if (IsNew || ShippingTypesRadioButtonList.SelectedIndex == -1)
            {
                ShippingTypesRadioButtonList.Items[0].Selected = true;
            }
        }

        private void LoadData()
        {
            // Name
            NameField.Text = ShippingMethod.Name;

            if (string.IsNullOrEmpty(NameField.Text))
            {
                NameField.Text = Localization.GetString("UPSFreightShipping");
            }

            // Adjustment
            AdjustmentDropDownList.SelectedValue = ((int)ShippingMethod.AdjustmentType).ToString();

            if (ShippingMethod.AdjustmentType == ShippingMethodAdjustmentType.Amount)
            {
                AdjustmentTextBox.Text = string.Format("{0:c}", ShippingMethod.Adjustment);
            }
            else
            {
                AdjustmentTextBox.Text = string.Format("{0:f}", ShippingMethod.Adjustment);
            }

            // Zones
            if (lstZones.Items.FindByValue(ShippingMethod.ZoneId.ToString()) != null)
            {
                lstZones.ClearSelection();
                lstZones.Items.FindByValue(ShippingMethod.ZoneId.ToString()).Selected = true;
            }

            // Global Settings
            UserNameField.Text = HccApp.CurrentStore.Settings.ShippingUpsUsername;
            PasswordField.Text = HccApp.CurrentStore.Settings.ShippingUpsPassword;
            AccessKeyField.Text = HccApp.CurrentStore.Settings.ShippingUpsLicense;
            AccountNumberField.Text = HccApp.CurrentStore.Settings.ShippingUpsAccountNumber;
            ResidentialAddressCheckBox.Checked = HccApp.CurrentStore.Settings.ShippingUpsFreightForceResidential;
            PayerShipmentBillingOptionDropDownList.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightBillingOption.ToString();
            HandlineOneUnitTypeDropDownList.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightHandleOneUnitType;

            DefaultPackagingField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightDefaultPackaging.ToString();
            DefaultServiceField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsDefaultService.ToString();
            DefaultFreightClassField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightFreightClass.ToString();

            //if (HccApp.CurrentStore.Settings.ShippingUpsLicense.Trim().Length > 0)
            //{
            //    lnkRegister.Text = Localization.GetString("AlreadyRegistered");
            //}
            //else
            //{
            //    lnkRegister.Text = Localization.GetString("RegisterWithUPS");
            //}

            SkipDimensionsCheckBox.Checked = HccApp.CurrentStore.Settings.ShippingUpsFreightSkipDimensions;
            chkDiagnostics.Checked = HccApp.CurrentStore.Settings.ShippingUPSFreightDiagnostics;

            // Method Settings
            var Settings = new UPSFreightServiceSettings();
            Settings.Merge(ShippingMethod.Settings);

            if (Settings.ServiceCodeFilter == null)
            {
                Settings.ServiceCodeFilter = new List<IServiceCode>();
            }

            foreach (ServiceCode code in Settings.ServiceCodeFilter)
            {
                foreach (ListItem item in ShippingTypesRadioButtonList.Items)
                {
                    if (string.Compare(item.Value, code.Code, true) == 0)
                    {
                        item.Selected = true;
                        break;
                    }
                }
            }

            // Select Hightlights
            var highlight = Settings.GetSettingOrEmpty("highlight");

            if (lstHighlights.Items.FindByText(highlight) != null)
            {
                lstHighlights.ClearSelection();
                lstHighlights.Items.FindByText(highlight).Selected = true;
            }
        }

        private void SaveData()
        {
            ShippingMethod.Name = NameField.Text.Trim();
            ShippingMethod.ZoneId = long.Parse(lstZones.SelectedItem.Value);
            ShippingMethod.AdjustmentType =
                (ShippingMethodAdjustmentType)int.Parse(AdjustmentDropDownList.SelectedValue);
            ShippingMethod.Adjustment = decimal.Parse(AdjustmentTextBox.Text, NumberStyles.Currency);

            if (ShippingMethod.AdjustmentType == ShippingMethodAdjustmentType.Amount)
            {
                ShippingMethod.Adjustment = Money.RoundCurrency(ShippingMethod.Adjustment);
            }

            // Global Settings
            HccApp.CurrentStore.Settings.ShippingUpsUsername = UserNameField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingUpsPassword = PasswordField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingUpsLicense = AccessKeyField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingUpsAccountNumber = AccountNumberField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingUpsFreightForceResidential = ResidentialAddressCheckBox.Checked;
          
            HccApp.CurrentStore.Settings.ShippingUpsDefaultService = int.Parse(DefaultServiceField.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingUpsFreightDefaultPackaging = int.Parse(DefaultPackagingField.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingUpsFreightSkipDimensions = SkipDimensionsCheckBox.Checked;
            HccApp.CurrentStore.Settings.ShippingUPSFreightDiagnostics = chkDiagnostics.Checked;

            HccApp.CurrentStore.Settings.ShippingUpsFreightBillingOption = Convert.ToInt32(PayerShipmentBillingOptionDropDownList.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingUpsFreightHandleOneUnitType =PayerShipmentBillingOptionDropDownList.SelectedValue;
            HccApp.CurrentStore.Settings.ShippingUpsFreightFreightClass = DefaultFreightClassField.SelectedValue;

            // Method Settings
            var Settings = new UPSFreightServiceSettings();
            Settings.Merge(ShippingMethod.Settings);

            var filter = new List<IServiceCode>();
            Settings.GetAllRates = false;
            foreach (ListItem item in ShippingTypesRadioButtonList.Items)
            {
                if (item.Selected)
                {
                    var code = new ServiceCode { Code = item.Value, DisplayName = item.Text };
                    filter.Add(code);
                }
            }

            Settings.ServiceCodeFilter = filter;
            Settings["highlight"] = lstHighlights.SelectedValue;
            ShippingMethod.Settings.Merge(Settings);

            HccApp.AccountServices.Stores.Update(HccApp.CurrentStore);
        }

        private void LocalizeView()
        {
            rfvAdjustmentTextBox.ErrorMessage = Localization.GetString("rfvAdjustmentTextBox.ErrorMessage");
            cvAdjustmentTextBox.ErrorMessage = Localization.GetString("cvAdjustmentTextBox.ErrorMessage");

            if (AdjustmentDropDownList.Items.Count == 0)
            {
                AdjustmentDropDownList.Items.Add(new ListItem(Localization.GetString("Amount"), "1"));
                AdjustmentDropDownList.Items.Add(new ListItem(Localization.GetString("Percentage"), "2"));

                AdjustmentDropDownList.Items[1].Selected = true;
            }

            if (PayerShipmentBillingOptionDropDownList.Items.Count == 0)
            {
                PayerShipmentBillingOptionDropDownList.Items.Add(new ListItem(Localization.GetString("Prepaid"), "10"));
                PayerShipmentBillingOptionDropDownList.Items.Add(new ListItem(Localization.GetString("BilltoThirdParty"), "30"));
                PayerShipmentBillingOptionDropDownList.Items.Add(new ListItem(Localization.GetString("FreightCollect"), "40"));

                PayerShipmentBillingOptionDropDownList.Items[0].Selected = true;
            }

            if (HandlineOneUnitTypeDropDownList.Items.Count == 0)
            {
                var items = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string,string>(Localization.GetString("Skid"), "SKD"),
                    new KeyValuePair<string,string>(Localization.GetString("Carboy"), "CBY"),
                    new KeyValuePair<string,string>(Localization.GetString("Pallet"), "PLT"),
                    new KeyValuePair<string,string>(Localization.GetString("Totes"), "TOT")
                };

                HandlineOneUnitTypeDropDownList.DataSource = items.OrderBy(i => i.Key);
                HandlineOneUnitTypeDropDownList.DataTextField = "Key";
                HandlineOneUnitTypeDropDownList.DataValueField = "Value";
                HandlineOneUnitTypeDropDownList.DataBind();

                HandlineOneUnitTypeDropDownList.Items[0].Selected = true;
            }


            if (DefaultServiceField.Items.Count == 0)
            {
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSFreightLTL"), "308"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSFreightLTLGuaranteed"), "309"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSFreightLTLGuaranteedAM"), "334"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSStandardLTL"), "349"));
            }

            if (DefaultPackagingField.Items.Count == 0)
            {
                var items = new List<KeyValuePair<string, string>>
                {
                    new KeyValuePair<string,string>(Localization.GetString("Bag"), "7"),
                    new KeyValuePair<string,string>(Localization.GetString("Bale"), "31"),
                    new KeyValuePair<string,string>(Localization.GetString("Barrel"), "8"),
                    new KeyValuePair<string,string>(Localization.GetString("Basket"), "32"),
                    new KeyValuePair<string,string>(Localization.GetString("Bin"), "33"),
                    new KeyValuePair<string,string>(Localization.GetString("Box"), "34"),
                    new KeyValuePair<string,string>(Localization.GetString("Bunch"), "35"),
                    new KeyValuePair<string,string>(Localization.GetString("Bundle"), "10"),
                    new KeyValuePair<string,string>(Localization.GetString("Cabinet"), "36"),
                    new KeyValuePair<string,string>(Localization.GetString("Can"), "11"),
                    new KeyValuePair<string,string>(Localization.GetString("Carboy"), "37"),
                    new KeyValuePair<string,string>(Localization.GetString("Carrier"), "38"),
                    new KeyValuePair<string,string>(Localization.GetString("Carton"), "39"),
                    new KeyValuePair<string,string>(Localization.GetString("Case"), "40"),
                    new KeyValuePair<string,string>(Localization.GetString("Cask"), "54"),
                    new KeyValuePair<string,string>(Localization.GetString("Container"), "41"),
                    new KeyValuePair<string,string>(Localization.GetString("Crate"), "14"),
                    new KeyValuePair<string,string>(Localization.GetString("Cylinder"), "15"),
                    new KeyValuePair<string,string>(Localization.GetString("Drum"), "16"),
                    new KeyValuePair<string,string>(Localization.GetString("Loose"), "42"),
                    new KeyValuePair<string,string>(Localization.GetString("Other"), "99"),
                    new KeyValuePair<string,string>(Localization.GetString("Package"), "43"),
                    new KeyValuePair<string,string>(Localization.GetString("Pail"), "44"),
                    new KeyValuePair<string,string>(Localization.GetString("Pallet"), "18"),
                    new KeyValuePair<string,string>(Localization.GetString("Pieces"), "45"),
                    new KeyValuePair<string,string>(Localization.GetString("PipeLine"), "46"),
                    new KeyValuePair<string,string>(Localization.GetString("Rack"), "53"),
                    new KeyValuePair<string,string>(Localization.GetString("Reel"), "47"),
                    new KeyValuePair<string,string>(Localization.GetString("Roll"), "20"),
                    new KeyValuePair<string,string>(Localization.GetString("Skid"), "48"),
                    new KeyValuePair<string,string>(Localization.GetString("Spool"), "19"),
                    new KeyValuePair<string,string>(Localization.GetString("Tank"), "49"),
                    new KeyValuePair<string,string>(Localization.GetString("Tube"), "3"),
                    new KeyValuePair<string,string>(Localization.GetString("Unit"), "50"),
                    new KeyValuePair<string,string>(Localization.GetString("VanPack"), "51"),
                    new KeyValuePair<string,string>(Localization.GetString("Wrapped"), "52")
                };

                DefaultPackagingField.DataSource = items.OrderBy(i => i.Key);
                DefaultPackagingField.DataTextField = "Key";
                DefaultPackagingField.DataValueField = "Value";
                DefaultPackagingField.DataBind();
            }

            if (DefaultFreightClassField.Items.Count == 0)
            {
                var items = new List<KeyValuePair<string, double>>
                {
                    new KeyValuePair<string,double>("50", 50),
                    new KeyValuePair<string,double>("55", 55),
                    new KeyValuePair<string,double>("60", 60),
                    new KeyValuePair<string,double>("65", 65),
                    new KeyValuePair<string,double>("70", 70),
                    new KeyValuePair<string,double>("77.5", 77.5),
                    new KeyValuePair<string,double>("92.5", 92.5),
                    new KeyValuePair<string,double>("100", 100),
                    new KeyValuePair<string,double>("125", 125),
                    new KeyValuePair<string,double>("150", 150),
                    new KeyValuePair<string,double>("175", 175),
                    new KeyValuePair<string,double>("200", 200),
                    new KeyValuePair<string,double>("250", 250),
                    new KeyValuePair<string,double>("300", 300),
                    new KeyValuePair<string,double>("400", 400),
                    new KeyValuePair<string,double>("500", 500)
                };

                DefaultFreightClassField.DataSource = items.OrderBy(i => i.Value);
                DefaultFreightClassField.DataTextField = "Key";
                DefaultFreightClassField.DataValueField = "Value";
                DefaultFreightClassField.DataBind();
            }
        }
        #endregion
    }
}