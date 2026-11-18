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
    /// <summary>
    /// Defines the optional filters used when listing referrals for the current store.
    /// </summary>
    /// <remarks>Both date bounds must be supplied for the repository to apply the date range.</remarks>
    public class AffiliateReferralSearchCriteria
    {
        /// <summary>Initializes criteria with no affiliate or date restriction.</summary>
        public AffiliateReferralSearchCriteria()
        {
            AffiliateId = 0;
            StartDateUtc = null;
            EndDateUtc = null;
        }

        /// <summary>Gets or sets the numeric affiliate identifier to match; zero leaves this filter unused.</summary>
        public long AffiliateId { get; set; }

        /// <summary>Gets or sets the inclusive UTC start of the referral date range.</summary>
        /// <remarks>This value is considered only when <see cref="EndDateUtc"/> is also set.</remarks>
        public DateTime? StartDateUtc { get; set; }

        /// <summary>Gets or sets the inclusive UTC end of the referral date range.</summary>
        /// <remarks>This value is considered only when <see cref="StartDateUtc"/> is also set.</remarks>
        public DateTime? EndDateUtc { get; set; }
    }
}
