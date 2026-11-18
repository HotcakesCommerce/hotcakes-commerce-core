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

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Represents the origin or destination address supplied to a tax provider.
    /// </summary>
    /// <remarks>
    /// The properties are provider-neutral address fields. Providers may require only a subset of them,
    /// and their interpretation is delegated to the selected <see cref="ITaxProvider"/>.
    /// </remarks>
    [Serializable]
    public class BaseAddress
    {
        /// <summary>Gets or sets the city or locality.</summary>
        public string City { get; set; }

        /// <summary>Gets or sets the country or country code.</summary>
        public string Country { get; set; }

        /// <summary>Gets or sets the first street-address line.</summary>
        public string Line1 { get; set; }

        /// <summary>Gets or sets the second street-address line.</summary>
        public string Line2 { get; set; }

        /// <summary>Gets or sets the third street-address line.</summary>
        public string Line3 { get; set; }

        /// <summary>Gets or sets the postal or ZIP code.</summary>
        public string PostalCode { get; set; }

        /// <summary>Gets or sets the region, state, or province.</summary>
        public string Region { get; set; }
    }
}
