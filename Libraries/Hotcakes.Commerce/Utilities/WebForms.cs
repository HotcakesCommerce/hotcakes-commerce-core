#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    public class WebForms
    {
        private const int DefaultTimeout = 100000;

        public static string SendRequestByPost(string serviceUrl, string postData)
        {
            return SendRequestByPost(serviceUrl, postData, null, DefaultTimeout);
        }

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