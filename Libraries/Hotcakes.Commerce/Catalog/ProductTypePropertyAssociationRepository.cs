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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>Provides the catalog ProductTypePropertyAssociationRepository API.</summary>
    public class ProductTypePropertyAssociationRepository :
        HccSimpleRepoBase<hcc_ProductTypeXProductProperty, ProductTypePropertyAssociation>
    {
        public ProductTypePropertyAssociationRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Gets or sets the CopyDataToModel value.</summary>
        protected override void CopyDataToModel(hcc_ProductTypeXProductProperty data,
            ProductTypePropertyAssociation model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.ProductTypeBvin = DataTypeHelper.GuidToBvin(data.ProductTypeBvin);
            model.PropertyId = data.PropertyId;
            model.SortOrder = data.SortOrder;
        }

        /// <summary>Gets or sets the CopyModelToData value.</summary>
        protected override void CopyModelToData(hcc_ProductTypeXProductProperty data,
            ProductTypePropertyAssociation model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.ProductTypeBvin = DataTypeHelper.BvinToGuid(model.ProductTypeBvin);
            data.PropertyId = model.PropertyId;
            data.SortOrder = model.SortOrder;
        }

        /// <summary>Gets or sets the Find value.</summary>
        /// <param name="id">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the FindForAllStores value.</summary>
        /// <param name="id">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public ProductTypePropertyAssociation FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Gets or sets the FindForTypeAndProperty value.</summary>
        /// <param name="typeBvin">Input used by this operation.</param>
        /// <param name="propertyId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public ProductTypePropertyAssociation FindForTypeAndProperty(string typeBvin, long propertyId)
        {
            var storeId = Context.CurrentStore.Id;
            var typeGuid = DataTypeHelper.BvinToGuid(typeBvin);
            return
                FindFirstPoco(y => y.StoreId == storeId && y.PropertyId == propertyId && y.ProductTypeBvin == typeGuid);
        }

        /// <summary>Gets or sets the Create value.</summary>
        /// <param name="item">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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
            using (var s = CreateReadStrategy())
            {
                var maxSortOrder = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductTypeBvin == typeGuid)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        /// <summary>Gets or sets the Update value.</summary>
        /// <param name="c">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool Update(ProductTypePropertyAssociation c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>Gets or sets the ResortProperties value.</summary>
        /// <param name="typeBvin">Input used by this operation.</param>
        /// <param name="sortedIds">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the UpdateSortOrder value.</summary>
        /// <param name="typeBvin">Input used by this operation.</param>
        /// <param name="propertyId">Input used by this operation.</param>
        /// <param name="sortOrder">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the UpdateSortOrder value.</summary>
        /// <param name="id">Input used by this operation.</param>
        /// <param name="sortOrder">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the Delete value.</summary>
        /// <param name="id">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        /// <summary>Gets or sets the DeleteForProperty value.</summary>
        /// <param name="propertyId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool DeleteForProperty(long propertyId)
        {
            return Delete(y => y.PropertyId == propertyId);
        }

        /// <summary>Gets or sets the DeleteForProductType value.</summary>
        /// <param name="productTypeBvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool DeleteForProductType(string productTypeBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(productTypeBvin);
            return Delete(y => y.ProductTypeBvin == guid);
        }

        /// <summary>Gets or sets the DeleteForTypeAndProperty value.</summary>
        /// <param name="typeBvin">Input used by this operation.</param>
        /// <param name="propertyId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool DeleteForTypeAndProperty(string typeBvin, long propertyId)
        {
            var typeGuid = DataTypeHelper.BvinToGuid(typeBvin);
            return Delete(y => y.PropertyId == propertyId && y.ProductTypeBvin == typeGuid);
        }

        /// <summary>Gets or sets the FindByProperty value.</summary>
        /// <param name="propertyId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the FindByProductType value.</summary>
        /// <param name="productTypeBvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the CountByProductType value.</summary>
        /// <param name="productTypeBvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public int CountByProductType(string productTypeBvin)
        {
            var result = 0;
            var storeId = Context.CurrentStore.Id;
            var productTypeGuid = DataTypeHelper.BvinToNullableGuid(productTypeBvin);
            using (var s = CreateReadStrategy())
            {
                var temp = s.GetQuery().AsNoTracking().Where(y => y.StoreId == storeId)
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
    }
}
