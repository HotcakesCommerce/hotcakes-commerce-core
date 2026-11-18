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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Content;
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
        /// <summary>
        /// Initializes a customer account with empty text values, current UTC timestamps, an empty address book,
        /// and new billing and shipping <see cref="Address"/> instances.
        /// </summary>
        /// <remarks>
        /// The constructor does not assign a customer identifier or store identifier. <see cref="Password"/> is
        /// initialized to an empty string; providers determine how a supplied password is stored.
        /// </remarks>
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
        /// Gets or sets the unique identifier of the customer account.
        /// </summary>
        /// <remarks>Providers may assign this value when the account is created.</remarks>
        public string Bvin { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the Hotcakes store that owns the account.
        /// </summary>
        /// <remarks>This is typically 1, but differs in multi-tenant environments.</remarks>
        public long StoreId { get; set; }

        /// <summary>
        /// Gets or sets the customer's primary email address.
        /// </summary>
        /// <remarks>The address is also used by some membership providers as the account lookup value and to obtain a Gravatar.</remarks>
        public string Email { get; set; }

        /// <summary>
        /// Gets or sets the username recognized by the configured CMS or membership provider.
        /// </summary>
        /// <remarks>The REST DTO does not carry this property; provider mappings may derive or omit it.</remarks>
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
        /// Gets or sets the provider-specific stored password value.
        /// </summary>
        /// <remarks>This value is not necessarily clear text. Providers receive a clear password through their account and password-change workflows and determine the stored representation; the DNN mapping does not expose a stored password when reading an account.</remarks>
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
        /// Creates the email-template replacement tokens supported by this account.
        /// </summary>
        /// <param name="context">The request context whose current store supplies the time-zone conversion.</param>
        /// <returns>A list of token names and replacement values, including the stored <see cref="Password"/> value.</returns>
        /// <remarks>The date tokens are converted from UTC to the current store's local time. The method does not sanitize or redact values.</remarks>
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
        /// Adds an address to the address book when no equal address is already present.
        /// </summary>
        /// <param name="address">The address to compare and, when new, add.</param>
        /// <returns><see langword="true"/> when an address was added; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The supplied address is mutated with a new GUID <see cref="Address.Bvin"/> when added. This method does not persist the account; use <see cref="MembershipServices.CheckIfNewAddressAndAddWithUpdate"/> when the account must also be updated.</remarks>
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
        /// Removes the address with the specified identifier from the address book.
        /// </summary>
        /// <param name="bvin">The address identifier to remove.</param>
        /// <returns><see langword="true"/> when an address was removed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This changes only the in-memory address book. It does not persist the customer.</remarks>
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
        /// Replaces an address-book entry with the supplied address when their identifiers match.
        /// </summary>
        /// <param name="a">The replacement address; its <see cref="Address.Bvin"/> identifies the entry.</param>
        /// <returns><see langword="true"/> when an entry was replaced; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This changes only the in-memory address book. It does not persist the customer.</remarks>
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
        /// Converts this account to the REST API <see cref="CustomerAccountDTO"/> representation.
        /// </summary>
        /// <returns>A new DTO containing the account fields exposed by the REST API.</returns>
        /// <remarks><see cref="StoreId"/>, <see cref="Username"/>, <see cref="LastUsedCulture"/>, <see cref="Phones"/>, <see cref="TaxExemptionNumber"/>, and <see cref="Locked"/> are not copied because they are not represented by <see cref="CustomerAccountDTO"/>. Address entries are converted individually.</remarks>
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
        /// Populates this account from a REST API <see cref="CustomerAccountDTO"/>.
        /// </summary>
        /// <param name="dto">The DTO whose exposed values are copied into this account.</param>
        /// <remarks><see cref="StoreId"/>, <see cref="Username"/>, <see cref="LastUsedCulture"/>, <see cref="Phones"/>, <see cref="TaxExemptionNumber"/>, and <see cref="Locked"/> are not changed because the DTO does not contain them. Address DTOs are appended to the existing address book; they are not used to clear it.</remarks>
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
