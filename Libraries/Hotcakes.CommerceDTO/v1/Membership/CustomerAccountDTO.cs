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
        /// <summary>
        /// Initializes a customer account with empty collections, addresses, and default account values.
        /// </summary>
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
        /// Gets or sets the unique identifier of the customer account.
        /// </summary>
        /// <value>The customer account identifier.</value>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        /// Gets or sets the customer's primary email address.
        /// </summary>
        /// <remarks>The address is also used to retrieve the customer's Gravatar.</remarks>
        /// <value>The primary email address.</value>
        [DataMember]
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the customer's first name.
        /// </summary>
        /// <value>The first name.</value>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        /// Gets or sets the customer's last name or surname.
        /// </summary>
        /// <value>The last name.</value>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        /// Gets or sets the customer's encrypted password value.
        /// </summary>
        /// <value>The encrypted password.</value>
        [DataMember]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the addresses in the customer's address book.
        /// </summary>
        /// <value>A list of <see cref="AddressDTO"/> instances.</value>
        [DataMember]
        public List<AddressDTO> Addresses { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the customer is exempt from paying taxes.
        /// </summary>
        /// <value><see langword="true"/> if the customer is tax exempt; otherwise, <see langword="false"/>.</value>
        [DataMember]
        public bool TaxExempt { get; set; }

        /// <summary>
        /// Gets or sets notes associated with the customer.
        /// </summary>
        /// <value>The customer notes.</value>
        [DataMember]
        public string Notes { get; set; }

        /// <summary>
        /// Gets or sets the unique identifier of the price group to which the customer belongs.
        /// </summary>
        /// <remarks>An empty string indicates that the customer is not part of a price group.</remarks>
        /// <value>The price group identifier.</value>
        [DataMember]
        public string PricingGroupId { get; set; }

        /// <summary>
        /// Gets or sets the number of failed customer login attempts.
        /// </summary>
        /// <value>The failed login count.</value>
        [DataMember]
        public int FailedLoginCount { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the customer was last updated.
        /// </summary>
        /// <value>The last-update timestamp in UTC.</value>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the customer account was created.
        /// </summary>
        /// <value>The account creation timestamp in UTC.</value>
        [DataMember]
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when the customer last logged in.
        /// </summary>
        /// <value>The last-login timestamp in UTC.</value>
        [DataMember]
        public DateTime LastLoginDateUtc { get; set; }

        /// <summary>
        /// Gets or sets the customer's primary shipping address.
        /// </summary>
        /// <value>An <see cref="AddressDTO"/> containing shipping address details.</value>
        [DataMember]
        public AddressDTO ShippingAddress { get; set; }

        /// <summary>
        /// Gets or sets the customer's billing address.
        /// </summary>
        /// <value>An <see cref="AddressDTO"/> containing billing address details.</value>
        [DataMember]
        public AddressDTO BillingAddress { get; set; }
    }
}
