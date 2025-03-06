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

using System;
using System.Net;
using System.Web;

namespace Hotcakes.Commerce.Extensions
{
    public class HccHttpResponseWrapper : HttpResponseWrapper
    {
        protected HttpResponse _httpResponse;
        protected bool _useCustomRedirect;

        public HccHttpResponseWrapper(HttpResponse httpResponse, bool useCustomRedirect)
            : base(httpResponse)
        {
            _httpResponse = httpResponse;
            _useCustomRedirect = useCustomRedirect;
        }

        public override void Redirect(string url)
        {
            Redirect(url, true);
        }

        public override void Redirect(string url, bool endResponse)
        {
            Redirect(url, endResponse, false, _useCustomRedirect);
        }

        public override void RedirectPermanent(string url)
        {
            RedirectPermanent(url, true);
        }

        public override void RedirectPermanent(string url, bool endResponse)
        {
            Redirect(url, endResponse, true, _useCustomRedirect);
        }

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