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
using System.Globalization;
using System.Text;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;
using Hotcakes.Shipping.FedEx;
using Hotcakes.Web.Logging;
using Address = Hotcakes.Commerce.Contacts.Address;
using WeightType = Hotcakes.Shipping.WeightType;

namespace Hotcakes.Modules.Core.Admin.Parts.Shipping.FedEx
{
    partial class Edit : HccShippingPart
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LocalizeView();

            AddHighlightColors(lstHighlights);
            PopulateLists();
            LoadZones();
            LoadData();

            var sourceAddress = HccApp.ContactServices.Addresses.FindStoreContactAddress();
            SourceAddress.LoadFromAddress(sourceAddress);

            var destinationAdress = new Address();
            destinationAdress.CountryBvin = Country.UnitedStatesCountryBvin;
            destinationAdress.Line1 = "319 N. Clematis St.";
            destinationAdress.Line2 = "Suite 500";
            destinationAdress.RegionBvin = "FL";
            destinationAdress.City = "West Palm Beach";
            destinationAdress.PostalCode = "33401";
            DestinationAddress.LoadFromAddress(destinationAdress);
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

        private void PopulateLists()
        {
            var provider = AvailableServices.FindById(ShippingMethod.ShippingProviderId, HccApp.CurrentStore);
            if (provider != null)
            {
                var codes = provider.ListAllServiceCodes();
                lstServiceCode.Items.Clear();
                foreach (var code in codes)
                {
                    lstServiceCode.Items.Add(new ListItem(code.DisplayName, code.Code));
                    lstServicesTest.Items.Add(new ListItem(code.DisplayName, code.Code));
                }
            }
        }

