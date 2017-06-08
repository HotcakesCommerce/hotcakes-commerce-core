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

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product inventory in the REST API.
    /// </summary>
    /// <remarks>The REST API equivalent is ProductInventoryy.</remarks>
    [DataContract]
    [Serializable]
    public class ProductInventoryDTO
    {
        public ProductInventoryDTO()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            ProductBvin = string.Empty;
            VariantId = string.Empty;
            QuantityOnHand = 0;
            QuantityReserved = 0;
            LowStockPoint = 0;
            OutOfStockPoint = 0;
        }

        /// <summary>
        ///     This is the unique ID or primary key of the product inventory record.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product inventory was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The unique ID or Bvin of the product that this inventory relates to.
        /// </summary>
        [DataMember]
        public string ProductBvin { get; set; }

        /// <summary>
        ///     When populated, the variant ID specifies that this record relates to a specific variant of the product.
        /// </summary>
        [DataMember]
        public string VariantId { get; set; }

        /// <summary>
        ///     The total physical count of items on hand.
        /// </summary>
        [DataMember]
        public int QuantityOnHand { get; set; }

        /// <summary>
        ///     Count of items in stock but reserved for carts or orders.
        /// </summary>
        [DataMember]
        public int QuantityReserved { get; set; }

        /// <summary>
        ///     Determines when a product has hit a point to where it is considered to be low on stock.
        /// </summary>
        [DataMember]
        public int LowStockPoint { get; set; }

        /// <summary>
        ///     The value that signifies that the the product should be considered out of stock.
        /// </summary>
        [DataMember]
        public int OutOfStockPoint { get; set; }
    }
}