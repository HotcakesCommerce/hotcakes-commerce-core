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

using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Builds Gravatar image URLs from email addresses.</summary>
    public class GravatarHelper
    {
        /// <summary>
        /// Builds the default Gravatar URL by trimming and lower-casing the email before hashing it with MD5.
        /// </summary>
        /// <param name="email">The email address to hash.</param>
        /// <returns>An HTTP Gravatar URL ending in the hexadecimal email hash and <c>.jpg</c>.</returns>
        public static string GetGravatarUrlForEmail(string email)
        {
            return "http://www.gravatar.com/avatar/" + HashUserEmail(email) + ".jpg";
        }

        /// <summary>Adds a size query parameter to the Gravatar URL for an email address.</summary>
        /// <param name="email">The email address to hash.</param>
        /// <param name="size">The requested image size.</param>
        /// <returns>The Gravatar URL with an <c>s</c> query parameter.</returns>
        public static string GetGravatarUrlForEmailWithSize(string email, int size)
        {
            return GetGravatarUrlForEmail(email) + "?s=" + size;
        }

        /// <summary>Adds size and URL-encoded default-image parameters to a Gravatar URL.</summary>
        /// <param name="email">The email address to hash.</param>
        /// <param name="size">The requested image size.</param>
        /// <param name="defaultImageUrl">The fallback image URL, URL-encoded in the query string.</param>
        /// <returns>The Gravatar URL with <c>s</c> and <c>d</c> query parameters.</returns>
        public static string GetGravatarUrlForEmailWithSizeAndDefault(string email, int size, string defaultImageUrl)
        {
            return GetGravatarUrlForEmailWithSize(email, size) + "&d=" + HttpUtility.UrlEncode(defaultImageUrl);
        }

        private static string HashUserEmail(string theEmail)
        {
            var email = theEmail.Trim().ToLower();
            var md5Obj =
                new MD5CryptoServiceProvider();
            var bytesToHash = Encoding.ASCII.GetBytes(email);
            bytesToHash = md5Obj.ComputeHash(bytesToHash);
            var strResult = "";
            foreach (var b in bytesToHash)
            {
                strResult += b.ToString("x2");
            }
            return strResult;
        }
    }
}
