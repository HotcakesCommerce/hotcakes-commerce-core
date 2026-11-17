#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// <summary>
    /// Represents the <see cref="OrderTask"/> that performs the EmailProductIsOut workflow operation. The stable task identifier is <c>E2DB096A-2E8D-4757-A381-EA684375B4F2</c>. Execution and rollback preserve existing order workflow behavior, including persistence, <see cref="WorkflowMessage"/> errors, payment activity, email delivery, and rollback limitations.
    /// </summary>
    /// </summary>
    public class EmailProductIsOut : OrderTask
    {
        /// <summary>
        /// Sends the out-of-stock product email using the supplied application and order. Email failures are logged and are not returned as workflow failure.
        /// </summary>
        /// <param name="hccApp">The value used by this member.</param>
        /// <param name="order">The value used by this member.</param>
        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public void Execute(HotcakesApplication hccApp, Order order)
        {
            try
            {
                var toEmail = hccApp.CurrentStore.Settings.MailServer.EmailForGeneral;

                if (!string.IsNullOrEmpty(toEmail))
                {
                    var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
                    var defaultCulture = storeSettingsProvider.GetDefaultLocale();
                    var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(hccApp.CurrentRequestContext,
                        defaultCulture);
                    var contentService = Factory.CreateService<ContentService>(hccRequestContext);

                    var t = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.FreeProductIsOutOfStock);
                    t = t.ReplaceTagsInTemplate(hccRequestContext, order, order.ItemsAsReplaceable());

                    var m = t.ConvertToMailMessage(toEmail);
                    MailServices.SendMail(m, hccRequestContext.CurrentStore);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        /// <summary>
        /// Executes this order workflow task using the supplied <see cref="OrderTaskContext"/>. The return value controls whether the workflow continues; order changes, persistence, messages, payments, or email follow the existing implementation.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Execute(OrderTaskContext context)
        {
            try
            {
                if (context.Order.ApplyVATRules && !context.Order.IsRecurring)
                {
                    var toEmail = context.HccApp.CurrentStore.Settings.MailServer.EmailForGeneral;

                    if (!string.IsNullOrEmpty(toEmail))
                    {
                        var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
                        var defaultCulture = storeSettingsProvider.GetDefaultLocale();
                        var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context.RequestContext,
                            defaultCulture);
                        var contentService = Factory.CreateService<ContentService>(hccRequestContext);

                        var t = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.FreeProductIsOutOfStock);
                        t = t.ReplaceTagsInTemplate(hccRequestContext, context.Order, context.Order.ItemsAsReplaceable());

                        var m = t.ConvertToMailMessage(toEmail);
                        MailServices.SendMail(m, hccRequestContext.CurrentStore);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }

            return true;
        }

        /// <summary>
        /// Attempts to roll back this task using the supplied <see cref="OrderTaskContext"/>. A successful return does not imply that external persistence, payment processing, account creation, or email delivery was reversed.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets this task's stable workflow identifier.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskId()
        {
            return "E2DB096A-2E8D-4757-A381-EA684375B4F2";
        }

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string TaskName()
        {
            return "Email Product IS Out of Stock";
        }

        /// <summary>
        /// Gets the display name used for this workflow step.
        /// </summary>

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override string StepName()
        {
            return "Email Product IS Out of Stock";
        }

        /// <summary>
        /// Documents this public workflow member and preserves its existing behavior.
        /// </summary>
        public override Task Clone()
        {
            return new EmailProductIsOut();
        }
    }
}
