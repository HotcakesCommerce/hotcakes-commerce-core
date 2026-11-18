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

namespace Hotcakes.Modules.Core.Api.Mobile.Models
{
    /// <summary>
    /// Represents one point in a mobile sales series.
    /// </summary>
    [Serializable]
    public class Point
    {
        /// <summary>Gets or sets the period position on the horizontal axis.</summary>
        public int X { get; set; }
        /// <summary>Gets or sets the sales value on the vertical axis.</summary>
        public decimal Y { get; set; }
    }

    /// <summary>
    /// Contains sales and order counts for one reporting period.
    /// </summary>
    [Serializable]
    public class ReportPeriodSummary
    {
        /// <summary>Gets or sets the period name.</summary>
        public string Period { get; set; }
        /// <summary>Gets or sets the total sales value for the period.</summary>
        public decimal Total { get; set; }
        /// <summary>Gets or sets the sales series for the period.</summary>
        public List<Point> Series { get; set; }
        /// <summary>Gets or sets the number of orders in the period.</summary>
        public int OrdersCount { get; set; }
    }

    /// <summary>
    /// Contains the mobile API's store-level report summary.
    /// </summary>
    [Serializable]
    public class ReportSummary
    {
        /// <summary>Gets or sets the store display name.</summary>
        public string StoreName { get; set; }
        /// <summary>Gets or sets the number of orders ready for payment.</summary>
        public int ReadyForPaymentCount { get; set; }
        /// <summary>Gets or sets the number of orders ready for shipping.</summary>
        public int ReadyForShippingCount { get; set; }
        /// <summary>Gets or sets the summaries for the requested reporting periods.</summary>
        public List<ReportPeriodSummary> PeriodSummaries { get; set; }
    }
}
