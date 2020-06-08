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
using System.Collections.Generic;
using System.Linq;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Controls;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    public partial class CreateOrder : BaseOrderPage
    {
        private const string ERRORFORMAT = "<span class=\"errormessage\">{0}</span>";
        private const string SUCCESSFORMAT = "<span class=\"successmessage\">{0}</span>";

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Orders;
            ValidateCurrentUserHasPermission(SystemPermissions.OrdersEdit);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            gridSelectUser.RowEditing += gridSelectUser_RowEditing;

            ucOrderItems.CurrentOrder = CurrentOrder;
            ucOrderItems.OrderEdited += ucOrderItems_OrderEdited;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Tag an id to the querystring to support back button
            if (string.IsNullOrEmpty(OrderId))
            {
                var order = new Order();
                HccApp.OrderServices.Orders.Create(order);
                Response.Redirect("CreateOrder.aspx?id=" + order.bvin);
            }

            if (!Page.IsPostBack)
            {
                if (!string.IsNullOrEmpty(ReturnUrl) && ReturnUrl == "Y")
                {
                    lnkBacktoAbandonedCartsReport.Visible = true;
                    lnkBacktoAbandonedCartsReport.NavigateUrl =
                        "~/DesktopModules/Hotcakes/Core/Admin/reports/AbandonedCarts/view.aspx";
                }
                else
                {
                    lnkBacktoAbandonedCartsReport.Visible = false;
                }

                LoadOrder();
                LoadPaymentMethods();
                LoadCurrentUser();

                CouponGrid.Attributes.Add("style", "word-break:break-all;word-wrap:break-word");
            }
        }

        private void ucOrderItems_OrderEdited(object sender, EventArgs e)
        {
            LoadShippingMethods();
            LoadPaymentMethods();
            LoadOrder();

            upMain.Update();
        }

        protected void chkBillToSame_CheckedChanged(object sender, EventArgs e)
        {
            if (chkBillToSame.Checked)
            {
                BillToAddress.LoadFromAddress(ShipToAddress.GetAsAddress());
                pnlBillTo.Visible = false;
            }
            else
            {
                pnlBillTo.Visible = true;
            }
            LoadShippingMethods();
        }

        private void LoadOrder()
        {
            //Items
            ucOrderItems.Rebind();

            litOrderSummary.Text = CurrentOrder.TotalsAsTable();
            txtInstructions.Text = CurrentOrder.Instructions;

            CouponGrid.DataSource = CurrentOrder.Coupons;
            CouponGrid.DataBind();
        }

        private void LoadCurrentUser()
        {
            UserIdField.Value = CurrentOrder.UserID;
            EmailAddressTextBox.Text = CurrentOrder.UserEmail;
            BillToAddress.LoadFromAddress(CurrentOrder.BillingAddress);
            ShipToAddress.LoadFromAddress(CurrentOrder.ShippingAddress);
        }

        private void LoadPaymentMethods()
        {
            // Hide all first
            var divs = new List<Control>
            {
                divNoPaymentNeeded,
                divCreditCard,
                divPurchaseOrder,
                divCompanyAccount,
                divCheck,
                divTelephone,
                divCashOnDelivery
            };
            divs.ForEach(d => d.Visible = false);

            // Show enabled only
            var enabledMethods = PaymentMethods.EnabledMethods(HccApp.CurrentStore, CurrentOrder.IsRecurring);
            foreach (var method in enabledMethods)
            {
                var divWrapper = upMain.FindControl("div" + method.MethodName) as HtmlGenericControl;
                var radioButton = upMain.FindControl("rb" + method.MethodName) as RadioButton;

                if (divWrapper != null)
                    divWrapper.Visible = true;
                if (radioButton != null)
                    radioButton.Text = LocalizationUtils.GetPaymentMethodFriendlyName(method.MethodName);
            }

            // Select first one
            var firstMethod = enabledMethods.FirstOrDefault();
            if (firstMethod != null)
            {
                var radioButton = upMain.FindControl("rb" + firstMethod.MethodName) as RadioButton;
                if (radioButton != null)
                    radioButton.Checked = true;
            }
        }

        private void SaveInfoToOrder(bool savePaymentData)
        {
            if (chkBillToSame.Checked)
            {
                BillToAddress.LoadFromAddress(ShipToAddress.GetAsAddress());
            }

            // Save Information to Cart in Case Save as Order Fails
            CurrentOrder.BillingAddress = BillToAddress.GetAsAddress();
            CurrentOrder.ShippingAddress = ShipToAddress.GetAsAddress();
            TagOrderWithUser();

            CurrentOrder.UserEmail = EmailAddressTextBox.Text;
            CurrentOrder.Instructions = txtInstructions.Text.Trim();

            // Save Shipping Selection
            var r = FindSelectedRate(ShippingRatesList.SelectedValue, CurrentOrder);
            if (r != null)
            {
                HccApp.OrderServices.OrdersRequestShippingMethodByUniqueKey(r.UniqueKey, CurrentOrder);
            }

            if (savePaymentData)
            {
                // Save Payment Information
                SavePaymentInfo();
            }

            HccApp.CalculateOrderAndSave(CurrentOrder);
        }

        protected void btnSubmit_Click(object sender, EventArgs e)
        {
            if (ValidateSelections())
            {
                SaveInfoToOrder(true);

                // Save as Order
                var c = new OrderTaskContext();
                c.UserId = string.Empty;
                // Use currently selected user for process new order context
                var u = GetSelectedUserAccount();
                if (u != null && !string.IsNullOrEmpty(u.Bvin))
                {
                    c.UserId = u.Bvin;
                    CurrentOrder.UserID = u.Bvin;
                }
                c.Order = CurrentOrder;

                var cardData = ucCreditCardInput.GetCardData();
                c.Inputs.Add("hcc", "CardSecurityCode", cardData.SecurityCode);

                if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrder))
                {
                    // Process Payment
                    if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrderPayments))
                    {
                        Workflow.RunByName(c, WorkflowNames.ProcessNewOrderAfterPayments);
                        var tempOrder = HccApp.OrderServices.Orders.FindForCurrentStore(CurrentOrder.bvin);
                        HccApp.CurrentRequestContext.IntegrationEvents.OrderReceived(tempOrder, HccApp);
                        Response.Redirect("ViewOrder.aspx?id=" + CurrentOrder.bvin);
                    }
                    else
                    {
                        ProcessWorkflowErrors(c);
                    }
                }
                else
                {
                    ProcessWorkflowErrors(c);
                }
            }
        }

        private void ProcessWorkflowErrors(OrderTaskContext orderContext)
        {
            // Show Errors
            var errors = orderContext.GetCustomerVisibleErrors();
            if (errors.Count > 0)
            {
                foreach (var item in orderContext.GetCustomerVisibleErrors())
                {
                    ucMessageBox.ShowWarning(item.Description);
                }
            }
            else
            {
                ucMessageBox.ShowWarning(Localization.GetString("OrderProcessErrors"));
            }
        }

        private bool ValidateSelections()
        {
            var result = true;

            if (string.IsNullOrEmpty(EmailAddressTextBox.Text))
            {
                ucMessageBox.ShowError(Localization.GetString("rfvEmailAddress"));
                result = false;
            }

            if (string.IsNullOrEmpty(ShippingRatesList.SelectedValue))
            {
                ucMessageBox.ShowError(Localization.GetString("rfvShippingRate"));
                result = false;
            }

            // Gift Card Check
            if (CurrentOrder.TotalGrandAfterStoreCredits(HccApp.OrderServices) <= 0)
            {
                return result;
            }

            var paymentFound = false;
            paymentFound |= rbCreditCard.Checked;
            paymentFound |= rbCheck.Checked;
            paymentFound |= rbTelephone.Checked;
            paymentFound |= rbCompanyAccount.Checked;
            paymentFound |= rbPurchaseOrder.Checked;
            paymentFound |= rbCashOnDelivery.Checked;

            if (rbCreditCard.Checked)
            {
                if (!ucCreditCardInput.IsValid())
                {
                    foreach (var item in ucCreditCardInput.GetRuleViolations())
                    {
                        ucMessageBox.ShowError(item.ErrorMessage);
                    }
                    result = false;
                }
            }

            if (!paymentFound)
            {
                ucMessageBox.ShowError(Localization.GetString("rfvPaymentMethod"));
                result = false;
            }

            return result;
        }

        private CustomerAccount GetSelectedUserAccount()
        {
            var result = new CustomerAccount();
            result = HccApp.MembershipServices.Customers.Find(UserIdField.Value);
            return result;
        }

        private void TagOrderWithUser()
        {
            TagOrderWithUser(GetSelectedUserAccount());
        }

        private void TagOrderWithUser(CustomerAccount account)
        {
            var u = account;
            if (u != null)
            {
                if (u.Bvin != string.Empty)
                {
                    UserIdField.Value = u.Bvin;
                    CurrentOrder.UserID = u.Bvin;
                    u.CheckIfNewAddressAndAddNoUpdate(BillToAddress.GetAsAddress());
                    u.CheckIfNewAddressAndAddNoUpdate(ShipToAddress.GetAsAddress());
                    HccApp.MembershipServices.Customers.Update(u);
                }
            }
        }

        private void SavePaymentInfo()
        {
            var payManager = new OrderPaymentManager(CurrentOrder, HccApp);
            payManager.ClearAllNonStoreCreditTransactions();

            // Don't add payment info if gift cards or points cover the entire order.
            var total = CurrentOrder.TotalGrandAfterStoreCredits(HccApp.OrderServices);
            if (total <= 0 && !CurrentOrder.IsRecurring)
            {
                return;
            }

            if (rbCreditCard.Checked)
            {
                var creditCardData = ucCreditCardInput.GetCardData();
                if (!CurrentOrder.IsRecurring)
                    payManager.CreditCardAddInfo(creditCardData, total);
                else
                    payManager.RecurringSubscriptionAddCardInfo(creditCardData);
            }
            else if (rbPurchaseOrder.Checked)
            {
                payManager.PurchaseOrderAddInfo(txtPurchaseOrder.Text.Trim(), total);
            }
            else if (rbCompanyAccount.Checked)
            {
                payManager.CompanyAccountAddInfo(txtAccountNumber.Text.Trim(), total);
            }
            else if (rbCheck.Checked)
            {
                payManager.OfflinePaymentAddInfo(total, Localization.GetFormattedString("CustomerPayByCheck"));
            }
            else if (rbTelephone.Checked)
            {
                payManager.OfflinePaymentAddInfo(total, Localization.GetString("CustomerPayByPhone"));
            }
            else if (rbCashOnDelivery.Checked)
            {
                payManager.OfflinePaymentAddInfo(total, Localization.GetString("CustomerPayCod"));
            }
        }

        private ShippingRateDisplay FindSelectedRate(string uniqueKey, Order o)
        {
            ShippingRateDisplay result = null;

            if (!o.HasShippingItems)
            {
                result = new ShippingRateDisplay
                {
                    DisplayName = GlobalLocalization.GetString("NoShippingRequired"),
                    ProviderId = string.Empty,
                    ProviderServiceCode = string.Empty,
                    Rate = 0m,
                    ShippingMethodId = "NOSHIPPING"
                };
            }
            else
            {
                result = HccApp.OrderServices.FindShippingRateByUniqueKey(uniqueKey, o);
            }

            return result;
        }

        protected void btnCalculateShipping_Click(object sender, EventArgs e)
        {
            SaveInfoToOrder(false);
            FillShippingMethods();
        }

        private void FillShippingMethods()
        {
            // Shipping Methods
            ShippingRatesList.Items.Clear();

            if (!CurrentOrder.HasShippingItems)
            {
                ShippingRatesList.Items.Add(new ListItem(GlobalLocalization.GetString("NoShippingRequired"),
                    "NOSHIPPING"));
            }
            else
            {
                var rates = HccApp.OrderServices.FindAvailableShippingRates(CurrentOrder);
                ShippingRatesList.DataTextField = "RateAndNameForDisplay";
                ShippingRatesList.DataValueField = "UniqueKey";
                ShippingRatesList.DataSource = rates;
                ShippingRatesList.DataBind();
            }
        }

        private void LoadShippingMethods()
        {
            var selectedIndex = ShippingRatesList.SelectedIndex;
            FillShippingMethods();
            if (ShippingRatesList.Items.Count > selectedIndex)
            {
                ShippingRatesList.SelectedIndex = selectedIndex;
            }
            if (ShippingRatesList.SelectedItem != null)
            {
                SaveInfoToOrder(false);
                LoadOrder();
            }
        }

        protected void btnAddCoupon_Click(object sender, EventArgs e)
        {
            Page.Validate("vgCoupon");
            if (Page.IsValid)
            {
                var couponResult = CurrentOrder.AddCouponCode(txtCoupon.Text.Trim());
                if (couponResult == false)
                {
                    ucMessageBox.ShowError(Localization.GetString("InvalidCoupon"));
                }
                else
                {
                    HccApp.OrderServices.Orders.Update(CurrentOrder);
                }
                LoadOrder();
            }
        }

        protected void CouponGrid_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var code = (string) CouponGrid.DataKeys[e.RowIndex].Value;
            if (code != string.Empty)
            {
                CurrentOrder.RemoveCouponCodeByCode(code);
                HccApp.OrderServices.Orders.Update(CurrentOrder);
            }
            LoadOrder();
        }

        protected void UserSelected(UserSelectedEventArgs args)
        {
            if (args.UserAccount == null) return;

            UserIdField.Value = args.UserAccount.Bvin;

            CurrentOrder.UserID = args.UserAccount.Bvin;

            HccApp.OrderServices.Orders.Update(CurrentOrder);

            BillToAddress.LoadFromAddress(args.UserAccount.BillingAddress);
            ShipToAddress.LoadFromAddress(args.UserAccount.ShippingAddress);
            if (BillToAddress.FirstName == string.Empty)
            {
                BillToAddress.FirstName = args.UserAccount.FirstName;
            }
            if (BillToAddress.LastName == string.Empty)
            {
                BillToAddress.LastName = args.UserAccount.LastName;
            }
            if (ShipToAddress.FirstName == string.Empty)
            {
                ShipToAddress.FirstName = args.UserAccount.FirstName;
            }
            if (ShipToAddress.LastName == string.Empty)
            {
                ShipToAddress.LastName = args.UserAccount.LastName;
            }

            EmailAddressTextBox.Text = args.UserAccount.Email;


            LoadShippingMethods();

            CurrentOrder.UserEmail = EmailAddressTextBox.Text;
            CurrentOrder.BillingAddress = BillToAddress.GetAsAddress();
            CurrentOrder.ShippingAddress = ShipToAddress.GetAsAddress();
            HccApp.OrderServices.Orders.Update(CurrentOrder);
        }

        protected void ShippingRatesList_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (ShippingRatesList.SelectedItem != null)
            {
                SaveInfoToOrder(false);
                LoadOrder();
            }
        }

        protected void btnUpdateShipping_Click(object sender, EventArgs e)
        {
            SaveInfoToOrder(false);
            LoadOrder();
        }

        protected void gridSelectUser_RowEditing(object sender, GridViewEditEventArgs e)
        {
            var bvin = gridSelectUser.DataKeys[e.NewEditIndex]["Bvin"].ToString();
            var u = HccApp.MembershipServices.Customers.Find(bvin);
            var args = new UserSelectedEventArgs();
            args.UserAccount = u;
            UserSelected(args);
        }

        protected void BindUserGrid()
        {
            lblNewUserMessage.Text = string.Empty;
            gridSelectUser.Visible = false;

            var totalCount = 0;
            var found = false;
            var users = HccApp.MembershipServices.Customers.FindByFilter(FilterUserField.Text.Trim(),
                gridSelectUser.PageIndex, gridSelectUser.PageSize, ref totalCount);
            if (users != null)
            {
                if (totalCount > 0)
                {
                    found = true;

                    if (totalCount == 1)
                    {
                        var message = string.Format(Localization.GetString("UserFound"), users[0].Email);
                        lblFindUserMessage.Text = string.Format(SUCCESSFORMAT, message);
                        var args = new UserSelectedEventArgs();
                        args.UserAccount = users[0];
                        UserSelected(args);
                    }
                    else
                    {
                        var message = string.Format(Localization.GetString("UsersFound"), totalCount);
                        lblFindUserMessage.Text = string.Format(SUCCESSFORMAT, message);
                        gridSelectUser.Visible = true;
                        gridSelectUser.VirtualItemCount = totalCount;
                        gridSelectUser.DataSource = users;
                    }
                }
            }

            if (!found)
            {
                lblFindUserMessage.Text = string.Format(ERRORFORMAT, Localization.GetString("NoUsers"));
            }
        }

        #region " Find User Code "

        protected void btnFindUsers_Click(object sender, EventArgs e)
        {
            viewFindUsers.ActiveViewIndex = 0;
        }

        protected void btnNewUsers_Click(object sender, EventArgs e)
        {
            viewFindUsers.ActiveViewIndex = 1;
        }

        protected void btnFindOrders_Click(object sender, EventArgs e)
        {
            viewFindUsers.ActiveViewIndex = 2;
        }

        protected void btnGoFindOrder_Click(object sender, EventArgs e)
        {
            lblFindOrderMessage.Text = string.Empty;

            var c = new OrderSearchCriteria();
            c.OrderNumber = FindOrderNumberField.Text.Trim();
            c.IsPlaced = true;
            c.StartDateUtc = DateTime.UtcNow.AddYears(-5);
            c.EndDateUtc = DateTime.UtcNow.AddYears(1);

            var found = false;

            var totalCount = 0;
            var results = HccApp.OrderServices.Orders.FindByCriteriaPaged(c, 1, 10, ref totalCount);
            if (results != null)
            {
                if (results.Count > 0)
                {
                    found = true;
                    var args = new UserSelectedEventArgs();
                    args.UserAccount = HccApp.MembershipServices.Customers.Find(results[0].UserID);
                    UserSelected(args);
                }
            }

            if (!found)
            {
                lblFindOrderMessage.Text = string.Format(ERRORFORMAT, Localization.GetString("NoOrder"));
            }
        }

        protected void btnNewUserSave_Click(object sender, EventArgs e)
        {
            lblNewUserMessage.Text = string.Empty;
            var u = new CustomerAccount();
            u.Email = NewUserEmailField.Text.Trim();
            u.FirstName = NewUserFirstNameField.Text.Trim();
            u.LastName = NewUserLastNameField.Text.Trim();
            var clearPassword = PasswordGenerator.GeneratePassword(12);

            if (HccApp.MembershipServices.CreateCustomer(u, clearPassword))
            {
                var args = new UserSelectedEventArgs();
                args.UserAccount = HccApp.MembershipServices.Customers.Find(u.Bvin);
                UserSelected(args);
            }
            else
            {
                lblNewUserMessage.Text = string.Format(ERRORFORMAT, Localization.GetString("CreateUserError"));
            }
        }

        protected void btnFindUser_Click(object sender, EventArgs e)
        {
            BindUserGrid();
        }

        #endregion
    }
}