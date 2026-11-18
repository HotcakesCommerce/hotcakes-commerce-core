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
using Hotcakes.Web;

namespace Hotcakes.CommerceDTO.v1.Client
{
    /// <summary>
    ///     Provides the low-level HTTP operations used by <see cref="Api"/> to communicate with the Commerce REST API.
    /// </summary>
    /// <remarks>
    ///     Each operation deserializes the response into the requested reference type. Transport and deserialization
    ///     failures are represented through <see cref="IApiResponse.Errors"/> when the requested type implements
    ///     <see cref="IApiResponse"/>.
    /// </remarks>
    [Serializable]
    public class RestHelper
    {
        private const int DefaultTimeout = 100000;

        /// <summary>
        ///     Sends an HTTP GET request and deserializes the response into the requested type.
        /// </summary>
        /// <typeparam name="T">The reference type into which the response is deserialized.</typeparam>
        /// <param name="uri">The absolute URI of the REST endpoint.</param>
        /// <returns>A new instance of <typeparamref name="T"/> populated from the response, or containing an API error.</returns>
        /// <remarks>This is the read-only counterpart to <see cref="PostRequest{T}"/>, <see cref="PutRequest{T}"/>, and <see cref="DeleteRequest{T}"/>.</remarks>
        public static T GetRequest<T>(string uri)
            where T : class, new()
        {
            var result = SendGet<T>(uri);
            return result;
        }

        /// <summary>
        ///     Sends an HTTP POST request with the supplied data and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The reference type into which the response is deserialized.</typeparam>
        /// <param name="uri">The absolute URI of the REST endpoint.</param>
        /// <param name="data">The request body, normally serialized by the calling API method.</param>
        /// <returns>A new instance of <typeparamref name="T"/> populated from the response, or containing an API error.</returns>
        public static T PostRequest<T>(string uri, string data)
            where T : class, new()
        {
            var result = SendWithData<T>(uri, "POST", data);
            return result;
        }

        /// <summary>
        ///     Sends an HTTP PUT request with the supplied data and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The reference type into which the response is deserialized.</typeparam>
        /// <param name="uri">The absolute URI of the REST endpoint.</param>
        /// <param name="data">The request body to send to the endpoint.</param>
        /// <returns>A new instance of <typeparamref name="T"/> populated from the response, or containing an API error.</returns>
        public static T PutRequest<T>(string uri, string data)
            where T : class, new()
        {
            var result = SendWithData<T>(uri, "PUT", data);
            return result;
        }

        /// <summary>
        ///     Sends an HTTP DELETE request and deserializes the response.
        /// </summary>
        /// <typeparam name="T">The reference type into which the response is deserialized.</typeparam>
        /// <param name="uri">The absolute URI of the REST endpoint.</param>
        /// <param name="data">The request body to send to the endpoint, when the endpoint requires one.</param>
        /// <returns>A new instance of <typeparamref name="T"/> populated from the response, or containing an API error.</returns>
        public static T DeleteRequest<T>(string uri, string data)
            where T : class, new()
        {
            var result = SendWithData<T>(uri, "DELETE", data);
            return result;
        }

        private static T SendGet<T>(string uri)
            where T : class, new()
        {
            try
            {
                var response = SendRequest(uri, "GET", null);
                var result = Json.ObjectFromJson<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                var result = new T();
                var apiResponse = result as IApiResponse;
                if (apiResponse != null)
                {
                    apiResponse.Errors.Add(new ApiError("EXCEPTION", ex.Message + " | " + ex.StackTrace));
                }
                return result;
            }
        }

        private static T SendWithData<T>(string uri, string method, string data)
            where T : class, new()
        {
            try
            {
                var response = SendRequest(uri, method, data);
                var result = Json.ObjectFromJson<T>(response);
                return result;
            }
            catch (Exception ex)
            {
                var result = new T();
                var apiResponse = result as IApiResponse;
                if (apiResponse != null)
                {
                    apiResponse.Errors.Add(new ApiError("EXCEPTION", ex.Message + " | " + ex.StackTrace));
                }
                return result;
            }
        }

        private static string SendRequest(string serviceUrl, string method, string data)
        {
            return SendRequest(serviceUrl, method, data, null, DefaultTimeout);
        }

        private static string SendRequest(string serviceUrl, string method, string data, WebProxy proxy, int timeout)
        {
            WebResponse objResp;
            WebRequest objReq;
            var strResp = string.Empty;
            byte[] byteReq;

            try
            {
                if (data == null)
                {
                    byteReq = null;
                }
                else
                {
                    byteReq = Encoding.UTF8.GetBytes(data);
                }
                objReq = WebRequest.Create(serviceUrl);
                objReq.Method = method.ToUpperInvariant();

                // Set Content Length If we Have Some
                if (byteReq != null)
                {
                    objReq.ContentLength = byteReq.Length;
                    objReq.ContentType = "application/x-www-form-urlencoded";
                }

                objReq.Timeout = timeout;
                if (proxy != null)
                {
                    objReq.Proxy = proxy;
                }

                // Send Data if we have some
                if (byteReq != null)
                {
                    var OutStream = objReq.GetRequestStream();
                    OutStream.Write(byteReq, 0, byteReq.Length);
                    OutStream.Close();
                }
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
    }
}