        private void LoadData()
        {
            // Method Settings
            var Settings = new FedExServiceSettings();
            Settings.Merge(ShippingMethod.Settings);

            NameField.Text = string.IsNullOrEmpty(ShippingMethod.Name) ? "FedEx" : ShippingMethod.Name;

            if (lstServiceCode.Items.FindByValue(Settings.ServiceCode.ToString()) != null)
            {
                lstServiceCode.ClearSelection();
                lstServiceCode.Items.FindByValue(Settings.ServiceCode.ToString()).Selected = true;
            }

            AdjustmentDropDownList.SelectedValue = ((int) ShippingMethod.AdjustmentType).ToString();
            AdjustmentTextBox.Text =
                string.Format(ShippingMethod.AdjustmentType == ShippingMethodAdjustmentType.Amount ? "{0:c}" : "{0:f}",
                    ShippingMethod.Adjustment);

            // set the negotiated rates to true, or the setting in the local settings
            chkNegotiatedRates.Checked = !Settings.ContainsKey("UseNegotiatedRates") ||
                                         bool.Parse(Settings["UseNegotiatedRates"]);

            // Zones
            if (lstZones.Items.FindByValue(ShippingMethod.ZoneId.ToString()) != null)
            {
                lstZones.ClearSelection();
                lstZones.Items.FindByValue(ShippingMethod.ZoneId.ToString()).Selected = true;
            }

            // Globals
            if (lstPackaging.Items.FindByValue(HccApp.CurrentStore.Settings.ShippingFedExDefaultPackaging.ToString()) !=
                null)
            {
                lstPackaging.ClearSelection();
                lstPackaging.Items.FindByValue(HccApp.CurrentStore.Settings.ShippingFedExDefaultPackaging.ToString())
                    .Selected = true;
            }

            if (lstPackaging.Items.FindByValue(Settings.Packaging.ToString()) != null)
            {
                lstPackaging.ClearSelection();
                lstPackaging.Items.FindByValue(Settings.Packaging.ToString()).Selected = true;
            }

            KeyField.Text = HccApp.CurrentStore.Settings.ShippingFedExKey;
            PasswordField.Text = HccApp.CurrentStore.Settings.ShippingFedExPassword;
            AccountNumberField.Text = HccApp.CurrentStore.Settings.ShippingFedExAccountNumber;
            MeterNumberField.Text = HccApp.CurrentStore.Settings.ShippingFedExMeterNumber;

            if (
                lstDefaultPackaging.Items.FindByValue(
                    HccApp.CurrentStore.Settings.ShippingFedExDefaultPackaging.ToString()) != null)
            {
                lstDefaultPackaging.ClearSelection();
                lstDefaultPackaging.Items.FindByValue(
                    HccApp.CurrentStore.Settings.ShippingFedExDefaultPackaging.ToString()).Selected = true;
            }

            if (lstDropOffType.Items.FindByValue(HccApp.CurrentStore.Settings.ShippingFedExDropOffType.ToString()) !=
                null)
            {
                lstDropOffType.ClearSelection();
                lstDropOffType.Items.FindByValue(HccApp.CurrentStore.Settings.ShippingFedExDropOffType.ToString())
                    .Selected = true;
            }

            chkResidential.Checked = HccApp.CurrentStore.Settings.ShippingFedExForceResidentialRates;

            chkDiagnostics.Checked = HccApp.CurrentStore.Settings.ShippingFedExDiagnostics;

            chkDevelopmentUrl.Checked = HccApp.CurrentStore.Settings.ShippingFedExUseDevelopmentServiceUrl;

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
            ShippingMethod.ZoneId = long.Parse(lstZones.SelectedValue);
            ShippingMethod.AdjustmentType =
                (ShippingMethodAdjustmentType) int.Parse(AdjustmentDropDownList.SelectedValue);
            ShippingMethod.Adjustment = decimal.Parse(AdjustmentTextBox.Text, NumberStyles.Currency);

            if (ShippingMethod.AdjustmentType == ShippingMethodAdjustmentType.Amount)
            {
                ShippingMethod.Adjustment = Money.RoundCurrency(ShippingMethod.Adjustment);
            }

            // Method Settings
            var Settings = new FedExServiceSettings();
            Settings.Merge(ShippingMethod.Settings);
            Settings.ServiceCode = int.Parse(lstServiceCode.SelectedValue);
            Settings.Packaging = int.Parse(lstPackaging.SelectedValue);
            Settings["highlight"] = lstHighlights.SelectedValue;
            Settings["UseNegotiatedRates"] = chkNegotiatedRates.Checked.ToString();
            ShippingMethod.Settings.Merge(Settings);

            // Globals
            HccApp.CurrentStore.Settings.ShippingFedExKey = KeyField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingFedExPassword = PasswordField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingFedExAccountNumber = AccountNumberField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingFedExMeterNumber = MeterNumberField.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingFedExDefaultPackaging = int.Parse(lstDefaultPackaging.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingFedExDropOffType = int.Parse(lstDropOffType.SelectedValue);
            HccApp.CurrentStore.Settings.ShippingFedExForceResidentialRates = chkResidential.Checked;
            HccApp.CurrentStore.Settings.ShippingFedExDiagnostics = chkDiagnostics.Checked;
            HccApp.CurrentStore.Settings.ShippingFedExUseDevelopmentServiceUrl = chkDevelopmentUrl.Checked;

            HccApp.UpdateCurrentStore();
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

        protected void btnTest_Click(object sender, EventArgs e)
        {
            SaveData();

            var testSettings = new FedExGlobalServiceSettings
            {
                AccountNumber = AccountNumberField.Text,
                DefaultDropOffType = (DropOffType) int.Parse(lstDropOffType.SelectedValue),
                DefaultPackaging = (PackageType) int.Parse(lstPackaging.SelectedValue),
                DiagnosticsMode = true,
                ForceResidentialRates = chkResidential.Checked,
                MeterNumber = MeterNumberField.Text.Trim(),
                UserKey = KeyField.Text.Trim(),
                UserPassword = PasswordField.Text.Trim(),
                UseDevelopmentServiceUrl = chkDevelopmentUrl.Checked
            };

            var logger = new TextLogger();

            var testSvc = new FedExProvider(testSettings, logger)
            {
                Settings =
                {
                    ServiceCode = int.Parse(lstServicesTest.SelectedValue),
                    Packaging = (int) testSettings.DefaultPackaging
                }
            };

            var testShipment = new Shipment
            {
                DestinationAddress = DestinationAddress.GetAsAddress(),
                SourceAddress = SourceAddress.GetAsAddress()
            };

            var testItem = new Shippable
            {
                BoxHeight = decimal.Parse(TestHeight.Text),
                BoxLength = decimal.Parse(TestLength.Text),
                BoxWidth = decimal.Parse(TestWidth.Text),
                BoxLengthType = LengthType.Inches,
                BoxWeight = decimal.Parse(TestWeight.Text),
                BoxWeightType = WeightType.Pounds
            };
            
            testShipment.Items.Add(testItem);

            var sb = new StringBuilder();
            sb.AppendFormat(Localization.GetString("StartingTest"), DateTime.Now);
            sb.Append("<br />");
            
            var rates = testSvc.RateShipment(testShipment);
            foreach (var r in rates)
            {
                sb.AppendFormat(Localization.GetString("RateFound"), r.EstimatedCost.ToString("C"), r.DisplayName,
                    r.ServiceCodes, r.ServiceId);
                sb.Append("<br />");
            }
            sb.Append("<br />");
            sb.Append(Localization.GetString("Log"));
            sb.Append(":<br />");
            foreach (var m in logger.Messages)
            {
                sb.Append(m + "<br />");
            }
            sb.AppendFormat(Localization.GetString("FinishedTest"), DateTime.Now);

            litTestOuput.Text = sb.ToString();
        }

        private void LocalizeView()
        {
            if (lstPackaging.Items.Count == 0)
            {
                lstPackaging.Items.Add(new ListItem(Localization.GetString("FedExEnvelope"), "1"));
                lstPackaging.Items.Add(new ListItem(Localization.GetString("FedExPak"), "2"));
                lstPackaging.Items.Add(new ListItem(Localization.GetString("FedExBox"), "3"));
                lstPackaging.Items.Add(new ListItem(Localization.GetString("FedExTube"), "4"));
                lstPackaging.Items.Add(new ListItem(Localization.GetString("FedEx25kgBox"), "5"));
                lstPackaging.Items.Add(new ListItem(Localization.GetString("FedEx10kgBox"), "6"));
                lstPackaging.Items.Add(new ListItem(Localization.GetString("YourPackaging"), "7"));
            }

            rfvAdjustmentTextBox.ErrorMessage = Localization.GetString("rfvAdjustmentTextBox.ErrorMessage");
            cvAdjustmentTextBox.ErrorMessage = Localization.GetString("cvAdjustmentTextBox.ErrorMessage");

            if (AdjustmentDropDownList.Items.Count == 0)
            {
                AdjustmentDropDownList.Items.Add(new ListItem(Localization.GetString("Amount"), "1"));
                AdjustmentDropDownList.Items.Add(new ListItem(Localization.GetString("Percentage"), "2"));

                AdjustmentDropDownList.Items[0].Selected = true;
            }

            if (lstDefaultPackaging.Items.Count == 0)
            {
                foreach (var item in lstPackaging.Items)
                {
                    var listItem = (ListItem) item;
                    lstDefaultPackaging.Items.Add(new ListItem(listItem.Text, listItem.Value));
                }
            }

            if (lstDropOffType.Items.Count == 0)
            {
                lstDropOffType.Items.Add(new ListItem(Localization.GetString("RegularPickup"), "1"));
                lstDropOffType.Items.Add(new ListItem(Localization.GetString("RequestCourier"), "2"));
                lstDropOffType.Items.Add(new ListItem(Localization.GetString("DropBox"), "3"));
                lstDropOffType.Items.Add(new ListItem(Localization.GetString("BusinessServiceCenter"), "4"));
                lstDropOffType.Items.Add(new ListItem(Localization.GetString("Station"), "5"));
            }
        }
    }
}