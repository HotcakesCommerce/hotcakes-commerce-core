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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Products_Edit_Inventory : BaseProductPage
    {
        private Product localProduct;

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit Product Inventory";
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);

            localProduct = HccApp.CatalogServices.Products.Find(ProductId);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                CurrentTab = AdminTabType.Catalog;
                LoadInventory();
                SetOutOfStockMode();
            }
        }

        private void SetOutOfStockMode()
        {
            if (localProduct != null)
            {
                if (OutOfStockModeField.Items.FindByValue(((int) localProduct.InventoryMode).ToString()) != null)
                {
                    OutOfStockModeField.ClearSelection();
                    OutOfStockModeField.Items.FindByValue(((int) localProduct.InventoryMode).ToString()).Selected = true;
                }
            }
        }

        private void LoadInventory()
        {
            var inventory = new List<ProductInventory>();
            HccApp.CatalogServices.InventoryGenerateForProduct(localProduct);
            HccApp.CatalogServices.CleanUpInventory(localProduct);
            HccApp.CatalogServices.UpdateProductVisibleStatusAndSave(localProduct);
            inventory = HccApp.CatalogServices.ProductInventories.FindByProductId(ProductId);

            if (localProduct.IsAvailableForSale)
            {
                ucMessageBox.ShowOk("This product is displayed on the store based on inventory");
            }
            else
            {
                ucMessageBox.ShowWarning("This product is NOT displayed on the store based on inventory");
            }

            EditsGridView.DataSource = inventory;
            EditsGridView.DataBind();
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Catalog/Products_Edit.aspx?id=" +
                              Request.QueryString["id"]);
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                ucMessageBox.ShowOk("Changes Saved!");
            }
            LoadInventory();
        }

        protected bool Save()
        {
            var result = false;

            localProduct.InventoryMode = (ProductInventoryMode) int.Parse(OutOfStockModeField.SelectedValue);
            HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(localProduct);

            // Process each variant/product row
            foreach (GridViewRow row in EditsGridView.Rows)
            {
                if (row.RowType == DataControlRowType.DataRow)
                {
                    ProcessRow(row);
                }
            }

            HccApp.CatalogServices.UpdateProductVisibleStatusAndSave(localProduct);
            result = true;
            return result;
        }

        private void ProcessRow(GridViewRow row)
        {
            var inventoryBvin = (string) EditsGridView.DataKeys[row.RowIndex].Value;
            var localInventory = HccApp.CatalogServices.ProductInventories.Find(inventoryBvin);

            var AdjustmentField = (TextBox) row.FindControl("AdjustmentField");
            var AdjustmentModeField = (DropDownList) row.FindControl("AdjustmentModeField");
            var LowStockPointField = (TextBox) row.FindControl("LowPointField");
            var OutOfStockPointField = (TextBox) row.FindControl("OutOfStockPointField");

            if (LowStockPointField != null)
            {
                int temp;
                if (int.TryParse(LowStockPointField.Text, out temp))
                {
                    localInventory.LowStockPoint = temp;
                }
            }

            if (OutOfStockModeField != null)
            {
                int tempOut;
                if (int.TryParse(OutOfStockPointField.Text, out tempOut))
                {
                    localInventory.OutOfStockPoint = tempOut;
                }
            }

            if (AdjustmentModeField != null)
            {
                if (AdjustmentField != null)
                {
                    var qty = 0;
                    int.TryParse(AdjustmentField.Text, out qty);
                    switch (AdjustmentModeField.SelectedValue)
                    {
                        case "1":
                            // Add
                            localInventory.QuantityOnHand += qty;
                            break;
                        case "2":
                            // Subtract
                            localInventory.QuantityOnHand -= qty;
                            break;
                        case "3":
                            // Set To
                            localInventory.QuantityOnHand = qty;
                            break;
                        default:
                            // Add
                            localInventory.QuantityOnHand += qty;
                            break;
                    }
                }
            }

            HccApp.CatalogServices.ProductInventories.Update(localInventory);
        }

        protected void EditsGridView_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var rowP = (ProductInventory) e.Row.DataItem;
                if (rowP != null)
                {
                    var lblSKU = e.Row.FindControl("lblSKU") as Label;
                    if (lblSKU != null)
                    {
                        if (!string.IsNullOrEmpty(rowP.VariantId))
                        {
                            var v = localProduct.Variants.FindByBvin(rowP.VariantId);
                            if (v != null)
                            {
                                var sku = v.Sku;
                                if (string.IsNullOrWhiteSpace(v.Sku))
                                    sku = localProduct.Sku;
                                var variantName = sku + "<br />";
                                var selectionNames = v.SelectionNames(localProduct.Options.VariantsOnly());
                                for (var i = 0; i < selectionNames.Count; i++)
                                {
                                    variantName += selectionNames[i];
                                    if (i != selectionNames.Count - 1)
                                        variantName += ", ";
                                }
                                lblSKU.Text = variantName;
                            }
                        }
                        else
                        {
                            lblSKU.Text = localProduct.Sku;
                        }
                    }

                    var lblQuantityOnHand = e.Row.FindControl("lblQuantityOnHand") as Label;
                    var lblQuantityReserved = e.Row.FindControl("lblQuantityReserved") as Label;
                    var lblQuantityAvailableForSale = e.Row.FindControl("lblQuantityAvailableForSale") as Label;
                    var LowStockPointField = e.Row.FindControl("LowPointField") as TextBox;
                    var OutOfStockPointField = e.Row.FindControl("OutOfStockPointField") as TextBox;
                    if (lblQuantityOnHand != null)
                    {
                        lblQuantityOnHand.Text = rowP.QuantityOnHand.ToString();
                    }
                    if (lblQuantityAvailableForSale != null)
                    {
                        lblQuantityAvailableForSale.Text = rowP.QuantityAvailableForSale.ToString();
                    }
                    if (lblQuantityReserved != null)
                    {
                        lblQuantityReserved.Text = rowP.QuantityReserved.ToString();
                    }
                    if (LowStockPointField != null)
                    {
                        LowStockPointField.Text = rowP.LowStockPoint.ToString();
                    }
                    if (OutOfStockPointField != null)
                    {
                        OutOfStockPointField.Text = rowP.OutOfStockPoint.ToString();
                    }
                }
            }
        }
    }
}