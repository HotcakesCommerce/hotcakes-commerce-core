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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Countries : BaseAdminPage
    {
        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("Countries");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadCountries();
            }
        }

        private void LoadCountries()
        {
            var allCountries = HccApp.GlobalizationServices.Countries.FindAll();
            gvCountries.DataSource = allCountries;
            gvCountries.DataBind();

            var disabledList = HccApp.CurrentStore.Settings.DisabledCountryIso3Codes;
            foreach (GridViewRow row in gvCountries.Rows)
            {
                var chbEnabled = row.FindControl("chbEnabled") as CheckBox;
                var btnDelete = row.FindControl("btnDelete") as LinkButton;

                var country = allCountries[row.DataItemIndex];
                if (!disabledList.Contains(country.IsoAlpha3))
                    chbEnabled.Checked = true;

                var isUSorCA = country.IsoCode == "US";
                btnDelete.Visible = !isUSorCA;
            }
        }

        protected void gvCountries_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            var counryId = (string) gvCountries.DataKeys[e.NewEditIndex].Value;
            Response.Redirect(string.Format("Countries_Edit.aspx?countryId={0}", counryId));
        }

        protected void gvCountries_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            e.Cancel = true;

            var countryId = (string) gvCountries.DataKeys[e.RowIndex].Value;
            HccApp.GlobalizationServices.Countries.Delete(countryId);

            LoadCountries();
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var allCountries = HccApp.GlobalizationServices.Countries.FindAll();

                var iso3Codes = new List<string>();
                foreach (GridViewRow row in gvCountries.Rows)
                {
                    var chbEnabled = row.FindControl("chbEnabled") as CheckBox;
                    if (!chbEnabled.Checked)
                    {
                        var country = allCountries[row.DataItemIndex];
                        iso3Codes.Add(country.IsoAlpha3);
                    }
                }

                HccApp.CurrentStore.Settings.DisabledCountryIso3Codes = iso3Codes;
                HccApp.UpdateCurrentStore();

                ucMessageBox.ShowOk(Localization.GetString("SettingsSuccessful"));
            }
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            Response.Redirect("Countries_Edit.aspx");
        }

        protected void gvCountries_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[1].Text = Localization.GetString("SystemName");
                e.Row.Cells[2].Text = Localization.GetString("IsoCode");
                e.Row.Cells[3].Text = Localization.GetString("IsoAlpha3");
                e.Row.Cells[4].Text = Localization.GetString("IsoNumeric");
                e.Row.Cells[5].Text = Localization.GetString("DisplayName");
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
    }
}