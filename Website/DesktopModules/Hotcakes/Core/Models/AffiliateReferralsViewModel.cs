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
using System.Collections.Generic;
using Newtonsoft.Json;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The AffiliateReferralsViewModel makes it possible to display the history of referrals
    ///     for a specific affiliate over a period of time.
    /// </summary>
    [Serializable]
    public class AffiliateReferralsViewModel
    {
        /// <summary>
        ///     List of referrals for the specific affiliate which are shown on affiliate dashboard
        /// </summary>
        public List<ReferalViewModel> Referrals { get; set; }

        /// <summary>
        ///     Number of referrals for the affiliate
        /// </summary>
        public int TotalCount { get; set; }

        /// <summary>
        ///     Total revenue of all referrals for the affiliate
        /// </summary>
        public string TotalAmount { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }

        /// <summary>
        ///     Individual Referral information which needs to be dispayed for Affiliate
        /// </summary>
        public class ReferalViewModel
        {
            public string Name { get; set; }
            public string Id { get; set; }
            public string Email { get; set; }
            public string Revenue { get; set; }
        }
    }
}