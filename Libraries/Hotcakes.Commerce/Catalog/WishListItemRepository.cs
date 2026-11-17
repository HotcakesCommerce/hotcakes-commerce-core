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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>Provides the catalog WishListItemRepository API.</summary>
    public class WishListItemRepository : HccSimpleRepoBase<hcc_WishListItem, WishListItem>
    {
        public WishListItemRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Gets or sets the CopyDataToModel value.</summary>
        /// <param name="data">Input used by this operation.</param>
        /// <param name="model">Input used by this operation.</param>
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

        /// <summary>Gets or sets the CopyModelToData value.</summary>
        /// <param name="data">Input used by this operation.</param>
        /// <param name="model">Input used by this operation.</param>
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

        /// <summary>Gets or sets the Find value.</summary>
        /// <param name="id">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the FindForAllStores value.</summary>
        /// <param name="id">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public WishListItem FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Gets or sets the Create value.</summary>
        /// <param name="item">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public override bool Create(WishListItem item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>Gets or sets the Update value.</summary>
        /// <param name="c">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool Update(WishListItem c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
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

        /// <summary>Gets or sets the FindByProductId value.</summary>
        /// <param name="productId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public List<WishListItem> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductIdForStore(productId, storeId);
        }

        /// <summary>Gets or sets the FindByProductIdForStore value.</summary>
        /// <param name="productId">Input used by this operation.</param>
        /// <param name="storeId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public List<WishListItem> FindByProductIdForStore(string productId, long storeId)
        {
            return FindByProductIdPaged(productId, 1, int.MaxValue, storeId);
        }

        /// <summary>Gets or sets the FindByProductIdPaged value.</summary>
        /// <param name="productId">Input used by this operation.</param>
        /// <param name="pageNumber">Input used by this operation.</param>
        /// <param name="pageSize">Input used by this operation.</param>
        /// <param name="storeId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the FindByCustomerIdPaged value.</summary>
        /// <param name="customerId">Input used by this operation.</param>
        /// <param name="pageNumber">Input used by this operation.</param>
        /// <param name="pageSize">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
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

        /// <summary>Gets or sets the DeleteForProductId value.</summary>
        /// <param name="productBvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool DeleteForProductId(string productBvin)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            return Delete(y => y.ProductId == productGuid);
        }

        /// <summary>Gets or sets the DeleteForProductIdForStore value.</summary>
        /// <param name="productBvin">Input used by this operation.</param>
        /// <param name="storeId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool DeleteForProductIdForStore(string productBvin, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            return Delete(y => y.ProductId == productGuid && y.StoreId == storeId);
        }

        /// <summary>Gets or sets the DeleteForCustomerId value.</summary>
        /// <param name="customerId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool DeleteForCustomerId(string customerId)
        {
            return Delete(y => y.CustomerId == customerId);
        }


        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
