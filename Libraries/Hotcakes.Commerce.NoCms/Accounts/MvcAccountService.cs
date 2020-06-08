#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
    public class MvcAccountService : AccountService
    {
        public MvcAccountService(HccRequestContext context)
            : base(context)
        {
        }

        public IUserAccountRepository AdminUsers { get; protected set; }
        public StoreUserRelationshipRepository AdminUsersXStores { get; protected set; }

        // Admin Users
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