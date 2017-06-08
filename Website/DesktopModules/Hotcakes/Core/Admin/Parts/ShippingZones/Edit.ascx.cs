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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Parts.ShippingZones
{
    public partial class Edit : HccPart
    {
        private Zone _shippingZone;
        public long ShippingZoneId { get; set; }

        protected Zone GetShippingZone(bool forceReload)
        {
            if (_shippingZone == null || _shippingZone.Id != ShippingZoneId || forceReload)
                _shippingZone = HccApp.OrderServices.ShippingZones.Find(ShippingZoneId);

            return _shippingZone;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            var zone = GetShippingZone(true);

            if (zone == null)
            {
                throw new ArgumentException(Localization.GetString("ShippingZoneIdIsIncorrect"));
            }

            ZoneNameField.Text = zone.Name;

            PopulateCountries();
            LoadZoneAreas(false);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            PopulateRegions(lstCountry.SelectedValue);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            SaveData();
            NotifyFinishedEditing( /*this.NameField.Text.Trim()*/);
        }

        private bool SaveData()
        {
            var zone = GetShippingZone(false);
            zone.Name = ZoneNameField.Text;
            return HccApp.OrderServices.ShippingZones.Update(zone);
        }

        private void LoadZoneAreas(bool forceReload)
        {
            var zone = GetShippingZone(forceReload);

            gridZoneAreas.DataSource = zone.Areas;
            gridZoneAreas.DataBind();
        }

        private void PopulateCountries()
        {
            lstCountry.DataSource = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            lstCountry.DataValueField = "IsoAlpha3";
            lstCountry.DataTextField = "DisplayName";
            lstCountry.DataBind();
        }

        private void PopulateRegions(string isoCode)
        {
            var country = HccApp.GlobalizationServices.Countries.FindByISOCode(isoCode);
            var regions = country.Regions;
            lstState.DataSource = regions;
            lstState.DataTextField = "DisplayName";
            lstState.DataValueField = "Abbreviation";
            lstState.DataBind();

            var li = new ListItem(Localization.GetString("AllStatesRegions"), string.Empty);
            lstState.Items.Insert(0, li);
        }

        protected void gridZoneAreas_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var areaInfo = gridZoneAreas.DataKeys[e.RowIndex].Values;
            HccApp.OrderServices.ShippingZoneRemoveArea(ShippingZoneId, Convert.ToString(areaInfo["CountryIsoAlpha3"]),
                Convert.ToString(areaInfo["RegionAbbreviation"]));
            LoadZoneAreas(true);
        }

        protected void lstCountry_SelectedIndexChanged(object sender, EventArgs e)
        {
            PopulateRegions(lstCountry.SelectedItem.Value);
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            var zone = GetShippingZone(false);

            var area = new ZoneArea
            {
                CountryIsoAlpha3 = lstCountry.SelectedItem.Value,
                RegionAbbreviation = lstState.SelectedItem.Value
            };

            zone.Areas.Add(area);
            HccApp.OrderServices.ShippingZones.Update(zone);
            LoadZoneAreas(false);
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
        }

        protected void gridZoneAreas_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("AreasInThisZone");
            }
        }
    }
}