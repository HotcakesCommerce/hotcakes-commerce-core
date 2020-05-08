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
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Modules.Core.Admin.Parts.ShippingZones;

namespace Hotcakes.Modules.Core.Admin.SetupWizard
{
    public partial class Step3Shipping : HccPart
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gridMethods.RowDeleting += gridMethods_RowDeleting;
            gridMethods.RowCreated += gridMethods_RowCreating;
            gridMethods.RowEditing += gridMethods_RowEditing;

            gridZones.RowDeleting += gridZones_RowDeleting;
            gridZones.RowCreated += gridZones_RowCreated;
            gridZones.RowEditing += gridZones_RowEditing;
            gridZones.RowDataBound += gridZones_RowDataBound;

            LocalizeView();

            LoadShippingMethods();
            LoadProviders();
            LoadShippingZones();
            LoadHandlingSettings();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Load previous used module
            if (!string.IsNullOrEmpty(EditedShippingMethodId))
                LoadShippingMethodEditor(EditedShippingMethodId);
            else if (NewShippingMethod != null)
                LoadShippingMethodEditor(NewShippingMethod);
            else if (EditedShippingZoneId > 0)
                LoadShippingZoneEditor(EditedShippingZoneId);
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveHandlingSetting();
                //Show next step
                NotifyFinishedEditing();
            }
        }

        protected void btnLater_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing();
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            NotifyFinishedEditing("EXIT");
        }

        protected void gridMethods_RowCreating(object sender, GridViewRowEventArgs e)
        {
            var headerItem = e.Row;
            if (headerItem != null && headerItem.RowType == DataControlRowType.Header)
            {
                headerItem.Cells[0].Text = Localization.GetString("ShippingMethod");
            }
        }

        protected void gridZones_RowCreated(object sender, GridViewRowEventArgs e)
        {
            var headerItem = e.Row;
            if (headerItem != null)
            {
                headerItem.Cells[0].Text = Localization.GetString("ShippingZones");
            }
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void btnDeleteMethod_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ShippingMethodDelete"));
        }

        protected void btnDeleteZone_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ShippingZoneDelete"));
        }

        private void LocalizeView()
        {
            txtShippingZoneName.Attributes["placeholder"] = Localization.GetString("NewZoneField.EmptyMessage");
            rfvShippingZoneName.ErrorMessage = Localization.GetString("NewZoneFieldValidator.ErrorMessage");
            HandlingFeeAmountCustomValidator.ErrorMessage =
                Localization.GetString("HandlingFeeAmountCustomValidator.ErrorMessage");

            if (rbtnHandlingMethod.Items.Count == 0)
            {
                rbtnHandlingMethod.Items.Add(new ListItem(Localization.GetString("PerItem"), "0"));
                rbtnHandlingMethod.Items.Add(new ListItem(Localization.GetString("PerOrder"), "1"));
            }
        }

        #region Properties

        private ShippingMethod _method;

        protected ShippingMethod Method
        {
            get
            {
                if (_method == null || !_method.Bvin.Equals(EditedShippingMethodId))
                    _method = HccApp.OrderServices.ShippingMethods.Find(EditedShippingMethodId);

                return _method;
            }
        }

        private string EditedShippingMethodId
        {
            get { return (string) ViewState["EditedShippingMethodId"]; }
            set
            {
                if (string.IsNullOrEmpty(value))
                    ViewState.Remove("EditedShippingMethodId");
                else
                    ViewState["EditedShippingMethodId"] = value;
            }
        }

        private ShippingMethod NewShippingMethod
        {
            get { return (ShippingMethod) ViewState["NewShippingMethodProvider"]; }
            set
            {
                if (value == null)
                    ViewState.Remove("NewShippingMethodProvider");
                else
                    ViewState["NewShippingMethodProvider"] = value;
            }
        }

        private long EditedShippingZoneId
        {
            get { return ViewState["EditedShippingZoneId"] == null ? -1 : (long) ViewState["EditedShippingZoneId"]; }
            set
            {
                if (value < 0)
                    ViewState.Remove("EditedShippingZoneId");
                else
                    ViewState["EditedShippingZoneId"] = value;
            }
        }

        #endregion

        #region Client Script Helper Methods

        private void RegisterOpenDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrScripts, phrScripts.GetType(), "jsOpenDialog", "openDialog();", true);
        }

        private void AddCloseDialogScript(HccPart editor)
        {
            var btnsave = (LinkButton) editor.FindControl("btnSave");

            if (btnsave != null)
                btnsave.OnClientClick += "closeDialog();";

            var btncancel = (LinkButton) editor.FindControl("btnCancel");
            if (btncancel != null)
                btncancel.OnClientClick += "closeDialog();";
        }

        private void ClearEditorsInfo()
        {
            EditedShippingZoneId = -1;
            EditedShippingMethodId = null;
            NewShippingMethod = null;
            phrEditor.Controls.Clear();
        }

        #endregion

        #region Shipping Methods

        private void LoadShippingMethods()
        {
            gridMethods.DataSource = HccApp.OrderServices.ShippingMethods.FindAll(HccApp.CurrentStore.Id);
            gridMethods.DataBind();
        }

        private void LoadProviders()
        {
            ddlProviders.ClearSelection();
            foreach (var shippingService in AvailableServices.FindAll(HccApp.CurrentStore))
            {
                ddlProviders.Items.Add(new ListItem(shippingService.Name, shippingService.Id));
            }
        }

        protected void btnCreateMethod_Click(object sender, EventArgs e)
        {
            var newMethod = new ShippingMethod();
            newMethod.ShippingProviderId = ddlProviders.SelectedValue;
            newMethod.Name = Localization.GetString("NewShippingMethod");
            LoadShippingMethodEditor(newMethod);
            RegisterOpenDialogScript();
        }

        protected void gridMethods_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var method = (ShippingMethod) gridMethods.Rows[e.RowIndex].DataItem;
            HccApp.OrderServices.ShippingMethods.Delete(method.Bvin);
            LoadShippingMethods();

            //Clear this values to avoid issues with viewstate
            ClearEditorsInfo();
        }

        protected void gridMethods_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var method = (ShippingMethod) gridMethods.Rows[e.NewEditIndex].DataItem;

            NewShippingMethod = null;
            //Open Method Edit dialog
            LoadShippingMethodEditor(method.Bvin);
            RegisterOpenDialogScript();
        }

        private void LoadShippingMethodEditor(string shippingMethodId)
        {
            EditedShippingMethodId = shippingMethodId;

            if (Method == null)
            {
                ClearEditorsInfo();
                return;
            }
            LoadShippingMethodEditor(Method);
        }

        private void LoadShippingMethodEditor(ShippingMethod shippingMethod)
        {
            var p = AvailableServices.FindById(shippingMethod.ShippingProviderId, HccApp.CurrentStore);

            var editor = (HccShippingPart) HccPartController.LoadShippingEditor(p.Name, Page);

            if (editor != null)
            {
                ClearEditorsInfo();
                if (string.IsNullOrEmpty(shippingMethod.Bvin))
                    NewShippingMethod = shippingMethod;
                else
                    EditedShippingMethodId = shippingMethod.Bvin;
                editor.ShippingMethod = shippingMethod;
                editor.ID = string.Format("ShippingMethod_{0}_{1}", shippingMethod.ShippingProviderId,
                    shippingMethod.Bvin);
                editor.EditingComplete += ShippingMethodEditor_EditingComplete;
                AddCloseDialogScript(editor);
                phrEditor.Controls.Add(editor);
            }
        }

        private void ShippingMethodEditor_EditingComplete(object sender, HccPartEventArgs e)
        {
            if (e.Info.ToUpper() != "CANCELED")
            {
                if (NewShippingMethod != null)
                {
                    HccApp.OrderServices.ShippingMethods.Create(NewShippingMethod);
                }
                else
                {
                    HccApp.OrderServices.ShippingMethods.Update(Method);
                }
                //Refresh
                LoadShippingMethods();
            }

            ClearEditorsInfo();
        }

        #endregion

        #region Shipping Zones

        protected void btnCreateZone_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var zone = new Zone();
                zone.Name = txtShippingZoneName.Text.Trim();
                zone.StoreId = HccApp.CurrentStore.Id;

                if (HccApp.OrderServices.ShippingZones.Create(zone))
                {
                    LoadShippingZoneEditor(zone.Id);
                    RegisterOpenDialogScript();
                }
                LoadShippingZones();
            }
        }

        private void LoadShippingZones()
        {
            var zones = HccApp.OrderServices.ShippingZones.FindForStore(HccApp.CurrentStore.Id);
            gridZones.DataSource = zones;
            gridZones.DataBind();
        }

        protected void gridZones_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            var zone = (Zone) e.Row.DataItem;
            if (zone != null && zone.IsBuiltInZone)
            {
                var btnEdit = (LinkButton) e.Row.FindControl("btnEdit");
                if (btnEdit != null)
                    btnEdit.Visible = false;

                var btnDelete = (LinkButton) e.Row.FindControl("btnDelete");
                if (btnDelete != null)
                    btnDelete.Visible = false;
            }
        }

        protected void gridZones_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var zone = (Zone) gridZones.Rows[e.RowIndex].DataItem;
            HccApp.OrderServices.ShippingZones.Delete(zone.Id);
            ClearEditorsInfo();
            LoadShippingZones();
        }

        protected void gridZones_RowEditing(object sender, GridViewEditEventArgs e)
        {
            //Open Zone Edit dialog
            var zone = (Zone) gridZones.Rows[e.NewEditIndex].DataItem;
            if (zone != null)
            {
                LoadShippingZoneEditor(zone.Id);
                RegisterOpenDialogScript();
            }
        }

        private void LoadShippingZoneEditor(long shippingZoneId)
        {
            var editor = (Edit) HccPartController.LoadShippingZoneEditor(Page);
            if (editor == null)
                return;

            ClearEditorsInfo();
            EditedShippingZoneId = shippingZoneId;
            editor.ShippingZoneId = shippingZoneId;
            editor.ID = string.Format("ShippingZone_{0}", shippingZoneId);
            editor.EditingComplete += ShippingZoneEditor_EditingComplete;
            AddCloseDialogScript(editor);
            phrEditor.Controls.Add(editor);
        }

        private void ShippingZoneEditor_EditingComplete(object sender, HccPartEventArgs e)
        {
            ClearEditorsInfo();
            LoadShippingZones();
        }

        #endregion

        #region Handling

        private void SaveHandlingSetting()
        {
            HccApp.CurrentStore.Settings.HandlingAmount = decimal.Parse(txtHandlingFeeAmount.Text, NumberStyles.Currency);
            HccApp.CurrentStore.Settings.HandlingType = rbtnHandlingMethod.SelectedIndex;
            HccApp.CurrentStore.Settings.HandlingNonShipping = chkChargeNonShipping.Checked;
            HccApp.UpdateCurrentStore();
        }

        private void LoadHandlingSettings()
        {
            txtHandlingFeeAmount.Text = HccApp.CurrentStore.Settings.HandlingAmount.ToString("c");
            rbtnHandlingMethod.SelectedIndex = HccApp.CurrentStore.Settings.HandlingType;
            chkChargeNonShipping.Checked = HccApp.CurrentStore.Settings.HandlingNonShipping;
        }

        protected void HandlingFeeAmountCustomValidator_ServerValidate(object source, ServerValidateEventArgs args)
        {
            decimal temp;
            args.IsValid = decimal.TryParse(args.Value, NumberStyles.Currency, Thread.CurrentThread.CurrentUICulture,
                out temp);
        }

        #endregion
    }
}