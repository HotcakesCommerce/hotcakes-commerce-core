﻿#region License

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

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary class used for wish list items in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is WishListItem.</remarks>
    [DataContract]
    [Serializable]
    public class WishListItemDTO
    {
        public WishListItemDTO()
        {
            Init();
        }

        /// <summary>
        ///     The unique ID or primary key of the wish list item.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     Defines the user account that the wish list item belongs to.
        /// </summary>
        [DataMember]
        public string CustomerId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the wish list item was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID of the product that this wish list item refers to.
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        ///     The amount of products that this wish list item is saving.
        /// </summary>
        [DataMember]
        public int Quantity { get; set; }

        /// <summary>
        ///     When applicable, the choices or options of the product.
        /// </summary>
        [DataMember]
        public List<OptionSelectionDTO> SelectionData { get; set; }

        private void Init()
        {
            Id = 0;
            StoreId = 0;
            CustomerId = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            ProductId = string.Empty;
            Quantity = 1;
            SelectionData = new List<OptionSelectionDTO>();
        }
    }
}