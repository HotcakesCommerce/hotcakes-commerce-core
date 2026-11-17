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
    /// <summary>Represents a localized HTML message template and its optional repeating section.</summary>
    public class HtmlTemplate : ILocalizableModel
    {
        /// <summary>Initializes an empty custom template with zero identifiers and current UTC update time.</summary>
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

        /// <summary>Gets or sets the template identifier.</summary>
        public long Id { get; set; }
        /// <summary>Gets or sets the owning store identifier.</summary>
        public long StoreId { get; set; }
        /// <summary>Gets or sets the UTC time when the template was last updated.</summary>
        public DateTime LastUpdatedUtc { get; set; }
        /// <summary>Gets or sets the display name.</summary>
        public string DisplayName { get; set; }
        /// <summary>Gets or sets the sender address used when creating a message.</summary>
        public string From { get; set; }
        /// <summary>Gets or sets the message subject, including replacement tags.</summary>
        public string Subject { get; set; }
        /// <summary>Gets or sets the HTML message body, including replacement tags.</summary>
        public string Body { get; set; }
        /// <summary>Gets or sets the repeated HTML fragment inserted for repeating items.</summary>
        public string RepeatingSection { get; set; }
        /// <summary>Gets or sets the business type of this template.</summary>
        public HtmlTemplateType TemplateType { get; set; }
        /// <summary>Gets or sets the culture of the localized content.</summary>
        public string ContentCulture { get; set; }

        /// <summary>Creates a copy of this template's content and metadata without copying its identifier.</summary>
        /// <remarks>The clone's <see cref="TemplateType"/> is always <see cref="HtmlTemplateType.Custom"/>.</remarks>
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

        /// <summary>Returns a copy with store defaults and tags from one item replaced in the sender, subject, and body.</summary>
        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, IReplaceable item)
        {
            var items = new List<IReplaceable>();
            items.Add(item);
            return ReplaceTagsInTemplate(context, items);
        }

        /// <summary>Returns a copy with store defaults, one primary item, and repeating items applied.</summary>
        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, IReplaceable item,
            List<IReplaceable> repeatingItems)
        {
            var items = new List<IReplaceable>();
            items.Add(item);
            return ReplaceTagsInTemplate(context, items, repeatingItems);
        }

        /// <summary>Returns a copy with store defaults and tags from the supplied items replaced.</summary>
        public HtmlTemplate ReplaceTagsInTemplate(HccRequestContext context, List<IReplaceable> items)
        {
            return ReplaceTagsInTemplate(context, items, new List<IReplaceable>());
        }

        /// <summary>Returns a copy with store defaults, primary items, and repeated section items applied.</summary>
        /// <remarks>Each repeated item expands <c>RepeatingSection</c>; the concatenated result replaces <c>[[RepeatingSection]]</c> in the body.</remarks>
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

        /// <summary>Converts the template into an HTML <see cref="MailMessage"/> for the recipient.</summary>
        /// <remarks>If the mail message cannot be constructed, the implementation returns <see langword="null"/>.</remarks>
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

        /// <summary>Gets replacement tags derived from the current store, URLs, logo, and current local and UTC times.</summary>
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

        /// <summary>Gets default replacement tags plus empty-value tags appropriate to this template type.</summary>
        /// <remarks>Several retained template types intentionally provide no additional tags because they are not used.</remarks>
        public List<HtmlTemplateTag> GetEmptyReplacementTags(HccRequestContext context)
        {
            var tags = new List<HtmlTemplateTag>();

            tags.AddRange(DefaultReplacementTags(context));

            switch (TemplateType)
            {
                case HtmlTemplateType.NewOrderForAdmin:
                case HtmlTemplateType.AbandonedCart:
                case HtmlTemplateType.PaymentFailure:
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

        /// <summary>Determines whether this template type supports a repeating section.</summary>
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
                case HtmlTemplateType.PaymentFailure:
                case HtmlTemplateType.RecurringPaymentSuccess:
                case HtmlTemplateType.RecurringPaymentFailed:
                    return true;
                default:
                    break;
            }

            return false;
        }

        /// <summary>Returns an order template copy with order and item or package replacement tags applied.</summary>
        /// <remarks>Shipment templates repeat packages; all other order template types repeat order items.</remarks>
        public HtmlTemplate ReplaceTagsInTemplateForOrder(HccRequestContext context, Order order)
        {
            if (TemplateType != HtmlTemplateType.OrderShipment)
            {
                return ReplaceTagsInTemplate(context, order, order.ItemsAsReplaceable());
            }
            return ReplaceTagsInTemplate(context, order, order.PackagesAsReplaceable());
        }
    }
}
