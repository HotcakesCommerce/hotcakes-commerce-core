#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class SimpleProductFilter : HccUserControl
    {
        public delegate void FilterChangedDelegate(object sender, EventArgs e);

        public delegate void GoPressedDelegate(object sender, EventArgs e);

        public event FilterChangedDelegate FilterChanged;
        public event GoPressedDelegate GoPressed;


        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!Page.IsPostBack)
            {
                PopulateFilterFields();
            }
        }

        public override void Focus()
        {
            txtFilterField.Focus();
        }

        public ProductSearchCriteria LoadProductCriteria()
        {
            var c = new ProductSearchCriteria();
            c.DisplayInactiveProducts = true;

            if (txtFilterField.Text.Trim().Length > 0)
            {
                c.Keyword = txtFilterField.Text.Trim();
            }
            if (!string.IsNullOrEmpty(ddlManufacturerFilter.SelectedValue))
            {
                c.ManufacturerId = ddlManufacturerFilter.SelectedValue;
            }
            if (!string.IsNullOrEmpty(ddlVendorFilter.SelectedValue))
            {
                c.VendorId = ddlVendorFilter.SelectedValue;
            }
            if (!string.IsNullOrEmpty(ddlCategoryFilter.SelectedValue))
            {
                c.CategoryId = ddlCategoryFilter.SelectedValue;
            }
            if (!string.IsNullOrEmpty(ddlStatusFilter.SelectedValue))
            {
                c.Status = (ProductStatus) int.Parse(ddlStatusFilter.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlInventoryStatusFilter.SelectedValue))
            {
                c.InventoryStatus = (ProductInventoryStatus) int.Parse(ddlInventoryStatusFilter.SelectedValue);
            }
            if (!string.IsNullOrEmpty(ddlProductTypeFilter.SelectedValue))
            {
                c.ProductTypeId = ddlProductTypeFilter.SelectedValue;
            }

            // Save Setting to Session
            SessionManager.AdminProductCriteriaKeyword = txtFilterField.Text.Trim();
            SessionManager.AdminProductCriteriaCategory = ddlCategoryFilter.SelectedValue;
            SessionManager.AdminProductCriteriaManufacturer = ddlManufacturerFilter.SelectedValue;
            SessionManager.AdminProductCriteriaVendor = ddlVendorFilter.SelectedValue;
            SessionManager.AdminProductCriteriaStatus = ddlStatusFilter.SelectedValue;
            SessionManager.AdminProductCriteriaInventoryStatus = ddlInventoryStatusFilter.SelectedValue;
            SessionManager.AdminProductCriteriaProductType = ddlProductTypeFilter.SelectedValue;

            return c;
        }

        private void PopulateFilterFields()
        {
            PopulateCategories();
            PopulateManufacturers();
            PopulateVendors();
            PopulateStatus();
            PopulateInventoryStatus();
            PopulateProductTypes();

            txtFilterField.Text = SessionManager.AdminProductCriteriaKeyword.Trim();
            SetListToValue(ddlCategoryFilter, SessionManager.AdminProductCriteriaCategory);
            SetListToValue(ddlManufacturerFilter, SessionManager.AdminProductCriteriaManufacturer);
            SetListToValue(ddlVendorFilter, SessionManager.AdminProductCriteriaVendor);
            SetListToValue(ddlStatusFilter, SessionManager.AdminProductCriteriaStatus);
            SetListToValue(ddlInventoryStatusFilter, SessionManager.AdminProductCriteriaInventoryStatus);
            SetListToValue(ddlProductTypeFilter, SessionManager.AdminProductCriteriaProductType);
        }

        private void SetListToValue(DropDownList l, string value)
        {
            if (l != null && l.Items.FindByValue(value) != null)
            {
                l.ClearSelection();
                l.Items.FindByValue(value).Selected = true;
            }
        }

        private void PopulateCategories()
        {
            var tree = CategoriesHelper.ListFullTreeWithIndents(HccApp.CatalogServices.Categories);
            ddlCategoryFilter.Items.Clear();
            foreach (var li in tree)
            {
                var item = new ListItem(li.Text, li.Value);
                ddlCategoryFilter.Items.Add(item);
            }
            ddlCategoryFilter.Items.Insert(0, new ListItem(Localization.GetString("AnyCategory"), string.Empty));
        }

        private void PopulateManufacturers()
        {
            ddlManufacturerFilter.DataSource = HccApp.ContactServices.Manufacturers.FindAll();
            ddlManufacturerFilter.DataTextField = "DisplayName";
            ddlManufacturerFilter.DataValueField = "Bvin";
            ddlManufacturerFilter.DataBind();
            ddlManufacturerFilter.Items.Insert(0,
                new ListItem(Localization.GetString("AnyManufacturer"), string.Empty));
        }

        private void PopulateVendors()
        {
            ddlVendorFilter.DataSource = HccApp.ContactServices.Vendors.FindAll();
            ddlVendorFilter.DataTextField = "DisplayName";
            ddlVendorFilter.DataValueField = "Bvin";
            ddlVendorFilter.DataBind();
            ddlVendorFilter.Items.Insert(0, new ListItem(Localization.GetString("AnyVendor"), string.Empty));
        }

        private void PopulateStatus()
        {
            ddlStatusFilter.Items.Clear();
            ddlStatusFilter.Items.Add(new ListItem(Localization.GetString("AnyStatus"), string.Empty));
            ddlStatusFilter.Items.Add(new ListItem(Localization.GetString("Active"), "1"));
            ddlStatusFilter.Items.Add(new ListItem(Localization.GetString("Disabled"), "0"));
        }

        private void PopulateInventoryStatus()
        {
            ddlInventoryStatusFilter.Items.Clear();
            ddlInventoryStatusFilter.Items.Add(new ListItem(Localization.GetString("AnyInventoryStatus"),
                string.Empty));
            ddlInventoryStatusFilter.Items.Add(new ListItem(Localization.GetString("NotAvailable"), "0"));
            ddlInventoryStatusFilter.Items.Add(new ListItem(Localization.GetString("Available"), "1"));
            ddlInventoryStatusFilter.Items.Add(new ListItem(Localization.GetString("LowInventory"), "2"));
        }

        private void PopulateProductTypes()
        {
            ddlProductTypeFilter.Items.Clear();
            ddlProductTypeFilter.DataSource = HccApp.CatalogServices.ProductTypes.FindAll();
            ddlProductTypeFilter.DataTextField = "ProductTypeName";
            ddlProductTypeFilter.DataValueField = "bvin";
            ddlProductTypeFilter.DataBind();
            ddlProductTypeFilter.Items.Insert(0, new ListItem(Localization.GetString("AnyType"), string.Empty));
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            GoPressed(LoadProductCriteria(), new EventArgs());
            txtFilterField.Focus();
        }

        protected void ddlProductTypeFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChanged(LoadProductCriteria(), new EventArgs());
        }

        protected void ddlCategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChanged(LoadProductCriteria(), new EventArgs());
        }

        protected void ddlManufacturerFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChanged(LoadProductCriteria(), new EventArgs());
        }

        protected void ddlVendorFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChanged(LoadProductCriteria(), new EventArgs());
        }

        protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChanged(LoadProductCriteria(), new EventArgs());
        }

        protected void ddlInventoryStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            FilterChanged(LoadProductCriteria(), new EventArgs());
        }
    }
}