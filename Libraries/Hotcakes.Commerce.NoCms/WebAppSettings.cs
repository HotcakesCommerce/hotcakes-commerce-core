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

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Provides application settings used by the NoCms authentication and account workflows.
    /// </summary>
    public class WebAppSettings
    {
        /// <summary>
        /// Gets the number of unsuccessful sign-in attempts allowed before a user is locked out.
        /// </summary>
        /// <value>Five attempts.</value>
        public static int UserLockoutAttempts
        {
            get { return 5; }
        }

        /// <summary>
        /// Gets the number of minutes for which a user remains locked out.
        /// </summary>
        /// <value>Fifteen minutes.</value>
        public static int UserLockoutMinutes
        {
            get { return 15; }
        }

        /// <summary>
        /// Gets the minimum password length required by the configured membership provider.
        /// </summary>
        /// <value>The value of <see cref="System.Web.Security.Membership.MinRequiredPasswordLength"/>.</value>
        public static int PasswordMinimumLength
        {
            get { return System.Web.Security.Membership.MinRequiredPasswordLength; }
        }

        /// <summary>
        /// Gets the number of days for which an authentication token remains valid.
        /// </summary>
        /// <returns>Ninety days.</returns>
        public static int AuthenticationTokenValidForDays()
        {
            return 90;
        }

        /// <summary>
        /// Builds the authentication-token cookie name used for an administrator of a store.
        /// </summary>
        /// <param name="storeId">The identifier of the store associated with the cookie.</param>
        /// <returns>A cookie name containing the store identifier and the administrator token prefix.</returns>
        public static string CookieNameAuthenticationTokenAdmin(long storeId)
        {
            return "hotcakes-authtoken-" + storeId;
        }

        /// <summary>
        /// Builds the authentication-token cookie name used for a customer of a store.
        /// </summary>
        /// <param name="storeId">The identifier of the store associated with the cookie.</param>
        /// <returns>A cookie name containing the store identifier and the customer token prefix.</returns>
        public static string CookieNameAuthenticationTokenCustomer(long storeId)
        {
            return "hotcakes-authtokencustomer-" + storeId;
        }
    }
}
