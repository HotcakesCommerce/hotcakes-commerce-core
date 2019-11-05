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
using System.Linq;
using Hotcakes.Commerce.Content;
using Hotcakes.CommerceDTO.v1.Contacts;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    ///     This is the main object that is used for all vendor/manufacturer management in the main application
    /// </summary>
    /// <remarks>The main REST API equivalent to this is the VendorManufacturerDTO object</remarks>
    [Serializable]
    public class VendorManufacturer : IReplaceable
    {
        public VendorManufacturer()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            DisplayName = string.Empty;
            EmailAddress = string.Empty;
            Address = new Address();
            DropShipEmailTemplateId = string.Empty;
            ContactType = VendorManufacturerType.Unknown;
#pragma warning disable 0612, 0618
            Contacts = new List<VendorManufacturerContact>();
#pragma warning restore 0612, 0618
        }

        /// <summary>
        ///     This is the ID of the vendor/manufacturer.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     An auditing value to track the last time the vendor/manufacturer record was updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The value used in all administrative and customer views to identify the vendor/manufacturer.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     The primary email address used to contact the vendor/manufacturer. This is used for system emails for actions such
        ///     as drop-shipping.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     An address object matching the vendor/manufacturer.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        ///     The email template ID used when workflow sends a notification email about a drop shipment.
        /// </summary>
        public string DropShipEmailTemplateId { get; set; }

        /// <summary>
        ///     Specifies whether this instance of the object contains contacts for a vendor or a manufacturer.
        /// </summary>
        public VendorManufacturerType ContactType { get; set; }

        /// <summary>
        ///     Contains a listing of user accounts that belong to the current vendor/manufacturer.
        /// </summary>
        [Obsolete("Obsolete in 1.8.0. Contacts property is not used")]
        public List<VendorManufacturerContact> Contacts { get; set; }

        /// <summary>
        ///     Returns a listing of tokens and their replacement used by email templates.
        /// </summary>
        /// <param name="context">A populated instance of the Hotcakes Request context</param>
        /// <returns>List - a key value pairing of tokens and their replacement content.</returns>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();
            result.Add(new HtmlTemplateTag("[[VendorManufacturer.EmailAddress]]", EmailAddress));
            result.Add(new HtmlTemplateTag("[[VendorManufacturer.Name]]", DisplayName));
            return result;
        }

        #region Obsolete

#pragma warning disable 0612, 0618
        /// <summary>
        ///     Allows for a contact to be added to the current vendor/manufacturer.
        /// </summary>
        /// <param name="userId">The unique ID of the user to add to this vendor/manufacturer.</param>
        /// <returns>If true, the contact was added successfully.</returns>
        [Obsolete("Obsolete in 1.8.0. Contacts property is not used")]
        public bool AddContact(string userId)
        {
            if (!ContactExists(userId))
            {
                Contacts.Add(new VendorManufacturerContact
                {
                    StoreId = StoreId,
                    UserId = userId,
                    VendorManufacturerId = Bvin
                });
            }
            return true;
        }

        /// <summary>
        ///     Allows for a specified contact to be removed from the Contacts property in the current vendor/manufacturer.
        /// </summary>
        /// <param name="userId">The unique ID of the user account to remove from the property.</param>
        /// <returns>If true, the contact was successfully removed.</returns>
        [Obsolete("Obsolete in 1.8.0. Contacts property is not used")]
        public bool RemoveContact(string userId)
        {
            var c = Contacts.Where(y => y.UserId == userId).SingleOrDefault();
            if (c != null)
            {
                Contacts.Remove(c);
            }
            return true;
        }

        /// <summary>
        ///     Performs a check using the specified UserID to see if the user exists in the Contacts property.
        /// </summary>
        /// <param name="userId">The unique ID of the user account to look for.</param>
        /// <returns>If true, the contact exists in the Contacts property.</returns>
        [Obsolete("Obsolete in 1.8.0. Contacts property is not used")]
        public bool ContactExists(string userId)
        {
            var c = Contacts.Where(y => y.UserId == userId).SingleOrDefault();
            if (c != null) return true;
            return false;
        }
#pragma warning restore 0612, 0618

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current vendor/manufacturer object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of VendorManufacturerDTO</returns>
        public VendorManufacturerDTO ToDto()
        {
            var dto = new VendorManufacturerDTO();

            dto.Bvin = Bvin ?? string.Empty;
            dto.StoreId = StoreId;
            dto.LastUpdated = LastUpdated;
            dto.DisplayName = DisplayName ?? string.Empty;
            dto.EmailAddress = EmailAddress ?? string.Empty;
            dto.Address = Address.ToDto() ?? new Address().ToDto();
            dto.DropShipEmailTemplateId = DropShipEmailTemplateId ?? string.Empty;
            dto.ContactType = (VendorManufacturerTypeDTO) (int) ContactType;
#pragma warning disable 0612, 0618
            foreach (var contact in Contacts)
            {
                dto.Contacts.Add(contact.ToDto());
            }
#pragma warning restore 0612, 0618
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current vendor/manufacturer object using an VendorManufacturerDTO instance
        /// </summary>
        /// <param name="dto">An instance of the vendor/manufacturer from the REST API</param>
        public void FromDto(VendorManufacturerDTO dto)
        {
            if (dto == null) return;

            Bvin = dto.Bvin;
            StoreId = dto.StoreId;
            LastUpdated = dto.LastUpdated;
            DisplayName = dto.DisplayName;
            EmailAddress = dto.EmailAddress;
            Address.FromDto(dto.Address);
            DropShipEmailTemplateId = dto.DropShipEmailTemplateId;
            ContactType = (VendorManufacturerType) (int) dto.ContactType;
#pragma warning disable 0612, 0618
            Contacts.Clear();
            foreach (var c in dto.Contacts)
            {
                var v = new VendorManufacturerContact();
                v.FromDto(c);
                Contacts.Add(v);
            }
#pragma warning restore 0612, 0618
        }

        #endregion
    }
}