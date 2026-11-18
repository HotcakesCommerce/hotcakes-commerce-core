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
        /// <summary>
        /// Initializes a vendor or manufacturer with empty text values, a new <see cref="Address"/>, an unknown contact type,
        /// and a UTC audit timestamp.
        /// </summary>
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
        /// Gets or sets the postal address associated with the vendor or manufacturer.
        /// </summary>
        /// <remarks>The repositories persist this value as the address XML column.</remarks>
        public Address Address { get; set; }

        /// <summary>Gets or sets the stable Hotcakes identifier for the contact.</summary>
        /// <remarks>The repository maps this value to the database GUID representation.</remarks>
        public string Bvin { get; set; }

        /// <summary>Gets or sets whether this contact represents a vendor or manufacturer.</summary>
        /// <remarks><see cref="VendorRepository"/> and <see cref="ManufacturerRepository"/> force this value when loading records.</remarks>
        public VendorManufacturerType ContactType { get; set; }

        /// <summary>Gets or sets the display name used to identify the contact in administrative and customer views.</summary>
        public string DisplayName { get; set; }

        /// <summary>Gets or sets the e-mail template identifier used for drop-shipment notifications.</summary>
        public string DropShipEmailTemplateId { get; set; }

        /// <summary>Gets or sets the primary e-mail address used for system notifications such as drop-shipping.</summary>
        public string EmailAddress { get; set; }

        /// <summary>Gets or sets the UTC timestamp used to audit the last update to the contact.</summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>Gets or sets the Hotcakes store that owns this contact.</summary>
        /// <remarks>Repository operations use the current store context to enforce store scope.</remarks>
        public long StoreId { get; set; }

        /// <summary>Returns the replacement tokens exposed to e-mail templates for this contact.</summary>
        /// <param name="context">The populated Hotcakes request context for the template operation.</param>
        /// <returns>A list containing the e-mail address and display-name token replacements.</returns>
        /// <remarks>The current implementation does not read the context; it is accepted to satisfy <see cref="IReplaceable"/>.</remarks>
        public List<HtmlTemplateTag> GetReplaceableTags(HccRequestContext context)
        {
            var result = new List<HtmlTemplateTag>();
            result.Add(new HtmlTemplateTag("[[VendorManufacturer.EmailAddress]]", EmailAddress));
            result.Add(new HtmlTemplateTag("[[VendorManufacturer.Name]]", DisplayName));
            return result;
        }

        #region DTO

        /// <summary>Creates the REST API representation of this vendor or manufacturer.</summary>
        /// <returns>A new <see cref="VendorManufacturerDTO"/> containing the current values.</returns>
        /// <remarks>The contact type is converted by numeric value to preserve DTO compatibility.</remarks>
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

        /// <summary>Populates this contact from a REST API data-transfer object.</summary>
        /// <param name="dto">The source <see cref="VendorManufacturerDTO"/>; null leaves this instance unchanged.</param>
        /// <remarks>The address is populated through its existing DTO conversion and the contact type is converted by numeric value.</remarks>
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
