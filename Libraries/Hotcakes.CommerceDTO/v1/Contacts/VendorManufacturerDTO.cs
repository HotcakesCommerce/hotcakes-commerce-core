#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
    ///     This is the main object that is used for all vendor/manufacturer management in the REST API
    /// </summary>
    /// <remarks>The main application equivalent to this is the VendorManufacturer object</remarks>
    [DataContract]
    [Serializable]
    public class VendorManufacturerDTO
    {
        public VendorManufacturerDTO()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            DisplayName = string.Empty;
            EmailAddress = string.Empty;
            Address = new AddressDTO();
            DropShipEmailTemplateId = string.Empty;
            ContactType = VendorManufacturerTypeDTO.Vendor;
        }

        /// <summary>
        ///     This is the ID of the vendor/manufacturer.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     An auditing value to track the last time the vendor/manufacturer record was updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The value used in all administrative and customer views to identify the vendor/manufacturer.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        ///     The primary email address used to contact the vendor/manufacturer. This is used for system emails for actions such
        ///     as drop-shipping.
        /// </summary>
        [DataMember]
        public string EmailAddress { get; set; }

        /// <summary>
        ///     An address object matching the vendor/manufacturer.
        /// </summary>
        [DataMember]
        public AddressDTO Address { get; set; }

        /// <summary>
        ///     The email template ID used when workflow sends a notification email about a drop shipment.
        /// </summary>
        [DataMember]
        public string DropShipEmailTemplateId { get; set; }

        /// <summary>
        ///     Specifies whether this instance of the object contains contacts for a vendor or a manufacturer.
        /// </summary>
        [DataMember]
        public VendorManufacturerTypeDTO ContactType { get; set; }
    }
}