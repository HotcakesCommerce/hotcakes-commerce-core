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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of choices that are part of product properties in the
    ///     REST API.
    /// </summary>
    /// <remarks>The main application equivalent is ProductPropertyChoice.</remarks>
    [DataContract]
    [Serializable]
    public class ProductPropertyChoiceDTO
    {
        public ProductPropertyChoiceDTO()
        {
            Id = 0;
            StoreId = 0;
            PropertyId = 0;
            ChoiceName = string.Empty;
            SortOrder = 0;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        ///     This is the unique ID or primary key of the choice.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID of the product property that this choice is assigned to.
        /// </summary>
        [DataMember]
        public long PropertyId { get; set; }

        /// <summary>
        ///     This is the system or localization-safe name of the choice.
        /// </summary>
        [DataMember]
        public string ChoiceName { get; set; }

        /// <summary>
        ///     When localization is enabled, this is the language-friendly name that is used over the ChoiceName.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        ///     This is the numeric representation of the order that this choice will appear when listed with the other product
        ///     property choices.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product property was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }
    }
}