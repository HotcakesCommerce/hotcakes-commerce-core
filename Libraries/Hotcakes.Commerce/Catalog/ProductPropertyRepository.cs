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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductPropertyRepository :
        HccLocalizationRepoBase<hcc_ProductProperty, hcc_ProductPropertyTranslation, ProductProperty, long>
    {
        private readonly ProductPropertyChoiceRepository choiceRepository;

        public ProductPropertyRepository(HccRequestContext c)
            : base(c)
        {
            choiceRepository = new ProductPropertyChoiceRepository(c);
        }

        protected override Expression<Func<hcc_ProductProperty, long>> ItemKeyExp
        {
            get { return pp => pp.Id; }
        }

        protected override Expression<Func<hcc_ProductPropertyTranslation, long>> ItemTranslationKeyExp
        {
            get { return ppt => ppt.ProductPropertyId; }
        }


        protected override void CopyItemToModel(hcc_ProductProperty data, ProductProperty model)
        {
            model.CultureCode = data.CultureCode;
            model.DefaultValue = data.DefaultValue;
            model.DisplayOnSite = data.DisplayOnSite == 1;
            model.DisplayToDropShipper = data.DisplayToDropShipper == 1;
            model.DisplayOnSearch = data.DisplayOnSearch;
            model.Id = data.Id;
            model.PropertyName = data.PropertyName;
            model.StoreId = data.StoreId;
            model.TypeCode = (ProductPropertyType) data.TypeCode;
            model.LastUpdatedUtc = data.LastUpdated;
            model.IsLocalizable = data.IsLocalizable;
        }

        protected override void CopyTransToModel(hcc_ProductPropertyTranslation data, ProductProperty model)
        {
            model.DisplayName = data.DisplayName;
            model.DefaultLocalizableValue = data.DefaultLocalizableValue;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_ProductProperty, hcc_ProductPropertyTranslation> data,
            ProductProperty model)
        {
            data.Item.CultureCode = model.CultureCode;
            data.Item.DefaultValue = model.DefaultValue;
            data.Item.DisplayOnSite = model.DisplayOnSite ? 1 : 0;
            data.Item.DisplayToDropShipper = model.DisplayToDropShipper ? 1 : 0;
            data.Item.DisplayOnSearch = model.DisplayOnSearch;
            data.Item.Id = model.Id;
            data.Item.PropertyName = model.PropertyName;
            data.Item.StoreId = model.StoreId;
            data.Item.TypeCode = (int) model.TypeCode;
            data.Item.LastUpdated = model.LastUpdatedUtc;
            data.Item.IsLocalizable = model.IsLocalizable;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_ProductProperty, hcc_ProductPropertyTranslation> data,
            ProductProperty model)
        {
            data.ItemTranslation.ProductPropertyId = data.Item.Id;

            data.ItemTranslation.DisplayName = model.DisplayName;
            data.ItemTranslation.DefaultLocalizableValue = model.DefaultLocalizableValue;
        }

        protected override void GetSubItems(List<ProductProperty> models)
        {
            var propertyIds = models.Select(s => s.Id).ToList();
            var propertyChoices = choiceRepository.FindForManyProperty(propertyIds);

            foreach (var model in models)
            {
                var property = propertyChoices.Where(pc => pc.PropertyId == model.Id).ToList();
                model.Choices = property;
            }
        }

        protected override void MergeSubItems(ProductProperty model)
        {
            choiceRepository.MergeList(model.Id, model.Choices);
        }

        public ProductProperty Find(long id)
        {
            var result = FindForAllStores(id);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        public ProductProperty FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Item.Id == id);
        }

        public ProductProperty FindByName(string name)
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var item = GetJoinedQuery(s).AsNoTracking().Where(y => y.Item.StoreId == storeId)
                    .Where(y => y.Item.PropertyName == name).FirstOrDefault();
                return FirstPoco(item);
            }
        }

        public override bool Create(ProductProperty item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(ProductProperty c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id);
        }

        public List<ProductProperty> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.StoreId == storeId)
                    .OrderBy(y => y.Item.PropertyName);
            });
        }

        public List<ProductProperty> FindMany(List<long> ids)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => ids.Contains(y.Item.Id))
                    .Where(y => y.Item.StoreId == storeId)
                    .OrderBy(y => y.Item.PropertyName);
            });
        }

        public bool DeleteChoice(long id)
        {
            return choiceRepository.Delete(id);
        }

        public bool ResortChoices(long propertyId, List<long> sortedItemIds)
        {
            return choiceRepository.Resort(propertyId, sortedItemIds);
        }

        public ProductPropertyChoice ChoiceFind(long id)
        {
            return choiceRepository.Find(id);
        }

        public bool ChoiceUpdate(ProductPropertyChoice item)
        {
            return choiceRepository.Update(item);
        }

        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }


        public bool ResortProductPropertyChoices(List<long> sortedProductPropertyChoices)
        {
            if (sortedProductPropertyChoices != null)
            {
                for (var i = 0; i < sortedProductPropertyChoices.Count; i++)
                {
                    UpdateSortOrderForPropertyChoices(sortedProductPropertyChoices[i], i + 1);
                }
            }
            return true;
        }

        private bool UpdateSortOrderForPropertyChoices(long productPropertyChoiceId, int newSortOrder)
        {
            var item = choiceRepository.Find(productPropertyChoiceId);
            if (item == null)
                return false;
            item.SortOrder = newSortOrder;
            return choiceRepository.Update(item);
        }
    }
}