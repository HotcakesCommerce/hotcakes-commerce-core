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
using System.Collections.Generic;
using System.Dynamic;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Provides helpers for validating and generating Hotcakes Commerce URLs.
    /// </summary>
    /// <remarks>URL generation delegates route construction to <see cref="HccUrlBuilder"/> and the configured HCC routes.</remarks>
    public class UrlRewriter
    {
        private const string CART_ROUTE_FORMAT = "{0}?AddSku={1}&AddSkuQty=1";

        /// <summary>Determines whether a product slug belongs to a different product.</summary>
        /// <param name="slug">The product URL slug to find.</param>
        /// <param name="bvin">The product identifier that is allowed to retain the slug.</param>
        /// <param name="app">The application whose catalog repository is queried.</param>
        /// <returns><see langword="true"/> when another product uses the slug; otherwise, <see langword="false"/>.</returns>
        public static bool IsProductSlugInUse(string slug, string bvin, HotcakesApplication app)
        {
            var p = app.CatalogServices.Products.FindBySlug(slug);
            if (p != null)
            {
                if (p.Bvin != string.Empty)
                {
                    if (p.Bvin != bvin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Determines whether a category slug belongs to a different category.</summary>
        /// <param name="slug">The category URL slug to find.</param>
        /// <param name="bvin">The category identifier that is allowed to retain the slug.</param>
        /// <param name="app">The application providing the category repository.</param>
        /// <returns><see langword="true"/> when another category uses the slug; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="IsCategorySlugInUse(string, string, ICategoryRepository)"/>
        public static bool IsCategorySlugInUse(string slug, string bvin, HotcakesApplication app)
        {
            return IsCategorySlugInUse(slug, bvin, app.CatalogServices.Categories);
        }

        /// <summary>Determines whether a category repository contains a different category with a slug.</summary>
        /// <param name="slug">The category URL slug to find.</param>
        /// <param name="bvin">The category identifier that is allowed to retain the slug.</param>
        /// <param name="repository">The category repository to query.</param>
        /// <returns><see langword="true"/> when another category uses the slug; otherwise, <see langword="false"/>.</returns>
        public static bool IsCategorySlugInUse(string slug, string bvin, ICategoryRepository repository)
        {
            var c = repository.FindBySlug(slug);
            if (c != null)
            {
                if (c.Bvin != string.Empty)
                {
                    if (c.Bvin != bvin)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        /// <summary>Determines whether a requested URL conflicts with a category, product, or custom URL.</summary>
        /// <param name="requestedUrl">The requested URL or slug to check.</param>
        /// <param name="thisCustomUrlBvin">The current custom URL identifier, which is excluded from the conflict.</param>
        /// <param name="context">The request context associated with the URL check.</param>
        /// <param name="app">The application whose catalog and content repositories are queried.</param>
        /// <returns><see langword="true"/> when the URL is already used by another record; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The category and product checks use a lower-case working value; custom URL lookup uses the original input.</remarks>
        public static bool IsUrlInUse(string requestedUrl, string thisCustomUrlBvin, HccRequestContext context,
            HotcakesApplication app)
        {
            var result = false;
            var working = requestedUrl.ToLowerInvariant();

            // Check for Generic Page Use in a Flex Page
            if (IsCategorySlugInUse(working, thisCustomUrlBvin, app)) return true;

            // Check for Products
            if (IsProductSlugInUse(working, thisCustomUrlBvin, app)) return true;

            // Check Custom Urls
            var url = app.ContentServices.CustomUrls.FindByRequestedUrl(requestedUrl);
            if (url != null)
            {
                if (url.Bvin != string.Empty)
                {
                    if (url.Bvin != thisCustomUrlBvin)
                    {
                        return true;
                    }
                }
            }
            return result;
        }

        /// <summary>Prefixes a custom URL with an application path when the application is not rooted at <c>/</c>.</summary>
        /// <param name="customUrl">The custom URL to prefix.</param>
        /// <param name="appPath">The application path, including or excluding a trailing slash.</param>
        /// <returns>The custom URL made relative to the application path.</returns>
        public static string MakeRelativeCustomUrlSafeForApp(string customUrl, string appPath)
        {
            var result = customUrl;

            var testAppPath = appPath.TrimEnd('/');

            if (appPath != "/")
            {
                if (testAppPath.Length > 0)
                {
                    result = testAppPath + customUrl;
                }
            }

            return result;
        }

        /// <summary>Builds a route URL for a category, or returns its custom link when it has one.</summary>
        /// <param name="c">The category snapshot to represent.</param>
        /// <param name="additionalParams">Optional route values merged after the category slug.</param>
        /// <returns>The category custom link or the generated HCC category route URL.</returns>
        public static string BuildUrlForCategory(CategorySnapshot c, object additionalParams = null)
        {
            if (c.SourceType == CategorySourceType.CustomLink)
            {
                if (c.CustomPageUrl != string.Empty)
                    return c.CustomPageUrl;
            }

            var parameters = Merge(new {slug = c.RewriteUrl}, additionalParams);
            var routeValues = new RouteValueDictionary(parameters);
            return HccUrlBuilder.RouteHccUrl(HccRoute.Category, routeValues);
        }

        /// <summary>Builds a route URL for a category page, or returns its custom link when it has one.</summary>
        /// <param name="c">The category snapshot to represent.</param>
        /// <param name="pageNumber">The page value added to the route.</param>
        /// <param name="additionalParams">Optional route values merged after the category slug and page.</param>
        /// <returns>The category custom link or the generated paged HCC category route URL.</returns>
        public static string BuildUrlForCategory(CategorySnapshot c, string pageNumber, object additionalParams = null)
        {
            if (c.SourceType == CategorySourceType.CustomLink)
            {
                if (c.CustomPageUrl != string.Empty)
                    return c.CustomPageUrl;
            }

            var parameters = Merge(new {slug = c.RewriteUrl, page = pageNumber}, additionalParams);
            var routeValues = new RouteValueDictionary(parameters);
            return HccUrlBuilder.RouteHccUrl(HccRoute.Category, routeValues);
        }

        /// <summary>Builds an HCC product route URL from a product slug.</summary>
        /// <param name="p">The product to represent.</param>
        /// <param name="additionalParams">Optional route values merged after the product slug.</param>
        /// <returns>The generated product route URL.</returns>
        public static string BuildUrlForProduct(Product p, object additionalParams = null)
        {
            var parameters = Merge(new {slug = p.UrlSlug}, additionalParams);
            var routeValues = new RouteValueDictionary(parameters);
            return HccUrlBuilder.RouteHccUrl(HccRoute.Product, routeValues);
        }

        /// <summary>Builds a direct add-to-cart URL for a product without options.</summary>
        /// <param name="p">The product to add.</param>
        /// <returns>A cart URL containing the product SKU and quantity one, or an empty string for products requiring options or gift-card configuration.</returns>
        public static string BuildUrlForProductAddToCart(Product p)
        {
            if (p.HasOptions() || p.IsGiftCard)
            {
                return string.Empty;
            }

            var route = string.Format(CART_ROUTE_FORMAT, HccUrlBuilder.RouteHccUrl(HccRoute.Cart), p.Sku);

            return route;
        }

        private static IDictionary<string, object> Merge(object item1, object item2)
        {
            dynamic expando = new ExpandoObject();
            var result = expando as IDictionary<string, object>;
            if (item1 != null)
            {
                foreach (var fi in item1.GetType().GetProperties())
                {
                    result[fi.Name] = fi.GetValue(item1, null);
                }
            }
            if (item2 != null)
            {
                foreach (var fi in item2.GetType().GetProperties())
                {
                    result[fi.Name] = fi.GetValue(item2, null);
                }
            }
            return result;
        }

        private static string CleanNameForUrl(string input)
        {
            var result = input;

            result = result.Replace(" ", "-");
            result = result.Replace("\"", string.Empty);
            result = result.Replace("&", "and");
            result = result.Replace("?", string.Empty);
            result = result.Replace("=", string.Empty);
            result = result.Replace("/", string.Empty);
            result = result.Replace("\\", string.Empty);
            result = result.Replace("%", string.Empty);
            result = result.Replace("#", string.Empty);
            result = result.Replace("*", string.Empty);
            result = result.Replace("!", string.Empty);
            result = result.Replace("$", string.Empty);
            result = result.Replace("+", "-plus-");
            result = result.Replace(",", "-");
            result = result.Replace("@", "-at-");
            result = result.Replace(":", "-");
            result = result.Replace(";", "-");
            result = result.Replace(">", string.Empty);
            result = result.Replace("<", string.Empty);
            result = result.Replace("{", string.Empty);
            result = result.Replace("}", string.Empty);
            result = result.Replace("~", string.Empty);
            result = result.Replace("|", "-");
            result = result.Replace("^", string.Empty);
            result = result.Replace("[", string.Empty);
            result = result.Replace("]", string.Empty);
            result = result.Replace("`", string.Empty);
            result = result.Replace("'", string.Empty);
            result = result.Replace("�", string.Empty);
            result = result.Replace("�", string.Empty);
            result = result.Replace("�", string.Empty);
            result = result.Replace(".", string.Empty);

            result = HttpUtility.UrlDecode(result);

            return result;
        }

        private static string CleanSkuForUrl(string input)
        {
            var result = input;

            result = result.Replace(" ", "-spc-");
            result = result.Replace("\"", "-quot-");
            result = result.Replace("&", "-and-");
            result = result.Replace("?", "-ques-");
            result = result.Replace("=", "-eql-");
            result = result.Replace("/", "-fslsh-");
            result = result.Replace("\\", "-bslsh-");
            result = result.Replace("%", "-perc-");
            result = result.Replace("#", "-hash-");
            result = result.Replace("*", "-aste-");
            result = result.Replace("!", "-excl-");
            result = result.Replace("$", "-dolr-");
            result = result.Replace("+", "-plus-");
            result = result.Replace(",", "-comm-");
            result = result.Replace("@", "-at-");
            result = result.Replace(":", "-colo-");
            result = result.Replace(";", "-semc-");
            result = result.Replace(">", "-gt-");
            result = result.Replace("<", "-lt-");
            result = result.Replace("{", "-lcb-");
            result = result.Replace("}", "-rcb-");
            result = result.Replace("~", "-til-");
            result = result.Replace("|", "-pip-");
            result = result.Replace("^", "-crt-");
            result = result.Replace("[", "-lsqb-");
            result = result.Replace("]", "-rsqb");
            result = result.Replace("`", "-btck-");
            result = result.Replace("'", "-aps-");
            result = result.Replace("�", "-cpy-");
            result = result.Replace("�", "-tm-");
            result = result.Replace("�", "-rgtm-");
            result = result.Replace(".", "-prd-");

            result = HttpUtility.UrlEncode(result);

            return result;
        }

        private static string GetUnEscapedSku(string input)
        {
            var result = input;

            result = result.Replace("-spc-", " ");
            result = result.Replace("-quot-", "\"");
            result = result.Replace("-and-", "&");
            result = result.Replace("-ques-", "?");
            result = result.Replace("-eql-", "=");
            result = result.Replace("-fslsh-", "/");
            result = result.Replace("-bslsh-", "\\");
            result = result.Replace("-perc-", "%");
            result = result.Replace("-hash-", "#");
            result = result.Replace("-aste-", "*");
            result = result.Replace("-excl-", "!");
            result = result.Replace("-dolr-", "$");
            result = result.Replace("-plus-", "+");
            result = result.Replace("-comm-", ",");
            result = result.Replace("-at-", "@");
            result = result.Replace("-colo-", ":");
            result = result.Replace("-semc-", ";");
            result = result.Replace("-gt-", ">");
            result = result.Replace("-lt-", "<");
            result = result.Replace("-lcb-", "{");
            result = result.Replace("-rcb-", "}");
            result = result.Replace("-til-", "~");
            result = result.Replace("-pip-", "|");
            result = result.Replace("-crt-", "^");
            result = result.Replace("-lsqb-", "[");
            result = result.Replace("-rsqb", "]");
            result = result.Replace("-btck-", "`");
            result = result.Replace("-aps-", "'");
            result = result.Replace("-cpy-", "�");
            result = result.Replace("-tm-", "�");
            result = result.Replace("-rgtm-", "�");
            result = result.Replace("-prd-", ".");

            return result;
        }

        /// <summary>Appends a query string to a URL using the appropriate separator.</summary>
        /// <param name="url">The URL to extend.</param>
        /// <param name="query">The query text without a leading separator.</param>
        /// <returns>The URL followed by <c>?</c> or <c>&amp;</c> and the supplied query.</returns>
        public static string JoinUrlAndQuery(string url, string query)
        {
            if (url.Contains("?"))
            {
                return url + "&" + query;
            }
            return url + "?" + query;
        }

        /// <summary>Builds an absolute URL from the request host, scheme, and raw URL.</summary>
        /// <param name="Request">The HTTP request whose scheme, host, and raw URL are used.</param>
        /// <returns>The request URL reconstructed as an absolute URL.</returns>
        /// <remarks>The raw URL is preserved, including its query text; this method does not canonicalize or validate it.</remarks>
        public static string GetRewrittenUrlFromRequest(HttpRequest Request)
        {
            if (Request.RawUrl.StartsWith("/"))
            {
                return Request.Url.Scheme + "://" + Request.Url.Host + Request.RawUrl;
            }
            return Request.Url.Scheme + "://" + Request.Url.Host + "/" + Request.RawUrl;
        }

        /// <summary>Changes a URL beginning with <c>http://</c> to <c>https://</c>.</summary>
        /// <param name="currentUrl">The URL whose scheme should be changed.</param>
        /// <returns>The URL with its scheme changed when it uses lower-case <c>http://</c>; otherwise, the original value.</returns>
        public static string SwitchUrlToSecure(string currentUrl)
        {
            if (currentUrl.StartsWith("http://"))
            {
                return currentUrl.Insert(4, "s");
            }
            return currentUrl;
        }

        /// <summary>Changes a URL beginning with <c>https://</c> to <c>http://</c>.</summary>
        /// <param name="currentUrl">The URL whose scheme should be changed.</param>
        /// <returns>The URL with its scheme changed when it uses lower-case <c>https://</c>; otherwise, the original value.</returns>
        public static string SwitchUrlToStandard(string currentUrl)
        {
            if (currentUrl.StartsWith("https://"))
            {
                return currentUrl.Remove(4, 1);
            }
            return currentUrl;
        }
    }
}
