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

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    /// Provides the globalization repositories used within a commerce request context.
    /// </summary>
    /// <remarks>
    /// The service composes <see cref="CountryRepository"/> and <see cref="RegionRepository"/>
    /// instances through the established <see cref="Factory.CreateRepo{T}(HccRequestContext)"/>
    /// mechanism. Repository creation therefore retains the context and store scoping supplied
    /// to the base <see cref="HccServiceBase"/>.
    /// </remarks>
    public class GlobalizationService : HccServiceBase
    {
        /// <summary>
        /// Initializes a globalization service for the specified commerce request context.
        /// </summary>
        /// <param name="context">The request context used to compose the repositories.</param>
        public GlobalizationService(HccRequestContext context)
            : base(context)
        {
            Countries = Factory.CreateRepo<CountryRepository>(Context);
            Regions = Factory.CreateRepo<RegionRepository>(Context);
        }

        /// <summary>
        /// Gets the country repository composed for this service's request context.
        /// </summary>
        /// <value>The context-bound country repository.</value>
        public CountryRepository Countries { get; protected set; }

        /// <summary>
        /// Gets the region repository composed for this service's request context.
        /// </summary>
        /// <value>The context-bound region repository.</value>
        public RegionRepository Regions { get; protected set; }
    }
}
