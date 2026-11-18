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

namespace Hotcakes.Shipping.Services
{
    /// <summary>
    /// Defines the values used to calculate shipping from a shipment's weight.
    /// </summary>
    /// <seealso cref="ServiceSettings"/>
    /// <seealso cref="RatePerWeightFormula"/>
    [Serializable]
    public class RatePerWeightFormulaSettings : ServiceSettings
    {
        /// <summary>
        /// Gets or sets the base shipping amount charged at the base weight.
        /// </summary>
        /// <value>The base amount stored under the <c>BaseAmount</c> setting key.</value>
        /// <seealso cref="ServiceSettings.GetDecimalSetting(string)"/>
        /// <seealso cref="ServiceSettings.SetDecimalSetting(string, decimal)"/>
        public decimal BaseAmount
        {
            get { return GetDecimalSetting("BaseAmount"); }
            set { SetDecimalSetting("BaseAmount", value); }
        }

        /// <summary>
        /// Gets or sets the weight at which the base amount applies.
        /// </summary>
        /// <value>The base weight stored under the <c>BaseWeight</c> setting key.</value>
        /// <seealso cref="ServiceSettings.GetDecimalSetting(string)"/>
        /// <seealso cref="ServiceSettings.SetDecimalSetting(string, decimal)"/>
        public decimal BaseWeight
        {
            get { return GetDecimalSetting("BaseWeight"); }
            set { SetDecimalSetting("BaseWeight", value); }
        }

        /// <summary>
        /// Gets or sets the charge applied for weight beyond the base weight.
        /// </summary>
        /// <value>The additional weight charge stored under the <c>AdditionalWeightCharge</c> setting key.</value>
        /// <seealso cref="ServiceSettings.GetDecimalSetting(string)"/>
        /// <seealso cref="ServiceSettings.SetDecimalSetting(string, decimal)"/>
        public decimal AdditionalWeightCharge
        {
            get { return GetDecimalSetting("AdditionalWeightCharge"); }
            set { SetDecimalSetting("AdditionalWeightCharge", value); }
        }

        /// <summary>
        /// Gets or sets the minimum shipment weight to which the formula applies.
        /// </summary>
        /// <value>The minimum weight stored under the <c>MinWeight</c> setting key.</value>
        /// <seealso cref="ServiceSettings.GetDecimalSetting(string)"/>
        /// <seealso cref="ServiceSettings.SetDecimalSetting(string, decimal)"/>
        public decimal MinWeight
        {
            get { return GetDecimalSetting("MinWeight"); }
            set { SetDecimalSetting("MinWeight", value); }
        }

        /// <summary>
        /// Gets or sets the maximum shipment weight to which the formula applies.
        /// </summary>
        /// <value>The maximum weight stored under the <c>MaxWeight</c> setting key.</value>
        /// <seealso cref="ServiceSettings.GetDecimalSetting(string)"/>
        /// <seealso cref="ServiceSettings.SetDecimalSetting(string, decimal)"/>
        public decimal MaxWeight
        {
            get { return GetDecimalSetting("MaxWeight"); }
            set { SetDecimalSetting("MaxWeight", value); }
        }
    }
}
