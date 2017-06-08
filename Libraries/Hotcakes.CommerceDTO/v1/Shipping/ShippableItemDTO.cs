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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Shipping
{
    /// <summary>
    ///     This is the primary class used for all shippable items in the REST API
    /// </summary>
    [DataContract]
    [Serializable]
    public class ShippableItemDTO
    {
        public ShippableItemDTO()
        {
            IsNonShipping = false;
            ExtraShipFee = 0m;
            Weight = 0m;
            Length = 0m;
            Width = 0m;
            Height = 0m;
            ShippingSource = ShippingModeDTO.ShipFromSite;
            ShippingSourceId = string.Empty;
            ShipSeparately = false;
        }

        /// <summary>
        ///     If true, the associated product will not be shipped and therefore should not have shipping logic applied.
        /// </summary>
        [DataMember]
        public bool IsNonShipping { get; set; }

        /// <summary>
        ///     If greater than zero, the specified fee should be added to the shipping fee presented to the customer.
        /// </summary>
        [DataMember]
        public decimal ExtraShipFee { get; set; }

        /// <summary>
        ///     The shippable weight of the product in pounds.
        /// </summary>
        [DataMember]
        public decimal Weight { get; set; }

        /// <summary>
        ///     The shippable length of the product in inches.
        /// </summary>
        [DataMember]
        public decimal Length { get; set; }

        /// <summary>
        ///     The shippable width of the product in inches.
        /// </summary>
        [DataMember]
        public decimal Width { get; set; }

        /// <summary>
        ///     The shippable height of the product in inches.
        /// </summary>
        [DataMember]
        public decimal Height { get; set; }

        /// <summary>
        ///     This defines where the product will be shipped from.
        /// </summary>
        [DataMember]
        public ShippingModeDTO ShippingSource { get; set; }

        /// <summary>
        ///     This ID value should match a vendor or manufacture when that respective ShippingSource is specified.
        /// </summary>
        [DataMember]
        public string ShippingSourceId { get; set; }

        /// <summary>
        ///     If true, the associated product cannot be shipped with other products.
        /// </summary>
        [DataMember]
        public bool ShipSeparately { get; set; }
    }
}