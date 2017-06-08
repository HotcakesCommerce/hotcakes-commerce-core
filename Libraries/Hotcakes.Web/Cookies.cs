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
using System.Linq;
using System.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Web
{
    [Serializable]
    public class Cookies
    {
        public static string GetCookieString(string cookieName, HttpContextBase context, ILogger log)
        {
            var result = string.Empty;

            try
            {
                if (context != null)
                {
                    if (context.Request != null)
                    {
                        if (context.Request.Browser.Cookies && context.Request.Cookies.AllKeys.Contains(cookieName))
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

        public static long GetCookieLong(string cookieName, HttpContextBase context, ILogger log)
        {
            long result = -1;
            var temp = GetCookieString(cookieName, context, log);
            long.TryParse(temp, out result);
            return result;
        }

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

        public static void SetCookieString(string cookieName, string value, HttpContextBase context, bool temporary,
            ILogger log)
        {
            SetCookieString(cookieName, value, context, temporary, null, log);
        }

        public static void SetCookieStringWithDomain(string cookieName, string value, HttpContextBase context,
            bool temporary, string domain, ILogger log)
        {
            SetCookieString(cookieName, value, context, temporary, null, domain, log);
        }

        public static void SetCookieString(string cookieName, string value, HttpContextBase context, bool temporary,
            DateTime? expirationDate, ILogger log)
        {
            SetCookieString(cookieName, value, context, temporary, expirationDate, "", log);
        }

        public static void SetCookieString(string cookieName, string value, HttpContextBase context, bool temporary,
            DateTime? expirationDate, string domain, ILogger log)
        {
            try
            {
                if (context != null)
                {
                    if (context.Request != null)
                    {
                        if (context.Request.Browser.Cookies)
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
                }
            }
            catch (Exception ex)
            {
                log.LogException(ex);
            }
        }

        public static void SetCookieLong(string cookieName, long value, HttpContextBase context, bool temporary,
            ILogger log)
        {
            SetCookieString(cookieName, value.ToString(), context, temporary, log);
        }

        public static void SetCookieGuid(string cookieName, Guid value, HttpContextBase context, bool temporary,
            ILogger log)
        {
            SetCookieString(cookieName, value.ToString(), context, temporary, log);
        }
    }
}