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
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class Shipping_Zones : BaseAdminPage
    {
        #region Implementation

        private void LoadZones()
        {
            gvZones.DataSource = HccApp.OrderServices.ShippingZones.FindForStore(HccApp.CurrentStore.Id);
            gvZones.DataBind();
        }

        #endregion

        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("ShippingZones");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvZones.RowDeleting += gvZones_RowDeleting;
            gvZones.RowEditing += gvZones_RowEditing;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadZones();
            }
        }

        private void gvZones_RowEditing(object sender, GridViewEditEventArgs e)
        {
            Response.Redirect(string.Format("Shipping_Zones_Edit.aspx?id={0}", gvZones.DataKeys[e.NewEditIndex].Value));
        }

        private void gvZones_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var zoneId = (long) e.Keys[0];
            HccApp.OrderServices.ShippingZones.Delete(zoneId);
            LoadZones();
        }

        protected void btnNew_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var zone = new Zone();
                zone.Name = txtZoneName.Text.Trim();
                zone.StoreId = HccApp.CurrentStore.Id;

                if (HccApp.OrderServices.ShippingZones.Create(zone))
                {
                    ucMessageBox.ShowOk(Localization.GetString("ShippingZoneCreated"));
                }
                else
                {
                    ucMessageBox.ShowWarning(Localization.GetString("ShippingZoneError"));
                }

                LoadZones();
            }
        }

        protected void gvZones_OnRowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.Header)
            {
                e.Row.Cells[0].Text = Localization.GetString("ShippingZone");
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
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("DeleteConfirm"));
        }

        #endregion
    }
}