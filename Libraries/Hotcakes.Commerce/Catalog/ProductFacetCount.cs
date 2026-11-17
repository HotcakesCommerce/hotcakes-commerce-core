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

using System.Collections.Generic;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Gets or sets the product facet count.
    /// </summary>
    public class ProductFacetCount
    {
        /// <summary>
        /// Provides the catalog type or contract represented by this declaration.
        /// </summary>
        public ProductFacetCount()
        {
            Key = string.Empty;
            ProductCount = 0;
        }

        /// <summary>
        /// Gets or sets the key.
        /// </summary>
        public string Key { get; set; }
        /// <summary>
        /// Gets or sets the product count.
        /// </summary>
        public int ProductCount { get; set; }

        /// <summary>
        /// Performs the FindCountForKey catalog operation.
        /// </summary>
        public static int FindCountForKey(string key, List<ProductFacetCount> counts)
        {
            var sqlKey = CategoryFacetKeyHelper.ParseKeyToSqlList(key);
            return FindCountForSqlKey(sqlKey, counts);
        }

        /// <summary>
        /// Performs the FindCountForSqlKey catalog operation.
        /// </summary>
        public static int FindCountForSqlKey(string sqlKey, List<ProductFacetCount> counts)
        {
            foreach (var c in counts)
            {
                if (c.Key == sqlKey)
                {
                    return c.ProductCount;
                }
            }

            return 0;
        }
    }
}
