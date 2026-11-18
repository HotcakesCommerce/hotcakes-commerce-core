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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Provides persistence operations for <see cref="UserAccount"/> records.
    /// </summary>
    [Serializable]
    public class UserAccountRepository : HccSimpleRepoBase<hcc_UserAccounts, UserAccount>, IUserAccountRepository
    {
        /// <summary>
        /// Initializes a repository using the specified commerce request context.
        /// </summary>
        /// <param name="c">The request context used by the repository.</param>
        public UserAccountRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        /// Creates a user account after assigning its creation time and ensuring its password is hashed.
        /// </summary>
        /// <param name="item">The account to persist.</param>
        /// <returns><see langword="true"/> when the account is created; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="UserAccount.HashPasswordIfNeeded"/>
        public override bool Create(UserAccount item)
        {
            item.DateCreated = DateTime.UtcNow;
            item.HashPasswordIfNeeded();
            return base.Create(item);
        }

        /// <summary>
        /// Updates a user account after ensuring its password is hashed.
        /// </summary>
        /// <param name="c">The account containing the values to persist.</param>
        /// <returns><see langword="true"/> when the account is updated; otherwise, <see langword="false"/>.</returns>
        public bool Update(UserAccount c)
        {
            c.HashPasswordIfNeeded();
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>
        /// Deletes a user account by identifier.
        /// </summary>
        /// <param name="id">The identifier of the account to delete.</param>
        /// <returns><see langword="true"/> when the account is deleted; otherwise, <see langword="false"/>.</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>
        /// Finds a user account by identifier.
        /// </summary>
        /// <param name="id">The identifier of the account to find.</param>
        /// <returns>The matching account, or <see langword="null"/> when no account is found.</returns>
        public UserAccount FindById(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>
        /// Finds a user account by email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The matching account, or <see langword="null"/> when no account is found.</returns>
        public UserAccount FindByEmail(string email)
        {
            return FindFirstPoco(y => y.email == email);
        }

        protected override void CopyDataToModel(hcc_UserAccounts data, UserAccount model)
        {
            model.Id = data.Id;
            model.DateCreated = data.DateCreated;
            model.Email = data.email;
            model.HashedPassword = data.password;
            model.Salt = data.Salt;
            model.Status = (UserAccountStatus) data.statuscode;
            model.ResetKey = data.ResetKey;
        }

        protected override void CopyModelToData(hcc_UserAccounts data, UserAccount model)
        {
            data.Id = model.Id;
            data.DateCreated = model.DateCreated;
            data.email = model.Email;
            data.password = model.HashedPassword;
            data.Salt = model.Salt;
            data.statuscode = (int) model.Status;
            data.ResetKey = model.ResetKey;
        }
    }
}
