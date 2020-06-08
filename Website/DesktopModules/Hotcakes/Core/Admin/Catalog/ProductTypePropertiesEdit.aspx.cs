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
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY daOF ANY KIND, EXPRESS OR 
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY, 
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE 
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER 
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, 
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN 
// THE SOFTWARE.

#endregion

using System;
using System.Globalization;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class ProductTypePropertiesEdit : BaseAdminPage
    {
        public long? _productPropertyId;
        private const string DATEFORMAT = "MM/dd/yyyy";

        public long? ProductPropertyId
        {
            get
            {
                var propertyId = Request.QueryString["id"];
                long productPropertyId;
                if (long.TryParse(propertyId, out productPropertyId))
                    _productPropertyId = productPropertyId;
                return _productPropertyId;
            }
        }

        private long ProductPropertyChoiceId
        {
            get { return (long) ViewState["ProductPropertyChoiceId"]; }
            set { ViewState["ProductPropertyChoiceId"] = value; }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            rgChoices.RowDataBound += rgChoices_RowDataBound;
            rgChoices.RowDeleting += rgChoices_RowDeleting;
            rgChoices.RowEditing += rgChoices_RowEditing;

            btnUpdateChoice.Click += btnUpdateChoice_Click;
            btnCancelUpdateChoice.Click += btnCancelUpdateChoice_Click;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                var property = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);
                if (property == null)
                {
                    Response.Redirect("ProductTypeProperties.aspx");
                }

                LocalizeView();
                PopulateCultureCodeList();
                LoadProperty(property);
            }
        }

        private void LocalizeView()
        {
            var localization = Factory.Instance.CreateLocalizationHelper(LocalResourceFile);
            LocalizationUtils.LocalizeGridView(rgChoices, localization);
        }

        private void PopulateCultureCodeList()
        {
            var allCountries = HccApp.GlobalizationServices.Countries.FindAllForCurrency();

            lstCultureCode.DataSource = allCountries;
            lstCultureCode.DataValueField = "CultureCode";
            lstCultureCode.DataTextField = "SampleNameAndCurrency";
            lstCultureCode.DataBind();
        }

        private void LoadProperty(ProductProperty prop)
        {
            txtPropertyName.Text = prop.PropertyName;
            txtDisplayName.Text = prop.DisplayName;
            chkDisplayOnSite.Checked = prop.DisplayOnSite;
            chkDisplayToDropShipper.Checked = prop.DisplayToDropShipper;
            chkDisplayOnSearch.Checked = prop.DisplayOnSearch;

            InitSpecificSettings(prop);
        }

        private void InitSpecificSettings(ProductProperty property)
        {
            switch (property.TypeCode)
            {
                case ProductPropertyType.CurrencyField:
                    mvTypeSettings.SetActiveView(vCurrency);

                    var liCultureCode = lstCultureCode.Items.FindByValue(property.CultureCode);
                    if (liCultureCode != null)
                        liCultureCode.Selected = true;
                    txtDefaultCurrencyValue.Text = property.DefaultValue;
                    break;
                case ProductPropertyType.DateField:
                    mvTypeSettings.SetActiveView(vDate);

                    if (!string.IsNullOrEmpty(property.DefaultValue))
                    {
                        DateTime selectedDate;
                        if (
                            !DateTime.TryParse(property.DefaultValue, CultureInfo.InvariantCulture, DateTimeStyles.None,
                                out selectedDate))
                        {
                            selectedDate = DateTime.UtcNow;
                        }
                        radDefaultDate.Text = selectedDate.ToString(DATEFORMAT);
                    }
                    break;
                case ProductPropertyType.MultipleChoiceField:
                    mvTypeSettings.SetActiveView(vMultipleChoice);

                    PopulateMultipleChoice(property);
                    break;
                case ProductPropertyType.TextField:
                    mvTypeSettings.SetActiveView(vText);

                    chkIsLocalizable.Checked = property.IsLocalizable;
                    var realValue = property.IsLocalizable ? property.DefaultLocalizableValue : property.DefaultValue;
                    txtDefaultTextValue.Text = realValue;
                    break;
            }
        }

        private void PopulateMultipleChoice(ProductProperty property)
        {
            BindGridView();

            foreach (GridViewRow row in rgChoices.Rows)
            {
                var chbDefault = row.FindControl("chbDefault") as CheckBox;
                var choiceId = rgChoices.DataKeys[row.RowIndex]["Id"].ToString();
                if (choiceId == property.DefaultValue)
                {
                    chbDefault.Checked = true;
                    break;
                }
            }
        }

        private void BindGridView()
        {
            var property = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);

            if (property != null && property.Choices != null && property.Choices.Count > 0)
            {
                property.Choices = property.Choices.OrderBy(y => y.SortOrder).ToList();
                rgChoices.DataSource = property.Choices;
                rgChoices.DataBind();
            }
        }

        private void rgChoices_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var productPropertyChoice = e.Row.DataItem as ProductPropertyChoice;
                e.Row.Attributes["productPropertyChoiceId"] = productPropertyChoice.Id.ToString();
            }
        }

        private void rgChoices_RowEditing(object sender, GridViewEditEventArgs e)
        {
            e.Cancel = true;

            var ProductPropertyChoiceId = long.Parse(rgChoices.DataKeys[e.NewEditIndex]["Id"].ToString());
            var prop = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);
            var choice = prop.Choices.FirstOrDefault(y => y.Id == ProductPropertyChoiceId);

            txtChoiceName.Text = choice.ChoiceName;
            txtChoiceDisplayName.Text = choice.DisplayName;

            ShowEditor(true);
        }

        private void rgChoices_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var choiceId = long.Parse(rgChoices.DataKeys[e.RowIndex]["Id"].ToString());
            var prop = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);
            var c = prop.Choices.FirstOrDefault(y => y.Id == choiceId);
            if (c != null)
            {
                prop.Choices.Remove(c);
                HccApp.CatalogServices.ProductProperties.Update(prop);
            }
            BindGridView();
            ShowEditor(false);
        }

        private void btnUpdateChoice_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var prop = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);
                var choice = prop.Choices.FirstOrDefault(y => y.Id == ProductPropertyChoiceId);
                choice.ChoiceName = txtChoiceName.Text.Trim();
                choice.DisplayName = txtChoiceDisplayName.Text.Trim();

                if (HccApp.CatalogServices.ProductProperties.Update(prop))
                {
                    BindGridView();
                }
                else
                {
                    msg.ShowError(Localization.GetString("AddChoiceFailure"));
                }

                BindGridView();
                ShowEditor(false);
            }
        }

        private void btnCancelUpdateChoice_Click(object sender, EventArgs e)
        {
            BindGridView();
            ShowEditor(false);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("ProductTypeProperties.aspx");
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var prop = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);

                prop.PropertyName = txtPropertyName.Text;
                prop.DisplayName = txtDisplayName.Text;
                prop.DisplayOnSite = chkDisplayOnSite.Checked;
                prop.DisplayToDropShipper = chkDisplayToDropShipper.Checked;
                prop.DisplayOnSearch = chkDisplayOnSearch.Checked;
                switch (prop.TypeCode)
                {
                    case ProductPropertyType.CurrencyField:
                        prop.CultureCode = lstCultureCode.SelectedValue;
                        prop.DefaultValue = txtDefaultCurrencyValue.Text.Trim();
                        break;
                    case ProductPropertyType.MultipleChoiceField:
                        var selectedValue = string.Empty;
                        foreach (GridViewRow row in rgChoices.Rows)
                        {
                            var chbDefault = row.FindControl("chbDefault") as CheckBox;
                            if (chbDefault != null && chbDefault.Checked)
                            {
                                selectedValue = rgChoices.DataKeys[row.RowIndex]["Id"].ToString();
                                break;
                            }
                        }
                        prop.DefaultValue = selectedValue;
                        break;
                    case ProductPropertyType.DateField:
                        prop.DefaultValue = string.Format("{0:d}", radDefaultDate.Text.Trim(),
                            CultureInfo.InvariantCulture);
                        break;
                    case ProductPropertyType.TextField:
                        prop.IsLocalizable = chkIsLocalizable.Checked;
                        if (prop.IsLocalizable)
                            prop.DefaultLocalizableValue = txtDefaultTextValue.Text.Trim();
                        else
                            prop.DefaultValue = txtDefaultTextValue.Text.Trim();
                        break;
                }
                var success = HccApp.CatalogServices.ProductProperties.Update(prop);
                if (success)
                {
                    Response.Redirect("ProductTypeProperties.aspx");
                }
                else
                {
                    msg.ShowError(Localization.GetString("SavePropertyFailure"));
                }
            }
        }

        protected void btnNewChoice_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                var prop = HccApp.CatalogServices.ProductProperties.Find(ProductPropertyId.Value);

                var maxSortOrder = prop.Choices.Max(c => (int?) c.SortOrder) ?? 0;

                var ppc = new ProductPropertyChoice();
                ppc.ChoiceName = txtNewChoice.Text.Trim();
                ppc.DisplayName = txtNewChoice.Text.Trim();
                ppc.PropertyId = ProductPropertyId.Value;
                ppc.SortOrder = maxSortOrder + 1;
                prop.Choices.Add(ppc);
                if (HccApp.CatalogServices.ProductProperties.Update(prop))
                {
                    BindGridView();
                }
                else
                {
                    msg.ShowError(Localization.GetString("AddChoiceFailure"));
                }
                txtNewChoice.Text = string.Empty;

                ProductPropertyChoiceId = ppc.Id;
                txtChoiceName.Text = ppc.ChoiceName;
                txtChoiceDisplayName.Text = ppc.DisplayName;

                ShowEditor(true);
            }
        }

        #region Private

        private void ShowEditor(bool show)
        {
            pnlEditChoice.Visible = show;

            if (show)
            {
                ScriptManager.RegisterStartupScript(Page, GetType(), "hcEditChoiceDialog", "hcEditChoiceDialog();", true);
            }
        }

        #endregion
    }
}