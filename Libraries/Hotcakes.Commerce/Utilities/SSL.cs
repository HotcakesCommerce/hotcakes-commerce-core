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

using System.Web;

namespace Hotcakes.Commerce.Utilities
{
    public class SSL
    {
        public enum SSLRedirectTo
        {
            SSL = 1,
            NonSSL = 2
        }

        public static string BuildUrlForRedirect(string currentUrl, SSLRedirectTo redirectTo)
        {
            RemoveAllEncoding(ref currentUrl);

            var url = string.Empty;
            switch (redirectTo)
            {
                case SSLRedirectTo.NonSSL:
                    url = UrlRewriter.SwitchUrlToStandard(currentUrl);
                    break;
                case SSLRedirectTo.SSL:
                    url = UrlRewriter.SwitchUrlToSecure(currentUrl);
                    break;
            }
            return url;
        }

        public static void SSLRedirect(SSLRedirectTo RedirectTo)
        {
            var CurrentUrl = HttpContext.Current.Request.Url.AbsoluteUri.ToLower();
            var url = BuildUrlForRedirect(CurrentUrl, RedirectTo);

            //if the urls match, then for some reason we aren't replacing anything
            //so if we redirect then we will go into a loop
            if (url != CurrentUrl)
            {
                HttpContext.Current.Response.Redirect(url);
            }
        }

        public static void RemoveAllEncoding(ref string URL)
        {
            var NewURL = string.Empty;
            while (URL != NewURL)
            {
                NewURL = URL;
                URL = HttpUtility.UrlDecode(NewURL);
            }
        }
    }
}