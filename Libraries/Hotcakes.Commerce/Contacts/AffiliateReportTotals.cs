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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Aggregates order, referral, payment, sales, and commission values for an affiliate report.</summary>
    public class AffiliateReportTotals
    {
        /// <summary>Gets or sets the number of referred orders.</summary>
        public int OrdersCount { get; set; }
        /// <summary>Gets or sets the number of tracked referrals.</summary>
        public int ReferralsCount { get; set; }
        /// <summary>Gets or sets the number of recorded payments.</summary>
        public int PaymentsCount { get; set; }
        /// <summary>Gets or sets the total referred sales amount.</summary>
        public decimal SalesAmount { get; set; }
        /// <summary>Gets or sets the commission earned from referred sales.</summary>
        public decimal Commission { get; set; }
        /// <summary>Gets or sets the commission already paid.</summary>
        public decimal CommissionPaid { get; set; }

        /// <summary>Gets the difference between <see cref="Commission"/> and <see cref="CommissionPaid"/>.</summary>
        /// <remarks>This is calculated on demand and is not independently stored.</remarks>
        public decimal CommissionOwed
        {
            get { return Commission - CommissionPaid; }
        }
    }
}
