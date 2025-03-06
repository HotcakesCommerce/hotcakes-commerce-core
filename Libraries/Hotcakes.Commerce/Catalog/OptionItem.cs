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
    ///     This is the primary object that is used to manage all aspects of product option items in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is OptionItemDTO.</remarks>
    [Serializable]
    public class OptionItem
    {
        public OptionItem()
        {
            Bvin = string.Empty;
            StoreId = 0;
            OptionBvin = string.Empty;
            Name = string.Empty;
            PriceAdjustment = 0;
            WeightAdjustment = 0;
            IsLabel = false;
            SortOrder = 0;
            IsDefault = false;
        }

        /// <summary>
        ///     The unique ID or primary key of the current product option item.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     This value refers to the Option ID that this item belongs to.
        /// </summary>
        public string OptionBvin { get; set; }

        /// <summary>
        ///     This is that localized name of the product option item as customers will see it.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     If this item belongs to a variant, this value will be the new price when this product option item is selected.
        /// </summary>
        public decimal PriceAdjustment { get; set; }

        /// <summary>
        ///     If this item belongs to a variant, this value will be the new weight when this product option item is selected.
        /// </summary>
        public decimal WeightAdjustment { get; set; }

        /// <summary>
        ///     Used with the drop down list OptionType, this item will be rendered differently if true. All other OptionTypes will
        ///     ignore this item unless this value is false.
        /// </summary>
        public bool IsLabel { get; set; }

        /// <summary>
        ///     This value is used to sort this item when grouped with other items.
        /// </summary>
        public int SortOrder { get; set; }

        /// <summary>
        ///     When True, this item will be used as the default item over other items that might be associated with the same
        ///     product option.
        /// </summary>
        public bool IsDefault { get; set; }

        /// <summary>
        ///     Use this method to create a copy of the current option item.
        /// </summary>
        /// <returns>A duplicate of the current OptionItem.</returns>
        public OptionItem Clone()
        {
            var result = new OptionItem();

            result.Bvin = string.Empty;
            result.IsLabel = IsLabel;
            result.Name = Name;
            result.OptionBvin = OptionBvin;
            result.PriceAdjustment = PriceAdjustment;
            result.SortOrder = SortOrder;
            result.StoreId = StoreId;
            result.WeightAdjustment = WeightAdjustment;
            result.IsDefault = IsDefault;
            return result;
        }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current product option item object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OptionItemDTO</returns>
        public OptionItemDTO ToDto()
        {
            var dto = new OptionItemDTO();

            dto.Bvin = Bvin;
            dto.IsLabel = IsLabel;
            dto.Name = Name;
            dto.OptionBvin = OptionBvin;
            dto.PriceAdjustment = PriceAdjustment;
            dto.SortOrder = SortOrder;
            dto.StoreId = StoreId;
            dto.WeightAdjustment = WeightAdjustment;
            dto.IsDefault = IsDefault;
            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current product option item object using an instance of OptionItemDTO
        /// </summary>
        /// <param name="dto">An instance of the product option item from the REST API</param>
        public void FromDto(OptionItemDTO dto)
        {
            if (dto == null) return;

            Bvin = dto.Bvin ?? string.Empty;
            IsLabel = dto.IsLabel;
            Name = dto.Name ?? string.Empty;
            OptionBvin = dto.OptionBvin ?? string.Empty;
            PriceAdjustment = dto.PriceAdjustment;
            SortOrder = dto.SortOrder;
            StoreId = dto.StoreId;
            WeightAdjustment = dto.WeightAdjustment;
            IsDefault = dto.IsDefault;
        }

        #endregion
    }
}