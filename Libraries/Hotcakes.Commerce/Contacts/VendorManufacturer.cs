#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
        }

        /// <summary>
        ///     An address object matching the vendor/manufacturer.
        /// </summary>
        public Address Address { get; set; }

        /// <summary>
        ///     This is the ID of the vendor/manufacturer.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     Specifies whether this instance of the object contains contacts for a vendor or a manufacturer.
        /// </summary>
        public VendorManufacturerType ContactType { get; set; }

        /// <summary>
        ///     The value used in all administrative and customer views to identify the vendor/manufacturer.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     The e-mail template ID used when workflow sends a notification e-mail about a drop shipment.
        /// </summary>
        public string DropShipEmailTemplateId { get; set; }

        /// <summary>
        ///     The primary e-mail address used to contact the vendor/manufacturer. This is used for system e-mails for actions such as drop-shipping.
        /// </summary>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     An auditing value to track the last time the vendor/manufacturer record was updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

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
        }

        #endregion
    }
}