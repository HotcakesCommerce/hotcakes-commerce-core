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
using Hotcakes.CommerceDTO.v1.Contacts;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    ///     This is the main object that is used for all vendor/manufacturer contact management in the main application
    /// </summary>
    /// <remarks>The main REST API equivalent to this is the VendorManufacturerContactDTO object</remarks>
    [Serializable]
    [Obsolete("Obsolete in 1.8.0. VendorManufacturer contacts are not used")]
    public class VendorManufacturerContact
    {
        public VendorManufacturerContact()
        {
            Id = 0;
            VendorManufacturerId = string.Empty;
            UserId = string.Empty;
            StoreId = 0;
        }

        /// <summary>
        ///     The unique ID of the vendor/manufacturer contact.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The bvin or ID of the VendorManufacturer that this contact belongs to.
        /// </summary>
        public string VendorManufacturerId { get; set; }

        /// <summary>
        ///     The unqiue ID of the user account that this object is referring to.
        /// </summary>
        public string UserId { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current vendor/manufacturer contact object to the DTO equivalent for use with the REST
        ///     API
        /// </summary>
        /// <returns>A new instance of VendorManufacturerContactDTO</returns>
        public VendorManufacturerContactDTO ToDto()
        {
            var dto = new VendorManufacturerContactDTO();

            dto.Id = Id;
            dto.StoreId = StoreId;
            dto.UserId = UserId;
            dto.VendorManufacturerId = VendorManufacturerId;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current vendor/manufacturer contact object using an VendorManufacturerContactDTO
        ///     instance
        /// </summary>
        /// <param name="dto">An instance of the vendor/manufacturer contact from the REST API</param>
        public void FromDto(VendorManufacturerContactDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            StoreId = dto.StoreId;
            UserId = dto.UserId;
            VendorManufacturerId = dto.VendorManufacturerId;
        }

        #endregion
    }
}