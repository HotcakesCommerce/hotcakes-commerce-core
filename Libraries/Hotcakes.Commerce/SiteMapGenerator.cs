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

using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;

namespace Hotcakes.Commerce
{
    public class SiteMapGenerator
    {
        public static string BuildForStore(HotcakesApplication app)
        {
            if (app == null) return string.Empty;

            var root = app.CurrentStore.RootUrl();
            var rootNode = new SiteMapNode();

            // home
            rootNode.AddUrl(root);
            // sitemap
            rootNode.AddUrl(root + "sitemap");

            // Categories
            foreach (var cat in app.CatalogServices.Categories.FindAll())
            {
                var caturl = UrlRewriter.BuildUrlForCategory(cat);

                // Skip Pages with Outbound links as they aren't supported in sitemap format
                var temp = caturl.ToUpperInvariant();
                if (temp.StartsWith("HTTP:") || temp.StartsWith("HTTPS:")) continue;

                rootNode.AddUrl(root.TrimEnd('/') + caturl);
            }

            // Products
            foreach (var p in app.CatalogServices.Products.FindAllPagedWithCache(1, 3000))
            {
                var produrl = UrlRewriter.BuildUrlForProduct(p);
                rootNode.AddUrl(root.TrimEnd('/') + produrl);
            }
            return rootNode.RenderAsXmlSiteMap();
        }
    }
}