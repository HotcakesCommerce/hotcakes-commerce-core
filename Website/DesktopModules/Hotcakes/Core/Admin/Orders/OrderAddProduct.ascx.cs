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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class OrderAddProduct : HccUserControl
    {
        protected void btnCloseGiftCardDetails_Click(object sender, EventArgs e)
        {
            SwitchProductPicker(true);
            ClearGiftCardDetails();
        }

        protected void btnAddGiftCard_Click(object sender, EventArgs e)
        {
            Page.Validate("ValidationGiftCard");
            if (Page.IsValid)
            {
                AddProductBySku();
            }
        }

        protected void cvGiftCardAmount_ServerValidate(object sender, ServerValidateEventArgs e)
        {
            if (string.IsNullOrEmpty(lstAmount.SelectedValue))
            {
                if (string.IsNullOrEmpty(GiftCardAmount.Text))
                {
                    cvGiftCardAmount.ErrorMessage = Localization.GetString("GiftCardAmountRequired");
                    e.IsValid = false;
                    return;
                }

                int val;
                if (!int.TryParse(GiftCardAmount.Text, out val))
                {
                    cvGiftCardAmount.ErrorMessage = Localization.GetString("GiftCardAmountPositiveNumber");
                    e.IsValid = false;
                    return;
                }

                if (val <= 0)
                {
                    cvGiftCardAmount.ErrorMessage = Localization.GetString("GiftCardAmountPositiveNumber");
                    e.IsValid = false;
                    return;
                }

                if (!ValidateGiftCardAmount())
                {
                    e.IsValid = false;
                    return;
                }
            }
            e.IsValid = true;
        }

        private bool ValidateGiftCardAmount()
        {
            var valid = true;
            var gcSetting = HccApp.CurrentStore.Settings.GiftCard;

            var strAmount = GiftCardAmount.Text;
            decimal gcAmount = 0;
            if (decimal.TryParse(strAmount, out gcAmount))
            {
                decimal amount = 0;
                amount = Money.RoundCurrency(gcAmount);
                if (amount < gcSetting.MinAmount)
                {
                    valid = false;
                    cvGiftCardAmount.ErrorMessage = Localization.GetString("GiftCardAmountTooSmall")
                        .Replace("{0}", gcSetting.MinAmount.ToString());
                }
                if (amount > gcSetting.MaxAmount)
                {
                    valid = false;
                    cvGiftCardAmount.ErrorMessage = Localization.GetString("GiftCardAmountTooBig")
                        .Replace("{0}", gcSetting.MaxAmount.ToString());
                }
            }
            else
            {
                valid = false;
                cvGiftCardAmount.ErrorMessage = Localization.GetString("GiftCardAmountInvalid");
            }
            return valid;
        }

        #region Properties

        public Order CurrentOrder { get; set; }
        public event EventHandler ProductAdded;

        #endregion

        #region Event handlers

        protected void Page_Load(object sender, EventArgs e)
        {
            if (mvPanels.GetActiveView() == vwProductChoices)
            {
                var product = HccApp.CatalogServices.Products.Find(AddProductSkuHiddenField.Value);
                // Render Options
                RenderProductOptions(product);
            }

            if (!IsPostBack)
            {
                ClearGiftCardDetails();
            }
        }

        protected void btnBrowseProducts_Click(object sender, EventArgs e)
        {
            SwitchProductPicker(mvPanels.GetActiveView() != vwProductPicker);
        }

        protected void btnAddProductBySku_Click(object sender, EventArgs e)
        {
            Page.Validate("vgQuantity");
            if (Page.IsValid)
            {
                AddProductBySku();
            }
        }

        protected void btnProductPickerCancel_Click(object sender, EventArgs e)
        {
            SwitchProductPicker(false);
        }

        protected void btnProductPickerAddProduct_Click(object sender, EventArgs e)
        {
            Page.Validate("vgQuantity");
            if (Page.IsValid)
            {
                if (ucProductPicker.SelectedProducts.Count > 0)
                {
                    var p = HccApp.CatalogServices.Products.Find(ucProductPicker.SelectedProducts[0]);
                    if (p != null)
                    {
                        NewSkuField.Text = p.Sku;
                    }
                    AddProductBySku();
                }
                else
                {
                    ucMessageBox.ShowWarning("Please select a product first.");
                }
            }
        }

        protected void btnCloseVariants_Click(object sender, EventArgs e)
        {
            SwitchProductPicker(false);
        }

        protected void btnAddVariant_Click(object sender, EventArgs e)
        {
            SwitchProductPicker(false);

            var product = HccApp.CatalogServices.Products.Find(AddProductSkuHiddenField.Value);
            if (product != null && product.Sku.Trim().Length > 0)
            {
                var quantity = 1;
                if (!product.HideQty)
                {
                    int.TryParse(NewProductQuantity.Text, out quantity);
                }
                var selections = ParseSelections(product);
                if (ValidateSelections(product, selections))
                {
                    decimal? userPrice = null;

                    if (product.IsUserSuppliedPrice)
                    {
                        decimal up = 0;
                        if (decimal.TryParse(txtUserPrice.Text, out up))
                        {
                            userPrice = Money.RoundCurrency(up);
                        }
                        else
                        {
                            userPrice = null;
                        }
                    }

                    if (CurrentOrder != null)
                    {
                        var li = product.ConvertToLineItem(HccApp, quantity, selections, userPrice);

                        if (!CheckValidQty(CurrentOrder, li))
                        {
                            return;
                        }

                        HccApp.OrderServices.AddItemToOrder(CurrentOrder, li);

                        HccApp.CalculateOrder(CurrentOrder);
                        HccApp.OrderServices.EvaluatePaymentStatus(CurrentOrder);
                        HccApp.OrderServices.Orders.Upsert(CurrentOrder);

                        li.Quantity = quantity;
                        HccApp.CatalogServices.InventoryLineItemReserveInventory(li);

                        ucMessageBox.ShowOk("Product Added!");
                        AddProductSkuHiddenField.Value = string.Empty;

                        ProductAdded(this, EventArgs.Empty);
                    }
                }
                else
                {
                    litMessage.Text =
                        "That combination of choices/options is not available at the moment. Please select proper choices/options.";
                    if (product != null && product.Sku.Trim().Length > 0)
                    {
                        if (product.HasOptions() || product.IsUserSuppliedPrice || product.IsBundle)
                        {
                            NewSkuField.Enabled = false;
                            btnAddProductBySku.Visible = false;
                            mvPanels.SetActiveView(vwProductChoices);
                        }
                    }
                }
            }
        }

        #endregion

        #region Implementation

        private void SwitchProductPicker(bool showPicker)
        {
            if (showPicker)
            {
                mvPanels.SetActiveView(vwProductPicker);
                ucProductPicker.Keyword = string.Empty;
                if (!string.IsNullOrWhiteSpace(NewSkuField.Text))
                {
                    ucProductPicker.Keyword = NewSkuField.Text;
                }
                ucProductPicker.LoadSearch();
            }
            else
            {
                mvPanels.SetActiveView(vwEmpty);
            }

            NewSkuField.Enabled = !showPicker;
            btnAddProductBySku.Visible = !showPicker;
        }

        private void AddProductBySku()
        {
            SwitchProductPicker(false);

            if (string.IsNullOrWhiteSpace(NewSkuField.Text))
            {
                ucMessageBox.ShowWarning("Please enter a sku first.");
                return;
            }

            var p = HccApp.CatalogServices.Products.FindBySku(NewSkuField.Text.Trim());
            if (p != null && p.Sku.Length > 0)
            {
                if (CurrentOrder.Items.Count > 0 && p.IsRecurring != CurrentOrder.IsRecurring)
                {
                    ucMessageBox.ShowError(
                        "You can not mix recurring products with regular products within the same order");
                    return;
                }

                if (p.HasOptions() || p.IsUserSuppliedPrice || p.IsBundle)
                {
                    ShowProductOptions(p);
                }
                else
                {
                    if (CurrentOrder != null)
                    {
                        var quantity = 1;

                        if (!p.HideQty)
                        {
                            int.TryParse(NewProductQuantity.Text, out quantity);
                        }
                        var selections = new OptionSelections();
                        var li = p.ConvertToLineItem(HccApp, quantity, selections);

                        if (!CheckValidQty(CurrentOrder, li))
                        {
                            return;
                        }

                        if (li.IsGiftCard)
                        {
                            if (IsGiftCardView.Value.ToLower() != "true")
                            {
                                RenderGiftCardDetails(li);
                                IsGiftCardView.Value = "true";
                                btnAddProductBySku.Visible = false;
                                return;
                            }

                            li.CustomPropGiftCardEmail = GiftCardRecEmail.Text;
                            li.CustomPropGiftCardName = GiftCardRecName.Text;
                            li.CustomPropGiftCardMessage = GiftCardMessage.Text;

                            decimal gcAmount = 0;

                            if (string.IsNullOrEmpty(lstAmount.SelectedValue))
                            {
                                if (decimal.TryParse(GiftCardAmount.Text, out gcAmount))
                                {
                                    li.BasePricePerItem = Money.RoundCurrency(gcAmount);
                                }
                            }
                            else
                            {
                                if (decimal.TryParse(lstAmount.SelectedValue, out gcAmount))
                                {
                                    li.BasePricePerItem = Money.RoundCurrency(gcAmount);
                                }
                            }
                        }

                        foreach (var item in CurrentOrder.Items)
                        {
                            item.QuantityReserved = item.Quantity;
                        }

                        HccApp.OrderServices.AddItemToOrder(CurrentOrder, li);

                        HccApp.CalculateOrder(CurrentOrder);
                        HccApp.OrderServices.EvaluatePaymentStatus(CurrentOrder);
                        HccApp.OrderServices.Orders.Upsert(CurrentOrder);

                        // Update Inventory only.
                        li.Quantity = quantity;
                        HccApp.CatalogServices.InventoryLineItemReserveInventory(li);

                        ucMessageBox.ShowOk("Product Added!");

                        ProductAdded(this, EventArgs.Empty);

                        IsGiftCardView.Value = "false";
                        ClearGiftCardDetails();
                    }
                }
            }
            else
            {
                ucMessageBox.ShowWarning("That SKU could not be located. Please try again.");
            }
        }

        private void ClearGiftCardDetails()
        {
            IsGiftCardView.Value = "false";
            GiftCardRecEmail.Text = string.Empty;
            GiftCardRecName.Text = string.Empty;
            GiftCardMessage.Text = string.Empty;
            GiftCardAmount.Text = string.Empty;
            lstAmount.SelectedValue = string.Empty;
            btnAddProductBySku.Visible = true;
        }

        private void RenderGiftCardDetails(LineItem li)
        {
            lstAmount.Items.Clear();
            mvPanels.SetActiveView(vwGiftCard);
            //Set predefine amount
            var amounts = HccApp.CurrentStore.Settings.GiftCard.PredefinedAmounts.Split(',');
            lstAmount.Items.Add(new ListItem("Set my own", string.Empty));
            foreach (var amount in amounts)
            {
                decimal dec = 0;

                if (decimal.TryParse(amount, out dec))
                {
                    lstAmount.Items.Add(new ListItem(dec.ToString("c"), dec.ToString()));
                }
            }
        }

        private bool CheckValidQty(Order o, LineItem li)
        {
            var qtyToAdd = 1;
            var item =
                o.Items.FirstOrDefault(y => y.Id == li.Id && y.SelectionData.Equals(li.SelectionData) && y.VariantId == li.VariantId &&
                        y.ProductId == li.ProductId);
            if (item != null)
            {
                var tempQty = item.Quantity + li.Quantity;
                var actualQty = li.Quantity;
                if (tempQty > actualQty && tempQty - actualQty >= 0)
                {
                    qtyToAdd = tempQty - actualQty;
                }
            }
            else
            {
                qtyToAdd = li.Quantity;
            }

            var qty = InventoryReserveQuantity(li.ProductId, li.VariantId, qtyToAdd, true);
            if (qty == 0)
            {
                ucMessageBox.ShowError(string.Format("Item {0} did not have enough quantity to complete order.",
                    li.ProductName));
                return false;
            }

            return true;
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

        private void ShowProductOptions(Product p)
        {
            NewSkuField.Enabled = false;
            btnAddProductBySku.Visible = false;
            mvPanels.SetActiveView(vwProductChoices);

            AddProductSkuHiddenField.Value = p.Bvin;

            // Render Options
            RenderProductOptions(p);
        }

        private void RenderProductOptions(Product p)
        {
            litProductInfo.Text = p.ProductName;
            phChoices.Controls.Clear();
            HtmlRendering.ProductOptionsAsControls(p, phChoices);

            pnlUserPrice.Visible = p.IsUserSuppliedPrice;
            lblUserPrice.Text = p.UserSuppliedPriceLabel ?? "User Price";
        }

        private OptionSelections ParseSelections(Product product)
        {
            var result = new OptionSelections();

            if (!product.IsBundle)
            {
                foreach (var opt in product.Options)
                {
                    var selected = opt.ParseFromPlaceholder(phChoices);
                    if (selected != null)
                        result.OptionSelectionList.Add(selected);
                }
            }
            else
            {
                foreach (var bundledProductAdv in product.BundledProducts)
                {
                    var bundledProduct = bundledProductAdv.BundledProduct;
                    if (bundledProduct == null)
                        continue;
                    foreach (var opt in bundledProduct.Options)
                    {
                        var selected = opt.ParseFromPlaceholder(phChoices, bundledProductAdv.Id.ToString());
                        if (selected != null)
                            result.AddBundleSelections(bundledProductAdv.Id, selected);
                    }
                }
            }

            return result;
        }

        private bool ValidateSelections(Product p, OptionSelections selections)
        {
            var result = p.ValidateSelections(selections);
            return result == ValidateSelectionsResult.Success;
        }

        #endregion
    }
}