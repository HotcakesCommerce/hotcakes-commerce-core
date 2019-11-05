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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.CategoryRotator
{
    public partial class AdminView : HccContentBlockPart
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            LoadCategory();
        }

        private void LoadCategory()
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            var myCategories = b.Lists.FindList("Categories");

            if (myCategories != null && myCategories.Count > 0)
            {
                litCount.Text = myCategories.Count.ToString();
                var settItem = myCategories[0];
                var cat = HccApp.CatalogServices.Categories.Find(settItem.Setting1);

                if (cat != null)
                    RenderCategory(cat);
            }
        }

        private void RenderCategory(Category cat)
        {
            var imageUrl = DiskStorage.CategoryIconUrl(HccApp, cat.Bvin, cat.ImageUrl, Page.Request.IsSecureConnection);

            var htmlDiv = new HtmlGenericControl("div");
            htmlDiv.Attributes["class"] = "hcBlockContent";
            htmlDiv.Controls.Add(new LiteralControl(cat.Name));

            phCategory.Controls.Clear();
            phCategory.Controls.Add(new HtmlImage
            {
                Src = imageUrl
            });

            phCategory.Controls.Add(htmlDiv);
        }
    }
}