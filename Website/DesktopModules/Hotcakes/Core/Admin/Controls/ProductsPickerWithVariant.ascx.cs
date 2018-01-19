using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Text;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.Admin.AppCode;
using System.Reflection;

namespace Hotcakes.Modules.Core.Admin.Controls
{

    partial class ProductsPickerWithVariant : HccUserControl
    {
        public int CurrentPage
        {
            get
            {
                int result = 1;
                int temp = 0;
                if (int.TryParse(this.currentpagefield.Value, out temp))
                {
                    return temp;
                }
                return result;
            }
            set
            {
                this.currentpagefield.Value = value.ToString();
            }
        }
        public string Keyword
        {
            get { return this.FilterField.Text.Trim(); }
            set { this.FilterField.Text = value; }
        }
        protected bool IsInitialized
        {
            get
            {
                object obj = ViewState["IsInitialized"];
                if (obj != null)
                {
                    return (bool)obj;
                }
                else
                {
                    return false;
                }
            }
            set { ViewState["IsInitialized"] = value; }
        }
        public bool IsMultiSelect
        {
            get
            {
                object obj = ViewState["IsMultiSelect"];
                if (obj != null)
                {
                    return (bool)obj;
                }
                else
                {
					StateBag prePage = PreviousPageViewState;
					if (prePage != null)
					{
						if (prePage["IsMultiSelect"] != null)
						{
							return (bool)prePage["IsMultiSelect"];
						}
					}
                    return true;
                }
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
					Object objPreviousPage = (Object)viewStatePage;
					MethodInfo objMethod = objPreviousPage.GetType().GetMethod
							("ReturnViewState");
					if (objMethod != null)
					{
						return (StateBag)objMethod.Invoke(objPreviousPage, null);
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
		public ProductsPickerWithVariant()
		{
			DisplayBundles = true;
			DisplayGiftCards = true;
			DisplayRecurring = true;
			DisplayProductWithChoice = true;
		}

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            GridView1.PageSize = int.Parse(DropDownList1.SelectedValue);
            if (!this.IsInitialized)
            {
                PopulateCategories();
                PopulateManufacturers();
                PopulateVendors();
                this.IsInitialized = true;
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
            this.FilterField.Text = SessionManager.AdminProductCriteriaKeyword.Trim();
            SetListToValue(this.CategoryFilter, SessionManager.AdminProductCriteriaCategory);
            SetListToValue(this.ManufacturerFilter, SessionManager.AdminProductCriteriaManufacturer);
            SetListToValue(this.VendorFilter, SessionManager.AdminProductCriteriaVendor);
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

        public string ExcludeCategoryBvin
        {
            get { return this.ExcludeCategoryBvinField.Value; }
            set { this.ExcludeCategoryBvinField.Value = value; }
        }

        private void PopulateCategories()
        {
			Collection<ListItem> tree = CategoriesHelper.ListFullTreeWithIndents(HccApp.CatalogServices.Categories);
            this.CategoryFilter.Items.Clear();
            foreach (ListItem li in tree)
            {
                this.CategoryFilter.Items.Add(li);
            }
            this.CategoryFilter.Items.Insert(0, new ListItem("- Any Category -", ""));
        }

        private void PopulateManufacturers()
        {
            this.ManufacturerFilter.DataSource = HccApp.ContactServices.Manufacturers.FindAll();
            this.ManufacturerFilter.DataTextField = "DisplayName";
            this.ManufacturerFilter.DataValueField = "Bvin";
            this.ManufacturerFilter.DataBind();
            this.ManufacturerFilter.Items.Insert(0, new ListItem("- Any Manufacturer -", ""));
        }

        private void PopulateVendors()
        {
            this.VendorFilter.DataSource = HccApp.ContactServices.Vendors.FindAll();
            this.VendorFilter.DataTextField = "DisplayName";
            this.VendorFilter.DataValueField = "Bvin";
            this.VendorFilter.DataBind();
            this.VendorFilter.Items.Insert(0, new ListItem("- Any Vendor -", ""));
        }

        protected void btnGo_Click(object sender, EventArgs e)
        {
            RunSearch();
        }

        private ProductSearchCriteria GetCurrentCriteria()
        {
            Hotcakes.Commerce.Catalog.ProductSearchCriteria c = new Hotcakes.Commerce.Catalog.ProductSearchCriteria();

            if (this.FilterField.Text.Trim().Length > 0)
            {
                c.Keyword = this.FilterField.Text.Trim();
            }
            if (this.ManufacturerFilter.SelectedValue != "")
            {
                c.ManufacturerId = this.ManufacturerFilter.SelectedValue;
            }
            if (this.VendorFilter.SelectedValue != "")
            {
                c.VendorId = this.VendorFilter.SelectedValue;
            }
            if (this.CategoryFilter.SelectedValue != "")
            {
                c.CategoryId = this.CategoryFilter.SelectedValue;
            }
            if (this.ExcludeCategoryBvin.Trim().Length > 0)
            {
                c.NotCategoryId = this.ExcludeCategoryBvin.Trim();
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
			int totalCount = 0;
			this.GridView1.PageIndex = 0;
			this.GridView1.DataSource = HccApp.CatalogServices.Products.FindByCriteria(GetCurrentCriteria(),
																					   CurrentPage,
																					   this.GridView1.PageSize,
																					   ref totalCount);
			this.GridView1.DataBind();
			this.lstPage.Items.Clear();
			int totalPages = Hotcakes.Web.Paging.TotalPages(totalCount, this.GridView1.PageSize);
			for (int i = 1; i <= totalPages; i++)
			{
				ListItem li = new ListItem(i.ToString(), i.ToString());
				if (i == CurrentPage)
				{
					li.Selected = true;
				}
				this.lstPage.Items.Add(li);
			}
			SessionManager.AdminProductCriteriaKeyword = this.FilterField.Text.Trim();
			SessionManager.AdminProductCriteriaCategory = this.CategoryFilter.SelectedValue;
			SessionManager.AdminProductCriteriaManufacturer = this.ManufacturerFilter.SelectedValue;
			SessionManager.AdminProductCriteriaVendor = this.VendorFilter.SelectedValue;
		}

        public StringCollection SelectedProducts
        {
            get
            {
                StringCollection result = new StringCollection();

                if (this.IsMultiSelect)
                {
                    for (int i = 0; i <= this.GridView1.Rows.Count - 1; i++)
                    {
                        if (GridView1.Rows[i].RowType == DataControlRowType.DataRow)
                        {
                            CheckBox chkSelected = (CheckBox)this.GridView1.Rows[i].Cells[0].FindControl("chkSelected");
                            if (chkSelected != null)
                            {
                                if (chkSelected.Checked == true)
                                {
                                    result.Add((string)GridView1.DataKeys[GridView1.Rows[i].RowIndex].Value);
                                }
                            }
                        }
                    }
                }
                else
                {
                    string val = (string)Request.Form[this.GridView1.ClientID + "CheckBoxSelected"];
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
                if (!this.IsMultiSelect)
                {
                    ((CheckBox)e.Row.FindControl("chkSelected")).Visible = false;
                    ((Literal)e.Row.FindControl("radioButtonLiteral")).Text = "<input name='" + this.GridView1.ClientID + "CheckBoxSelected' type='radio' value='" + GridView1.DataKeys[e.Row.RowIndex].Value + "' />";
                }

                Label PriceLabel = (Label)e.Row.FindControl("PriceLabel");
                Label InventoryLabel = (Label)e.Row.FindControl("InventoryLabel");
                if (this.DisplayPrice)
                {
                    PriceLabel.Text = ((Hotcakes.Commerce.Catalog.Product)e.Row.DataItem).SitePrice.ToString("c");
                }

                if (this.DisplayInventory)
                {
					Commerce.Catalog.Product prod = (Commerce.Catalog.Product)e.Row.DataItem;
                    if (prod.IsAvailableForSale)
                    {
                        InventoryLabel.Text = "Available for Sale";
                    }
                }
            }
            else if (e.Row.RowType == DataControlRowType.Header)
            {
                if (this.DisplayPrice)
                {
                    e.Row.Cells[3].Text = "Site Price";
                }

                if (this.DisplayInventory)
                {
                    e.Row.Cells[4].Text = "Available Qty";
                }

            }
        }

        protected void lstPage_SelectedIndexChanged(object sender, EventArgs e)
        {
            int temp = 1;
            if (int.TryParse(this.lstPage.SelectedItem.Value, out temp))
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