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
    /// <summary>Defines date and text filters for affiliate payment reports.</summary>
    /// <remarks>The payment repository applies both date bounds inclusively and orders results by payment date.</remarks>
    public class AffiliatePaymentReportCriteria
    {
        /// <summary>Specifies which payment field is compared with <see cref="SearchText"/>.</summary>
        public enum SearchType
        {
            /// <summary>Searches by the numeric payment identifier.</summary>
            PaymentId = 0,
            /// <summary>Searches within the affiliate's referral identifier.</summary>
            AffiliateId = 1
        }

        /// <summary>Initializes the report to the broad historical date range used by the existing API.</summary>
        public AffiliatePaymentReportCriteria()
        {
            StartDateUtc = new DateTime(1900, 1, 1);
            EndDateUtc = new DateTime(3000, 1, 1);
        }

        /// <summary>Gets or sets the inclusive UTC start date for payments.</summary>
        public DateTime StartDateUtc { get; set; }

        /// <summary>Gets or sets the inclusive UTC end date for payments.</summary>
        public DateTime EndDateUtc { get; set; }

        /// <summary>Gets or sets the field used to interpret <see cref="SearchText"/>.</summary>
        public SearchType SearchBy { get; set; }

        /// <summary>Gets or sets the optional text used by the selected <see cref="SearchBy"/> criterion.</summary>
        /// <remarks>Non-numeric payment searches are treated as an unmatched payment identifier.</remarks>
        public string SearchText { get; set; }
    }
}
