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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Marketing.PromotionQualifications;
using Hotcakes.Modules.Core.Admin.AppCode;
using System.Collections.Generic;
using Hotcakes.Commerce.Catalog;
using System.Web.UI;

namespace Hotcakes.Modules.Core.Admin.Marketing.Qualifications
{
    public partial class ProductIsEditor : BaseQualificationControl
    {
        private HasProductsQualificationBase TypedQualification
        {
            get { return Qualification as HasProductsQualificationBase; }
        }

		public string Title { get; set; }

		public override void LoadQualification()
		{
			ucProductPicker.LoadSearch();

			List<FriendlyBvinDisplay> displayData = new List<FriendlyBvinDisplay>();
			Dictionary<Product, List<string>> selectedIds = LoadIds();
			foreach (Product product in selectedIds.Keys)
			{
				FriendlyBvinDisplay item = new FriendlyBvinDisplay();
				item.bvin = product.Bvin;
				item.DisplayName = product.Bvin;
				item.Sku = product.Sku;

				Hotcakes.Commerce.Catalog.Variant v = HccApp.CatalogServices.ProductVariants.Find(item.bvin);

				item.DisplayName = "[" + product.Sku + "] " + product.ProductName;
				bool hasVariants = product.HasVariants();

				int availableVariants = 0;
				if (product.IsBundle)
				{
					availableVariants = product.BundledProducts.Sum(x => x.BundledProduct.Variants.Count());
					hasVariants = availableVariants > 0;
				}


				item.HasVariants = hasVariants;
				item.AvailableVariants = availableVariants;
				if (hasVariants)
				{
					item.AvailableVariants = product.IsBundle ? availableVariants : product.Variants.Count();
					item.SelectedVariants = selectedIds[product];
				}

				displayData.Add(item);
			}

			gvProductBvins.DataSource = displayData;
			gvProductBvins.DataBind();
			upProductPicker.Update();
		}

		public override bool SaveQualification()
		{
			return UpdatePromotion();
		}

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            gvProductBvins.RowDeleting += gvProductBvins_RowDeleting;
            btnAddProduct.Click += btnAddProduct_Click;
			if (!string.IsNullOrEmpty(Title))
			{
				Title = Localization.GetString(Title);
			}
			else
			{
				Title = Localization.GetString("WhenProductIs");
			}
        }

		protected void btnAddProduct_Click(object sender, EventArgs e)
        {
            TypedQualification.AddProductIds(ucProductPicker.SelectedProducts.OfType<string>());
            UpdatePromotion();
            LoadQualification();
        }

		protected void btnDelete_OnPreRender(object sender, EventArgs e)
		{
			var link = (LinkButton)sender;
			link.Text = Localization.GetString("Delete");
		}

		protected void gvProductBvins_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				LinkButton variants = (LinkButton)e.Row.FindControl("btnProductVariants");
				Literal ltrNoVariants = (Literal)e.Row.FindControl("ltrNoVariants");
				HiddenField hdfSelectedVariants = (HiddenField)e.Row.FindControl("hdfSelectedVariants");

