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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Metrics
{
    public class SearchQueryRepository : HccSimpleRepoBase<hcc_SearchQuery, SearchQuery>
    {
        public SearchQueryRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_SearchQuery data, SearchQuery model)
        {
            model.Bvin = data.bvin;
            model.StoreId = data.StoreId;
            model.LastUpdated = data.LastUpdated;
            model.QueryPhrase = data.QueryPhrase;
            model.ShopperID = data.ShopperId;
        }

        protected override void CopyModelToData(hcc_SearchQuery data, SearchQuery model)
        {
            data.bvin = model.Bvin;
            data.StoreId = model.StoreId;
            data.LastUpdated = model.LastUpdated;
            data.QueryPhrase = model.QueryPhrase;
            data.ShopperId = model.ShopperID;
        }

        public SearchQuery Find(string bvin)
        {
            var result = FindForAllStores(bvin);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        public SearchQuery FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public override bool Create(SearchQuery item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;
            return base.Create(item);
        }

        public bool Update(SearchQuery c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            var item = FindForAllStores(bvin);
            if (item == null) return false;
            if (item.StoreId != storeId) return false;
            return Delete(y => y.bvin == bvin);
        }

        public bool DeleteAll()
        {
            return Delete(q => q.StoreId == Context.CurrentStore.Id);
        }

        public List<SearchQuery> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        public List<SearchQuery> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.QueryPhrase);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize);
                return ListPoco(items);
            }
        }


        public List<SearchQuery> FindByShopperId(string shopperId, int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .Where(y => y.ShopperId == shopperId)
                    .OrderByDescending(y => y.LastUpdated);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize);
                return ListPoco(items);
            }
        }

        public List<SearchQueryData> FindQueryCountReport()
        {
            var result = new List<SearchQueryData>();
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                result = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .GroupBy(y => y.QueryPhrase)
                    .Select(grouping => grouping.Select(y => new SearchQueryData
                    {
                        QueryPhrase = y.QueryPhrase,
                        Count = grouping.Count()
                    })
                        .FirstOrDefault())
                    .OrderByDescending(y => y.Count).ToList();
            }

            if (result == null) result = new List<SearchQueryData>();
            return result;
        }

        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        public class SearchQueryData
        {
            public SearchQueryData()
            {
                QueryPhrase = string.Empty;
                Count = 0;
                Percentage = 0;
            }

            public string QueryPhrase { get; set; }
            public int Count { get; set; }
            public decimal Percentage { get; set; }
        }
    }
}