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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of package items in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is OrderPackageItem.</remarks>
    [DataContract]
    [Serializable]
    public class OrderPackageItemDTO
    {
        public OrderPackageItemDTO()
        {
            ProductBvin = string.Empty;
            LineItemId = 0;
            Quantity = 0;
            ShippingMethodBvin = string.Empty;
        }

        /// <summary>
        ///     The unique ID or bvin of the product that is in this package.
        /// </summary>
        [DataMember]
        public string ProductBvin { get; set; }

        /// <summary>
        ///     The unique ID of the line item that matches this package.
        /// </summary>
        [DataMember]
        public long LineItemId { get; set; }

        /// <summary>
        ///     Total number of products that are in this package.
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        ///     Shipping method unique identifier.
        /// </summary>
        [DataMember]
        public string ShippingMethodBvin { get; set; }
    }
}