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
    ///     This is the primary class used for wish list items in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is WishListItemDTO.</remarks>
    [Serializable]
    public class WishListItem
    {
        public WishListItem()
        {
            Init();
        }

        /// <summary>
        ///     Defines the user account that the wish list item belongs to.
        /// </summary>
        public string CustomerId { get; set; }

        /// <summary>
        /// The unique ID or primary key of the wish list item.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// The last updated date is used for auditing purposes to know when the wish list item was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        /// The unique ID of the product that this wish list item refers to.
        /// </summary>
        public string ProductId { get; set; }

        /// <summary>
        /// Displays any selected choices made by the customer when saving the product to the wish list.
        /// </summary>
        public string ProductShortDescription { get; set; }

        /// <summary>
        /// The amount of products that this wish list item is saving.
        /// </summary>
        public int Quantity { get; set; }

        /// <summary>
        /// When applicable, the choices or options for  the saved product.
        /// </summary>
        public OptionSelections SelectionData { get; set; }

        /// <summary>
        /// This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        private void Init()
        {
            Id = 0;
            StoreId = 0;
            CustomerId = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            ProductId = string.Empty;
            Quantity = 1;
            SelectionData = new OptionSelections();
            ProductShortDescription = string.Empty;
        }

        /// <summary>
        ///     Returns an instance of the Product object using the ProductId property.
        /// </summary>
        /// <param name="app">An instance of the hotcakes application</param>
        /// <returns>Product - an instance of the Product object</returns>
        public Product GetAssociatedProduct(HotcakesApplication app)
        {
            return app.CatalogServices.Products.FindWithCache(ProductId);
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current wish list item object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of WishListItemDTO</returns>
        public WishListItemDTO ToDto()
        {
            var dto = new WishListItemDTO();
            dto.Id = Id;
            dto.StoreId = StoreId;
            dto.CustomerId = CustomerId;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.ProductId = ProductId ?? string.Empty;
            dto.Quantity = Quantity;
            foreach (var op in SelectionData.OptionSelectionList)
            {
                dto.SelectionData.Add(op.ToDto());
            }
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current wish list item object using a WishListItemDTO instance
        /// </summary>
        /// <param name="dto">An instance of the wish list item from the REST API</param>
        public void FromDto(WishListItemDTO dto)
        {
            if (dto == null) return;

            Id = dto.Id;
            StoreId = dto.StoreId;
            CustomerId = dto.CustomerId;
            LastUpdatedUtc = dto.LastUpdatedUtc;
            ProductId = dto.ProductId ?? string.Empty;
            Quantity = dto.Quantity;
            SelectionData.Clear();
            if (dto.SelectionData != null)
            {
                foreach (var op in dto.SelectionData)
                {
                    var o = new OptionSelection();
                    o.FromDto(op);
                    SelectionData.OptionSelectionList.Add(o);
                }
            }
        }

        #endregion
    }
}