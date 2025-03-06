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
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class BreadCrumbController : BaseAppController
    {
        //
        // GET: /BreadCrumb/
        [ChildActionOnly]
        public ActionResult CategoryTrail(Category cat, List<BreadCrumbItem> extras)
        {
            var snap = new CategorySnapshot(cat);

            var model = new BreadCrumbViewModel();

            LoadTrailForCategory(model, snap, false);

            if (extras != null && extras.Count > 0)
            {
                foreach (var item in extras)
                {
                    model.Items.Enqueue(item);
                }
            }
            return View("BreadCrumb", model);
        }

        private void LoadTrailForCategory(BreadCrumbViewModel model, CategorySnapshot cat, bool linkAll)
        {
            if (cat == null) return;
            if (cat.Hidden) return;

            var allCats = HccApp.CatalogServices.Categories.FindAllSnapshotsPaged(1, int.MaxValue);

            var trail = Category.BuildTrailToRoot(cat.Bvin, HccApp.CatalogServices.Categories);

            if (trail == null) return;

            // Walk list backwards
            for (var j = trail.Count - 1; j >= 0; j += -1)
            {
                if (j != 0 || linkAll)
                {
                    model.Items.Enqueue(AddCategoryLink(trail[j]));
                }
                else
                {
                    model.Items.Enqueue(new BreadCrumbItem {Name = trail[j].Name});
                }
            }
        }

        private BreadCrumbItem AddCategoryLink(CategorySnapshot c)
        {
            var result = new BreadCrumbItem
            {
                Name = c.Name,
                Title = c.MetaTitle,
                Link = UrlRewriter.BuildUrlForCategory(c)
            };
            return result;
        }

        [ChildActionOnly]
        public ActionResult ProductTrail(Product product, List<BreadCrumbItem> extras)
        {
            var model = new BreadCrumbViewModel();

            LoadTrailForProduct(model, product);

            if (extras != null)
            {
                foreach (var item in extras)
                {
                    model.Items.Enqueue(item);
                }
            }
            return View("BreadCrumb", model);
        }

        private void LoadTrailForProduct(BreadCrumbViewModel model, Product p)
        {
            if (p == null) return;
            CategorySnapshot currentCategory = null;
            var cats = HccApp.CatalogServices.FindCategoriesForProduct(p.Bvin);
            if (cats.Count > 0)
            {
                currentCategory = cats[0];
            }
            LoadTrailForCategory(model, currentCategory, true);
            model.Items.Enqueue(new BreadCrumbItem {Name = p.ProductName});
        }

        [ChildActionOnly]
        public ActionResult ManualTrail(List<BreadCrumbItem> extras)
        {
            var model = new BreadCrumbViewModel();
            if (extras != null)
            {
                foreach (var item in extras)
                {
                    model.Items.Enqueue(item);
                }
            }
            return View("BreadCrumb", model);
        }
    }
}