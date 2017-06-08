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
using System.Collections.Generic;
using System.Net.Mail;
using System.Text;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Content
{
    public class HtmlTemplate : ILocalizableModel
    {
        public HtmlTemplate()
        {
            Id = 0;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            From = string.Empty;
            DisplayName = string.Empty;
            Subject = string.Empty;
            Body = string.Empty;
            RepeatingSection = string.Empty;
            TemplateType = HtmlTemplateType.Custom;
        }

        public long Id { get; set; }
        public long StoreId { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string DisplayName { get; set; }
        public string From { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
        public string RepeatingSection { get; set; }
        public HtmlTemplateType TemplateType { get; set; }
        public string ContentCulture { get; set; }

        public HtmlTemplate Clone()
        {
            var t = new HtmlTemplate();
            t.Body = Body;
            t.DisplayName = DisplayName;
            t.From = From;
            t.LastUpdatedUtc = LastUpdatedUtc;
            t.RepeatingSection = RepeatingSection;
            t.StoreId = StoreId;
            t.Subject = Subject;
            t.TemplateType = HtmlTemplateType.Custom;
            t.ContentCulture = ContentCulture;
            return t;
        }

        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, IReplaceable item)
        {
            var items = new List<IReplaceable>();
            items.Add(item);
            return ReplaceTagsInTemplate(context, items);
        }

        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, IReplaceable item,
            List<IReplaceable> repeatingItems)
        {
            var items = new List<IReplaceable>();
            items.Add(item);
            return ReplaceTagsInTemplate(context, items, repeatingItems);
        }

        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, List<IReplaceable> items)
        {
            return ReplaceTagsInTemplate(context, items, new List<IReplaceable>());
        }

        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, List<IReplaceable> items,
            List<IReplaceable> repeatingItems)
        {
            var copy = Clone();

            // Replace Store Defaults
            foreach (var tag in DefaultReplacementTags(context))
            {
                copy.Subject = tag.ReplaceTags(copy.Subject);
                copy.Body = tag.ReplaceTags(copy.Body);
                copy.From = tag.ReplaceTags(copy.From);
            }

            // Replace Tags in Body and Subject
            foreach (var item in items)
            {
                foreach (var tag in item.GetReplaceableTags(context))
                {
                    copy.Subject = tag.ReplaceTags(copy.Subject);
                    copy.Body = tag.ReplaceTags(copy.Body);
                    copy.From = tag.ReplaceTags(copy.From);
                }
            }

            // Build Repeating Section
            var sb = new StringBuilder();
            foreach (var repeatingItem in repeatingItems)
            {
                var temp = copy.RepeatingSection;
                foreach (var tag in repeatingItem.GetReplaceableTags(context))
                {
                    temp = tag.ReplaceTags(temp);
                }
                sb.Append(temp);
            }

            // Copy repeating section to body
            var allrepeating = sb.ToString();
            copy.Body = copy.Body.Replace("[[RepeatingSection]]", allrepeating);

            return copy;
        }

        public MailMessage ConvertToMailMessage(string toEmail)
        {
            try
            {
                var result = new MailMessage(From, toEmail);
                result.IsBodyHtml = true;
                result.Body = Body;
                result.Subject = Subject;
                return result;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public List<HtmlTemplateTag> DefaultReplacementTags(HccRequestContext context)
        {
            var tags = new List<HtmlTemplateTag>();

            var store = context.CurrentStore;
            var hccApp = new HotcakesApplication(context);
            var addressRepository = Factory.CreateRepo<AddressRepository>(context);
            tags.Add(new HtmlTemplateTag("[[Store.Address]]", addressRepository.FindStoreContactAddress().ToHtmlString()));
            tags.Add(new HtmlTemplateTag("[[Store.ContactEmail]]", store.Settings.MailServer.EmailForGeneral));
            tags.Add(new HtmlTemplateTag("[[Store.Logo]]", HtmlRendering.Logo(hccApp, false)));
            tags.Add(new HtmlTemplateTag("[[Store.SecureUrl]]", store.RootUrlSecure()));
            tags.Add(new HtmlTemplateTag("[[Store.StoreName]]", store.Settings.FriendlyName));
            tags.Add(new HtmlTemplateTag("[[Store.RawStoreName]]", store.StoreName));
            tags.Add(new HtmlTemplateTag("[[Store.StandardUrl]]", store.RootUrl()));
            tags.Add(new HtmlTemplateTag("[[Store.CurrentLocalTime]]", DateTime.Now.ToString()));
            tags.Add(new HtmlTemplateTag("[[Store.CurrentUtcTime]]", DateTime.UtcNow.ToString()));

            return tags;
        }

        public List<HtmlTemplateTag> GetEmptyReplacementTags(HccRequestContext context)
        {
            var tags = new List<HtmlTemplateTag>();

            tags.AddRange(DefaultReplacementTags(context));

            switch (TemplateType)
            {
                case HtmlTemplateType.NewOrderForAdmin:
                case HtmlTemplateType.AbandonedCart:
                case HtmlTemplateType.RecurringPaymentSuccess:
                case HtmlTemplateType.RecurringPaymentFailed:
                case HtmlTemplateType.NewOrder:
                case HtmlTemplateType.VATInvoice:
                case HtmlTemplateType.Custom:
                    tags.AddRange(new Order().GetReplaceableTags(context));
                    tags.AddRange(new LineItem().GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.OrderUpdated:
                    // Not used !!
                    break;
                case HtmlTemplateType.OrderShipment:
                    tags.AddRange(new Order().GetReplaceableTags(context));
                    tags.AddRange(new OrderPackage().GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.ForgotPassword:
                    tags.AddRange(new CustomerAccount().GetReplaceableTags(context));
                    tags.AddRange(new Replaceable("[[NewPassword]]", string.Empty).GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.EmailFriend:
                    // Not used !!
                    break;
                case HtmlTemplateType.ContactFormToAdmin:
                    // Not used !!
                    break;
                case HtmlTemplateType.DropShippingNotice:
                    tags.AddRange(new Order().GetReplaceableTags(context));
                    tags.AddRange(new VendorManufacturer().GetReplaceableTags(context));
                    tags.AddRange(new LineItem().GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.ReturnForm:
                    // Not used !!
                    break;
                case HtmlTemplateType.AffiliateRegistration:
                case HtmlTemplateType.AffiliateReview:
                    tags.AddRange(new Affiliate().GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.AffiliateApprovement:
                    tags.AddRange(new Affiliate().GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.NewRoleAssignment:
                    tags.AddRange(new CustomerAccount().GetReplaceableTags(context));
                    tags.AddRange(new Replaceable("[[User.NewRoles]]", string.Empty).GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.GiftCardNotification:
                    tags.AddRange(new LineItem().GetReplaceableTags(context));
                    tags.AddRange(new GiftCard().GetReplaceableTags(context));
                    break;
                case HtmlTemplateType.ContactAbandonedCartUsers:
                    tags.AddRange(new Replaceable("[[CustomMessage]]", string.Empty).GetReplaceableTags(context));
                    break;
                default:
                    break;
            }

            return tags;
        }

        public bool HasRepeatingSection()
        {
            switch (TemplateType)
            {
                case HtmlTemplateType.Custom:
                case HtmlTemplateType.NewOrder:
                case HtmlTemplateType.NewOrderForAdmin:
                case HtmlTemplateType.VATInvoice:
                case HtmlTemplateType.OrderUpdated:
                case HtmlTemplateType.OrderShipment:
                case HtmlTemplateType.DropShippingNotice:
                case HtmlTemplateType.AbandonedCart:
                case HtmlTemplateType.RecurringPaymentSuccess:
                case HtmlTemplateType.RecurringPaymentFailed:
                    return true;
                default:
                    break;
            }

            return false;
        }

        public HtmlTemplate ReplaceTagsInTemplateForOrder(HccRequestContext context, Order order)
        {
            if (TemplateType != HtmlTemplateType.OrderShipment)
            {
                return ReplaceTagsInTemplate(context, order, order.ItemsAsReplaceable());
            }
            return ReplaceTagsInTemplate(context, order, order.PackagesAsReplaceable());
        }

        #region Obsolete

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public List<HtmlTemplateTag> GetEmptyReplacementTags(HotcakesApplication app)
        {
            return GetEmptyReplacementTags(app.CurrentRequestContext);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public HtmlTemplate ReplaceTagsInTemplate(HotcakesApplication app, IReplaceable item)
        {
            return ReplaceTagsInTemplate(app.CurrentRequestContext, item);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public HtmlTemplate ReplaceTagsInTemplate(HotcakesApplication app, IReplaceable item,
            List<IReplaceable> repeatingItems)
        {
            return ReplaceTagsInTemplate(app.CurrentRequestContext, item, repeatingItems);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public HtmlTemplate ReplaceTagsInTemplate(HotcakesApplication app, List<IReplaceable> items)
        {
            return ReplaceTagsInTemplate(app.CurrentRequestContext, items, new List<IReplaceable>());
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public HtmlTemplate ReplaceTagsInTemplate(HotcakesApplication app, List<IReplaceable> items,
            List<IReplaceable> repeatingItems)
        {
            return ReplaceTagsInTemplate(app.CurrentRequestContext, items, repeatingItems);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public List<HtmlTemplateTag> DefaultReplacementTags(HotcakesApplication app)
        {
            return DefaultReplacementTags(app.CurrentRequestContext);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public HtmlTemplate ReplaceTagsInTemplateForOrder(HotcakesApplication app, Order order)
        {
            return ReplaceTagsInTemplateForOrder(app.CurrentRequestContext, order);
        }

        #endregion
    }
}