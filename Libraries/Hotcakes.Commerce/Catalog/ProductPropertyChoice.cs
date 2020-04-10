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
    ///     This is the primary object that is used to manage all aspects of choices that are part of product properties.
    /// </summary>
    /// <remarks>The REST API equivalent is ProductPropertyChoiceDTO.</remarks>
    [Serializable]
    public class ProductPropertyChoice
    {
        public ProductPropertyChoice()
        {
            Id = 0;
            StoreId = 0;
            ChoiceName = string.Empty;
            SortOrder = 0;
            LastUpdated = DateTime.UtcNow;
        }

        /// <summary>
        ///     This is the unique ID or primary key of the choice.
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID of the product property that this choice is assigned to.
        /// </summary>
        public long PropertyId { get; set; }

        /// <summary>
        ///     This is the system or localization-safe name of the choice.
        /// </summary>
        public string ChoiceName { get; set; }

        /// <summary>
        ///     When localization is enabled, this is the language-friendly name that is used over the ChoiceName.
        /// </summary>
        public string DisplayName { get; set; }

        /// <summary>
        ///     This is the numeric representation of the order that this choice will appear when listed with the other product
        ///     property choices.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the product property was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product property object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of ProductPropertyDTO</returns>
        public ProductPropertyChoiceDTO ToDto()
        {
            var dto = new ProductPropertyChoiceDTO();

            dto.ChoiceName = ChoiceName;
            dto.DisplayName = DisplayName;
            dto.Id = Id;
            dto.LastUpdated = LastUpdated;
            dto.PropertyId = PropertyId;
            dto.SortOrder = SortOrder;
            dto.StoreId = StoreId;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product property object using a ProductPropertyDTO instance
        /// </summary>
        /// <param name="dto">An instance of the ProductProperty from the REST API</param>
        public void FromDto(ProductPropertyChoiceDTO dto)
        {
            if (dto == null) return;

            ChoiceName = dto.ChoiceName ?? string.Empty;
            DisplayName = dto.DisplayName;
            Id = dto.Id;
            LastUpdated = dto.LastUpdated;
            PropertyId = dto.PropertyId;
            SortOrder = dto.SortOrder;
            StoreId = dto.StoreId;
        }

        #endregion
    }
}