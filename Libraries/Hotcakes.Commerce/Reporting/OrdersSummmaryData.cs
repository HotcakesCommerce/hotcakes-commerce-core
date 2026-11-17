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

namespace Hotcakes.Commerce.Reporting
{
    /// <summary>Contains order counts grouped by the operational status used by the dashboard.</summary>
    /// <remarks>The source file name and type name retain the compatibility-sensitive spelling of this API.</remarks>
    public class OrdersSummaryData
    {
        /// <summary>Gets or sets the number of new orders.</summary>
        public int NewCount { get; set; }
        /// <summary>Gets or sets the number of orders currently on hold.</summary>
        public int OnHoldCount { get; set; }
        /// <summary>Gets or sets the number of orders ready for payment.</summary>
        public int ReadyForPaymentCount { get; set; }
        /// <summary>Gets or sets the number of orders ready for shipping.</summary>
        public int ReadyForShippingCount { get; set; }
    }
}
