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
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Web.UI;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Provides HTTP request and ASP.NET Web Forms response-cache helpers.
    /// </summary>
    public class WebForms
    {
        private const int DefaultTimeout = 100000;

        /// <summary>
        /// Sends UTF-8 form-urlencoded data to a URL using an HTTP POST request and the default timeout.
        /// </summary>
        /// <param name="serviceUrl">The URL of the service endpoint.</param>
        /// <param name="postData">The form data sent as UTF-8 bytes.</param>
        /// <returns>The response body decoded as UTF-8.</returns>
        /// <exception cref="ArgumentException">The request fails; the original exception message and source are included.</exception>
        public static string SendRequestByPost(string serviceUrl, string postData)
        {
            return SendRequestByPost(serviceUrl, postData, null, DefaultTimeout);
        }

        /// <summary>
        /// Sends UTF-8 form-urlencoded data to a URL using an HTTP POST request.
        /// </summary>
        /// <param name="serviceUrl">The URL of the service endpoint.</param>
        /// <param name="postData">The form data sent as UTF-8 bytes.</param>
        /// <param name="proxy">The proxy used for the request, or <see langword="null"/> for the default proxy behavior.</param>
        /// <param name="timeout">The request timeout in milliseconds.</param>
        /// <returns>The response body decoded as UTF-8.</returns>
        /// <exception cref="ArgumentException">The request or response read fails; the original exception message and
        /// source are included.</exception>
        /// <remarks>The method does not expose response headers or status details and wraps all failures in an
        /// <see cref="ArgumentException"/>.</remarks>
        public static string SendRequestByPost(string serviceUrl, string postData, WebProxy proxy, int timeout)
        {
            WebResponse objResp;
            WebRequest objReq;
            var strResp = string.Empty;
            byte[] byteReq;

            try
            {
                byteReq = Encoding.UTF8.GetBytes(postData);
                objReq = WebRequest.Create(serviceUrl);
                objReq.Method = "POST";
                objReq.ContentLength = byteReq.Length;
                objReq.ContentType = "application/x-www-form-urlencoded";
                objReq.Timeout = timeout;
                if (proxy != null)
                {
                    objReq.Proxy = proxy;
                }
                var OutStream = objReq.GetRequestStream();
                OutStream.Write(byteReq, 0, byteReq.Length);
                OutStream.Close();
                objResp = objReq.GetResponse();
                var sr = new StreamReader(objResp.GetResponseStream(), Encoding.UTF8, true);
                strResp += sr.ReadToEnd();
                sr.Close();
            }
            catch (Exception ex)
            {
                throw new ArgumentException("Error SendRequest: " + ex.Message + " " + ex.Source);
            }

            return strResp;
        }

        /// <summary>
        /// Configures a Web Forms response so that browsers and proxies must not cache the page.
        /// </summary>
        /// <param name="currentPage">The page whose response cache headers are changed.</param>
        /// <remarks>Sets no-cache and no-store directives, an expired date, a zero max age, and revalidation extensions.</remarks>
        public static void MakePageNonCacheable(Page currentPage)
        {
            currentPage.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            currentPage.Response.Cache.SetNoStore();
            currentPage.Response.Cache.SetExpires(DateTime.Now.AddDays(-5000));
            currentPage.Response.Cache.SetMaxAge(new TimeSpan(0));
            currentPage.Response.Cache.AppendCacheExtension("must-revalidate, proxy-revalidate");
        }
    }
}
