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
    ///     This is the primary object that is used to manage all aspects of product option items in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is OptionItem.</remarks>
    [DataContract]
    [Serializable]
    public class OptionItemDTO
    {
        public OptionItemDTO()
        {
            Bvin = string.Empty;
            StoreId = 0;
            OptionBvin = string.Empty;
            Name = string.Empty;
            PriceAdjustment = 0;
            WeightAdjustment = 0;
            IsLabel = false;
            SortOrder = 0;
            IsDefault = false;
        }

        /// <summary>
        ///     The unique ID or primary key of the current product option item.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     This value refers to the Option ID that this item belongs to.
        /// </summary>
        [DataMember]
        public string OptionBvin { get; set; }

        /// <summary>
        ///     This is that localized name of the product option item as customers will see it.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     If this item belongs to a variant, this value will be the new price when this product option item is selected.
        /// </summary>
        [DataMember]
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        ///     If this item belongs to a variant, this value will be the new weight when this product option item is selected.
        /// </summary>
        [DataMember]
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        ///     Used with the drop down list OptionType, this item will be rendered differently if true. All other OptionTypes will
        ///     ignore this item unless this value is false.
        /// </summary>
        [DataMember]
        public bool IsLabel { get; set; }

        /// <summary>
        ///     This value is used to sort this item when grouped with other items.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        ///     When True, this item will be used as the default item over other items that might be associated with the same
        ///     product option.
        /// </summary>
        [DataMember]
        public bool IsDefault { get; set; }
    }
}