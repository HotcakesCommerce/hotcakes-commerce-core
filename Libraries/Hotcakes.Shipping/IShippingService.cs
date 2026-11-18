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

using System.Collections.Generic;

namespace Hotcakes.Shipping
{
    /// <summary>
    /// Defines a provider that calculates shipping rates and optionally creates tracking links.
    /// </summary>
    /// <remarks>
    /// Implementations receive an <see cref="IShipment" /> and return <see cref="IShippingRate" /> values.
    /// Provider diagnostics and errors can be exposed through <see cref="LatestMessages" />.
    /// </remarks>
    public interface IShippingService
    {
        /// <summary>
        /// Gets the display name of the shipping service.
        /// </summary>
        string Name { get; }

        /// <summary>
        /// Gets the stable identifier of the shipping service.
        /// </summary>
        string Id { get; }

        /// <summary>
        /// Gets a value indicating whether the service supports shipment tracking.
        /// </summary>
        bool IsSupportsTracking { get; }

        /// <summary>
        /// Gets the service's configured settings.
        /// </summary>
        ServiceSettings BaseSettings { get; }

        /// <summary>
        /// Gets or sets messages produced by the most recent service operation.
        /// </summary>
        List<ShippingServiceMessage> LatestMessages { get; set; }

        /// <summary>
        /// Calculates rates for the specified shipment.
        /// </summary>
        /// <param name="shipment">The <see cref="IShipment" /> for which rates are requested.</param>
        /// <returns>A list of available <see cref="IShippingRate" /> values.</returns>
        List<IShippingRate> RateShipment(IShipment shipment);

        /// <summary>
        /// Lists the service codes supported by this provider.
        /// </summary>
        /// <returns>A list of supported <see cref="IServiceCode" /> values.</returns>
        List<IServiceCode> ListAllServiceCodes();

        /// <summary>
        /// Builds a tracking URL for the supplied tracking code.
        /// </summary>
        /// <param name="trackingCode">The provider-issued tracking code.</param>
        /// <returns>A URL that can be used to view tracking information.</returns>
        string GetTrackingUrl(string trackingCode);
    }
}
