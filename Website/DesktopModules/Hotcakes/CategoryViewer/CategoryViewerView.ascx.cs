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
using Hotcakes.Commerce.Dnn.Web;

namespace Hotcakes.Modules.CategoryViewer
{
    public partial class CategoryViewerView : HotcakesModuleBase
    {
        private readonly int DefaultProductPageSize = 9;

        protected override string RenderView()
        {
            var isConcreteItemModule = false;
            var slug = (string) Settings["Slug"];
            if (!string.IsNullOrEmpty(slug))
                isConcreteItemModule = true;

            if (string.IsNullOrEmpty(slug))
                slug = Request.Params["slug"];

            var productPageSizeSetting = Convert.ToString(Settings["ProductPageSize"]);
            var productPageSize = 0;

            if (!int.TryParse(productPageSizeSetting, out productPageSize))
            {
                productPageSize = DefaultProductPageSize;
            }

            RegisterKOScripts();
            RegisterViewScript("Pager.js");
            RegisterViewScript("Category.js");
            return MvcRenderingEngine.Render("Category", "Index", new
            {
                slug,
                isConcreteItemModule,
                productPageSize,
                preContentColumnId = Convert.ToString(Settings["DefaultPreContentColumnId"]),
                postContentColumnId = Convert.ToString(Settings["DefaultPostContentColumnId"])
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }
    }
}