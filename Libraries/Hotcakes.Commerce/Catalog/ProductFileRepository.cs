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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Storage;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Perform database operation on hcc_ProductFile table
    /// </summary>
    public class ProductFileRepository : HccSimpleRepoBase<hcc_ProductFile, ProductFile>
    {
        private readonly ProductFileAssociationRepository crosses;

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public ProductFileRepository(HccRequestContext c)
            : base(c)
        {
            crosses = new ProductFileAssociationRepository(c);
        }


        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override void CopyDataToModel(hcc_ProductFile data, ProductFile model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.FileName = data.FileName;
            model.LastUpdated = data.LastUpdated;
            model.ShortDescription = data.ShortDescription;
            model.StoreId = data.StoreId;

            if (data.hcc_ProductFileXProduct.FirstOrDefault() != null)
            {
                model.MaxDownloads = data.hcc_ProductFileXProduct.FirstOrDefault()?.MaxDownloads ?? 0;
            }

            if (data.hcc_ProductFileXProduct.FirstOrDefault() != null) 
            {
                model.AvailableMinutes = data.hcc_ProductFileXProduct?.FirstOrDefault()?.AvailableMinutes ?? 0;
            }
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        protected override void CopyModelToData(hcc_ProductFile data, ProductFile model)
        {
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.FileName = model.FileName;
            data.LastUpdated = model.LastUpdated;
            data.ShortDescription = model.ShortDescription;
            data.StoreId = model.StoreId;
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public ProductFile Find(string bvin)
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

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public ProductFile FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public override bool Create(ProductFile item)
        {
            item.LastUpdated = DateTime.UtcNow;
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public bool Update(ProductFile c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        protected override void MergeSubItems(ProductFile model)
        {
            if (!string.IsNullOrEmpty(model.ProductId))
            {
                var assoc = crosses.FindByFileIdAndProductId(model.Bvin, model.ProductId);
                if (assoc == null)
                {
                    assoc = new ProductFileAssociation();
                    assoc.StoreId = model.StoreId;
                    assoc.FileId = model.Bvin;
                    assoc.ProductId = model.ProductId;
                }
                assoc.LastUpdatedUtc = DateTime.UtcNow;
                assoc.MaxDownloads = model.MaxDownloads;
                assoc.AvailableMinutes = model.AvailableMinutes;
                if (assoc.Id <= 0)
                    crosses.Create(assoc);
                else
                    crosses.Update(assoc);
            }
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            var item = FindForAllStores(bvin);
            if (item == null) return false;

            // remove from products
            crosses.DeleteForFileId(item.Bvin, storeId);

            var diskFileName = item.Bvin + "_" + item.FileName + ".config";
            DiskStorage.FileVaultRemove(item.StoreId, diskFileName);

            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid && y.StoreId == storeId);
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool DeleteForProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForProductId(productId, storeId);
        }

        internal bool DeleteForProductId(string productId, long storeId)
        {
            var toDelete = FindByProductId(productId, storeId);
            foreach (var item in toDelete)
            {
                DeleteForStore(item.Bvin, storeId);
            }
            return true;
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<ProductFile> FindByProductId(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindByProductId(productId, storeId);
        }

        internal List<ProductFile> FindByProductId(string productId, long storeId)
        {
            var result = new List<ProductFile>();

            var linkedFiles = crosses.FindByProductIdForStore(productId, storeId);
            foreach (var x in linkedFiles)
            {
                var f = FindForAllStores(x.FileId);
                if (f != null)
                {
                    f.ProductId = x.ProductId;
                    f.AvailableMinutes = x.AvailableMinutes;
                    f.MaxDownloads = x.MaxDownloads;
                    result.Add(f);
                }
            }

            return result;
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<ProductFile> FindByFileNameAndDescription(string fileName, string description)
        {
            var result = new List<ProductFile>();
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.FileName == fileName)
                    .Where(y => y.ShortDescription == description);
            });
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public ProductFile FindByBvinAndProductBvin(string bvin, string productBvin)
        {
            var x = crosses.FindByFileIdAndProductId(bvin, productBvin);
            if (x != null)
            {
                var f = Find(x.FileId);
                f.ProductId = x.ProductId;
                f.AvailableMinutes = x.AvailableMinutes;
                f.MaxDownloads = x.MaxDownloads;
                return f;
            }
            return null;
        }

        /// <summary>
        /// Defines the catalog data or behavior represented by this member.
        /// </summary>
        public bool FileAlreadyExists(string fileBvin)
        {
            var f = Find(fileBvin);
            if (f != null) return true;
            return false;
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public int CountOfProductsUsingFile(string fileId)
        {
            var result = 0;
            result = crosses.CountByFileId(fileId);
            return result;
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<string> FindProductIdsForFile(string fileId)
        {
            var ids = new List<string>();
            var data = crosses.FindByFileId(fileId);
            foreach (var x in data)
            {
                ids.Add(x.ProductId);
            }
            return ids;
        }

        /// <summary>
        /// Deletes the matching catalog records and applies the repository scope shown by the implementation.
        /// </summary>
        public bool RemoveAssociatedProduct(string fileBvin, string productBvin)
        {
            var x = crosses.FindByFileIdAndProductId(fileBvin, productBvin);
            if (x == null) return false;
            return crosses.Delete(x.Id);
        }

        /// <summary>
        /// Changes catalog data according to the repository and persistence behavior implemented by this member.
        /// </summary>
        public bool AddAssociatedProduct(string fileBvin, string productBvin, int availableMinutes, int maxDownloads)
        {
            var storeId = Context.CurrentStore.Id;
            RemoveAssociatedProduct(fileBvin, productBvin);
            var x = new ProductFileAssociation();
            x.AvailableMinutes = availableMinutes;
            x.FileId = fileBvin;
            x.MaxDownloads = maxDownloads;
            x.ProductId = productBvin;
            x.StoreId = storeId;
            return crosses.Create(x);
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public int FindAllCount()
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateReadStrategy())
            {
                return s.GetQuery().AsNoTracking().Where(y => y.StoreId == storeId)
                    .Count();
            }
        }

        /// <summary>
        /// Retrieves catalog data using the criteria represented by this member.
        /// </summary>
        public List<ProductFile> FindAll(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.FileName);
            }, pageNumber, pageSize);
        }
    }
}
