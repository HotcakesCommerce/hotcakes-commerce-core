#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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

namespace Hotcakes.Commerce.Catalog
{
    public class ProductVolumeDiscountRepository : HccSimpleRepoBase<hcc_ProductVolumeDiscounts, ProductVolumeDiscount>
    {
        public ProductVolumeDiscountRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_ProductVolumeDiscounts data, ProductVolumeDiscount model)
        {
            model.Amount = data.Amount;
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.DiscountType = (ProductVolumeDiscountType) data.DiscountType;
            model.LastUpdated = data.LastUpdated;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductID);
            model.Qty = data.Qty;
            model.StoreId = data.StoreId;
        }

        protected override void CopyModelToData(hcc_ProductVolumeDiscounts data, ProductVolumeDiscount model)
        {
            data.Amount = model.Amount;
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.DiscountType = (int) model.DiscountType;
            data.LastUpdated = model.LastUpdated;
            data.ProductID = DataTypeHelper.BvinToGuid(model.ProductId);
            data.Qty = model.Qty;
            data.StoreId = model.StoreId;
        }

        public ProductVolumeDiscount Find(string bvin)
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

        public ProductVolumeDiscount FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        public override bool Create(ProductVolumeDiscount item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;
            return base.Create(item);
        }

        public bool Update(ProductVolumeDiscount c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid && y.StoreId == storeId);
        }

        public List<ProductVolumeDiscount> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductId(productId, storeId);
        }

        internal List<ProductVolumeDiscount> FindByProductId(string productId, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductID == productGuid)
                    .OrderBy(y => y.Qty);
            });
        }

        public bool DeleteByProductId(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductID == productGuid);
        }

        public bool DeleteByProductId(string productId, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductID == productGuid && y.StoreId == storeId);
        }
    }
}