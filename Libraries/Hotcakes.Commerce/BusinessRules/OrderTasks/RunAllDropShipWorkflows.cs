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

using System.Collections.Generic;
using System.Collections.ObjectModel;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class RunAllDropShipWorkflows : OrderTask
    {
        public override Task Clone()
        {
            return new RunAllDropShipWorkflows();
        }

        public override bool Execute(OrderTaskContext context)
        {
            var manufacturers = new Collection<string>();
            var vendors = new Collection<string>();
            foreach (var item in context.Order.Items)
            {
                if (item.IsBundle)
                    continue;
                if (item.ShipFromMode == ShippingMode.ShipFromManufacturer)
                {
                    if (!string.IsNullOrEmpty(item.ShipFromNotificationId))
                    {
                        if (!manufacturers.Contains(item.ShipFromNotificationId))
                        {
                            manufacturers.Add(item.ShipFromNotificationId);
                        }
                    }
                    else
                    {
                        EventLog.LogEvent("RunAllDropShipWorkflows",
                            "Item with sku " + item.ProductSku +
                            " is marked as Ship From Manufacturer, but contains no Manufacturer Id",
                            EventLogSeverity.Warning);
                    }
                }
                else if (item.ShipFromMode == ShippingMode.ShipFromVendor)
                {
                    if (!string.IsNullOrEmpty(item.ShipFromNotificationId))
                    {
                        if (!vendors.Contains(item.ShipFromNotificationId))
                        {
                            vendors.Add(item.ShipFromNotificationId);
                        }
                    }
                    else
                    {
                        EventLog.LogEvent("RunAllDropShipWorkflows",
                            "Item with sku " + item.ProductSku +
                            " is marked as Ship From Vendor, but contains no Vendor Id", EventLogSeverity.Warning);
                    }
                }
            }

            foreach (var item in manufacturers)
            {
                var manufacturer = context.HccApp.ContactServices.Manufacturers.Find(item);
                ProcessVendorManufacturer(context, manufacturer);
            }

            foreach (var item in vendors)
            {
                var vendor = context.HccApp.ContactServices.Vendors.Find(item);
                ProcessVendorManufacturer(context, vendor);
            }

            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "995E482F-8BDC-47E2-926F-D7248553035F";
        }

        public override string TaskName()
        {
            return "Run All Dropship Notifications";
        }

        private void ProcessVendorManufacturer(OrderTaskContext context, VendorManufacturer vendorOrManufacturer)
        {
            if (vendorOrManufacturer != null)
            {
                var n = new OrderNote();
                n.IsPublic = false;

                if (!SendEmail(context, vendorOrManufacturer, context.Order))
                    n.Note = "Drop shipper notices for " + vendorOrManufacturer.DisplayName +
                             " were not able to send correctly.";
                else
                    n.Note = "Drop shipper notices for " + vendorOrManufacturer.DisplayName + " were sent successfully.";

                context.Order.Notes.Add(n);
                context.HccApp.OrderServices.Orders.Upsert(context.Order);
            }
        }

        private bool SendEmail(OrderTaskContext context, VendorManufacturer vendorOrManufacturer, Order order)
        {
            var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
            var defaultCulture = storeSettingsProvider.GetDefaultLocale();
            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context.RequestContext, defaultCulture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);

            var toEmail = vendorOrManufacturer.EmailAddress;

            HtmlTemplate t = null;
            var templateBvin = vendorOrManufacturer.DropShipEmailTemplateId;
            if (templateBvin != string.Empty)
            {
                long templateId = 0;
                long.TryParse(templateBvin, out templateId);
                t = contentService.HtmlTemplates.Find(templateId);
            }
            if (t == null)
            {
                t = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.DropShippingNotice);
            }

            if (!string.IsNullOrEmpty(toEmail))
            {
                var replacers = new List<IReplaceable>();
                replacers.Add(order);
                replacers.Add(vendorOrManufacturer);
                t = t.ReplaceTagsInTemplate(hccRequestContext, replacers, order.ItemsAsReplaceable());

                var m = t.ConvertToMailMessage(toEmail);
                return MailServices.SendMail(m, hccRequestContext.CurrentStore);
            }
            return false;
        }
    }
}