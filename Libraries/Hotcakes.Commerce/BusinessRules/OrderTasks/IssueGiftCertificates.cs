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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class IssueGiftCertificates : OrderTask
    {
        public override Task Clone()
        {
            return new IssueGiftCertificates();
        }

        public override bool Execute(OrderTaskContext context)
        {
            foreach (var item in context.Order.Items)
            {
                if (item.IsGiftCard)
                {
                    var product = item.GetAssociatedProduct(context.HccApp);

                    // assign if issue to line item additional props
                    var gcsAlreadyIssued = item.CustomProperties.GetPropertyAsInt("hcc", "giftCardsIssued");
                    var gcsToIssue = 0;
                    if (gcsAlreadyIssued < item.Quantity)
                    {
                        gcsToIssue = item.Quantity - gcsAlreadyIssued;
                    }

                    for (var i = 0; i < gcsToIssue; i++)
                    {
                        AddNewGiftCard(context, item);
                    }

                    item.CustomProperties.SetProperty("hcc", "giftCardsIssued", item.Quantity);
                }
            }

            return true;
        }

        /// <summary>
        ///     Adds the new gift card.
        /// </summary>
        /// <param name="context">The context.</param>
        /// <param name="lineItem">The line item.</param>
        private static void AddNewGiftCard(OrderTaskContext context, LineItem lineItem)
        {
            var paymentManager = new OrderPaymentManager(context.Order, context.HccApp);

            var store = context.HccApp.CurrentStore;
            var giftCard = new GiftCardData
            {
                LineItemId = lineItem.Id,
                CardNumber = context.HccApp.CatalogServices.GenerateGiftCardNumber(),
                Amount = Money.RoundCurrency(lineItem.AdjustedPricePerItem),
                RecipientEmail = lineItem.CustomPropGiftCardEmail,
                RecipientName = lineItem.CustomPropGiftCardName,
                ExpirationDate = DateTime.UtcNow.AddMonths(store.Settings.GiftCard.ExpirationPeriodMonths)
            };
            var gcNumber = paymentManager.GiftCardCreate(giftCard);

            if (string.IsNullOrEmpty(gcNumber))
            {
                var message = new WorkflowMessage(
                    "Gift Certificate Insert Failed",
                    "Gift Certificate for line item " + lineItem.Id + " in order " + context.Order.OrderNumber +
                    " failed.", false);
                context.Errors.Add(message);
            }
            else
            {
                if (string.IsNullOrEmpty(lineItem.CustomPropGiftCardNumber))
                {
                    lineItem.CustomPropGiftCardNumber = gcNumber;
                }
                else
                {
                    lineItem.CustomPropGiftCardNumber = lineItem.CustomPropGiftCardNumber + "<br/>" + gcNumber;
                }

                context.HccApp.OrderServices.Orders.Update(context.Order);

                var gc = new GiftCard(giftCard, lineItem.CustomPropGiftCardMessage);

                if (string.IsNullOrEmpty(gc.RecipientEmail))
                {
                    gc.RecipientEmail = context.Order.UserEmail;

                    if (string.IsNullOrEmpty(gc.RecipientName))
                    {
                        gc.RecipientName = context.Order.ShippingAddress.FirstName;
                    }
                }

                try
                {
                    context.HccApp.ContentServices.SendGiftCardNotification(gc, context.Order, lineItem);
                }
                catch (Exception ex)
                {
                    context.Errors.Add(new WorkflowMessage("Exception Sending Gift Card", ex.Message + ex.StackTrace,
                        false));
                    var note = new OrderNote();
                    note.IsPublic = false;
                    note.Note = "EXCEPTION: " + ex.Message + " | " + ex.StackTrace;
                    context.Order.Notes.Add(note);

                    EventLog.LogEvent(ex);
                }
            }
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "203bf29e-52e4-468a-8899-0498f1f48886";
        }

        public override string TaskName()
        {
            return "Issue Gift Certificates";
        }
    }
}