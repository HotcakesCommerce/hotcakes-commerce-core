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
using System.Net;
using System.Web;

namespace Hotcakes.Commerce.Extensions
{
    /// <summary>
    /// Wraps an ASP.NET response and preserves HCC redirect behavior.
    /// </summary>
    /// <remarks>
    /// Ordinary redirects use the standard location and status fields. When enabled,
    /// custom redirects use status <c>333</c> and the <c>CustomLocation</c> header.
    /// Both paths still render the inherited ASP.NET-style object-moved body.
    /// </remarks>
    public class HccHttpResponseWrapper : HttpResponseWrapper
    {
        /// <summary>Stores the underlying ASP.NET response.</summary>
        protected HttpResponse _httpResponse;
        /// <summary>Indicates whether redirects use HCC's custom redirect protocol.</summary>
        protected bool _useCustomRedirect;

        /// <summary>
        /// Initializes a response wrapper.
        /// </summary>
        /// <param name="httpResponse">The response to wrap.</param>
        /// <param name="useCustomRedirect">Whether redirects use status 333 and <c>CustomLocation</c>.</param>
        public HccHttpResponseWrapper(HttpResponse httpResponse, bool useCustomRedirect)
            : base(httpResponse)
        {
            _httpResponse = httpResponse;
            _useCustomRedirect = useCustomRedirect;
        }

        /// <summary>Redirects to a URL and ends the response.</summary>
        /// <param name="url">The destination URL.</param>
        public override void Redirect(string url)
        {
            Redirect(url, true);
        }

        /// <summary>Redirects to a URL, optionally ending the response.</summary>
        /// <param name="url">The destination URL.</param>
        /// <param name="endResponse">Whether to end the response after writing the redirect.</param>
        public override void Redirect(string url, bool endResponse)
        {
            Redirect(url, endResponse, false, _useCustomRedirect);
        }

        /// <summary>Redirects permanently to a URL and ends the response.</summary>
        /// <param name="url">The destination URL.</param>
        public override void RedirectPermanent(string url)
        {
            RedirectPermanent(url, true);
        }

        /// <summary>Redirects permanently to a URL, optionally ending the response.</summary>
        /// <param name="url">The destination URL.</param>
        /// <param name="endResponse">Whether to end the response after writing the redirect.</param>
        public override void RedirectPermanent(string url, bool endResponse)
        {
            Redirect(url, endResponse, true, _useCustomRedirect);
        }

        /// <summary>
        /// Performs the HCC redirect operation used by the public redirect overloads.
        /// </summary>
        /// <param name="url">The destination URL; it is first passed to <see cref="HttpResponseBase.ApplyAppPathModifier"/>.</param>
        /// <param name="endResponse">Whether to end the response after writing the body.</param>
        /// <param name="permanent">Whether to use HTTP 301 for a standard redirect.</param>
        /// <param name="custom">Whether to use status 333 and the <c>CustomLocation</c> header.</param>
        /// <exception cref="ArgumentNullException">Thrown when <paramref name="url"/> is <see langword="null"/>.</exception>
        /// <remarks>
        /// Standard redirects set <see cref="HttpResponseBase.RedirectLocation"/>;
        /// custom redirects do not set that property and instead replace the
        /// <c>CustomLocation</c> header. URL encoding and the generated HTML body
        /// intentionally follow the existing implementation.
        /// </remarks>
        protected void Redirect(string url, bool endResponse, bool permanent, bool custom)
        {
            if (url == null)
            {
                throw new ArgumentNullException("url");
            }
            url = ApplyAppPathModifier(url);

            Clear();
            if (custom)
            {
                StatusCode = 333;
                Headers.Remove("CustomLocation");
                Headers.Add("CustomLocation", url);
            }
            else
            {
                StatusCode = permanent ? (int) HttpStatusCode.MovedPermanently : (int) HttpStatusCode.Redirect;
                RedirectLocation = url;
            }
            if ((url.IndexOf(":", StringComparison.Ordinal) == -1) ||
                url.StartsWith("http:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("https:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("ftp:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("file:", StringComparison.OrdinalIgnoreCase) ||
                url.StartsWith("news:", StringComparison.OrdinalIgnoreCase))
            {
                url = HttpUtility.HtmlAttributeEncode(url);
            }
            else
            {
                url = HttpUtility.HtmlAttributeEncode(HttpUtility.UrlEncode(url));
            }
            Write("<html><head><title>Object moved</title></head><body>\r\n");
            Write("<h2>Object moved to <a href=\"" + url + "\">here</a>.</h2>\r\n");
            Write("</body></html>\r\n");

            if (endResponse)
            {
                End();
            }
        }
    }
}
