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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Perform database operation on hcc_ProductXCategory table.
    /// </summary>
    public class CategoryProductAssociationRepository :
        HccSimpleRepoBase<hcc_ProductXCategory, CategoryProductAssociation>
    {
        public CategoryProductAssociationRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        ///     Copy database object class to View Model
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductXCategory" /> instance</param>
        /// <param name="model"><see cref="CategoryProductAssociation" /> instance</param>
        protected override void CopyDataToModel(hcc_ProductXCategory data, CategoryProductAssociation model)
        {
            model.Id = data.Id;
            model.CategoryId = DataTypeHelper.GuidToBvin(data.CategoryId);
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
            model.SortOrder = data.SortOrder;
            model.StoreId = data.StoreId;
        }

        /// <summary>
        ///     Copy View Model to database object model.
        /// </summary>
        /// <param name="data"><see cref="hcc_ProductXCategory" /> instance</param>
        /// <param name="model"><see cref="CategoryProductAssociation" /> instance</param>
        protected override void CopyModelToData(hcc_ProductXCategory data, CategoryProductAssociation model)
        {
            data.Id = model.Id;
            data.CategoryId = DataTypeHelper.BvinToGuid(model.CategoryId);
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
            data.SortOrder = model.SortOrder;
            data.StoreId = model.StoreId;
        }

        /// <summary>
        ///     Find CategoryProductAssociation by id.
        /// </summary>
        /// <param name="id">ProductCategoryAssociation unique identifier</param>
        /// <returns>Returns <see cref="CategoryProductAssociation" /> instance for given id</returns>
        public CategoryProductAssociation Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>
        ///     Find CategoryProductAssociation.
        /// </summary>
        /// <param name="categoryId">Unique category identifier</param>
        /// <param name="productId">Unique product identifier</param>
        /// <returns>Returns <see cref="CategoryProductAssociation" /> instance for given parameters</returns>
        public CategoryProductAssociation FindByCategoryAndProduct(string categoryId, string productId)
        {
            return FindByCategoryAndProduct(categoryId, productId, Context.CurrentStore.Id);
        }

        /// <summary>
        ///     Find CategoryProductAssociation.
        /// </summary>
        /// <param name="categoryId">Unique category identifier</param>
        /// <param name="productId">Unique category identifier</param>
        /// <param name="storeId">Unique storeid</param>
        /// <returns>Returns <see cref="CategoryProductAssociation" /> instance for given parameters</returns>
        public CategoryProductAssociation FindByCategoryAndProduct(string categoryId, string productId, long storeId)
        {
            var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            using (var s = CreateReadStrategy())
            {
                var item = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.CategoryId == categoryGuid)
                    .Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder)
                    .FirstOrDefault();

                return FirstPoco(item);
            }
        }

        /// <summary>
        ///     Create new CategoryProductAssociation
        /// </summary>
        /// <param name="item"><see cref="CategoryProductAssociation" /> instance</param>
        /// <returns>Returns true if new record created successfully otherwise returns false.</returns>
        public override bool Create(CategoryProductAssociation item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.SortOrder = FindMaxSort(item.CategoryId) + 1;
            return base.Create(item);
        }

        /// <summary>
        ///     Find maximum sort order for the given category
        /// </summary>
        /// <param name="categoryId">Category unique identifier</param>
        /// <returns>Returns max sort number</returns>
        private int FindMaxSort(string categoryId)
        {
            var storeId = Context.CurrentStore.Id;
            var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
            using (var s = CreateReadStrategy())
            {
                var maxSortOrder = s.GetQuery().AsNoTracking().Where(y => y.CategoryId == categoryGuid)
                    .Where(y => y.StoreId == storeId)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        /// <summary>
        ///     Update CategoryProductAssociation.
        /// </summary>
        /// <param name="c"><see cref="CategoryProductAssociation" /> instance</param>
        /// <returns>Returns true if new record updated successfully otherwise returns false.</returns>
        public bool Update(CategoryProductAssociation c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>
        ///     Find all CategoryProductAssociation.
        /// </summary>
        /// <returns>Returns list of <see cref="CategoryProductAssociation" /></returns>
        public List<CategoryProductAssociation> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderBy(y => y.SortOrder); });
        }

        /// <summary>
        ///     Get all CategoryProductAssociation for current store.
        /// </summary>
        /// <returns>Returns list of <see cref="CategoryProductAssociation" /> instances</returns>
        public List<CategoryProductAssociation> FindAllForAllStores()
        {
            return FindAllPagedForAllStores(1, int.MaxValue);
        }

        /// <summary>
        ///     Get list of CategoryProductAssociation with paging for current stored
        /// </summary>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>Returns list of <see cref="CategoryProductAssociation" /> instances</returns>
        public new List<CategoryProductAssociation> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderBy(y => y.SortOrder); },
                pageNumber, pageSize);
        }

        /// <summary>
        ///     Get list of CategoryProductAssociation with paging from all stores
        /// </summary>
        /// <param name="pageNumber">Current PageNumber</param>
        /// <param name="pageSize">Current PageSize</param>
        /// <returns>Returns list of <see cref="CategoryProductAssociation" /> instances</returns>
        public List<CategoryProductAssociation> FindAllPagedForAllStores(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.OrderBy(y => y.SortOrder); }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Find list of CategoryProductAssociation for a given category.
        /// </summary>
        /// <param name="categoryId">Category unique identifier</param>
        /// <param name="pageNumber">Current PageNumber</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>Returns list of <see cref="CategoryProductAssociation" /> instances</returns>
        public List<CategoryProductAssociation> FindForCategory(string categoryId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.CategoryId == categoryGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Find list of CategoryProductAssociation for a given product.
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="pageNumber">Current PageNumber</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>Returns list of <see cref="CategoryProductAssociation" /> instances</returns>
        public List<CategoryProductAssociation> FindForProduct(string productId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        /// <summary>
        ///     Delete all CategoryProductAssociation for given category.
        /// </summary>
        /// <param name="categoryId">Category unique identifier</param>
        /// <returns>Returns true if records removed successfully otherwise returns false.</returns>
        public bool DeleteAllForCategory(string categoryId)
        {
            var storeId = Context.CurrentStore.Id;
            var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
            return Delete(y => y.CategoryId == categoryGuid && y.StoreId == storeId);
        }

        /// <summary>
        ///     Delete all CategoryProductAssociation for given product.
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <returns>Returns true if records removed successfully otherwise returns false.</returns>
        public bool DeleteAllForProduct(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteAllForProductForStore(productId, storeId);
        }

        /// <summary>
        ///     Delete all CategoryProductAssociation for given product and store.
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns true if records removed successfully otherwise returns false.</returns>
        public bool DeleteAllForProductForStore(string productId, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductId == productGuid && y.StoreId == storeId);
        }

        public CategoryProductAssociation AddProductToCategory(string productId, string categoryId)
        {
            var storeId = Context.CurrentStore.Id;
            var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
            var productGuid = DataTypeHelper.BvinToGuid(productId);

            using (var s = CreateStrategy())
            {
                var exists = s.GetQuery()
                    .Where(y => y.CategoryId == categoryGuid)
                    .Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .SingleOrDefault();

                if (exists == null)
                {
                    var x = new CategoryProductAssociation();
                    x.ProductId = productId;
                    x.CategoryId = categoryId;
                    Create(x);
                    return x;
                }
            }

            return null;
        }

        /// <summary>
        ///     Remove product category association for given product and category.
        /// </summary>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="categoryId">Category unique identifier</param>
        /// <returns>Returns true if the removed successfully otherwise returns false.</returns>
        public bool RemoveProductFromCategory(string productId, string categoryId)
        {
            var storeId = Context.CurrentStore.Id;
            var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.CategoryId == categoryGuid && y.ProductId == productGuid && y.StoreId == storeId);
        }

        /// <summary>
        ///     Update sort order of the CategoryProductAssociation.
        /// </summary>
        /// <param name="categoryId">Category unique identifier</param>
        /// <param name="productId">Product unique identifier</param>
        /// <param name="newSortOrder">Sort order needs to be set for given criteria</param>
        /// <returns>Returns true if new order set successfully otherwise returns false.</returns>
        private bool UpdateSortOrderForProduct(string categoryId, string productId, int newSortOrder)
        {
            var c1 = FindByCategoryAndProduct(categoryId, productId);
            if (c1 == null) return false;
            c1.SortOrder = newSortOrder;
            return Update(c1);
        }

        //private bool SwapOrder(string categoryId, string currentId, int currentSort, string targetId, int targetSort)
        //{
        //    bool result = false;

        //    // Update Target
        //    result = UpdateSortOrderForProduct(categoryId, targetId, currentSort);

        //    // Update Current
        //    result = UpdateSortOrderForProduct(categoryId, currentId, targetSort);

        //    return result;
        //}

        //public bool MoveProductUpInCategory(string categoryId, string productId)
        //{
        //    bool result = false;

        //    Collection<Catalog.CategoryProductAssociation> peers = new Collection<Catalog.CategoryProductAssociation>();
        //    peers = Datalayer.CategoryProductAssociationMapper.FindByCategory(categoryId);

        //    if (peers != null)
        //    {

        //        int currentSort = 0;
        //        string currentId = productId;
        //        int targetSort = 0;
        //        string targetId = string.Empty;
        //        bool foundTarget = false;

        //        // Find current and Target Information
        //        for (int i = 0; i <= peers.Count - 1; i++)
        //        {
        //            if (peers[i].ProductId == productId)
        //            {
        //                foundTarget = true;
        //                currentSort = peers[i].SortOrder;
        //            }
        //            else
        //            {
        //                if (foundTarget == false)
        //                {
        //                    targetSort = peers[i].SortOrder;
        //                    targetId = peers[i].ProductId;
        //                }
        //            }
        //        }

        //        // Swap Sort Order
        //        if (foundTarget == true)
        //        {
        //            SwapOrder(categoryId, currentId, currentSort, targetId, targetSort);
        //        }

        //    }

        //    peers = null;

        //    return result;
        //}
        //public bool MoveProductDownInCategory(string categoryId, string productId)
        //{
        //    bool result = false;

        //    Collection<Catalog.CategoryProductAssociation> peers = new Collection<Catalog.CategoryProductAssociation>();
        //    peers = Datalayer.CategoryProductAssociationMapper.FindByCategory(categoryId);

        //    if (peers != null)
        //    {

        //        int currentSort = 0;
        //        string currentId = productId;
        //        int targetSort = 0;
        //        string targetId = string.Empty;
        //        bool foundCurrent = false;
        //        bool foundTarget = false;

        //        // Find current and Target Information
        //        for (int i = 0; i <= peers.Count - 1; i++)
        //        {
        //            if (foundCurrent == true)
        //            {
        //                targetId = peers[i].ProductId;
        //                targetSort = peers[i].SortOrder;
        //                foundCurrent = false;
        //                foundTarget = true;
        //            }
        //            if (peers[i].ProductId == productId)
        //            {
        //                foundCurrent = true;
        //                currentSort = peers[i].SortOrder;
        //            }
        //        }

        //        // Swap Sort Order
        //        if (foundTarget == true)
        //        {
        //            SwapOrder(categoryId, currentId, currentSort, targetId, targetSort);
        //        }

        //    }

        //    peers = null;

        //    return result;
        //}

        /// <summary>
        ///     Update soring order of all the products for given category
        /// </summary>
        /// <param name="categoryId">Category unique identifier</param>
        /// <param name="sortedIds">List of sort order id</param>
        /// <returns>Returns true if order set successfully otherwise returns false.</returns>
        public bool ResortProducts(string categoryId, List<string> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForProduct(categoryId, sortedIds[i - 1], i);
                }
            }
            return true;
        }

        /// <summary>
        ///     Remove all CategoryProductAssociation for given store id.
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}