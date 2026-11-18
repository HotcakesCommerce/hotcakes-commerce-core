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
using Hotcakes.Web.Geography;

namespace Hotcakes.Payment
{
    /// <summary>
    /// Represents billing and shipping contact information supplied for a payment transaction.
    /// </summary>
    /// <remarks>Billing values use the unprefixed properties; shipping values use the <c>Ship</c>-prefixed properties.</remarks>
    [Serializable]
    public class CustomerData
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomerData"/> class with empty contact values and a default IP address.
        /// </summary>
        public CustomerData()
        {
            UserId = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Company = string.Empty;
            Street = string.Empty;
            City = string.Empty;
            RegionName = string.Empty;
            RegionBvin = string.Empty;
            PostalCode = string.Empty;
            CountryName = string.Empty;
            CountryBvin = string.Empty;
            Phone = string.Empty;
            Email = string.Empty;
            IpAddress = "0.0.0.0";

            ShipFirstName = string.Empty;
            ShipLastName = string.Empty;
            ShipCompany = string.Empty;
            ShipStreet = string.Empty;
            ShipCity = string.Empty;
            ShipRegionName = string.Empty;
            ShipRegionBvin = string.Empty;
            ShipPostalCode = string.Empty;
            ShipCountryName = string.Empty;
            ShipCountryBvin = string.Empty;
            ShipPhone = string.Empty;
        }

        /// <summary>Gets or sets the customer identifier.</summary>
        public string UserId { get; set; }
        /// <summary>Gets or sets the billing contact's first name.</summary>
        public string FirstName { get; set; }
        /// <summary>Gets or sets the billing contact's last name.</summary>
        public string LastName { get; set; }
        /// <summary>Gets or sets the billing company name.</summary>
        public string Company { get; set; }
        /// <summary>Gets or sets the billing street address.</summary>
        public string Street { get; set; }
        /// <summary>Gets or sets the billing city.</summary>
        public string City { get; set; }
        /// <summary>Gets or sets the billing region name.</summary>
        public string RegionName { get; set; }
        /// <summary>Gets or sets the billing region identifier.</summary>
        public string RegionBvin { get; set; }
        /// <summary>Gets or sets the billing postal code.</summary>
        public string PostalCode { get; set; }
        /// <summary>Gets or sets the billing country name.</summary>
        public string CountryName { get; set; }
        /// <summary>Gets or sets the billing country identifier.</summary>
        public string CountryBvin { get; set; }
        /// <summary>Gets or sets the billing country data.</summary>
        /// <seealso cref="ShipCountryData"/>
        public ICountry CountryData { get; set; }
        /// <summary>Gets or sets the billing telephone number.</summary>
        public string Phone { get; set; }
        /// <summary>Gets or sets the billing email address.</summary>
        public string Email { get; set; }
        /// <summary>Gets or sets the originating client IP address.</summary>
        public string IpAddress { get; set; }

        /// <summary>Gets or sets the shipping contact's first name.</summary>
        public string ShipFirstName { get; set; }
        /// <summary>Gets or sets the shipping contact's last name.</summary>
        public string ShipLastName { get; set; }
        /// <summary>Gets or sets the shipping company name.</summary>
        public string ShipCompany { get; set; }
        /// <summary>Gets or sets the shipping street address.</summary>
        public string ShipStreet { get; set; }
        /// <summary>Gets or sets the shipping city.</summary>
        public string ShipCity { get; set; }
        /// <summary>Gets or sets the shipping region name.</summary>
        public string ShipRegionName { get; set; }
        /// <summary>Gets or sets the shipping region identifier.</summary>
        public string ShipRegionBvin { get; set; }
        /// <summary>Gets or sets the shipping postal code.</summary>
        public string ShipPostalCode { get; set; }
        /// <summary>Gets or sets the shipping country name.</summary>
        public string ShipCountryName { get; set; }
        /// <summary>Gets or sets the shipping country identifier.</summary>
        public string ShipCountryBvin { get; set; }
        /// <summary>Gets or sets the shipping country data.</summary>
        /// <seealso cref="CountryData"/>
        public ICountry ShipCountryData { get; set; }
        /// <summary>Gets or sets the shipping telephone number.</summary>
        public string ShipPhone { get; set; }
    }
}
