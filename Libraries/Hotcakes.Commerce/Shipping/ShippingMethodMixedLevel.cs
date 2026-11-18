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

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>Represents a shipping level combining a fixed amount and a percentage.</summary>
    public class ShippingMethodMixedLevel
    {
        private string _Bvin = Guid.NewGuid().ToString();

        /// <summary>Initializes a mixed level with zero threshold, fixed amount, and percentage.</summary>
        public ShippingMethodMixedLevel()
        {
            Percentage = 0m;
            FixedAmount = 0m;
            Level = 0m;
        }

        /// <summary>Initializes a mixed level with the specified threshold and adjustments.</summary>
        /// <param name="levelValue">The threshold at which this level applies.</param>
        /// <param name="fixedAmountValue">The fixed amount added to the calculated amount.</param>
        /// <param name="percentage">The percentage applied to the order subtotal.</param>
        public ShippingMethodMixedLevel(decimal levelValue, decimal fixedAmountValue, decimal percentage)
        {
            Level = levelValue;
            FixedAmount = fixedAmountValue;
            Percentage = percentage;
        }

        /// <summary>Gets or sets the identifier of this mixed level.</summary>
        public string Bvin
        {
            get { return _Bvin; }
            set { _Bvin = value; }
        }

        /// <summary>Gets or sets the threshold at which this level applies.</summary>
        public decimal Level { get; set; }

        /// <summary>Gets or sets the fixed amount added to the percentage result.</summary>
        public decimal FixedAmount { get; set; }

        /// <summary>Gets or sets the percentage applied to the order subtotal.</summary>
        public decimal Percentage { get; set; }

        /// <summary>Calculates the combined percentage and fixed amount for an order subtotal.</summary>
        /// <param name="orderSubTotal">The order subtotal to which <see cref="Percentage"/> is applied.</param>
        /// <returns>The percentage of the subtotal plus <see cref="FixedAmount"/>.</returns>
        public decimal TotalAmount(decimal orderSubTotal)
        {
            var result = orderSubTotal*(Percentage/100) + FixedAmount;
            return result;
        }
    }
}
