#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    public class WishListItemRepository : HccSimpleRepoBase<hcc_WishListItem, WishListItem>
    {
        public WishListItemRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_WishListItem data, WishListItem model)
        {
            model.Id = data.Id;
            model.CustomerId = data.CustomerId;
            model.LastUpdatedUtc = data.LastUpdated;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
            model.Quantity = data.Quantity;
            model.SelectionData.DeserializeFromXml(data.SelectionData);
            model.StoreId = data.StoreId;
        }

        protected override void CopyModelToData(hcc_WishListItem data, WishListItem model)
        {
            data.Id = model.Id;
            data.CustomerId = model.CustomerId;
            data.LastUpdated = model.LastUpdatedUtc;
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
            data.Quantity = model.Quantity;
            data.SelectionData = model.SelectionData.SerializeToXml();
            data.StoreId = model.StoreId;
        }

        public WishListItem Find(long id)
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

        public WishListItem FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public override bool Create(WishListItem item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(WishListItem c)
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
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        public List<WishListItem> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductIdForStore(productId, storeId);
        }

        public List<WishListItem> FindByProductIdForStore(string productId, long storeId)
        {
            return FindByProductIdPaged(productId, 1, int.MaxValue, storeId);
        }

        public List<WishListItem> FindByProductIdPaged(string productId, int pageNumber, int pageSize, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.ProductId);
            }, pageNumber, pageSize);
        }

        public List<WishListItem> FindByCustomerIdPaged(string customerId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.CustomerId == customerId)
                    .Where(y => y.StoreId == storeId)
                    .OrderByDescending(y => y.LastUpdated);
            }, pageNumber, pageSize);
        }

        public bool DeleteForProductId(string productBvin)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            return Delete(y => y.ProductId == productGuid);
        }

        public bool DeleteForProductIdForStore(string productBvin, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            return Delete(y => y.ProductId == productGuid && y.StoreId == storeId);
        }

        public bool DeleteForCustomerId(string customerId)
        {
            return Delete(y => y.CustomerId == customerId);
        }


        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static WishListItemRepository InstantiateForMemory(HccRequestContext c)
        {
            return new WishListItemRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static WishListItemRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new WishListItemRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public WishListItemRepository(HccRequestContext c, IRepositoryStrategy<hcc_WishListItem> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}