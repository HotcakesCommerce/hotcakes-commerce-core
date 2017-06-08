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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Contacts
{
    /// <summary>
    ///     This is the main object used for all affiliate referrals through the REST API.
    /// </summary>
    /// <remarks>This is the REST API equivalent of the AffiliateReferral object.</remarks>
    [DataContract]
    [Serializable]
    public class AffiliateReferralDTO
    {
        public AffiliateReferralDTO()
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
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The date/time stamp that the referral was made, used for auditing purposes only.
        /// </summary>
        [DataMember]
        public DateTime TimeOfReferralUtc { get; set; }

        /// <summary>
        ///     The unique ID of the affiliate to properly relate the referral to the right affiliate.
        /// </summary>
        [DataMember]
        public long AffiliateId { get; set; }

        /// <summary>
        ///     This value should contain the referring URL that the affiliate referral came from. It can occasionally be an empty
        ///     string.
        /// </summary>
        [DataMember]
        public string ReferrerUrl { get; set; }
    }
}