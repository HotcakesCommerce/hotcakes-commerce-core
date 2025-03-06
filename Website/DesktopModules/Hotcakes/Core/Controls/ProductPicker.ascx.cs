#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Collections.Specialized;
using System.IO;
using System.Web.UI;
using System.Web.UI.WebControls;
using DotNetNuke.Security;
using DotNetNuke.Services.Localization;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Controls
{
    partial class ProductPicker : UserControl
    {
        public string LocalResourceFile
        {
            get
            {
                return Path.Combine(TemplateSourceDirectory + "/",
                    Localization.LocalResourceDirectory + "/ProductPicker");
            }
        }

        protected HotcakesApplication HccApp
        {
            get { return HotcakesApplication.Current; }
        }

        public string Keyword
        {
            get { return FilterField.Text.Trim(); }
            set { FilterField.Text = value; }
        }

        protected bool IsInitialized
        {
            get
            {
                var obj = ViewState["IsInitialized"];
                if (obj != null)
                {
                    return (bool) obj;
                }
                return false;
            }
            set { ViewState["IsInitialized"] = value; }
        }

        public bool DisplayPrice
        {
            get
            {
                var obj = ViewState["DisplayPrice"];
                if (obj != null)
                {
                    return (bool) obj;
                }
                return false;
            }
            set { ViewState["DisplayPrice"] = value; }
        }

        public bool DisplayInventory
        {
            get
            {
                var obj = ViewState["DisplayInventory"];
                if (obj != null)
                {
                    return (bool) obj;
                }
                return false;
            }
            set { ViewState["DisplayInventory"] = value; }
        }

        public string ExcludeCategoryBvin
        {
            get { return ExcludeCategoryBvinField.Value; }
            set { ExcludeCategoryBvinField.Value = value; }
        }

        public StringCollection SelectedProducts
        {
            get
            {
                var result = new StringCollection();

                for (var i = 0; i <= rgProducts.Rows.Count - 1; i++)
                {
                    var item = rgProducts.Rows[i];
                    var chkSelected = (CheckBox) item.Cells[0].FindControl("chkSelected");
                    if (chkSelected != null && chkSelected.Checked)
                    {
                        result.Add(Convert.ToString(rgProducts.DataKeys[i]["Bvin"]));
                    }
                }

                return result;
            }
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            if (!DisplayInventory)
                GetGridViewColumnByName("Available").Visible = false;
            if (!DisplayPrice)
                GetGridViewColumnByName("SitePrice").Visible = false;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsInitialized)
            {
                LocalizeView();
                PopulateCategories();
                PopulateManufacturers();
                PopulateVendors();
                IsInitialized = true;
                if (!Page.IsPostBack)
                {
                    RunSearch();
                }
            }
        }

        private void LocalizeView()
        {
            Localization.LocalizeGridView(ref rgProducts, LocalResourceFile);
        }

        private void PopulateCategories()
        {
            var tree = CategoriesHelper.ListFullTreeWithIndents(HccApp.CatalogServices.Categories);
            CategoryFilter.Items.Clear();
            foreach (var li in tree)
            {
                CategoryFilter.Items.Add(new ListItem(li.Text, li.Value));
            }
            CategoryFilter.Items.Insert(0, new ListItem("- Any Category -", string.Empty));
        }

        private void PopulateManufacturers()
        {
            ManufacturerFilter.DataSource = HccApp.ContactServices.Manufacturers.FindAll();
            ManufacturerFilter.DataTextField = "DisplayName";
            ManufacturerFilter.DataValueField = "Bvin";
            ManufacturerFilter.DataBind();
            ManufacturerFilter.Items.Insert(0, new ListItem("- Any Manufacturer -", string.Empty));
        }

        private void PopulateVendors()
        {
            VendorFilter.DataSource = HccApp.ContactServices.Vendors.FindAll();
            VendorFilter.DataTextField = "DisplayName";
            VendorFilter.DataValueField = "Bvin";
            VendorFilter.DataBind();
            VendorFilter.Items.Insert(0, new ListItem("- Any Vendor -", string.Empty));
        }

        protected void btnSearch_Click(object sender, EventArgs e)
        {
            RunSearch();
        }

        private ProductSearchCriteria GetCurrentCriteria()
        {
            var security = new PortalSecurity();
            var c = new ProductSearchCriteria();

            if (FilterField.Text.Trim().Length > 0)
            {
                c.Keyword = security.InputFilter(FilterField.Text.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            }
            if (!string.IsNullOrEmpty(ManufacturerFilter.SelectedValue))
            {
                c.ManufacturerId = ManufacturerFilter.SelectedValue;
            }
            if (!string.IsNullOrEmpty(VendorFilter.SelectedValue))
            {
                c.VendorId = VendorFilter.SelectedValue;
            }
            if (!string.IsNullOrEmpty(CategoryFilter.SelectedValue))
            {
                c.CategoryId = CategoryFilter.SelectedValue;
            }
            if (ExcludeCategoryBvin.Trim().Length > 0)
            {
                c.NotCategoryId = security.InputFilter(ExcludeCategoryBvin.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            }
            c.DisplayInactiveProducts = true;
            return c;
        }

        public void RunSearch()
        {
            rgProducts.PageIndex = 0;
            BindGrid();
        }

        protected void rgProducts_PageIndexChanging(object sender, GridViewPageEventArgs e)
        {
            rgProducts.PageIndex = e.NewPageIndex;
            BindGrid();
        }

        protected void ManufacturerFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void VendorFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void CategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            BindGrid();
        }

        protected void BindGrid()
        {
            var totalCount = 0;
            var items = HccApp.CatalogServices.Products.FindByCriteria(GetCurrentCriteria(),
                rgProducts.PageIndex,
                rgProducts.PageSize,
                ref totalCount);
            rgProducts.DataSource = items;
            rgProducts.DataBind();
            rgProducts.VirtualItemCount = totalCount;
        }

        private DataControlField GetGridViewColumnByName(string name)
        {
            foreach (DataControlField col in rgProducts.Columns)
            {
                if (col.HeaderText.ToLower().Trim() == name.ToLower().Trim())
                {
                    return col;
                }
            }

            return null;
        }

        protected string ParseProductAvailability(object value)
        {
            if (value != null)
            {
                return Convert.ToBoolean(value) ? "Yes" : "No";
            }

            return "No";
        }
    }
}