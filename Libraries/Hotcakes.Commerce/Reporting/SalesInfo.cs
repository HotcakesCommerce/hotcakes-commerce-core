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
using System.Globalization;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Contains sales totals, order counts, device breakdowns, and chart data for a reporting period.</summary>
    public class SalesInfo
    {
        /// <summary>Gets or sets the labels corresponding to <see cref="ChartData"/>.</summary>
        public List<string> ChartLabels { get; set; }
        /// <summary>Gets or sets the sales values corresponding to <see cref="ChartLabels"/>.</summary>
        public List<decimal> ChartData { get; set; }

        /// <summary>Gets or sets the total number of orders.</summary>
        public int OrdersCount { get; set; }
        /// <summary>Gets or sets the number of completed orders.</summary>
        public int OrdersCompleted { get; set; }

        /// <summary>Gets the rounded percentage of orders that were not completed; returns zero when there are no orders.</summary>
        /// <remarks>The value is calculated from <see cref="OrdersCount"/> and <see cref="OrdersCompleted"/>.</remarks>
        public int OrdersAbandonedPercent
        {
            get
            {
                if (OrdersCount > 0)
                {
                    return (int) Math.Round(
                        (1 - (decimal) OrdersCompleted/OrdersCount)*100
                        );
                }

                return 0;
            }
        }

        /// <summary>Gets or sets the total value of orders.</summary>
        public decimal OrdersTotalSum { get; set; }
        /// <summary>Gets or sets the number of successful transactions.</summary>
        public int OrdersSuccessfulTransactions { get; set; }

        /// <summary>Gets or sets the number of sales made from desktop devices.</summary>
        public int SalesByDesktopCount { get; set; }
        /// <summary>Gets or sets the current desktop sales percentage.</summary>
        public decimal SalesByDesktopPercent { get; set; }
        /// <summary>Gets or sets the comparison-period desktop sales percentage.</summary>
        public decimal SalesByDesktopPercentPrev { get; set; }
        /// <summary>Gets or sets the number of sales made from tablet devices.</summary>
        public int SalesByTabletCount { get; set; }
        /// <summary>Gets or sets the current tablet sales percentage.</summary>
        public decimal SalesByTabletPercent { get; set; }
        /// <summary>Gets or sets the comparison-period tablet sales percentage.</summary>
        public decimal SalesByTabletPercentPrev { get; set; }
        /// <summary>Gets or sets the number of sales made from phone devices.</summary>
        public int SalesByPhoneCount { get; set; }
        /// <summary>Gets or sets the current phone sales percentage.</summary>
        public decimal SalesByPhonePercent { get; set; }
        /// <summary>Gets or sets the comparison-period phone sales percentage.</summary>
        public decimal SalesByPhonePercentPrev { get; set; }
    }

    /// <summary>Provides the display-ready JSON projection of <see cref="SalesInfo"/>.</summary>
    public class SalesInfoJson
    {
        /// <summary>Initializes an empty JSON projection.</summary>
        public SalesInfoJson()
        {
        }

        /// <summary>Initializes a localized projection from sales data.</summary>
        /// <param name="info">The source sales data.</param>
        /// <param name="localization">The helper used for comparison labels.</param>
        public SalesInfoJson(SalesInfo info, ILocalizationHelper localization)
        {
            ChartLabels = info.ChartLabels;
            ChartData = info.ChartData;
            OrdersCount = info.OrdersCount;
            OrdersCompleted = info.OrdersCompleted;
            OrdersAbandonedPercent = info.OrdersAbandonedPercent;
            OrdersTotalSum = Money.GetFriendlyAmount(info.OrdersTotalSum, CultureInfo.CurrentUICulture);
            OrdersSuccessfulTransactions = info.OrdersSuccessfulTransactions;

            var noChangeString = localization.GetString("NoChange");
            var salesByDeviceComparisonString = localization.GetString("SalesByDeviceComparison");

            var ci = CultureInfo.CurrentUICulture;
            SalesByDesktopCount = info.SalesByDesktopCount;
            SalesByDesktopComparison = noChangeString;
            if (Math.Abs(info.SalesByDesktopPercent - info.SalesByDesktopPercentPrev) > 0.01m)
            {
                SalesByDesktopGrowing = info.SalesByDesktopPercent > info.SalesByDesktopPercentPrev;
                SalesByDesktopComparison = string.Format(salesByDeviceComparisonString, info.SalesByDesktopPercent,
                    info.SalesByDesktopPercentPrev);
            }

            SalesByTabletCount = info.SalesByTabletCount;
            SalesByTabletComparison = noChangeString;
            if (Math.Abs(info.SalesByTabletPercent - info.SalesByTabletPercentPrev) > 0.01m)
            {
                SalesByTabletGrowing = info.SalesByTabletPercent > info.SalesByTabletPercentPrev;
                SalesByTabletComparison = string.Format(salesByDeviceComparisonString, info.SalesByTabletPercent,
                    info.SalesByTabletPercentPrev);
            }

            SalesByPhoneCount = info.SalesByPhoneCount;
            SalesByPhoneComparison = noChangeString;
            if (Math.Abs(info.SalesByPhonePercent - info.SalesByPhonePercentPrev) > 0.01m)
            {
                SalesByPhoneGrowing = info.SalesByPhonePercent > info.SalesByPhonePercentPrev;
                SalesByPhoneComparison = string.Format(salesByDeviceComparisonString, info.SalesByPhonePercent,
                    info.SalesByPhonePercentPrev);
            }
        }

        /// <summary>Gets or sets the chart labels.</summary>
        public List<string> ChartLabels { get; set; }
        /// <summary>Gets or sets the chart values.</summary>
        public List<decimal> ChartData { get; set; }
        /// <summary>Gets or sets the total order count.</summary>
        public int OrdersCount { get; set; }
        /// <summary>Gets or sets the completed order count.</summary>
        public int OrdersCompleted { get; set; }
        /// <summary>Gets or sets the abandoned-order percentage.</summary>
        public int OrdersAbandonedPercent { get; set; }
        /// <summary>Gets or sets the localized order total.</summary>
        public string OrdersTotalSum { get; set; }
        /// <summary>Gets or sets the successful transaction count.</summary>
        public int OrdersSuccessfulTransactions { get; set; }

        /// <summary>Gets or sets the desktop sales count.</summary>
        public int SalesByDesktopCount { get; set; }
        /// <summary>Gets or sets the localized desktop comparison.</summary>
        public string SalesByDesktopComparison { get; set; }
        /// <summary>Gets whether desktop sales are growing; null indicates the comparison threshold was not exceeded.</summary>
        public bool? SalesByDesktopGrowing { get; set; }
        /// <summary>Gets or sets the tablet sales count.</summary>
        public int SalesByTabletCount { get; set; }
        /// <summary>Gets or sets the localized tablet comparison.</summary>
        public string SalesByTabletComparison { get; set; }
        /// <summary>Gets whether tablet sales are growing; null indicates the comparison threshold was not exceeded.</summary>
        public bool? SalesByTabletGrowing { get; set; }
        /// <summary>Gets or sets the phone sales count.</summary>
        public int SalesByPhoneCount { get; set; }
        /// <summary>Gets or sets the localized phone comparison.</summary>
        public string SalesByPhoneComparison { get; set; }
        /// <summary>Gets whether phone sales are growing; null indicates the comparison threshold was not exceeded.</summary>
        public bool? SalesByPhoneGrowing { get; set; }
    }
}
