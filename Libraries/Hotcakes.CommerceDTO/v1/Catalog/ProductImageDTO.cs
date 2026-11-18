#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
    ///     Represents an image associated with a catalog product, including its display text and ordering.
    /// </summary>
    [DataContract]
    [Serializable]
    public class ProductImageDTO
    {
        /// <summary>
        ///     Initializes a new instance of the <see cref="ProductImageDTO"/> type with its default DTO values.
        /// </summary>
        public ProductImageDTO()
        {
            Bvin = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            ProductId = string.Empty;
            FileName = string.Empty;
            Caption = string.Empty;
            AlternateText = string.Empty;
            SortOrder = 0;
            StoreId = 0;
        }

        /// <summary>
        ///     The unique identifier of the product image.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The UTC date and time when the product image was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique identifier of the product to which the image belongs.
        /// </summary>
        [DataMember]
        public string ProductId { get; set; }

        /// <summary>
        ///     The image file name used to render the product image.
        /// </summary>
        [DataMember]
        public string FileName { get; set; }

        /// <summary>
        ///     The caption associated with the product image.
        /// </summary>
        [DataMember]
        public string Caption { get; set; }

        /// <summary>
        ///     Alternative text for the product image.
        /// </summary>
        [DataMember]
        public string AlternateText { get; set; }

        /// <summary>
        ///     The display order of the image relative to other product images.
        /// </summary>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        ///     The identifier of the Hotcakes store that owns the product image.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }
    }
}
