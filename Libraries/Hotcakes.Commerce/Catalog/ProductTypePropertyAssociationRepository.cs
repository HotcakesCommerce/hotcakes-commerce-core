#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductTypePropertyAssociationRepository :
        HccSimpleRepoBase<hcc_ProductTypeXProductProperty, ProductTypePropertyAssociation>
    {
        public ProductTypePropertyAssociationRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_ProductTypeXProductProperty data,
            ProductTypePropertyAssociation model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.ProductTypeBvin = DataTypeHelper.GuidToBvin(data.ProductTypeBvin);
            model.PropertyId = data.PropertyId;
            model.SortOrder = data.SortOrder;
        }

        protected override void CopyModelToData(hcc_ProductTypeXProductProperty data,
            ProductTypePropertyAssociation model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.ProductTypeBvin = DataTypeHelper.BvinToGuid(model.ProductTypeBvin);
            data.PropertyId = model.PropertyId;
            data.SortOrder = model.SortOrder;
        }

        public ProductTypePropertyAssociation Find(long id)
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

        public ProductTypePropertyAssociation FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public ProductTypePropertyAssociation FindForTypeAndProperty(string typeBvin, long propertyId)
        {
            var storeId = Context.CurrentStore.Id;
            var typeGuid = DataTypeHelper.BvinToGuid(typeBvin);
            return
                FindFirstPoco(y => y.StoreId == storeId && y.PropertyId == propertyId && y.ProductTypeBvin == typeGuid);
        }

        public override bool Create(ProductTypePropertyAssociation item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.SortOrder = FindMaxSort(item.ProductTypeBvin);
            return base.Create(item);
        }

        private int FindMaxSort(string typeBvin)
        {
            var storeId = Context.CurrentStore.Id;
            var typeGuid = DataTypeHelper.BvinToGuid(typeBvin);
            using (var s = CreateStrategy())
            {
                var maxSortOrder = s.GetQuery()
                    .Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductTypeBvin == typeGuid)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        public bool Update(ProductTypePropertyAssociation c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        public bool ResortProperties(string typeBvin, List<long> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrder(typeBvin, sortedIds[i - 1], i);
                }
            }
            return true;
        }

        public bool UpdateSortOrder(string typeBvin, long propertyId, int sortOrder)
        {
            var item = FindForTypeAndProperty(typeBvin, propertyId);
            if (item != null)
            {
                item.SortOrder = sortOrder;
                return Update(item);
            }
            return false;
        }

        public bool UpdateSortOrder(long id, int sortOrder)
        {
            var item = Find(id);
            if (item != null)
            {
                item.SortOrder = sortOrder;
                return Update(item);
            }
            return false;
        }

        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        public bool DeleteForProperty(long propertyId)
        {
            return Delete(y => y.PropertyId == propertyId);
        }

        public bool DeleteForProductType(string productTypeBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(productTypeBvin);
            return Delete(y => y.ProductTypeBvin == guid);
        }

        public bool DeleteForTypeAndProperty(string typeBvin, long propertyId)
        {
            var typeGuid = DataTypeHelper.BvinToGuid(typeBvin);
            return Delete(y => y.PropertyId == propertyId && y.ProductTypeBvin == typeGuid);
        }

        public List<ProductTypePropertyAssociation> FindByProperty(long propertyId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.PropertyId == propertyId)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            });
        }

        public List<ProductTypePropertyAssociation> FindByProductType(string productTypeBvin)
        {
            var storeId = Context.CurrentStore.Id;
            var productTypeGuid = DataTypeHelper.BvinToNullableGuid(productTypeBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductTypeBvin == productTypeGuid)
                    .OrderBy(y => y.SortOrder);
            });
        }

        public int CountByProductType(string productTypeBvin)
        {
            var result = 0;
            var storeId = Context.CurrentStore.Id;
            var productTypeGuid = DataTypeHelper.BvinToNullableGuid(productTypeBvin);
            using (var s = CreateStrategy())
            {
                var temp = s.GetQuery().Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductTypeBvin == productTypeGuid).Count();

                if (temp >= 0)
                {
                    result = temp;
                }
                return result;
            }
        }

        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ProductTypePropertyAssociationRepository InstantiateForMemory(HccRequestContext c)
        {
            return new ProductTypePropertyAssociationRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ProductTypePropertyAssociationRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new ProductTypePropertyAssociationRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ProductTypePropertyAssociationRepository(HccRequestContext c,
            IRepositoryStrategy<hcc_ProductTypeXProductProperty> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}