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
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Peform database operation on  hcc_Category and hcc_CategoryTranslation table.
    /// </summary>
    public class CategoryRepository : HccLocalizationRepoBase<hcc_Category, hcc_CategoryTranslation, Category, Guid>,
        ICategoryRepository
    {
        #region Constructor

        public CategoryRepository(HccRequestContext c)
            : base(c)
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Create new category.
        /// </summary>
        /// <param name="item"><see cref="Category" /> instance</param>
        /// <returns>Returns true if category created successfully otherwise returns false.</returns>
        public override bool Create(Category item)
        {
            item.CreationDateUtc = DateTime.UtcNow;
            item.LastUpdatedUtc = DateTime.UtcNow;
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.SortOrder = FindMaxSort(item.ParentId) + 1;

            // Make sure we have a rewrite URL if missing
            if (item.RewriteUrl == string.Empty)
            {
                item.RewriteUrl = Text.Slugify(item.Name, true);
            }

            // Try 10000 times to append to URL if in use
            var baseRewriteUrl = item.RewriteUrl;
            var rewriteUrlInUse = UrlRewriter.IsCategorySlugInUse(item.RewriteUrl, item.Bvin, this);
            for (var i = 2; i < 10000; i++)
            {
                if (rewriteUrlInUse)
                {
                    item.RewriteUrl = string.Concat(baseRewriteUrl, "-", i.ToString());
                    rewriteUrlInUse = UrlRewriter.IsCategorySlugInUse(item.RewriteUrl, item.Bvin, this);
                    if (!rewriteUrlInUse)
                        break;
                }
            }
            if (rewriteUrlInUse)
                throw new ApplicationException("Can't generate unique slug for category");

            return base.Create(item);
        }

        /// <summary>
        ///     Update category.
        /// </summary>
        /// <param name="c"><see cref="Category" /> instance</param>
        /// <returns>Returns true if category updated successfully otherwise returns false.</returns>
        public virtual bool Update(Category c)
        {
            var result = UpdateAdv(c);
            return result.Success;
        }

        /// <summary>
        ///     Update category
        /// </summary>
        /// <param name="c"><see cref="Category" /> instance</param>
        /// <returns></returns>
        public virtual DalSingleOperationResult<Category> UpdateAdv(Category c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return new DalSingleOperationResult<Category>();
            }

            c.LastUpdatedUtc = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);

            return UpdateAdv(c, y => y.bvin == guid, false, true);
        }

        /// <summary>
        ///     Delete category.
        /// </summary>
        /// <param name="bvin">Category unique identifier</param>
        /// <returns>Returns true if category removed successfully otherwise returns false.</returns>
        public virtual bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        /// <summary>
        ///     Remove all categories for given store.
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        public void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        /// <summary>
        ///     Search category by unique identifier in current store.
        /// </summary>
        /// <param name="bvin">Category unique identifier</param>
        /// <returns>Returns <see cref="Category" /> instance</returns>
        public Category Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid && y.Item.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>
        ///     Search category on all stores.
        /// </summary>
        /// <param name="bvin">Category unique identifier</param>
        /// <returns>Returns <see cref="Category" /> instance</returns>
        public Category FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid);
        }

        /// <summary>
        ///     Find category by slug
        /// </summary>
        /// <param name="urlSlug">Slug string parameter</param>
        /// <returns>Returns <see cref="Category" /> instance</returns>
        public Category FindBySlug(string urlSlug)
        {
            return FindBySlugForStore(urlSlug, Context.CurrentStore.Id);
        }

        /// <summary>
        ///     Find category by slug in given store
        /// </summary>
        /// <param name="urlSlug">Slug string parameter in URL</param>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>Returns <see cref="Category" /> instance</returns>
        public Category FindBySlugForStore(string urlSlug, long storeId)
        {
            using (var s = CreateReadStrategy())
            {
                var item = GetJoinedQuery(s)
                    .AsNoTracking()
                    .Where(y => y.Item.RewriteUrl == urlSlug && y.Item.StoreId == storeId)
                    .OrderBy(y => y.Item.SortOrder)
                    .FirstOrDefault();

                return FirstPoco(item);
            }
        }

        /// <summary>
        ///     Get list of all categories.
        /// </summary>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindAll()
        {
            using (var strategy = CreateReadStrategy())
            {
                var result = GetSecureQueryForCurrentStore(strategy)
                    .AsNoTracking()
                    .OrderBy(y => y.Item.SortOrder)
                    .ToList();
                return ListPocoSnapshot(result);
            }
        }

        /// <summary>
        ///     Get list of all categories from all stores.
        /// </summary>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindAllForAllStores()
        {
            return FindAllSnapshotsPagedForAllStores(1, int.MaxValue);
        }

        /// <summary>
        ///     Get list of paged category information.
        /// </summary>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindAllSnapshotsPaged(int pageNumber, int pageSize)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = GetSecureQueryForCurrentStore(strategy).AsNoTracking().OrderBy(y => y.Item.SortOrder);
                var items = GetPagedItems(query, pageNumber, pageSize).ToList();

                return ListPocoSnapshot(items);
            }
        }

        /// <summary>
        ///     Get list of paged category information from all stores.
        /// </summary>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindAllSnapshotsPagedForAllStores(int pageNumber, int pageSize)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = GetSecureQuery(strategy).AsNoTracking().OrderBy(y => y.Item.SortOrder);
                var items = GetPagedItems(query, pageNumber, pageSize).ToList();

                return ListPocoSnapshot(items);
            }
        }

        /// <summary>
        ///     Get list of paged category information.
        /// </summary>
        /// <param name="pageNumber">Current page number</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public override List<Category> FindAllPaged(int pageNumber, int pageSize)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = GetSecureQueryForCurrentStore(strategy).AsNoTracking().OrderBy(y => y.Item.SortOrder);
                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        /// <summary>
        ///     Get list of children category for given parent category.
        /// </summary>
        /// <param name="parentId">Categoy unique identifier</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindChildren(string parentId)
        {
            var totalRowCount = 0;
            return FindChildren(parentId, 1, int.MaxValue, ref totalRowCount);
        }

        /// <summary>
        ///     Get list of children category for given parent category.
        /// </summary>
        /// <param name="parentId">Categoy unique identifier</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalRowCount">Total records reference parameter</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindChildren(string parentId, int pageNumber, int pageSize, ref int totalRowCount)
        {
            var parentGuid = DataTypeHelper.BvinToNullableGuid(parentId);
            using (var s = CreateReadStrategy())
            {
                var query = GetSecureQueryForCurrentStore(s, parentId).AsNoTracking().OrderBy(y => y.Item.SortOrder);
                var items = GetPagedItems(query, pageNumber, pageSize);
                return ListPocoSnapshot(items);
            }
        }

        /// <summary>
        ///     Get list of visible categories for given parent category.
        /// </summary>
        /// <param name="parentId">Category unique identifier</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindVisibleChildren(string parentId)
        {
            var total = 0;
            return FindVisibleChildren(parentId, 1, int.MaxValue, ref total);
        }

        /// <summary>
        ///     Get list of visible categories for given parent category.
        /// </summary>
        /// <param name="parentId">Category unique identifier</param>
        /// <param name="pageNumber">Page number</param>
        /// <param name="pageSize">Page Size</param>
        /// <param name="totalRowCount">Total records reference parameter</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindVisibleChildren(string parentId, int pageNumber, int pageSize,
            ref int totalRowCount)
        {
            var parentGuid = DataTypeHelper.BvinToNullableGuid(parentId);
            using (var s = CreateReadStrategy())
            {
                var query = GetSecureQueryForCurrentStore(s, parentId)
                    .AsNoTracking()
                    .Where(y => y.Item.Hidden == 0)
                    .OrderBy(y => y.Item.SortOrder);

                var items = GetPagedItems(query, pageNumber, pageSize);

                return ListPocoSnapshot(items);
            }
        }

        /// <summary>
        ///     Find list of categories for given unique identifier list.
        /// </summary>
        /// <param name="bvins">List of category unique identifier</param>
        /// <returns>Returns list of <see cref="Category" /> instances</returns>
        public List<Category> FindMany(List<string> bvins)
        {
            var guids = bvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            using (var strategy = CreateReadStrategy())
            {
                var items = GetSecureQueryForCurrentStore(strategy)
                    .AsNoTracking()
                    .Where(y => guids.Contains(y.Item.bvin))
                    .OrderBy(y => y.Item.SortOrder);

                return ListPoco(items);
            }
        }

        /// <summary>
        ///     Find list of categories for given unique identifier list.
        /// </summary>
        /// <param name="bvins">List of category unique identifier</param>
        /// <returns>Returns list of <see cref="CategorySnapshot" /> instances</returns>
        public List<CategorySnapshot> FindManySnapshots(List<string> bvins)
        {
            var guids = bvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            using (var s = CreateReadStrategy())
            {
                var items = GetSecureQueryForCurrentStore(s)
                    .AsNoTracking()
                    .Where(y => guids.Contains(y.Item.bvin))
                    .OrderBy(y => y.Item.SortOrder)
                    .ToList();

                return ListPocoSnapshot(items);
            }
        }

        /// <summary>
        ///     Get list of categories for matching name
        /// </summary>
        /// <param name="name">Category name</param>
        /// <returns>Returns list of <see cref="Category" /> instances</returns>
        public List<Category> FindMany(string name)
        {
            using (var strategy = CreateReadStrategy())
            {
                var items = GetSecureQueryForCurrentStore(strategy)
                    .AsNoTracking()
                    .Where(y => y.ItemTranslation.Name.Contains(name))
                    .OrderBy(y => y.Item.SortOrder);

                return ListPoco(items);
            }
        }

        #endregion

        #region Implementation

        protected override Expression<Func<hcc_Category, Guid>> ItemKeyExp
        {
            get { return c => c.bvin; }
        }

        protected override Expression<Func<hcc_CategoryTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return ct => ct.CategoryId; }
        }

        protected override void CopyItemToModel(hcc_Category data, Category model)
        {
            model.BannerImageUrl = data.BannerImageURL;
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.CustomPageOpenInNewWindow = data.CustomPageNewWindow == 1 ? true : false;
            model.CustomPageUrl = data.CustomPageURL;
            model.DisplaySortOrder = (CategorySortOrder) data.DisplaySortOrder;
            model.Hidden = data.Hidden == 1 ? true : false;
            model.ImageUrl = data.ImageURL;
            model.CreationDateUtc = data.CreationDate;
            model.LastUpdatedUtc = data.LastUpdated;
            model.ParentId = DataTypeHelper.GuidToBvin(data.ParentID);
            model.PostContentColumnId = data.PostContentColumnId;
            model.PreContentColumnId = data.PreContentColumnId;
            model.RewriteUrl = data.RewriteUrl;
            model.ShowInTopMenu = data.ShowInTopMenu == 1 ? true : false;
            model.ShowTitle = data.ShowTitle == 1 ? true : false;
            model.SortOrder = data.SortOrder;
            model.SourceType = (CategorySourceType) data.SourceType;
            model.StoreId = data.StoreId;
            model.TemplateName = data.TemplateName;
        }

        protected override void CopyTransToModel(hcc_CategoryTranslation data, Category model)
        {
            model.Name = data.Name;
            model.Description = data.Description;
            model.Keywords = data.Keywords;
            model.MetaDescription = data.MetaDescription;
            model.MetaKeywords = data.MetaKeywords;
            model.MetaTitle = data.MetaTitle;
        }

        /// <summary>
        ///     Copies the model to item.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <param name="model">The model.</param>
        protected override void CopyModelToItem(JoinedItem<hcc_Category, hcc_CategoryTranslation> data, Category model)
        {
            data.Item.BannerImageURL = model.BannerImageUrl;
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.CustomPageNewWindow = model.CustomPageOpenInNewWindow ? 1 : 0;
            data.Item.CustomPageURL = model.CustomPageUrl;

            data.Item.DisplaySortOrder = (int) model.DisplaySortOrder;
            data.Item.Hidden = model.Hidden ? 1 : 0;
            data.Item.ImageURL = model.ImageUrl;
            data.Item.CreationDate = model.CreationDateUtc;
            data.Item.LastUpdated = model.LastUpdatedUtc;
            data.Item.ParentID = DataTypeHelper.BvinToNullableGuid(model.ParentId);
            data.Item.PostContentColumnId = model.PostContentColumnId;
            data.Item.PreContentColumnId = model.PreContentColumnId;
            data.Item.RewriteUrl = model.RewriteUrl;
            data.Item.ShowInTopMenu = model.ShowInTopMenu ? 1 : 0;
            data.Item.ShowTitle = model.ShowTitle ? 1 : 0;
            data.Item.SortOrder = model.SortOrder;
            data.Item.SourceType = (int) model.SourceType;
            data.Item.StoreId = model.StoreId;
            data.Item.TemplateName = model.TemplateName;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_Category, hcc_CategoryTranslation> data, Category model)
        {
            data.ItemTranslation.CategoryId = data.Item.bvin;

            data.ItemTranslation.Name = model.Name;
            data.ItemTranslation.Description = model.Description;
            data.ItemTranslation.Keywords = model.Keywords;
            data.ItemTranslation.MetaDescription = model.MetaDescription;
            data.ItemTranslation.MetaKeywords = model.MetaKeywords;
            data.ItemTranslation.MetaTitle = model.MetaTitle;
        }

        protected virtual List<CategorySnapshot> ListPocoSnapshot(
            IEnumerable<JoinedItem<hcc_Category, hcc_CategoryTranslation>> items)
        {
            var result = new List<CategorySnapshot>();

            if (items != null)
            {
                foreach (var item in items)
                {
                    var temp = new CategorySnapshot();
                    temp.Bvin = DataTypeHelper.GuidToBvin(item.Item.bvin);
                    temp.CustomPageOpenInNewWindow = item.Item.CustomPageNewWindow == 1;
                    temp.CustomPageUrl = item.Item.CustomPageURL;
                    temp.Hidden = item.Item.Hidden == 1;
                    temp.ImageUrl = item.Item.ImageURL;
                    temp.BannerImageUrl = item.Item.BannerImageURL;
                    temp.ParentId = DataTypeHelper.GuidToBvin(item.Item.ParentID);
                    temp.RewriteUrl = item.Item.RewriteUrl;
                    temp.ShowInTopMenu = item.Item.ShowInTopMenu == 1;
                    temp.SourceType = (CategorySourceType) item.Item.SourceType;
                    temp.StoreId = item.Item.StoreId;
                    temp.SortOrder = item.Item.SortOrder;

                    if (item.ItemTranslation != null)
                    {
                        temp.Name = item.ItemTranslation.Name;
                        temp.Description = item.ItemTranslation.Description;
                        temp.MetaTitle = item.ItemTranslation.MetaTitle;
                    }

                    result.Add(temp);
                }
            }

            return result;
        }

        public int FindMaxSort(string parentId)
        {
            var storeId = Context.CurrentStore.Id;
            var parentGuid = DataTypeHelper.BvinToNullableGuid(parentId);
            using (var s = CreateReadStrategy())
            {
                var maxSortOrder = GetJoinedQuery(s)
                    .AsNoTracking()
                    .Where(y => y.Item.ParentID == parentGuid || !(parentGuid.HasValue || y.Item.ParentID.HasValue))
                    .Where(y => y.Item.StoreId == storeId)
                    .Max(y => (int?) y.Item.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        internal List<string> FindAllBvinsForStore(long storeId)
        {
            using (var s = CreateReadStrategy())
            {
                return GetSecureQueryForCurrentStore(s)
                    .AsNoTracking()
                    .Select(y => DataTypeHelper.GuidToBvin(y.Item.bvin))
                    .ToList();
            }
        }

        protected virtual IQueryable<JoinedItem<hcc_Category, hcc_CategoryTranslation>> GetSecureQuery(
            IRepoStrategy<hcc_Category> strategy)
        {
            return GetJoinedQuery(strategy);
        }

        protected virtual IQueryable<JoinedItem<hcc_Category, hcc_CategoryTranslation>> GetSecureQueryForCurrentStore(
            IRepoStrategy<hcc_Category> strategy)
        {
            return GetSecureQuery(strategy).Where(i => i.Item.StoreId == Context.CurrentStore.Id);
        }

        protected IQueryable<JoinedItem<hcc_Category, hcc_CategoryTranslation>> GetSecureQueryForCurrentStore(
            IRepoStrategy<hcc_Category> strategy, string parentId)
        {
            var parentGuid = DataTypeHelper.BvinToNullableGuid(parentId);
            return
                GetSecureQueryForCurrentStore(strategy)
                    .Where(i => i.Item.ParentID == parentGuid || (!i.Item.ParentID.HasValue && !parentGuid.HasValue));
        }

        #endregion
    }
}