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

using System.Collections.Generic;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce
{
    public class PersonalizationServices
    {
        private PersonalizationServices()
        {
        }

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