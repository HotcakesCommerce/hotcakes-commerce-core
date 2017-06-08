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
using System.Collections.ObjectModel;
using System.Linq;
using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    partial class ViewOrder : BaseOrderPage
    {
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            ucPaymentInformation.CurrentOrder = CurrentOrder;
            ucOrderStatusDisplay.CurrentOrder = CurrentOrder;

            ucOrderItems.CurrentOrder = CurrentOrder;
            ucOrderItems.OrderEdited += ucOrderItems_OrderEdited;
        }

        private void ucOrderItems_OrderEdited(object sender, EventArgs e)
        {
            ucOrderItems.Rebind();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                if (string.IsNullOrEmpty(OrderId))
                {
                    // Show Error
                }
                LoadTemplates();
                LoadOrder();
            }
        }

        private void LoadTemplates()
        {
            lstEmailTemplate.Items.Clear();
            var templates = HccApp.ContentServices.GetAllOrderTemplates();
            if (templates != null)
            {
                foreach (var t in templates)
                {
                    var li = new ListItem
                    {
                        Text = t.DisplayName,
                        Value = t.Id.ToString()
                    };
                    if (t.TemplateType == HtmlTemplateType.OrderUpdated)
                    {
                        lstEmailTemplate.ClearSelection();
                        li.Selected = true;
                    }
                    lstEmailTemplate.Items.Add(li);
                }
            }
        }

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = "View Order";
            CurrentTab = AdminTabType.Orders;
            ValidateCurrentUserHasPermission(SystemPermissions.OrdersView);
        }

        private void LoadOrder()
        {
            // Header
            OrderNumberField.Text = CurrentOrder.OrderNumber;
            TimeOfOrderField.Text =
                TimeZoneInfo.ConvertTimeFromUtc(CurrentOrder.TimeOfOrderUtc, HccApp.CurrentStore.Settings.TimeZone)
                    .ToString();

            // Fraud Score Display            
            if (CurrentOrder.FraudScore < 0) lblFraudScore.Text = "No Fraud Score Data";
            if (CurrentOrder.FraudScore >= 0 && CurrentOrder.FraudScore < 3)
                lblFraudScore.Text = CurrentOrder.FraudScore + "<span class=\"fraud-low\"><strong>low risk</strong></span>";
            if (CurrentOrder.FraudScore >= 3 && CurrentOrder.FraudScore <= 5)
                lblFraudScore.Text = "<span class=\"fraud-medium\"><strong>medium risk</strong></span>";
            if (CurrentOrder.FraudScore > 5) lblFraudScore.Text = "<span class=\"fraud-high\"><strong>high risk</strong></span>";

            // Billing
            lblBillingAddress.Text = CurrentOrder.BillingAddress.ToHtmlString();

            //Email
            ltEmailAddress.Text = MailServices.MailToLink(CurrentOrder.UserEmail, "Order " + CurrentOrder.OrderNumber,
                CurrentOrder.BillingAddress.FirstName + ",");

            // Shipping
            if (CurrentOrder.HasShippingItems)
                lblShippingAddress.Text = CurrentOrder.ShippingAddress.ToHtmlString();
            else
                lblShippingAddress.Text = "Non-shipping order";

            //Items
            ucOrderItems.Rebind();

            // Instructions
            if (!string.IsNullOrWhiteSpace(CurrentOrder.Instructions))
            {
                pnlInstructions.Visible = true;
                lblInstructions.Text =
                    CurrentOrder.Instructions.Replace("\r\n", "<br />").Replace("\r", "<br />").Replace("\n", "<br />");
            }


            // Totals
            litTotals.Text = CurrentOrder.TotalsAsTable(HccApp.CurrentRequestContext.MainContentCulture);

            // Coupons
            CouponField.Text = string.Empty;
            for (var i = 0; i <= CurrentOrder.Coupons.Count - 1; i++)
            {
                CouponField.Text += CurrentOrder.Coupons[i].CouponCode.Trim().ToUpper() + "<br />";
            }

            // Notes
            var publicNotes = new Collection<OrderNote>();
            var privateNotes = new Collection<OrderNote>();
            foreach (var note in CurrentOrder.Notes)
            {
                if (note.IsPublic)
                    publicNotes.Add(note);
                else
                    privateNotes.Add(note);
            }
            PublicNotesField.DataSource = publicNotes;
            PublicNotesField.DataBind();
            PrivateNotesField.DataSource = privateNotes;
            PrivateNotesField.DataBind();

            if (!InventoryControl.CheckCurrentOrderhasInventoryProduct())
            {
                btnDelete.OnClientClick += "return hcConfirm(event, 'Delete this order forever?');";
            }

            if (CurrentOrder.PaymentStatus != OrderPaymentStatus.Paid &&
                CurrentOrder.PaymentStatus != OrderPaymentStatus.PartiallyPaid ||
                CurrentOrder.PaymentStatus == OrderPaymentStatus.Overpaid)
            {
                btnDelete.OnClientClick += "return hcConfirm(event, 'Delete this order forever?');";
            }
        }

        protected void btnNewPublicNote_Click(object sender, EventArgs e)
        {
            if (NewPublicNoteField.Text.Trim().Length > 0)
            {
                AddNote(NewPublicNoteField.Text.Trim(), true);
            }
            NewPublicNoteField.Text = string.Empty;
        }

        private void AddNote(string message, bool isPublic)
        {
            var n = new OrderNote
            {
                OrderID = OrderId,
                IsPublic = isPublic,
                Note = message
            };
            CurrentOrder.Notes.Add(n);
            HccApp.OrderServices.Orders.Update(CurrentOrder);
            LoadOrder();
        }

        protected void btnNewPrivateNote_Click(object sender, EventArgs e)
        {
            if (NewPrivateNoteField.Text.Trim().Length > 0)
            {
                AddNote(NewPrivateNoteField.Text.Trim(), false);
            }
            NewPrivateNoteField.Text = string.Empty;
        }

        protected void PublicNotesField_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var Id = (long) PublicNotesField.DataKeys[e.RowIndex].Value;
            var n = CurrentOrder.Notes.SingleOrDefault(y => y.Id == Id);
            if (n != null)
            {
                CurrentOrder.Notes.Remove(n);
                HccApp.OrderServices.Orders.Update(CurrentOrder);
            }
            LoadOrder();
        }

        protected void PrivateNotesField_RowDeleting(object sender, GridViewDeleteEventArgs e)
        {
            var Id = (long) PrivateNotesField.DataKeys[e.RowIndex].Value;
            var n = CurrentOrder.Notes.SingleOrDefault(y => y.Id == Id);
            if (n != null)
            {
                CurrentOrder.Notes.Remove(n);
                HccApp.OrderServices.Orders.Update(CurrentOrder);
            }
            LoadOrder();
        }

        protected void btnSendStatusEmail_Click(object sender, EventArgs e)
        {
            long templateId = 0;
            long.TryParse(lstEmailTemplate.SelectedValue, out templateId);

            var t = HccApp.ContentServices.HtmlTemplates.Find(templateId);

            if (t == null) return;

            var toEmail = CurrentOrder.UserEmail;
            var culture = CurrentOrder.UsedCulture;

            if (t.TemplateType == HtmlTemplateType.NewOrderForAdmin ||
                t.TemplateType == HtmlTemplateType.DropShippingNotice ||
                t.TemplateType == HtmlTemplateType.ContactFormToAdmin)
            {
                toEmail = HccApp.CurrentStore.Settings.MailServer.EmailForNewOrder;

                var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
                culture = storeSettingsProvider.GetDefaultLocale();
            }

            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(HccRequestContext.Current, culture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);

            if (!string.IsNullOrWhiteSpace(toEmail))
            {
                // this time get template in correct culture
                t = contentService.HtmlTemplates.Find(templateId);
                t = t.ReplaceTagsInTemplateForOrder(hccRequestContext, CurrentOrder);
                var m = t.ConvertToMailMessage(toEmail);
                if (MailServices.SendMail(m, hccRequestContext.CurrentStore))
                {
                    ucMessageBox.ShowOk("E-Mail Sent!");
                }
                else
                {
                    ucMessageBox.ShowError("Message Send Failed.");
                }
            }

            LoadOrder();
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
        }


        protected void btnOK_Click(object sender, EventArgs e)
        {
            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/Orders/OrderPayments.aspx?id=" + CurrentOrder.bvin);
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            LoadInventory();
        }

        private void LoadInventory()
        {
            InventoryControl.BindGrids();
            RegisterOpenInventoryDialogScript();
        }

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
    }
}