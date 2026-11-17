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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    /// Provides contact-related services and repositories for the current commerce request.
    /// </summary>
    /// <remarks>
    /// Construction initializes repositories for affiliates, addresses, price groups, mailing lists,
    /// vendors, manufacturers, referrals, and affiliate payments using the supplied request context.
    /// </remarks>
    public abstract class ContactService : HccServiceBase
    {
        /// <summary>
        /// Initializes a contact service and its repositories for the specified request context.
        /// </summary>
        /// <param name="context">The request context that supplies the current store and service scope.</param>
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

        /// <summary>Gets the affiliate repository initialized for this request.</summary>
        public AffiliateRepository Affiliates { get; protected set; }
        /// <summary>Gets the affiliate-referral repository initialized for this request.</summary>
        public AffiliateReferralRepository AffiliateReferrals { get; protected set; }
        /// <summary>Gets the affiliate-payment repository initialized for this request.</summary>
        public AffiliatePaymentRepository AffiliatePayments { get; protected set; }
        /// <summary>Gets the address repository initialized for this request.</summary>
        public AddressRepository Addresses { get; protected set; }
        /// <summary>Gets the price-group repository initialized for this request.</summary>
        public PriceGroupRepository PriceGroups { get; protected set; }
        /// <summary>Gets the mailing-list repository initialized for this request.</summary>
        public MailingListRepository MailingLists { get; protected set; }
        /// <summary>Gets the vendor repository initialized for this request.</summary>
        public VendorRepository Vendors { get; protected set; }
        /// <summary>Gets the manufacturer repository initialized for this request.</summary>
        public ManufacturerRepository Manufacturers { get; protected set; }

        /// <summary>Updates the affiliate identifier associated with the current profile.</summary>
        /// <param name="affiliateId">The affiliate identifier to associate with the profile.</param>
        public abstract void UpdateProfileAffiliateId(long affiliateId);
        /// <summary>Stores the affiliate referral associated with the current request.</summary>
        /// <param name="affiliateId">The affiliate identifier supplied by the referral.</param>
        /// <param name="referralUrl">The URL from which the referral originated.</param>
        public abstract void SetAffiliateReferral(string affiliateId, string referralUrl);
        /// <summary>Gets the affiliate identifier currently associated with the request, if any.</summary>
        /// <returns>The current affiliate identifier, or <see langword="null"/> when none is available.</returns>
        public abstract long? GetCurrentAffiliateId();

        /// <summary>
        /// Applies affiliate promotions to an approved affiliate's customer account and sends the approval email.
        /// </summary>
        /// <param name="aff">The approved affiliate.</param>
        /// <param name="culture">The culture used for the approval template; the store default is used when empty.</param>
        public void AffiliateWasApproved(Affiliate aff, string culture = "")
        {
            var membershipServices = Factory.CreateService<MembershipServices>(Context);
            var marketingService = Factory.CreateService<MarketingService>(Context);
            var acc = membershipServices.Customers.Find(aff.UserId.ToString());
            marketingService.ApplyAffiliatePromotions(acc);
            SendAffiliateApprovementEmail(aff, culture);
        }

        /// <summary>
        /// Sends the new-role-assignment template to a customer after replacing the customer and role tags.
        /// </summary>
        /// <param name="acc">The customer account receiving the message.</param>
        /// <param name="roles">The roles assigned to the customer.</param>
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

        /// <summary>Sends the affiliate registration confirmation template to the affiliate.</summary>
        /// <param name="aff">The affiliate receiving the message.</param>
        /// <param name="defaultCulture">The template culture; the store default is used when empty.</param>
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

        /// <summary>Sends the affiliate review template to the store's new-order email address.</summary>
        /// <param name="aff">The affiliate represented in the review message.</param>
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

        /// <summary>Sends the affiliate approval template to the approved affiliate.</summary>
        /// <param name="aff">The affiliate receiving the message.</param>
        /// <param name="culture">The template culture; the store default is used when empty.</param>
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

        /// <summary>
        /// Records a referral only when the affiliate exists and is both enabled and approved.
        /// </summary>
        /// <param name="affiliateId">The affiliate identifier.</param>
        /// <param name="referrerUrl">The URL that referred the visitor.</param>
        /// <returns><see langword="true"/> when a referral was created; otherwise, <see langword="false"/>.</returns>
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
    }
}
