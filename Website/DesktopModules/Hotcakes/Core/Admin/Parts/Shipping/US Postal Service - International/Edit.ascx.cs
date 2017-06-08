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
using Hotcakes.Shipping.USPostal;

namespace Hotcakes.Modules.Core.Modules.Shipping.US_Postal_Service___International
{
    public partial class Edit : HccShippingPart
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
            if (Page.IsValid)
            {
                SaveData();
                NotifyFinishedEditing(NameField.Text.Trim());
            }
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
            var uspostal = AvailableServices.FindById(ShippingMethod.ShippingProviderId, HccApp.CurrentStore);
            ShippingTypesCheckBoxList.DataSource = uspostal.ListAllServiceCodes();
            ShippingTypesCheckBoxList.DataTextField = "DisplayName";
            ShippingTypesCheckBoxList.DataValueField = "Code";
            ShippingTypesCheckBoxList.DataBind();
        }

        private void LoadData()
        {
            NameField.Text = ShippingMethod.Name;

            if (NameField.Text == string.Empty)
            {
                NameField.Text = Localization.GetString("USPostalServiceInternational");
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

            // Global
            txtUserId.Text = HccApp.CurrentStore.Settings.ShippingUSPostalUserId;
            chbDiagnostics.Checked = HccApp.CurrentStore.Settings.ShippingUSPostalDiagnostics;

            var settings = new USPostalServiceSettings();
            settings.Merge(ShippingMethod.Settings);

            foreach (ServiceCode code in settings.ServiceCodeFilter)
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

            // Select Hightlights
            var highlight = settings.GetSettingOrEmpty("highlight");

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
            HccApp.CurrentStore.Settings.ShippingUSPostalUserId = txtUserId.Text.Trim();
            HccApp.CurrentStore.Settings.ShippingUSPostalDiagnostics = chbDiagnostics.Checked;
            HccApp.UpdateCurrentStore();

            // Method Settings
            var Settings = new USPostalServiceSettings();

            Settings.Merge(ShippingMethod.Settings);

            // Service Code
            var filter = new List<IServiceCode>();

            foreach (ListItem item in ShippingTypesCheckBoxList.Items)
            {
                if (item.Selected)
                {
                    var code = new ServiceCode {Code = item.Value, DisplayName = item.Text};
                    filter.Add(code);
                }
            }

            Settings.ServiceCodeFilter = filter;

            Settings["highlight"] = lstHighlights.SelectedValue;
            ShippingMethod.Settings.Merge(Settings);
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

                AdjustmentDropDownList.Items[0].Selected = true;
            }
        }
    }
}