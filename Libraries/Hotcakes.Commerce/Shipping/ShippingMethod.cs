#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    [Serializable]
    public class ShippingMethod
    {
        public const string MethodUnknown = "UNKNOWN";
        public const string MethodToBeDetermined = "TOBEDETERMINED";
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

        // Methods
        public ShippingMethod()
        {
            StoreId = 0;
        }

        // Properties   
        public virtual string Bvin
        {
            get { return _bvin; }
            set { _bvin = value; }
        }

        public virtual DateTime LastUpdated
        {
            get { return _LastUpdated; }
            set { _LastUpdated = value; }
        }

        public decimal Adjustment
        {
            get { return _Adjustment; }
            set { _Adjustment = value; }
        }

        public ShippingMethodAdjustmentType AdjustmentType
        {
            get { return _AdjustmentType; }
            set { _AdjustmentType = value; }
        }

        public string Name
        {
            get { return _Name; }
            set { _Name = value; }
        }

        public string ShippingProviderId
        {
            get { return _ShippingProviderId; }
            set { _ShippingProviderId = value; }
        }

        public long ZoneId
        {
            get { return _ZoneId; }
            set { _ZoneId = value; }
        }

        public ServiceSettings Settings
        {
            get
            {
                if (_Settings == null) _Settings = new ServiceSettings();
                return _Settings;
            }
            set { _Settings = value; }
        }

        public long StoreId { get; set; }

        public ShippingVisibilityMode VisibilityMode { get; set; }

        public decimal? VisibilityAmount { get; set; }

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