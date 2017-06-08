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
using Newtonsoft.Json;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The AffiliateViewModel is the primary view model that's used across the various affiliate
    ///     views. It's used to generate the rendered views, as we as to help update affiliate
    ///     information.
    /// </summary>
    [Serializable]
    public class AffiliateViewModel
    {
        public AffiliateViewModel()
        {
        }

        /// <summary>
        ///     Set the value of all parameters from the given AFfiliate
        ///     This generally called from Affiliate Dashboard  view.
        /// </summary>
        /// <param name="aff"></param>
        public AffiliateViewModel(Affiliate aff)
        {
            if (aff == null)
            {
                return;
            }

            Id = aff.Id;
            UserId = aff.UserId;
            Username = aff.Username;
            MyAffiliateId = aff.AffiliateId;
            ReferralAffiliateId = aff.ReferralAffiliateId;
            Approved = aff.Approved;
            Email = aff.Email;

            if (aff.Address == null)
            {
                return;
            }

            FirstName = aff.Address.FirstName;
            LastName = aff.Address.LastName;
            CountryId = aff.Address.CountryBvin;
            Company = aff.Address.Company;
            AddressLine = aff.Address.Line1;
            Company = aff.Address.Company;
            City = aff.Address.City;
            State = aff.Address.RegionBvin;
            PostalCode = aff.Address.PostalCode;
            Phone = aff.Address.Phone;
        }

        /// <summary>
        ///     Check if AFfiliate is logged in as customer to the site or not
        /// </summary>
        /// <returns></returns>
        public bool IsLoggedIn()
        {
            return UserId.HasValue;
        }

        /// <summary>
        ///     Check for empty values for first name and last name
        /// </summary>
        /// <returns></returns>
        public bool FirstLastNameRequired()
        {
            return string.IsNullOrEmpty(FirstName) || string.IsNullOrEmpty(LastName);
        }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        #region Properties

        /// <summary>
        ///     The unique ID for the affiliate
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     Current logged in customer id
        /// </summary>
        public int? UserId { get; set; }

        /// <summary>
        ///     Current logged in customer username
        /// </summary>
        [Required]
        public string Username { get; set; }

        /// <summary>
        ///     Value used as the password to login using the affiliate account
        /// </summary>
        [Required]
        public string Password { get; set; }

        /// <summary>
        ///     Confirmation value of the desired password
        /// </summary>
        [Compare("Password")]
        public string ConfirmPassword { get; set; }

        /// <summary>
        ///     The first name of the Affiliate
        /// </summary>
        [Required]
        public string FirstName { get; set; }

        /// <summary>
        ///     The last name or surname of the affiliate.
        /// </summary>
        [Required]
        public string LastName { get; set; }

        /// <summary>
        ///     Email address of the affiliate.
        /// </summary>
        [Required]
        [RegularExpression(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*")]
        public string Email { get; set; }

        /// <summary>
        ///     User friendly/readable ID for the affiliate
        /// </summary>
        [Required]
        [RegularExpression(@"[-\w]*", ErrorMessage = "Only alpha-numeric and '-', '_' characters are allowed")]
        public string MyAffiliateId { get; set; }

        /// <summary>
        ///     If another affiliate referred the current affiliate applicant, the referring affiliate ID should be here.
        /// </summary>
        [RegularExpression(@"[-\w]*", ErrorMessage = "Only alpha-numeric and '-', '_' characters are allowed")]
        public string ReferralAffiliateId { get; set; }

        /// <summary>
        ///     Checkbox to confirm the terms
        /// </summary>
        public bool ConfirmTerms { get; set; }

        /// <summary>
        ///     Flag which indicates whether affiliate has been approved or not.
        /// </summary>
        public bool Approved { get; set; }

        /// <summary>
        ///     This should container valid country id
        /// </summary>
        public string CountryId { get; set; }

        /// <summary>
        ///     If this is not a individual customer, a company name should be specified.
        /// </summary>
        public string Company { get; set; }

        /// <summary>
        ///     The address line of the street address, such as 123 Main Street.
        /// </summary>
        public string AddressLine { get; set; }

        /// <summary>
        ///     The name of the city.
        /// </summary>
        public string City { get; set; }

        /// <summary>
        ///     The name of the state.
        /// </summary>
        public string State { get; set; }

        /// <summary>
        ///     Contains the zip or postal code of the address, as necessary.
        /// </summary>
        public string PostalCode { get; set; }

        /// <summary>
        ///     A telephone number for the affiliate.
        /// </summary>
        public string Phone { get; set; }

        /// <summary>
        ///     List of the countries needs to be shown on view
        /// </summary>
        public List<Country> Countries { get; set; }

        /// <summary>
        ///     List of valid regions needs to be shown on view
        /// </summary>
        public List<Region> Regions { get; set; }

        /// <summary>
        ///     Agreement text needs to be shown to end user.
        /// </summary>
        public string AgreementText { get; set; }

        /// <summary>
        ///     Used to determine whether referrals are allowed for this affiliate or not.
        /// </summary>
        public bool AllowReferral { get; set; }

        #endregion
    }
}