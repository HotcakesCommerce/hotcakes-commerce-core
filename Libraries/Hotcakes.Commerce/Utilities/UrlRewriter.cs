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
using System.Dynamic;
using System.Web;
using System.Web.Routing;
using System.Web.UI;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.Utilities
{
    public class UrlRewriter
    {
        private const string CART_ROUTE_FORMAT = "{0}?AddSku={1}&AddSkuQty=1";

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

        public static bool IsCategorySlugInUse(string slug, string bvin, HotcakesApplication app)
        {
            return IsCategorySlugInUse(slug, bvin, app.CatalogServices.Categories);
        }

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

        public static string BuildUrlForProduct(Product p, object additionalParams = null)
        {
            var parameters = Merge(new {slug = p.UrlSlug}, additionalParams);
            var routeValues = new RouteValueDictionary(parameters);
            return HccUrlBuilder.RouteHccUrl(HccRoute.Product, routeValues);
        }

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

        public static string JoinUrlAndQuery(string url, string query)
        {
            if (url.Contains("?"))
            {
                return url + "&" + query;
            }
            return url + "?" + query;
        }

        public static string GetRewrittenUrlFromRequest(HttpRequest Request)
        {
            if (Request.RawUrl.StartsWith("/"))
            {
                return Request.Url.Scheme + "://" + Request.Url.Host + Request.RawUrl;
            }
            return Request.Url.Scheme + "://" + Request.Url.Host + "/" + Request.RawUrl;
        }

        public static string SwitchUrlToSecure(string currentUrl)
        {
            if (currentUrl.StartsWith("http://"))
            {
                return currentUrl.Insert(4, "s");
            }
            return currentUrl;
        }

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