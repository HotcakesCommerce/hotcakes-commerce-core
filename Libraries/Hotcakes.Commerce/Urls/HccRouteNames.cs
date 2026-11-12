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

namespace Hotcakes.Commerce.Urls
{
    /// <summary>
    /// Contains the route-name values used by Hotcakes Commerce storefront routes.
    /// </summary>
    /// <remarks>
    /// These constants identify registered route names and are consumed by routing and URL
    /// generation code. Use <see cref="HccRoute"/> with <see cref="HccUrlBuilder"/> when the
    /// destination is known as a commerce route rather than as a raw route-name string.
    /// </remarks>
    public class HccRouteNames
    {
        /// <summary>Gets the registered shopping-cart route name.</summary>
        public const string Cart = "cart-route";

        /// <summary>Gets the registered custom route name.</summary>
        public const string Custom = "bvroute";

        /// <summary>Gets the registered product-review route name.</summary>
        public const string ProductReview = "productreview-route";

        /// <summary>Gets the registered product-reviews route name.</summary>
        public const string ProductReviews = "productreviews";

        /// <summary>Gets the registered checkout route name.</summary>
        public const string Checkout = "checkout-route";

        /// <summary>Gets the registered bulk-add-to-cart route name.</summary>
        public const string CartBulkAdd = "cart-bulkadd";

        /// <summary>Gets the registered remove-cart-item route name.</summary>
        public const string CartRemoveItem = "cart-removeitem";

        /// <summary>Gets the registered update-cart-item route name.</summary>
        public const string CartUpdateItem = "cart-updateitem";

        /// <summary>Gets the registered add-coupon-to-cart route name.</summary>
        public const string CartAddCoupon = "cart-addcoupon";

        /// <summary>Gets the registered remove-coupon-from-cart route name.</summary>
        public const string CartRemoveCoupon = "cart-removecoupont";

        /// <summary>Gets the registered PayPal checkout route name.</summary>
        public const string CheckoutPayPal = "checkout-paypal-route";

        /// <summary>Gets the registered wish-list or saved-items route name.</summary>
        public const string WishList = "saveditems-route";

        /// <summary>Gets the registered storefront search route name.</summary>
        public const string Search = "search-route";

        /// <summary>Gets the registered default-account route name.</summary>
        public const string AccountDefault = "account_default";

        /// <summary>Gets the registered customer-login route name.</summary>
        public const string Login = "customerlogin";

        /// <summary>Gets the registered customer-logoff route name.</summary>
        public const string Logoff = "customerlogout";

        /// <summary>Gets the registered forgotten-password route name.</summary>
        public const string SendPassword = "forgotpassword-route";

        /// <summary>Gets the registered terms or policy route name.</summary>
        public const string Terms = "policy-route";
    }
}
