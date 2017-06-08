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
using System.ComponentModel.DataAnnotations;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Shipping;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The EstimateShippingViewModel will be called upon if the customer chooses to
    ///     estimate the cost of shipping before proceeding to checkout.
    /// </summary>
    [Serializable]
    public class EstimateShippingViewModel
    {
        /// <summary>
        ///     Set default values
        /// </summary>
        public EstimateShippingViewModel()
        {
            CountryId = Country.UnitedStatesCountryBvin;
            RegionId = string.Empty;
            PostalCode = string.Empty;
            City = string.Empty;

            Rates = new List<ShippingRateDisplay>();
        }

        /// <summary>
        ///     Unique identifier of the country where the order will be shipped.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string CountryId { get; set; }

        /// <summary>
        ///     Unique identifier of the region where the order will be shipped.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string RegionId { get; set; }

        /// <summary>
        ///     Postal code where the order will be shipped to.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string PostalCode { get; set; }

        /// <summary>
        ///     The name of the city to ship to.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public string City { get; set; }

        /// <summary>
        ///     Lists the rates returned from estimating the current order shipping costs.
        /// </summary>
        [DisplayFormat(ConvertEmptyStringToNull = false)]
        public List<ShippingRateDisplay> Rates { get; set; }
    }
}