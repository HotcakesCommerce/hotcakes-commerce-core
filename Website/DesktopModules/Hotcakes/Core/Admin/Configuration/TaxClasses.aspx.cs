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
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Taxes.Providers;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    partial class TaxClasses : BaseAdminPage
    {
        public const string DNNTaxProvider = "Hotcakes";

        public string ProviderID
        {
            get { return Request.QueryString["providerid"]; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("Taxes");
            CurrentTab = AdminTabType.Configuration;
            ValidateCurrentUserHasPermission(SystemPermissions.SettingsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LocalizeView();

                BindForm();

                FillForm();
            }
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (ucTaxScheduleEditor.Save())
            {
                dgTaxClasses.DataSource = HccApp.OrderServices.TaxSchedules.FindAll(HccApp.CurrentStore.Id);
                dgTaxClasses.DataBind();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                HccApp.CurrentStore.Settings.ApplyVATRules = chkApplyVATRules.Checked;

                HccApp.UpdateCurrentStore();

                SaveChanges();
            }
        }

        private void SaveChanges()
        {
            if (ddlTaxProviders.SelectedValue != DNNTaxProvider)
                HccApp.CurrentStore.Settings.TaxProviderEnabled = ddlTaxProviders.SelectedValue;
            else
                HccApp.CurrentStore.Settings.TaxProviderEnabled = string.Empty;

            HccApp.UpdateCurrentStore();
        }

        public void dgTaxClasses_Delete(object sender, DataGridCommandEventArgs e)
        {
            var editID = (long) dgTaxClasses.DataKeys[e.Item.ItemIndex];
            HccApp.OrderServices.TaxSchedulesDestroy(editID);
            BindForm();
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);
            if (HccApp.CurrentStore.Settings.ApplyVATRules)
            {
                msg.ShowInformation(Localization.GetString("IncludeVat"));
            }
            else
            {
                msg.ShowInformation(Localization.GetString("ExcludeVat"));
            }
        }

        protected void btnEdit_OnPreRender(object sender, EventArgs e)
        {
            var link = (HyperLink) sender;
            link.Text = Localization.GetString("Edit");
        }

        protected void btnDelete_OnPreRender(object sender, EventArgs e)
        {
            var link = (LinkButton) sender;
            link.Text = Localization.GetString("Delete");
            link.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmDelete"));
        }

        private void BindForm()
        {
            var result = TaxProviders.AvailableProviders();

            ddlTaxProviders.Items.Clear();

            foreach (var item in result)
            {
                ddlTaxProviders.Items.Add(new ListItem(item.ProviderName, item.ProviderId));
            }
            ddlTaxProviders.Items.Insert(0, new ListItem(DNNTaxProvider, DNNTaxProvider));

            var currentProvider = ddlTaxProviders.Items.FindByValue(HccApp.CurrentStore.Settings.TaxProviderEnabled);

            if (ProviderID != null)
            {
                ddlTaxProviders.SelectedValue = ProviderID;
            }
            else if (currentProvider != null)
                ddlTaxProviders.SelectedValue = currentProvider.Value;
            else
                ddlTaxProviders.SelectedValue = DNNTaxProvider;


            dgTaxClasses.DataSource = HccApp.OrderServices.TaxSchedules.FindAll(HccApp.CurrentStore.Id);
            dgTaxClasses.DataBind();
        }

        private void FillForm()
        {
            chkApplyVATRules.Checked = HccApp.CurrentStore.Settings.ApplyVATRules;
        }

        private void LocalizeView()
        {
            dgTaxClasses.Columns[0].HeaderText = Localization.GetString("ScheduleName");
        }

        protected void btnEdit_Click(object sender, EventArgs e)
        {
            var redirectURL = "TaxProvider_Edit.aspx?id=" + ddlTaxProviders.SelectedValue;
            Response.Redirect(redirectURL);
        }
    }
}