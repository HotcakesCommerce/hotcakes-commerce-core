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

using System.Collections.Generic;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.Extensions
{
    public static class HccFormExtensions
    {
        // Methods
        public static MvcForm BeginHccForm(this HtmlHelper htmlHelper)
        {
            var rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, rawUrl, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginHccForm(this HtmlHelper htmlHelper, object htmlAttributes)
        {
            var s = htmlHelper.ViewDataContainer as WebViewPage;
            var rawUrl = htmlHelper.ViewContext.HttpContext.Request.RawUrl;
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, rawUrl, FormMethod.Post,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), FormMethod.Post,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(routeValues), FormMethod.Post,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), method,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName,
            RouteValueDictionary routeValues)
        {
            return htmlHelper.BeginHccRouteForm(routeName, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues,
            FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(routeValues), method,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), method, htmlAttributes);
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, FormMethod method,
            object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName,
            RouteValueDictionary routeValues, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(routeName, routeValues, method, new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName, object routeValues,
            FormMethod method, object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(routeName, new RouteValueDictionary(routeValues), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), FormMethod.Post,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, object routeValues)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(routeValues), FormMethod.Post,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), method, new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route,
            RouteValueDictionary routeValues)
        {
            return htmlHelper.BeginHccRouteForm(route, routeValues, FormMethod.Post, new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, object routeValues,
            FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(routeValues), method,
                new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, FormMethod method,
            IDictionary<string, object> htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), method, htmlAttributes);
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, FormMethod method,
            object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route,
            RouteValueDictionary routeValues, FormMethod method)
        {
            return htmlHelper.BeginHccRouteForm(route, routeValues, method, new RouteValueDictionary());
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route, object routeValues,
            FormMethod method, object htmlAttributes)
        {
            return htmlHelper.BeginHccRouteForm(route, new RouteValueDictionary(routeValues), method,
                HtmlHelper.AnonymousObjectToHtmlAttributes(htmlAttributes));
        }


        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, string routeName,
            RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var formAction = UrlHelper.GenerateUrl(routeName, null, null, routeValues, htmlHelper.RouteCollection,
                htmlHelper.ViewContext.RequestContext, false);
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, formAction, method, htmlAttributes);
        }

        public static MvcForm BeginHccRouteForm(this HtmlHelper htmlHelper, HccRoute route,
            RouteValueDictionary routeValues, FormMethod method, IDictionary<string, object> htmlAttributes)
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            var url = urlResolver.RouteHccUrl(route, null, null, null, null, null, routeValues,
                htmlHelper.RouteCollection, htmlHelper.ViewContext.RequestContext, false);
            var formRenderer = Factory.CreateHccFormRenderer();
            return formRenderer.FormHelper(htmlHelper, url, method, htmlAttributes);
        }

        public static void EndHccForm(this HtmlHelper htmlHelper)
        {
            htmlHelper.ViewContext.Writer.Write("</div><!--div[data-type]=form-->");
            htmlHelper.ViewContext.OutputClientValidation();
        }
    }
}