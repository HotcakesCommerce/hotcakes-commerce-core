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
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class Categories_Sort : BaseAdminJsonPage
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                var ids = Request.Form["ids"];
                var parentId = Request.Form["bvin"];
                if (parentId.StartsWith("children"))
                {
                    parentId = parentId.Substring(8);
                }

                Resort(ids, parentId);
            }
        }

        private void Resort(string ids, string parentId)
        {
            var children = HccApp.CatalogServices.Categories.FindChildren(parentId);

            var sorted = ids.Split(',');
            var l = new List<string>();
            foreach (var id in sorted)
            {
                l.Add(id);
            }

            var counter = 1;
            foreach (var s in l)
            {
                foreach (var child in children)
                {
                    if (child.Bvin == s)
                    {
                        var c = HccApp.CatalogServices.Categories.Find(child.Bvin);
                        c.SortOrder = counter;
                        counter += 1;
                        HccApp.CatalogServices.CategoryUpdate(c);
                    }
                }
            }

            litOutput.Text = "true";
        }
    }
}