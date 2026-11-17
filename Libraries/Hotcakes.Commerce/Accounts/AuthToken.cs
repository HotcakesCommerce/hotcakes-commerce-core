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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Represents AuthToken data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class AuthToken
    {
        /// <summary>Performs the AuthToken operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public AuthToken()
        {
            Id = -1;
            TokenId = Guid.NewGuid();
            UserId = 0;
            Expires = DateTime.UtcNow;
        }

        /// <summary>Gets or sets Id.</summary>
        /// <remarks>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</remarks>
        /// <value>The database identifier for this record; a newly constructed model uses its established unsaved sentinel value unless otherwise noted.</value>
        public long Id { get; set; }
        /// <summary>Gets or sets TokenId.</summary>
        /// <remarks>The opaque token identifier used to locate the authentication token. Treat it as sensitive.</remarks>
        /// <value>The opaque token identifier used to locate the authentication token. Treat it as sensitive.</value>
        public Guid TokenId { get; set; }
        /// <summary>Gets or sets UserId.</summary>
        /// <remarks>The DNN user identifier associated with the token or store relationship.</remarks>
        /// <value>The DNN user identifier associated with the token or store relationship.</value>
        public long UserId { get; set; }
        /// <summary>Gets or sets Expires.</summary>
        /// <remarks>The UTC expiration date and time of the token.</remarks>
        /// <value>The UTC expiration date and time of the token.</value>
        public DateTime Expires { get; set; }
    }
}
