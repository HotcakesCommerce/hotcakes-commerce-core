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
using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Modules.Core.Admin.Controls
{
	partial class ProductVariantsPicker : HccUserControl
	{
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
		private Product CurrentProduct { get; set; }
		private List<Variant> ProductVariants { get; set; }
		private List<string> DefaultSelection 
		{
			get 
			{
				if (ViewState["DefaultSelection"] != null)
				{
					return (List<string>)ViewState["DefaultSelection"];  
				}
				return new List<string>();
				
			}
			set 
			{ 
				ViewState["DefaultSelection"] = value; 
			}
		}

		public string ProductBvin
		{
			get
			{
				if (ViewState["SelectedProductBvin"] != null)
				{
					return ViewState["SelectedProductBvin"].ToString();
				}
				return string.Empty;

			}
			set { ViewState["SelectedProductBvin"] = value; }
		}
		public List<string> SelectedVariants 
		{
			get 
			{ 
				return GetSelectedVariants(); 
			} 
		}

		public void ClearProduct()
		{
			this.ProductBvin = string.Empty;
			this.DefaultSelection = new List<string>();

			this.CurrentProduct = null;
			this.ProductVariants = null;

			ProducSku.Text = string.Empty;
			ProductName.Text = string.Empty;
			TotalVariants.Text = string.Empty;

			CurrentProductBvin.Value = string.Empty;

			gvVariants.DataSource = new List<Variant>();
			gvVariants.DataBind();
			upVariantsSelection.Update();
		}

		public void LoadProduct(string bvin, List<string> selectedVariants)
		{
			this.ProductBvin = bvin;
			this.DefaultSelection = selectedVariants;

			LoadData();
			BindData();

		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
		}

		protected string GetVariantName(IDataItemContainer cont)
		{
			var variant = cont.DataItem as Variant;
			if (variant != null)
			{
				return GetVariantDescription(variant);
			}
			return string.Empty;
			
		}

		protected void gvVariants_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				CheckBox chkSelected = e.Row.FindControl("chkSelected") as CheckBox;
				Variant dataItem = (Variant)e.Row.DataItem;
				if (dataItem == null)
				{
					return;
				}
				chkSelected.Checked = this.DefaultSelection.Count == 0 || this.DefaultSelection.Contains(dataItem.Bvin);
			}
		}

		private void LoadData()
		{
			Hotcakes.Commerce.Catalog.Product p = HccApp.CatalogServices.Products.FindWithCache(this.ProductBvin);
			if (p != null)
			{
				this.CurrentProduct = p;
				List<Variant> allVariants = new List<Variant>();
				if (p.IsBundle)
				{
					foreach (var bItem in p.BundledProducts)
					{
						if (bItem.BundledProduct.HasVariants())
						{
							foreach (var v in bItem.BundledProduct.Variants)
							{
								var newV = new Variant
								{
									StoreId = v.StoreId,
									Bvin = v.Bvin,
									ProductId = v.ProductId,
									Price = (v.Price < 0) ? bItem.BundledProduct.SitePrice : v.Price,
									Sku = string.IsNullOrEmpty(v.Sku) ? bItem.BundledProduct.Sku : v.Sku
								};
								newV.Selections.AddRange(v.Selections);
								allVariants.Add(newV);
							}
						}
					}
				}
				else
				{
					foreach (var v in p.Variants)
					{
						var newV = new Variant
						{
							StoreId = v.StoreId,
							Bvin = v.Bvin,
							ProductId = v.ProductId,
							Price = (v.Price < 0) ? p.SitePrice : v.Price,
							Sku = string.IsNullOrEmpty(v.Sku) ? p.Sku : v.Sku
						};
						newV.Selections.AddRange(v.Selections);
						allVariants.Add(newV);
					}
				}
				this.ProductVariants = allVariants;
			}
		}

		private void BindData()
		{
			if (this.CurrentProduct == null || this.ProductVariants == null)
			{
				return;
			}

			ProducSku.Text = this.CurrentProduct.Sku;
			ProductName.Text = this.CurrentProduct.ProductName;
			TotalVariants.Text = this.CurrentProduct.Variants.Count.ToString();

			gvVariants.EnableViewState = true;
			gvVariants.DataSource = this.ProductVariants;
			gvVariants.DataBind();
		}

		private string GetVariantDescription(Variant variant)
		{
			if (CurrentProduct.IsBundle)
			{
				var parentProduct = CurrentProduct.BundledProducts.Where(x => x.BundledProduct.Bvin  == variant.ProductId).FirstOrDefault().BundledProduct;
				var selections = variant.SelectionNames(parentProduct.Options);
				return string.Join("/", selections);
			}
			else
			{
				var selections = variant.SelectionNames(CurrentProduct.Options);
				return string.Join("/", selections);
			}
		}

		private List<string> GetSelectedVariants()
		{
			List<string> selectedItems = new List<string>();

			var gridItems = gvVariants.Rows;
			for (int i = 0; i < gridItems.Count; i++)
			{
				GridViewRow currentRow = gvVariants.Rows[i];
				if (currentRow.RowType == DataControlRowType.DataRow)
				{
					string bvin = gvVariants.DataKeys[currentRow.RowIndex].Value.ToString();
					CheckBox chkSelected = currentRow.FindControl("chkSelected") as CheckBox;

					if(chkSelected.Checked) 
					{
						selectedItems.Add(bvin);
					}
				}
			}

			return selectedItems;
		}
	}
}