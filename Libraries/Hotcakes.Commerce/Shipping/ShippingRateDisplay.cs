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
using System.Collections.ObjectModel;
using Hotcakes.Commerce.Orders;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Shipping
{
    public class ShippingRateDisplay
    {
        public ShippingRateDisplay()
        {
            DisplayName = string.Empty;
            ProviderId = string.Empty;
            ProviderServiceCode = string.Empty;
            ShippingMethodId = string.Empty;
            ResponseMessage = string.Empty;
            SuggestedPackages = new Collection<OrderPackage>();
        }

        public ShippingRateDisplay(IShippingRate rate)
            : this()
        {
            DisplayName = rate.DisplayName;
            Rate = rate.EstimatedCost;
            ProviderId = rate.ServiceId;
            ProviderServiceCode = rate.ServiceCodes;
        }

        public ShippingRateDisplay(string name, string shipProviderId, string shipProviderServiceCode, decimal totalRate,
            string shipMethodId)
            : this()
        {
            DisplayName = name;
            ProviderId = shipProviderId;
            ProviderServiceCode = shipProviderServiceCode;
            Rate = totalRate;
            ShippingMethodId = shipMethodId;
        }

        public string DisplayName { get; set; }
        public string ProviderId { get; set; }
        public string ProviderServiceCode { get; set; }
        public decimal Rate { get; set; }
        public string ShippingMethodId { get; set; }
        public string ResponseMessage { get; set; }
        public decimal PotentialDiscount { get; set; }
        public Collection<OrderPackage> SuggestedPackages { get; set; }

        public string RateAndNameForDisplay
        {
            get
            {
                var result = string.Empty;
                if (PotentialDiscount > 0)
                    result = string.Format("<span class=\"hc-shipping-discount\">{0:C}</span>{1:C} - {2}", Rate,
                        Rate - PotentialDiscount, DisplayName);
                else
                    result = string.Format("{0:C} - {1}", Rate, DisplayName);

                return result;
            }
        }

        public string UniqueKey
        {
            get { return ShippingMethodId + ProviderId + ProviderServiceCode; }
        }

        public ShippingRateDisplay GetCopy()
        {
            var result = new ShippingRateDisplay();

            result.DisplayName = DisplayName;
            result.ProviderId = ProviderId;
            result.ProviderServiceCode = ProviderServiceCode;
            result.Rate = Rate;
            result.ShippingMethodId = ShippingMethodId;
            result.ResponseMessage = ResponseMessage;

            foreach (var item in SuggestedPackages)
            {
                result.SuggestedPackages.Add(item.Clone());
            }

            return result;
        }

        public ShippingRateDisplay AdjustRate(ShippingMethodAdjustmentType adjustmentType, decimal amount)
        {
            if (Rate != 0)
            {
                decimal adjustment = 0;
                switch (adjustmentType)
                {
                    case ShippingMethodAdjustmentType.None:
                        return this;
                    case ShippingMethodAdjustmentType.Amount:
                        adjustment = amount;
                        break;
                    case ShippingMethodAdjustmentType.Percentage:
                        adjustment = Math.Round(Rate*(amount/100m), 2);
                        break;
                }
                Rate += adjustment;
            }
            return this;
        }
    }
}