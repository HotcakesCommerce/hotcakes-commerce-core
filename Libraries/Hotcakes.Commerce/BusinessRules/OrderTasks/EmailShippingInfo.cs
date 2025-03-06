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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class EmailShippingInfo : OrderTask
    {
        private readonly string _ToEmail = "Customer";

        public EmailShippingInfo(string toEmail)
        {
            _ToEmail = toEmail;
        }

        public EmailShippingInfo()
        {
        }

        public override bool Execute(OrderTaskContext context)
        {
            var EmailSelection = string.Empty;
            EmailSelection = _ToEmail;
            var toEmail = string.Empty;
            switch (EmailSelection)
            {
                case "Admin":
                    toEmail = context.HccApp.CurrentRequestContext.CurrentStore.Settings.MailServer.EmailForNewOrder;
                    break;
                case "Customer":
                    toEmail = context.Order.UserEmail;
                    break;
                default:
                    toEmail = context.Order.UserEmail;
                    EmailSelection = "Customer";
                    break;
            }

            try
            {
                if (toEmail.Trim().Length > 0)
                {
                    var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
                    var defaultCulture = context.Order.UsedCulture;
                    var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context.RequestContext,
                        defaultCulture);
                    var contentService = Factory.CreateService<ContentService>(hccRequestContext);
                    var t = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.OrderShipment);
                    t = t.ReplaceTagsInTemplateForOrder(hccRequestContext, context.Order);

                    var m = t.ConvertToMailMessage(toEmail);
                    MailServices.SendMail(m, hccRequestContext.CurrentStore);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }

            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "7817bfd9-1075-4bac-9189-3c76c3ec17a6";
        }

        public override string TaskName()
        {
            return "Email Shipping Info";
        }

        public override string StepName()
        {
            var result = string.Empty;
            result = "Send Shipping Info";
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        public override Task Clone()
        {
            return new EmailShippingInfo();
        }
    }
}