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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>Represents StoreRepository data and behavior within the Hotcakes Commerce account and store boundary.</summary>
    [Serializable]
    public class StoreRepository : HccSimpleRepoBase<hcc_Stores, Store>
    {
        private readonly StoreSettingsRepository settingsRepository;

        /// <summary>Performs the StoreRepository operation.</summary>
        /// <param name="context">The context value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public StoreRepository(HccRequestContext context)
            : base(context)
        {
            settingsRepository = new StoreSettingsRepository(context);
        }

        protected override void CopyDataToModel(hcc_Stores data, Store model)
        {
            model.Id = data.Id;
            model.StoreGuid = data.StoreGuid;
            model.CustomUrl = data.CustomUrl;
            model.DateCreated = data.DateCreated;
            model.StoreName = data.StoreName;
        }

        protected override void CopyModelToData(hcc_Stores data, Store model)
        {
            data.Id = model.Id;
            data.StoreGuid = model.StoreGuid;
            data.CustomUrl = model.CustomUrl;
            data.DateCreated = model.DateCreated;
            data.StoreName = model.StoreName;
        }

        protected override void MergeSubItems(Store model)
        {
            settingsRepository.MergeList(model.Id, model.Settings.AllSettings);
        }

        /// <summary>Performs the FindById operation.</summary>
        /// <param name="id">The id value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public Store FindById(long id)
        {
            return FindByIdWithCache(id);
        }

        /// <summary>Performs the FindByIdWithCache operation.</summary>
        /// <param name="id">The id value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public Store FindByIdWithCache(long id)
        {
            return CacheManager.GetStore(id, Context.MainContentCulture,
                () => { return FindFirstPoco(y => y.Id == id); });
        }

        /// <summary>Performs the FindByStoreGuid operation.</summary>
        /// <param name="guid">The guid value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public Store FindByStoreGuid(Guid guid)
        {
            var storeId = CacheManager.GetStoreIdByGuid(guid, () =>
            {
                return null;
            });

            if (storeId != null)
                return FindByIdWithCache(storeId.GetValueOrDefault());

            var store = FindFirstPoco(y => y.StoreGuid == guid);
            if(store != null)
                CacheManager.AddStore(store, Context.MainContentCulture);
            
            return store;
        }

        protected override void GetSubItems(List<Store> models)
        {
            var storeIds = models.Select(s => s.Id).ToList();
            var allSettings = settingsRepository.FindManySetting(storeIds);

            foreach (var model in models)
            {
                var settings = allSettings.Where(s => s.StoreId == model.Id).ToList();
                model.Settings.Init(settings);
            }
        }

        /// <summary>Performs the FindStoreIdByCustomUrl operation.</summary>
        /// <param name="hostName">The hostName value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public long FindStoreIdByCustomUrl(string hostName)
        {
            return (long)CacheManager.GetStoreIdByHostName(hostName, () =>
            {
                long result = -1;
                var s = FindFirstPoco(y => y.CustomUrl == hostName);
                if (s != null)
                {
                    result = s.Id;
                }

                return result;
            });
        }

        /// <summary>Performs the Update operation.</summary>
        /// <param name="s">The s value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public bool Update(Store s)
        {
            CacheManager.ClearStoreById(s.Id, Context.MainContentCulture);
            return Update(s, y => y.Id == s.Id);
        }

        /// <summary>Performs the Delete operation.</summary>
        /// <param name="id">The id value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public bool Delete(long id)
        {
            CacheManager.ClearStoreById(id);
            return Delete(y => y.Id == id);
        }


        /// <summary>Performs the ListAllForSuper operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public List<Store> ListAllForSuper()
        {
            return FindListPoco(q => { return q.OrderByDescending(y => y.DateCreated); });
        }

        /// <summary>Performs the FindStoresCreatedAfterDateForSuper operation.</summary>
        /// <param name="cutOffDateUtc">The cutOffDateUtc value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public List<Store> FindStoresCreatedAfterDateForSuper(DateTime cutOffDateUtc)
        {
            return
                FindListPoco(
                    q => { return q.Where(y => y.DateCreated >= cutOffDateUtc).OrderByDescending(y => y.DateCreated); });
        }

        /// <summary>Performs the FindDomainSnapshotsByIds operation.</summary>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public List<StoreDomainSnapshot> FindDomainSnapshotsByIds(List<long> ids)
        {
            var result = new List<StoreDomainSnapshot>();
            using (var s = CreateReadStrategy())
            {
                result = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => ids.Contains(y.Id))
                    .OrderBy(y => y.Id)
                    .Select(q => new StoreDomainSnapshot
                    {
                        CustomUrl = q.CustomUrl,
                        Id = q.Id,
                        StoreName = q.StoreName
                    }).ToList();
            }
            ;

            return result;
        }

        /// <summary>Performs the GetLastOrderNumber operation.</summary>
        /// <param name="storeId">The storeId value used by the operation.</param>
        /// <returns>The operation result, or the documented empty/default result when no matching data is available.</returns>
        public int GetLastOrderNumber(long storeId)
        {
            using (var s = CreateReadStrategy())
            {
                var store = s.GetQuery(y => y.Id == storeId).AsNoTracking().FirstOrDefault();
                if (store != null)
                    return store.LastOrderNumber;
                return 0;
            }
        }

        /// <summary>Performs the SetLastOrderNumber operation.</summary>
        /// <param name="storeId">The storeId value used by the operation.</param>
        /// <param name="lastOrderNumber">The lastOrderNumber value used by the operation.</param>
        public void SetLastOrderNumber(long storeId, int lastOrderNumber)
        {
            using (var s = CreateStrategy())
            {
                var store = s.GetQuery(y => y.Id == storeId).FirstOrDefault();
                if (store != null)
                {
                    store.LastOrderNumber = lastOrderNumber;
                    s.SubmitChanges();
                }
            }
        }
    }
}
