#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreUserRelationshipRepository : HccSimpleRepoBase<hcc_StoresXUsers, StoreUserRelationship>
    {
        public StoreUserRelationshipRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override void CopyDataToModel(hcc_StoresXUsers data, StoreUserRelationship model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.UserId = data.UserId;
            model.AccessMode = (StoreAccessMode) data.AccessMode;
        }

        protected override void CopyModelToData(hcc_StoresXUsers data, StoreUserRelationship model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.UserId = model.UserId;
            data.AccessMode = (int) model.AccessMode;
        }

        public bool Update(StoreUserRelationship relationship)
        {
            return Update(relationship, r => r.Id == relationship.Id);
        }

        public bool Delete(long id)
        {
            return Delete(r => r.Id == id);
        }

        public bool Delete(long storeId, long userId)
        {
            return Delete(r => r.StoreId == storeId && r.UserId == userId);
        }

        public List<StoreUserRelationship> FindByUserId(long id)
        {
            return FindListPoco(q => q.Where(r => r.UserId == id));
        }

        public List<StoreUserRelationship> FindByStoreId(long id)
        {
            return FindListPoco(q => q.Where(r => r.StoreId == id));
        }

        public StoreUserRelationship FindByUserIdAndStoreId(long userId, long storeId)
        {
            return FindFirstPoco(r => r.StoreId == storeId && r.UserId == userId);
        }
    }
}