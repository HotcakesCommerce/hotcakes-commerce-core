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

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Product Relationships in the REST API
    /// </summary>
    /// <remarks>The main application equivalent is ProductRelationship.</remarks>
    [Serializable]
    public class ProductRelationshipDTO
    {
        public ProductRelationshipDTO()
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
    }
}