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
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.Urls
{
    /// <summary>
    /// Defines the service used to resolve Hotcakes Commerce routes and store URLs.
    /// </summary>
    /// <remarks>
    /// Implementations connect the commerce route abstraction represented by <see cref="HccRoute"/>
    /// to the active ASP.NET route collection and store configuration. Obtain the configured
    /// implementation through the application factory rather than constructing an implementation
    /// directly. <see cref="HccUrlBuilder"/> provides the standard convenience API for callers that
    /// only need to generate a commerce route URL.
    /// </remarks>
    public interface IHccUrlResolver
    {
        /// <summary>
        /// Resolves a commerce route into a URL using the supplied MVC and routing context.
        /// </summary>
        /// <param name="route">The Hotcakes Commerce destination to resolve.</param>
        /// <param name="actionName">The MVC action name to use when resolving the route, if applicable.</param>
        /// <param name="controllerName">The MVC controller name to use when resolving the route, if applicable.</param>
        /// <param name="protocol">The URL protocol, such as <c>http</c> or <c>https</c>.</param>
        /// <param name="hostName">The host name to use when generating an absolute URL.</param>
        /// <param name="fragment">The optional URL fragment to append to the generated URL.</param>
        /// <param name="routeValues">The values used to select and populate the route.</param>
        /// <param name="routeCollection">The ASP.NET route collection used for resolution.</param>
        /// <param name="requestContext">The ASP.NET request context used for implicit route values and URL generation.</param>
        /// <param name="includeImplicitMvcValues">
        /// <see langword="true"/> to include implicit MVC values from the request context; otherwise,
        /// <see langword="false"/>.
        /// </param>
        /// <returns>The resolved route URL.</returns>
        /// <seealso cref="HccUrlBuilder.RouteHccUrl(HccRoute)"/>
        string RouteHccUrl(HccRoute route, string actionName, string controllerName, string protocol, string hostName,
            string fragment, RouteValueDictionary routeValues, RouteCollection routeCollection,
            RequestContext requestContext, bool includeImplicitMvcValues);

        /// <summary>
        /// Gets the virtual path used to access store data for the specified store.
        /// </summary>
        /// <param name="storeId">The identifier of the store for which to resolve a virtual path.</param>
        /// <returns>The store data virtual path.</returns>
        /// <remarks>
        /// The returned path is resolved by the active commerce URL provider and may vary by deployment
        /// or store configuration.
        /// </remarks>
        string GetStoreDataVirtualPath(long storeId);

        /// <summary>
        /// Gets the root URL for the specified store.
        /// </summary>
        /// <param name="store">The store whose root URL should be resolved.</param>
        /// <param name="secure">
        /// <see langword="true"/> to request an HTTPS URL; otherwise, <see langword="false"/> to use
        /// the provider's default protocol.
        /// </param>
        /// <returns>The store root URL.</returns>
        /// <remarks>
        /// The result is determined by the active store and URL-provider configuration. Callers should
        /// use this method instead of constructing store URLs from host or portal values directly.
        /// </remarks>
        string GetStoreRootUrl(Store store, bool secure = false);
    }
}
