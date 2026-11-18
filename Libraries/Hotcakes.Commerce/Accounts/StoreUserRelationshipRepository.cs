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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Represents StoreUserRelationshipRepository data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class StoreUserRelationshipRepository : HccSimpleRepoBase<hcc_StoresXUsers, StoreUserRelationship>
    {
        /// <summary>Performs the StoreUserRelationshipRepository operation.</summary>
        /// <param name="context">The context value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
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

        /// <summary>Performs the Update operation.</summary>
        /// <param name="relationship">The relationship value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public bool Update(StoreUserRelationship relationship)
        {
            return Update(relationship, r => r.Id == relationship.Id);
        }

        /// <summary>Performs the Delete operation.</summary>
        /// <param name="id">The id value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public bool Delete(long id)
        {
            return Delete(r => r.Id == id);
        }

        /// <summary>Performs the Delete operation.</summary>
        /// <param name="storeId">The storeId value used by the operation.</param>
        /// <param name="userId">The userId value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public bool Delete(long storeId, long userId)
        {
            return Delete(r => r.StoreId == storeId && r.UserId == userId);
        }

        /// <summary>Performs the FindByUserId operation.</summary>
        /// <param name="id">The id value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public List<StoreUserRelationship> FindByUserId(long id)
        {
            return FindListPoco(q => q.Where(r => r.UserId == id));
        }

        /// <summary>Performs the FindByStoreId operation.</summary>
        /// <param name="id">The id value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public List<StoreUserRelationship> FindByStoreId(long id)
        {
            return FindListPoco(q => q.Where(r => r.StoreId == id));
        }

        /// <summary>Performs the FindByUserIdAndStoreId operation.</summary>
        /// <param name="userId">The userId value used by the operation.</param>
        /// <param name="storeId">The storeId value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public StoreUserRelationship FindByUserIdAndStoreId(long userId, long storeId)
        {
            return FindFirstPoco(r => r.StoreId == storeId && r.UserId == userId);
        }
    }
}
