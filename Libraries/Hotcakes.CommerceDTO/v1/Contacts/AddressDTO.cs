#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    ///     This is the main object that is used for all address management in the REST API
    /// </summary>
    /// <remarks>The main application equivalent to this is the Address object</remarks>
    [DataContract]
    [Serializable]
    public class AddressDTO
    {
        public AddressDTO()
        {
            Init();
        }

        /// <summary>
        ///     This is the unique ID of the address.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the address was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     A user-defined value to name this address for easy identification in the user interface.
        /// </summary>
        [DataMember]
        public string NickName { get; set; }

        /// <summary>
        ///     The first name of the recipient at this address.
        /// </summary>
        [DataMember]
        public string FirstName { get; set; }

        /// <summary>
        ///     The middle initial of the recipient at this address.
        /// </summary>
        [DataMember]
        public string MiddleInitial { get; set; }

        /// <summary>
        ///     The last name or surname of the recipient at this address.
        /// </summary>
        [DataMember]
        public string LastName { get; set; }

        /// <summary>
        ///     If this is not a residential address, a company name should be specified.
        /// </summary>
        [DataMember]
        public string Company { get; set; }

        /// <summary>
        ///     The first line of the street address, such as 123 Main Street.
        /// </summary>
        [DataMember]
        public string Line1 { get; set; }

        /// <summary>
        ///     The second line of the street address, such as Suite 100.
        /// </summary>
        [DataMember]
        public string Line2 { get; set; }

        /// <summary>
        ///     The third line of the street address. Usually used for non-US or military addresses.
        /// </summary>
        [DataMember]
        public string Line3 { get; set; }

        /// <summary>
        ///     The name of the city.
        /// </summary>
        [DataMember]
        public string City { get; set; }

        /// <summary>
        ///     This property will contain the SystemName of the region.
        /// </summary>
        [DataMember]
        public string RegionName { get; set; }

        /// <summary>
        ///     This should contain a valid ID of the Region, if applicable.
        /// </summary>
        [DataMember]
        public string RegionBvin { get; set; }

        /// <summary>
        ///     Contains the zip or postal code of the address, as necessary.
        /// </summary>
        [DataMember]
        public string PostalCode { get; set; }

        /// <summary>
        ///     This property will contain the SystemName of the country.
        /// </summary>
        [DataMember]
        public string CountryName { get; set; }

        /// <summary>
        ///     This is the unique ID or bvin of the country related to this address.
        /// </summary>
        [DataMember]
        public string CountryBvin { get; set; }

        /// <summary>
        ///     A telephone number for the address.
        /// </summary>
        [DataMember]
        public string Phone { get; set; }

        /// <summary>
        ///     A fax or facsimile number for the address.
        /// </summary>
        [DataMember]
        public string Fax { get; set; }

        /// <summary>
        ///     A website URL for the address. Primarily used for vendors and manufacturers.
        /// </summary>
        [DataMember]
        public string WebSiteUrl { get; set; }

        /// <summary>
        ///     Addresses are mapped back to a CMS user record, even for vendors and manufacturers. This field is the user ID.
        /// </summary>
        [DataMember]
        public string UserBvin { get; set; }

        /// <summary>
        ///     Specifies if the address belongs to the merchant or not.
        /// </summary>
        [DataMember]
        public AddressTypesDTO AddressType { get; set; }

        private void Init()
        {
            StoreId = 0;
            NickName = string.Empty;
            FirstName = string.Empty;
            MiddleInitial = string.Empty;
            LastName = string.Empty;
            Company = string.Empty;
            Line1 = string.Empty;
            Line2 = string.Empty;
            Line3 = string.Empty;
            City = string.Empty;
            RegionName = string.Empty;
            RegionBvin = string.Empty;
            PostalCode = string.Empty;
            CountryName = string.Empty;
            CountryBvin = string.Empty;
            Phone = string.Empty;
            Fax = string.Empty;
            WebSiteUrl = string.Empty;
            UserBvin = string.Empty;
            AddressType = AddressTypesDTO.General;
            LastUpdatedUtc = DateTime.UtcNow;
        }
    }
}