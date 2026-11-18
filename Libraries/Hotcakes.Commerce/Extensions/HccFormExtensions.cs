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
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.Extensions
{
    /// <summary>
    /// Provides HCC-aware MVC form helpers.
    /// </summary>
    /// <remarks>
    /// These helpers obtain the configured <see cref="IHccFormRenderer"/> from
    /// <see cref="Factory.CreateHccFormRenderer"/>. Route-based overloads either use
    /// MVC route generation or delegate HCC routes to the configured URL resolver.
    /// </remarks>
    public static class HccFormExtensions
    {
        // Methods
        /// <summary>Begins a POST HCC form targeting the current raw request URL.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccForm(this HtmlHelper htmlHelper)
        {
            var rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, rawUrl, FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Begins a POST HCC form targeting the current raw request URL.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="htmlAttributes">HTML attributes passed to the form renderer.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccForm(this HtmlHelper htmlHelper, object htmlAttributes)
        {
            var s = htmlHelper.ViewDataContainer as WebViewPage;
            var rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, rawUrl, FormMethod.Post,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Begins a POST form for a named MVC route.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), FormMethod.Post,
                new RouteValueDictionary());
        }

        /// <summary>Begins a POST form for a named MVC route and route values.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="routeValues">Values used to generate the route URL.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(routeValues), FormMethod.Post,
                new RouteValueDictionary());
        }

        /// <summary>Begins a form for a named MVC route using the specified method.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), method,
                new RouteValueDictionary());
        }

        /// <summary>Begins a POST form for a named MVC route and dictionary route values.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="routeValues">Values used to generate the route URL.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName,
            RouteValueDictionary routeValues)
        {
            return htmlHelper.BeginHccRouteForm(routeName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Begins a form for a named MVC route, values, and method.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="routeValues">Values used to generate the route URL.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues,
            FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(routeValues), method,
                new RouteValueDictionary());
        }

        /// <summary>Begins a form for a named MVC route, method, and attribute dictionary.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">HTML attributes passed to the form renderer.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), method, htmlAttributes);
        }

        /// <summary>Begins a form for a named MVC route, method, and anonymous attributes.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">Anonymous HTML attributes converted by MVC.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method,
            object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Begins a form for a named MVC route, dictionary values, and method.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="routeValues">Values used to generate the route URL.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName,
            RouteValueDictionary routeValues, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(routeName, routeValues, method, new RouteValueDictionary());
        }

        /// <summary>Begins a form for a named MVC route with values, method, and attributes.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="routeValues">Values used to generate the route URL.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">Anonymous HTML attributes converted by MVC.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues,
            FormMethod method, object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(routeValues), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        /// <summary>Begins a POST form for an HCC route.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), FormMethod.Post,
                new RouteValueDictionary());
        }

        /// <summary>Begins a POST form for an HCC route and route values.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="routeValues">Values used while resolving the route.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, object routeValues)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(routeValues), FormMethod.Post,
                new RouteValueDictionary());
        }

        /// <summary>Begins a form for an HCC route using the specified method.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), method, new RouteValueDictionary());
        }

        /// <summary>Begins a POST form for an HCC route and dictionary route values.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="routeValues">Values used while resolving the route.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route,
            RouteValueDictionary routeValues)
        {
            return htmlHelper.BeginHccRouteForm(route, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        /// <summary>Begins an HCC route form with values and method.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="routeValues">Values used while resolving the route.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, object routeValues,
            FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(routeValues), method,
                new RouteValueDictionary());
        }

        /// <summary>Begins an HCC route form with method and attribute dictionary.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">HTML attributes passed to the form renderer.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), method, htmlAttributes);
        }

        /// <summary>Begins an HCC route form with method and anonymous attributes.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">Anonymous HTML attributes converted by MVC.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, FormMethod method,
            object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        /// <summary>Begins an HCC route form with dictionary values and method.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="routeValues">Values used while resolving the route.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route,
            RouteValueDictionary routeValues, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(route, routeValues, method, new RouteValueDictionary());
        }

        /// <summary>Begins an HCC route form with values, method, and anonymous attributes.</summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route resolved by the configured URL resolver.</param>
        /// <param name="routeValues">Values used while resolving the route.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">Anonymous HTML attributes converted by MVC.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, object routeValues,
            FormMethod method, object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(routeValues), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        /// <summary>
        /// Begins a form for a named MVC route using explicit values, method, and attributes.
        /// </summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="routeName">The MVC route name.</param>
        /// <param name="routeValues">Values used to generate the route URL.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">HTML attributes passed to the form renderer.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName,
            RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var formAction = UrlHelper.GenerateUrl(routeName, null, null, routeValues, htmlHelper.RouteCollection,
                htmlHelper.ViewContext.RequestContext, false);
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, formAction, method, htmlAttributes);
        }

        /// <summary>
        /// Begins a form for an HCC route using explicit values, method, and attributes.
        /// </summary>
        /// <param name="htmlHelper">The MVC HTML helper for the current view.</param>
        /// <param name="route">The HCC route delegated to the configured URL resolver.</param>
        /// <param name="routeValues">Values used while resolving the route.</param>
        /// <param name="method">The HTTP method represented by the form.</param>
        /// <param name="htmlAttributes">HTML attributes passed to the form renderer.</param>
        /// <returns>The MVC form scope returned by the configured HCC form renderer.</returns>
        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route,
            RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            var url = urlResolver.RouteHccUrl(route, null, null, null, null, null, routeValues,
                htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, false);
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, url, method, htmlAttributes);
        }

        /// <summary>
        /// Ends an HCC form by writing its closing marker and MVC client-validation output.
        /// </summary>
        /// <param name="htmlHelper">The MVC HTML helper whose view context receives the output.</param>
        /// <remarks>
        /// The closing marker is the compatibility-sensitive
        /// <c>&lt;/div&gt;&lt;!--div[data-type]=form--&gt;</c> literal emitted by the existing implementation.
        /// </remarks>
        public static void EndHccForm(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewContext.Writer.Write("</div><!--div[data-type]=form-->");
            htmlHelper.ViewContext.OutputClientValidation();
        }
    }
}
