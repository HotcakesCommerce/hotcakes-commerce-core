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
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class CategoryMenuController : BaseAppController
    {
        private const string VirtualRootCategoryBvin = "";
        private const string VIEW_TITLE = "<h4>{0}</h4>";

        #region Controller actions

        public ActionResult Index(CategoryMenuViewModel model, string slug)
        {
            LoadMenu(model, slug);
            return View(model);
        }

        #endregion

        #region Load Menu modes

        private void LoadMenu(CategoryMenuViewModel model, string slug)
        {
            model.CurrentId = LocateCurrentCategory(slug);

            var isAllProducts = false;
            
            // Load Title
            if (!string.IsNullOrEmpty(model.Title))
            {
                model.Title = string.Format(VIEW_TITLE, model.Title);
            }

            var mode = model.CategoryMenuMode;
            switch (mode)
            {
                case "1":
                    // All Categories
                    LoadAllCategories(model);
                    isAllProducts = true;
                    break;
                case "2":
                    // Peers, Children and Parents
                    LoadPeersAndChildren(model);
                    break;
                case "3":
                    // Show root and expanded children
                    LoadRootPlusExpandedChildren(model);
                    break;
                case "4":
                    LoadSelectedCategories(model);
                    break;
                case "5":
                    LoadChildrenOfSelected(model);
                    break;
                case "0":
                default:
                    // Root Categories Only
                    LoadRoots(model);
                    break;
            }

            if (model.ShowProductCounts)
            {
                //Set Product Count
                SetProductsCount(model, model.MenuItem, isAllProducts);
            }

            if (model.ShowHomeLink)
            {
                AddHomeLink(model);
            }
        }

        private void LoadRoots(CategoryMenuViewModel model)
        {
            var cats = HccApp.CatalogServices.Categories.FindVisibleChildren(VirtualRootCategoryBvin);
            var menu = BuildItemsTree(cats, cats, 1, 1, model.CurrentId, model.ShowProductCounts);
            model.MenuItem = menu;
        }

        private void LoadAllCategories(CategoryMenuViewModel model)
        {
            var allCats = HccApp.CatalogServices.Categories.FindAll();
            var children = Category.FindChildrenInList(allCats, VirtualRootCategoryBvin, false);
            var menu = BuildItemsTree(allCats, children, 0, model.MaximumDepth, model.CurrentId, model.ShowProductCounts);
            model.MenuItem = menu;
        }

        private void SetProductsCount(CategoryMenuViewModel model, CategoryMenuItemViewModel outMenu, bool isAllProducts)
        {
            CategoryMenuItemViewModel allMenu;
            if (!isAllProducts)
            {
                var allCats = HccApp.CatalogServices.Categories.FindAll();
                var children = Category.FindChildrenInList(allCats, VirtualRootCategoryBvin, false);
                allMenu = BuildItemsTree(allCats, children, 0, model.MaximumDepth, model.CurrentId,
                    model.ShowProductCounts);
            }
            else
            {
                allMenu = model.MenuItem;
            }

            GetProductsCount(allMenu);

            foreach (var menu in outMenu.Items)
            {
                if (allMenu != null)
                {
                    menu.ProductsCount = allMenu.Items.FirstOrDefault(x => x.Bvin == menu.Bvin).ProductsCount;
                    menu.Title = allMenu.Items.FirstOrDefault(x => x.Bvin == menu.Bvin).Title;
                }
            }

            if (allMenu != null)
            {
                if (allMenu.Bvin == outMenu.Bvin)
                {
                    outMenu.ProductsCount = allMenu.ProductsCount;
                    outMenu.Title = allMenu.Title;
                }
            }
        }

        private int GetProductsCount(CategoryMenuItemViewModel category)
        {
            var totalCount = 0;
            foreach (var item in category.Items)
            {
                totalCount = item.ProductsCount;
                if (!item.Items.Any() || item.Items.Count != 0)
                {
                    totalCount += GetProductsCount(item);
                }
            }
            if (category.Items == null || !category.Items.Any())
            {
                totalCount += category.ProductsCount;
            }
            category.ProductsCount = totalCount;
            category.Title += " (" + totalCount + ")";
            return totalCount;
        }

        private void LoadRootPlusExpandedChildren(CategoryMenuViewModel model)
        {
            var allCats = HccApp.CatalogServices.Categories.FindAll();

            // Get Current Category
            var currentCategory = Category.FindInList(allCats, model.CurrentId) ??
                                  new CategorySnapshot {Bvin = VirtualRootCategoryBvin};

            // Find the trail from this category back to the root of the site
            var trail = new List<CategorySnapshot>();
            BuildParentTrail(allCats, model.CurrentId, trail);

            var roots = Category.FindChildrenInList(allCats, VirtualRootCategoryBvin, false);
            model.MenuItem = BuildItemsTree(allCats, roots, trail, currentCategory.Bvin, model.ShowProductCounts);
        }

        private void LoadPeersAndChildren(CategoryMenuViewModel model)
        {
            var allCats = HccApp.CatalogServices.Categories.FindAll();

            // Get Current Category
            // Trick system into accepting root category of zero which never exists in database
            var currentCategory = Category.FindInList(allCats, model.CurrentId) ??
                                  new CategorySnapshot {Bvin = VirtualRootCategoryBvin};

            // Find the trail from this category back to the root of the site
            var trail = new List<CategorySnapshot>();
            BuildParentTrail(allCats, model.CurrentId, trail);

            if (trail.Count < 1)
            {
                // Load Roots Only
                LoadRoots(model);
            }
            else
            {
                var neighbors = GetPeerSet(allCats, currentCategory);
                model.MenuItem = BuildParentsPeersChildren(neighbors, currentCategory, model.ShowProductCounts);
            }
        }

        private void LoadSelectedCategories(CategoryMenuViewModel model)
        {
            var listAll = HccApp.CatalogServices.Categories.FindAll();
            var selectedCategories = listAll.Where(cat => model.SelectedCategories.Contains(cat.Bvin)).ToList();

            var menu = BuildItemsTree(listAll, selectedCategories, 0, model.MaximumDepth, model.CurrentId,
                model.ShowProductCounts);
            model.MenuItem = menu;
        }

        private void LoadChildrenOfSelected(CategoryMenuViewModel model)
        {
            var listAll = HccApp.CatalogServices.Categories.FindAll();
            var children = Category.FindChildrenInList(listAll, model.ChildrenOfCategory, false);

            var menu = BuildItemsTree(listAll, children, 0, model.MaximumDepth, model.CurrentId, model.ShowProductCounts);
            model.MenuItem = menu;
        }

        #endregion

        #region Helpers

        private string LocateCurrentCategory(string slug)
        {
            var cat = HccApp.CatalogServices.Categories.FindBySlugForStore(slug,
                HccApp.CurrentRequestContext.CurrentStore.Id);

            return cat == null ? VirtualRootCategoryBvin : cat.Bvin;
        }

        private bool IsInTrail(string testBvin, List<CategorySnapshot> trail)
        {
            if (trail != null && trail.Any(c => c.Bvin == testBvin))
                return true;

            return false;
        }

        private void BuildParentTrail(List<CategorySnapshot> allCats, string currentId, List<CategorySnapshot> trail)
        {
            if (string.IsNullOrEmpty(currentId) || currentId == VirtualRootCategoryBvin)
                return;

            var current = Category.FindInList(allCats, currentId);

            if (current != null)
            {
                trail.Add(current);

                if (current.ParentId == VirtualRootCategoryBvin || string.IsNullOrEmpty(current.ParentId))
                    return;

                BuildParentTrail(allCats, current.ParentId, trail);
            }
        }

        private CategoryPeerSet GetPeerSet(List<CategorySnapshot> allCats, CategorySnapshot cat)
        {
            var result = new CategoryPeerSet();

            var parent = Category.FindInList(allCats, cat.ParentId);
            if (parent != null)
            {
                result.Parents = Category.FindChildrenInList(allCats, parent.ParentId, false);
            }
            result.Peers = Category.FindChildrenInList(allCats, cat.ParentId, false);
            result.Children = Category.FindChildrenInList(allCats, cat.Bvin, false);

            return result;
        }

        private void AddHomeLink(CategoryMenuViewModel model)
        {
            if (model.MenuItem == null)
                model.MenuItem = new CategoryMenuItemViewModel();

            var item = new CategoryMenuItemViewModel
            {
                Title = Localization.GetString("Home"),
                Url = Url.Content("~")
            };
            model.MenuItem.Items.Insert(0, item);
        }

        #endregion

        #region Build View Models Helpers

        private CategoryMenuItemViewModel BuildParentsPeersChildren(CategoryPeerSet neighbors,
            CategorySnapshot currentCategory, bool isDisplayProductsCount)
        {
            CategoryMenuItemViewModel result;
            if (neighbors.Parents.Count > 0)
            {
                result = new CategoryMenuItemViewModel();

                foreach (var parent in neighbors.Parents)
                {
                    CategoryMenuItemViewModel item;

                    if (parent.Bvin == currentCategory.ParentId)
                    {
                        item = BuildPeersAndChildren(neighbors, currentCategory, isDisplayProductsCount);
                    }
                    else
                    {
                        item = new CategoryMenuItemViewModel();
                    }
                    item.Title = parent.Name;
                    item.Description = parent.Description;
                    item.Url = UrlRewriter.BuildUrlForCategory(parent);
                    if (isDisplayProductsCount)
                    {
                        item.Bvin = parent.Bvin;
                        item.ProductsCount = HccApp.CatalogServices.FindProductCountsForCategory(parent.Bvin, false);
                    }

                    result.Items.Add(item);
                }
            }
            else
            {
                result = BuildPeersAndChildren(neighbors, currentCategory, isDisplayProductsCount);
            }

            return result;
        }

        private CategoryMenuItemViewModel BuildPeersAndChildren(CategoryPeerSet neighbors,
            CategorySnapshot currentCategory, bool isDisplayProductsCount)
        {
            var result = new CategoryMenuItemViewModel();

            foreach (var peer in neighbors.Peers)
            {
                var item = new CategoryMenuItemViewModel();

                item.Title = peer.Name;
                item.Description = peer.Description;
                item.Url = UrlRewriter.BuildUrlForCategory(peer);
                if (peer.Bvin == currentCategory.Bvin)
                {
                    item.IsCurrent = true;
                    // Load Children
                    foreach (var c in neighbors.Children)
                    {
                        var childItem = new CategoryMenuItemViewModel();
                        childItem.Title = c.Name;
                        childItem.Description = c.Description;
                        if (isDisplayProductsCount)
                        {
                            item.Bvin = c.Bvin;
                            item.ProductsCount = HccApp.CatalogServices.FindProductCountsForCategory(c.Bvin, false);
                        }
                        childItem.Url = UrlRewriter.BuildUrlForCategory(c);
                        item.Items.Add(childItem);
                    }
                }
                if (isDisplayProductsCount)
                {
                    item.Bvin = peer.Bvin;
                    item.ProductsCount = HccApp.CatalogServices.FindProductCountsForCategory(peer.Bvin, false);
                }

                result.Items.Add(item);
            }
            return result;
        }

        private CategoryMenuItemViewModel BuildItemsTree(List<CategorySnapshot> listAll,
            List<CategorySnapshot> catToDisplay, int currentDepth, int maximumDepth, string currentId,
            bool isDisplayProductsCount)
        {
            var result = new CategoryMenuItemViewModel();
            foreach (var c in catToDisplay)
            {
                CategoryMenuItemViewModel item;
                if ((maximumDepth == 0) || (currentDepth + 1 < maximumDepth))
                    item = BuildItemsTree(listAll, Category.FindChildrenInList(listAll, c.Bvin, false), currentDepth + 1,
                        maximumDepth, currentId, isDisplayProductsCount);
                else
                    item = new CategoryMenuItemViewModel();
                item.Title = c.Name;
                item.Description = c.Description;
                item.Url = UrlRewriter.BuildUrlForCategory(c);
                if (isDisplayProductsCount)
                {
                    item.Bvin = c.Bvin;
                    item.ProductsCount = HccApp.CatalogServices.FindProductCountsForCategory(c.Bvin, false);
                }
                if (c.Bvin == currentId)
                    item.IsCurrent = true;

                result.Items.Add(item);
            }
            return result;
        }

        private CategoryMenuItemViewModel BuildItemsTree(List<CategorySnapshot> allCats, List<CategorySnapshot> children,
            List<CategorySnapshot> trail, string currentId, bool isDisplayProductsCount)
        {
            var result = new CategoryMenuItemViewModel();

            foreach (var c in children)
            {
                CategoryMenuItemViewModel item = null;

                if (IsInTrail(c.Bvin, trail))
                {
                    var newChildren = Category.FindChildrenInList(allCats, c.Bvin, false);
                    if (newChildren != null && newChildren.Count > 0)
                        item = BuildItemsTree(allCats, newChildren, trail, currentId, isDisplayProductsCount);
                }

                if (item == null)
                    item = new CategoryMenuItemViewModel();

                item.Title = c.Name;
                item.Description = c.Description;
                item.Url = UrlRewriter.BuildUrlForCategory(c);
                if (isDisplayProductsCount)
                {
                    item.Bvin = c.Bvin;
                    item.ProductsCount = HccApp.CatalogServices.FindProductCountsForCategory(c.Bvin, false);
                }
                if (c.Bvin == currentId)
                    item.IsCurrent = true;

                result.Items.Add(item);
            }
            return result;
        }

        #endregion
    }
}