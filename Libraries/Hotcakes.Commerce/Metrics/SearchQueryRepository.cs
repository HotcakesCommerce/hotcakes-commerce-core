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

namespace Hotcakes.Commerce.Metrics
{
    /// <summary>
    /// Provides store-scoped persistence and reporting operations for <see cref="SearchQuery"/> records.
    /// </summary>
    /// <remarks>
    /// Reads and writes use <see cref="HccRequestContext.CurrentStore"/> as the store boundary unless
    /// an operation explicitly accepts a store identifier for internal cleanup. Query results are
    /// projected through the repository's model/data mapping methods.
    /// </remarks>
    public class SearchQueryRepository : HccSimpleRepoBase<hcc_SearchQuery, SearchQuery>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SearchQueryRepository"/> class.
        /// </summary>
        /// <param name="c">The commerce request context used for repository operations and store scoping.</param>
        public SearchQueryRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        /// Copies persistence data into the public search-query model.
        /// </summary>
        /// <param name="data">The persistence entity to read.</param>
        /// <param name="model">The model to populate.</param>
        protected override void CopyDataToModel(hcc_SearchQuery data, SearchQuery model)
        {
            model.Bvin = data.bvin;
            model.StoreId = data.StoreId;
            model.LastUpdated = data.LastUpdated;
            model.QueryPhrase = data.QueryPhrase;
            model.ShopperID = data.ShopperId;
        }

        /// <summary>
        /// Copies a public search-query model into persistence data.
        /// </summary>
        /// <param name="data">The persistence entity to populate.</param>
        /// <param name="model">The model to read.</param>
        protected override void CopyModelToData(hcc_SearchQuery data, SearchQuery model)
        {
            data.bvin = model.Bvin;
            data.StoreId = model.StoreId;
            data.LastUpdated = model.LastUpdated;
            data.QueryPhrase = model.QueryPhrase;
            data.ShopperId = model.ShopperID;
        }

        /// <summary>
        /// Finds a search query by identifier in the current store.
        /// </summary>
        /// <param name="bvin">The search-query identifier.</param>
        /// <returns>The matching query when it belongs to the current store; otherwise, <see langword="null"/>.</returns>
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

        /// <summary>
        /// Finds a search query by identifier without restricting the lookup to the current store.
        /// </summary>
        /// <param name="bvin">The search-query identifier.</param>
        /// <returns>The matching query, or <see langword="null"/> when no record has that identifier.</returns>
        public SearchQuery FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        /// <summary>
        /// Creates a search-query record in the current store.
        /// </summary>
        /// <param name="item">The query to create. An empty <see cref="SearchQuery.Bvin"/> receives a new identifier.</param>
        /// <returns><see langword="true"/> when the base repository creates the record; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The current store identifier and the current UTC time replace the corresponding model values before persistence.</remarks>
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

        /// <summary>
        /// Updates a search-query record belonging to the current store.
        /// </summary>
        /// <param name="c">The query to update.</param>
        /// <returns><see langword="true"/> when the record is in the current store and is updated; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The last-updated value is replaced with the current UTC time before persistence.</remarks>
        public bool Update(SearchQuery c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        /// <summary>
        /// Deletes a search-query record from the current store.
        /// </summary>
        /// <param name="bvin">The search-query identifier.</param>
        /// <returns><see langword="true"/> when the matching current-store record is deleted; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>
        /// Deletes all search-query records belonging to the current store.
        /// </summary>
        /// <returns><see langword="true"/> when the repository completes the delete operation.</returns>
        public bool DeleteAll()
        {
            return Delete(q => q.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>
        /// Returns all search-query records for the current store.
        /// </summary>
        /// <returns>All current-store queries ordered by <see cref="SearchQuery.QueryPhrase"/>.</returns>
        public List<SearchQuery> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        /// <summary>
        /// Returns a page of current-store search queries ordered by phrase.
        /// </summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of records in the page.</param>
        /// <param name="totalCount">When this method returns, the total number of matching current-store records.</param>
        /// <returns>The requested page of queries.</returns>
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


        /// <summary>
        /// Returns a page of current-store queries for a shopper, newest first.
        /// </summary>
        /// <param name="shopperId">The shopper identifier used for filtering.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of records in the page.</param>
        /// <param name="totalCount">When this method returns, the total number of matching current-store records.</param>
        /// <returns>The requested page of shopper queries ordered by descending <see cref="SearchQuery.LastUpdated"/>.</returns>
        /// <remarks>The persisted field is named <c>ShopperId</c>, while the public model property is intentionally <c>ShopperID</c>.</remarks>
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

        /// <summary>
        /// Builds a current-store report grouping records by query phrase.
        /// </summary>
        /// <returns>Report rows ordered by descending occurrence count.</returns>
        /// <remarks>
        /// Each distinct phrase produces one <see cref="SearchQueryData"/> row. <see cref="SearchQueryData.Percentage"/>
        /// is initialized to zero and is not calculated by this method; <see cref="SearchQueryData.Count"/> contains
        /// the number of records in the phrase's group.
        /// </remarks>
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

        /// <summary>
        /// Represents one row in the search-query count report.
        /// </summary>
        /// <remarks>
        /// The report repository populates <see cref="QueryPhrase"/> and <see cref="Count"/>. The
        /// <see cref="Percentage"/> field is retained for compatibility and defaults to zero; the
        /// repository's grouping operation does not calculate it.
        /// </remarks>
        public class SearchQueryData
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="SearchQueryData"/> class with empty or zero values.
            /// </summary>
            public SearchQueryData()
            {
                QueryPhrase = string.Empty;
                Count = 0;
                Percentage = 0;
            }

            /// <summary>
            /// Gets or sets the phrase represented by this report row.
            /// </summary>
            public string QueryPhrase { get; set; }

            /// <summary>
            /// Gets or sets the number of records grouped under <see cref="QueryPhrase"/>.
            /// </summary>
            public int Count { get; set; }

            /// <summary>
            /// Gets or sets the percentage associated with this row.
            /// </summary>
            /// <value>The percentage value; repository-generated count reports leave it at zero.</value>
            public decimal Percentage { get; set; }
        }
    }
}
