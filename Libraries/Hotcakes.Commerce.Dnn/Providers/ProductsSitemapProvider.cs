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
using DotNetNuke.Entities.Portals;
using DotNetNuke.Services.Sitemap;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Dnn.Providers
{
    [Serializable]
    internal class ProductsSitemapProvider : SitemapProvider
    {
        public override List<SitemapUrl> GetUrls(int portalId, PortalSettings ps, string version)
        {
            var productsRepo = Factory.CreateRepo<ProductRepository>();
            var products = productsRepo.FindAllPagedWithCache(1, int.MaxValue);

            var urls = new List<SitemapUrl>();
            SitemapUrl pageUrl = null;

            if (HccRequestContext.Current != null) // Localize HccRequestContext
                HccRequestContext.Current = HccRequestContextUtils.GetContextWithCulture(HccRequestContext.Current, ps.CultureCode);

            foreach (var product in products)
            {
                pageUrl = GetPageUrl(product);
                urls.Add(pageUrl);
            }

            return urls;
        }

        private SitemapUrl GetPageUrl(Product product)
        {
            var pageUrl = new SitemapUrl();
            pageUrl.Url = HccUrlBuilder.RouteHccUrl(HccRoute.Product, new {slug = product.UrlSlug});
            pageUrl.Priority = 0.5F;
            pageUrl.LastModified = product.LastUpdated;
            pageUrl.ChangeFrequency = SitemapChangeFrequency.Daily;

            return pageUrl;
        }
    }
}