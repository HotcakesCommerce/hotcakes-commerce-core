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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing
{
    /// <summary>
    /// Carries contextual information used while evaluating promotions.
    /// </summary>
    public class PromotionContext
    {
        /// <summary>Initializes a context carrying the request, promotion mode, and promotion identifier.</summary>
        /// <param name="requestContext">The non-null commerce request context.</param>
        /// <param name="mode">The promotion mode being evaluated.</param>
        /// <param name="promotionId">The promotion identifier used when recording discounts.</param>
        /// <exception cref="ArgumentNullException">The request context is null.</exception>
        public PromotionContext(HccRequestContext requestContext, PromotionType mode, long promotionId)
        {
            if (requestContext == null) throw new ArgumentNullException(nameof(requestContext));

            CustomerDescription = string.Empty;
            CurrentShippingMethodId = string.Empty;
            AdjustedShippingRate = 0m;
            RequestContext = requestContext;
            Mode = mode;
            PromotionId = promotionId;
        }

        /// <summary>Gets or sets the promotion identifier used by actions and discount details.</summary>
        public long PromotionId { get; set; }
        /// <summary>Gets or sets the promotion mode being evaluated.</summary>
        public PromotionType Mode { get; set; }
        /// <summary>Gets or sets the commerce request context.</summary>
        public HccRequestContext RequestContext { get; set; }
        /// <summary>Gets or sets the order being evaluated, when applicable.</summary>
        public Order Order { get; set; }
        /// <summary>Gets or sets the product being evaluated, when applicable.</summary>
        public Product Product { get; set; }
        /// <summary>Gets or sets the user-specific price being evaluated.</summary>
        public UserSpecificPrice UserPrice { get; set; }
        /// <summary>Gets or sets the customer-facing promotion description supplied to actions.</summary>
        public string CustomerDescription { get; set; }
        /// <summary>Gets or sets the customer account associated with the evaluation.</summary>
        public CustomerAccount CurrentCustomer { get; set; }
        /// <summary>Gets or sets the line currently being processed by a line-item evaluation.</summary>
        public LineItem CurrentlyProcessingLineItem { get; set; }

        /// <summary>Gets or sets whether another offer has already been applied; consumers should preserve the current behavior.</summary>
        public bool OtherOffersApplied { get; set; }

        /// <summary>Gets or sets the current shipping method identifier.</summary>
        public string CurrentShippingMethodId { get; set; }
        /// <summary>Gets or sets the mutable shipping rate after actions adjust it.</summary>
        public decimal AdjustedShippingRate { get; set; }
        /// <summary>Creates an application facade for the current <see cref="RequestContext"/>.</summary>
        public HotcakesApplication HccApp
        {
            get { return new HotcakesApplication(RequestContext); }
        }
    }
}