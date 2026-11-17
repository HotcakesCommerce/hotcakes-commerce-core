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

using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Taxes.Providers
{
    /// <summary>
    /// Defines the integration contract for a store tax provider.
    /// </summary>
    /// <remarks>
    /// Implementations are discovered by <see cref="TaxProviders"/> and are selected by provider ID.
    /// Implementations receive the current <see cref="Order"/> and request context and are responsible
    /// for applying provider results to the order according to the provider's tax-document semantics.
    /// </remarks>
    public interface ITaxProvider
    {
        /// <summary>Gets the stable identifier used to select and persist this provider.</summary>
        string ProviderId { get; }

        /// <summary>Gets the display name of this provider.</summary>
        string ProviderName { get; }

        /// <summary>Gets the ordering index used when providers are listed.</summary>
        /// <remarks>Lower values are listed first by <see cref="TaxProviders.AvailableProviders"/>.</remarks>
        int SortIndex { get; }

        /// <summary>
        /// Gets the provider's settings object.
        /// </summary>
        /// <remarks>
        /// Store-specific settings are merged into this object by <see cref="TaxProviders.CurrentTaxProvider"/>
        /// and <see cref="TaxProviders.Find"/>. The property name is compatibility-sensitive.
        /// </remarks>
        TaxProviderSettings Basesettings { get; }

        /// <summary>Tests connectivity or credentials for the provider.</summary>
        /// <param name="hccContext">The current commerce request context.</param>
        /// <returns><see langword="true"/> when the provider connection test succeeds; otherwise, <see langword="false"/>.</returns>
        bool TestConnection(HccRequestContext hccContext);

        /// <summary>Calculates and applies taxes for the supplied order.</summary>
        /// <param name="order">The order to calculate taxes for.</param>
        /// <param name="hccContext">The current commerce request context.</param>
        /// <remarks>The implementation may update the order with line, shipping, and aggregate tax values.</remarks>
        void GetTaxes(Order order, HccRequestContext hccContext);

        /// <summary>Commits the order's tax document to the provider when supported.</summary>
        /// <param name="order">The order whose calculated tax document should be committed.</param>
        /// <param name="hccContext">The current commerce request context.</param>
        void CommitTaxes(Order order, HccRequestContext hccContext);

        /// <summary>Cancels the order's tax document with the provider when supported.</summary>
        /// <param name="order">The order whose tax document should be cancelled.</param>
        /// <param name="hccContext">The current commerce request context.</param>
        void CancelTaxDocument(Order order, HccRequestContext hccContext);
    }
}
