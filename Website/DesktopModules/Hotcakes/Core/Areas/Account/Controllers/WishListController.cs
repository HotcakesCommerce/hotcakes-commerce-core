#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Areas.Account.Models;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Areas.Account.Controllers
{
    [CustomerSignedInFilter]
    [Serializable]
    public class WishListController : BaseStoreController
    {
        public ActionResult Index()
        {
            var items = HccApp.CatalogServices.WishListItems.FindByCustomerIdPaged(HccApp.CurrentCustomerId, 1, 50);
            var model = PrepItems(items);
            if (model.Count < 1)
            {
                FlashInfo(Localization.GetString("WishListEmpty"));
            }
            return View(model);
        }

        [HccHttpPost]
        public ActionResult Delete(long itemid)
        {
            var customerId = HccApp.CurrentCustomerId;
            var wi = HccApp.CatalogServices.WishListItems.Find(itemid);
            if (wi != null)
            {
                if (wi.CustomerId == customerId)
                {
                    HccApp.CatalogServices.WishListItems.Delete(itemid);
                }
            }
            return Redirect(Url.RouteHccUrl(HccRoute.WishList));
        }

        [HccHttpPost]
        public ActionResult AddToCart(long itemid)
        {
            var customerId = HccApp.CurrentCustomerId;
            var wi = HccApp.CatalogServices.WishListItems.Find(itemid);
            if (wi == null || wi.CustomerId != customerId)
                return Redirect(Url.RouteHccUrl(HccRoute.WishList));

            // Add to Cart
            var p = HccApp.CatalogServices.Products.FindWithCache(wi.ProductId);

            var isPurchasable = ValidateSelections(p, wi.SelectionData);
            var isNotRecurringMixed = ValidateRecurringItems(p);
            if (isPurchasable && isNotRecurringMixed)
            {
                var li = p.ConvertToLineItem(HccApp, 1, wi.SelectionData);

                HccApp.OrderServices.EnsureShoppingCart();
                HccApp.AddToOrderWithCalculateAndSave(CurrentCart, li);

                return Redirect(Url.RouteHccUrl(HccRoute.Cart));
            }
            var items = HccApp.CatalogServices.WishListItems.FindByCustomerIdPaged(HccApp.CurrentCustomerId, 1, 50);
            var model = PrepItems(items);
            if (model.Count < 1)
            {
                FlashInfo(Localization.GetString("WishListEmpty"));
            }
            return View("Index", model);
        }

        private bool ValidateSelections(Product p, OptionSelections selections)
        {
            var result = p.ValidateSelections(selections);
            return result == ValidateSelectionsResult.Success;
        }

        private List<SavedItemViewModel> PrepItems(List<WishListItem> items)
        {
            var result = new List<SavedItemViewModel>();

            if (items == null) return result;

            foreach (var item in items)
            {
                var p = HccApp.CatalogServices.Products.FindWithCache(item.ProductId);
                item.ProductShortDescription = p.Options.CartDescription(item.SelectionData.OptionSelectionList);
                if (p != null)
                {
                    var m = new SavedItemViewModel
                    {
                        SavedItem = item,
                        FullProduct = new SingleProductViewModel(p, HccApp)
                    };
                    result.Add(m);
                }
            }

            return result;
        }

        private bool ValidateRecurringItems(Product product)
        {
            if (CurrentCart != null && CurrentCart.Items.Count > 0)
            {
                if (product.IsRecurring != CurrentCart.IsRecurring)
                {
                    FlashFailure(Localization.GetString("YouCannotMixRecurringLineItems"));
                    return false;
                }
            }

            return true;
        }
    }
}