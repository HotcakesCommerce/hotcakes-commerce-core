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
using System.Collections.Generic;

namespace Hotcakes.Shipping.Services
{
    /// <summary>
    /// Calculates one configured flat shipping charge for the entire shipment.
    /// </summary>
    /// <seealso cref="FlatRatePerOrderSettings"/>
    /// <seealso cref="IShippingService"/>
    [Serializable]
    public class FlatRatePerOrder : IShippingService
    {
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        /// <summary>
        /// Initializes a new instance with default <see cref="FlatRatePerOrderSettings"/>.
        /// </summary>
        public FlatRatePerOrder()
        {
            Settings = new FlatRatePerOrderSettings();
        }

        /// <summary>
        /// Gets or sets the settings used to calculate the order charge.
        /// </summary>
        /// <seealso cref="BaseSettings"/>
        public FlatRatePerOrderSettings Settings { get; set; }

        /// <summary>
        /// Gets the display name of this shipping service.
        /// </summary>
        public string Name
        {
            get { return "Flat Rate Per Order"; }
        }

        /// <summary>
        /// Gets the stable identifier for this shipping service.
        /// </summary>
        public string Id
        {
            get { return "301AA2B8-F43C-42fe-B77E-A7E1CB1DD40E"; }
        }

        /// <summary>
        /// Gets a value indicating whether this service supports shipment tracking.
        /// </summary>
        public bool IsSupportsTracking
        {
            get { return false; }
        }

        /// <summary>
        /// Gets the configured settings through the common <see cref="ServiceSettings"/> base type.
        /// </summary>
        /// <seealso cref="Settings"/>
        public ServiceSettings BaseSettings
        {
            get { return Settings; }
        }

        /// <summary>
        /// Gets the service codes supported by this shipping service.
        /// </summary>
        /// <returns>An empty list because this built-in service does not expose service codes.</returns>
        public List<IServiceCode> ListAllServiceCodes()
        {
            return new List<IServiceCode>();
        }

        /// <summary>
        /// Calculates one flat shipping rate for <paramref name="shipment"/>.
        /// </summary>
        /// <param name="shipment">The shipment to rate.</param>
        /// <returns>A list containing one <see cref="ShippingRate"/> whose cost is the configured order amount.</returns>
        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            var r = new ShippingRate
            {
                ServiceId = Id,
                EstimatedCost = Settings.Amount
            };

            var rates = new List<IShippingRate> {r};

            return rates;
        }

        /// <summary>
        /// Gets or sets the latest messages produced by this service.
        /// </summary>
        public List<ShippingServiceMessage> LatestMessages
        {
            get { return _Messages; }
            set { _Messages = value; }
        }

        /// <summary>
        /// Gets a tracking URL for a tracking code.
        /// </summary>
        /// <param name="trackingCode">The carrier tracking code.</param>
        /// <returns>Always returns <see langword="null"/> because this service does not support tracking.</returns>
        public string GetTrackingUrl(string trackingCode)
        {
            return null;
        }
    }
}
