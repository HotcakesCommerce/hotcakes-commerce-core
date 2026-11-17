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
using System.Reflection;

namespace Hotcakes.Commerce
{
    public partial class WebAppSettings
    {
        /// <summary>
        ///     Gets the name of the cookie that stores the customer's Hotcakes session identifier.
        /// </summary>
        public const string CookieNameSessionGuid = "HotcakesSessionGuid";

        /// <summary>
        ///     Gets the name of the cookie that stores the customer's shopping session identifier.
        /// </summary>
        public const string CookieNameShoppingSessionGuid = "HotcakesShoppingSessionGuid";

        /// <summary>
        ///     Gets the name of the cookie that stores the identifiers of the customer's recently viewed products.
        /// </summary>
        public const string CookieNameLastProductsViewed = "HotcakesLastProductsViewed";

        /// <summary>
        ///     Gets the name of the cookie that stores the affiliate referral identifier.
        /// </summary>
        public const string CookieNameAffiliateId = "HotcakesCustomerIDReferrer";

        /// <summary>
        ///     Gets the application version from the <c>Hotcakes.Modules</c> assembly.
        /// </summary>
        /// <value>The assembly version of <c>Hotcakes.Modules.Core.HotcakesController</c>.</value>
        public static Version AppVersion
        {
            get
            {
                var type = Type.GetType("Hotcakes.Modules.Core.HotcakesController, Hotcakes.Modules");
                return Assembly.GetAssembly(type).GetName().Version;
            }
        }

        /// <summary>
        ///     Gets the product SKU for this application build.
        /// </summary>
        /// <value>An empty string; this edition does not expose a SKU.</value>
        public static string SKU
        {
            get { return string.Empty; }
        }

        /// <summary>
        ///     Gets the application version formatted with major, minor, and build components.
        /// </summary>
        /// <value>The first three components of <see cref="AppVersion"/>.</value>
        public static string FriendlyAppVersion
        {
            get { return AppVersion.ToString(3); }
        }

        /// <summary>
        ///     Gets the product SKU formatted for display.
        /// </summary>
        /// <value>An empty string; this edition does not expose a SKU.</value>
        public static string FriendlySKU
        {
            get { return string.Empty; }
        }

        /// <summary>
        ///     Creates the cookie name used for a store's cart when payment is pending.
        /// </summary>
        /// <param name="storeId">The store identifier to include in the cookie name.</param>
        /// <returns>The cookie name in the form <c>hotcakes-cartid-pendingpayment-{storeId}</c>.</returns>
        public static string CookieNameCartIdPaymentPending(long storeId)
        {
            return "hotcakes-cartid-pendingpayment-" + storeId;
        }

        /// <summary>
        ///     Creates the cookie name used for a store's cart.
        /// </summary>
        /// <param name="storeId">The store identifier to include in the cookie name.</param>
        /// <returns>The cookie name in the form <c>hotcakes-cartid-{storeId}</c>.</returns>
        public static string CookieNameCartId(long storeId)
        {
            return "hotcakes-cartid-" + storeId;
        }

        /// <summary>
        ///     Creates the cookie name used for a store's last selected category.
        /// </summary>
        /// <param name="storeId">The store identifier to include in the cookie name.</param>
        /// <returns>The cookie name in the form <c>hotcakes-lastcategory-{storeId}</c>.</returns>
        public static string CookieNameLastCategory(long storeId)
        {
            return "hotcakes-lastcategory-" + storeId;
        }
    }
}
