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
    /// Defines persistence operations for <see cref="UserAccount"/> records.
    /// </summary>
    public interface IUserAccountRepository
    {
        /// <summary>
        /// Creates a user account record.
        /// </summary>
        /// <param name="item">The account to persist.</param>
        /// <returns><see langword="true"/> when the record is created; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="UserAccount"/>
        bool Create(UserAccount item);

        /// <summary>
        /// Updates an existing user account record.
        /// </summary>
        /// <param name="c">The account containing the values to persist.</param>
        /// <returns><see langword="true"/> when the record is updated; otherwise, <see langword="false"/>.</returns>
        bool Update(UserAccount c);

        /// <summary>
        /// Deletes a user account by identifier.
        /// </summary>
        /// <param name="id">The identifier of the account to delete.</param>
        /// <returns><see langword="true"/> when the record is deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(long id);

        /// <summary>
        /// Finds a user account by identifier.
        /// </summary>
        /// <param name="id">The identifier of the account to find.</param>
        /// <returns>The matching account, or <see langword="null"/> when no account is found.</returns>
        UserAccount FindById(long id);

        /// <summary>
        /// Finds a user account by email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The matching account, or <see langword="null"/> when no account is found.</returns>
        UserAccount FindByEmail(string email);
    }
}
