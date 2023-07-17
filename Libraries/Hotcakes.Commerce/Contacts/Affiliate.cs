#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Content;
using Hotcakes.CommerceDTO.v1.Contacts;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    ///     This is the main object that is used for all affiliate management in the main application
    /// </summary>
    /// <remarks>The main REST API equivalent to this is the AffiliateDTO object</remarks>
    [Serializable]
    public class Affiliate : IReplaceable
    {
        public Affiliate()
        {
            Id = 0;
            StoreId = 0;
            Enabled = false;
            AffiliateId = string.Empty;
            Username = string.Empty;
            Address = new Address();
            CommissionAmount = 0;
            CommissionType = AffiliateCommissionType.PercentageCommission;
            ReferralDays = 30;
            TaxId = string.Empty;
            DriversLicenseNumber = string.Empty;
            WebSiteUrl = string.Empty;
            Notes = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            CreationDate = DateTime.UtcNow;
        }

        /// <summary>
        ///     The unique ID for the affiliate
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     The unique ID to identify and work with the affiliate in the CMS
        /// </summary>
        /// <remarks>Used for other things like security role assignment, and loyalty point credits.</remarks>
        public int UserId { get; set; }

        /// <summary>
        ///     This is the name of the affiliate that is used to map back to the CMS username
        /// </summary>
        /// <remarks>This maps to the DisplayName property in the AffiliateDTO object</remarks>
        public string Username { get; set; }

        /// <summary>
        ///     The password of the affiliate, used only when creating a new affiliate during registration
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The creation date is only used for auditing purposes to know when the affiliate was created
        /// </summary>
        public DateTime CreationDate { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the affiliate was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     Used to specify if the affiliate is allowed to receive credit for referred purchases and promotions
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        ///     If true, this affiliate is enabled for use on the store. If disabled, all promotions and other affiliate activity
        ///     will not apply.
        /// </summary>
        /// <remarks>This is not able to be set in the UI</remarks>
        public bool Enabled { get; set; }

        /// <summary>
        ///     A unique value used to refer customers to the store for affiliate credit
        /// </summary>
        /// <remarks>This maps to the ReferralId in the AffiliateDTO object</remarks>
        public string AffiliateId { get; set; }

        /// <summary>
        ///     A unique value to identify an affiliate that referred the current affiliate to the store
        /// </summary>
        public string ReferralAffiliateId { get; set; }

        /// <summary>
        ///     Primary email address used to contact the affiliate
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     The primary or business address for the affiliate
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        ///     The amount to be used to calculate commissions on referred store purchases
        /// </summary>
        public decimal CommissionAmount { get; set; }

        /// <summary>
        ///     Reflects how the CommissionAmount should be calculated when applying referred purchase commissions
        /// </summary>
        /// <remarks>Possible values include: None, Percentage, and Flat Rate</remarks>
        public AffiliateCommissionType CommissionType { get; set; }

        /// <summary>
        ///     Reflects the number of days that an affiliate will "own" the rights to commissions on purchases made by a referred
        ///     customer
        /// </summary>
        public int ReferralDays { get; set; }

        /// <summary>
        ///     The ID that the affiliate uses to report taxes
        /// </summary>
        public string TaxId { get; set; }

        /// <summary>
        ///     An optional field that allows for more specific reporting on affiliates and also allows for compliance with law in
        ///     some regions
        /// </summary>
        public string DriversLicenseNumber { get; set; }

        /// <summary>
        ///     The URL of the website of the affiliate
        /// </summary>
        public string WebSiteUrl { get; set; }

        /// <summary>
        ///     Contains private notes about the affiliate. Only store merchants will see them.
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        ///     Allows you to return a listing of token key/value pairs that can be replaced in areas such as the email template
        /// </summary>
        /// <param name="context">The context.</param>
        /// <returns>
        ///     Currently returning an empty list
        /// </returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var store = context.CurrentStore;
            var culture = context.MainContentCulture;
            var result = new List<HtmlTemplateTag>();
            result.Add(new HtmlTemplateTag("[[Affiliate.Approve.Url]]",
                string.Format("{0}DesktopModules/Hotcakes/Core/Admin/people/Affiliates_Edit.aspx?id={1}",
                    store.RootUrl(), Id)));
            result.Add(new HtmlTemplateTag("[[Affiliate.AffiliateName]]", Username));
            return result;
        }

        /// <summary>
        ///     Returns the referral URL that can be used by the affiliate to refer customers
        /// </summary>
        /// <param name="currentStore">Instance of the current store</param>
        /// <returns></returns>
        public string GetDefaultLink(Store currentStore)
        {
            var result = "";
            result = currentStore.RootUrl();
            result += "?" + WebAppSettings.AffiliateQueryStringName + "=" + AffiliateId;
            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current affiliate object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of AffiliateDTO</returns>
        public AffiliateDTO ToDto()
        {
            var dto = new AffiliateDTO();

            dto.Id = Id;
            dto.StoreId = StoreId;
            dto.Enabled = Enabled;
            dto.ReferralId = AffiliateId;
            dto.DisplayName = Username;
            dto.Address = Address.ToDto();
            dto.CommissionAmount = CommissionAmount;
            dto.CommissionType = (AffiliateCommissionTypeDTO) (int) CommissionType;
            dto.ReferralDays = ReferralDays;
            dto.TaxId = TaxId;
            dto.DriversLicenseNumber = DriversLicenseNumber;
            dto.WebSiteUrl = WebSiteUrl;
            dto.Notes = Notes;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current affiliate object using an AffiliateDTO instance
        /// </summary>
        /// <param name="dto">An instance of the affiliate from the REST API</param>
        public void FromDto(AffiliateDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            UserId = (int) dto.Id; // missing after the Affiliate updates
            StoreId = dto.StoreId;
            Enabled = dto.Enabled;
            AffiliateId = dto.ReferralId;
            Username = dto.DisplayName;
            Address.FromDto(dto.Address);
            CommissionAmount = dto.CommissionAmount;
            CommissionType = (AffiliateCommissionType) (int) dto.CommissionType;
            ReferralDays = dto.ReferralDays;
            TaxId = dto.TaxId;
            DriversLicenseNumber = dto.DriversLicenseNumber;
            WebSiteUrl = dto.WebSiteUrl;
            Notes = dto.Notes;
            LastUpdatedUtc = dto.LastUpdatedUtc;
        }

        #endregion
    }
}