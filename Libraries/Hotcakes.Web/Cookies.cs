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
using System.Linq;
using System.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web
{
    /// <summary>Provides helpers for reading and writing HTTP cookies.</summary>
    [Serializable]
    public class Cookies
    {
        /// <summary>Gets a cookie value from the request, or response when absent from the request.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="context">The HTTP context to inspect.</param><param name="log">The logger used for failures.</param><returns>The cookie value, or an empty string.</returns>
        public static string GetCookieString(string cookieName, HttpContextBase context, ILogger log)
        {
            var result = string.Empty;

            try
            {
                if (context != null)
                {
                    if (context.Request != null)
                    {
                        if (context.Request.Cookies.AllKeys.Contains(cookieName))
                        {
                            var checkCookie = context.Request.Cookies[cookieName];
                            if (checkCookie != null)
                            {
                                result = checkCookie.Value;
                            }
                        }
                    }
                    if (string.IsNullOrEmpty(result) && context.Response != null &&
                        context.Response.Cookies.AllKeys.Contains(cookieName))
                    {
                        var checkCookie = context.Response.Cookies[cookieName];
                        if (checkCookie != null)
                        {
                            result = checkCookie.Value;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                log.LogException(ex);
            }

            return result;
        }

        /// <summary>Gets a cookie converted to a 64-bit integer.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="context">The HTTP context.</param><param name="log">The logger used for failures.</param><returns>The parsed value, or -1.</returns>
        public static long GetCookieLong(string cookieName, HttpContextBase context, ILogger log)
        {
            long result = -1;
            var temp = GetCookieString(cookieName, context, log);
            long.TryParse(temp, out result);
            return result;
        }

        /// <summary>Gets a cookie converted to a <see cref="Guid"/>.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="context">The HTTP context.</param><param name="log">The logger used for failures.</param><returns>The parsed identifier, or <see langword="null"/>.</returns>
        public static Guid? GetCookieGuid(string cookieName, HttpContextBase context, ILogger log)
        {
            Guid? result = null;
            var temp = GetCookieString(cookieName, context, log);
            try
            {
                result = new Guid(temp);
            }
            catch
            {
                result = null;
            }
            return result;
        }

        /// <summary>Sets a string cookie using default expiration behavior.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="value">The value.</param><param name="context">The HTTP context.</param><param name="temporary">Whether the cookie is temporary.</param><param name="log">The logger used for failures.</param>
        public static void SetCookieString(string cookieName, string value, HttpContextBase context, bool temporary,
            ILogger log)
        {
            SetCookieString(cookieName, value, context, temporary, null, log);
        }

        /// <summary>Sets a string cookie for the specified domain.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="value">The value.</param><param name="context">The HTTP context.</param><param name="temporary">Whether the cookie is temporary.</param><param name="domain">The cookie domain.</param><param name="log">The logger used for failures.</param>
        public static void SetCookieStringWithDomain(string cookieName, string value, HttpContextBase context,
            bool temporary, string domain, ILogger log)
        {
            SetCookieString(cookieName, value, context, temporary, null, domain, log);
        }

        /// <summary>Sets a string cookie with an optional expiration date.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="value">The value.</param><param name="context">The HTTP context.</param><param name="temporary">Whether the cookie is temporary.</param><param name="expirationDate">The optional expiration date.</param><param name="log">The logger used for failures.</param>
        public static void SetCookieString(string cookieName, string value, HttpContextBase context, bool temporary,
            DateTime? expirationDate, ILogger log)
        {
            SetCookieString(cookieName, value, context, temporary, expirationDate, "", log);
        }

        /// <summary>Sets a string cookie with an optional expiration date and domain.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="value">The value.</param><param name="context">The HTTP context.</param><param name="temporary">Whether the cookie is temporary.</param><param name="expirationDate">The optional expiration date.</param><param name="domain">The cookie domain.</param><param name="log">The logger used for failures.</param>
        public static void SetCookieString(string cookieName, string value, HttpContextBase context, bool temporary,
            DateTime? expirationDate, string domain, ILogger log)
        {
            try
            {
                if (context != null)
                {
                    var saveCookie = new HttpCookie(cookieName, value);
                    if (!temporary)
                    {
                        if (expirationDate.HasValue)
                        {
                            saveCookie.Expires = expirationDate.Value;
                        }
                        else
                        {
                            saveCookie.Expires = DateTime.Now.AddYears(50);
                        }
                    }
                    if (domain.Trim().Length > 0)
                    {
                        saveCookie.Domain = domain;
                    }
                    context.Response.Cookies.Add(saveCookie);
                }
            }
            catch (Exception ex)
            {
                log.LogException(ex);
            }
        }

        /// <summary>Sets a 64-bit integer cookie.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="value">The value.</param><param name="context">The HTTP context.</param><param name="temporary">Whether the cookie is temporary.</param><param name="log">The logger used for failures.</param>
        public static void SetCookieLong(string cookieName, long value, HttpContextBase context, bool temporary,
            ILogger log)
        {
            SetCookieString(cookieName, value.ToString(), context, temporary, log);
        }

        /// <summary>Sets a <see cref="Guid"/> cookie.</summary>
        /// <param name="cookieName">The cookie name.</param><param name="value">The value.</param><param name="context">The HTTP context.</param><param name="temporary">Whether the cookie is temporary.</param><param name="log">The logger used for failures.</param>
        public static void SetCookieGuid(string cookieName, Guid value, HttpContextBase context, bool temporary,
            ILogger log)
        {
            SetCookieString(cookieName, value.ToString(), context, temporary, log);
        }
    }
}
