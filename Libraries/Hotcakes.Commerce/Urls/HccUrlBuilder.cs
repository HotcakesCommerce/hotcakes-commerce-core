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
    /// values are passed to the selected route and are not interpreted or encoded by this class;
    /// the configured resolver owns route selection, URL encoding, and store/portal-aware URL
    /// behavior. This class is a convenience facade and does not change the active route contract.
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
        /// The current request context is used when available. This overload supplies no additional
        /// route values, protocol, host name, or fragment to the resolver. Use an overload with
        /// route values or a protocol when the route requires additional URL information.
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
        /// The properties of <paramref name="routeValues"/> are converted to a
        /// <see cref="RouteValueDictionary"/> before the configured
        /// <see cref="IHccUrlResolver"/> generates the URL. The resolver, rather than this
        /// builder, determines how values are selected, encoded, and placed in the resulting URL.
        /// A <see langword="null"/> value object is passed through the same convenience chain as
        /// an empty set of route values.
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
        /// The dictionary is forwarded to the configured resolver without being copied, normalized,
        /// or encoded by this builder; route and URL semantics remain resolver-owned.
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
        /// The protocol is forwarded to the resolver. The object route values are converted to a
        /// <see cref="RouteValueDictionary"/>; the builder does not itself encode route values.
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
        /// The protocol is accepted by this overload and forwarded to the resolver. Although this
        /// overload accepts <paramref name="hostName"/>, the current builder implementation does not
        /// forward that argument: its internal delegation supplies a <see langword="null"/> host,
        /// along with <see langword="null"/> action, controller, and fragment values. Document and
        /// preserve this observed compatibility behavior; URL host and encoding semantics remain
        /// the responsibility of the configured <see cref="IHccUrlResolver"/>.
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
