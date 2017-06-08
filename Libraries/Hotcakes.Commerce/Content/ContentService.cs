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
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Content
{
    public class ContentService : HccServiceBase
    {
        public ContentService(HccRequestContext context)
            : base(context)
        {
            HtmlTemplates = Factory.CreateRepo<HtmlTemplateRepository>(Context);
            CustomUrls = Factory.CreateRepo<CustomUrlRepository>(Context);
            Columns = Factory.CreateRepo<ContentColumnRepository>(Context);
        }

        public HtmlTemplateRepository HtmlTemplates { get; protected set; }
        public CustomUrlRepository CustomUrls { get; protected set; }
        public ContentColumnRepository Columns { get; protected set; }

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

        public List<HtmlTemplate> GetAllOrderTemplates()
        {
            return GetAllTemplatesForStoreOrDefaults().Where(t =>
                t.TemplateType == HtmlTemplateType.NewOrderForAdmin ||
                t.TemplateType == HtmlTemplateType.NewOrder ||
                t.TemplateType == HtmlTemplateType.OrderShipment ||
                t.TemplateType == HtmlTemplateType.Custom)
                .ToList();
        }

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

        #region

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public static ContentService InstantiateForMemory(HccRequestContext c)
        {
            return Factory.CreateService<ContentService>();
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public static ContentService InstantiateForDatabase(HccRequestContext c)
        {
            return Factory.CreateService<ContentService>();
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public ContentService(HccRequestContext c,
            HtmlTemplateRepository templates,
            CustomUrlRepository customUrls,
            ContentColumnRepository cols)
            : this(c)
        {
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public void SendGiftCardNotification(GiftCard giftCard, LineItem item, HotcakesApplication app)
        {
            SendGiftCardNotification(giftCard, item);
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public void SendGiftCardNotification(GiftCard giftCard, LineItem item)
        {
            SendGiftCardNotification(giftCard, null, item);
        }

        #endregion
    }
}