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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.Marketing
{
    public class PromotionContext
    {
        public PromotionContext(HccRequestContext requestContext, PromotionType mode, long promotionId)
        {
            CustomerDescription = string.Empty;
            CurrentShippingMethodId = string.Empty;
            AdjustedShippingRate = 0;
            RequestContext = requestContext;
            Mode = mode;
            PromotionId = promotionId;
        }

        #region Obsolete

        [Obsolete("Obsolete in 2.0.0. Use constructor with other parameters instead")]
        public PromotionContext(HotcakesApplication app, PromotionType mode, long promotionId)
            : this(app.CurrentRequestContext, mode, promotionId)
        {
        }

        #endregion

        public long PromotionId { get; set; }
        public PromotionType Mode { get; set; }
        public HccRequestContext RequestContext { get; set; }
        public Order Order { get; set; }
        public Product Product { get; set; }
        public UserSpecificPrice UserPrice { get; set; }
        public string CustomerDescription { get; set; }
        public CustomerAccount CurrentCustomer { get; set; }
        public LineItem CurrentlyProcessingLineItem { get; set; }
        // TODO: Review this property and ensure it gets implemented properly
        public bool OtherOffersApplied { get; set; }

        public string CurrentShippingMethodId { get; set; }
        public decimal AdjustedShippingRate { get; set; }

        public HotcakesApplication HccApp
        {
            get { return new HotcakesApplication(RequestContext); }
        }
    }
}