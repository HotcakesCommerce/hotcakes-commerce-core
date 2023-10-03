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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Perform database operation on hcc_ProductFileXProduct table
    /// </summary>
    public class ProductFileAssociationRepository : HccSimpleRepoBase<hcc_ProductFileXProduct, ProductFileAssociation>
    {
        public ProductFileAssociationRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        ///     Copy database object model to view model
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductFileXProduct" /> instance</param>
        /// <param name="model"><see cref="ProductFileAssociation" /> instance</param>
        protected override void CopyDataToModel(hcc_ProductFileXProduct data, ProductFileAssociation model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.AvailableMinutes = data.AvailableMinutes;
            model.FileId = DataTypeHelper.GuidToBvin(data.ProductFileId);
            model.LastUpdatedUtc = data.LastUpdated;
            model.MaxDownloads = data.MaxDownloads;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
        }

        /// <summary>
        ///     Copy view model to the database object model
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductFileXProduct" /> instance</param>
        /// <param name="model"><see cref="ProductFileAssociation" /> instance</param>
        protected override void CopyModelToData(hcc_ProductFileXProduct data, ProductFileAssociation model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.AvailableMinutes = model.AvailableMinutes;
            data.ProductFileId = DataTypeHelper.BvinToGuid(model.FileId);
            data.LastUpdated = model.LastUpdatedUtc;
            data.MaxDownloads = model.MaxDownloads;
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
        }

        /// <summary>
        ///     Find ProductFileAssociation by id in current store
        /// </summary>
        /// <param name="id">ProductFileAssociation unique identifier</param>
        /// <returns><see cref="ProductFileAssociation" /> instance</returns>
        public ProductFileAssociation Find(long id)
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
        ///     Find ProductFileAssociation by id in all store
        /// </summary>
        /// <param name="id">ProductFileAssociation unique identifier</param>
        /// <returns><see cref="ProductFileAssociation" /> instance</returns>
        public ProductFileAssociation FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>
        ///     Create new ProductFileAssociation
        /// </summary>
        /// <param name="item"><see cref="ProductFileAssociation" /> instance</param>
        /// <returns>Returns true if new record created successfully</returns>
        public override bool Create(ProductFileAssociation item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        ///     Update ProductFileAssociation
        /// </summary>
        /// <param name="c"><see cref="ProductFileAssociation" /> instance</param>
        /// <returns>Returns true if the record updated successfully</returns>
        public bool Update(ProductFileAssociation c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdatedUtc = DateTime.UtcNow;
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>
        ///     Delete ProductFileAssociation
        /// </summary>
        /// <param name="id">ProductFileAssociation unique identifier</param>
        /// <returns>Returns true if the record deleted successfully</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>
        ///     Delete ProductFileAssociation for given store
        /// </summary>
        /// <param name="id">ProductFileAssociation unique identifier</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns true if the record deleted successfully</returns>
        public bool Delete(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        /// <summary>
        ///     Delete all files association for given product
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <returns>Returns true if the record deleted successfully</returns>
        public bool DeleteForProductId(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductId == productGuid);
        }

        /// <summary>
        ///     Delete Product file
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <returns>Returns true if the File has been removed successfully.</returns>
        public bool DeleteForFileId(string fileId)
        {
            var fileGuid = DataTypeHelper.BvinToGuid(fileId);
            return Delete(y => y.ProductFileId == fileGuid);
        }

        /// <summary>
        ///     Delete Product file for given store
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns true if file removed successfully</returns>
        public bool DeleteForFileId(string fileId, long storeId)
        {
            var fileGuid = DataTypeHelper.BvinToGuid(fileId);
            return Delete(y => y.ProductFileId == fileGuid && y.StoreId == storeId);
        }

        /// <summary>
        ///     Find all files associated with given product id
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <returns>Returns list of <see cref="ProductFileAssociation" /> instances</returns>
        public List<ProductFileAssociation> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductIdPaged(productId, 1, int.MaxValue, storeId);
        }

        /// <summary>
        ///     Find all files associated with given product id for given store
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns list of <see cref="ProductFileAssociation" /> instances</returns>
        public List<ProductFileAssociation> FindByProductIdForStore(string productId, long storeId)
        {
            return FindByProductIdPaged(productId, 1, int.MaxValue, storeId);
        }

        /// <summary>
        ///     Find all files associated with given product id for given store with paging
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns list of <see cref="ProductFileAssociation" /> instances</returns>
        public List<ProductFileAssociation> FindByProductIdPaged(string productId, int pageNumber, int pageSize,
            long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Id);
            }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Find all product file association related to given file
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <returns>Returns list of <see cref="ProductFileAssociation" /> instances</returns>
        public List<ProductFileAssociation> FindByFileId(string fileId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByFileIdPaged(fileId, 1, int.MaxValue, storeId);
        }

        /// <summary>
        ///     Find all product file association related to given file within given store
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns list of <see cref="ProductFileAssociation" /> instances</returns>
        public List<ProductFileAssociation> FindByFileId(string fileId, long storeId)
        {
            return FindByFileIdPaged(fileId, 1, int.MaxValue, storeId);
        }

        /// <summary>
        ///     Find all product file association related to given file within given store with paging
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <param name="pageNumber">Page Number</param>
        /// <param name="pageSize">Page size</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns list of <see cref="ProductFileAssociation" /> instances</returns>
        public List<ProductFileAssociation> FindByFileIdPaged(string fileId, int pageNumber, int pageSize, long storeId)
        {
            var fileGuid = DataTypeHelper.BvinToGuid(fileId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductFileId == fileGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Id);
            }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Find count of products with which this file associated.
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <returns>Returns number of association for this file</returns>
        public int CountByFileId(string fileId)
        {
            var result = 0;
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                var fileGuid = DataTypeHelper.BvinToGuid(fileId);
                var temp = s.GetQuery().AsNoTracking().Where(y => y.ProductFileId == fileGuid)
                    .Where(y => y.StoreId == storeId).Count();

                if (temp >= 0)
                {
                    result = temp;
                }
                return result;
            }
        }

        /// <summary>
        ///     Find Product file association for given product and file
        /// </summary>
        /// <param name="fileId">File unique identifier</param>
        /// <param name="productId">Product unique identifier</param>
        /// <returns>Returns instance of the <see cref="ProductFileAssociation" /></returns>
        public ProductFileAssociation FindByFileIdAndProductId(string fileId, string productId)
        {
            var fileGuid = DataTypeHelper.BvinToGuid(fileId);
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindFirstPoco(y => y.ProductFileId == fileGuid && y.ProductId == productGuid);
        }
    }
}