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
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Product Relationships
    /// </summary>
    /// <remarks>The REST API equivalent is ProductRelationshipDTO.</remarks>
    [Serializable]
    public class ProductRelationship
    {
        public ProductRelationship()
        {
            Id = 0;
            StoreId = 0;
            ProductId = string.Empty;
            RelatedProductId = string.Empty;
            IsSubstitute = false;
            SortOrder = 0;
            MarketingDescription = string.Empty;
        }

        /// <summary>
        ///     The unique ID or primary key of the product relationship.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID or Bvin of the primary product.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        ///     The unique ID or Bvin of the related product.
        /// </summary>
        public string RelatedProductId { get; set; }

        /// <summary>
        ///     Allows the related product to replace the primary product.
        /// </summary>
        /// <remarks>This property is not used in the application, but False should be saved for all products.</remarks>
        public bool IsSubstitute { get; set; }

        /// <summary>
        ///     Used to sort the related products for display to customers.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        ///     This property is not used in the application.
        /// </summary>
        /// <remarks>This property is not used in the application.</remarks>
        public string MarketingDescription { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product relationship object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductRelationshipDTO</returns>
        public ProductRelationshipDTO ToDto()
        {
            var dto = new ProductRelationshipDTO();

            dto.Id = Id;
            dto.StoreId = StoreId;
            dto.ProductId = ProductId;
            dto.RelatedProductId = RelatedProductId;
            dto.IsSubstitute = IsSubstitute;
            dto.SortOrder = SortOrder;
            dto.MarketingDescription = MarketingDescription;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product relationship object using a ProductRelationshipDTO instance
        /// </summary>
        /// <param name="dto">An instance of the product relationship from the REST API</param>
        public void FromDto(ProductRelationshipDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            StoreId = dto.StoreId;
            ProductId = dto.ProductId;
            RelatedProductId = dto.RelatedProductId;
            IsSubstitute = dto.IsSubstitute;
            SortOrder = dto.SortOrder;
            MarketingDescription = dto.MarketingDescription;
        }

        #endregion
    }
}