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
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core
{
    public class AbandonedCartEmailJob : SchedulerClient
    {
        public AbandonedCartEmailJob(ScheduleHistoryItem item)
        {
            ScheduleHistoryItem = item;
        }

        public override void DoWork()
        {
            try
            {
                Progressing();

                var context = new HccRequestContext();
                var accountServices = Factory.CreateService<AccountService>(context);
                var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

                foreach (var store in stores)
                {
                    context.CurrentStore = store;

                    SendAbandonedCartEmails(context);
                }

                //Show success
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                Errored(ref ex);
                Exceptions.LogException(ex);
            }
        }

        private void SendAbandonedCartEmails(HccRequestContext context)
        {
            if (context.CurrentStore.Settings.SendAbandonedCartEmails)
            {
                var orderService = Factory.CreateService<OrderService>(context);
                var orders = orderService.Orders.FindAbandonedCarts();
                foreach (var order in orders)
                {
                    SendAbandonedCartEmail(context, order);
                }
            }
        }

        private void SendAbandonedCartEmail(HccRequestContext context, Order order)
        {
            try
            {
                var membershipServices = Factory.CreateService<MembershipServices>(context);
                var customer = membershipServices.Customers.Find(order.UserID);

                if (customer != null && !string.IsNullOrEmpty(customer.Email))
                {
                    var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context, order.UsedCulture);
                    var contentService = Factory.CreateService<ContentService>(hccRequestContext);
                    var template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.AbandonedCart);
                    template = template.ReplaceTagsInTemplate(hccRequestContext, order, order.ItemsAsReplaceable());

                    var mailMessage = template.ConvertToMailMessage(customer.Email);
                    MailServices.SendMail(mailMessage, hccRequestContext.CurrentStore);

                    order.IsAbandonedEmailSent = true;
                    var orderService = Factory.CreateService<OrderService>(hccRequestContext);
                    orderService.Orders.Update(order);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }
    }
}