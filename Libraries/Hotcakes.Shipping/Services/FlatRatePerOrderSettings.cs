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
    /// Defines the flat shipping amount charged once for an order.
    /// </summary>
    /// <seealso cref="ServiceSettings"/>
    /// <seealso cref="FlatRatePerOrder"/>
    [Serializable]
    public class FlatRatePerOrderSettings : ServiceSettings
    {
        /// <summary>
        /// Gets or sets the shipping amount applied to the order.
        /// </summary>
        /// <value>The per-order amount stored under the <c>Amount</c> setting key.</value>
        /// <seealso cref="ServiceSettings.GetDecimalSetting(string)"/>
        /// <seealso cref="ServiceSettings.SetDecimalSetting(string, decimal)"/>
        public decimal Amount
        {
            get { return GetDecimalSetting("Amount"); }
            set { SetDecimalSetting("Amount", value); }
        }
    }
}
