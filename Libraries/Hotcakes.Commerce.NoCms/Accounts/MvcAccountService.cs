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
using System.Collections.Generic;
using Hotcakes.Commerce.Accounts;

namespace Hotcakes.Commerce.NoCms.Accounts
{
    /// <summary>
    /// Provides account-service operations for associating NoCms administrative users with stores.
    /// </summary>
    public class MvcAccountService : AccountService
    {
        /// <summary>
        /// Initializes an account service for the specified commerce request context.
        /// </summary>
        /// <param name="context">The request context used to resolve commerce services and store data.</param>
        public MvcAccountService(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Gets or sets the repository used to retrieve administrative user accounts.
        /// </summary>
        public IUserAccountRepository AdminUsers { get; protected set; }

        /// <summary>
        /// Gets or sets the repository used to retrieve relationships between administrative users and stores.
        /// </summary>
        public StoreUserRelationshipRepository AdminUsersXStores { get; protected set; }

        // Admin Users
        /// <summary>
        /// Finds the administrative users associated with a store.
        /// </summary>
        /// <param name="storeId">The identifier of the store whose administrative users should be returned.</param>
        /// <returns>A list containing the user accounts associated with the store.</returns>
        /// <seealso cref="IUserAccountRepository.FindById(long)"/>
        public virtual List<UserAccount> FindAdminUsersByStoreId(long storeId)
        {
            var result = new List<UserAccount>();

            var relationships = AdminUsersXStores.FindByStoreId(storeId);

            foreach (var rel in relationships)
            {
                result.Add(AdminUsers.FindById(rel.UserId));
            }

            return result;
        }

        // Stores X Users
        /// <summary>
        /// Determines whether a user has any access relationship with a store.
        /// </summary>
        /// <param name="storeId">The identifier of the store to check.</param>
        /// <param name="userId">The identifier of the user to check.</param>
        /// <returns><see langword="true"/> when the user has a relationship with the store; otherwise, <see langword="false"/>.</returns>
        public virtual bool DoesUserHaveAccessToStore(long storeId, long userId)
        {
            var relationships = AdminUsersXStores.FindByStoreId(storeId);
            foreach (var r in relationships)
            {
                if (r.UserId == userId)
                {
                    return true;
                }
            }
            return false;
        }

        /// <summary>
        /// Determines whether a user is the owner of a store.
        /// </summary>
        /// <param name="storeId">The identifier of the store to check.</param>
        /// <param name="userId">The identifier of the user to check.</param>
        /// <returns><see langword="true"/> when the user has an owner relationship with the store; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="StoreAccessMode.Owner"/>
        public virtual bool IsUserStoreOwner(long storeId, long userId)
        {
            var relationships = AdminUsersXStores.FindByStoreId(storeId);
            foreach (var r in relationships)
            {
                if (r.UserId == userId)
                {
                    if (r.AccessMode == StoreAccessMode.Owner)
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}
