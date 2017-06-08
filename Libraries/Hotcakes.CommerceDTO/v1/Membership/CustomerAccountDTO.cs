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
using Hotcakes.CommerceDTO.v1.Contacts;

namespace Hotcakes.CommerceDTO.v1.Membership
{
    /// <summary>
    ///     This is the primary class used to manage customer accounts in the REST API. Many of these properties are duplicated
    ///     in the CMS.
    /// </summary>
    /// <remarks>The equivalent throughout the rest of the application is CustomerAccount.</remarks>
    [DataContract]
    [Serializable]
    public class CustomerAccountDTO
    {
        // Constructor			
        public CustomerAccountDTO()
        {
            Addresses = new List<AddressDTO>();
            Bvin = string.Empty;
            CreationDateUtc = DateTime.UtcNow;
            Email = string.Empty;
            FailedLoginCount = 0;
            FirstName = string.Empty;
            LastLoginDateUtc = DateTime.UtcNow;
            LastName = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            Notes = string.Empty;
            Password = string.Empty;
            PricingGroupId = string.Empty;
            //this.Salt = string.Empty;
            TaxExempt = false;
            ShippingAddress = new AddressDTO();
            BillingAddress = new AddressDTO();
        }

        /// <summary>
        ///     The unique ID of the customer account
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The primary email address for the customer and it is used to get their avatar from gravatar.
        /// </summary>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        ///     First name of the customer
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        ///     Last or surname of the customer
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        ///     Encrypted password of the customer
        /// </summary>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        ///     Addresses from the customer's address book
        /// </summary>
        [DataMember]
        public List<AddressDTO> Addresses { get; set; }

        /// <summary>
        ///     Defines whether the customer is exempt from paying taxes
        /// </summary>
        [DataMember]
        public bool TaxExempt { get; set; }

        /// <summary>
        ///     Notes about the customer
        /// </summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        ///     The unique ID (bvin) of the price group that this customer belongs to.
        /// </summary>
        /// <remarks>Enmpty string means that they are not part of any price group.</remarks>
        [DataMember]
        public string PricingGroupId { get; set; }

        /// <summary>
        ///     The number of times that the customer failed to login
        /// </summary>
        [DataMember]
        public int FailedLoginCount { get; set; }

        /// <summary>
        ///     The date/time stamp of the last time the customer was updated
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The date/time stamp of when the customer was created
        /// </summary>
        [DataMember]
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        ///     The date/time stamp of the last time the customer logged in
        /// </summary>
        [DataMember]
        public DateTime LastLoginDateUtc { get; set; }

        /// <summary>
        ///     Address details for the primary shipping address
        /// </summary>
        [DataMember]
        public AddressDTO ShippingAddress { get; set; }

        /// <summary>
        ///     Address details for billing the customer
        /// </summary>
        [DataMember]
        public AddressDTO BillingAddress { get; set; }
    }
}