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
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     Product variant information
    /// </summary>
    [DataContract]
    [Serializable]
    public class VariantDTO
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public VariantDTO()
        {
            Bvin = string.Empty;
            ProductId = string.Empty;
            Selections = new List<OptionSelectionDTO>();
            Sku = string.Empty;
            Price = -1;
            CustomProperty = string.Empty;
        }

        /// <summary>
        ///     Unique identifier of the variant.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     Unique product identifier for which this variant has been created.
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        ///     A SKU is the "stock keeping unit" and is often used as a primary or unique key to identify the product across
        ///     multiple mediums and systems.
        /// </summary>
        [DataMember]
        public string Sku { get; set; }

        /// <summary>
        ///     Additional price that will be added to the product price when this variant has been chosen.
        /// </summary>
        [DataMember]
        public decimal Price { get; set; }

        /// <summary>
        ///     Options available for the purchase of this product
        /// </summary>
        [DataMember]
        public List<OptionSelectionDTO> Selections { get; set; }

        /// <summary>
        ///     The Custom Property to add further variant details
        /// </summary>
        [DataMember]
        public string CustomProperty { get; set; }
    }
}