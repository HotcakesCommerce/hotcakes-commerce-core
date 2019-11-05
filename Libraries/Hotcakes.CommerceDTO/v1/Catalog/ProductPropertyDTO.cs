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
    ///     This is the primary object that is used to manage all aspects of ProductProperties in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is ProductProperty.</remarks>
    [DataContract]
    [Serializable]
    public class ProductPropertyDTO
    {
        public ProductPropertyDTO()
        {
            Id = 0;
            StoreId = 0;
            PropertyName = string.Empty;
            DisplayName = string.Empty;
            DisplayOnSite = true;
            DisplayToDropShipper = true;
            TypeCode = ProductPropertyTypeDTO.None;
            DefaultValue = string.Empty;
            CultureCode = "en-US";
            Choices = new List<ProductPropertyChoiceDTO>();
            LastUpdatedUtc = DateTime.UtcNow;
        }

        /// <summary>
        ///     The unique ID or identifier/primary key of the product property.
        /// </summary>
        [DataMember]
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The globally used system name for the product property.
        /// </summary>
        [DataMember]
        public string PropertyName { get; set; }

        /// <summary>
        ///     The name used on localized sites to recognize the product property in the native or chosen language.
        /// </summary>
        [DataMember]
        public string DisplayName { get; set; }

        /// <summary>
        ///     A setting used to determine whether or not this product property is displayed on the site to customers.
        /// </summary>
        [DataMember]
        public bool DisplayOnSite { get; set; }

        /// <summary>
        ///     A setting used to determine whether or not this product property is displayed to drop shippers.
        /// </summary>
        [DataMember]
        public bool DisplayToDropShipper { get; set; }

        /// <summary>
        ///     The type of property that this product property is, which helps determine how it's displayed to merchants and
        ///     customers.
        /// </summary>
        [DataMember]
        public ProductPropertyTypeDTO TypeCode { get; set; }

        /// <summary>
        ///     This is the default value that will first be used in products, until the product has been updated to have another
        ///     value.
        /// </summary>
        [DataMember]
        public string DefaultValue { get; set; }

        /// <summary>
        ///     This property reflects the language that this product property has been saved for.
        /// </summary>
        [DataMember]
        public string CultureCode { get; set; }

        /// <summary>
        ///     A collection of choices that belong to this product property.
        /// </summary>
        /// <remarks>This property only applies when the TypeCode property is MultipleChoiceField (2).</remarks>
        [DataMember]
        public List<ProductPropertyChoiceDTO> Choices { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product property was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }
    }
}