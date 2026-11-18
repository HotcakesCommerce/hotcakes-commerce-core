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

using Hotcakes.Web.Geography;

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Defines address validation and normalization operations supplied by a shipping integration.
    /// </summary>
    /// <remarks>
    /// Implementations operate on the shared geography model represented by <see cref="IAddress" />.
    /// The <c>message</c> output should provide an actionable explanation when an operation cannot be completed.
    /// </remarks>
    public interface IAddressProvider
    {
        /// <summary>
        /// Determines whether an address is acceptable for the provider's shipping operations.
        /// </summary>
        /// <param name="address">The <see cref="IAddress" /> to validate.</param>
        /// <param name="message">Receives a provider-specific explanation of the validation result.</param>
        /// <returns><c>true</c> when the address is valid; otherwise, <c>false</c>.</returns>
        bool ValidateAddress(IAddress address, out string message);

        /// <summary>
        /// Normalizes the supplied address into the provider's preferred representation.
        /// </summary>
        /// <param name="address">The <see cref="IAddress" /> to normalize.</param>
        /// <param name="message">Receives a provider-specific explanation of the normalization result.</param>
        /// <returns><c>true</c> when normalization succeeds; otherwise, <c>false</c>.</returns>
        bool NormalizeAddress(IAddress address, out string message);
    }
}
