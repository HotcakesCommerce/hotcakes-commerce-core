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
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of ProductType in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is ProductTypeDTO.</remarks>
    [Serializable]
    public class ProductType
    {
        public ProductType()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdated = DateTime.UtcNow;
            ProductTypeName = string.Empty;
            IsPermanent = false;
            TemplateName = string.Empty;
        }

        /// <summary>
        ///     The unique ID or primary key of the product type.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This property is used to tell the main application when a product type is a core feature.
        /// </summary>
        /// <remarks>If true, the product type is a built-in product type.</remarks>
        public bool IsPermanent { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product property was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        ///     The name used in the merchant application to label the products that will be using this product type.
        /// </summary>
        public string ProductTypeName { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     When used, this property allows you to set a high-level default template for products of this type to use.
        /// </summary>
        public string TemplateName { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product type object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductTypeDTO</returns>
        public ProductTypeDTO ToDto()
        {
            var dto = new ProductTypeDTO();

            dto.Bvin = Bvin;
            dto.StoreId = StoreId;
            dto.IsPermanent = IsPermanent;
            dto.LastUpdated = LastUpdated;
            dto.ProductTypeName = ProductTypeName;
            dto.TemplateName = TemplateName;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product type object using a ProductTypeDTO instance
        /// </summary>
        /// <param name="dto">An instance of the ProductType from the REST API</param>
        public void FromDto(ProductTypeDTO dto)
        {
            if (dto == null) return;

            Bvin = dto.Bvin ?? string.Empty;
            StoreId = dto.StoreId;
            IsPermanent = dto.IsPermanent;
            LastUpdated = dto.LastUpdated;
            ProductTypeName = dto.ProductTypeName ?? string.Empty;
            TemplateName = dto.TemplateName ?? string.Empty;
        }

        #endregion
    }
}