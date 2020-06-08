#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Globalization;
using System.Threading;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Controls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class EditOrder : BaseOrderPage
    {
        private const string CONFIRMFORMAT = "return hcConfirm(event, '{0}');";

        private void LoadInventory()
        {
            InventoryControl.BindGrids();
            RegisterOpenInventoryDialogScript();
        }

        private void InitializeAddresses()
        {
            ucBillingAddress.RequireCompany = false;
            ucBillingAddress.RequireFirstName = true;
            ucBillingAddress.RequireLastName = true;
            ucBillingAddress.RequirePhone = false;
            ucBillingAddress.ShowCompanyName = true;
            ucBillingAddress.ShowPhoneNumber = true;
            ucBillingAddress.ShowCounty = true;

            ucShippingAddress.RequireCompany = false;
            ucShippingAddress.RequireFirstName = true;
            ucShippingAddress.RequireLastName = true;
            ucShippingAddress.RequirePhone = false;
            ucShippingAddress.ShowCompanyName = true;
            ucShippingAddress.ShowPhoneNumber = true;
            ucShippingAddress.ShowCounty = true;
        }

        private void LoadOrder()
        {
            // Header
            OrderNumberField.Text = CurrentOrder.OrderNumber;
            TimeOfOrderField.Text =
                TimeZoneInfo.ConvertTimeFromUtc(CurrentOrder.TimeOfOrderUtc, HccApp.CurrentStore.Settings.TimeZone)
                    .ToString();

            // Billing
            ucBillingAddress.LoadFromAddress(CurrentOrder.BillingAddress);

            UserIdField.Value = CurrentOrder.UserID;
            // Email            
            ucUserPicker.UserName = CurrentOrder.UserEmail;

            // Shipping (hide if order is non-shipping)
            divShipToAddress.Visible = CurrentOrder.HasShippingItems;
            if (CurrentOrder.HasShippingItems)
                ucShippingAddress.LoadFromAddress(CurrentOrder.ShippingAddress);
            else
                lblShippingAddress.Text = Localization.GetString("lblNonShippingOrder");

            // Items
            ucOrderItems.Rebind();

            txtInstructions.Text = CurrentOrder.Instructions;

            // Totals
            litTotals.Text = CurrentOrder.TotalsAsTable();

            if (CurrentOrder.TotalShippingBeforeDiscountsOverride >= 0)
                ShippingOverride.Text = CurrentOrder.TotalShippingBeforeDiscountsOverride.ToString("C");
            else
                ShippingOverride.Text = string.Empty;

            // Coupons
            lstCoupons.DataSource = CurrentOrder.Coupons;
            lstCoupons.DataBind();

            LoadShippingMethods(CurrentOrder);
        }

        private void RunOrderEditedWorkflow()
        {
            var c = new OrderTaskContext();
            c.Order = CurrentOrder;
            c.UserId = c.Order.UserID;

            if (!Workflow.RunByName(c, WorkflowNames.OrderEdited))
            {
                foreach (var msg in c.Errors)
                {
                    EventLog.LogEvent("Order Edited Workflow", msg.Description, EventLogSeverity.Warning);
                }
                ucMessageBox.ShowError(Localization.GetString("lblErrorOrderSave"));
            }
            else
            {
                ucMessageBox.ShowOk(Localization.GetString("lblSaveSuccess"));
            }
        }

        private bool SaveOrder()
        {
            CurrentOrder.UserID = UserIdField.Value;
            CurrentOrder.UserEmail = ucUserPicker.UserName;
            CurrentOrder.Instructions = txtInstructions.Text.Trim();

            var newBillingAddress = ucBillingAddress.GetAsAddress();
            var isBillingAddressChanged = !CurrentOrder.BillingAddress.IsEqualTo(newBillingAddress);
            CurrentOrder.BillingAddress = newBillingAddress;
            if (CurrentOrder.HasShippingItems)
                CurrentOrder.ShippingAddress = ucShippingAddress.GetAsAddress();

            if (!ucOrderItems.UpdateQuantities())
            {
                return false;
            }

            // Save Shipping Selection
            var shippingRateKey = Request.Form["shippingrate"];
            HccApp.OrderServices.OrdersRequestShippingMethodByUniqueKey(shippingRateKey, CurrentOrder);

            // Shipping Override
            if (string.IsNullOrWhiteSpace(ShippingOverride.Text))
            {
                CurrentOrder.TotalShippingBeforeDiscountsOverride = -1m;
            }
            else
            {
                var shipOverride = CurrentOrder.TotalShippingBeforeDiscountsOverride;
                decimal.TryParse(ShippingOverride.Text, NumberStyles.Currency, Thread.CurrentThread.CurrentUICulture,
                    out shipOverride);
                CurrentOrder.TotalShippingBeforeDiscountsOverride = Money.RoundCurrency(shipOverride);
            }

            HccApp.CalculateOrder(CurrentOrder);
            HccApp.OrderServices.EvaluatePaymentStatus(CurrentOrder);
            var success = HccApp.OrderServices.Orders.Upsert(CurrentOrder);

            if (CurrentOrder.IsRecurring && isBillingAddressChanged)
            {
                var allSucceeded = true;
                var paymentManager = new OrderPaymentManager(CurrentOrder, HccApp);
                foreach (var li in CurrentOrder.Items)
                {
                    if (!li.RecurringBilling.IsCancelled)
                    {
                        var result = paymentManager.RecurringSubscriptionUpdate(li.Id, null);

                        allSucceeded &= result.Succeeded;

                        if (!result.Succeeded)
                        {
                            ucMessageBox.ShowError(string.Format(Localization.GetString("lblSubscriptionUpdateFall"), li.ProductName));
                        }
                    }
                }
            }

            if (success)
                RunOrderEditedWorkflow();

            return true;
        }

        private void LoadShippingMethods(Order o)
        {
            var rates = new SortableCollection<ShippingRateDisplay>();

            if (!o.HasShippingItems)
            {
                var r = new ShippingRateDisplay
                {
                    DisplayName = GlobalLocalization.GetString("NoShippingRequired"),
                    ProviderId = string.Empty,
                    ProviderServiceCode = string.Empty,
                    Rate = 0,
                    ShippingMethodId = "NOSHIPPING"
                };
                rates.Add(r);
            }
            else
            {
                // Shipping Methods
                rates = HccApp.OrderServices.FindAvailableShippingRates(o);

                if (rates.Count < 1)
                {
                    var result = new ShippingRateDisplay();
                    result.DisplayName = GlobalLocalization.GetString("ToBeDetermined");
                    result.ShippingMethodId = "TOBEDETERMINED";
                    result.Rate = 0;
                    rates.Add(result);
                }
            }

            litShippingMethods.Text = HtmlRendering.ShippingRatesToRadioButtons(rates, 300, o.ShippingMethodUniqueKey);
        }

        private void BindCoupons()
        {
            // Coupons
            lstCoupons.DataSource = CurrentOrder.Coupons;
            lstCoupons.DataBind();
        }

        #region Event handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Orders;
            ValidateCurrentUserHasPermission(SystemPermissions.OrdersView);
            ViewState["IsMultiSelect"] = false;
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitializeAddresses();

            ucPaymentInformation.CurrentOrder = CurrentOrder;
            ucOrderStatusDisplay.CurrentOrder = CurrentOrder;

            ucUserPicker.MessageBox = ucMessageBox;
            ucUserPicker.UserSelected += ucUserPicker_UserSelected;

            ucOrderItems.CurrentOrder = CurrentOrder;
            ucOrderItems.EditMode = !CurrentOrder.IsRecurring;
            ucOrderItems.OrderEdited += ucOrderItems_OrderEdited;
        }

        public StateBag ReturnViewState()
        {
            return ViewState;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(OrderId))
                {
                    // Show Error
                    ucMessageBox.ShowWarning(Localization.GetString("OrderNotFound"));
                }
                LoadOrder();

                if (!InventoryControl.CheckCurrentOrderhasInventoryProduct())
                {
                    btnDelete.OnClientClick += string.Format(CONFIRMFORMAT, Localization.GetString("ConfirmDeleteOrder"));
                }

                if (CurrentOrder.PaymentStatus != OrderPaymentStatus.Paid &&
                    CurrentOrder.PaymentStatus != OrderPaymentStatus.PartiallyPaid ||
                    CurrentOrder.PaymentStatus == OrderPaymentStatus.Overpaid)
                {
                    btnDelete.OnClientClick += string.Format(CONFIRMFORMAT, Localization.GetString("ConfirmDeleteOrder"));
                }
            }
        }

        private void ucOrderItems_OrderEdited(object sender, EventArgs e)
        {
            RunOrderEditedWorkflow();
            LoadOrder();

            upMain.Update();
            upPaymentInfo.Update();
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Page.IsValid)
            {
                if (SaveOrder())
                {
                    LoadOrder();
                    upPaymentInfo.Update();
                }
            }
        }

        private void ucUserPicker_UserSelected(object sender, UserSelectedEventArgs e)
        {
            UserIdField.Value = e.UserAccount.Bvin;
        }

        protected void btnDeleteCoupon_Click(object sender, EventArgs e)
        {
            foreach (ListItem li in lstCoupons.Items)
            {
                if (li.Selected)
                {
                    CurrentOrder.RemoveCouponCodeByCode(li.Text);
                }
            }
            HccApp.OrderServices.Orders.Update(CurrentOrder);
            BindCoupons();
        }

        protected void btnAddCoupon_Click(object sender, EventArgs e)
        {
            Page.Validate("vgCoupon");
            if (Page.IsValid)
            {
                CurrentOrder.AddCouponCode(txtCoupon.Text.Trim().ToUpper());
                HccApp.OrderServices.Orders.Update(CurrentOrder);
                BindCoupons();
            }
        }

        protected void btnDelete_Click(object sender, EventArgs e)
        {
            if (InventoryControl.CheckOrderHasRefunded())
            {
                if (InventoryControl.CheckCurrentOrderhasInventoryProduct())
                {
                    LoadInventory();
                }
                else
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
                            ucMessageBox.ShowWarning(Localization.GetString("PartialShipWarning"));
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
            else
            {
                if (CurrentOrder.PaymentStatus == OrderPaymentStatus.Paid ||
                    CurrentOrder.PaymentStatus == OrderPaymentStatus.PartiallyPaid ||
                    CurrentOrder.PaymentStatus == OrderPaymentStatus.Overpaid)
                {
                    RegisterOpenRefundDialogScript();
                }
                else
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
                            ucMessageBox.ShowWarning(Localization.GetString("PartialShipWarning"));
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
        }


        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Orders/OrderPayments.aspx?id=" + CurrentOrder.bvin);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LoadInventory();
        }

        #endregion

        #region Client Script

        private void RegisterOpenInventoryDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrInventoryScripts, phrInventoryScripts.GetType(), "jsOpenDialog",
                "openInventoryEditorDialog();", true);
        }

        private void RegisterCloseInventoryDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrInventoryScripts, phrInventoryScripts.GetType(), "jsOpenDialog",
                "closeInventoryEditorDialog();", true);
        }

        private void RegisterOpenRefundDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrRefundAmountScript, phrRefundAmountScript.GetType(), "jsOpenDialog",
                "openRefundAmountDialog();", true);
        }

        private void RegisterCloseRefundDialogScript()
        {
            ScriptManager.RegisterStartupScript(phrRefundAmountScript, phrRefundAmountScript.GetType(), "jsOpenDialog",
                "closeRefundAmountDialog();", true);
        }

        #endregion

        #region Edit Enventory

        #endregion
    }
}