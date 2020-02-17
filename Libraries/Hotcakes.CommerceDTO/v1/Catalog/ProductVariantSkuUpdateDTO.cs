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
using System.Collections.Generic;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product variant options in the REST API.
    /// </summary>
    [Serializable]
    public class VariantOptionDataDTO
    {
        public VariantOptionDataDTO()
        {
            ChoiceId = string.Empty;
            ChoiceItemId = string.Empty;
        }

        /// <summary>
        ///     The unique ID or Bvin of this option.
        /// </summary>
        public string ChoiceId { get; set; }

        /// <summary>
        ///     The unique ID of the selected choice in this option.
        /// </summary>
        public string ChoiceItemId { get; set; }
    }

    /// <summary>
    ///     This is the primary object that is used to manage all aspects of product variant SKU updates in the REST API.
    /// </summary>
    [Serializable]
    public class ProductVariantSkuUpdateDTO
    {
        public ProductVariantSkuUpdateDTO()
        {
            ProductBvin = string.Empty;
            Sku = string.Empty;
            MatchingOptions = new List<VariantOptionDataDTO>();
        }

        /// <summary>
        ///     The unique ID or Bvin of an existing product.
        /// </summary>
        public string ProductBvin { get; set; }

        /// <summary>
        ///     The new SKU that you wish for the variant to have.
        /// </summary>
        public string Sku { get; set; }

        /// <summary>
        ///     A collection of choices (options) and their ID's
        /// </summary>
        public List<VariantOptionDataDTO> MatchingOptions { get; set; }
    }
}