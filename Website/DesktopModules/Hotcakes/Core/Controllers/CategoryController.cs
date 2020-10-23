#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Text;
using System.Web;
using System.Web.Mvc;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Search;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Modules.Core.Models.Json;
using Hotcakes.Modules.Core.Settings;
using Newtonsoft.Json;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class CategoryController : BaseStoreController
    {
        #region Properties

        protected bool IsConcreteItemModule
        {
            get { return Convert.ToBoolean(RouteData.Values["isConcreteItemModule"]); }
        }

        #endregion

        #region Implementation / Load category model

        private CategoryPageViewModel LoadCategoryModel(string slug, string preContentColumnId,
            string postContentColumnId)
        {
            Category cat = null;
            if (!string.IsNullOrWhiteSpace(slug))
            {
                CustomUrl customUrl;
                cat = HccApp.ParseCategoryBySlug(slug, out customUrl);
                if (customUrl != null && !IsConcreteItemModule)
                {
                    var redirectUrl = HccUrlBuilder.RouteHccUrl(HccRoute.Category, new {slug = customUrl.RedirectToUrl});
                    if (customUrl.IsPermanentRedirect)
                        RedirectPermanent(redirectUrl);
                    else
                        Redirect(redirectUrl);
                }
                if (cat == null)
                {
                    StoreExceptionHelper.ShowInfo(Localization.GetString("CategoryNotFound"));
                }
                else if (!HccApp.CatalogServices.TestCategoryAccess(cat))
                {
                    StoreExceptionHelper.ShowInfo(Localization.GetString("CategoryNotEnoughPermission"));
                }
            }
            else
            {
                cat = new Category
                {
                    Bvin = string.Empty,
                    PreContentColumnId = preContentColumnId,
                    PostContentColumnId = postContentColumnId
                };
            }

            return new CategoryPageViewModel {LocalCategory = cat};
        }

        #endregion

        #region Public methods

        public ActionResult Index(string slug, int productPageSize = 9, string preContentColumnId = null,
            string postContentColumnId = null)
        {
            var model = LoadCategoryModel(slug, preContentColumnId, postContentColumnId);

            IndexSetup(model);

            var viewName = GetViewName(model);
            if (viewName == null || !viewName.StartsWith("DrillDown"))
            {
                LoadSubCategories(model);
                LoadProducts(model, productPageSize);
            }
            else
            {
                var filter = new CategoryFilterViewModel
                {
                    CategoryId = model.LocalCategory.Bvin,
                    PageNumber = 1,
                    IsConsiderSearchable = false,
                    SortOrder = model.LocalCategory.DisplaySortOrder
                };
                var sett = new CategoryModuleSettings(ModuleContext.ModuleId);
                var ddModel = BuildDrillDownModel(filter, sett, 1, productPageSize);
                model.DrillDownJsonModel = JsonConvert.SerializeObject(ddModel);
                model.SortSelectList = LoadSortSelectList(model.LocalCategory, GetSort(model.LocalCategory), true);
            }

            if (AuthorizedToEditCatalog())
            {
                if (!string.IsNullOrEmpty(model.LocalCategory.Bvin))
                {
                    model.CategoryAnalyticsUrl = string.Format(CategoryAnalyticsUrlFormat, model.LocalCategory.Bvin);
                    model.CategoryEditUrl = string.Format(CategoryEditUrlFormat, model.LocalCategory.Bvin);
                }
                else
                {
                    model.CategoriesManagementUrl = CategoriesAdminUrl;
                    model.ProductsManagementUrl = ProductsAdminUrl;
                }

                model.StoreAdminUrl = DashboardAdminUrl;
                model.AuthorizedToEditCatalog = true;
            }
            else
            {
                model.AuthorizedToEditCatalog = false;
            }

            LogCategoryViewActivity(model.LocalCategory);

            return View(viewName, model);
        }

        [HccHttpPost]
        public ActionResult DrillDown(CategoryFilterViewModel filter)
        {
            var sett = new CategoryModuleSettings(filter.ModuleId);
            var pageNumber = filter.PageNumber ?? 1;
            var pageSize = sett.PageSize;

            filter.IsConsiderSearchable = false;

            var model = BuildDrillDownModel(filter, sett, pageNumber, pageSize);

            return Json(model);
        }

        #endregion

        #region Implementation / Index setup

        private void IndexSetup(CategoryPageViewModel model)
        {
            SetViewBugs(model.LocalCategory);
            SetPageMetaData(model.LocalCategory);
            RegisterSocialFunctionality(model);
        }

        private void RegisterSocialFunctionality(CategoryPageViewModel model)
        {
            ViewBag.UseFaceBook = HccApp.CurrentStore.Settings.FaceBook.UseFaceBook;

            if (!string.IsNullOrWhiteSpace(model.LocalCategory.Bvin))
            {
                var socialService = HccApp.SocialService;
                socialService.SaveCategoryToJournal(model.LocalCategory);

                // Obsolete in 3.0 - may need to consider adding back in a future release, if integrating with Evoq Social again
                //model.SocialItem = socialService.GetCategorySocialItem(model.LocalCategory);

                RenderFacebookMetaTags(model);
            }
        }

        private void SetViewBugs(Category cat)
        {
            ViewBag.DisplayHtml = TagReplacer.ReplaceContentTags(cat.Description, HccApp);
            ViewBag.LinkUrl = BuildUrlForCategory(cat, null, null);

            // Banner
            if (cat.BannerImageUrl.Trim().Length > 0)
            {
                ViewBag.ShowBanner = true;
                ViewBag.BannerUrl = DiskStorage.CategoryBannerUrl(
                    HccApp,
                    cat.Bvin,
                    cat.BannerImageUrl,
                    Request.IsSecureConnection);
                ViewBag.ImageUrl =
                    DiskStorage.CategoryIconUrl(HccApp, cat.Bvin, cat.ImageUrl, Request.IsSecureConnection);
            }
            else
            {
                ViewBag.ShowBanner = false;
            }
        }

        private void SetPageMetaData(Category cat)
        {
            if (!IsConcreteItemModule)
            {
                var title = string.Empty;
                if (!string.IsNullOrWhiteSpace(cat.MetaTitle))
                    title = cat.MetaTitle;
                else
                    title = cat.Name;
                if (!string.IsNullOrWhiteSpace(title))
                    PageTitle = title;

                if (!string.IsNullOrWhiteSpace(cat.MetaKeywords))
                    PageKeywords = cat.MetaKeywords;
                if (!string.IsNullOrWhiteSpace(cat.MetaDescription))
                    PageDescription = cat.MetaDescription;
            }
        }

        private void RenderFacebookMetaTags(CategoryPageViewModel model)
        {
            if (ViewBag.UseFaceBook)
            {
                var faceBookAdmins = HccApp.CurrentStore.Settings.FaceBook.Admins;
                var faceBookAppId = HccApp.CurrentStore.Settings.FaceBook.AppId;
                var canonicalUrl = HccUrlBuilder.RouteHccUrl(HccRoute.Category, new {slug = model.LocalCategory.RewriteUrl.ToLower()});

                var sb = new StringBuilder();

                sb.AppendFormat(Constants.TAG_CANONICAL, canonicalUrl);
                sb.AppendFormat(Constants.TAG_OGTITLE, PageTitle);
                sb.Append(Constants.TAG_OGTYPE);
                sb.AppendFormat(Constants.TAG_OGURL, canonicalUrl);
                sb.AppendFormat(Constants.TAG_OGIMAGE, model.LocalCategory.ImageUrl);
                sb.AppendFormat(Constants.TAG_OGSITENAME, ViewBag.StoreName);
                sb.AppendFormat(Constants.TAG_OGFBADMIN, faceBookAdmins);
                sb.AppendFormat(Constants.TAG_OGFBAPPID, faceBookAppId);

                RenderToHead("FaceBookMetaTags", sb.ToString());
            }
        }

        #endregion

        #region Implementation / Load Products and subcategories

        private void LoadProducts(CategoryPageViewModel model, int productPageSize)
        {
            var cat = model.LocalCategory;
            var pageNumber = GetPageNumber();
            var pageSize = productPageSize <= 0 ? 9 : productPageSize;
            var totalItems = 0;

            var sortOrder = GetSort(cat);
            var products = HccApp.CatalogServices.
                FindProductForCategoryWithSort(cat.Bvin, sortOrder, false, pageNumber, pageSize, ref totalItems);

            model.Products = products.Select(p => new SingleProductViewModel(p, HccApp)).ToList();

            model.PagerData.PageSize = pageSize;
            model.PagerData.TotalItems = totalItems;
            model.PagerData.CurrentPage = pageNumber;
            model.PagerData.PagerUrlFormat = BuildUrlForCategory(cat, HttpUtility.HtmlEncode("{0}"),
                new {sort = (int) sortOrder});
            model.PagerData.PagerUrlFormatFirst = BuildUrlForCategory(cat, null, new {sort = (int) sortOrder});

            model.SortSelectList = LoadSortSelectList(cat, sortOrder);
        }

        private List<SelectListItem> LoadSortSelectList(Category cat, CategorySortOrder sortOrder,
            bool isDrillDown = false)
        {
            var categorySettings = new CategoryModuleSettings(ModuleContext.ModuleId);
            var items = new List<SelectListItem>();
            var sortOrders = categorySettings.SortOrderOptions;
            sortOrders.Insert(0, CategorySortOrder.ManualOrder);

            foreach (var order in sortOrders)
            {
                var url = !isDrillDown
                    ? BuildUrlForCategory(cat, null, new {sort = (int) order})
                    : ((int) order).ToString();
                var text = Localization.GetString(string.Format("CategorySortOrder{0}", order));
                items.Add(new SelectListItem {Selected = sortOrder == order, Text = text, Value = url});
            }

            return items;
        }

        private void LoadSubCategories(CategoryPageViewModel model)
        {
            model.SubCategories = new List<SingleCategoryViewModel>();
            var children = HccApp.CatalogServices.Categories.FindVisibleChildren(model.LocalCategory.Bvin);

            foreach (var snap in children)
            {
                var cat = new SingleCategoryViewModel
                {
                    LinkUrl = UrlRewriter.BuildUrlForCategory(snap),
                    IconUrl = DiskStorage.CategoryIconUrl(HccApp, snap.Bvin, snap.ImageUrl, Request.IsSecureConnection),
                    AltText = snap.Name,
                    Name = snap.Name,
                    LocalCategory = snap
                };

                model.SubCategories.Add(cat);
            }
        }

        #endregion

        #region Implementation / DrillDown

        private DrillDownJsonModel BuildDrillDownModel(CategoryFilterViewModel filter, CategoryModuleSettings sett,
            int pageNumber, int pageSize)
        {
            var model = new DrillDownJsonModel();
            var manager = new SearchManager(HccApp.CurrentRequestContext);
            var queryAdv = BuildDrillDownQuery(filter);
            var result = manager.DoProductSearch(HccApp.CurrentStore.Id, null, queryAdv, pageNumber, pageSize);

            model.Manufacturers = sett.ShowManufacturers
                ? ToCheckBoxItems(result.Manufacturers, result.SelectedManufacturers, result.TotalCount)
                : new List<CheckboxFacetItem>();
            model.Vendors = sett.ShowVendors
                ? ToCheckBoxItems(result.Vendors, result.SelectedVendors, result.TotalCount)
                : new List<CheckboxFacetItem>();

            model.SubCategories = LoadDrillDownCategories(filter.CategoryId);
            model.SortOrder = queryAdv.SortOrder;

            LoadDrillDownModel(model, pageNumber, pageSize, result);
            return model;
        }

        private List<CategoryMenuItemViewModel> LoadDrillDownCategories(string catId)
        {
            var cats = HccApp.CatalogServices.Categories.FindVisibleChildren(catId);

            return cats.Select(c =>
            {
                var cat = new CategoryMenuItemViewModel
                {
                    Title = c.Name,
                    Description = c.Description,
                    Url = UrlRewriter.BuildUrlForCategory(c)
                };
                return cat;
            }).ToList();
        }

        private void LoadDrillDownModel(DrillDownJsonModel model, int pageNumber, int pageSize,
            ProductSearchResultAdv result)
        {
            var products = HccApp.CatalogServices.Products.FindManyWithCache(result.Products);

            model.Types = ToCheckBoxItems(result.Types, result.SelectedTypes, result.TotalCount);

            model.Properties = new List<PropertyFacetItem>();

            foreach (var prop in result.Properties)
            {
                var selProp = result.SelectedProperties.FirstOrDefault(s => s.Id == prop.Id);
                var prop2 = new PropertyFacetItem
                {
                    Id = prop.Id,
                    DisplayName = prop.DisplayName,
                    PropertyName = prop.PropertyName
                };

                if (selProp != null && selProp.PropertyValues.Count > 0)
                {
                    prop2.FacetItems = prop.FacetItems
                        .Select(
                            m =>
                                new CheckboxFacetItem(m, selProp.PropertyValues.Any(pv => pv.Id == m.Id),
                                    result.TotalCount))
                        .ToList<FacetItem>();
                }
                else
                {
                    prop2.FacetItems = prop.FacetItems
                        .Select(m => new CheckboxFacetItem(m, false))
                        .ToList<FacetItem>();
                }

                model.Properties.Add(prop2);
            }

            model.Products = products.Select(p => new SingleProductJsonModel(p, HccApp)).ToList();
            model.TotalCount = result.TotalCount;
            model.MinPrice = result.MinPrice;
            model.MaxPrice = result.MaxPrice;
            model.SelectedMinPrice = result.SelectedMinPrice;
            model.SelectedMaxPrice = result.SelectedMaxPrice;
            model.SelectedManufacturers = result.SelectedManufacturers;
            model.SelectedVendors = result.SelectedVendors;
            model.SelectedTypes = result.SelectedTypes;
            model.SelectedProperties = result.SelectedProperties;

            model.PagerData.PageSize = pageSize;
            model.PagerData.TotalItems = result.TotalCount;
            model.PagerData.CurrentPage = pageNumber;
            model.PagerData.PageRange = 20;
        }

        private ProductSearchQueryAdv BuildDrillDownQuery(CategoryFilterViewModel filter)
        {
            var queryAdv = new ProductSearchQueryAdv
            {
                MinPrice = filter.MinPrice,
                MaxPrice = filter.MaxPrice,
                Manufacturers = filter.Manufacturers ?? new List<string>(),
                Vendors = filter.Vendors ?? new List<string>(),
                Types = filter.Types ?? new List<string>(),
                Categories = new List<string> {filter.CategoryId},
                Properties = string.IsNullOrEmpty(filter.PropertiesJson)
                    ? new Dictionary<long, string[]>()
                    : JsonConvert.DeserializeObject<Dictionary<long, string[]>>(filter.PropertiesJson),
                IsConsiderSearchable = filter.IsConsiderSearchable,
                IsSearchable = filter.IsSearchable
            };

            var cat = HccApp.CatalogServices.Categories.Find(filter.CategoryId);
            queryAdv.SortOrder = (filter.SortOrder != CategorySortOrder.None) ? filter.SortOrder : cat != null ? cat.DisplaySortOrder : CategorySortOrder.ManualOrder;
            return queryAdv;
        }

        private List<CheckboxFacetItem> ToCheckBoxItems(List<FacetItem> items, List<SelectedFacetItem> selected,
            int baseCount)
        {
            if (selected != null && selected.Count > 0)
                return items.Select(m => new CheckboxFacetItem(m, selected.Any(s => s.Id == m.Id), baseCount)).ToList();
            return items.Select(m => new CheckboxFacetItem(m, false)).ToList();
        }

        #endregion

        #region Implementation

        private void LogCategoryViewActivity(Category cat)
        {
            SessionManager.CategoryLastId = cat.Bvin;

            if (!IsConcreteItemModule)
                HccApp.AnalyticsService.RegisterEvent(HccApp.CurrentCustomerId, ActionTypes.CategoryViewed, cat.Bvin);
        }

        private int GetPageNumber()
        {
            var result = 1;
            if (Request.QueryString["page"] != null)
            {
                int.TryParse(Request.QueryString["page"], out result);
            }
            if (result < 1) result = 1;
            return result;
        }

        private CategorySortOrder GetSort(Category cat)
        {
            var result = CategorySortOrder.ManualOrder;

            if (!string.IsNullOrEmpty(Request.QueryString["sort"]))
            {
                Enum.TryParse(Request.QueryString["sort"], out result);
            }
            else
            {
                result = cat.DisplaySortOrder;
            }

            return result;
        }

        private string BuildUrlForCategory(Category cat, string pageNumber, object addParams)
        {
            if (pageNumber != null)
                return UrlRewriter.BuildUrlForCategory(new CategorySnapshot(cat), pageNumber, addParams);
            return UrlRewriter.BuildUrlForCategory(new CategorySnapshot(cat), addParams);
        }

        private string GetViewName(CategoryPageViewModel model)
        {
            string[] viewNames = {ModuleViewName, model.LocalCategory.TemplateName};
            if (IsConcreteItemModule)
                return viewNames.FirstOrDefault(view => !string.IsNullOrWhiteSpace(view));
            return viewNames.LastOrDefault(view => !string.IsNullOrWhiteSpace(view));
        }

        #endregion
    }
}