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
using System.Collections.Generic;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This class is used as a portable collection of products by the REST API.
    /// </summary>
    [Serializable]
    public class PageOfProducts
    {
        public PageOfProducts()
        {
            Products = new List<ProductDTO>();
            TotalProductCount = 0;
        }

        /// <summary>
        ///     A collection of products that matched a query, but for a specific page in the results.
        /// </summary>
        public List<ProductDTO> Products { get; set; }

        /// <summary>
        ///     An integer value representing the total number of products in the query which is used to ask for other pages of
        ///     products.
        /// </summary>
        public int TotalProductCount { get; set; }
    }
}