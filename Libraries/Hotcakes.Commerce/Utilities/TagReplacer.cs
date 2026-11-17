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
using Hotcakes.Commerce.Storage;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Replaces legacy store content tags in a string.
    /// </summary>
    /// <remarks>New code should prefer the tag-handler and template mechanisms used by the application.</remarks>
    public class TagReplacer
    {
        /// <summary>
        /// Replaces recognized legacy tags with store-specific URLs, branding, links, files, and address markup.
        /// </summary>
        /// <param name="source">The content containing tags such as <c>{{homelink}}</c> and <c>{{logo}}</c>.</param>
        /// <param name="app">The current application used to resolve request security, customer, store, and contact data.</param>
        /// <returns>
        /// The source with recognized tags replaced. If the source does not contain <c>{{</c>, it is returned unchanged.
        /// </returns>
        /// <remarks>
        /// The store address is inserted as HTML from <see cref="Hotcakes.Commerce.Contacts.Address.ToHtmlString"/>; callers must treat the
        /// returned value as markup rather than encode it a second time. Unrecognized tags remain unchanged.
        /// </remarks>
        public static string ReplaceContentTags(string source, HotcakesApplication app)
        {
            if (source.Contains("{{"))
            {
                var isSecureRequest = app.IsCurrentRequestSecure();
                var currentUserId = app.CurrentCustomerId;

                var output = source;

                var r = RouteTable.Routes;

                output = output.Replace("{{homelink}}", app.StoreUrl(isSecureRequest, false));
                output = output.Replace("{{logo}}", HtmlRendering.Logo(app, isSecureRequest));
                output = output.Replace("{{logotext}}", HtmlRendering.LogoText(app));
                output = output.Replace("{{headerlinks}}", HtmlRendering.HeaderLinks(app, currentUserId));
                output = output.Replace("{{sitefiles}}", DiskStorage.GetStoreDataUrl(app, isSecureRequest));

                output = output.Replace("{{storeaddress}}",
                    app.ContactServices.Addresses.FindStoreContactAddress().ToHtmlString());

                return output;
            }
            return source;
        }
    }
}
