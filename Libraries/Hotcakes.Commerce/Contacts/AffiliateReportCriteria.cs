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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Defines filters and ordering for affiliate performance reports.</summary>
    /// <remarks>Date filtering and store scoping are applied by the affiliate repository implementation.</remarks>
    public class AffiliateReportCriteria
    {
        /// <summary>Specifies the affiliate field used for text searching.</summary>
        public enum SearchType
        {
            /// <summary>Searches the affiliate's last name.</summary>
            LastName = 0,
            /// <summary>Searches the affiliate's compatibility referral identifier.</summary>
            AffiliateId = 1,
            /// <summary>Searches the affiliate's email address.</summary>
            Email = 2,
            /// <summary>Searches the affiliate's company name.</summary>
            CompanyName = 3
        }

        /// <summary>Specifies the metric used to order report results.</summary>
        public enum SortingType
        {
            /// <summary>Orders by sales amount.</summary>
            Sales = 0,
            /// <summary>Orders by order count.</summary>
            Orders = 1,
            /// <summary>Orders by commission.</summary>
            Commission = 2,
            /// <summary>Orders by signup count.</summary>
            Signups = 3
        }

        /// <summary>Initializes the report to the broad historical date range used by the existing API.</summary>
        public AffiliateReportCriteria()
        {
            StartDateUtc = new DateTime(1900, 1, 1);
            EndDateUtc = new DateTime(3000, 1, 1);
        }

        /// <summary>Gets or sets the inclusive UTC start date for report activity.</summary>
        public DateTime StartDateUtc { get; set; }

        /// <summary>Gets or sets the inclusive UTC end date for report activity.</summary>
        public DateTime EndDateUtc { get; set; }

        /// <summary>Gets or sets the optional text matched according to <see cref="SearchBy"/>.</summary>
        public string SearchText { get; set; }

        /// <summary>Gets or sets the affiliate field used for <see cref="SearchText"/>.</summary>
        public SearchType SearchBy { get; set; }

        /// <summary>Gets or sets whether only affiliates that are not approved are included.</summary>
        public bool ShowOnlyNonApproved { get; set; }

        /// <summary>Gets or sets whether the report is limited to affiliates with commission owed.</summary>
        public bool ShowCommissionOwed { get; set; }

        /// <summary>Gets or sets the metric used to sort report results.</summary>
        public SortingType SortBy { get; set; }

        /// <summary>Gets or sets the referral affiliate identifier used to narrow the report.</summary>
        /// <remarks>The capitalized <c>ID</c> spelling is retained for compatibility.</remarks>
        public string ReferralAffiliateID { get; set; }
    }
}
