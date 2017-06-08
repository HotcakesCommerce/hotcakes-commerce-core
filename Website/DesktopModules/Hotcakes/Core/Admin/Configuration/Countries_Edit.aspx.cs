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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Data;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class Countries_Edit : BaseAdminPage
    {
        public string CountryId
        {
            get { return Request.Params["countryId"]; }
        }

        private Guid RegionId
        {
            get { return ViewState["RegionId"] != null ? (Guid) ViewState["RegionId"] : Guid.Empty; }
            set { ViewState["RegionId"] = value; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("EditCountry");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LocalizeView();

                FillData();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var country = HccApp.GlobalizationServices.Countries.Find(CountryId);
                if (country == null)
                    country = new Country();

                country.SystemName = txtSystemName.Text.Trim();
                country.IsoCode = txtIsoCode.Text.Trim();
                country.IsoAlpha3 = txtIsoAlpha3.Text.Trim();
                country.IsoNumeric = txtIsoNumeric.Text.Trim();
                country.DisplayName = txtDisplayName.Text.Trim();
                if (string.IsNullOrEmpty(CountryId))
                {
                    HccApp.GlobalizationServices.Countries.Create(country);
                    Response.Redirect(string.Format("Countries_Edit.aspx?countryId={0}", country.Bvin));
                }
                else
                {
                    HccApp.GlobalizationServices.Countries.Update(country);

                    ucMessageBox.ShowOk(Localization.GetString("CountrySaveSuccessful"));
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Countries.aspx");
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            txtRegionAbbreviation.Text = string.Empty;
            txtRegionSystemName.Text = string.Empty;
            txtRegionDisplayName.Text = string.Empty;

            ShowEditor(true);
        }

        protected void gvRegions_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            RegionId = (Guid) gvRegions.DataKeys[e.NewEditIndex].Value;

            var country = HccApp.GlobalizationServices.Countries.Find(CountryId);
            var region = country.FindRegion(RegionId);

            txtRegionAbbreviation.Text = region.Abbreviation;
            txtRegionSystemName.Text = region.SystemName;
            txtRegionDisplayName.Text = region.DisplayName;

            ShowEditor(true);
        }

        protected void gvRegions_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;

            var regionId = (Guid) gvRegions.DataKeys[e.RowIndex].Value;
            HccApp.GlobalizationServices.Regions.Delete(regionId);

            FillData();
        }

        protected void btnUpdateRegion_Click(object sender, EventArgs e)
        {
            var country = HccApp.GlobalizationServices.Countries.Find(CountryId);
            var region = country.FindRegion(RegionId);
            if (region == null)
            {
                region = new Region {CountryId = DataTypeHelper.BvinToGuid(CountryId)};
            }

            region.Abbreviation = txtRegionAbbreviation.Text.Trim();
            region.SystemName = txtRegionSystemName.Text.Trim();
            region.DisplayName = txtRegionDisplayName.Text.Trim();

            if (RegionId == Guid.Empty)
            {
                HccApp.GlobalizationServices.Regions.Create(region);
            }
            else
                HccApp.GlobalizationServices.Regions.Update(region);

            ucMessageBox.ShowOk(Localization.GetString("RegionSaveSuccessful"));

            ShowEditor(false);

            FillData();
        }

        protected void btnCancelUpdateRegion_Click(object sender, EventArgs e)
        {
            ShowEditor(false);

            FillData();
        }

        protected void gvRegions_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Abbreviation");
                e.Row.Cells[1].Text = Localization.GetString("SystemName");
                e.Row.Cells[2].Text = Localization.GetString("DisplayName");
            }
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }

        #region Private

        private void FillData()
        {
            var createMode = string.IsNullOrEmpty(CountryId);
            btnCreate.Visible = !createMode;
            divRegion.Visible = !createMode;

            var country = HccApp.GlobalizationServices.Countries.Find(CountryId);
            if (country != null)
            {
                LoadCountry(country);
                LoadRegions(country);
            }
        }

        private void LoadCountry(Country country)
        {
            txtSystemName.Text = country.SystemName;
            txtIsoCode.Text = country.IsoCode;
            txtIsoAlpha3.Text = country.IsoAlpha3;
            txtIsoNumeric.Text = country.IsoNumeric;
            txtDisplayName.Text = country.DisplayName;
        }

        private void LoadRegions(Country country)
        {
            gvRegions.DataSource = country.Regions;
            gvRegions.DataBind();
        }

        private void ShowEditor(bool show)
        {
            pnlEditRegion.Visible = show;

            if (show)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "hcEditRegionDialog", "hcEditRegionDialog();", true);
            }
        }

        private void LocalizeView()
        {
            rfvSystemName.ErrorMessage = Localization.GetString("rfvSystemName.ErrorMessage");
            rfvIsoCode.ErrorMessage = Localization.GetString("rfvIsoCode.ErrorMessage");
            rfvIsoAlpha3.ErrorMessage = Localization.GetString("rfvIsoAlpha3.ErrorMessage");
            rfvIsoNumeric.ErrorMessage = Localization.GetString("rfvIsoNumeric.ErrorMessage");
            rfvDisplayName.ErrorMessage = Localization.GetString("rfvDisplayName.ErrorMessage");
        }

        #endregion
    }
}