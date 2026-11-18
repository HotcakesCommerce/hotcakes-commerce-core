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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Marketing
{
    /// <summary>Persists promotions and their localized descriptions within the current store.</summary>
    public class PromotionRepository :
        HccLocalizationRepoBase<hcc_Promotions, hcc_PromotionTranslation, Promotion, long>
    {
        /// <summary>Initializes the repository for the supplied commerce request context.</summary>
        /// <param name="c">The context whose current store scopes all promotion queries.</param>
        public PromotionRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Gets the database expression identifying a promotion.</summary>
        protected override Expression<Func<hcc_Promotions, long>> ItemKeyExp
        {
            get { return p => p.Id; }
        }

        /// <summary>Gets the database expression identifying a promotion translation.</summary>
        protected override Expression<Func<hcc_PromotionTranslation, long>> ItemTranslationKeyExp
        {
            get { return pt => pt.PromotionId; }
        }

        /// <summary>Copies persisted promotion fields and rule XML into a domain model.</summary>
        protected override void CopyItemToModel(hcc_Promotions data, Promotion model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Mode = (PromotionType)data.Mode;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.Name = data.Name;
            model.StartDateUtc = data.StartDateUtc;
            model.EndDateUtc = data.EndDateUtc;
            model.IsEnabled = data.IsEnabled;
            model.ActionsFromXml(data.ActionsXml);
            model.QualificationsFromXml(data.QualificationsXml);
            model.DoNotCombine = data.DoNotCombine;
            model.SortOrder = data.SortOrder;
        }

        /// <summary>Copies the localized customer description into a domain model.</summary>
        protected override void CopyTransToModel(hcc_PromotionTranslation data, Promotion model)
        {
            model.CustomerDescription = data.CustomerDescription;
        }

        /// <summary>Copies a domain model, including serialized actions and qualifications, to persistence.</summary>
        protected override void CopyModelToItem(JoinedItem<hcc_Promotions, hcc_PromotionTranslation> data,
            Promotion model)
        {
            data.Item.Id = model.Id;
            data.Item.StoreId = model.StoreId;
            data.Item.Mode = (int)model.Mode;
            data.Item.LastUpdatedUtc = model.LastUpdatedUtc;
            data.Item.Name = model.Name;
            data.Item.StartDateUtc = model.StartDateUtc;
            data.Item.EndDateUtc = model.EndDateUtc;
            data.Item.IsEnabled = model.IsEnabled;
            data.Item.ActionsXml = model.ActionsToXml();
            data.Item.QualificationsXml = model.QualificationsToXml();
            data.Item.DoNotCombine = model.DoNotCombine;
            data.Item.SortOrder = model.SortOrder;
        }

        /// <summary>Copies the customer description to the localized persistence record.</summary>
        protected override void CopyModelToTrans(JoinedItem<hcc_Promotions, hcc_PromotionTranslation> data,
            Promotion model)
        {
            data.ItemTranslation.PromotionId = data.Item.Id;

            data.ItemTranslation.CustomerDescription = model.CustomerDescription;
        }

        /// <summary>Finds a promotion by identifier in the current store scope.</summary>
        /// <param name="id">The promotion identifier.</param>
        /// <returns>The promotion, or <see langword="null"/> when not found.</returns>
        public Promotion Find(long id)
        {
            return FindFirstPoco(y => y.Item.Id == id);
        }

        /// <summary>Creates a promotion in the current store and assigns its initial sort position.</summary>
        /// <param name="item">The promotion to persist.</param>
        /// <returns>Whether persistence succeeded.</returns>
        public override bool Create(Promotion item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdatedUtc = DateTime.UtcNow;

            // Performance improvement: Query only for MIN(SortOrder) instead of loading all promotions
            var storeId = Context.CurrentStore.Id;
            var mode = (int)item.Mode;

            using (var s = CreateReadStrategy())
            {
                var minSortOrder = s.GetQuery()
                    .AsNoTracking()
                    .Where(p => p.StoreId == storeId && p.Mode == mode)
                    .Min(p => (int?)p.SortOrder);

                item.SortOrder = minSortOrder.HasValue ? minSortOrder.Value - 1 : 1;
            }

            return base.Create(item);
        }

        /// <summary>Updates a promotion only when its store matches the current store.</summary>
        /// <param name="item">The promotion to update.</param>
        /// <returns><see langword="false"/> for a different store or a failed update.</returns>
        public bool Update(Promotion item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            item.LastUpdatedUtc = DateTime.UtcNow;
            return Update(item, y => y.Id == item.Id);
        }

        /// <summary>Deletes a promotion by identifier.</summary>
        /// <param name="id">The promotion identifier.</param>
        /// <returns>Whether the delete operation succeeded.</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>Returns all promotions for the current store.</summary>
        public List<Promotion> FindAll()
        {
            var rows = 0;
            return FindAllPaged(1, int.MaxValue, ref rows);
        }

        /// <summary>Returns a page of promotions for the current store, including disabled promotions.</summary>
        public List<Promotion> FindAllPaged(int pageNumber, int pageSize, ref int totalRowCount)
        {
            return FindAllWithFilter(PromotionType.Unknown, null, true, pageNumber, pageSize, ref totalRowCount);
        }

        /// <summary>Returns store-scoped promotions filtered by type, keyword, and enabled state, ordered by sort order.</summary>
        public List<Promotion> FindAllWithFilter(PromotionType type, string keyword, bool showDisabled, int pageNumber,
            int pageSize, ref int totalRowCount)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var query = GetJoinedQuery(s)
                    .AsNoTracking()
                    .Where(y => y.Item.StoreId == storeId);

                // type
                if (type != PromotionType.Unknown)
                {
                    query = query.Where(y => y.Item.Mode == (int)type);
                }

                // keyword 
                if (!string.IsNullOrWhiteSpace(keyword))
                {
                    query =
                        query.Where(
                            y =>
                                y.Item.Name.Contains(keyword) || y.ItemTranslation.CustomerDescription.Contains(keyword));
                }

                // show/hide disabled flag
                if (!showDisabled)
                {
                    query = query.Where(y => y.Item.IsEnabled);
                }

                // Performance improvement: Use OrderBy before getting count to enable potential query optimization
                var orderedQuery = query.OrderBy(y => y.Item.SortOrder);

                // Performance improvement: Execute count and paging in same query context
                var items = GetPagedItems(orderedQuery, pageNumber, pageSize).ToList();

                if (pageSize < int.MaxValue)
                {
                    totalRowCount = orderedQuery.Count();
                }
                else
                {
                    totalRowCount = items.Count;
                }

                return ListPoco(items);
            }
        }

        /// <summary>Returns enabled, date-window-matching promotions of a mode for the current store.</summary>
        /// <param name="currentDateTimeUtc">The UTC instant used for the inclusive date comparison.</param>
        /// <param name="type">The promotion mode.</param>
        public List<Promotion> FindAllPotentiallyActive(DateTime currentDateTimeUtc, PromotionType type)
        {
            var storeId = Context.CurrentStore.Id;
            var intMode = (int)type;

            // Performance improvement: Combine all Where clauses into a single predicate
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.StoreId == storeId
                                 && y.Item.IsEnabled
                                 && y.Item.StartDateUtc <= currentDateTimeUtc
                                 && y.Item.EndDateUtc >= currentDateTimeUtc
                                 && y.Item.Mode == intMode)
                    .OrderBy(y => y.Item.SortOrder);
            });
        }

        /// <summary>Returns potentially active sale promotions for the current store.</summary>
        public List<Promotion> FindAllPotentiallyActiveSales(DateTime currentDateTimeUtc)
        {
            return FindAllPotentiallyActive(currentDateTimeUtc, PromotionType.Sale);
        }

        /// <summary>Returns potentially active affiliate promotions for the current store.</summary>
        public List<Promotion> FindAllAffiliatePromotions(DateTime currentDateTimeUtc)
        {
            return FindAllPotentiallyActive(currentDateTimeUtc, PromotionType.Affiliate);
        }

        /// <summary>Returns promotions whose identifiers are supplied; an empty input returns an empty list.</summary>
        /// <param name="ids">Promotion identifiers to find.</param>
        public List<Promotion> FindByIds(List<long> ids)
        {
            if (ids == null || ids.Count == 0)
            {
                return new List<Promotion>();
            }

            return FindListPoco(q => { return q.Where(y => ids.Contains(y.Item.Id)); });
        }

        private int FindMaxSort()
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var maxSortOrder = s.GetQuery()
                    .AsNoTracking()
                    .Where(p => p.StoreId == storeId)
                    .Max(p => (int?)p.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        /// <summary>Updates sort order according to the supplied identifier sequence.</summary>
        /// <param name="sortedIds">Identifiers in desired order.</param>
        /// <param name="orderOffset">Value added before the one-based sequence position.</param>
        /// <returns>Whether the update succeeded.</returns>
        public bool Resort(List<long> sortedIds, int orderOffset = 0)
        {
            if (sortedIds == null || sortedIds.Count == 0)
            {
                return true;
            }

            // Performance improvement: Batch update all sort orders in a single database operation
            using (var s = CreateStrategy())
            {
                var promotions = s.GetQuery()
                    .Where(p => sortedIds.Contains(p.Id))
                    .ToList();

                for (var i = 0; i < sortedIds.Count; i++)
                {
                    var promotion = promotions.FirstOrDefault(p => p.Id == sortedIds[i]);
                    if (promotion != null)
                    {
                        promotion.SortOrder = orderOffset + i + 1;
                    }
                }

                return s.SubmitChanges();
            }
        }

        private bool UpdateSortOrder(long id, int newSortOrder)
        {
            // Performance improvement: Update only the SortOrder field without loading the entire Promotion model
            using (var s = CreateStrategy())
            {
                var item = s.GetQuery().FirstOrDefault(p => p.Id == id);
                if (item == null) return false;

                item.SortOrder = newSortOrder;
                return s.SubmitChanges();
            }
        }
    }
}