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

namespace Hotcakes.Commerce.Membership
{
    /// <summary>Describes the normalized outcome of a membership-account operation.</summary>
    /// <remarks>The numeric values are compatibility-sensitive and are retained for serialized and provider-facing use.</remarks>
    public enum CreateUserStatus
    {
        /// <summary>No operation outcome has been assigned.</summary>
        None = 0,
        /// <summary>The account operation succeeded.</summary>
        Success = 1,
        /// <summary>The requested user could not be found.</summary>
        UserNotFound = 2,
        /// <summary>The supplied password did not satisfy the provider's requirements.</summary>
        InvalidPassword = 3,
        /// <summary>The username is already associated with another account.</summary>
        DuplicateUsername = 4,
        /// <summary>The provider rejected the account operation or user.</summary>
        UserRejected = 5,
        /// <summary>The provider could not persist or complete the update.</summary>
        UpdateFailed = 6,
        /// <summary>The email address is already associated with another account.</summary>
        DuplicateEmail = 7
    }
}
