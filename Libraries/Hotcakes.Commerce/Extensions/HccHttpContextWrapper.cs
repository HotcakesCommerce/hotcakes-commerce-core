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

using System.Web;

namespace Hotcakes.Commerce.Extensions
{
    /// <summary>
    /// Wraps an ASP.NET <see cref="HttpContext"/> and exposes HCC request and response wrappers.
    /// </summary>
    /// <remarks>
    /// Each access to <see cref="HttpContextBase.Request"/> or
    /// <see cref="HttpContextBase.Response"/> creates the corresponding HCC wrapper.
    /// The response wrapper receives the request's <c>CustomRedirect</c> selection.
    /// </remarks>
    public class HccHttpContextWrapper : HttpContextWrapper
    {
        /// <summary>Stores the underlying ASP.NET context used by this wrapper.</summary>
        protected HttpContext _httpContext;

        /// <summary>
        /// Initializes a wrapper for the specified ASP.NET context.
        /// </summary>
        /// <param name="httpContext">The context to wrap.</param>
        public HccHttpContextWrapper(HttpContext httpContext)
            : base(httpContext)
        {
            _httpContext = httpContext;
        }

        /// <summary>
        /// Gets a new <see cref="HccHttpRequestWrapper"/> over the underlying request.
        /// </summary>
        public override HttpRequestBase Request
        {
            get { return new HccHttpRequestWrapper(_httpContext.Request); }
        }

        /// <summary>
        /// Gets a new <see cref="HccHttpResponseWrapper"/> configured from the request's custom redirect header.
        /// </summary>
        public override HttpResponseBase Response
        {
            get { return new HccHttpResponseWrapper(_httpContext.Response, UseCustomRedirect()); }
        }

        /// <summary>
        /// Determines whether the response should use HCC's custom redirect protocol.
        /// </summary>
        /// <returns><see langword="true"/> only when the <c>CustomRedirect</c> request header equals <c>1</c>.</returns>
        protected bool UseCustomRedirect()
        {
            return _httpContext.Request.Headers["CustomRedirect"] == "1";
        }
    }
}
