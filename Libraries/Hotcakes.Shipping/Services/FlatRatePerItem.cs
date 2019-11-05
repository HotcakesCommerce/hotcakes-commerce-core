#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    [Serializable]
    public class FlatRatePerItem : IShippingService
    {
        private List<ShippingServiceMessage> _Messages = new List<ShippingServiceMessage>();

        public FlatRatePerItem()
        {
            Settings = new FlatRatePerItemSettings();
        }

        public FlatRatePerItemSettings Settings { get; set; }

        public string Name
        {
            get { return "Flat Rate Per Item"; }
        }

        public string Id
        {
            get { return "3D6623E7-1E2C-444d-B860-A8F542133093"; }
        }

        public bool IsSupportsTracking
        {
            get { return false; }
        }

        public ServiceSettings BaseSettings
        {
            get { return Settings; }
        }

        public List<IServiceCode> ListAllServiceCodes()
        {
            return new List<IServiceCode>();
        }

        public List<IShippingRate> RateShipment(IShipment shipment)
        {
            var totalItems = 0;

            foreach (var item in shipment.Items)
            {
                totalItems += item.QuantityOfItemsInBox;
            }

            var perItemAmount = Settings.Amount;

            var r = new ShippingRate
            {
                ServiceId = Id,
                EstimatedCost = perItemAmount*totalItems
            };

            var rates = new List<IShippingRate> {r};

            return rates;
        }

        public List<ShippingServiceMessage> LatestMessages
        {
            get { return _Messages; }
            set { _Messages = value; }
        }

        public string GetTrackingUrl(string trackingCode)
        {
            return null;
        }
    }
}