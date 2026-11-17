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
using System.Collections.ObjectModel;
using Hotcakes.Commerce.Orders;
using Hotcakes.Shipping;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Holds a shipping rate and the provider identifiers and package suggestions used to display or select it.
    /// </summary>
    public class ShippingRateDisplay
    {
        /// <summary>Initializes an empty display with empty identifiers, message, and package suggestions.</summary>
        public ShippingRateDisplay()
        {
            DisplayName = string.Empty;
            ProviderId = string.Empty;
            ProviderServiceCode = string.Empty;
            ShippingMethodId = string.Empty;
            ResponseMessage = string.Empty;
            SuggestedPackages = new Collection<OrderPackage>();
        }

        /// <summary>Initializes a display from a provider rate.</summary>
        /// <param name="rate">The provider rate whose name, cost, service identifier, and service codes are copied.</param>
        public ShippingRateDisplay(IShippingRate rate)
            : this()
        {
            DisplayName = rate.DisplayName;
            Rate = rate.EstimatedCost;
            ProviderId = rate.ServiceId;
            ProviderServiceCode = rate.ServiceCodes;
        }

        /// <summary>Initializes a display from explicit shipping method and provider values.</summary>
        /// <param name="name">The display name.</param>
        /// <param name="shipProviderId">The provider identifier.</param>
        /// <param name="shipProviderServiceCode">The provider service code.</param>
        /// <param name="totalRate">The displayed rate.</param>
        /// <param name="shipMethodId">The shipping method identifier.</param>
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

        /// <summary>Gets or sets the customer-facing rate name.</summary>
        public string DisplayName { get; set; }
        /// <summary>Gets or sets the provider identifier.</summary>
        public string ProviderId { get; set; }
        /// <summary>Gets or sets the provider service code or codes.</summary>
        public string ProviderServiceCode { get; set; }
        /// <summary>Gets or sets the rate before any potential discount display.</summary>
        public decimal Rate { get; set; }
        /// <summary>Gets or sets the selected shipping method identifier.</summary>
        public string ShippingMethodId { get; set; }
        /// <summary>Gets or sets a provider response or diagnostic message.</summary>
        public string ResponseMessage { get; set; }
        /// <summary>Gets or sets the potential discount used only when formatting <see cref="RateAndNameForDisplay"/>.</summary>
        public decimal PotentialDiscount { get; set; }
        /// <summary>Gets or sets the provider-suggested order packages.</summary>
        public Collection<OrderPackage> SuggestedPackages { get; set; }

        /// <summary>
        /// Gets the raw HTML fragment containing the formatted rate, optional discount, and display name.
        /// </summary>
        /// <remarks>The returned value is intentionally unencoded and includes the <c>hc-shipping-discount</c> span when applicable.</remarks>
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

        /// <summary>Gets the concatenated shipping method, provider, and service-code key.</summary>
        public string UniqueKey
        {
            get { return ShippingMethodId + ProviderId + ProviderServiceCode; }
        }

        /// <summary>Creates a copy of this display, including cloned suggested packages.</summary>
        /// <returns>A new display instance. <see cref="PotentialDiscount"/> is not copied.</returns>
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

        /// <summary>Adjusts the rate in place by a fixed amount or percentage.</summary>
        /// <param name="adjustmentType">The adjustment mode.</param>
        /// <param name="amount">The amount or percentage to apply.</param>
        /// <returns>This instance, except that a <see cref="ShippingMethodAdjustmentType.None"/> adjustment returns it unchanged.</returns>
        /// <remarks>A zero rate is not adjusted; percentage adjustments are rounded to two decimal places.</remarks>
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
