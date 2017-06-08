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
using System.Globalization;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Reporting
{
    public class SalesInfo
    {
        public List<string> ChartLabels { get; set; }
        public List<decimal> ChartData { get; set; }

        public int OrdersCount { get; set; }
        public int OrdersCompleted { get; set; }

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

        public decimal OrdersTotalSum { get; set; }
        public int OrdersSuccessfulTransactions { get; set; }

        public int SalesByDesktopCount { get; set; }
        public decimal SalesByDesktopPercent { get; set; }
        public decimal SalesByDesktopPercentPrev { get; set; }
        public int SalesByTabletCount { get; set; }
        public decimal SalesByTabletPercent { get; set; }
        public decimal SalesByTabletPercentPrev { get; set; }
        public int SalesByPhoneCount { get; set; }
        public decimal SalesByPhonePercent { get; set; }
        public decimal SalesByPhonePercentPrev { get; set; }
    }

    public class SalesInfoJson
    {
        public SalesInfoJson()
        {
        }

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

        public List<string> ChartLabels { get; set; }
        public List<decimal> ChartData { get; set; }

        public int OrdersCount { get; set; }
        public int OrdersCompleted { get; set; }
        public int OrdersAbandonedPercent { get; set; }
        public string OrdersTotalSum { get; set; }
        public int OrdersSuccessfulTransactions { get; set; }

        public int SalesByDesktopCount { get; set; }
        public string SalesByDesktopComparison { get; set; }
        public bool? SalesByDesktopGrowing { get; set; }

        public int SalesByTabletCount { get; set; }
        public string SalesByTabletComparison { get; set; }
        public bool? SalesByTabletGrowing { get; set; }

        public int SalesByPhoneCount { get; set; }
        public string SalesByPhoneComparison { get; set; }
        public bool? SalesByPhoneGrowing { get; set; }
    }
}