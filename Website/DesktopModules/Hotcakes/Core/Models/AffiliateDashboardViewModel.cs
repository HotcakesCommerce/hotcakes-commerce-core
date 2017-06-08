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
using Newtonsoft.Json;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The AffiliateDashboardViewModel is pretty much what it sounds like. When you're using the
    ///     affiliate program features, this is the model used when displaying the dashboard view to
    ///     individual affiliates. This article will help you understand what's available to you in
    ///     the view model.
    /// </summary>
    [Serializable]
    public class AffiliateDashboardViewModel
    {
        /// <summary>
        ///     Affiliate Information which needs to be displayed on dashboard
        /// </summary>
        public AffiliateViewModel Affiliate { get; set; }

        /// <summary>
        ///     Order information for the Affiliate which needs to be displayed on dashboard
        /// </summary>
        public AffiliateOrdersViewModel Orders { get; set; }

        /// <summary>
        ///     Payment information for the Affiliate which needs to be displayed on dashboard
        /// </summary>
        public AffiliatePaymentsViewModel Payments { get; set; }

        /// <summary>
        ///     Referrals information for the Affiliate which needs to be displayed on dashboard
        /// </summary>
        public AffiliateReferralsViewModel Referrals { get; set; }

        public AffiliateUrlBuilderViewModel UrlBuilder { get; set; }

        [Obsolete("Obsolete in 1.7.1. Used rendering of list on server in the view.")]
        public Dictionary<int, string> TimeRangesLocalized { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}