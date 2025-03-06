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
using System.Runtime.Serialization;
using Hotcakes.CommerceDTO.v1.Shipping;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Order Package in the REST API
    /// </summary>
    /// <remarks>The main application equivalent is OrderPackage.</remarks>
    [DataContract]
    [Serializable]
    public class OrderPackageDTO
    {
        public OrderPackageDTO()
        {
            CustomProperties = new List<CustomPropertyDTO>();
            Description = string.Empty;
            EstimatedShippingCost = 0;
            HasShipped = false;
            Height = 0;
            Id = 0;
            Items = new List<OrderPackageItemDTO>();
            LastUpdatedUtc = DateTime.UtcNow;
            Length = 0;
            OrderId = string.Empty;
            ShipDateUtc = DateTime.UtcNow;
            ShippingMethodId = string.Empty;
            ShippingProviderId = string.Empty;
            ShippingProviderServiceCode = string.Empty;
            SizeUnits = LengthTypeDTO.Inches;
            StoreId = 0;
            TrackingNumber = string.Empty;
            Weight = 0;
            WeightUnits = WeightTypeDTO.Pounds;
            Width = 0;
        }

        /// <summary>
        ///     This is the ID of the current order package.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the order package was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     A listing of the products or line items in the order package
        /// </summary>
        [DataMember]
        public List<OrderPackageItemDTO> Items { get; set; }

        /// <summary>
        ///     A description of the package consisting of the SKU and product names.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     The unique ID of the order that this package belongs to.
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }

        /// <summary>
        ///     The width of the package.
        /// </summary>
        [DataMember]
        public decimal Width { get; set; }

        /// <summary>
        ///     The height of the package.
        /// </summary>
        [DataMember]
        public decimal Height { get; set; }

        /// <summary>
        ///     The length of the package.
        /// </summary>
        [DataMember]
        public decimal Length { get; set; }

        /// <summary>
        ///     The units used to measure the dimensions of the package.
        /// </summary>
        [DataMember]
        public LengthTypeDTO SizeUnits { get; set; }

        /// <summary>
        ///     The weight of the package.
        /// </summary>
        [DataMember]
        public decimal Weight { get; set; }

        /// <summary>
        ///     The units used to measure the weight of the package.
        /// </summary>
        [DataMember]
        public WeightTypeDTO WeightUnits { get; set; }

        /// <summary>
        ///     The unique ID of the shipping provider used for this package.
        /// </summary>
        [DataMember]
        public string ShippingProviderId { get; set; }

        /// <summary>
        ///     The service code used by the shipping provider to describe the type of package.
        /// </summary>
        [DataMember]
        public string ShippingProviderServiceCode { get; set; }

        /// <summary>
        ///     A unique ID used by the shipping provider to track the package.
        /// </summary>
        [DataMember]
        public string TrackingNumber { get; set; }

        /// <summary>
        ///     A boolean value to determine if the package has shipped or not.
        /// </summary>
        [DataMember]
        public bool HasShipped { get; set; }

        /// <summary>
        ///     The date/time stamp with the package shipped.
        /// </summary>
        [DataMember]
        public DateTime ShipDateUtc { get; set; }

        /// <summary>
        ///     A shipping estimate returned from the shipping provider.
        /// </summary>
        [DataMember]
        public decimal EstimatedShippingCost { get; set; }

        /// <summary>
        ///     The unique ID of the shipping provider method
        /// </summary>
        [DataMember]
        public string ShippingMethodId { get; set; }

        /// <summary>
        ///     A collection of settings or meta data used for the package.
        /// </summary>
        /// <remarks>
        ///     Highly useful for things like ERP integrations.
        /// </remarks>
        [DataMember]
        public List<CustomPropertyDTO> CustomProperties { get; set; }
    }
}