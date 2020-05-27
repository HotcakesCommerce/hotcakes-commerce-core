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
using System.Collections.Generic;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1.Membership;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    ///     This is the primary class used to manage customer accounts. Many of these properties are duplicated in the CMS.
    /// </summary>
    [Serializable]
    public class CustomerAccount : IReplaceable
    {
        // Constructor			
        public CustomerAccount()
        {
            Bvin = string.Empty;
            StoreId = 0;
            Email = string.Empty;
            FirstName = string.Empty;
            LastName = string.Empty;
            Password = string.Empty;

            TaxExempt = false;
            Notes = string.Empty;
            PricingGroupId = string.Empty;

            Locked = false;
            LockedUntilUtc = DateTime.UtcNow;
            FailedLoginCount = 0;

            LastUpdatedUtc = DateTime.UtcNow;
            CreationDateUtc = DateTime.UtcNow;
            LastLoginDateUtc = DateTime.UtcNow;
            BillingAddress = new Address();
            ShippingAddress = new Address();
        }

        /// <summary>
        ///     The unique ID of the customer account
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The primary email address for the customer and it is used to get their avatar from gravatar.
        /// </summary>
        public string Email { get; set; }

        /// <summary>
        ///     The username of the customer in the CMS
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     First name of the customer
        /// </summary>
        public string FirstName { get; set; }

        /// <summary>
        ///     Last or surname of the customer
        /// </summary>
        public string LastName { get; set; }

        /// <summary>
        ///     Encrypted password of the customer
        /// </summary>
        public string Password { get; set; }

        /// <summary>
        ///     Address details for the primary shipping address
        /// </summary>
        public Address ShippingAddress { get; set; }

        /// <summary>
        ///     Address details for billing the customer
        /// </summary>
        public Address BillingAddress { get; set; }

        /// <summary>
        ///     Addresses from the customer's address book
        /// </summary>
        public AddressList Addresses
        {
            get { return _Addresses; }
            set { _Addresses = value; }
        }

        /// <summary>
        ///     Culture that was used when placing last order
        /// </summary>
        public string LastUsedCulture { get; set; }

        /// <summary>
        ///     Contains a list of phones for the customer
        /// </summary>
        /// <remarks>This doesn't appear to be populated anywhere. Might not be in use, or legacy code.</remarks>
        public PhoneNumberList Phones
        {
            get { return _Phones; }
            set { _Phones = value; }
        }

        // Other
        /// <summary>
        ///     Defines whether the customer is exempt from paying taxes
        /// </summary>
        public bool TaxExempt { get; set; }

        /// <summary>
        ///     If the customer is tax exempt, this number is used for tax reporting
        /// </summary>
        public string TaxExemptionNumber { get; set; }

        /// <summary>
        ///     Notes about the customer
        /// </summary>
        public string Notes { get; set; }

        /// <summary>
        ///     The unique ID (bvin) of the price group that this customer belongs to.
        /// </summary>
        /// <remarks>Enmpty string means that they are not part of any price group.</remarks>
        public string PricingGroupId { get; set; }

        // Security
        /// <summary>
        ///     Defines whether the customer is locked out or the site or not
        /// </summary>
        /// <remarks>This normally happens due to too many invalid login attempts</remarks>
        public bool Locked { get; set; }

        /// <summary>
        ///     The date that the user will become unlocked automatically
        /// </summary>
        public DateTime LockedUntilUtc { get; set; }

        /// <summary>
        ///     The number of times that the customer failed to login
        /// </summary>
        public int FailedLoginCount { get; set; }

        // Tracking
        /// <summary>
        ///     The date/time stamp of the last time the customer was updated
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The date/time stamp of when the customer was created
        /// </summary>
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        ///     The date/time stamp of the last time the customer logged in
        /// </summary>
        public DateTime LastLoginDateUtc { get; set; }

        /// <summary>
        ///     Provides a listing of the tokens that the customer information can be replaced with in email templates
        /// </summary>
        /// <param name="context">An instance of the Hotcakes Request context object</param>
        /// <returns>List of name/value pairs of the tokens and replacement content</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();

            result.Add(new HtmlTemplateTag("[[User.Bvin]]", Bvin));

            result.Add(new HtmlTemplateTag("[[User.Notes]]", Notes));
            result.Add(new HtmlTemplateTag("[[User.CreationDate]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, CreationDateUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[User.Email]]", Email));
            result.Add(new HtmlTemplateTag("[[User.UserName]]", Username));
            result.Add(new HtmlTemplateTag("[[User.FirstName]]", FirstName));
            result.Add(new HtmlTemplateTag("[[User.LastLoginDate]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, LastLoginDateUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[User.LastName]]", LastName));
            result.Add(new HtmlTemplateTag("[[User.LastUpdated]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, LastUpdatedUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[User.Locked]]", Locked.ToString()));
            result.Add(new HtmlTemplateTag("[[User.LockedUntil]]",
                DateHelper.ConvertUtcToStoreTime(context.CurrentStore, LockedUntilUtc).ToString()));
            result.Add(new HtmlTemplateTag("[[User.Password]]", Password));

            return result;
        }

        /// <summary>
        ///     Verifies the passed address for the customer, and if new, updates the customer record with it
        /// </summary>
        /// <param name="address">New address object to compare against the customer's current address</param>
        /// <returns>Boolean - if true, the address was created</returns>
        public bool CheckIfNewAddressAndAddNoUpdate(Address address)
        {
            var addressFound = false;
            foreach (var currAddress in Addresses)
            {
                if (currAddress.IsEqualTo(address))
                {
                    addressFound = true;
                    break;
                }
            }

            var createdAddress = false;

            if (!addressFound)
            {
                address.Bvin = Guid.NewGuid().ToString();
                _Addresses.Add(address);
                createdAddress = true;
            }

            return createdAddress;
        }

        /// <summary>
        ///     Finds the specified address and deletes it from the customer record
        /// </summary>
        /// <param name="bvin"></param>
        /// <returns></returns>
        public bool DeleteAddress(string bvin)
        {
            var result = false;

            var index = -1;
            for (var i = 0; i < _Addresses.Count; i++)
            {
                if (_Addresses[i].Bvin == bvin)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                _Addresses.RemoveAt(index);
                return true;
            }

            return result;
        }

        /// <summary>
        ///     Updates the customer record with the given address
        /// </summary>
        /// <param name="a">The new address you want the customer to have</param>
        /// <returns>Boolean - if true, the update was successful</returns>
        public bool UpdateAddress(Address a)
        {
            var result = false;

            var index = -1;
            for (var i = 0; i < _Addresses.Count; i++)
            {
                if (_Addresses[i].Bvin == a.Bvin)
                {
                    index = i;
                    break;
                }
            }
            if (index >= 0)
            {
                _Addresses[index] = a;
                return true;
            }

            return result;
        }

        //DTO
        /// <summary>
        ///     Allows you to convert the current customer account object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of CustomerAccountDTO</returns>
        public CustomerAccountDTO ToDto()
        {
            var dto = new CustomerAccountDTO();

            dto.Bvin = Bvin;
            dto.Email = Email;
            dto.FirstName = FirstName;
            dto.LastName = LastName;
            dto.Password = Password;

            dto.TaxExempt = TaxExempt;
            dto.Notes = Notes;
            dto.PricingGroupId = PricingGroupId;

            dto.FailedLoginCount = FailedLoginCount;

            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.CreationDateUtc = CreationDateUtc;
            dto.LastLoginDateUtc = LastLoginDateUtc;

            foreach (var a in Addresses)
            {
                dto.Addresses.Add(a.ToDto());
            }

            dto.ShippingAddress = ShippingAddress.ToDto();
            dto.BillingAddress = BillingAddress.ToDto();
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current customer account object using a CustomerAccount instance
        /// </summary>
        /// <param name="dto">An instance of the customer account from the REST API</param>
        public void FromDto(CustomerAccountDTO dto)
        {
            Bvin = dto.Bvin;
            Email = dto.Email;
            FirstName = dto.FirstName;
            LastName = dto.LastName;
            Password = dto.Password;

            TaxExempt = dto.TaxExempt;
            Notes = dto.Notes;
            PricingGroupId = dto.PricingGroupId;

            FailedLoginCount = dto.FailedLoginCount;

            LastUpdatedUtc = dto.LastUpdatedUtc;
            CreationDateUtc = dto.CreationDateUtc;
            LastLoginDateUtc = dto.LastLoginDateUtc;

            foreach (var a in dto.Addresses)
            {
                var addr = new Address();
                addr.FromDto(a);
                Addresses.Add(addr);
            }

            ShippingAddress.FromDto(dto.ShippingAddress);
            BillingAddress.FromDto(dto.BillingAddress);
        }

        #region Fields

        // Addresses        
        private AddressList _Addresses = new AddressList();
        private PhoneNumberList _Phones = new PhoneNumberList();

        #endregion
    }
}