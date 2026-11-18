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
using System.Web.Mvc;

namespace Hotcakes.Commerce.Extensions
{
    /// <summary>
    /// Executes an MVC redirect through the HCC response wrapper.
    /// </summary>
    /// <remarks>
    /// The destination is resolved with <c>UrlHelper.GenerateContentUrl</c>,
    /// temporary data is retained, and the response is deliberately not ended by the redirect call.
    /// The wrapped response then determines whether standard or custom redirect headers are emitted.
    /// </remarks>
    public class HccRedirectResult : RedirectResult
    {
        /// <summary>
        /// Initializes a non-permanent HCC redirect.
        /// </summary>
        /// <param name="url">The destination URL.</param>
        public HccRedirectResult(string url)
            : base(url, false)
        {
        }

        /// <summary>
        /// Initializes an HCC redirect with the requested permanence.
        /// </summary>
        /// <param name="url">The destination URL.</param>
        /// <param name="permanent">Whether to use the permanent redirect path.</param>
        public HccRedirectResult(string url, bool permanent)
            : base(url, permanent)
        {
        }

        /// <summary>
        /// Executes the redirect against the current controller context.
        /// </summary>
        /// <param name="context">The controller context receiving the redirect.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="context"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// The result preserves controller temporary data and invokes either
        /// the response's redirect or permanent-redirect operation with
        /// <c>endResponse</c> set to <see langword="false"/>.
        /// </remarks>
        public override void ExecuteResult(ControllerContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException("context");
            }
            var url = UrlHelper.GenerateContentUrl(Url, context.HttpContext);
            context.Controller.TempData.Keep();
            var endResponse = false;
            if (Permanent)
            {
                context.HttpContext.Response.RedirectPermanent(url, endResponse);
            }
            else
            {
                context.HttpContext.Response.Redirect(url, endResponse);
            }
        }
    }
}
