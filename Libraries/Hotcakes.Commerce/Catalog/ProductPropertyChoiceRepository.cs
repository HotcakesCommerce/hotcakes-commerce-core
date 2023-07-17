#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductPropertyChoiceRepository :
        HccLocalizationRepoBase
            <hcc_ProductPropertyChoice, hcc_ProductPropertyChoiceTranslation, ProductPropertyChoice, long>
    {
        public ProductPropertyChoiceRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Expression<Func<hcc_ProductPropertyChoice, long>> ItemKeyExp
        {
            get { return ppc => ppc.Id; }
        }

        protected override Expression<Func<hcc_ProductPropertyChoiceTranslation, long>> ItemTranslationKeyExp
        {
            get { return ppct => ppct.ProductPropertyChoiceId; }
        }

        protected override Func<JoinedItem<hcc_ProductPropertyChoice, hcc_ProductPropertyChoiceTranslation>, bool>
            MatchItems(ProductPropertyChoice item)
        {
            return ppc => ppc.Item.Id == item.Id;
        }

        protected override Func<JoinedItem<hcc_ProductPropertyChoice, hcc_ProductPropertyChoiceTranslation>, bool>
            NotMatchItems(List<ProductPropertyChoice> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return ppc => !itemIds.Contains(ppc.Item.Id);
        }

        protected override void CopyItemToModel(hcc_ProductPropertyChoice data, ProductPropertyChoice model)
        {
            model.ChoiceName = data.ChoiceName;
            model.Id = data.Id;
            model.LastUpdated = data.LastUpdated;
            model.PropertyId = data.PropertyId;
            model.SortOrder = data.SortOrder;
            model.StoreId = data.StoreId;
        }

        protected override void CopyTransToModel(hcc_ProductPropertyChoiceTranslation data, ProductPropertyChoice model)
        {
            model.DisplayName = data.DisplayName;
        }

        protected override void CopyModelToItem(
            JoinedItem<hcc_ProductPropertyChoice, hcc_ProductPropertyChoiceTranslation> data,
            ProductPropertyChoice model)
        {
            data.Item.ChoiceName = model.ChoiceName;
            data.Item.Id = model.Id;
            data.Item.LastUpdated = model.LastUpdated;
            data.Item.PropertyId = model.PropertyId;
            data.Item.SortOrder = model.SortOrder;
            data.Item.StoreId = model.StoreId;
        }

        protected override void CopyModelToTrans(
            JoinedItem<hcc_ProductPropertyChoice, hcc_ProductPropertyChoiceTranslation> data,
            ProductPropertyChoice model)
        {
            data.ItemTranslation.ProductPropertyChoiceId = data.Item.Id;

            data.ItemTranslation.DisplayName = model.DisplayName;
        }

        public ProductPropertyChoice Find(long id)
        {
            return FindFirstPoco(y => y.Item.Id == id);
        }

        public override bool Create(ProductPropertyChoice item)
        {
            item.LastUpdated = DateTime.UtcNow;
            //item.SortOrder = FindMaxSort(FindForProperty(item.PropertyId));
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(ProductPropertyChoice item)
        {
            item.LastUpdated = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<ProductPropertyChoice> FindForProperty(long propertyId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.PropertyId == propertyId)
                    .OrderBy(y => y.Item.SortOrder);
            });
        }

        public List<ProductPropertyChoice> FindForManyProperty(List<long> propertyIds)
        {
            return FindListPoco(q => q.Where(cb => propertyIds.Contains(cb.Item.PropertyId)));
        }

        public void DeleteForProperty(long propertyId)
        {
            Delete(y => y.PropertyId == propertyId);
        }

        private int FindMaxSort(List<ProductPropertyChoice> items)
        {
            var maxSort = 0;
            if (items == null) return 0;
            if (items.Count < 1) return 0;
            maxSort = items.Max(y => y.SortOrder);
            return maxSort;
        }

        public bool Resort(long propertyId, List<long> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForChoice(propertyId, sortedIds[i - 1], i);
                }
            }
            return true;
        }

        private bool UpdateSortOrderForChoice(long propertyId, long choiceId, int newSortOrder)
        {
            var item = Find(choiceId);
            if (item == null) return false;
            if (item.PropertyId != propertyId) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        public void MergeList(long propertyId, List<ProductPropertyChoice> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.PropertyId = propertyId;
                item.LastUpdated = DateTime.UtcNow;
            }

            MergeList(subitems, ppc => ppc.PropertyId == propertyId);
        }
    }
}