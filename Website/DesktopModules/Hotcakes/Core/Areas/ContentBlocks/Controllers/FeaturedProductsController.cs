﻿#region License

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
using Hotcakes.Commerce.Content;
using Hotcakes.Modules.Core.Areas.ContentBlocks.Models;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Areas.ContentBlocks.Controllers
{
    [Serializable]
    public class FeaturedProductsController : BaseAppController
    {
        public ActionResult Index(ContentBlock block)
        {
            var model = new FeaturedProductsViewModel
            {
                Items = PrepProducts(HccApp.CatalogServices.Products.FindFeatured(1, 100))
            };
            return View(model);
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