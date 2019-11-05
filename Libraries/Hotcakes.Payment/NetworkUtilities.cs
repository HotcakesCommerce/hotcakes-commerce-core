#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

namespace Hotcakes.Payment
{
    [Serializable]
    public class NetworkUtilities
    {
        private const int DEFAULTTIMEOUT = 100000;

        public static string SendRequestByPost(string serviceUrl, string postData)
        {
            return SendRequestByPost(serviceUrl, postData, null, DEFAULTTIMEOUT);
        }

        public static string SendRequestByPost(string serviceUrl, string postData, WebProxy proxy, int timeout)
        {
            WebResponse resp = null;
            WebRequest req = null;
            var response = string.Empty;
            byte[] reqBytes = null;

            try
            {
                reqBytes = Encoding.UTF8.GetBytes(postData);
                req = WebRequest.Create(serviceUrl);
                req.Method = "POST";
                req.ContentLength = reqBytes.Length;
                req.ContentType = "application/x-www-form-urlencoded";
                req.Timeout = timeout;
                if (proxy != null)
                {
                    req.Proxy = proxy;
                }
                var reqStream = req.GetRequestStream();
                reqStream.Write(reqBytes, 0, reqBytes.Length);
                reqStream.Close();
                resp = req.GetResponse();
                var sr = new StreamReader(resp.GetResponseStream(), Encoding.UTF8, true);
                response += sr.ReadToEnd();
                sr.Close();
            }
            catch
            {
                throw;
            }

            return response;
        }
    }
}