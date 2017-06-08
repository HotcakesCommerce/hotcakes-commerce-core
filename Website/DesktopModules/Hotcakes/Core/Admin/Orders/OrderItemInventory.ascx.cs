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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class OrderItemInventory : HccUserControl
    {
        #region Properties

        private const string HCC_KEY = "hcc";

        private const string productLinkFormat =
            "<a href=\"/DesktopModules/Hotcakes/Core/Admin/catalog/Products_Performance.aspx?id={1}\">{0}</a>";

        public bool AllowUpdateQuantities { get; set; }

        private bool isPaymentRefunded { get; set; }

        private List<LineItem> lineItems { get; set; }

        public string OrderId
        {
            get
            {
                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                    return id;
                return (string) ViewState["OrderId"] ?? string.Empty;
            }
            set { ViewState["OrderId"] = value; }
        }

        private Order _currentOrder;

        public Order CurrentOrder
        {
            get
            {
                if (_currentOrder == null)
                {
                    _currentOrder = HccApp.OrderServices.Orders.FindForCurrentStore(OrderId);

                    if (_currentOrder != null && _currentOrder.IsRecurring)
                    {
                        foreach (var li in _currentOrder.Items)
                        {
                            li.RecurringBilling.LoadPaymentInfo(HccApp);
                        }
                    }
                }
                return _currentOrder;
            }
        }

        #endregion

        #region Public methods

        public bool CheckCurrentOrderhasInventoryProduct()
        {
            var result = false;
            lineItems = new List<LineItem>();
            foreach (var li in CurrentOrder.Items)
            {
                var product = HccApp.CatalogServices.Products.Find(li.ProductId);
                if (product != null)
                {
                    if (product.InventoryMode != ProductInventoryMode.AlwayInStock &&
                        product.InventoryMode != ProductInventoryMode.NotSet)
                    {
                        lineItems.Add(li);
                        result = true;
                    }
                }
                else
                {
                    result = true;
                }
            }
            return result;
        }

        public void BindGrids()
        {
            CheckCurrentOrderhasInventoryProduct();
            if (lineItems != null && lineItems.Any())
            {
                gvItems.DataSource = lineItems;
                gvItems.DataBind();
            }
        }

        public bool CheckOrderHasRefunded()
        {
            if (CurrentOrder == null)
            {
                return false;
            }

            var currentOrderTransactions =
                HccApp.OrderServices.Transactions.FindForOrder(CurrentOrder.bvin)
                    .OrderByDescending(y => y.TimeStampUtc)
                    .ToList();
            if (currentOrderTransactions == null || currentOrderTransactions.Count < 1)
            {
                return true;
            }
            foreach (var t in currentOrderTransactions)
            {
                if (t.Success)
                {
                    if (t.Action == ActionType.CashReturned || t.Action == ActionType.CreditCardRefund
                        || t.Action == ActionType.GiftCardIncrease || t.Action == ActionType.PayPalRefund
                        || t.Action == ActionType.RewardPointsIncrease ||
                        t.Action == ActionType.ThirdPartyPayMethodRefund
                        || t.Action == ActionType.CheckReturned || t.Action == ActionType.CreditCardHold
                        || t.Action == ActionType.OfflinePaymentRequest)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        #endregion

        #region Events

        protected void Page_Load(object sender, EventArgs e)
        {
            //BindGrids();
        }

        protected void gvItems_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lineItem = e.Row.DataItem as LineItem;
                if (lineItem == null)
                    return;

                var productLink = GetProductEditLink(lineItem.ProductId, lineItem.ProductSku);

                var lblSKU = e.Row.FindControl("lblSKU") as Label;
                var lblName = e.Row.FindControl("lblName") as Label;
                var lblOrderQty = e.Row.FindControl("lblOrderQty") as Label;
                var lblReplenishedQty = e.Row.FindControl("lblReplenishedQty") as Label;
                var txtQty = e.Row.FindControl("txtQty") as TextBox;
                var hdfReplenishedAll = e.Row.FindControl("hdfReplenishedAll") as HiddenField;
                var chbSelected = e.Row.FindControl("chbSelected") as CheckBox;

                var rvTxtQty = e.Row.FindControl("rvTxtQty") as RangeValidator;
                rvTxtQty.ErrorMessage = Localization.GetString("rvTxtQty.ErrorMessage");

                var hdfLineItemId = e.Row.FindControl("hdfLineItemId") as HiddenField;
                hdfLineItemId.Value = lineItem.Id.ToString();

                lblSKU.Text = string.IsNullOrEmpty(productLink) ? lineItem.ProductSku : productLink;
                lblName.Text = lineItem.ProductName;
                lblOrderQty.Text = lineItem.Quantity.ToString();

                var alreadyReservedQty = 0;
                var reservedQty = lineItem.CustomPropertyGet(HCC_KEY, "replenishedQuantity");
                if (!string.IsNullOrEmpty(reservedQty))
                {
                    alreadyReservedQty = int.Parse(reservedQty);
                }

                if (alreadyReservedQty >= lineItem.Quantity)
                {
                    txtQty.Text = "0";
                    txtQty.Enabled = false;
                    hdfReplenishedAll.Value = "true";
                    chbSelected.Enabled = false;
                    chbSelected.Checked = false;

                    rvTxtQty.MinimumValue = Convert.ToString("0");
                    rvTxtQty.MaximumValue = Convert.ToString("1");
                }
                else
                {
                    txtQty.Text = (lineItem.Quantity - alreadyReservedQty).ToString();

                    rvTxtQty.MinimumValue = Convert.ToString("1");
                    rvTxtQty.MaximumValue = (lineItem.Quantity - alreadyReservedQty).ToString();
                    rvTxtQty.ErrorMessage =
                        string.Format("Entered quantity should be equal to order quantity or less. {0}",
                            alreadyReservedQty > 0 ? "Already replenish quantity: " + alreadyReservedQty : string.Empty);
                    rvTxtQty.EnableClientScript = true;
                }

                lblReplenishedQty.Text = alreadyReservedQty.ToString();
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            if (UpdatReservedQuantity())
            {
                var success = false;

                switch (CurrentOrder.ShippingStatus)
                {
                    case OrderShippingStatus.FullyShipped:
                        success = HccApp.OrderServices.OrdersDelete(CurrentOrder.bvin, HccApp);
                        break;
                    case OrderShippingStatus.NonShipping:
                        success = HccApp.OrderServices.OrdersDelete(CurrentOrder.bvin, HccApp);
                        break;
                    case OrderShippingStatus.PartiallyShipped:
                        ucMessageBox.ShowWarning(
                            "Partially shipped orders can't be deleted. Either unship or ship all items before deleting.");
                        break;
                    case OrderShippingStatus.Unknown:
                        success = HccApp.OrderServices.OrdersDelete(CurrentOrder.bvin, HccApp);
                        break;
                    case OrderShippingStatus.Unshipped:
                        success = HccApp.OrderServices.OrdersDelete(CurrentOrder.bvin, HccApp);
                        break;
                }

                if (success)
                {
                    Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Orders/Default.aspx");
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
        }

        #endregion

        #region Private Methods

        private string GetProductEditLink(string productBvin, string textValue)
        {
            if (!string.IsNullOrEmpty(productBvin) && !string.IsNullOrEmpty(textValue))
            {
                return string.Format(productLinkFormat, textValue, productBvin);
            }

            return string.Empty;
        }

        public bool UpdatReservedQuantity()
        {
            var gridView = gvItems;
            for (var i = 0; i < gridView.Rows.Count; i++)
            {
                var currentRow = gridView.Rows[i];

                if (currentRow.RowType == DataControlRowType.DataRow)
                {
                    var lineitemId = (long) gridView.DataKeys[currentRow.RowIndex].Value;
                    var li = CurrentOrder.Items.FirstOrDefault(y => y.Id == lineitemId);
                    if (li != null)
                    {
                        var txtQty = currentRow.FindControl("txtQty") as TextBox;
                        var chbSelected = currentRow.FindControl("chbSelected") as CheckBox;
                        if (chbSelected.Checked)
                        {
                            if (txtQty != null)
                            {
                                var tempQty = txtQty.Text.Trim();
                                var actualQty = li.Quantity;
                                int.TryParse(tempQty, out actualQty);
                                SaveInventory(li.ProductId, li.VariantId, actualQty);
                            }
                        }
                    }
                }
            }
            return true;
        }


        public bool SaveInventory(string productId, string variantId, int replenishQty)
        {
            var result = false;
            var inv = HccApp.CatalogServices.ProductInventories.FindByProductIdAndVariantId(productId, variantId);
            var product = HccApp.CatalogServices.Products.Find(productId);
            var lineItem =
                CurrentOrder.Items
                    .FirstOrDefault(y => y.ProductId == product.Bvin && inv.VariantId == variantId);

            if (inv.ProductBvin == product.Bvin && inv.VariantId == variantId)
            {
                var alreadyReservedQty = 0;
                var reservedQty = lineItem.CustomPropertyGet(HCC_KEY, "replenishedQuantity");
                if (!string.IsNullOrEmpty(reservedQty))
                {
                    alreadyReservedQty = int.Parse(reservedQty);
                }

                if (
                    CurrentOrder.Items
                        .FirstOrDefault(y => y.ProductId == product.Bvin && y.VariantId == variantId)
                        .Quantity >= alreadyReservedQty)
                {
                    inv.QuantityReserved -= replenishQty;
                    result = HccApp.CatalogServices.ProductInventories.Update(inv);

                    alreadyReservedQty += replenishQty;
                    CurrentOrder.Items
                        .FirstOrDefault(y => y.ProductId == product.Bvin)
                        .CustomPropertySet(HCC_KEY, "replenishedQuantity", alreadyReservedQty.ToString());
                    HccApp.OrderServices.Orders.Update(CurrentOrder);
                    HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(product);
                }
                else
                {
                    return false;
                }
                return result;
            }

            ucMessageBox.ShowError("Inventory line item does not match.");
            return result;
        }

        #endregion
    }
}