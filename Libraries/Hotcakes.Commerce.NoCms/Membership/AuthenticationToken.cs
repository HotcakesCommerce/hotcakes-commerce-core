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

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Represents the authentication-token state associated with a customer account.
    /// </summary>
    /// <remarks>
    /// The token carries both UTC and local-time expiration accessors; use <see cref="ExpirationDateUTC"/>
    /// when a caller needs an unambiguous value and <see cref="IsExpired"/> to evaluate expiration.
    /// </remarks>
    [Serializable]
    public class AuthenticationToken
    {
        /// <summary>
        /// Initializes an authentication token with empty identifiers, a rejected state, and a default expiration.
        /// </summary>
        public AuthenticationToken()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            ExpirationDateUTC = DateTime.MinValue.AddDays(1);
            UserBvin = string.Empty;
            TokenRejected = true;
        }

        /// <summary>
        /// Gets or sets the unique identifier of this authentication token.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        /// Gets or sets the UTC date and time when this token was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the token expiration as local time.
        /// </summary>
        /// <value>The local representation of <see cref="ExpirationDateUTC"/>.</value>
        /// <remarks>Assigning a value converts it to UTC before storing it in <see cref="ExpirationDateUTC"/>.</remarks>
        public DateTime ExpirationDate
        {
            get { return ExpirationDateUTC.ToLocalTime(); }
            set { ExpirationDateUTC = value.ToUniversalTime(); }
        }

        /// <summary>
        /// Gets or sets the token expiration in UTC.
        /// </summary>
        /// <remarks>This is the canonical expiration value used by <see cref="ExpirationDate"/> and <see cref="IsExpired"/>.</remarks>
        public DateTime ExpirationDateUTC { get; set; }

        /// <summary>
        /// Gets or sets the customer-account identifier associated with this token.
        /// </summary>
        public string UserBvin { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the token has been rejected.
        /// </summary>
        public bool TokenRejected { get; set; }

        /// <summary>
        /// Gets a value indicating whether the token expiration has passed.
        /// </summary>
        /// <value><see langword="true"/> when the current local time is later than <see cref="ExpirationDate"/>; otherwise, <see langword="false"/>.</value>
        public bool IsExpired
        {
            get
            {
                if (DateTime.Compare(DateTime.Now, ExpirationDate) <= 0)
                {
                    return false;
                }
                return true;
            }
        }
    }
}
