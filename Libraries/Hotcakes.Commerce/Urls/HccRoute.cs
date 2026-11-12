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
    /// Identifies a supported Hotcakes Commerce storefront destination.
    /// </summary>
    /// <remarks>
    /// Pass a value from this enumeration to <see cref="HccUrlBuilder.RouteHccUrl(HccRoute)"/>
    /// or to an implementation of <see cref="IHccUrlResolver"/> to generate a URL using the
    /// active storefront route configuration. The numeric values are part of the existing public
    /// contract and must remain stable for persisted or serialized route values.
    /// </remarks>
    /// <seealso cref="HccRouteNames"/>
    public enum HccRoute
    {
        /// <summary>Represents an undefined or unavailable storefront destination.</summary>
        Undefined = 0,

        /// <summary>Represents the storefront category destination.</summary>
        Category = 1,

        /// <summary>Represents the storefront product destination.</summary>
        Product = 2,

        /// <summary>Represents the product-review destination.</summary>
        ProductReview = 3,

        /// <summary>Represents the shopping-cart destination.</summary>
        Cart = 4,

        /// <summary>Represents the checkout destination.</summary>
        Checkout = 5,

        /// <summary>Represents the PayPal checkout destination.</summary>
        CheckoutPayPal = 6,

        /// <summary>Represents the customer wish-list or saved-items destination.</summary>
        WishList = 7,

        /// <summary>Represents the storefront search destination.</summary>
        Search = 8,

        /// <summary>Represents the customer order-history destination.</summary>
        OrderHistory = 9,

        /// <summary>Represents the customer address-book destination.</summary>
        AddressBook = 10,

        /// <summary>Represents the customer login destination.</summary>
        Login = 12,

        /// <summary>Represents the customer logoff destination.</summary>
        Logoff = 13,

        /// <summary>Represents the forgotten-password or password-reset destination.</summary>
        SendPassword = 14,

        /// <summary>Represents the storefront terms or policy destination.</summary>
        Terms = 15,

        /// <summary>Represents the customer profile destination.</summary>
        UserProfile = 16,

        /// <summary>Represents the customer profile-edit destination.</summary>
        EditUserProfile = 17,

        /// <summary>Represents the affiliate-registration destination.</summary>
        AffiliateRegistration = 18,

        /// <summary>Represents the affiliate-dashboard destination.</summary>
        AffiliateDashboard = 19,

        /// <summary>Represents the storefront home destination.</summary>
        Home = 20,

        /// <summary>Represents a third-party payment return destination.</summary>
        ThirdPartyPayment = 21
    }
}
