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
    /// Calculates shipping from shipment weight using a base charge and a per-unit overage charge.
    /// </summary>
    /// <seealso cref="RatePerWeightFormulaSettings"/>
    /// <seealso cref="IShippingService"/>
    [Serializable]
    public class RatePerWeightFormula : IShippingService
    {
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        /// <summary>
        /// Initializes a new instance with default <see cref="RatePerWeightFormulaSettings"/>.
        /// </summary>
        public RatePerWeightFormula()
        {
            Settings = new RatePerWeightFormulaSettings();
        }

        /// <summary>
        /// Gets or sets the weight formula settings used to calculate rates.
        /// </summary>
        /// <seealso cref="BaseSettings"/>
        public RatePerWeightFormulaSettings Settings { get; set; }

        /// <summary>
        /// Gets the display name of this shipping service.
        /// </summary>
        public string Name
        {
            get { return "Rate Per Weight Formula"; }
        }

        /// <summary>
        /// Gets the stable identifier for this shipping service.
        /// </summary>
        public string Id
        {
            get { return "5AAF9016-B03F-4e7c-8596-193F5EFFFDC3"; }
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
        /// Calculates a rate from the total shipment weight and the configured weight formula.
        /// </summary>
        /// <param name="shipment">The shipment to rate.</param>
        /// <returns>A list containing one <see cref="ShippingRate"/>, or an empty list when the shipment weight is outside the configured bounds.</returns>
        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            var rates = new List<IShippingRate>();

            // Total Up Weight
            decimal totalWeight = 0;
            foreach (var item in shipment.Items)
            {
                totalWeight += item.BoxWeight;
            }

            // Check for max weight
            if (totalWeight > Settings.MaxWeight || totalWeight < Settings.MinWeight)
            {
                return rates;
            }

            // Calculate Overage
            decimal extraWeight = 0;
            if (totalWeight > Settings.BaseWeight)
            {
                extraWeight = totalWeight - Settings.BaseWeight;
            }
            var extraWeightWhole = (int) Math.Ceiling(extraWeight);

            // Base + Overage Charges
            var theRate = Settings.BaseAmount + extraWeightWhole*Settings.AdditionalWeightCharge;

            var r = new ShippingRate
            {
                ServiceId = Id,
                EstimatedCost = theRate
            };

            rates.Add(r);

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
