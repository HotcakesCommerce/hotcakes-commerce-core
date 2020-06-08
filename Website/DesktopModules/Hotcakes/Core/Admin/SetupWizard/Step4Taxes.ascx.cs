#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.SetupWizard
{
    public partial class Step4Taxes : HccPart
    {
        #region Properties

        private long? EditedTaxScheduleId
        {
            get { return (long?) ViewState["EditedTaxScheduleId"]; }
            set { ViewState["EditedTaxScheduleId"] = value; }
        }

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            btnTaxScheduleSave.Click += btnTaxScheduleSave_Click;
            btnTaxScheduleCancel.Click += btnTaxScheduleCancel_Click;

            gridTaxes.RowEditing += gridTaxes_RowEditing;
            gridTaxes.RowDeleting += gridTaxes_RowDeleting;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                LocalizeView();

                LoadSchedules();

                LoadTaxationSettings();
            }

            // Load editors to fire initiated events
            LoadTaxScheduleEditor();
        }

        protected void chkApplyVATRules_CheckedChanged(object sender, EventArgs e)
        {
            SaveTaxationSetting();
        }

        protected void btnCreate_Click(object sender, EventArgs e)
        {
            var t = new TaxSchedule {Name = txtDisplayName.Text.Trim()};
            HccApp.OrderServices.TaxSchedules.Create(t);

            txtDisplayName.Text = string.Empty;

            EditedTaxScheduleId = t.Id;
            LoadTaxScheduleEditor();
        }

        protected void gridTaxes_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var itemId = gridTaxes.DataKeys[e.RowIndex];
            HccApp.OrderServices.TaxSchedulesDestroy(long.Parse(itemId.Value.ToString()));
            LoadSchedules();
        }

        protected void gridTaxes_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            var taxSchedule = gridTaxes.DataKeys[e.NewEditIndex];

            EditedTaxScheduleId = long.Parse(taxSchedule.Value.ToString());
            LoadTaxScheduleEditor();
        }

        protected void btnTaxScheduleCancel_Click(object sender, EventArgs e)
        {
            CloseTaxScheduleEditor();
            Response.Redirect(Page.Request.RawUrl, false);
        }

        protected void btnTaxScheduleSave_Click(object sender, EventArgs e)
        {
            if (ucTaxScheduleEditor.Save())
            {
                CloseTaxScheduleEditor();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                SaveTaxationSetting();

                // hide wizard after setup completion
                var sett = HccApp.CurrentStore.Settings.Urls;
                sett.HideSetupWizardWelcome = true;
                HccApp.UpdateCurrentStore();

                //Show Next View
                NotifyFinishedEditing("EXIT");
            }
        }

        protected void btnExit_Click(object sender, EventArgs e)
        {
            // won't hide the wizard from the dashboard
            NotifyFinishedEditing("EXIT");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            ShowDialog();

            if (HccApp.CurrentStore.Settings.ApplyVATRules)
            {
                msg.ShowInformation(
                    "Product Catalog selling prices includes VAT. All store selling prices will be shown inclusive of VAT. VAT will be shown during checkout");
            }
            else
            {
                msg.ShowInformation(
                    "Product Catalog selling price excludes VAT. All store selling prices will be shown exclusive of Sales Tax/VAT. Sales Tax/VAT will be added during checkout");
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
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("TaxScheduleDelete"));
        }

        #endregion

        #region Implementation

        private void LoadSchedules()
        {
            gridTaxes.DataSource = HccApp.OrderServices.TaxSchedules.FindAll(HccApp.CurrentStore.Id);
            gridTaxes.DataBind();
        }

        private void ShowDialog()
        {
            if (EditedTaxScheduleId.HasValue)
            {
                ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcEditTaxScheduleDialog",
                    "hcEditTaxScheduleDialog();", true);
            }
        }

        private void CloseTaxScheduleEditor()
        {
            EditedTaxScheduleId = null;
            hcEditTaxScheduleDialog.Visible = false;
            LoadSchedules();
        }

        private void LoadTaxScheduleEditor()
        {
            if (EditedTaxScheduleId.HasValue)
            {
                hcEditTaxScheduleDialog.Visible = true;
                ucTaxScheduleEditor.TaxScheduleId = EditedTaxScheduleId;
                ucTaxScheduleEditor.LoadSchedule();
            }
        }

        private void SaveTaxationSetting()
        {
            HccApp.CurrentStore.Settings.ApplyVATRules = chkApplyVATRules.Checked;
            HccApp.UpdateCurrentStore();
        }

        private void LoadTaxationSettings()
        {
            chkApplyVATRules.Checked = HccApp.CurrentStore.Settings.ApplyVATRules;
        }

        private void LocalizeView()
        {
            txtDisplayName.Attributes["placeholder"] = Localization.GetString("txtDisplayName.EmptyMessage");
            DisplayNameValidator.ErrorMessage = Localization.GetString("DisplayNameValidator.ErrorMessage");

            var localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
            LocalizationUtils.LocalizeGridView(gridTaxes, localization);
        }

        #endregion
    }
}