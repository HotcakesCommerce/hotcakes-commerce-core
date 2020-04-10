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
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class ProductsEdit_TabsSort : BaseAdminJsonPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                var ids = Request.Form["ids"];
                var productBvin = Request.Form["bvin"];
                Resort(ids, productBvin);
            }
        }

        private void Resort(string ids, string productBvin)
        {
            var sorted = ids.Split(',');
            var l = new List<string>();
            foreach (var id in sorted)
            {
                l.Add(id);
            }

            var p = HccApp.CatalogServices.Products.Find(productBvin);
            var currentSort = 1;
            if (p != null)
            {
                foreach (var tabid in l)
                {
                    foreach (var t in p.Tabs)
                    {
                        if (t.Bvin == tabid)
                        {
                            t.SortOrder = currentSort;
                            currentSort += 1;
                        }
                    }
                }
            }

            if (HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p))
            {
                litOutput.Text = "{\"result\":true}";
            }
            else
            {
                litOutput.Text = "{\"result\":false}";
            }
        }
    }
}