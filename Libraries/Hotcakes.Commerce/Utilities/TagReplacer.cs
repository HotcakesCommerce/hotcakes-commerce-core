#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    // TODO: Get rid of this slow pig!!, TagHandlers and templates take care of this now.
    public class TagReplacer
    {
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