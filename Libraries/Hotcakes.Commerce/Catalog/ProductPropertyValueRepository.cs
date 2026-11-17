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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Defines the catalog data or behavior represented by this member.
    /// </summary>
    public class ProductPropertyValueRepository :
        HccLocalizationRepoBase
            <hcc_ProductPropertyValue, hcc_ProductPropertyValueTranslation, ProductPropertyValue, long>
    {
        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public ProductPropertyValueRepository(HccRequestContext c) :
            base(c)
        {
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override Expression<Func<hcc_ProductPropertyValue, long>> ItemKeyExp
        {
            get { return ppv => ppv.Id; }
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override Expression<Func<hcc_ProductPropertyValueTranslation, long>> ItemTranslationKeyExp
        {
            get { return ppvt => ppvt.ProductPropertyValueId; }
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override void CopyItemToModel(hcc_ProductPropertyValue data, ProductPropertyValue model)
        {
            model.Id = data.Id;
            model.ProductID = DataTypeHelper.GuidToBvin(data.ProductBvin);
            model.PropertyID = data.PropertyId;
            model.StoreId = data.StoreId;
            model.PropertyValue = data.PropertyValue;
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override void CopyTransToModel(hcc_ProductPropertyValueTranslation data, ProductPropertyValue model)
        {
            model.PropertyLocalizableValue = data.PropertyLocalizableValue;
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override void CopyModelToItem(
            JoinedItem<hcc_ProductPropertyValue, hcc_ProductPropertyValueTranslation> data, ProductPropertyValue model)
        {
            data.Item.Id = model.Id;
            data.Item.ProductBvin = DataTypeHelper.BvinToGuid(model.ProductID);
            data.Item.PropertyId = model.PropertyID;
            data.Item.StoreId = model.StoreId;
            data.Item.PropertyValue = model.PropertyValue;
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override void CopyModelToTrans(
            JoinedItem<hcc_ProductPropertyValue, hcc_ProductPropertyValueTranslation> data, ProductPropertyValue model)
        {
            data.ItemTranslation.ProductPropertyValueId = data.Item.Id;

            data.ItemTranslation.PropertyLocalizableValue = model.PropertyLocalizableValue;
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public ProductPropertyValue Find(long id)
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

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public ProductPropertyValue FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Item.Id == id);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public override bool Create(ProductPropertyValue item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public bool Update(ProductPropertyValue c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<ProductPropertyValue> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductId(productId, storeId);
        }

        internal List<ProductPropertyValue> FindByProductId(string productId, long storeId)
        {
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.Item.StoreId == storeId)
                    .Where(y => y.Item.ProductBvin == productBvin);
            });
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public ProductPropertyValue FindByProductIdAndPropertyId(string productId, long propertyId,
            bool singleCulture = false)
        {
            var storeId = Context.CurrentStore.Id;
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            using (var s = CreateReadStrategy())
            {
                var item = GetJoinedQuery(s)
                    .AsNoTracking()
                    .Where(y => y.Item.StoreId == storeId)
                    .Where(y => y.Item.ProductBvin == productBvin)
                    .Where(y => y.Item.PropertyId == propertyId).FirstOrDefault();

                return FirstPoco(item);
            }
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<string> FindProductsContainingAllChoiceIds(List<long> choiceIds)
        {
            return FindProductsContainingAllChoiceIds(choiceIds, 1, int.MaxValue);
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<string> FindProductsContainingAllChoiceIds(List<long> choiceIds, int pageNumber, int pageSize)
        {
            // Find all product bvins that have at least a single value for all 
            var result = new List<string>();


            // Convert longs to strings for matching
            var choiceIdStrings = new List<string>();
            foreach (var l in choiceIds)
            {
                if (l > 0)
                {
                    choiceIdStrings.Add(l.ToString());
                }
            }
            var choiceIdCount = choiceIdStrings.Count;

            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var matchingItems = GetJoinedQuery(s).Where(y => y.Item.StoreId == storeId);

                if (choiceIdStrings.Count > 0)
                {
                    matchingItems = matchingItems.Where(y => choiceIdStrings.Contains(y.Item.PropertyValue));
                }

                var match2 = matchingItems.GroupBy(y => y.Item.ProductBvin).
                    Select(grouping => grouping.Select(y => new
                    {
                        Bvin = y.Item.ProductBvin,
                        C = grouping.Count()
                    }).
                        FirstOrDefault()).
                    Where(y => y.C >= choiceIdCount).
                    OrderBy(y => y.Bvin);

                var take = pageSize;
                var skip = (pageNumber - 1)*pageSize;
                var paged = match2.Skip(skip).Take(take);
                if (paged == null) return result;
                var bvins = paged.Select(g => DataTypeHelper.GuidToBvin(g.Bvin)).ToList();
                result.AddRange(bvins);
                return result;
            }
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public int FindCountOfProductsContainingAllChoiceIds(List<long> choiceIds)
        {
            // Find all product bvins that have at least a single value for all 
            var result = 0;

            // Convert longs to strings for matching
            var choiceIdStrings = new List<string>();
            foreach (var l in choiceIds)
            {
                if (l > 0)
                {
                    choiceIdStrings.Add(l.ToString());
                }
            }
            var choiceIdCount = choiceIdStrings.Count;

            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var matchingItems = GetJoinedQuery(s).Where(y => y.Item.StoreId == storeId);

                if (choiceIdStrings.Count > 0)
                {
                    matchingItems = matchingItems.Where(y => choiceIdStrings.Contains(y.Item.PropertyValue));
                }

                var match2 = matchingItems.GroupBy(y => y.Item.ProductBvin).
                    Select(grouping => grouping.Select(y => new
                    {
                        Bvin = y.Item.ProductBvin,
                        C = grouping.Count()
                    }).
                        FirstOrDefault()).
                    Where(y => y.C >= choiceIdCount);

                result = match2.Count();

                return result;
            }
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<string> FindProductIdsMatchingKey(string key, int pageNumber, int pageSize)
        {
            var choiceIds = CategoryFacetKeyHelper.ParseKeyToList(key);
            return FindProductsContainingAllChoiceIds(choiceIds, pageNumber, pageSize);
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public int FindCountProductIdsMatchingKey(string key)
        {
            var choiceIds = CategoryFacetKeyHelper.ParseKeyToList(key);
            return FindCountOfProductsContainingAllChoiceIds(choiceIds);
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool DeleteByProductId(string productId)
        {
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductBvin == productBvin);
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool DeleteByProductId(string productId, long storeId)
        {
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductBvin == productBvin && y.StoreId == storeId);
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool DeleteByPropertyId(long propertyId)
        {
            return Delete(y => y.PropertyId == propertyId);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public bool SetPropertyValue(string productId, ProductProperty property, string propertyValueString)
        {
            var propertyValue = FindByProductIdAndPropertyId(productId, property.Id);

            var create = false;
            if (propertyValue == null)
            {
                propertyValue = new ProductPropertyValue();
                create = true;
            }
            propertyValue.ProductID = productId;
            propertyValue.PropertyID = property.Id;

            if (propertyValueString == null)
                propertyValueString = property.IsLocalizable ? property.DefaultLocalizableValue : property.DefaultValue;

            if (property.IsLocalizable)
                propertyValue.PropertyLocalizableValue = propertyValueString;
            else
                propertyValue.PropertyValue = propertyValueString;

            if (create)
                return Create(propertyValue);
            return Update(propertyValue);
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public string GetPropertyValue(string productId, ProductProperty property, bool singleCulture = false)
        {
            var propertyValue = FindByProductIdAndPropertyId(productId, property.Id, singleCulture);
            if (propertyValue != null)
            {
                return property.IsLocalizable ? propertyValue.PropertyLocalizableValue : propertyValue.PropertyValue;
            }
            return null;
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public void CloneForProduct(string productId, string newProductId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var newProductGuid = DataTypeHelper.BvinToGuid(newProductId);


            using (var s = CreateStrategy())
            {
                var propertyValues = s.GetQuery()
                    .Where(ppv => ppv.ProductBvin == productGuid)
                    .ToList();

                foreach (var propertyValue in propertyValues)
                {
                    s.Detach(propertyValue);

                    var oldPropertyValueKey = propertyValue.Id;
                    propertyValue.ProductBvin = newProductGuid;

                    s.Add(propertyValue);

                    var propertyValueTranslations = s.GetQuery<hcc_ProductPropertyValueTranslation>()
                        .Where(pvt => pvt.ProductPropertyValueId == oldPropertyValueKey)
                        .ToList();

                    foreach (var propertyValueTranslation in propertyValueTranslations)
                    {
                        s.Detach(propertyValueTranslation);
                        propertyValueTranslation.ProductPropertyValueId = propertyValue.Id;
                        s.AddEntity(propertyValueTranslation);
                    }

                    newProductId = newProductGuid.ToString();
                }
                s.SubmitChanges();
            }
        }
    }
}
