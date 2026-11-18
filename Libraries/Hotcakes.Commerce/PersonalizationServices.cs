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
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce
{
    /// <summary>
    /// Provides cookie-backed personalization helpers for recently viewed products.
    /// </summary>
    /// <remarks>The recent-product cookie stores product identifiers in most-recent-first order and is limited to ten entries.</remarks>
    public class PersonalizationServices
    {
        private PersonalizationServices()
        {
        }

        /// <summary>
        /// Records a product identifier in the recent-products cookie.
        /// </summary>
        /// <param name="bvin">The product identifier to place first in the recent-products list.</param>
        /// <param name="app">The current application context; retained for API compatibility and not used by this operation.</param>
        /// <remarks>
        /// The cookie name comes from <see cref="WebAppSettings.CookieNameLastProductsViewed"/>. Existing identifiers are
        /// de-duplicated, the new identifier is placed first, and at most ten identifiers are retained. When the configured
        /// cookie name is empty, no cookie is written.
        /// </remarks>
        public static void RecordProductViews(string bvin, HotcakesApplication app)
        {
            if (WebAppSettings.CookieNameLastProductsViewed != string.Empty)
            {
                var SavedProductIDs = SessionManager.GetCookieString(WebAppSettings.CookieNameLastProductsViewed);
                if (SavedProductIDs != string.Empty)
                {
                    var AllIDs = SavedProductIDs.Split(',');
                    var q = new Queue<string>();
                    q.Enqueue(bvin);
                    foreach (var id in AllIDs)
                    {
                        if (q.Count < 10)
                        {
                            if (!q.Contains(id))
                            {
                                q.Enqueue(id);
                            }
                        }
                    }
                    SessionManager.SetCookieString(WebAppSettings.CookieNameLastProductsViewed,
                        string.Join(",", q.ToArray()));
                }
                else
                {
                    SessionManager.SetCookieString(WebAppSettings.CookieNameLastProductsViewed, bvin);
                }
            }
        }

        /// <summary>
        /// Gets products whose identifiers are stored in the recent-products cookie.
        /// </summary>
        /// <param name="app">The application whose catalog service resolves the stored product identifiers.</param>
        /// <returns>A list resolved through the catalog cache, or an empty list when the cookie has no value.</returns>
        /// <remarks>The cookie order is passed to the catalog lookup, and the recent-products list is limited to ten identifiers by <see cref="RecordProductViews"/>.</remarks>
        public static List<Product> GetProductsViewed(HotcakesApplication app)
        {
            var SavedProductIDs = SessionManager.GetCookieString(WebAppSettings.CookieNameLastProductsViewed);
            var result = new List<Product>();
            if (SavedProductIDs != string.Empty)
            {
                var AllIDs = SavedProductIDs.Split(',');
                var ids = new List<string>();
                foreach (var id in AllIDs)
                {
                    ids.Add(id);
                }
                result = app.CatalogServices.Products.FindManyWithCache(ids);
            }
            return result;
        }
    }
}
