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
using System.ComponentModel.DataAnnotations;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     There are many views that make use of the AddressViewModel. It's necessary whenever you're dealing with any
    ///     address, such as checkout billing/shipping address, affiliate addresses, the address book, and so on. This article
    ///     will help you understand what's available to you in the view model.
    /// </summary>
    [Serializable]
    public class AddressViewModel
    {
        /// <summary>
        ///     Contains the zip or postal code of the address, as necessary.
        /// </summary>
        private string _PostalCode = string.Empty;

        /// <summary>
        ///     Default constructor to set the default values for
        ///     each of the address property
        /// </summary>
        public AddressViewModel()
        {
            Init();
        }

        /// <summary>
        ///     Constructor with the parameter to set the
        ///     value of the address properties from the passed address object.
        ///     This generally called for address book editing and billing address editing purpose on the system.
        /// </summary>
        /// <param name="a">address object</param>
        public AddressViewModel(Address a)
        {
            Init();
            StoreId = a.StoreId;
            NickName = a.NickName;
            FirstName = a.FirstName;
            MiddleInitial = a.MiddleInitial;
            LastName = a.LastName;
            Company = a.Company;
            Line1 = a.Line1;
            Line2 = a.Line2;
            Line3 = a.Line3;
            City = a.City;
            RegionName = a.RegionDisplayName;
            RegionBvin = a.RegionBvin;
            PostalCode = a.PostalCode;
            CountryName = a.CountryDisplayName;
            CountryBvin = a.CountryBvin;
            Phone = a.Phone;
            Fax = a.Fax;
            WebSiteUrl = a.WebSiteUrl;
            UserBvin = a.UserBvin;
            AddressType = a.AddressType;
            LastUpdatedUtc = a.LastUpdatedUtc;
            Countries = new List<Country>();
            Regions = new List<Region>();
        }

        /// <summary>
        ///     List of the countries shown on the view when it loads Address view
        /// </summary>
        public List<Country> Countries { get; set; }

        /// <summary>
        ///     List of the regions for the chosen country on the view.
        /// </summary>
        /// <remarks>When first time view loads it load the regions for the first country in the list</remarks>
        public List<Region> Regions { get; set; }

        /// <summary>
        ///     The unique ID or primary key of the Address.
        /// </summary>
        /// <remarks>This property should always be used instead of Id.</remarks>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the address was last updated.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public long StoreId { get; set; }

        /// <summary>
        ///     A user-defined value to name this address for easy identification in the user interface.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string NickName { get; set; }

        /// <summary>
        ///     The first name of the recipient at this address.
        /// </summary>
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string FirstName { get; set; }

        /// <summary>
        ///     The middle initial of the recipient at this address.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string MiddleInitial { get; set; }

        /// <summary>
        ///     The last name or surname of the recipient at this address.
        /// </summary>
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string LastName { get; set; }

        /// <summary>
        ///     If this is not a residential address, a company name should be specified.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Company { get; set; }

        /// <summary>
        ///     The first line of the street address, such as 123 Main Street.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required]
        public string Line1 { get; set; }

        /// <summary>
        ///     The second line of the street address, such as Suite 100.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Line2 { get; set; }

        /// <summary>
        ///     The third line of the street address. Usually used for non-US or military addresses.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Line3 { get; set; }

        /// <summary>
        ///     The name of the city.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        [Required]
        public string City { get; set; }

        /// <summary>
        ///     This property will contain the SystemName of the region.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RegionName { get; set; }

        /// <summary>
        ///     This should contain a valid ID of the Region, if applicable.
        /// </summary>
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RegionBvin { get; set; }

        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PostalCode
        {
            get { return _PostalCode; }
            set { _PostalCode = value; }
        }

        /// <summary>
        ///     This property will contain the SystemName of the country.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CountryName { get; set; }

        /// <summary>
        ///     This is the unique ID or bvin of the country related to this address.
        /// </summary>
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CountryBvin { get; set; }

        /// <summary>
        ///     A telephone number for the address.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Phone { get; set; }

        /// <summary>
        ///     A fax or facsimile number for the address.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string Fax { get; set; }

        /// <summary>
        ///     A website URL for the address. Primarily used for vendors and manufacturers.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string WebSiteUrl { get; set; }

        /// <summary>
        ///     Addresses are mapped back to a CMS user record, even for vendors and manufacturers. This field is the user ID.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string UserBvin { get; set; }

        /// <summary>
        ///     Specifies if the address belongs to the merchant or not.
        /// </summary>
        [Required]
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public AddressTypes AddressType { get; set; }

        /// <summary>
        ///     Set default values to each of the address properties
        /// </summary>
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
            AddressType = AddressTypes.General;
            LastUpdatedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     Copy current view model to address class
        ///     This generally called for address book editing and billing address editing purpose on the system.
        /// </summary>
        /// <param name="a">Address object in which needs to set value.</param>
        public void CopyTo(Address a)
        {
            a.StoreId = StoreId;
            a.NickName = NickName;
            a.FirstName = FirstName;
            a.MiddleInitial = MiddleInitial;
            a.LastName = LastName;
            a.Company = Company;
            a.Line1 = Line1;
            a.Line2 = Line2;
            a.Line3 = Line3;
            a.City = City;
            a.RegionBvin = RegionBvin;
            a.PostalCode = PostalCode;
            a.CountryBvin = CountryBvin;
            a.Phone = Phone;
            a.Fax = Fax;
            a.WebSiteUrl = WebSiteUrl;
            a.UserBvin = UserBvin;
            a.AddressType = AddressType;
            a.LastUpdatedUtc = LastUpdatedUtc;
        }
    }
}