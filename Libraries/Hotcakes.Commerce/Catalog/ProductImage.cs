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
    [Serializable]
    public class ProductImage
    {
        public ProductImage()
        {
            Bvin = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            ProductId = string.Empty;
            FileName = string.Empty;
            Caption = string.Empty;
            AlternateText = string.Empty;
            SortOrder = -1;
            StoreId = 0;
        }

        public string Bvin { get; set; }
        public DateTime LastUpdatedUtc { get; set; }
        public string ProductId { get; set; }
        public string FileName { get; set; }
        public string Caption { get; set; }
        public string AlternateText { get; set; }
        public int SortOrder { get; set; }
        public long StoreId { get; set; }

        public ProductImage Clone()
        {
            var result = new ProductImage();

            result.AlternateText = AlternateText;
            result.Bvin = string.Empty;
            result.Caption = Caption;
            result.FileName = FileName;
            result.LastUpdatedUtc = LastUpdatedUtc;
            result.ProductId = ProductId;
            result.SortOrder = SortOrder;
            result.StoreId = StoreId;

            return result;
        }

        // DTO
        public ProductImageDTO ToDto()
        {
            var dto = new ProductImageDTO();

            dto.AlternateText = AlternateText;
            dto.Bvin = Bvin;
            dto.Caption = Caption;
            dto.FileName = FileName;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.ProductId = ProductId;
            dto.SortOrder = SortOrder;
            dto.StoreId = StoreId;

            return dto;
        }

        public void FromDto(ProductImageDTO dto)
        {
            if (dto == null) return;

            AlternateText = dto.AlternateText ?? string.Empty;
            Bvin = dto.Bvin ?? string.Empty;
            Caption = dto.Caption ?? string.Empty;
            FileName = dto.FileName ?? string.Empty;
            LastUpdatedUtc = dto.LastUpdatedUtc;
            ProductId = dto.ProductId ?? string.Empty;
            SortOrder = dto.SortOrder;
            StoreId = dto.StoreId;
        }
    }
}