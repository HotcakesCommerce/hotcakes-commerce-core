#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class OrderItems : HccUserControl
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            ucOrderAddProduct.Visible = EditMode;
            ucOrderAddProduct.CurrentOrder = CurrentOrder;
            ucOrderAddProduct.ProductAdded += ucOrderAddProduct_ProductAdded;
        }

        protected void gvItems_DataBinding(object sender, EventArgs e)
        {
            // Check if column was found. Probably we are in PostBack and it's name was already translated
            var shippingColumn =
                gvItems.Columns.OfType<DataControlField>().FirstOrDefault(c => c.HeaderText == "Shipping");
            if (shippingColumn != null)
                shippingColumn.Visible = !EditMode;

            var actionsColumn =
                gvItems.Columns.OfType<DataControlField>().FirstOrDefault(c => c.HeaderText == "Actions");
            if (actionsColumn != null)
                actionsColumn.Visible = EditMode;

            LocalizationUtils.LocalizeGridView(gvItems, Localization);
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
                var lblDescription = e.Row.FindControl("lblDescription") as Label;
                var lblShippingStatus = e.Row.FindControl("lblShippingStatus") as Label;
                var lblAdjustedPrice = e.Row.FindControl("lblAdjustedPrice") as Label;
                var txtQty = e.Row.FindControl("txtQty") as TextBox;
                var lblQty = e.Row.FindControl("lblQty") as Label;
                var lblLineTotal = e.Row.FindControl("lblLineTotal") as Label;
                var btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
                var lblLineTotalWithoutDiscounts = e.Row.FindControl("lblLineTotalWithoutDiscounts") as Label;
                var lblLineTotalWithoutUpcharge = e.Row.FindControl("lblLineTotalWithoutUpcharge") as Label;
                var litDiscounts = (Literal) e.Row.FindControl("litDiscounts");
                var litUpcharge = (Literal)e.Row.FindControl("litUpcharge");
                var divGiftWrap = e.Row.FindControl("divGiftWrap") as HtmlGenericControl;
                var litGiftCardsNumbers = e.Row.FindControl("litGiftCardsNumbers") as Literal;
                var lnkGiftCards = e.Row.FindControl("lnkGiftCards") as HyperLink;

                var rvTxtQty = e.Row.FindControl("rvTxtQty") as RangeValidator;
                rvTxtQty.ErrorMessage = Localization.GetString("rvTxtQty.ErrorMessage");

                var lnkInventory = e.Row.FindControl("btnInventory") as LinkButton;
                lnkInventory.Visible = false;
                var product = HccApp.CatalogServices.Products.Find(lineItem.ProductId);
                if (product != null)
                {
                    if (product.InventoryMode != ProductInventoryMode.NotSet && EditMode &&
                        (CurrentOrder.StatusCode == OrderStatusCode.Cancelled
                         || isPaymentRefunded))
                    {
                        if (product.InventoryMode != ProductInventoryMode.AlwayInStock &&
                            product.InventoryMode != ProductInventoryMode.NotSet)
                        {
                            lnkInventory.Visible = true;
                        }
                    }
                }

                var hdfLineItemId = e.Row.FindControl("hdfLineItemId") as HiddenField;
                hdfLineItemId.Value = lineItem.Id.ToString();

                lblSKU.Text = string.IsNullOrEmpty(productLink) ? lineItem.ProductSku : productLink;
                lblDescription.Text = lineItem.ProductName;
                lblDescription.Text += "<br />" + lineItem.ProductShortDescription;
                lblShippingStatus.Text = LocalizationUtils.GetOrderShippingStatus(lineItem.ShippingStatus);
                lblAdjustedPrice.Text = lineItem.IsCoverCreditCardFees && lineItem.HasAnyUpcharge ? (lineItem.AdjustedPricePerItem + lineItem.TotalUpcharge()).ToString("C") : lineItem.AdjustedPricePerItem.ToString("C");
                txtQty.Text = lineItem.Quantity.ToString();
                txtQty.Visible = EditMode;
                lblQty.Text = lineItem.Quantity.ToString();
                lblQty.Visible = !EditMode;
                lblLineTotal.Text = lineItem.LineTotal.ToString("C");

                if (lineItem.HasAnyDiscounts)
                {
                    lblLineTotalWithoutDiscounts.Visible = true;
                    lblLineTotalWithoutDiscounts.Text = lineItem.LineTotalWithoutDiscounts.ToString("c");

                    litDiscounts.Text = "<div class=\"discounts\">" + lineItem.DiscountDetailsAsHtml() + "</div>";
                }

                if (lineItem.IsCoverCreditCardFees && lineItem.HasAnyUpcharge)
                {
                    lblLineTotalWithoutUpcharge.Visible = true;
                    lblLineTotalWithoutUpcharge.Text = lineItem.LineTotalWithoutUpcharge.ToString("c");

                    litUpcharge.Text = "<div class=\"discounts\">" + lineItem.UpchargeDetailsAsHtml() + "</div>";
                }

                if (!EditMode && !string.IsNullOrEmpty(lineItem.CustomPropGiftCardNumber))
                {
                    divGiftWrap.Visible = true;

                    litGiftCardsNumbers.Text = "Gift Card Number(s): <br/>" + lineItem.CustomPropGiftCardNumber +
                                               "<br/>";

                    lnkGiftCards.Text = Localization.GetString("GiftCardDetails");
                    lnkGiftCards.NavigateUrl = "/DesktopModules/Hotcakes/Core/Admin/catalog/GiftCards.aspx?lineitem=" +
                                               lineItem.Id;
                }

                btnDelete.CommandArgument = lineItem.Id.ToString();
                btnDelete.Text = Localization.GetString("Delete");
                btnDelete.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmDelete"));
            }
        }

        private string GetProductEditLink(string productBvin, string textValue)
        {
            if (!string.IsNullOrEmpty(productBvin) && !string.IsNullOrEmpty(textValue))
            {
                return string.Format(productLinkFormat, textValue, productBvin);
            }

            return string.Empty;
        }

        protected void gvSubscriptions_DataBinding(object sender, EventArgs e)
        {
            // Check if column was found. Probably we are in PostBack and it's name was already translated
            var totalPayedColumn =
                gvSubscriptions.Columns
                    .OfType<DataControlField>()
                    .FirstOrDefault(c => c.HeaderText == "TotalPayed");
            if (totalPayedColumn != null)
                totalPayedColumn.Visible = !EditMode;

            LocalizationUtils.LocalizeGridView(gvSubscriptions, Localization);
        }

        protected void gvSubscriptions_RowDataBound(object sender, GridViewRowEventArgs e)
        {
            if (e.Row.RowType == DataControlRowType.DataRow)
            {
                var lineItem = e.Row.DataItem as LineItem;
                if (lineItem == null)
                    return;

                var lblSKU = e.Row.FindControl("lblSKU") as Label;
                var lblDescription = e.Row.FindControl("lblDescription") as Label;
                var divGiftWrap = e.Row.FindControl("divGiftWrap") as HtmlGenericControl;
                var litGiftCardsNumbers = e.Row.FindControl("litGiftCardsNumbers") as Literal;
                var lnkGiftCards = e.Row.FindControl("lnkGiftCards") as HyperLink;
                var litDiscounts = e.Row.FindControl("litDiscounts") as Literal;
                var litUpcharge = e.Row.FindControl("litUpcharge") as Literal;
                var txtQty = e.Row.FindControl("txtQty") as TextBox;
                var lblQty = e.Row.FindControl("lblQty") as Label;
                var lblNextPaymentDate = e.Row.FindControl("lblNextPaymentDate") as Label;
                var lblLineTotalWithoutDiscounts = e.Row.FindControl("lblLineTotalWithoutDiscounts") as Label;
                var lblLineTotalWithoutUpcharge = e.Row.FindControl("lblLineTotalWithoutUpcharge") as Label;
                var lblLineTotal = e.Row.FindControl("lblLineTotal") as Label;
                var lblInterval = e.Row.FindControl("lblInterval") as Label;
                var lblTotalPayed = e.Row.FindControl("lblTotalPayed") as Label;
                var btnDelete = e.Row.FindControl("btnDelete") as LinkButton;
                var btnCancel = e.Row.FindControl("btnCancel") as LinkButton;

                lblSKU.Text = lineItem.ProductSku;
                lblDescription.Text = lineItem.ProductName;
                lblDescription.Text += "<br />" + lineItem.ProductShortDescription;
                txtQty.Text = lineItem.Quantity.ToString();
                txtQty.Visible = EditMode;
                lblQty.Text = lineItem.Quantity.ToString();
                lblQty.Visible = !EditMode;
                if (!lineItem.RecurringBilling.IsCancelled)
                    lblNextPaymentDate.Text = lineItem.RecurringBilling.NextPaymentDate.ToShortDateString();
                else
                    lblNextPaymentDate.Text = Localization.GetString("Cancelled");
                lblLineTotal.Text = lineItem.LineTotal.ToString("C");
                lblInterval.Text = Localization.GetFormattedString("Every", lineItem.RecurringBilling.Interval,
                    LocalizationUtils.GetRecurringIntervalLower(lineItem.RecurringBilling.IntervalType));
                lblTotalPayed.Text = lineItem.RecurringBilling.TotalPayed.ToString("C");

                if (lineItem.HasAnyDiscounts)
                {
                    lblLineTotalWithoutDiscounts.Visible = true;
                    lblLineTotalWithoutDiscounts.Text = lineItem.LineTotalWithoutDiscounts.ToString("c");

                    litDiscounts.Text = "<div class=\"discounts\">" + lineItem.DiscountDetailsAsHtml() + "</div>";
                }

                if (lineItem.HasAnyUpcharge)
                {
                    lblLineTotalWithoutUpcharge.Visible = true;
                    lblLineTotalWithoutUpcharge.Text = lineItem.LineTotalWithoutUpcharge.ToString("c");

                    litUpcharge.Text = "<div class=\"discounts\">" + lineItem.UpchargeDetailsAsHtml() + "</div>";
                }

                if (!EditMode && !string.IsNullOrEmpty(lineItem.CustomPropGiftCardNumber))
                {
                    divGiftWrap.Visible = true;

                    litGiftCardsNumbers.Text = "Gift Card Number(s): <br/>" + lineItem.CustomPropGiftCardNumber +
                                               "<br/>";

                    lnkGiftCards.Text = Localization.GetString("GiftCardDetails");
                    lnkGiftCards.NavigateUrl = "/DesktopModules/Hotcakes/Core/Admin/catalog/GiftCards.aspx?lineitem=" +
                                               lineItem.Id;
                }

                btnDelete.Visible = EditMode;
                btnDelete.CommandArgument = lineItem.Id.ToString();
                btnDelete.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmDelete"));

                btnCancel.Visible = !EditMode && !lineItem.RecurringBilling.IsCancelled;
                btnCancel.CommandArgument = lineItem.Id.ToString();
                btnCancel.OnClientClick = WebUtils.JsConfirmMessage(Localization.GetJsEncodedString("ConfirmCancel"));
            }
        }

        protected void gvSubscriptions_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "CancelSubscription")
            {
                var lineItemId = Convert.ToInt64(e.CommandArgument);
                var lineItem = CurrentOrder.Items.SingleOrDefault(y => y.Id == lineItemId);

                var payManager = new OrderPaymentManager(CurrentOrder, HccApp);
                var res = payManager.RecurringSubscriptionCancel(lineItemId);

                if (res.Succeeded)
                {
                    lineItem.RecurringBilling.LoadPaymentInfo(HccApp);

                    ucMessageBox.ShowOk(Localization.GetString("CancelledSuccessfully"));

                    var handler = OrderEdited;
                    if (handler != null)
                        handler(this, EventArgs.Empty);
                }
                else
                {
                    foreach (var error in res.Errors)
                    {
                        ucMessageBox.ShowError(error.Description);
                    }
                }
            }
        }

        protected void gridView_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var gridView = sender as GridView;
            var Id = (long) gridView.DataKeys[e.RowIndex].Value;

            var lineItem = CurrentOrder.Items.SingleOrDefault(y => y.Id == Id);
            if (lineItem != null)
            {
                // Before removing lineitem try to cancel subscription
                if (CurrentOrder.IsRecurring && !lineItem.RecurringBilling.IsCancelled)
                {
                    var payManager = new OrderPaymentManager(CurrentOrder, HccApp);
                    var res = payManager.RecurringSubscriptionCancel(lineItem.Id);
                }

                lineItem.QuantityReserved = lineItem.Quantity;
                HccApp.CatalogServices.InventoryLineItemUnreserveInventory(lineItem);

                CurrentOrder.Items.Remove(lineItem);

                HccApp.CalculateOrder(CurrentOrder);
                HccApp.OrderServices.EvaluatePaymentStatus(CurrentOrder);
                HccApp.OrderServices.Orders.Update(CurrentOrder);
            }

            var handler = OrderEdited;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected void btnUpdateQuantities_Click(object sender, EventArgs e)
        {
            var gridView = CurrentOrder.IsRecurring ? gvSubscriptions : gvItems;
            foreach (GridViewRow row in gridView.Rows)
            {
                if (row.RowType != DataControlRowType.DataRow)
                    continue;

                var itemId = (long) gridView.DataKeys[row.RowIndex].Value;
                var txtQty = row.FindControl("txtQty") as TextBox;

                var li = CurrentOrder.GetLineItem(itemId);
                var quantity = int.Parse(txtQty.Text.Trim());

                var opResult = HccApp.OrderServices.OrdersUpdateItemQuantity(itemId, quantity, CurrentOrder);
                if (!string.IsNullOrEmpty(opResult.Message))
                {
                    ucMessageBox.ShowError(opResult.Message);
                }
            }

            var result = HccApp.CheckForStockOnItems(CurrentOrder);
            if (!result.Success)
                ucMessageBox.ShowWarning(result.Message);

            HccApp.CalculateOrderAndSave(CurrentOrder);

            var handler = OrderEdited;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected void ucOrderAddProduct_ProductAdded(object sender, EventArgs e)
        {
            var handler = OrderEdited;
            if (handler != null)
                handler(this, EventArgs.Empty);
        }

        protected void gvItems_RowCommand(object sender, GridViewCommandEventArgs e)
        {
            if (e.CommandName == "Inventory")
            {
                var index = Convert.ToInt32(e.CommandArgument);
                var row = gvItems.Rows[index];

                var hdfLineItemId = row.FindControl("hdfLineItemId") as HiddenField;
                var lineItem =
                    CurrentOrder.Items.FirstOrDefault(y => y.Id == decimal.Parse(hdfLineItemId.Value));

                var alreadyReservedQty = 0;
                var reservedQty = lineItem.CustomPropertyGet("hcc", "replenishedQuantity");
                if (!string.IsNullOrEmpty(reservedQty))
                {
                    alreadyReservedQty = int.Parse(reservedQty);
                }

                if (alreadyReservedQty >= lineItem.Quantity)
                {
                    ucMessageBox.ShowError(string.Format("Already replenish the reserved quantity {0}.",
                        alreadyReservedQty));
                    return;
                }

                RegisterOpenDialogScript();
                LoadShippingMethodEditor(lineItem, alreadyReservedQty);
            }
        }

        private void LoadShippingMethodEditor(LineItem lineItem, int replenishQty)
        {
            if (lineItem.Quantity > 1)
            {
                txtReplenishQty.Visible = true;
                lblReplenishQty.Visible = true;
            }
            else
            {
                txtReplenishQty.Visible = false;
                lblReplenishQty.Visible = false;
            }

            hdfProductId.Value = lineItem.ProductId;
            hdfVariantId.Value = lineItem.VariantId;
            txtReplenishQty.Text = (lineItem.Quantity - replenishQty).ToString();
            lblInventoryReplenishMsg.Text = string.Format(
                "Are you sure you want to Replenish product '{0}' inventory?", lineItem.ProductName);

            if (lineItem.Quantity - replenishQty != 1)
            {
                rvtxtReplenishQty.MinimumValue = Convert.ToString("1");
                rvtxtReplenishQty.MaximumValue = (lineItem.Quantity - replenishQty).ToString();
                rvtxtReplenishQty.ErrorMessage =
                    string.Format("Entered quantity should be equal to order quantity or less. {0}",
                        replenishQty > 0 ? "Already replenish quantity: " + replenishQty : string.Empty);
            }
            else
            {
                rvtxtReplenishQty.Controls.Remove(txtReplenishQty);
                txtReplenishQty.Visible = false;
                lblReplenishQty.Visible = false;
            }
        }

        protected void btnSave_Click(object sender, EventArgs e)
        {
            Page.Validate("InventoryReplenish");

            if (Page.IsValid)
            {
                if (SaveInventory())
                {
                    ClearDialogControls();
                    RegisterCloseDialogScript();
                }
            }
        }

        private void ClearDialogControls()
        {
            hdfProductId.Value = string.Empty;
            hdfVariantId.Value = string.Empty;
            txtReplenishQty.Text = string.Empty;
        }

        private bool SaveInventory()
        {
            var result = false;

            var inv = HccApp.CatalogServices.ProductInventories.FindByProductIdAndVariantId(hdfProductId.Value,
                hdfVariantId.Value);
            var product = HccApp.CatalogServices.Products.Find(hdfProductId.Value);
            var lineItem =
                CurrentOrder.Items
                    .FirstOrDefault(y => y.ProductId == product.Bvin && y.VariantId == hdfVariantId.Value);

            if (inv.ProductBvin == product.Bvin && inv.VariantId == hdfVariantId.Value)
            {
                var alreadyReservedQty = 0;
                var reservedQty = lineItem.CustomPropertyGet("hcc", "replenishedQuantity");
                if (!string.IsNullOrEmpty(reservedQty))
                {
                    alreadyReservedQty = int.Parse(reservedQty);
                }

                if (
                    CurrentOrder.Items
                        .FirstOrDefault(y => y.ProductId == product.Bvin && y.VariantId == hdfVariantId.Value)
                        .Quantity >= alreadyReservedQty)
                {
                    var replenishedQty = lineItem.Quantity - alreadyReservedQty;
                    int.TryParse(txtReplenishQty.Text, out replenishedQty);

                    inv.QuantityReserved -= replenishedQty;
                    result = HccApp.CatalogServices.ProductInventories.Update(inv);

                    alreadyReservedQty += replenishedQty;
                    CurrentOrder.Items
                        .FirstOrDefault(y => y.ProductId == product.Bvin)
                        .CustomPropertySet("hcc", "replenishedQuantity", alreadyReservedQty.ToString());
                    HccApp.OrderServices.Orders.Update(CurrentOrder);
                    HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(product);
                }
                else
                {
                    ucMessageBox.ShowError(string.Format("Already replenish the reserved quantity {0}.",
                        alreadyReservedQty));
                    return false;
                }
                return result;
            }
            ucMessageBox.ShowError("Selected project in not found.");
            return result;
        }

        private bool CheckOrderHasRefunded()
        {
            currentOrderTransactions = currentOrderTransactions.OrderByDescending(y => y.TimeStampUtc).ToList();
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
                        || t.Action == ActionType.CheckReturned)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private int InventoryReserveQuantity(string productBvin, string variantId, int quantity,
            bool reserveZeroWhenQuantityTooLow)
        {
            var inv = HccApp.CatalogServices.ProductInventories.FindByProductIdAndVariantId(productBvin, variantId);

            // If no inventory, assume available
            if (inv == null)
                return quantity;

            var prod = HccApp.CatalogServices.Products.FindWithCache(productBvin);
            if (prod == null)
                return quantity;

            switch (prod.InventoryMode)
            {
                case ProductInventoryMode.AlwayInStock:
                    return quantity;
                case ProductInventoryMode.WhenOutOfStockAllowBackorders:
                    inv.QuantityReserved += quantity;
                    return quantity;
                case ProductInventoryMode.WhenOutOfStockShow:
                case ProductInventoryMode.WhenOutOfStockHide:
                    if (inv.QuantityAvailableForSale < quantity)
                    {
                        if (reserveZeroWhenQuantityTooLow)
                        {
                            return 0;
                        }

                        inv.QuantityReserved += inv.QuantityAvailableForSale;
                        return inv.QuantityAvailableForSale;
                    }
                    inv.QuantityReserved += quantity;
                    return quantity;
                default:
                    throw new NotImplementedException("InventoryMode is not implemented");
            }
        }

        #region Properties

        private const string productLinkFormat =
            "<a href=\"/DesktopModules/Hotcakes/Core/Admin/catalog/Products_Performance.aspx?id={1}\">{0}</a>";

        public bool EditMode { get; set; }
        public bool AllowUpdateQuantities { get; set; }
        public Order CurrentOrder { get; set; }
        public event EventHandler OrderEdited;
        private List<OrderTransaction> currentOrderTransactions { get; set; }
        private bool isPaymentRefunded { get; set; }
        private OrderPaymentSummary paySummary { get; set; }

        #endregion

        #region Public methods

        public void Rebind()
        {
            if (CurrentOrder != null)
            {
                currentOrderTransactions = HccApp.OrderServices.Transactions.FindForOrder(CurrentOrder.bvin);
                paySummary = HccApp.OrderServices.PaymentSummary(CurrentOrder);
                isPaymentRefunded = CheckOrderHasRefunded();
            }

            divUpdateQuantities.Visible = EditMode & AllowUpdateQuantities;

            lblSubTotal.Text = CurrentOrder.TotalOrderBeforeDiscounts.ToString("C");
            btnUpdateQuantities.Visible = CurrentOrder.Items.Any();

            gvItems.Visible = !CurrentOrder.IsRecurring;
            gvSubscriptions.Visible = CurrentOrder.IsRecurring;

            if (!CurrentOrder.IsRecurring)
            {
                gvItems.DataSource = CurrentOrder.Items;
                gvItems.DataBind();
            }
            else
            {
                gvSubscriptions.DataSource = CurrentOrder.Items;
                gvSubscriptions.DataBind();
            }
        }

        public bool UpdateQuantities()
        {
            var gridView = CurrentOrder.IsRecurring ? gvSubscriptions : gvItems;
            for (var i = 0; i < gridView.Rows.Count; i++)
            {
                var currentRow = gridView.Rows[i];
                if (currentRow.RowType == DataControlRowType.DataRow)
                {
                    var lineitemId = (long) gridView.DataKeys[currentRow.RowIndex].Value;
                    var lineItem = CurrentOrder.Items.FirstOrDefault(y => y.Id == lineitemId);
                    var li = CurrentOrder.Items.FirstOrDefault(y => y.Id == lineitemId);
                    if (li != null)
                    {
                        var txtQty = currentRow.FindControl("txtQty") as TextBox;
                        if (txtQty != null)
                        {
                            var tempQty = txtQty.Text.Trim();
                            var actualQty = li.Quantity;
                            if (int.Parse(tempQty) > actualQty && int.Parse(tempQty) - actualQty >= 0)
                            {
                                var qty = InventoryReserveQuantity(li.ProductId, li.VariantId,
                                    int.Parse(tempQty) - actualQty, true);
                                if (qty == 0)
                                {
                                    ucMessageBox.ShowError(
                                        string.Format("Item {0} did not have enough quantity to complete order.",
                                            li.ProductName));
                                    return false;
                                }
                                li.Quantity = int.Parse(tempQty) - actualQty;
                                li.QuantityReserved = li.Quantity;
                                HccApp.CatalogServices.InventoryLineItemReserveInventory(li);
                            }
                            if (int.Parse(tempQty) < actualQty && actualQty - int.Parse(tempQty) >= 0)
                            {
                                li.Quantity = actualQty - int.Parse(tempQty);
                                li.QuantityReserved = li.Quantity;
                                HccApp.CatalogServices.InventoryLineItemUnreserveInventory(li);
                            }

                            int.TryParse(tempQty, out actualQty);
                            li.Quantity = actualQty;
                        }
                    }
                }
            }
            return true;
        }

        #endregion

        #region Client Script Helper Methods

        private void RegisterOpenDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrScripts, phrScripts.GetType(), "jsOpenDialog", "openDialog();", true);
        }

        private void RegisterCloseDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrScripts, phrScripts.GetType(), "jsOpenDialog", "closeDialog();", true);
        }

        #endregion
    }
}