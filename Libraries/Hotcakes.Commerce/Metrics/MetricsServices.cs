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

namespace Hotcakes.Commerce.Metrics
{
    /// <summary>
    /// Composes the repositories used to record and report store search-query metrics.
    /// </summary>
    /// <remarks>
    /// The application-level service is exposed through the compatibility-sensitive
    /// <c>MetricsSerices</c> property name. The misspelling is preserved by the public
    /// application API and is not corrected here.
    /// </remarks>
    public class MetricsServices : HccServiceBase
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MetricsServices"/> class for the specified request context.
        /// </summary>
        /// <param name="context">The commerce request context that supplies the current store and repository scope.</param>
        public MetricsServices(HccRequestContext context)
            : base(context)
        {
            SearchQueries = Factory.CreateRepo<SearchQueryRepository>(Context);
        }

        /// <summary>
        /// Gets or sets the repository for persisted <see cref="SearchQuery"/> records and query reports.
        /// </summary>
        /// <value>The repository created for this service's request context.</value>
        /// <remarks>
        /// The protected setter permits derived services to replace the repository while preserving the
        /// service's public composition contract.
        /// </remarks>
        public SearchQueryRepository SearchQueries { get; protected set; }
    }
}
