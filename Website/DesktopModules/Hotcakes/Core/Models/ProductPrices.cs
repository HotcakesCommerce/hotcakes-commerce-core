#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using System.Threading;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     Product price information to be shown on product detail page
    /// </summary>
    [Serializable]
    public class ProductPrices
    {
        /// <summary>
        ///     Different price available for the product
        /// </summary>
        public PriceData ListPrice;

        public PriceData SitePrice;
        public PriceData YouSave;

        /// <summary>
        ///     Generate the different type of price information for the product
        /// </summary>
        /// <param name="price">
        ///     User specific price information based on the chosen option for the product on the product detail
        ///     page
        /// </param>
        public ProductPrices(UserSpecificPrice price)
        {
            if (price.ListPriceGreaterThanCurrentPrice)
            {
                ListPrice = new PriceData
                {
                    Label = GlobalLocalization.GetString("ListPrice"),
                    Text = price.ListPrice.ToString("C"),
                    RawValue = price.ListPrice.ToString("F")
                };
            }

            SitePrice = new PriceData
            {
                Label = GlobalLocalization.GetString("SitePrice"),
                Text = price.DisplayPrice(true, false),
                RawValue = price.DisplayPrice(true, false)
            };

            if (price.BasePrice < price.ListPrice && price.OverrideText.Trim().Length < 1)
            {
                YouSave = new PriceData
                {
                    Label = GlobalLocalization.GetString("YouSave"),
                    Text =
                        string.Format("{0} - {1}{2}", price.Savings.ToString("c"), price.SavingsPercent,
                            Thread.CurrentThread.CurrentUICulture.NumberFormat.PercentSymbol),
                    RawValue = price.SavingsPercent.ToString("N")
                };
            }
        }

        /// <summary>
        ///     Specific price infomration
        /// </summary>
        public class PriceData
        {
            /// <summary>
            ///     label to be shown when display price
            /// </summary>
            public string Label { get; set; }

            /// <summary>
            ///     Text to be shown for price
            /// </summary>
            public string Text { get; set; }

            /// <summary>
            ///     actual decimal value of the price for calculation
            /// </summary>
            public string RawValue { get; set; }
        }
    }
}