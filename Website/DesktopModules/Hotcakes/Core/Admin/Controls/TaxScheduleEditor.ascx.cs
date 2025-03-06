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
using Hotcakes.Commerce.Taxes;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    public partial class TaxScheduleEditor : HccUserControl
    {
        public long? TaxScheduleId
        {
            get
            {
                if (ViewState["TaxScheduleId"] == null)
                    return null;
                return Convert.ToInt64(ViewState["TaxScheduleId"]);
            }
            set { ViewState["TaxScheduleId"] = value; }
        }

        public bool Save()
        {
            var result = false;

            if (Page.IsValid)
            {
                TaxSchedule ts;
                if (TaxScheduleId.HasValue)
                {
                    ts = HccApp.OrderServices.TaxSchedules.FindForThisStore(TaxScheduleId.Value);
                    ts.Name = txtScheduleName.Text.Trim();
                }
                else
                {
                    ts = new TaxSchedule();
                    ts.Name = txtScheduleName.Text.Trim();
                    HccApp.OrderServices.TaxSchedules.Create(ts);

                    TaxScheduleId = ts.TaxScheduleId();
                }

                decimal defaultRate = 0;
                if (decimal.TryParse(txtDefaultRate.Text.Trim(), out defaultRate))
                {
                    ts.DefaultRate = defaultRate;
                }

                decimal defaultShippingRate = 0;
                if (decimal.TryParse(txtDefaultShippingRate.Text.Trim(), out defaultShippingRate))
                {
                    ts.DefaultShippingRate = defaultShippingRate;
                }

                result = HccApp.OrderServices.TaxSchedules.Update(ts);

                if (!result)
                {
                    ucMessageBox.ShowError("Failed to update tax schedule");
                }
            }

            return result;
        }

        public void LoadSchedule()
        {
            if (TaxScheduleId.HasValue)
            {
                var ts = HccApp.OrderServices.TaxSchedules.FindForThisStore(TaxScheduleId.Value);
                txtScheduleName.Text = ts.Name;
                txtDefaultRate.Text = ts.DefaultRate.ToString("#.000");
                txtDefaultShippingRate.Text = ts.DefaultShippingRate.ToString("#.000");
                LoadRates(TaxScheduleId.Value);
            }
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }

        protected void btnHdnClick_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(hdnTaxScheduleId.Value))
            {
                TaxScheduleId = null;
                ClearControls();
            }
            else
            {
                TaxScheduleId = Convert.ToInt64(hdnTaxScheduleId.Value);
                LoadSchedule();
            }
        }

        #region Event Handlers

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                TaxScheduleId = null;

                LocalizeView();

                PopulateCountries();

                var homeCountry = HccApp.GlobalizationServices.Countries.Find(WebAppSettings.ApplicationCountryBvin);

                ddlCountries.SelectedValue = homeCountry.IsoAlpha3;

                if (!string.IsNullOrEmpty(ddlCountries.SelectedValue))
                {
                    PopulateRegions(ddlCountries.SelectedValue);
                }

                LoadSchedule();
            }
        }

        protected void rptRates_ItemDataBound(object sender, RepeaterItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                var litCountryName = e.Item.FindControl("litCountryName") as Literal;
                var litRegionName = e.Item.FindControl("litRegionName") as Literal;
                var litPostalCode = e.Item.FindControl("litPostalCode") as Literal;
                var litRate = e.Item.FindControl("litRate") as Literal;
                var litShippingRate = e.Item.FindControl("litShippingRate") as Literal;
                var litApplyToShipping = e.Item.FindControl("litApplyToShipping") as Literal;
                var btnDelete = e.Item.FindControl("btnDelete") as LinkButton;

                var tax = e.Item.DataItem as Tax;
                var country = HccApp.GlobalizationServices.Countries.FindByISOCode(tax.CountryIsoAlpha3);
                var region = country != null ? country.FindRegion(tax.RegionAbbreviation) : null;

                litCountryName.Text = country != null ? country.DisplayName : tax.CountryIsoAlpha3;
                litRegionName.Text = region != null ? region.DisplayName : tax.RegionAbbreviation;
                litPostalCode.Text = tax.PostalCode;
                litRate.Text = tax.Rate.ToString("#.000");
                litShippingRate.Text = tax.ShippingRate.ToString("#.000");
                litApplyToShipping.Text = tax.ApplyToShipping ? "Yes" : "No";

                btnDelete.CommandArgument = tax.Id.ToString();
            }
        }

        protected void ddlCountries_SelectedIndexChanged(object Sender, EventArgs E)
        {
            PopulateRegions(ddlCountries.SelectedValue);
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (!TaxScheduleId.HasValue)
                {
                    Save();
                }

                var t = new Tax();
                t.CountryIsoAlpha3 = ddlCountries.SelectedValue;
                t.ApplyToShipping = chkApplyToShipping.Checked;
                t.PostalCode = txtPostalCode.Text.Trim();
                t.Rate = decimal.Parse(txtRate.Text.Trim());
                t.ShippingRate = decimal.Parse(txtShippingRate.Text.Trim());
                t.RegionAbbreviation = ddlRegions.SelectedValue;
                t.StoreId = HccApp.CurrentStore.Id;
                t.TaxScheduleId = TaxScheduleId.Value;

                HccApp.OrderServices.Taxes.Create(t);
                LoadRates(TaxScheduleId.Value);
            }
        }

        protected void btnDelete_Command(object sender, CommandEventArgs e)
        {
            var taxId = long.Parse(e.CommandArgument.ToString());
            HccApp.OrderServices.Taxes.Delete(taxId);

            LoadRates(TaxScheduleId.Value);
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            divDefaultRate.Visible = HccApp.CurrentStore.Settings.ApplyVATRules;
            divDefaultShippingRate.Visible = HccApp.CurrentStore.Settings.ApplyVATRules;
        }

        #endregion

        #region Implementation

        private void ClearControls()
        {
            txtScheduleName.Text = string.Empty;
            txtDefaultRate.Text = string.Empty;
            txtDefaultShippingRate.Text = string.Empty;

            txtPostalCode.Text = string.Empty;
            txtRate.Text = string.Empty;
            txtShippingRate.Text = string.Empty;

            rptRates.DataSource = null;
            rptRates.DataBind();
        }

        private void PopulateCountries()
        {
            ddlCountries.DataSource = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            ddlCountries.DataValueField = "IsoAlpha3";
            ddlCountries.DataTextField = "DisplayName";
            ddlCountries.DataBind();
        }

        private void PopulateRegions(string isoAlpha3)
        {
            ddlRegions.Items.Clear();
            var country = HccApp.GlobalizationServices.Countries.FindByISOCode(isoAlpha3);
            var regions = country != null ? country.Regions : null;
            ddlRegions.DataSource = regions;
            ddlRegions.DataTextField = "DisplayName";
            ddlRegions.DataValueField = "Abbreviation";
            ddlRegions.DataBind();

            var li = new ListItem("- All States/Regions -", string.Empty);
            ddlRegions.Items.Insert(0, li);
        }

        private void LoadRates(long taxScheduleId)
        {
            var taxRates = HccApp.OrderServices.Taxes.FindByTaxSchedule(HccApp.CurrentStore.Id, taxScheduleId);

            rptRates.DataSource = taxRates;
            rptRates.DataBind();

            txtPostalCode.Text = string.Empty;
            txtRate.Text = string.Empty;
            txtShippingRate.Text = string.Empty;
        }

        private void LocalizeView()
        {
            rfvRate.ErrorMessage = Localization.GetString("rfvRate.ErrorMessage");
            cvRate.ErrorMessage = Localization.GetString("cvRate.ErrorMessage");
            rfvShippingRate.ErrorMessage = Localization.GetString("rfvShippingRate.ErrorMessage");
            cvShippingRate.ErrorMessage = Localization.GetString("cvShippingRate.ErrorMessage");
            rvDefaultRate.ErrorMessage = Localization.GetString("rvDefaultRate.ErrorMessage");
            rvDefaultShippingRate.ErrorMessage = Localization.GetString("rvDefaultShippingRate.ErrorMessage");
            rfvScheduleName.ErrorMessage = Localization.GetString("rfvScheduleName.ErrorMessage");
            rfvScheduleNameWithGroup.ErrorMessage = Localization.GetString("rfvScheduleNameWithGroup.ErrorMessage");
        }

        #endregion
    }
}