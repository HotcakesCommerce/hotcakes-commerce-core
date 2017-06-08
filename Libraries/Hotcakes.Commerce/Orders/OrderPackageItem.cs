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
using Hotcakes.CommerceDTO.v1.Orders;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of package items in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is OrderPackageItemDTO.</remarks>
    [Serializable]
    public class OrderPackageItem
    {
        public OrderPackageItem()
        {
            ProductBvin = string.Empty;
            LineItemId = 0;
            Quantity = 0;
        }

        public OrderPackageItem(string bvin, long itemId, int qty)
        {
            ProductBvin = bvin;
            LineItemId = itemId;
            Quantity = qty;
        }

        /// <summary>
        ///     The unique ID or bvin of the product that is in this package.
        /// </summary>
        public string ProductBvin { get; set; }

        /// <summary>
        ///     The unique ID of the line item that matches this package.
        /// </summary>
        public long LineItemId { get; set; }

        /// <summary>
        ///     Total number of products that are in this package.
        /// </summary>
        public int Quantity { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current package item object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderPackageItemDTO</returns>
        public OrderPackageItemDTO ToDto()
        {
            var dto = new OrderPackageItemDTO();

            dto.LineItemId = LineItemId;
            dto.ProductBvin = ProductBvin ?? string.Empty;
            dto.Quantity = Quantity;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current package item object using an OrderPackageItemDTO instance
        /// </summary>
        /// <param name="dto">An instance of the package item from the REST API</param>
        public void FromDto(OrderPackageItemDTO dto)
        {
            if (dto == null) return;

            LineItemId = dto.LineItemId;
            ProductBvin = dto.ProductBvin ?? string.Empty;
            Quantity = dto.Quantity;
        }

        #endregion
    }
}