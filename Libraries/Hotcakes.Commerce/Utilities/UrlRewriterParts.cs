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
using System.Web;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Holds the decoded path and query portions of a URL for URL-rewriting rules.
    /// </summary>
    public class UrlRewriterParts
    {
        private const int UrlConst = 0;
        private const int QueryConst = 1;
        private string _query = string.Empty;
        private string _url = string.Empty;

        /// <summary>
        /// Initializes a new instance from a URI, decoding its path and query and lower-casing the path.
        /// </summary>
        /// <param name="url">The URI whose path and query should be split.</param>
        /// <remarks>
        /// The first <c>?</c> separates <see cref="Url"/> from <see cref="Query"/>. The query text is retained
        /// without the separator, and <see cref="HasQuery"/> indicates whether a separator was present.
        /// </remarks>
        public UrlRewriterParts(Uri url)
        {
            HasQuery = false;
            var decodedPathAndQuery = HttpUtility.UrlDecode(url.PathAndQuery);
            var parts = decodedPathAndQuery.Split('?');

            if (parts.Length > 0)
            {
                parts[UrlConst] = parts[UrlConst].ToLower();
            }

            if (parts.Length > 1)
            {
                HasQuery = true;
            }

            Url = parts[UrlConst];
            if (HasQuery)
            {
                Query = parts[QueryConst];
            }
        }

        /// <summary>Gets or sets the decoded, lower-case URL path portion.</summary>
        /// <value>The path portion without the query separator.</value>
        public string Url
        {
            get { return _url; }
            set { _url = value; }
        }

        /// <summary>Gets or sets the decoded query portion of the URL.</summary>
        /// <value>The query text without the leading <c>?</c>, or an empty string when none was supplied.</value>
        public string Query
        {
            get { return _query; }
            set { _query = value; }
        }

        /// <summary>Gets or sets whether the source URL contained a query separator.</summary>
        /// <value><see langword="true"/> when a query separator was present; otherwise, <see langword="false"/>.</value>
        public bool HasQuery { get; set; }
    }
}
