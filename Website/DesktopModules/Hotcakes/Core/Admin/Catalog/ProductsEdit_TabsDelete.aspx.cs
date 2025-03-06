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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class ProductsEdit_TabsDelete : BaseAdminJsonPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                var TabId = Request.Form["id"];
                var ProductId = Request.Form["bvin"];
                Delete(TabId, ProductId);
                var Redirect = Request.Form["redirect"];
                if ((Redirect == "1") | (Redirect == "y"))
                {
                    Response.Redirect("ProductsEdit_Tabs.aspx?id=" + ProductId);
                }
            }
        }

        private void Delete(string id, string bvin)
        {
            var result = false;

            var p = HccApp.CatalogServices.Products.Find(bvin);
            if (p != null)
            {
                var newTabs = new List<ProductDescriptionTab>();

                var currentSort = 1;
                foreach (var t in p.Tabs)
                {
                    if (t.Bvin != id)
                    {
                        t.SortOrder = currentSort;
                        currentSort += 1;
                        newTabs.Add(t);
                    }
                }
                p.Tabs = newTabs;
                result = HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(p);
            }

            if (result)
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