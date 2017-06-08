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

using System.Web.Mvc;
using System.Web.Routing;
using Hotcakes.Commerce.Urls;

namespace Hotcakes.Commerce.Extensions
{
    public static class HccUrlExtensions
    {
        public static string RouteHccUrl(this UrlHelper urlHelper, HccRoute route)
        {
            return urlHelper.RouteHccUrl(route, null);
        }

        public static string RouteHccUrl(this UrlHelper urlHelper, HccRoute route, object routeValues)
        {
            return urlHelper.RouteHccUrl(route, routeValues, null);
        }

        public static string RouteHccUrl(this UrlHelper urlHelper, HccRoute route, RouteValueDictionary routeValues)
        {
            return urlHelper.RouteHccUrl(route, routeValues, null, null);
        }

        public static string RouteHccUrl(this UrlHelper urlHelper, HccRoute route, object routeValues, string protocol)
        {
            return RouteHccUrl(route, null, null, protocol, null, null, new RouteValueDictionary(routeValues),
                urlHelper.RouteCollection, urlHelper.RequestContext, false);
        }

        public static string RouteHccUrl(this UrlHelper urlHelper, HccRoute route, RouteValueDictionary routeValues,
            string protocol, string hostName)
        {
            return RouteHccUrl(route, null, null, protocol, hostName, null, routeValues, urlHelper.RouteCollection,
                urlHelper.RequestContext, false);
        }

        private static string RouteHccUrl(HccRoute route, string actionName, string controllerName, string protocol,
            string hostName, string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection,
            RequestContext requestContext, bool includeImplicitMvcValues)
        {
            var urlResolver = Factory.CreateHccUrlResolver();
            return urlResolver.RouteHccUrl(route, null, null, protocol, null, null, routeValues, routeCollection,
                requestContext, false);
        }
    }
}