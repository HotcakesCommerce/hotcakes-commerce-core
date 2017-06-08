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
    ///     The AffiliateUrlBuilderViewModel is used to help render the view used to allow affiliates
    ///     generate referral URL's to refer new customers to the store.
    /// </summary>
    [Serializable]
    public class AffiliateUrlBuilderViewModel
    {
        /// <summary>
        ///     Unique identifier of affiliate
        /// </summary>
        public string AffiliateId { get; set; }

        /// <summary>
        ///     Registration URL which is set globally for the affiliate registration view in Hotcakes, received from the URL rule
        ///     table.
        /// </summary>
        public string RegistrationUrl { get; set; }

        /// <summary>
        ///     List of categories, used to generate the URL dynamically based on the categories.
        /// </summary>
        public List<ListItem> Categories { get; set; }

        public string ToJson()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}