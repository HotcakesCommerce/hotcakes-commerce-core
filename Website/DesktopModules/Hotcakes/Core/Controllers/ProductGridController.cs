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
using System.Linq;
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class ProductGridController : BaseAppController
    {
        public ActionResult Index(Dictionary<int, string> productsList, int maxColumns)
        {
            var model = LoadProductGrid(productsList, maxColumns);
            return View(model);
        }

        private List<SingleProductViewModel> LoadProductGrid(Dictionary<int, string> productsList, int maxColumns)
        {
            var result = new List<SingleProductViewModel>();

            var sortedProducts = new SortedList<int, Product>();
            if (productsList.Count > 0)
            {
                var products = HccApp.CatalogServices.Products.FindManyWithCache(productsList.Values);
                foreach (var item in productsList)
                {
                    sortedProducts.Add(item.Key, products.FirstOrDefault(p => p.Bvin == item.Value));
                }
            }

            if (maxColumns < 1)
                maxColumns = 3;

            ViewBag.MaxColumns = maxColumns;

            foreach (var item in sortedProducts)
            {
                var p = item.Value;
                if (p != null)
                {
                    var vm = new SingleProductViewModel(p, HccApp);
                    result.Add(vm);
                }
            }
            return result;
        }
    }
}