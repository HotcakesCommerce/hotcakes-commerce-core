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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Peform operation on the hcc_CategoryXProperty table.
    /// </summary>
    public class CategoryFacetManager : HccSimpleRepoBase<hcc_CategoryXProperty, CategoryFacet>
    {
        /// <summary>
        ///     ProductProperty repository to perform operation on hcc_ProductPropertyValue table.
        /// </summary>
        private readonly ProductPropertyValueRepository productValueRepository;

        public CategoryFacetManager(HccRequestContext c)
            : base(c)
        {
            productValueRepository = new ProductPropertyValueRepository(c);
        }

        /// <summary>
        ///     Convert the database table row to the model.
        /// </summary>
        /// <param name="data">hcc_CategoryXProperty table row data</param>
        /// <param name="model"><see cref="CategoryFacet" /> instance to be used on system</param>
        protected override void CopyDataToModel(hcc_CategoryXProperty data, CategoryFacet model)
        {
            model.CategoryId = data.CategoryId;
            model.DisplayMode = (CategoryFacetDisplayMode) data.DisplayMode;
            model.FilterName = data.FilterName;
            model.Id = data.Id;
            model.ParentPropertyId = data.ParentPropertyId;
            model.PropertyId = data.PropertyId;
            model.SortOrder = data.SortOrder;
            model.StoreId = data.StoreId;
        }

        /// <summary>
        ///     Convert the model data as hcc_CategoryXProperty table row data.
        /// </summary>
        /// <param name="data">hcc_CategoryXProperty table row data</param>
        /// <param name="model"><see cref="CategoryFacet" /> instance to be used on system</param>
        protected override void CopyModelToData(hcc_CategoryXProperty data, CategoryFacet model)
        {
            data.CategoryId = model.CategoryId;
            data.DisplayMode = (int) model.DisplayMode;
            data.FilterName = model.FilterName;
            data.Id = model.Id;
            data.ParentPropertyId = model.ParentPropertyId;
            data.PropertyId = model.PropertyId;
            data.SortOrder = model.SortOrder;
            data.StoreId = model.StoreId;
        }

        /// <summary>
        ///     Create new <see cref="CategoryFacet" />.
        /// </summary>
        /// <param name="item"><see cref="CategoryFacet" /> instance</param>
        /// <returns></returns>
        public override bool Create(CategoryFacet item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        ///     Update <see cref="CategoryFacet" />.
        /// </summary>
        /// <param name="c"><see cref="CategoryFacet" /> instance</param>
        /// <returns></returns>
        public bool Update(CategoryFacet c)
        {
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>
        ///     Delete <see cref="CategoryFacet" /> by its id.
        /// </summary>
        /// <param name="id">Unique identifier of the Facet</param>
        /// <returns>Return true if its removed successfully otherwise returns false</returns>
        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        /// <summary>
        ///     Find the <see cref="CategoryFacet" /> by id in current store.
        /// </summary>
        /// <param name="id">Unique identifier of the Facet</param>
        /// <returns>Returns CategoryFacet instance if find by given id</returns>
        public CategoryFacet Find(long id)
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
        ///     Find the <see cref="CategoryFacet" /> by id in all stores.
        /// </summary>
        /// <param name="id">Unique identifier of the Facet</param>
        /// <returns>Returns CategoryFacet instance if find by given id</returns>
        public CategoryFacet FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>
        ///     Get list of all <see cref="CategoryFacet" /> for matching parent id from given list.
        /// </summary>
        /// <param name="all">List of <see cref="CategoryFacet" /> instances</param>
        /// <param name="parentId">Parent facet unique identifier</param>
        /// <returns>Returns list of <see cref="CategoryFacet" /> instances for given criteria</returns>
        public List<CategoryFacet> FindByParentInList(List<CategoryFacet> all, long parentId)
        {
            var result = new List<CategoryFacet>();

            var x = (from n in all
                where n.ParentPropertyId == parentId
                orderby n.SortOrder
                select n).ToList();
            if (x != null)
            {
                result = x;
            }

            return result;
        }


        /// <summary>
        ///     Get list of all <see cref="CategoryFacet" /> for matching parent id from table.
        /// </summary>
        /// <param name="parentId">Parent facet unique identifier</param>
        /// <returns>Returns list of <see cref="CategoryFacet" /> instances for given criteria</returns>
        public List<CategoryFacet> FindByParent(long parentId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.ParentPropertyId == parentId)
                    .OrderBy(y => y.SortOrder);
            });
        }

        /// <summary>
        ///     Get list of all <see cref="CategoryFacet" /> for given category.
        /// </summary>
        /// <param name="categoryBvin">Category unique identifier</param>
        /// <returns>Returns list of <see cref="CategoryFacet" /> instances for given criteria</returns>
        public List<CategoryFacet> FindByCategory(string categoryBvin)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.CategoryId == categoryBvin)
                    .OrderBy(y => y.SortOrder);
            });
        }

        /// <summary>
        ///     Get maximum sort order for the <see cref="CategoryFacet" /> for given Category and parent
        ///     <see cref="CategoryFacet" />.
        /// </summary>
        /// <param name="categoryBvin">Category unique identifier</param>
        /// <param name="parentPropertyId">Parent <see cref="CategoryFacet" /> identifier</param>
        /// <returns>Maximum sort order for matching criteria</returns>
        public int FindMaxSortForCategoryParent(string categoryBvin, long parentPropertyId)
        {
            var result = 0;

            try
            {
                using (var s = CreateReadStrategy())
                {
                    result = s.GetQuery().AsNoTracking().Where(y => y.StoreId == Context.CurrentStore.Id)
                        .Where(y => y.CategoryId == categoryBvin)
                        .Where(y => y.ParentPropertyId == parentPropertyId)
                        .Max(y => y.SortOrder);
                }
                if (result < 0) result = 0;
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
            return result;
        }

        /// <summary>
        ///     Get list of <see cref="ProductFacetCount" /> count group by the key
        /// </summary>
        /// <param name="key">Key by which needs to found facets</param>
        /// <param name="allFacets">List of <see cref="CategoryFacet" /> which needs to be searched for</param>
        /// <param name="properties">List of <see cref="ProductProperty" /> to get the id and generate the new key</param>
        /// <returns></returns>
        public List<ProductFacetCount> FindCountsOfVisibleFacets(string key,
            List<CategoryFacet> allFacets,
            List<ProductProperty> properties)
        {
            var visibleFacets = FindVisibleFacetsIdsForKey(key, allFacets);
            var sqlKeys = new List<string>();
            for (var i = 0; i < allFacets.Count; i++)
            {
                if (visibleFacets.Contains(allFacets[i].Id))
                {
                    if (!IsFacetSelectedInKey(key, allFacets, allFacets[i].Id))
                    {
                        // It's a visible facet, not selected 
                        // so generate all possible SQL keys for choices

                        var p = (from pr in properties
                            where pr.Id == allFacets[i].PropertyId
                            select pr).SingleOrDefault();
                        if (p != null)
                        {
                            foreach (var c in p.Choices)
                            {
                                var updatedKey = CategoryFacetKeyHelper.ReplaceKeyValue(key, i, c.Id);
                                sqlKeys.Add(CategoryFacetKeyHelper.ParseKeyToSqlList(updatedKey));
                            }
                        }
                    }
                }
            }

            return FindProductCountsForKeys(sqlKeys);
        }

        /// <summary>
        ///     Get list of products for given keys.
        /// </summary>
        /// <param name="sqlKeys">List of Keys</param>
        /// <returns>Return list of <see cref="ProductFacetCount" /> for count information for each key</returns>
        public List<ProductFacetCount> FindProductCountsForKeys(List<string> sqlKeys)
        {
            var result = new List<ProductFacetCount>();
            foreach (var key in sqlKeys)
            {
                var f = new ProductFacetCount();
                f.Key = key;
                f.ProductCount = productValueRepository.FindCountProductIdsMatchingKey(key);
                result.Add(f);
            }
            return result;
        }

        /// <summary>
        ///     Find all visible <see cref="CategoryFacet" /> for the given key.
        /// </summary>
        /// <param name="key">Key to be searched for</param>
        /// <param name="allFacets">List of <see cref="CategoryFacet" /></param>
        /// <returns>Return list of <see cref="CategoryFacet" /> ids for matching criteria</returns>
        public List<long> FindVisibleFacetsIdsForKey(string key, List<CategoryFacet> allFacets)
        {
            var result = new List<long>();

            result = FindVisibleChildren(key, allFacets, 0);

            return result;
        }

        /// <summary>
        ///     Find visible children for given parent id.
        /// </summary>
        /// <param name="key">Key to be searched for</param>
        /// <param name="allFacets">List of <see cref="CategoryFacet" /> in which needs to search</param>
        /// <param name="parentId">Parent <see cref="CategoryFacet" /> id</param>
        /// <returns>Returns list of <see cref="CategoryFacet" /> ids for matching criteria</returns>
        private List<long> FindVisibleChildren(string key, List<CategoryFacet> allFacets, long parentId)
        {
            var result = new List<long>();

            var parentIsSelected = IsFacetSelectedInKey(key, allFacets, parentId);

            if (parentIsSelected || parentId == 0)
            {
                foreach (var f in FindByParentInList(allFacets, parentId))
                {
                    result.Add(f.Id);
                    var visibleChildren = FindVisibleChildren(key, allFacets, f.Id);
                    if (visibleChildren != null)
                    {
                        if (visibleChildren.Count > 0)
                        {
                            result.AddRange(visibleChildren);
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        ///     Check if given facet is selected for the given key.
        /// </summary>
        /// <param name="key">Key string</param>
        /// <param name="allFacets">List of <see cref="CategoryFacet" /> which needs to be checked for</param>
        /// <param name="facetId"><see cref="CategoryFacet" /> id which needs to be check</param>
        /// <returns>Returns true if its selected in given key otherwise returns false</returns>
        public bool IsFacetSelectedInKey(string key, List<CategoryFacet> allFacets, long facetId)
        {
            if (key == string.Empty) return false;

            var result = false;

            var keyparts = CategoryFacetKeyHelper.ParseKeyToList(key);
            for (var i = 0; i < allFacets.Count; i++)
            {
                if (allFacets[i].Id == facetId)
                {
                    if (keyparts[i] > 0)
                    {
                        return true;
                    }
                }
            }

            return result;
        }
    }
}