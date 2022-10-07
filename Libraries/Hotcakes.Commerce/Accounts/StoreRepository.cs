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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Accounts
{
    [Serializable]
    public class StoreRepository : HccSimpleRepoBase<hcc_Stores, Store>
    {
        private readonly StoreSettingsRepository settingsRepository;

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

        public Store FindById(long id)
        {
            return FindByIdWithCache(id);
        }

        public Store FindByIdWithCache(long id)
        {
            return CacheManager.GetStore(id, Context.MainContentCulture,
                () => { return FindFirstPoco(y => y.Id == id); });
        }

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

        public bool Update(Store s)
        {
            CacheManager.ClearStoreById(s.Id, Context.MainContentCulture);
            return Update(s, y => y.Id == s.Id);
        }

        public bool Delete(long id)
        {
            CacheManager.ClearStoreById(id);
            return Delete(y => y.Id == id);
        }


        public List<Store> ListAllForSuper()
        {
            return FindListPoco(q => { return q.OrderByDescending(y => y.DateCreated); });
        }

        public List<Store> FindStoresCreatedAfterDateForSuper(DateTime cutOffDateUtc)
        {
            return
                FindListPoco(
                    q => { return q.Where(y => y.DateCreated >= cutOffDateUtc).OrderByDescending(y => y.DateCreated); });
        }

        public List<StoreDomainSnapshot> FindDomainSnapshotsByIds(List<long> ids)
        {
            var result = new List<StoreDomainSnapshot>();
            using (var s = CreateStrategy())
            {
                result = s.GetQuery()
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

        public int GetLastOrderNumber(long storeId)
        {
            using (var s = CreateStrategy())
            {
                var store = s.GetQuery(y => y.Id == storeId).FirstOrDefault();
                if (store != null)
                    return store.LastOrderNumber;
                return 0;
            }
        }

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