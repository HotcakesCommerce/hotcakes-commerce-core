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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Shipping;
using Hotcakes.Shipping.Services;

namespace Hotcakes.Modules.Core.Modules.Shipping.Rate_Table_Per_Item
{
    partial class Edit : HccShippingPart
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            LocalizeView();

            AddHighlightColors(lstHighlights);
            LoadZones();
            LoadData();
            LoadLevels();
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

        private void LoadData()
        {
            NameField.Text = ShippingMethod.Name;

            if (NameField.Text == string.Empty)
            {
                NameField.Text = Localization.GetString("RateTablePerItem");
            }

            AdjustmentDropDownList.SelectedValue = ((int) ShippingMethod.AdjustmentType).ToString();

            if (ShippingMethod.AdjustmentType == ShippingMethodAdjustmentType.Amount)
            {
                AdjustmentTextBox.Text = string.Format("{0:c}", ShippingMethod.Adjustment);
            }
            else
            {
                AdjustmentTextBox.Text = string.Format("{0:f}", ShippingMethod.Adjustment);
            }

            // ZONES
            if (lstZones.Items.FindByValue(ShippingMethod.ZoneId.ToString()) != null)
            {
                lstZones.ClearSelection();
                lstZones.Items.FindByValue(ShippingMethod.ZoneId.ToString()).Selected = true;
            }

            // Select Hightlights
            var highlight = ShippingMethod.Settings.GetSettingOrEmpty("highlight");

            if (lstHighlights.Items.FindByText(highlight) != null)
            {
                lstHighlights.ClearSelection();
                lstHighlights.Items.FindByText(highlight).Selected = true;
            }
        }

        private void LoadLevels()
        {
            var settings = new RateTableSettings();

            settings.Merge(ShippingMethod.Settings);

            var levels = settings.GetLevels();

            gvRates.DataSource = levels;
            gvRates.DataBind();
        }

        private void SaveData()
        {
            ShippingMethod.Name = NameField.Text.Trim();
            ShippingMethod.AdjustmentType =
                (ShippingMethodAdjustmentType) int.Parse(AdjustmentDropDownList.SelectedValue);
            ShippingMethod.Adjustment = decimal.Parse(AdjustmentTextBox.Text, NumberStyles.Currency);

            if (ShippingMethod.AdjustmentType == ShippingMethodAdjustmentType.Amount)
            {
                ShippingMethod.Adjustment = Money.RoundCurrency(ShippingMethod.Adjustment);
            }

            ShippingMethod.ZoneId = long.Parse(lstZones.SelectedItem.Value);
            ShippingMethod.Settings["highlight"] = lstHighlights.SelectedValue;
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var r = new RateTableLevel
            {
                Level = decimal.Parse(NewLevelField.Text),
                Rate = decimal.Parse(NewAmountField.Text)
            };

            var settings = new RateTableSettings();

            settings.Merge(ShippingMethod.Settings);

            settings.AddLevel(r);

            ShippingMethod.Settings = settings;

            HccApp.OrderServices.ShippingMethods.Update(ShippingMethod);

            LoadLevels();
        }

        private void RemoveLevel(string level, string rate)
        {
            var settings = new RateTableSettings();

            settings.Merge(ShippingMethod.Settings);

            var r = new RateTableLevel
            {
                Level = decimal.Parse(level),
                Rate = Money.RoundCurrency(decimal.Parse(rate, NumberStyles.Currency))
            };

            settings.RemoveLevel(r);

            ShippingMethod.Settings = settings;

            HccApp.OrderServices.ShippingMethods.Update(ShippingMethod);

            LoadLevels();
        }

        protected void gvRates_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var lblLevel = (Label) gvRates.Rows[e.RowIndex].FindControl("lblLevel");
            var lblRate = (Label) gvRates.Rows[e.RowIndex].FindControl("lblAmount");

            if (lblLevel != null)
            {
                if (lblRate != null)
                {
                    RemoveLevel(lblLevel.Text, lblRate.Text);
                }
            }
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

            rfvNewLevelField.ErrorMessage = Localization.GetString("rfvNewLevelField.ErrorMessage");
            cvNewLevelField.ErrorMessage = Localization.GetString("cvNewLevelField.ErrorMessage");
            rfvNewAmountField.ErrorMessage = Localization.GetString("rfvNewAmountField.ErrorMessage");
            cvNewAmountField.ErrorMessage = Localization.GetString("cvNewAmountField.ErrorMessage");
        }

        protected void gvRates_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("TotalItemCountAtLeast");
                e.Row.Cells[1].Text = Localization.GetString("ChargeAmountPerItem");
            }

            rfvNewLevelField.ErrorMessage = Localization.GetString("rfvNewLevelField.ErrorMessage");
            cvNewLevelField.ErrorMessage = Localization.GetString("cvNewLevelField.ErrorMessage");
            rfvNewAmountField.ErrorMessage = Localization.GetString("rfvNewAmountField.ErrorMessage");
            cvNewAmountField.ErrorMessage = Localization.GetString("cvNewAmountField.ErrorMessage");
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }
    }
}