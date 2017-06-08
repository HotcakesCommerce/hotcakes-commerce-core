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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    public partial class Shipping_Zones_Edit : BaseAdminPage
    {
        #region Fields

        private Zone _zone;

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("EditShippingZone");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvAreas.RowDeleting += gvAreas_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            ucMessageBox.ClearMessage();

            var id = Request.QueryString["id"];
            _zone = HccApp.OrderServices.ShippingZones.Find(long.Parse(id));

            if (!IsPostBack)
            {
                LoadZone();

                PopulateCountries();
                lstCountry.SelectedValue = WebAppSettings.ApplicationCountryBvin;
                PopulateRegions(lstCountry.SelectedValue);
            }
        }

        private void gvAreas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var countryIso = e.Keys[0] as string;
            var region = e.Keys[1] as string;
            HccApp.OrderServices.ShippingZoneRemoveArea(_zone.Id, countryIso, region);
            LoadZone();
        }

        protected void lstCountry_SelectedIndexChanged(object Sender, EventArgs e)
        {
            PopulateRegions(lstCountry.SelectedValue);
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                Response.Redirect("Shipping_Zones.aspx");
            }
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var a = new ZoneArea
            {
                CountryIsoAlpha3 = lstCountry.SelectedItem.Value,
                RegionAbbreviation = lstState.SelectedItem.Value
            };

            _zone.Areas.Add(a);
            HccApp.OrderServices.ShippingZones.Update(_zone);
            LoadZone();

            ucMessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
        }

        protected void gvAreas_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("Country");
                e.Row.Cells[1].Text = Localization.GetString("Region");
            }
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("DeleteConfirmation"));
        }

        #endregion

        #region Implementation

        private void LoadZone()
        {
            _zone = HccApp.OrderServices.ShippingZones.Find(_zone.Id);

            if (_zone.StoreId == HccApp.CurrentStore.Id)
            {
                ZoneNameField.Text = _zone.Name;
                gvAreas.DataSource = _zone.Areas;
                gvAreas.DataBind();
            }
            else
            {
                Response.Redirect("Shipping_Zones.aspx");
            }
        }

        protected string GetCountryName(IDataItemContainer cont)
        {
            var a = cont.DataItem as ZoneArea;
            var country = HccApp.GlobalizationServices.Countries.FindByISOCode(a.CountryIsoAlpha3);
            return country.DisplayName;
        }

        private void PopulateCountries()
        {
            try
            {
                lstCountry.DataSource = HccApp.GlobalizationServices.Countries.FindActiveCountries();
                lstCountry.DataValueField = "IsoAlpha3";
                lstCountry.DataTextField = "DisplayName";
                lstCountry.DataBind();
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Localization.GetString("CountryException") + Ex.Message);
            }
        }

        private void PopulateRegions(string sID)
        {
            lstState.Items.Clear();

            try
            {
                var country = HccApp.GlobalizationServices.Countries.FindByISOCode(sID);
                var regions = country.Regions;

                lstState.DataSource = regions;
                lstState.DataTextField = "DisplayName";
                lstState.DataValueField = "Abbreviation";
                lstState.DataBind();

                var li = new ListItem(Localization.GetString("AllStatesRegions"), string.Empty);

                lstState.Items.Insert(0, li);
            }
            catch (Exception Ex)
            {
                throw new ArgumentException(Ex.Message);
            }
        }

        private bool Save()
        {
            _zone.Name = ZoneNameField.Text;
            return HccApp.OrderServices.ShippingZones.Update(_zone);
        }

        #endregion
    }
}