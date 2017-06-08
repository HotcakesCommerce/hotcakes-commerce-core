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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Contacts
{
    public abstract class ContactService : HccServiceBase
    {
        public ContactService(HccRequestContext context)
            : base(context)
        {
            Affiliates = Factory.CreateRepo<AffiliateRepository>(Context);
            Addresses = Factory.CreateRepo<AddressRepository>(Context);
            PriceGroups = Factory.CreateRepo<PriceGroupRepository>(Context);
            MailingLists = Factory.CreateRepo<MailingListRepository>(Context);
            Vendors = Factory.CreateRepo<VendorRepository>(Context);
            Manufacturers = Factory.CreateRepo<ManufacturerRepository>(Context);
            AffiliateReferrals = Factory.CreateRepo<AffiliateReferralRepository>(Context);
            AffiliatePayments = Factory.CreateRepo<AffiliatePaymentRepository>(Context);
        }

        public AffiliateRepository Affiliates { get; protected set; }
        public AffiliateReferralRepository AffiliateReferrals { get; protected set; }
        public AffiliatePaymentRepository AffiliatePayments { get; protected set; }
        public AddressRepository Addresses { get; protected set; }
        public PriceGroupRepository PriceGroups { get; protected set; }
        public MailingListRepository MailingLists { get; protected set; }
        public VendorRepository Vendors { get; protected set; }
        public ManufacturerRepository Manufacturers { get; protected set; }

        public abstract void UpdateProfileAffiliateId(long affiliateId);
        public abstract void SetAffiliateReferral(string affiliateId, string referralUrl);
        public abstract long? GetCurrentAffiliateId();

        public void AffiliateWasApproved(Affiliate aff, string culture = "")
        {
            var membershipServices = Factory.CreateService<MembershipServices>(Context);
            var marketingService = Factory.CreateService<MarketingService>(Context);
            var acc = membershipServices.Customers.Find(aff.UserId.ToString());
            marketingService.ApplyAffiliatePromotions(acc);
            SendAffiliateApprovementEmail(aff, culture);
        }

        public void SendNewRolesAssignment(CustomerAccount acc, string[] roles)
        {
            var roleTags = new Replaceable("[[User.NewRoles]]", string.Join(", ", roles));

            var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
            var defaultCulture = storeSettingsProvider.GetDefaultLocale();
            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(Context, defaultCulture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);
            var template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.NewRoleAssignment);
            template = template.ReplaceTagsInTemplate(hccRequestContext, new List<IReplaceable> {acc, roleTags});

            var message = template.ConvertToMailMessage(acc.Email);
            MailServices.SendMail(message, hccRequestContext.CurrentStore);
        }

        public void SendAffiliateConfirmationEmail(Affiliate aff, string defaultCulture = "")
        {
            var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
            if (string.IsNullOrEmpty(defaultCulture))
            {
                defaultCulture = storeSettingsProvider.GetDefaultLocale();
            }
            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(Context, defaultCulture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);
            var template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.AffiliateRegistration);
            template = template.ReplaceTagsInTemplate(hccRequestContext, aff);
            var message = template.ConvertToMailMessage(aff.Email);
            MailServices.SendMail(message, hccRequestContext.CurrentStore);
        }

        public void SendAffiliateReviewEmail(Affiliate aff)
        {
            var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
            var defaultCulture = storeSettingsProvider.GetDefaultLocale();
            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(Context, defaultCulture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);
            var template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.AffiliateReview);
            template = template.ReplaceTagsInTemplate(hccRequestContext, aff);
            var storeAdmin = hccRequestContext.CurrentStore.Settings.MailServer.EmailForNewOrder;
            var message = template.ConvertToMailMessage(storeAdmin);
            MailServices.SendMail(message, hccRequestContext.CurrentStore);
        }

        public void SendAffiliateApprovementEmail(Affiliate aff, string culture = "")
        {
            var storeSettingsProvider = Factory.CreateStoreSettingsProvider();
            if (string.IsNullOrEmpty(culture))
            {
                culture = storeSettingsProvider.GetDefaultLocale();
            }
            var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(Context, culture);
            var contentService = Factory.CreateService<ContentService>(hccRequestContext);
            var template = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.AffiliateApprovement);
            template = template.ReplaceTagsInTemplate(hccRequestContext, aff);
            var message = template.ConvertToMailMessage(aff.Email);
            MailServices.SendMail(message, hccRequestContext.CurrentStore);
        }

        #region Implementation

        protected bool LogReferral(long affiliateId, string referrerUrl)
        {
            var aff = Affiliates.Find(affiliateId);

            if (aff != null && aff.Enabled && aff.Approved)
            {
                var r = new AffiliateReferral();
                r.AffiliateId = aff.Id;
                r.ReferrerUrl = referrerUrl;
                return AffiliateReferrals.Create(r);
            }

            return false;
        }

        #endregion

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public ContactService(HccRequestContext context, bool isForMemoryOnly)
            : this(context)
        {
        }

        [Obsolete("Obsolete in 2.0.0. Use same method with other parameters instead")]
        public void AffiliateWasApproved(Affiliate aff, HotcakesApplication app)
        {
            AffiliateWasApproved(aff);
        }

        [Obsolete("Obsolete in 2.0.0. Use instance method with other parameters instead")]
        public static void SendNewRolesAssignment(CustomerAccount acc, string[] roles, HotcakesApplication app)
        {
            var contactService = Factory.CreateService<ContactService>(app.CurrentRequestContext);
            contactService.SendNewRolesAssignment(acc, roles);
        }

        [Obsolete("Obsolete in 2.0.0. Use instance method with other parameters instead")]
        public static void SendAffiliateConfirmationEmail(Affiliate aff, HotcakesApplication app)
        {
            var contactService = Factory.CreateService<ContactService>(app.CurrentRequestContext);
            contactService.SendAffiliateConfirmationEmail(aff);
        }

        [Obsolete("Obsolete in 2.0.0. Use instance method with other parameters instead")]
        public static void SendAffiliateApprovementEmail(Affiliate aff, HotcakesApplication app)
        {
            var contactService = Factory.CreateService<ContactService>(app.CurrentRequestContext);
            contactService.SendAffiliateApprovementEmail(aff);
        }

        #endregion
    }
}