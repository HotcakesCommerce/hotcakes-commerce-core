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
    /// Calculates shipping per item by selecting a rate from a quantity-based <see cref="RateTableSettings"/>.
    /// </summary>
    /// <seealso cref="RateTableLevel.FindRateForLevel(decimal, List{RateTableLevel})"/>
    /// <seealso cref="IShippingService"/>
    [Serializable]
    public class PerItem : IShippingService
    {
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        /// <summary>
        /// Initializes a new instance with default <see cref="RateTableSettings"/>.
        /// </summary>
        public PerItem()
        {
            Settings = new RateTableSettings();
        }

        /// <summary>
        /// Gets or sets the quantity-based rate table used by this service.
        /// </summary>
        /// <seealso cref="BaseSettings"/>
        public RateTableSettings Settings { get; set; }

        /// <summary>
        /// Gets the display name of this shipping service.
        /// </summary>
        public string Name
        {
            get { return "Rate Table Per Item"; }
        }

        /// <summary>
        /// Gets the stable identifier for this shipping service.
        /// </summary>
        public string Id
        {
            get { return "41B590A7-003C-48d1-8446-EAE93C156AA1"; }
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
        /// Calculates a per-item rate by selecting the configured level for the total item count.
        /// </summary>
        /// <param name="shipment">The shipment to rate.</param>
        /// <returns>A list containing one <see cref="ShippingRate"/> with the selected per-item rate multiplied by the item count.</returns>
        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            var totalItems = 0;

            foreach (var item in shipment.Items)
            {
                totalItems += item.QuantityOfItemsInBox;
            }

            decimal amountPerItem = 0;
            if (Settings != null)
            {
                if (Settings.GetLevels() != null)
                {
                    var level = RateTableLevel.FindRateForLevel(totalItems, Settings.GetLevels());
                    if (level != null && level.Rate >= 0)
                    {
                        amountPerItem = level.Rate;
                    }
                }
            }

            var r = new ShippingRate
            {
                ServiceId = Id,
                EstimatedCost = amountPerItem*totalItems
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
