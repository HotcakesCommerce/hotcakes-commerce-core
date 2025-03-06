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
    ///     This is the primary class used for category product associations in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is CategoryProductAssociationDTO.</remarks>
    [Serializable]
    public class CategoryProductAssociation
    {
        public CategoryProductAssociation()
        {
            Id = 0;
            CategoryId = string.Empty;
            ProductId = string.Empty;
            SortOrder = 0;
            StoreId = 0;
        }

        /// <summary>
        ///     The unique ID or primary key of the category product association.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     The unique ID of the category to associate to the product.
        /// </summary>
        public string CategoryId { get; set; }

        /// <summary>
        ///     The unique ID of the product to associate to the category.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        ///     Defines the order in which the associations appear. This is populated for you upon creation.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current category product association object to the DTO equivalent for use with the REST
        ///     API
        /// </summary>
        /// <returns>A new instance of CategoryProductAssociationDTO</returns>
        public CategoryProductAssociationDTO ToDto()
        {
            var dto = new CategoryProductAssociationDTO();

            dto.Id = Id;
            dto.CategoryId = CategoryId;
            dto.ProductId = ProductId;
            dto.SortOrder = SortOrder;
            dto.StoreId = StoreId;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current category product association object using a CategoryProductAssociationDTO
        ///     instance
        /// </summary>
        /// <param name="dto">An instance of the category product association from the REST API</param>
        public void FromDto(CategoryProductAssociationDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            CategoryId = dto.CategoryId;
            ProductId = dto.ProductId;
            SortOrder = dto.SortOrder;
            StoreId = dto.StoreId;
        }

        #endregion
    }
}