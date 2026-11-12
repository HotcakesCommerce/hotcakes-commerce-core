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

using System.Web.Routing;

namespace Hotcakes.Commerce.Urls
{
    /// <summary>
    /// Provides convenience methods for generating URLs for registered Hotcakes Commerce routes.
    /// </summary>
    /// <remarks>
    /// The builder delegates route resolution to the configured <see cref="IHccUrlResolver"/> and
    /// uses the current ASP.NET route table and request context when they are available. Route
    /// values are passed to the selected route and are not interpreted by this class.
    /// </remarks>
    /// <seealso cref="HccRoute"/>
    /// <seealso cref="HccRouteNames"/>
    public static class HccUrlBuilder
    {
        /// <summary>
        /// Generates a URL for the specified commerce route without additional route values.
        /// </summary>
        /// <param name="route">The commerce destination for which to generate a URL.</param>
        /// <returns>The generated route URL.</returns>
        /// <remarks>
        /// The current request context is used when available. Use an overload with route values,
        /// a protocol, or a host name when the route requires additional URL information.
        /// </remarks>
        /// <seealso cref="HccRoute"/>
        public static string RouteHccUrl(HccRoute route)
        {
            return RouteHccUrl(route, null);
        }

        /// <summary>
        /// Generates a URL for the specified commerce route and route values.
        /// </summary>
        /// <param name="route">The commerce destination for which to generate a URL.</param>
        /// <param name="routeValues">An object containing the values used to select and populate the route.</param>
        /// <returns>The generated route URL.</returns>
        /// <remarks>
        /// The properties of <paramref name="routeValues"/> are converted to a route-value dictionary
        /// before the configured <see cref="IHccUrlResolver"/> generates the URL.
        /// </remarks>
        public static string RouteHccUrl(HccRoute route, object routeValues)
        {
            return RouteHccUrl(route, routeValues, null);
        }

        /// <summary>
        /// Generates a URL for the specified commerce route and route values.
        /// </summary>
        /// <param name="route">The commerce destination for which to generate a URL.</param>
        /// <param name="routeValues">The route values used to select and populate the route.</param>
        /// <returns>The generated route URL.</returns>
        /// <remarks>
        /// Use this overload when the caller already has a <see cref="RouteValueDictionary"/>.
        /// </remarks>
        public static string RouteHccUrl(HccRoute route, RouteValueDictionary routeValues)
        {
            return RouteHccUrl(route, routeValues, null, null);
        }

        /// <summary>
        /// Generates a URL for the specified commerce route and route values using the given protocol.
        /// </summary>
        /// <param name="route">The commerce destination for which to generate a URL.</param>
        /// <param name="routeValues">An object containing the values used to select and populate the route.</param>
        /// <param name="protocol">The URL protocol, such as <c>http</c> or <c>https</c>.</param>
        /// <returns>The generated route URL.</returns>
        /// <remarks>
        /// The current request context and application route table are used when resolving the URL.
        /// </remarks>
        public static string RouteHccUrl(HccRoute route, object routeValues, string protocol)
        {
            var requestContext = Factory.HttpContext != null ? Factory.HttpContext.Request.RequestContext : null;
            return RouteHccUrl(route, null, null, protocol, null, null, new RouteValueDictionary(routeValues),
                RouteTable.Routes, requestContext, false);
        }

        /// <summary>
        /// Generates an absolute URL for the specified commerce route and route values.
        /// </summary>
        /// <param name="route">The commerce destination for which to generate a URL.</param>
        /// <param name="routeValues">The route values used to select and populate the route.</param>
        /// <param name="protocol">The URL protocol, such as <c>http</c> or <c>https</c>.</param>
        /// <param name="hostName">The host name to use for the generated URL.</param>
        /// <returns>The generated route URL.</returns>
        /// <remarks>
        /// Supplying a host name is useful when generating a URL outside the host and request context
        /// that would otherwise be used by the active ASP.NET route configuration.
        /// </remarks>
        public static string RouteHccUrl(HccRoute route, RouteValueDictionary routeValues, string protocol,
            string hostName)
        {
            var requestContext = Factory.HttpContext != null ? Factory.HttpContext.Request.RequestContext : null;
            return RouteHccUrl(route, null, null, protocol, hostName, null, routeValues, RouteTable.Routes,
                requestContext, false);
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
