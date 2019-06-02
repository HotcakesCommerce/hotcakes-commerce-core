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
using System.Globalization;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using DotNetNuke.Entities.Portals;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Search;
using Hotcakes.Commerce.Social;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Modules.Core.Settings;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class SearchController : BaseStoreController
    {
        //
        // GET: /Search/
        [ValidateInput(false)]
        public ActionResult Index(string search, int pageSize = 9)
        {
            // Initial Setup
            PageDescription += " | " + search;
            ViewBag.Search = search;

            var model = new SearchPageViewModel();

            if (!string.IsNullOrEmpty(search))
            {
                // Pager Vars
                var pageNumber = GetPageNumber();
                var logSearchQuery = pageNumber == 1;

                // Do Search
                var manager = new SearchManager(HccApp.CurrentRequestContext);
                var queryAdv = new ProductSearchQueryAdv();
                if (!string.IsNullOrEmpty(Request.Params["categories"]))
                {
                    queryAdv.Categories = Request.Params["categories"].Split(',').ToList();
                    logSearchQuery = false;
                }
                if (!string.IsNullOrEmpty(Request.Params["types"]))
                {
                    queryAdv.Types = Request.Params["types"].Split(',').ToList();
                    logSearchQuery = false;
                }
                if (!string.IsNullOrEmpty(Request.Params["manufacturers"]))
                {
                    queryAdv.Manufactures = Request.Params["manufacturers"].Split(',').ToList();
                    logSearchQuery = false;
                }
                if (!string.IsNullOrEmpty(Request.Params["vendors"]))
                {
                    queryAdv.Vendors = Request.Params["vendors"].Split(',').ToList();
                    logSearchQuery = false;
                }

                var keys = Request.Params.AllKeys.
                    Where(k => k != null && k.StartsWith("prop")).
                    ToList();

                foreach (var key in keys)
                {
                    var propId = int.Parse(key.Substring(4));
                    var propValue = Request.Params[key];
                    queryAdv.Properties.Add(propId, new[] {propValue});
                }
                if (keys.Count > 0)
                {
                    logSearchQuery = false;
                }

                if (!string.IsNullOrEmpty(Request.Params["minprice"]))
                {
                    queryAdv.MinPrice = decimal.Parse(Request.Params["minprice"], CultureInfo.InvariantCulture);
                    logSearchQuery = false;
                }
                if (!string.IsNullOrEmpty(Request.Params["maxprice"]))
                {
                    queryAdv.MaxPrice = decimal.Parse(Request.Params["maxprice"], CultureInfo.InvariantCulture);
                    logSearchQuery = false;
                }

                if (logSearchQuery && HttpContext.Items["HccSearchQueryLogged"] == null)
                {
                    HttpContext.Items["HccSearchQueryLogged"] = true;
                }
                else
                {
                    logSearchQuery = false;
                }

                var result = manager.DoProductSearch(HccApp.CurrentStore.Id,
                    search,
                    queryAdv,
                    pageNumber,
                    pageSize,
                    logSearchQuery);
                var products = HccApp.CatalogServices.Products.FindManyWithCache(result.Products);

                var searchSettings = new SearchModuleSettings(ModuleContext.ModuleId);

                model.ShowManufactures = searchSettings.ShowManufactures;
                model.ShowVendors = searchSettings.ShowVendors;

                model.Categories = result.Categories;
                model.Types = result.Types;
                model.Manufactures = result.Manufacturers;
                model.Vendors = result.Vendors;
                model.Properties = result.Properties;
                model.MinPrice = result.MinPrice;
                model.MaxPrice = result.MaxPrice;

                model.SelectedCategories = result.SelectedCategories;
                model.SelectedTypes = result.SelectedTypes;
                model.SelectedManufactures = result.SelectedManufacturers;
                model.SelectedVendors = result.SelectedVendors;
                model.SelectedProperties = result.SelectedProperties;
                model.SelectedMinPrice = result.SelectedMinPrice;
                model.SelectedMaxPrice = result.SelectedMaxPrice;

                model.Products = PrepProducts(products);
                model.PagerData.PageSize = pageSize;
                model.PagerData.TotalItems = result.TotalCount;
                model.PagerData.CurrentPage = pageNumber;
                model.PagerData.PageRange = 20;

                var currentParams = new List<string>();
                string[] parameters =
                {
                    "search", "categories", "types", "manufacturers", "vendors", "minprice",
                    "maxprice"
                };
                var props = Request.QueryString.AllKeys.
                    Where(k => k != null && k.StartsWith("prop")).
                    ToList();
                currentParams.AddRange(parameters);
                currentParams.AddRange(props);
                foreach (var param in currentParams)
                {
                    if (!string.IsNullOrEmpty(Request.QueryString[param]))
                        model.CurrentRouteValues[param] = HttpUtility.UrlEncode(Request.QueryString[param]);
                }

                var routeValues = new RouteValueDictionary(model.CurrentRouteValues);
                model.PagerData.PagerUrlFormatFirst = Url.RouteHccUrl(HccRoute.Search, routeValues);
                routeValues.Add("p", "{0}");
                model.PagerData.PagerUrlFormat = Url.RouteHccUrl(HccRoute.Search, routeValues);
            }

            return View(model);
        }

        [ValidateInput(false)]
        public ActionResult SearchInput(string search)
        {
            var searchTabId = HccApp.CurrentStore.Settings.Urls.SearchTabId;

            if (!string.IsNullOrEmpty(search) && PortalSettings.Current.ActiveTab.TabID != searchTabId)
            {
                return Redirect(Url.RouteHccUrl(HccRoute.Search));
            }
            ViewBag.Search = search;

            var model = new SearchPageViewModel();

            return View(model);
        }

        private int GetPageNumber()
        {
            var result = 1;
            if (Request.QueryString["p"] != null)
            {
                int.TryParse(Request.QueryString["p"], out result);
            }
            if (result < 1) result = 1;
            return result;
        }

        private List<SingleProductViewModel> PrepProducts(List<Product> products)
        {
            var result = new List<SingleProductViewModel>();

            foreach (var p in products)
            {
                var model = new SingleProductViewModel(p, HccApp);
                result.Add(model);
            }

            return result;
        }
    }
}