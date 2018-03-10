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

        private void LoadZones()
        {
            lstZones.DataSource = HccApp.OrderServices.ShippingZones.FindForStore(HccApp.CurrentStore.Id);
            lstZones.DataTextField = "Name";
            lstZones.DataValueField = "id";
            lstZones.DataBind();
        }

        private void LoadServiceCodes()
        {
            var ups = AvailableServices.FindById(ShippingMethod.ShippingProviderId, HccApp.CurrentStore);
            ShippingTypesRadioButtonList.DataSource = ups.ListAllServiceCodes();
            ShippingTypesRadioButtonList.DataTextField = "DisplayName";
            ShippingTypesRadioButtonList.DataValueField = "Code";
            ShippingTypesRadioButtonList.DataBind();
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
            AccountNumberField.Text = HccApp.CurrentStore.Settings.ShippingUpsAccountNumber;
            ResidentialAddressCheckBox.Checked = HccApp.CurrentStore.Settings.ShippingUpsFreightForceResidential;
            PayerShipmentBillingOptionDropDownList.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightBillingOption.ToString();
            HandlineOneUnitTypeDropDownList.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightHandleOneUnitType;


            
            DefaultPackagingField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightDefaultPackaging.ToString();
            DefaultServiceField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsDefaultService.ToString();
            DefaultFreightClassField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsFreightFreightClass.ToString();

            if (HccApp.CurrentStore.Settings.ShippingUpsLicense.Trim().Length > 0)
            {
                lnkRegister.Text = Localization.GetString("AlreadyRegistered");
            }
            else
            {
                lnkRegister.Text = Localization.GetString("RegisterWithUPS");
            }

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
                PayerShipmentBillingOptionDropDownList.Items.Add(new ListItem("Prepaid", "10"));
                PayerShipmentBillingOptionDropDownList.Items.Add(new ListItem("BilltoThirdParty", "30"));
                PayerShipmentBillingOptionDropDownList.Items.Add(new ListItem("FreightCollect", "40"));

                PayerShipmentBillingOptionDropDownList.Items[0].Selected = true;
            }

            if (HandlineOneUnitTypeDropDownList.Items.Count == 0)
            {
                HandlineOneUnitTypeDropDownList.Items.Add(new ListItem(Localization.GetString("SKID"), "SKD"));
                HandlineOneUnitTypeDropDownList.Items.Add(new ListItem(Localization.GetString("CARBOY"), "CBY"));
                HandlineOneUnitTypeDropDownList.Items.Add(new ListItem(Localization.GetString("PALLET"), "PLT"));
                HandlineOneUnitTypeDropDownList.Items.Add(new ListItem(Localization.GetString("TOTES"), "TOT"));

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
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Bag"), "7"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Bale"), "31"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Barrel"), "8"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Basket"), "32"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Bin"), "33"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Box"), "34"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Bunch"), "35"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Bundle"), "10"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Cabinet"), "36"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Can"), "11"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Carboy"), "37"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Carrier"), "38"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Carton"), "39"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Case"), "40"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Cask"), "54"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Container"), "41"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Crate"), "14"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Cylinder"), "15"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Drum"), "16"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Loose"), "42"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Other"), "99"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Package"), "43"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Pail"), "44"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Pallet"), "18"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Pieces"), "45"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("PipeLine"), "46"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Rack"), "53"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Reel"), "47"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Roll"), "20"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Skid"), "48"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Spool"), "19"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Tank"), "49"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Tube"), "3"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Unit"), "50"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("VanPack"), "51"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("Wrapped"), "52"));
            }

            if (DefaultFreightClassField.Items.Count == 0)
            {
                DefaultFreightClassField.Items.Add(new ListItem("50", "50"));
                DefaultFreightClassField.Items.Add(new ListItem("55", "55"));
                DefaultFreightClassField.Items.Add(new ListItem("60", "60"));
                DefaultFreightClassField.Items.Add(new ListItem("65", "65"));

                DefaultFreightClassField.Items.Add(new ListItem("70", "70"));
                DefaultFreightClassField.Items.Add(new ListItem("77.5", "77.5"));
                DefaultFreightClassField.Items.Add(new ListItem("92.5", "92.5"));
                DefaultFreightClassField.Items.Add(new ListItem("100", "110"));
                DefaultFreightClassField.Items.Add(new ListItem("125", "125"));
                DefaultFreightClassField.Items.Add(new ListItem("150", "150"));
                DefaultFreightClassField.Items.Add(new ListItem("175", "175"));
                DefaultFreightClassField.Items.Add(new ListItem("200", "200"));

                DefaultFreightClassField.Items.Add(new ListItem("200", "200"));
                DefaultFreightClassField.Items.Add(new ListItem("250", "250"));

                DefaultFreightClassField.Items.Add(new ListItem("300", "300"));
                DefaultFreightClassField.Items.Add(new ListItem("400", "400"));

                DefaultFreightClassField.Items.Add(new ListItem("500", "500"));
            }
        }
    }
}