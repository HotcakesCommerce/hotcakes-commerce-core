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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Contacts
{
    /// <summary>
    ///     This is the main object that is used for all affiliate management in the REST API
    /// </summary>
    /// <remarks>The main application equivalent to this is the Affiliate object</remarks>
    [DataContract]
    [Serializable]
    public class AffiliateDTO
    {
        public AffiliateDTO()
        {
            Id = 0;
            StoreId = 0;
            Enabled = false;
            ReferralId = string.Empty;
            DisplayName = string.Empty;
            Address = new AddressDTO();
            CommissionAmount = 0;
            CommissionType = AffiliateCommissionTypeDTO.PercentageCommission;
            ReferralDays = 30;
            TaxId = string.Empty;
            DriversLicenseNumber = string.Empty;
            WebSiteUrl = string.Empty;
            CustomThemeName = string.Empty;
            Notes = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
#pragma warning disable 0612, 0618
            Contacts = new List<AffiliateContactDTO>();
#pragma warning restore 0612, 0618
        }

        /// <summary>
        ///     The unique ID for the affiliate
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the affiliate was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     If true, this affiliate is enabled for use on the store
        /// </summary>
        /// <remarks>This is not able to be set in the UI</remarks>
        [DataMember]
        public bool Enabled { get; set; }

        /// <summary>
        ///     A unique value used to refer customers to the store for affiliate credit.
        /// </summary>
        /// <remarks>This maps to the AffiliateId property in the Affiliate object and is used in URL's.</remarks>
        [DataMember]
        public string ReferralId { get; set; }

        /// <summary>
        ///     This is the name of the affiliate that is used to map back to the CMS username
        /// </summary>
        /// <remarks>This maps to the Username property in the Affiliate object</remarks>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        ///     The primary or business address for the affiliate
        /// </summary>
        [DataMember]
        public AddressDTO Address { get; set; }

        /// <summary>
        ///     The amount to be used to calculate commissions on referred store purchases
        /// </summary>
        [DataMember]
        public decimal CommissionAmount { get; set; }

        /// <summary>
        ///     Reflects how the CommissionAmount should be calculated when applying referred purchase commissions
        /// </summary>
        /// <remarks>Possible values include: None, Percentage, and Flat Rate</remarks>
        [DataMember]
        public AffiliateCommissionTypeDTO CommissionType { get; set; }

        /// <summary>
        ///     Reflects the number of days that an affiliate will "own" the rights to commissions on purchases made by a referred
        ///     customer
        /// </summary>
        [DataMember]
        public int ReferralDays { get; set; }

        /// <summary>
        ///     The ID that the affiliate uses to report taxes
        /// </summary>
        [DataMember]
        public string TaxId { get; set; }

        /// <summary>
        ///     An optional field that allows for more specific reporting on affiliates and also allows for compliance with law in
        ///     some regions
        /// </summary>
        [DataMember]
        public string DriversLicenseNumber { get; set; }

        /// <summary>
        ///     The URL of the website of the affiliate
        /// </summary>
        [DataMember]
        public string WebSiteUrl { get; set; }

        /// <summary>
        ///     This property is not used anywhere in the application, but could be used to cutomize views in your store
        /// </summary>
        [DataMember]
        public string CustomThemeName { get; set; }

        /// <summary>
        ///     Contains private notes about the affiliate. Only store merchants will see them.
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        ///     Contains a listing of the customers that currently assigned to the affiliate
        /// </summary>
        [DataMember]
        [Obsolete("Obsolete in 1.8.0. Affiliate contacts are not used")]
        public List<AffiliateContactDTO> Contacts { get; set; }
    }
}