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
using System.Collections.Generic;
using System.Globalization;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;
using Hotcakes.Shipping.Ups;

namespace Hotcakes.Modules.Core.Admin.Parts.Shipping.UPS
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
            ShippingTypesCheckBoxList.DataSource = ups.ListAllServiceCodes();
            ShippingTypesCheckBoxList.DataTextField = "DisplayName";
            ShippingTypesCheckBoxList.DataValueField = "Code";
            ShippingTypesCheckBoxList.DataBind();
        }

        private void LoadData()
        {
            // Name
            NameField.Text = ShippingMethod.Name;

            if (string.IsNullOrEmpty(NameField.Text))
            {
                NameField.Text = Localization.GetString("UPSShipping");
            }

            // Adjustment
            AdjustmentDropDownList.SelectedValue = ((int) ShippingMethod.AdjustmentType).ToString();

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
            ResidentialAddressCheckBox.Checked = HccApp.CurrentStore.Settings.ShippingUpsForceResidential;
            PickupTypeRadioButtonList.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsPickupType.ToString();
            DefaultPackagingField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsDefaultPackaging.ToString();
            DefaultServiceField.SelectedValue = HccApp.CurrentStore.Settings.ShippingUpsDefaultService.ToString();

            //if (HccApp.CurrentStore.Settings.ShippingUpsLicense.Trim().Length > 0)
            //{
            //    lnkRegister.Text = Localization.GetString("AlreadyRegistered");
            //}
            //else
            //{
            //    lnkRegister.Text = Localization.GetString("RegisterWithUPS");
            //}

            SkipDimensionsCheckBox.Checked = HccApp.CurrentStore.Settings.ShippingUpsSkipDimensions;
            chkDiagnostics.Checked = HccApp.CurrentStore.Settings.ShippingUPSDiagnostics;

            // Method Settings
            var Settings = new UPSServiceSettings();
            Settings.Merge(ShippingMethod.Settings);

            if (Settings.ServiceCodeFilter == null)
            {
                Settings.ServiceCodeFilter = new List<IServiceCode>();
            }

            if (Settings.ServiceCodeFilter.Count < 1 || Settings.GetAllRates)
            {
                if (rbFilterMode.Items.FindByValue("1") != null)
                {
                    rbFilterMode.ClearSelection();
                    rbFilterMode.Items.FindByValue("1").Selected = true;
                }
            }
            else
            {
                if (rbFilterMode.Items.FindByValue("0") != null)
                {
                    rbFilterMode.ClearSelection();
                    rbFilterMode.Items.FindByValue("0").Selected = true;
                }

                foreach (ServiceCode code in Settings.ServiceCodeFilter)
                {
                    foreach (ListItem item in ShippingTypesCheckBoxList.Items)
                    {
                        if (string.Compare(item.Value, code.Code, true) == 0)
                        {
                            item.Selected = true;
                            break;
                        }
                    }
                }
            }

            ToggleFilterMode();

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
                (ShippingMethodAdjustmentType) int.Parse(AdjustmentDropDownList.SelectedValue);
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
            HccApp.CurrentStore.Settings.ShippingUpsForceResidential = ResidentialAddressCheckBox.Checked;
            HccApp.CurrentStore.Settings.ShippingUpsPickupType = int.Parse(PickupTypeRadioButtonList.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingUpsDefaultService = int.Parse(DefaultServiceField.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingUpsDefaultPackaging = int.Parse(DefaultPackagingField.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingUpsSkipDimensions = SkipDimensionsCheckBox.Checked;
            HccApp.CurrentStore.Settings.ShippingUPSDiagnostics = chkDiagnostics.Checked;

            // Method Settings
            var Settings = new UPSServiceSettings();
            Settings.Merge(ShippingMethod.Settings);

            var filter = new List<IServiceCode>();

            if (rbFilterMode.SelectedValue == "0")
            {
                Settings.GetAllRates = false;

                foreach (ListItem item in ShippingTypesCheckBoxList.Items)
                {
                    if (item.Selected)
                    {
                        var code = new ServiceCode {Code = item.Value, DisplayName = item.Text};
                        filter.Add(code);
                    }
                }
            }
            else
            {
                Settings.GetAllRates = true;
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

        protected void rbFilterMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            ToggleFilterMode();
        }

        private void ToggleFilterMode()
        {
            if (rbFilterMode.SelectedValue == "1")
            {
                pnlFilter.Visible = false;
            }
            else
            {
                pnlFilter.Visible = true;
            }
        }

        private void LocalizeView()
        {
            if (rbFilterMode.Items.Count == 0)
            {
                rbFilterMode.Items.Add(new ListItem(Localization.GetString("AllAvailable"), "1"));
                rbFilterMode.Items.Add(new ListItem(Localization.GetString("OnlySelected"), "0"));

                rbFilterMode.Items[0].Selected = true;
            }

            rfvAdjustmentTextBox.ErrorMessage = Localization.GetString("rfvAdjustmentTextBox.ErrorMessage");
            cvAdjustmentTextBox.ErrorMessage = Localization.GetString("cvAdjustmentTextBox.ErrorMessage");

            if (AdjustmentDropDownList.Items.Count == 0)
            {
                AdjustmentDropDownList.Items.Add(new ListItem(Localization.GetString("Amount"), "1"));
                AdjustmentDropDownList.Items.Add(new ListItem(Localization.GetString("Percentage"), "2"));

                AdjustmentDropDownList.Items[1].Selected = true;
            }

            if (PickupTypeRadioButtonList.Items.Count == 0)
            {
                // there used to be a third option, but it was removed for API compatibilty
                PickupTypeRadioButtonList.Items.Add(new ListItem
                {
                    Text = Localization.GetString("DailyPickup"),
                    Value = "1",
                    Enabled = true,
                    Selected = true
                });
                PickupTypeRadioButtonList.Items.Add(new ListItem
                {
                    Text = Localization.GetString("CustomerCounter"),
                    Value = "3",
                    Enabled = true,
                    Selected = false
                });
            }

            if (DefaultServiceField.Items.Count == 0)
            {
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSGround"), "3"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSNextDayAir"), "1"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSNextDayAirSaver"), "13"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSNextDayAirEarlyAM"), "14"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPS2ndDayAir"), "2"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPS2ndDayAirAM"), "59"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPS3DaySelect"), "12"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSExpressSaver"), "65"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSWorldwideExpress"), "7"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSWorldwideExpressPlus"), "54"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSWorldwideExpedited"), "8"));
                DefaultServiceField.Items.Add(new ListItem(Localization.GetString("UPSStandard"), "11"));
            }

            if (DefaultPackagingField.Items.Count == 0)
            {
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("CustomerSupplied"), "2"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("UPSLetter"), "1"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("UPSTube"), "3"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("UPSPak"), "4"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("UPSExpressBox"), "21"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("UPS10KgBox"), "25"));
                DefaultPackagingField.Items.Add(new ListItem(Localization.GetString("UPS25KgBox"), "24"));
            }
        }
    }
}