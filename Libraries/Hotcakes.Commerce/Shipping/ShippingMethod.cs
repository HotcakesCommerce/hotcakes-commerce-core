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
using System.Collections.ObjectModel;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Orders;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Describes a configured shipping method and obtains rates from its associated provider.
    /// </summary>
    /// <remarks>
    /// The method belongs to a store through <see cref="StoreId"/> and identifies its provider through
    /// <see cref="ShippingProviderId"/>. Rate adjustments are applied according to <see cref="AdjustmentType"/>.
    /// </remarks>
    [Serializable]
    public class ShippingMethod
    {
        /// <summary>Gets the compatibility identifier used when no shipping method is available.</summary>
        public const string MethodUnknown = "UNKNOWN";

        /// <summary>Gets the compatibility identifier used while a shipping method is not yet determined.</summary>
        public const string MethodToBeDetermined = "TOBEDETERMINED";

        /// <summary>Gets the compatibility identifier for free shipping.</summary>
        public const string MethodFreeShipping = "FREESHIPPING";

        // Variables    
        private decimal _Adjustment;
        private ShippingMethodAdjustmentType _AdjustmentType = ShippingMethodAdjustmentType.None;

        private string _bvin = string.Empty;
        private DateTime _LastUpdated = DateTime.MinValue;
        private string _Name = string.Empty;
        private ServiceSettings _Settings = new ServiceSettings();
        private string _ShippingProviderId = string.Empty;
        private long _ZoneId;

        /// <summary>
        /// Initializes a shipping method with no store association.
        /// </summary>
        public ShippingMethod()
        {
            StoreId = 0;
        }

        /// <summary>Gets or sets the string identifier of this shipping method.</summary>
        public virtual string Bvin
        {
            get { return _bvin; }
            set { _bvin = value; }
        }

        /// <summary>Gets or sets the date and time when this shipping method was last updated.</summary>
        public virtual DateTime LastUpdated
        {
            get { return _LastUpdated; }
            set { _LastUpdated = value; }
        }

        /// <summary>
        /// Gets or sets the adjustment applied to provider rates.
        /// </summary>
        /// <remarks>For <see cref="ShippingMethodAdjustmentType.Amount"/>, this is added as a fixed amount; for
        /// <see cref="ShippingMethodAdjustmentType.Percentage"/>, it is interpreted as a percentage of each rate.</remarks>
        public decimal Adjustment
        {
            get { return _Adjustment; }
            set { _Adjustment = value; }
        }

        /// <summary>Gets or sets how <see cref="Adjustment"/> is applied to provider rates.</summary>
        public ShippingMethodAdjustmentType AdjustmentType
        {
            get { return _AdjustmentType; }
            set { _AdjustmentType = value; }
        }

        /// <summary>Gets or sets the display name of the shipping method.</summary>
        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        /// <summary>Gets or sets the identifier of the shipping provider used to retrieve rates.</summary>
        public string ShippingProviderId
        {
            get { return _ShippingProviderId; }
            set { _ShippingProviderId = value; }
        }

        /// <summary>Gets or sets the zone associated with this shipping method.</summary>
        public long ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; }
        }

        /// <summary>
        /// Gets or sets the provider settings merged into the selected shipping service before rate retrieval.
        /// </summary>
        /// <remarks>The getter creates an empty <see cref="ServiceSettings"/> instance when the backing value is null.</remarks>
        public ServiceSettings Settings
        {
            get
            {
                if (_Settings == null) _Settings = new ServiceSettings();
                return _Settings;
            }
            set { _Settings = value; }
        }

        /// <summary>Gets or sets the store identifier to which this method belongs.</summary>
        public long StoreId { get; set; }

        /// <summary>Gets or sets the visibility rule used when considering this method for rates.</summary>
        public ShippingVisibilityMode VisibilityMode { get; set; }

        /// <summary>
        /// Gets or sets the threshold associated with <see cref="VisibilityMode"/>.
        /// </summary>
        /// <remarks>The value is used by visibility modes that require an amount or threshold; it is nullable for modes that do not.</remarks>
        public decimal? VisibilityAmount { get; set; }

        /// <summary>Gets or sets the order in which this method is presented relative to other methods.</summary>
        public int SortOrder { get; set; }

        private IShipment ConvertGroupsToShipments(List<ShippingGroup> groups)
        {
            IShipment result = new Shipment();
            foreach (var g in groups)
            {
                result.Items.Add(g.AsIShippable());
                result.DestinationAddress = g.DestinationAddress;
                result.SourceAddress = g.SourceAddress;
            }
            return result;
        }

        /// <summary>
        /// Retrieves provider rates for the shipping groups associated with this method and applies the configured adjustment.
        /// </summary>
        /// <param name="o">The order whose shipping groups and shipping address are used.</param>
        /// <param name="currentStore">The store used to resolve <see cref="ShippingProviderId"/>.</param>
        /// <returns>A collection of display rates, or an empty collection when the provider cannot be resolved or returns no rates.</returns>
        /// <remarks>The provider settings are merged with the payer name before the provider is called. A shipment with no items produces rates with a zero rate.</remarks>
        public Collection<ShippingRateDisplay> GetRates(Order o, Store currentStore)
        {
            var groups = o.GetShippingGroups(Bvin);
            var result = new Collection<ShippingRateDisplay>();
            var p = AvailableServices.FindById(ShippingProviderId, currentStore);

            if (p != null)
            {
                Settings.Add("PayerName", (o.ShippingAddress.FirstName + " " + o.ShippingAddress.LastName));
                p.BaseSettings.Clear();
                p.BaseSettings.Merge(Settings);

                var shipment = ConvertGroupsToShipments(groups);
                var tempRates = p.RateShipment(shipment);

                if (tempRates != null)
                {
                    for (var i = 0; i <= tempRates.Count - 1; i++)
                    {
                        var r = new ShippingRateDisplay(tempRates[i])
                        {
                            ShippingMethodId = Bvin
                        };

                        if (r.DisplayName == string.Empty)
                        {
                            r.DisplayName = Name;
                        }

                        AdjustRate(r);

                        // Free shipping if no included items
                        if (shipment.Items.Count < 1)
                        {
                            r.Rate = 0;
                        }

                        result.Add(r);
                    }
                }
            }

            return result;
        }

        private void AdjustRate(ShippingRateDisplay r)
        {
            switch (AdjustmentType)
            {
                case ShippingMethodAdjustmentType.Amount:
                    r.Rate = r.Rate + Adjustment;
                    break;
                case ShippingMethodAdjustmentType.None:
                    // Do Nothing
                    break;
                case ShippingMethodAdjustmentType.Percentage:
                    r.Rate = r.Rate + r.Rate*(Adjustment/100m);
                    break;
            }
        }
    }
}
