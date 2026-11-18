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
    /// Wraps an ASP.NET <see cref="HttpRequest"/> for HCC MVC request contexts.
    /// </summary>
    /// <remarks>
    /// The wrapper currently retains the underlying request for HCC extension points
    /// while inheriting the standard <see cref="HttpRequestBase"/> behavior.
    /// </remarks>
    public class HccHttpRequestWrapper : HttpRequestWrapper
    {
        /// <summary>Stores the underlying ASP.NET request.</summary>
        protected HttpRequest _httpRequest;

        /// <summary>
        /// Initializes a wrapper for the specified request.
        /// </summary>
        /// <param name="httpRequest">The request to wrap.</param>
        public HccHttpRequestWrapper(HttpRequest httpRequest)
            : base(httpRequest)
        {
            _httpRequest = httpRequest;
        }
    }
}
