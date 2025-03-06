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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class Categories_FacetSort : BaseAdminJsonPage
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
            long theParentId = 0;
            long.TryParse(parentId, out theParentId);

            var manager = Factory.CreateRepo<CategoryFacetManager>();

            var children = manager.FindByParent(theParentId);

            var sorted = ids.Split(',');
            var l = new List<long>();
            foreach (var id in sorted)
            {
                long temp = 0;
                long.TryParse(id, out temp);
                if (temp > 0)
                {
                    l.Add(temp);
                }
            }

            var counter = 1;
            foreach (var s in l)
            {
                foreach (var child in children)
                {
                    if (child.Id == s)
                    {
                        child.SortOrder = counter;
                        counter += 1;
                        manager.Update(child);
                    }
                }
            }

            litOutput.Text = "true";
        }
    }
}