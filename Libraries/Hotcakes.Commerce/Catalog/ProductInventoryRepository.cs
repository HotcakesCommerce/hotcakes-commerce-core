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
    public class ProductInventoryRepository : HccSimpleRepoBase<hcc_ProductInventory, ProductInventory>
    {
        public ProductInventoryRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_ProductInventory data, ProductInventory model)
        {
            model.Bvin = data.bvin;
            model.LastUpdated = data.LastUpdated;
            model.LowStockPoint = data.LowStockPoint;
            model.ProductBvin = DataTypeHelper.GuidToBvin(data.ProductBvin);
            // Calculated Field
            //model.QuantityAvailableForSale = data.QuantityAvailableForSale;
            model.QuantityOnHand = data.QuantityOnHand;
            model.QuantityReserved = data.QuantityReserved;
            model.StoreId = data.StoreId;
            model.VariantId = data.VariantId;
            model.OutOfStockPoint = data.OutOfStockPoint;
        }

        protected override void CopyModelToData(hcc_ProductInventory data, ProductInventory model)
        {
            data.bvin = model.Bvin;
            data.LastUpdated = model.LastUpdated;
            data.LowStockPoint = model.LowStockPoint;
            data.ProductBvin = DataTypeHelper.BvinToGuid(model.ProductBvin);
            data.QuantityAvailableForSale = model.QuantityAvailableForSale;
            data.QuantityOnHand = model.QuantityOnHand;
            data.QuantityReserved = model.QuantityReserved;
            data.StoreId = model.StoreId;
            data.VariantId = model.VariantId;
            data.OutOfStockPoint = model.OutOfStockPoint;
        }

        public ProductInventory Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin && y.StoreId == Context.CurrentStore.Id);
        }

        public ProductInventory FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public bool InventoryExists(ProductInventory item)
        {
            var inventory = FindByProductIdAndVariantId(item.ProductBvin, item.VariantId);
            if (inventory != null) return true;
            return false;
        }

        public override bool Create(ProductInventory item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;

            return base.Create(item);
        }

        public bool Update(ProductInventory c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            return Delete(y => y.bvin == bvin && y.StoreId == storeId);
        }

        public List<ProductInventory> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductId(productId, storeId);
        }

        public List<ProductInventory> FindByProductId(string productId, long storeId)
        {
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductBvin == productBvin);
            });
        }

        public ProductInventory FindByProductIdAndVariantId(string productId, string variantId)
        {
            var storeId = Context.CurrentStore.Id;
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            using (var s = CreateStrategy())
            {
                var query = s.GetQuery().Where(y => y.StoreId == storeId)
                    .Where(y => y.ProductBvin == productBvin)
                    .Where(y => y.VariantId == variantId).FirstOrDefault();
                return FirstPoco(query);
            }
        }

        public bool DeleteByProductId(string productId)
        {
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductBvin == productBvin);
        }

        public bool DeleteByProductId(string productId, long storeId)
        {
            var productBvin = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductBvin == productBvin && y.StoreId == storeId);
        }

        public List<ProductInventory> FindAllLowStock()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.QuantityAvailableForSale <= y.LowStockPoint);
            });
        }

        public List<ProductInventory> FindAllForCurrentStore()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId); });
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ProductInventoryRepository InstantiateForMemory(HccRequestContext c)
        {
            return new ProductInventoryRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ProductInventoryRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new ProductInventoryRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ProductInventoryRepository(HccRequestContext c, IRepositoryStrategy<hcc_ProductInventory> r,
            ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}