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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Marketing
{
    public class PromotionRepository :
        HccLocalizationRepoBase<hcc_Promotions, hcc_PromotionTranslation, Promotion, long>
    {
        public PromotionRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Expression<Func<hcc_Promotions, long>> ItemKeyExp
        {
            get { return p => p.Id; }
        }

        protected override Expression<Func<hcc_PromotionTranslation, long>> ItemTranslationKeyExp
        {
            get { return pt => pt.PromotionId; }
        }

        protected override void CopyItemToModel(hcc_Promotions data, Promotion model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Mode = (PromotionType) data.Mode;
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

        protected override void CopyTransToModel(hcc_PromotionTranslation data, Promotion model)
        {
            model.CustomerDescription = data.CustomerDescription;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_Promotions, hcc_PromotionTranslation> data,
            Promotion model)
        {
            data.Item.Id = model.Id;
            data.Item.StoreId = model.StoreId;
            data.Item.Mode = (int) model.Mode;
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

        protected override void CopyModelToTrans(JoinedItem<hcc_Promotions, hcc_PromotionTranslation> data,
            Promotion model)
        {
            data.ItemTranslation.PromotionId = data.Item.Id;

            data.ItemTranslation.CustomerDescription = model.CustomerDescription;
        }

        public Promotion Find(long id)
        {
            return FindFirstPoco(y => y.Item.Id == id);
        }

        public override bool Create(Promotion item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdatedUtc = DateTime.UtcNow;

            var totalRowCount = 0;
            int? minbSortOrder = null;
            var items = FindAllWithFilter(item.Mode, null, true, 1, int.MaxValue, ref totalRowCount);
            if (item != null && items.Any())
            {
                minbSortOrder = items.Min(y => y.SortOrder);
            }

            var newSortOrder = 1;
            if (minbSortOrder.HasValue)
            {
                newSortOrder = minbSortOrder.Value - 1;
            }
            item.SortOrder = newSortOrder;

            return base.Create(item);
        }

        public bool Update(Promotion item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            item.LastUpdatedUtc = DateTime.UtcNow;
            return Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<Promotion> FindAll()
        {
            var rows = 0;
            return FindAllPaged(1, int.MaxValue, ref rows);
        }

        public List<Promotion> FindAllPaged(int pageNumber, int pageSize, ref int totalRowCount)
        {
            return FindAllWithFilter(PromotionType.Unknown, null, true, pageNumber, pageSize, ref totalRowCount);
        }

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
                    query = query.Where(y => y.Item.Mode == (int) type);
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

                totalRowCount = query.Count();

                var items = GetPagedItems(query.OrderBy(y => y.Item.SortOrder), pageNumber, pageSize);
                return ListPoco(items);
            }
        }

        public List<Promotion> FindAllPotentiallyActive(DateTime currentDateTimeUtc, PromotionType type)
        {
            var storeId = Context.CurrentStore.Id;
            var intMode = (int) type;

            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.StoreId == storeId)
                    .Where(y => y.Item.IsEnabled)
                    .Where(y => y.Item.StartDateUtc <= currentDateTimeUtc)
                    .Where(y => y.Item.EndDateUtc >= currentDateTimeUtc)
                    .Where(i => i.Item.Mode == intMode)
                    .OrderBy(y => y.Item.SortOrder);
            });
        }

        public List<Promotion> FindAllPotentiallyActiveSales(DateTime currentDateTimeUtc)
        {
            return FindAllPotentiallyActive(currentDateTimeUtc, PromotionType.Sale);
        }

        public List<Promotion> FindAllAffiliatePromotions(DateTime currentDateTimeUtc)
        {
            return FindAllPotentiallyActive(currentDateTimeUtc, PromotionType.Affiliate);
        }

        public List<Promotion> FindByIds(List<long> ids)
        {
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
                    .Max(p => (int?) p.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        public bool Resort(List<long> sortedIds, int orderOffset = 0)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrder(sortedIds[i - 1], orderOffset + i);
                }
            }
            return true;
        }

        private bool UpdateSortOrder(long id, int newSortOrder)
        {
            var item = Find(id);
            if (item == null) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }
    }
}