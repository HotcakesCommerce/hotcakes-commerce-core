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

using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Shipping
{
    /// <summary>
    ///     This enumeration is used to determine where a product will be shipped from.
    /// </summary>
    [DataContract]
    public enum ShippingModeDTO
    {
        /// <summary>
        ///     This member should not be used.
        /// </summary>
        [EnumMember] None = 0,

        /// <summary>
        ///     If specified, this will have the product ship from the store address.
        /// </summary>
        [EnumMember] ShipFromSite = 1,

        /// <summary>
        ///     This member dictates that the product will ship from the vendors address.
        /// </summary>
        [EnumMember] ShipFromVendor = 2,

        /// <summary>
        ///     When specified, the product will ship from the manufacturers address.
        /// </summary>
        [EnumMember] ShipFromManufacturer = 3
    }
}