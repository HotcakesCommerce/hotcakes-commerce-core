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
    ///     This is the primary object that is used to manage all aspects of ProductType in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is ProductType.</remarks>
    [DataContract]
    [Serializable]
    public class ProductTypeDTO
    {
        public ProductTypeDTO()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            IsPermanent = false;
            ProductTypeName = string.Empty;
            TemplateName = string.Empty;
        }

        /// <summary>
        ///     The unique ID or primary key of the product type.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product property was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     This property is used to tell the main application when a product type is a core feature.
        /// </summary>
        /// <remarks>If true, the product type is a built-in product type.</remarks>
        [DataMember]
        public bool IsPermanent { get; set; }

        /// <summary>
        ///     The name used in the merchant application to label the products that will be using this product type.
        /// </summary>
        [DataMember]
        public string ProductTypeName { get; set; }

        /// <summary>
        ///     When used, this property allows you to set a high-level default template for products of this type to use.
        /// </summary>
        [DataMember]
        public string TemplateName { get; set; }
    }
}