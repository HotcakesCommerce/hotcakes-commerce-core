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

namespace Hotcakes.Commerce.Marketing
{
    /// <summary>Identifies the predefined promotion template returned by <see cref="MarketingService.GetPredefinedPromotion"/>.</summary>
    public enum PreDefinedPromotion
    {
        /// <summary>Blank sale template.</summary>
        CustomSale = 0,
        /// <summary>Blank line-item offer template.</summary>
        CustomOfferForLineItems = 1,
        /// <summary>Blank order offer template with an order adjustment action.</summary>
        CustomOfferForOrder = 2,
        /// <summary>Blank shipping offer template with a shipping adjustment action.</summary>
        CustomOfferForShipping = 3,
        /// <summary>Blank affiliate promotion template with a reward-points action.</summary>
        CustomAffiliatePromotion = 4,
        /// <summary>Blank free-item offer template.</summary>
        CustomOfferForFreeItems = 5,

        /// <summary>Storewide sale template.</summary>
        SaleStorewide = 10,
        /// <summary>Product-specific sale template.</summary>
        SaleProducts = 11,
        /// <summary>Category sale template.</summary>
        SaleCategories = 12,
        /// <summary>Product-type sale template.</summary>
        SaleProductType = 13,
        /// <summary>Price-group sale template.</summary>
        SaleUserGroup = 14,
        /// <summary>User-specific sale template.</summary>
        SaleUser = 15,
        /// <summary>Coupon-qualified order discount template.</summary>
        OrderDiscountCoupon = 16,
        /// <summary>User-qualified order discount template.</summary>
        OrderDiscountUser = 17,
        /// <summary>Price-group-qualified order discount template.</summary>
        OrderDiscountUserGroup = 18,
        /// <summary>Free-shipping template.</summary>
        OrderFreeShipping = 19,
        /// <summary>Shipping-discount template.</summary>
        OrderShippingDiscount = 20,
        /// <summary>Category-based free-shipping template.</summary>
        OrderFreeShippingByCategory = 21
    }
}
