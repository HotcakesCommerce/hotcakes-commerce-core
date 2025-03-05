#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
    [Serializable]
    public class UserAccountRepository : HccSimpleRepoBase<hcc_UserAccounts, UserAccount>, IUserAccountRepository
    {
        public UserAccountRepository(HccRequestContext c)
            : base(c)
        {
        }

        public override bool Create(UserAccount item)
        {
            item.DateCreated = DateTime.UtcNow;
            item.HashPasswordIfNeeded();
            return base.Create(item);
        }

        public bool Update(UserAccount c)
        {
            c.HashPasswordIfNeeded();
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public UserAccount FindById(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

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