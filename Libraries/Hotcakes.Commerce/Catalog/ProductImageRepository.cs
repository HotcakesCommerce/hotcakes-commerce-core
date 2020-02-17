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
using Hotcakes.Commerce.Storage;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductImageRepository : HccSimpleRepoBase<hcc_ProductImage, ProductImage>
    {
        public ProductImageRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_ProductImage, bool> MatchItems(ProductImage item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return pi => pi.bvin == guid;
        }

        protected override Func<hcc_ProductImage, bool> NotMatchItems(List<ProductImage> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return pi => !itemGuids.Contains(pi.bvin);
        }

        protected override void CopyDataToModel(hcc_ProductImage data, ProductImage model)
        {
            model.AlternateText = data.AlternateText;
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.Caption = data.Caption;
            model.FileName = data.FileName;
            model.LastUpdatedUtc = data.LastUpdated;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductID);
            model.SortOrder = data.SortOrder;
            model.StoreId = data.StoreId;
        }

        protected override void CopyModelToData(hcc_ProductImage data, ProductImage model)
        {
            data.AlternateText = model.AlternateText;
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Caption = model.Caption;
            data.FileName = model.FileName;
            data.LastUpdated = model.LastUpdatedUtc;
            data.ProductID = DataTypeHelper.BvinToGuid(model.ProductId);
            data.SortOrder = model.SortOrder;
            data.StoreId = model.StoreId;
        }


        public ProductImage Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid && y.StoreId == Context.CurrentStore.Id);
        }

        public ProductImage FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        public override bool Create(ProductImage item)
        {
            if (string.IsNullOrEmpty(item.Bvin))
                item.Bvin = Guid.NewGuid().ToString();
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.SortOrder = FindMaxSort(item.ProductId) + 1;

            return base.Create(item);
        }

        private int FindMaxSort(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            using (var s = CreateStrategy())
            {
                var maxSortOrder = s.GetQuery().Where(y => y.ProductID == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        public bool Update(ProductImage c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }

            c.LastUpdatedUtc = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        public bool Delete(string bvin)
        {
            var result = DeleteAdv(bvin);
            return result.Success;
        }

        public DalBatchOperationResult<ProductImage> DeleteAdv(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            var guid = DataTypeHelper.BvinToGuid(bvin);
            var result = DeleteAdv(y => y.bvin == guid);
            if (result.Success)
            {
                var image = result.OldValues.First();
                DiskStorage.DeleteAdditionalProductImage(storeId, image.ProductId, image.Bvin);
            }
            return result;
        }

        public bool DeleteForProductId(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductID == productGuid);
        }

        public List<ProductImage> FindByProductIds(List<string> productIds)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuids = productIds.Select(id => DataTypeHelper.BvinToGuid(id)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => productGuids.Contains(y.ProductID))
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.ProductID)
                    .ThenBy(y => y.SortOrder);
            });
        }

        public List<ProductImage> FindByProductId(string productId)
        {
            return FindByProductIdPaged(productId, 1, int.MaxValue);
        }

        public List<ProductImage> FindByProductIdPaged(string productId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductID == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        public void MergeList(string productBvin, List<ProductImage> subitems)
        {
            var storeId = Context.CurrentStore.Id;
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.StoreId = storeId;
                item.ProductId = productBvin;
                item.LastUpdatedUtc = DateTime.UtcNow;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            MergeList(subitems, pi => pi.ProductID == productGuid);
        }

        public bool Resort(string productBvin, List<string> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForImage(productBvin, sortedIds[i - 1], i);
                }
            }
            return true;
        }

        private bool UpdateSortOrderForImage(string productBvin, string imageBvin, int newSortOrder)
        {
            var item = Find(imageBvin);
            if (item == null) return false;
            if (item.ProductId != productBvin) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ProductImageRepository InstantiateForMemory(HccRequestContext c)
        {
            return new ProductImageRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ProductImageRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new ProductImageRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ProductImageRepository(HccRequestContext c, IRepositoryStrategy<hcc_ProductImage> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}