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
using System.Collections.ObjectModel;
using System.Web;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Defines an extension point for custom URL-rewriting rules.
    /// </summary>
    /// <remarks>
    /// Implementations can be registered in <see cref="AvailableRules"/>. A rule receives the current application,
    /// source URI, and decoded URL parts and returns whether it handled the request.
    /// </remarks>
    public abstract class UrlRewritingRule
    {
        static UrlRewritingRule()
        {
            AvailableRules = null;
        }

        /// <summary>
        /// Gets or sets the collection of custom rewriting rules available to the application.
        /// </summary>
        /// <value>The registered rules, or <see langword="null"/> when no collection has been configured.</value>
        /// <remarks>The collection is shared process-wide; callers are responsible for initialization and ordering.</remarks>
        public static Collection<UrlRewritingRule> AvailableRules { get; set; }

        /// <summary>
        /// Executes the rule against the current request.
        /// </summary>
        /// <param name="app">The current application; implementations may replace the referenced application.</param>
        /// <param name="sourceUrl">The original URI being evaluated.</param>
        /// <param name="parts">The decoded path and query parts derived for rewriting.</param>
        /// <returns><see langword="true"/> when the rule handles the request; otherwise, <see langword="false"/>.</returns>
        public abstract bool Execute(ref HttpApplication app, Uri sourceUrl, UrlRewriterParts parts);
    }
}
