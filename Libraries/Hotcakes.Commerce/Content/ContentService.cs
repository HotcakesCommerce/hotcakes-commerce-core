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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Content
{
    /// <summary>Provides store-scoped content operations and initializes the content repositories used by commerce services.</summary>
    public class ContentService : HccServiceBase
    {
        /// <summary>Initializes the service and creates the HTML-template, custom-URL, and content-column repositories for the context.</summary>
        public ContentService(HccRequestContext context)
            : base(context)
        {
            HtmlTemplates = Factory.CreateRepo<HtmlTemplateRepository>(Context);
            CustomUrls = Factory.CreateRepo<CustomUrlRepository>(Context);
            Columns = Factory.CreateRepo<ContentColumnRepository>(Context);
        }

        /// <summary>Gets the repository for store-scoped HTML templates and default templates.</summary>
        public HtmlTemplateRepository HtmlTemplates { get; protected set; }
        /// <summary>Gets the repository for store-scoped custom URLs.</summary>
        public CustomUrlRepository CustomUrls { get; protected set; }
        /// <summary>Gets the repository for store-scoped content columns.</summary>
        public ContentColumnRepository Columns { get; protected set; }

        /// <summary>Gets all supported templates for the current store, creating missing standard templates from store zero.</summary>
        /// <remarks>Order-update, email-friend, contact-form-to-admin, and return-form templates are excluded from the returned list.</remarks>
        public List<HtmlTemplate> GetAllTemplatesForStoreOrDefaults()
        {
            var templates = HtmlTemplates.FindAll();

            CheckForType(HtmlTemplateType.DropShippingNotice, templates);
            CheckForType(HtmlTemplateType.ForgotPassword, templates);
            CheckForType(HtmlTemplateType.NewOrder, templates);
            CheckForType(HtmlTemplateType.NewOrderForAdmin, templates);
            CheckForType(HtmlTemplateType.OrderShipment, templates);
            CheckForType(HtmlTemplateType.VATInvoice, templates);
            CheckForType(HtmlTemplateType.AffiliateRegistration, templates);
            CheckForType(HtmlTemplateType.AffiliateApprovement, templates);
            CheckForType(HtmlTemplateType.NewRoleAssignment, templates);
            CheckForType(HtmlTemplateType.GiftCardNotification, templates);
            CheckForType(HtmlTemplateType.AbandonedCart, templates);
            CheckForType(HtmlTemplateType.PaymentFailure, templates);
            CheckForType(HtmlTemplateType.RecurringPaymentSuccess, templates);
            CheckForType(HtmlTemplateType.RecurringPaymentFailed, templates);
            CheckForType(HtmlTemplateType.ContactAbandonedCartUsers, templates);
            CheckForType(HtmlTemplateType.AffiliateReview, templates);

            return templates.Where(t =>
                t.TemplateType != HtmlTemplateType.OrderUpdated &&
                t.TemplateType != HtmlTemplateType.EmailFriend &&
                t.TemplateType != HtmlTemplateType.ContactFormToAdmin &&
                t.TemplateType != HtmlTemplateType.ReturnForm
                ).OrderBy(t => t.TemplateType == HtmlTemplateType.Custom).ThenBy(t => t.DisplayName).ToList();
        }

        /// <summary>Gets templates usable for order notifications, including custom templates.</summary>
        public List<HtmlTemplate> GetAllOrderTemplates()
        {
            return GetAllTemplatesForStoreOrDefaults().Where(t =>
                t.TemplateType == HtmlTemplateType.NewOrderForAdmin ||
                t.TemplateType == HtmlTemplateType.NewOrder ||
                t.TemplateType == HtmlTemplateType.OrderShipment ||
                t.TemplateType == HtmlTemplateType.Custom)
                .ToList();
        }

        /// <summary>Gets the current-store template of the specified type, creating a store copy of the default when none exists.</summary>
        /// <param name="templateType">The template type to find or provision.</param>
        /// <returns>The existing or newly created template, or <see langword="null"/> when no default exists.</returns>
        public HtmlTemplate GetHtmlTemplateOrDefault(HtmlTemplateType templateType)
        {
            var existing = HtmlTemplates.FindByStoreAndType(Context.CurrentStore.Id, templateType);

            if (existing == null)
            {
                var standard = GetDefaultTemplate(templateType);

                if (standard != null)
                {
                    standard.Id = 0;
                    standard.StoreId = Context.CurrentStore.Id;
                    HtmlTemplates.Create(standard, true);
                }

                return standard;
            }

            return existing;
        }

        /// <summary>Replaces a template's editable content with the default content for its type.</summary>
        /// <param name="Template">The store template to update.</param>
        /// <returns>The default template used for the replacement, or <see langword="null"/> when no default exists.</returns>
        public HtmlTemplate UpdateTemplateToDefault(HtmlTemplate Template)
        {
            var standard = GetDefaultTemplate(Template.TemplateType);

            if (standard != null)
            {
                Template.From = standard.From;
                Template.DisplayName = standard.DisplayName;
                Template.Subject = standard.Subject;
                Template.Body = standard.Body;
                Template.RepeatingSection = standard.RepeatingSection;
                HtmlTemplates.Update(Template);
            }

            return standard;
        }

        /// <summary>Builds and sends a gift-card notification using the order culture and gift-card template.</summary>
        /// <param name="giftCard">The gift card supplied to replacement processing and recipient selection.</param>
        /// <param name="order">The order whose culture is used, when available.</param>
        /// <param name="item">The line item supplied to replacement processing.</param>
        public void SendGiftCardNotification(GiftCard giftCard, Order order, LineItem item)
        {
            var usedCulture = order != null ? order.UsedCulture : "en-US";
            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(Context, usedCulture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);
            var template = GetHtmlTemplateOrDefault(HtmlTemplateType.GiftCardNotification);
            template = template.ReplaceTagsInTemplate(hccRequestContext, new List<IReplaceable> {item, giftCard});
            var message = template.ConvertToMailMessage(giftCard.RecipientEmail);
            MailServices.SendMail(message, hccRequestContext.CurrentStore);
        }

        #region Implementation

        private void CheckForType(HtmlTemplateType checkType, List<HtmlTemplate> existing)
        {
            var existCount = existing.Where(y => y.TemplateType == checkType).Count();
            if (existCount < 1)
            {
                var standard = GetDefaultTemplate(checkType);
                if (standard != null)
                {
                    standard.Id = 0;
                    standard.StoreId = Context.CurrentStore.Id;
                    HtmlTemplates.Create(standard, true);
                    existing.Add(standard);
                }
            }
        }

        private HtmlTemplate GetDefaultTemplate(HtmlTemplateType templateType)
        {
            return HtmlTemplates.FindByStoreAndType(0, templateType);
        }

        #endregion
    }
}
