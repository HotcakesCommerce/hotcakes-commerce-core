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

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Provides URL helpers for switching between secure and non-secure HTTP schemes.
    /// </summary>
    public class SSL
    {
        /// <summary>
        /// Identifies the scheme to which a URL redirect should be changed.
        /// </summary>
        public enum SSLRedirectTo
        {
            /// <summary>Use the <c>https</c> scheme.</summary>
            SSL = 1,
            /// <summary>Use the <c>http</c> scheme.</summary>
            NonSSL = 2
        }

        /// <summary>
        /// Builds a URL with the requested HTTP scheme after repeatedly URL-decoding the input.
        /// </summary>
        /// <param name="currentUrl">The URL to normalize and convert.</param>
        /// <param name="redirectTo">The target scheme.</param>
        /// <returns>The converted URL, or the normalized URL when no scheme conversion is required.</returns>
        /// <remarks>Decoding is performed before scheme conversion and may therefore change encoded URL content.</remarks>
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

        /// <summary>
        /// Redirects the current HTTP response to the current request URL with the requested scheme.
        /// </summary>
        /// <param name="RedirectTo">The scheme to use for the redirect.</param>
        /// <remarks>
        /// The current absolute URL is lower-cased and repeatedly decoded before conversion. No response redirect
        /// is issued when the resulting URL is identical, preventing a redirect loop. The target is derived from
        /// the current request rather than validated as an arbitrary external destination.
        /// </remarks>
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

        /// <summary>
        /// Repeatedly URL-decodes a string until another decode produces no change.
        /// </summary>
        /// <param name="URL">The string to decode; the parameter is replaced with the final decoded value.</param>
        /// <remarks>This method mutates the caller's variable through the <see langword="ref"/> parameter.</remarks>
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
