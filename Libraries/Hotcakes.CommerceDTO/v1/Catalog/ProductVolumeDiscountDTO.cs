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

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product volume discount in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is ProductVolumeDiscount.</remarks>
    [DataContract]
    [Serializable]
    public class ProductVolumeDiscountDTO
    {
        public ProductVolumeDiscountDTO()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            ProductId = string.Empty;
            Qty = 0;
            Amount = 0;
            DiscountType = ProductVolumeDiscountTypeDTO.None;
        }

        /// <summary>
        ///     The unique ID or primary key of the product volume discount.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product volume discount was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The unique ID or Bvin of the product that this discount applies to.
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        ///     The minimum number of products that must be in the cart to qualify for this volume discount.
        /// </summary>
        [DataMember]
        public int Qty { get; set; }

        /// <summary>
        ///     This property should hold the new product price or the discount percentage for the product.
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Specifies the type of discount that the amount is, decimal or percentage.
        /// </summary>
        [DataMember]
        public ProductVolumeDiscountTypeDTO DiscountType { get; set; }
    }
}