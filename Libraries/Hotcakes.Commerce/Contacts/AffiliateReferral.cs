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

using System;
using Hotcakes.CommerceDTO.v1.Contacts;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    ///     This is the main object used for all affiliate referrals through the main application.
    /// </summary>
    /// <remarks>This is the application equivalent of the AffiliateReferralDTO REST API object.</remarks>
    [Serializable]
    public class AffiliateReferral
    {
        public AffiliateReferral()
        {
            Id = 0;
            StoreId = 0;
            TimeOfReferralUtc = DateTime.UtcNow;
            AffiliateId = 0;
            ReferrerUrl = string.Empty;
        }

        /// <summary>
        ///     The Unique ID of this affiliate referral instance used to find and update this item.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The date/time stamp that the referral was made, used for auditing purposes only.
        /// </summary>
        public DateTime TimeOfReferralUtc { get; set; }

        /// <summary>
        ///     The unique ID of the affiliate to properly relate the referral to the right affiliate.
        /// </summary>
        public long AffiliateId { get; set; }

        /// <summary>
        ///     This value should contain the referring URL that the affiliate referral came from. It can occasionally be an empty
        ///     string.
        /// </summary>
        public string ReferrerUrl { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current AffiliateReferral object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of AffiliateReferralDTO</returns>
        public AffiliateReferralDTO ToDto()
        {
            var dto = new AffiliateReferralDTO();

            dto.Id = Id;
            dto.StoreId = StoreId;
            dto.TimeOfReferralUtc = TimeOfReferralUtc;
            dto.AffiliateId = AffiliateId;
            dto.ReferrerUrl = ReferrerUrl;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current AffiliateReferral object using an AffiliateReferralDTO instance
        /// </summary>
        /// <param name="dto">An instance of the AffiliateReferral from the REST API</param>
        public void FromDto(AffiliateReferralDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            StoreId = dto.StoreId;
            TimeOfReferralUtc = dto.TimeOfReferralUtc;
            AffiliateId = dto.AffiliateId;
            ReferrerUrl = dto.ReferrerUrl;
        }

        #endregion
    }
}