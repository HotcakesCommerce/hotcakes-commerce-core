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
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class ProductPicker : HccUserControl
    {
        public ProductPicker()
        {
            DisplayBundles = true;
            DisplayGiftCards = true;
            DisplayRecurring = true;
            DisplayProductWithChoice = true;
        }

        public int CurrentPage
        {
            get
            {
                var result = 1;
                var temp = 0;
                if (int.TryParse(currentpagefield.Value, out temp))
                {
                    return temp;
                }
                return result;
            }
            set { currentpagefield.Value = value.ToString(); }
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

        public bool IsMultiSelect
        {
            get
            {
                var obj = ViewState["IsMultiSelect"];
                if (obj != null)
                {
                    return (bool) obj;
                }
                var prePage = PreviousPageViewState;
                if (prePage != null)
                {
                    if (prePage["IsMultiSelect"] != null)
                    {
                        return (bool) prePage["IsMultiSelect"];
                    }
                }
                return true;
            }
            set { ViewState["IsMultiSelect"] = value; }
        }

        private Page viewStatePage
        {
            get
            {
                if (Page.PreviousPage == null)
                {
                    return Page;
                }

                return Page.PreviousPage;
            }
        }

        private StateBag PreviousPageViewState
        {
            get
            {
                StateBag returnValue = null;
                if (viewStatePage != null)
                {
                    object objPreviousPage = viewStatePage;
                    var objMethod = objPreviousPage.GetType().GetMethod
                        ("ReturnViewState");
                    if (objMethod != null)
                    {
                        return (StateBag) objMethod.Invoke(objPreviousPage, null);
                    }
                }
                return returnValue;
            }
        }

        public bool DisplayPrice { get; set; }
        public bool DisplayInventory { get; set; }
        public bool DisplayBundles { get; set; }
        public bool DisplayGiftCards { get; set; }
        public bool DisplayRecurring { get; set; }
        public bool DisplayProductWithChoice { get; set; }

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

                if (IsMultiSelect)
                {
                    for (var i = 0; i <= GridView1.Rows.Count - 1; i++)
                    {
                        if (GridView1.Rows[i].RowType == DataControlRowType.DataRow)
                        {
                            var chkSelected = (CheckBox) GridView1.Rows[i].Cells[0].FindControl("chkSelected");
                            if (chkSelected != null)
                            {
                                if (chkSelected.Checked)
                                {
                                    result.Add((string) GridView1.DataKeys[GridView1.Rows[i].RowIndex].Value);
                                }
                            }
                        }
                    }
                }
                else
                {
                    var val = Request.Form[GridView1.ClientID + "CheckBoxSelected"];
                    if (val != null)
                    {
                        if (val != string.Empty)
                        {
                            result.Add(val);
                        }
                    }
                }
                return result;
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GridView1.PageSize = int.Parse(DropDownList1.SelectedValue);
            if (!IsInitialized)
            {
                PopulateCategories();
                PopulateManufacturers();
                PopulateVendors();
                IsInitialized = true;
                if (!Page.IsPostBack)
                {
                    CurrentPage = 1;
                    PopulateFilterFields();
                    RunSearch();

                    GridView1.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
                }
            }

            GridView1.Columns[3].Visible = DisplayPrice;
            GridView1.Columns[4].Visible = DisplayInventory;
        }

        public void PopulateFilterFields()
        {
            FilterField.Text = SessionManager.AdminProductCriteriaKeyword.Trim();
            SetListToValue(CategoryFilter, SessionManager.AdminProductCriteriaCategory);
            SetListToValue(ManufacturerFilter, SessionManager.AdminProductCriteriaManufacturer);
            SetListToValue(VendorFilter, SessionManager.AdminProductCriteriaVendor);
        }

        private void SetListToValue(DropDownList l, string value)
        {
            if (l != null)
            {
                if (l.Items.FindByValue(value) != null)
                {
                    l.ClearSelection();
                    l.Items.FindByValue(value).Selected = true;
                }
            }
        }

        private void PopulateCategories()
        {
            var tree = CategoriesHelper.ListFullTreeWithIndents(HccApp.CatalogServices.Categories);
            CategoryFilter.Items.Clear();
            foreach (var li in tree)
            {
                CategoryFilter.Items.Add(li);
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

        protected void btnGo_Click(object sender, EventArgs e)
        {
            RunSearch();
        }

        private ProductSearchCriteria GetCurrentCriteria()
        {
            var c = new ProductSearchCriteria();

            if (FilterField.Text.Trim().Length > 0)
            {
                c.Keyword = FilterField.Text.Trim();
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
                c.NotCategoryId = ExcludeCategoryBvin.Trim();
            }
            c.DisplayInactiveProducts = true;
            c.DisplayBundles = DisplayBundles;
            c.DisplayGiftCards = DisplayGiftCards;
            c.DisplayRecurring = DisplayRecurring;
            c.DisplayProductWithChoice = DisplayProductWithChoice;
            return c;
        }

        public void RunSearch()
        {
            CurrentPage = 1;
            LoadSearch();
        }

        public void LoadSearch()
        {
            var totalCount = 0;
            GridView1.PageIndex = 0;
            GridView1.DataSource = HccApp.CatalogServices.Products.FindByCriteria(GetCurrentCriteria(),
                CurrentPage,
                GridView1.PageSize,
                ref totalCount);
            GridView1.DataBind();
            lstPage.Items.Clear();
            var totalPages = Paging.TotalPages(totalCount, GridView1.PageSize);
            for (var i = 1; i <= totalPages; i++)
            {
                var li = new ListItem(i.ToString(), i.ToString());
                if (i == CurrentPage)
                {
                    li.Selected = true;
                }
                lstPage.Items.Add(li);
            }
            SessionManager.AdminProductCriteriaKeyword = FilterField.Text.Trim();
            SessionManager.AdminProductCriteriaCategory = CategoryFilter.SelectedValue;
            SessionManager.AdminProductCriteriaManufacturer = ManufacturerFilter.SelectedValue;
            SessionManager.AdminProductCriteriaVendor = VendorFilter.SelectedValue;
        }


        protected void ManufacturerFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            LoadSearch();
        }

        protected void VendorFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            LoadSearch();
        }

        protected void CategoryFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            LoadSearch();
        }

        protected void DropDownList1_SelectedIndexChanged(object sender, EventArgs e)
        {
            CurrentPage = 1;
            GridView1.PageSize = int.Parse(DropDownList1.SelectedValue);
            LoadSearch();
        }

        protected void GridView1_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                if (!IsMultiSelect)
                {
                    ((CheckBox) e.Row.FindControl("chkSelected")).Visible = false;
                    ((Literal) e.Row.FindControl("radioButtonLiteral")).Text = "<input name='" + GridView1.ClientID +
                                                                               "CheckBoxSelected' type='radio' value='" +
                                                                               GridView1.DataKeys[e.Row.RowIndex].Value +
                                                                               "' />";
                }

                var PriceLabel = (Label) e.Row.FindControl("PriceLabel");
                var InventoryLabel = (Label) e.Row.FindControl("InventoryLabel");
                if (DisplayPrice)
                {
                    PriceLabel.Text = ((Product) e.Row.DataItem).SitePrice.ToString("c");
                }

                if (DisplayInventory)
                {
                    var prod = (Product) e.Row.DataItem;
                    if (prod.IsAvailableForSale)
                    {
                        InventoryLabel.Text = "Available for Sale";
                    }
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                if (DisplayPrice)
                {
                    e.Row.Cells[3].Text = "Site Price";
                }

                if (DisplayInventory)
                {
                    e.Row.Cells[4].Text = "Available Qty";
                }
            }
        }

        protected void lstPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            var temp = 1;
            if (int.TryParse(lstPage.SelectedItem.Value, out temp))
            {
                CurrentPage = temp;
            }
            else
            {
                CurrentPage = 1;
            }
            LoadSearch();
        }
    }
}