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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class EmailOrder : OrderTask
    {
        private readonly string _emailTo = "Customer";

        public EmailOrder(string emailTo)
        {
            CustomEmail = string.Empty;
            _emailTo = emailTo;
        }

        public string CustomEmail { get; set; }

        public override bool Execute(OrderTaskContext context)
        {
            string toEmail = null;
            HtmlTemplate template = null;

            var culture = "en-US";
            switch (_emailTo)
            {
                case "Admin":
                    toEmail = context.HccApp.CurrentStore.Settings.MailServer.EmailForNewOrder;
                    var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
                    culture = storeSettingsProvider.GetDefaultLocale();
                    break;
                case "Customer":
                    toEmail = context.Order.UserEmail;
                    culture = context.Order.UsedCulture;
                    break;
            }

            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context.RequestContext, culture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);
            switch (_emailTo)
            {
                case "Admin":
                    template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.NewOrderForAdmin);
                    break;
                case "Customer":
                    template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.NewOrder);
                    break;
            }

            try
            {
                if (!string.IsNullOrEmpty(toEmail))
                {
                    template = template.ReplaceTagsInTemplateForOrder(hccRequestContext, context.Order);

                    var m = template.ConvertToMailMessage(toEmail);
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
            return "B1BAE947-3F33-473f-8AFE-F27A6B9625D3";
        }

        public override string TaskName()
        {
            return "Email Order";
        }

        public override string StepName()
        {
            var result = string.Empty;
            result = "Email Order";
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        public override Task Clone()
        {
            return new EmailOrder(_emailTo);
        }
    }
}