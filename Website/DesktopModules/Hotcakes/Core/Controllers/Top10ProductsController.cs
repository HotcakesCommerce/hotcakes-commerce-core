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
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class Top10ProductsController : BaseAppController
    {
        public ActionResult Index()
        {
            var model = new SideMenuViewModel();

            var s = new DateTime(1900, 1, 1);
            var e = new DateTime(3000, 12, 31);
            List<Product> products;
            products = HccApp.ReportingTopSellersByDate(s, e, 10);
            foreach (var p in products)
            {
                var item = new SideMenuItem
                {
                    Title = p.ProductName,
                    Name = p.ProductName,
                    Url = UrlRewriter.BuildUrlForProduct(p)
                };
                item.Name += " - " + p.SitePrice.ToString("C");
                model.Items.Add(item);
            }

            model.Title = "Top Sellers";
            return View(model);
        }
    }
}