				if (variants != null)
				{
					FriendlyBvinDisplay dataItem = (FriendlyBvinDisplay)e.Row.DataItem;
					if (dataItem.HasVariants == false)
					{
						variants.Visible = false;
						ltrNoVariants.Text = Localization.GetString("None");
					}
					else
					{
						ltrNoVariants.Visible = false;
						variants.Text = Localization.GetString("All");
						if (dataItem.AvailableVariants != dataItem.SelectedVariants.Count)
						{
							variants.Text = dataItem.SelectedVariants.Count.ToString() + " " + Localization.GetString("Of") + " " + dataItem.AvailableVariants.ToString();
							hdfSelectedVariants.Value = string.Join(",", dataItem.SelectedVariants.ToArray());
						}
					}
				}
			}
		}
		
		protected void gvProductBvins_RowCommand(object sender, GridViewCommandEventArgs e)
		{
			if (e.CommandName == "EditVariants")
			{
				var index = Convert.ToInt32(e.CommandArgument);
				var productBvin = (string)gvProductBvins.DataKeys[index].Value;
				List<string> selectedVariants = null;
				if (e.CommandSource is Control)
				{
					GridViewRow row = (GridViewRow)((Control)e.CommandSource).NamingContainer;
					HiddenField hdfSelectedVariants = (HiddenField)row.FindControl("hdfSelectedVariants");
					selectedVariants = string.IsNullOrEmpty(hdfSelectedVariants.Value) ? new List<string>() : hdfSelectedVariants.Value.Split(',').ToList();
				}

				pnlSelectProductVariants.Visible = true;
				ProductVariantsPicker.Visible = true;
				ProductVariantsPicker.LoadProduct(productBvin, selectedVariants);
				upPanel1.Update();

				//string parentElement = upProductPicker.ClientID;
				//string parentElement = pnlSelectProductVariants.ClientID;
				string parentElement = upPanel1.ClientID;
				string closeEvent = btnCloseProductVariantsSelectionEditor.UniqueID; ;
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcSelectVariantDialog", "hcSelectVariantDialog('" + productBvin + "', '" + parentElement + "','" + closeEvent + "');", true);
				ScriptManager.RegisterStartupScript(Page, Page.GetType(), "hcBindSelectionToggle", "hcBindSelectionToggle();", true);
			}
		}
		
		protected void gvProductBvins_RowDeleting(object sender, GridViewDeleteEventArgs e)
		{
			var q = TypedQualification;
			var selectedBvin = e.Keys[0] as string;
			bool hasProductId = q.GetProductIds().Contains(selectedBvin);

			if (hasProductId == false)
			{
				Product p = HccApp.CatalogServices.Products.FindWithCache(selectedBvin);
				List<string> variantsBvin = p.Variants.Select(x => x.Bvin).ToList();
				//If product is removed from promotion then clear all of its variants 
				if (p != null)
				{
					foreach (var variantBvin in variantsBvin)
					{
						q.RemoveProductId(variantBvin);
					}
				}
			}
			else
			{
				q.RemoveProductId(selectedBvin);
			}
			UpdatePromotion();
			LoadQualification();
		}
	
		protected void btnSaveProductVariantsSelection_Click(object sender, EventArgs e)
		{
			string productBvin = ProductVariantsPicker.ProductBvin;
			List<string> selectedVariants = ProductVariantsPicker.SelectedVariants;
			pnlSelectProductVariants.Visible = false;

			UpdateVariantSelectionForProduct(productBvin, selectedVariants);
			ClearProductVariantsPicker();
		}

		protected void btnCloseProductVariantsSelectionEditor_Click(object sender, EventArgs e)
		{
			pnlSelectProductVariants.Visible = false;
			ClearProductVariantsPicker();
		}

		private void ClearProductVariantsPicker()
		{
			ProductVariantsPicker.Visible = false;
			ProductVariantsPicker.ClearProduct();
			upPanel1.Update();
		}

		private void UpdateVariantSelectionForProduct(string selectedBvin, List<string> selection)
        {
			var q = TypedQualification;
			Product p = HccApp.CatalogServices.Products.FindWithCache(selectedBvin);
			if (p != null)
			{
				//Get all variants
				List<string> allVariants = new List<string>();
				if (p.IsBundle)
				{
					foreach (var item in p.BundledProducts)
					{
						if (item.BundledProduct.HasVariants())
						{
							allVariants.AddRange(item.BundledProduct.Variants.Select(x => (item.ProductId + "#" + x.Bvin)));
						}
					}
				}
				else
				{
					allVariants = p.Variants.Select(x => x.Bvin).ToList();
				}

				//Clean all existing id's for this product in promotion
				q.RemoveProductId(selectedBvin);
				foreach (string variantBvin in allVariants)
				{
					q.RemoveProductId(variantBvin);
				}

				//Save current selection in promotion
				//If all variants are selected then add Product Id instead
				if (selection.Count == allVariants.Count)
				{
					q.AddProductIds(new List<string>() { selectedBvin });
				}
				else
				{
					string prefix = string.Empty;
					if (p.IsBundle)
					{
						prefix = p.Bvin + "#";
					}

					foreach (string variantBvin in selection)
					{
						q.AddProductIds(new List<string>() { prefix + variantBvin });
					}
				}
			}
            UpdatePromotion();
            LoadQualification();
        }

		private Dictionary<Product, List<string>> LoadIds()
        {
			Dictionary<Product, List<string>> selectedIds = new Dictionary<Product, List<string>>();
			List<Tuple<Product, List<string>>> bundledProducts = new List<Tuple<Product, List<string>>>();

			foreach (string bvin in TypedQualification.GetProductIds())
			{
				//check if its single bvin or combined
				string productBvin = bvin;
				string variantBvin = bvin;
				bool isCombined = false;

				string[] aBvin = bvin.Split('#');
				if (aBvin.Count() > 1)
				{
					productBvin = aBvin[0];
					variantBvin = aBvin[1];
					isCombined = true;
				}

				if (isCombined)
				{
					Hotcakes.Commerce.Catalog.Product p = HccApp.CatalogServices.Products.FindWithCache(productBvin);
					Hotcakes.Commerce.Catalog.Variant v = HccApp.CatalogServices.ProductVariants.Find(variantBvin);

					if (p != null && v != null)
					{
						var loadedProduct = selectedIds.Keys.Where(x => x.Bvin == p.Bvin).FirstOrDefault();
						if (loadedProduct == null)
						{
							List<string> selectedVariants = new List<string>() { v.Bvin };
							selectedIds.Add(p, selectedVariants);
						}
						else
						{
							List<string> selectedVariants = selectedIds[loadedProduct];
							if (selectedVariants == null)
							{
								selectedIds[loadedProduct] = new List<string>() { v.Bvin };
							}
							else
							{
								selectedIds[loadedProduct].Add(v.Bvin);
							}
						}
					}
				}
				else
				{
					//if its product ID
					Hotcakes.Commerce.Catalog.Product p = HccApp.CatalogServices.Products.FindWithCache(productBvin);
					if (p != null)
					{
						if (!selectedIds.ContainsKey(p))
						{
							if (p.IsBundle)
							{
								List<string> allVariants = new List<string>();
								foreach (var product in p.BundledProducts)
								{
									if (product.BundledProduct.HasVariants())
									{
										allVariants.AddRange(product.BundledProduct.Variants.Select(x => x.Bvin).ToList());
									}
								}
								selectedIds.Add(p, allVariants);
							}
							else
							{
								List<string> allVariants = p.Variants.Select(x => x.Bvin).ToList();
								selectedIds.Add(p, allVariants);
							}
						}
					}
					else
					{
						//if its Variant ID
						Hotcakes.Commerce.Catalog.Variant v = HccApp.CatalogServices.ProductVariants.Find(bvin);
						if (v != null)
						{
							Product parentProduct = HccApp.CatalogServices.Products.FindWithCache(v.ProductId);

							//Check if product is already loaded in dictionary
							var loadedProduct = selectedIds.Keys.Where(x => x.Bvin == v.ProductId).FirstOrDefault();
							if (loadedProduct == null)
							{
								List<string> selectedVariants = new List<string>() { v.Bvin };
								selectedIds.Add(parentProduct, selectedVariants);
							}
							else
							{
								List<string> selectedVariants = selectedIds[loadedProduct];
								if (selectedVariants == null)
								{
									selectedIds[loadedProduct] = new List<string>() { v.Bvin };
								}
								else
        {
									selectedIds[loadedProduct].Add(v.Bvin);
								}
							}
						}
					}
				}
			}

			return selectedIds;
        }

    }
}