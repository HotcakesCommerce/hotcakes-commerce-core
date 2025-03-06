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
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Areas.ContentBlocks.Controllers
{
    [Serializable]
    public class CategoryRotatorController : BaseAppController
    {
        public ActionResult Index(ContentBlock block)
        {
            var model = new SingleCategoryViewModel();

            if (block != null)
            {
                var showInOrder = block.BaseSettings.GetBoolSetting("ShowInOrder");
                var settings = block.Lists.FindList("Categories");

                if (settings.Count != 0)
                {
                    var imageIndex = 0;
                    if (showInOrder)
                    {
                        if (Session[block.Bvin + "PrevImageIndex"] != null)
                            imageIndex = (int) Session[block.Bvin + "PrevImageIndex"];
                        imageIndex++;
                        if (imageIndex >= settings.Count)
                            imageIndex = 0;

                        Session[block.Bvin + "PrevImageIndex"] = imageIndex;
                    }
                    else
                    {
                        var random = new Random();
                        imageIndex = random.Next(settings.Count);
                    }
                    LoadCategory(model, settings[imageIndex].Setting1);
                }
            }

            return View(model);
        }

        private void LoadCategory(SingleCategoryViewModel model, string categoryId)
        {
            var c = HccApp.CatalogServices.Categories.Find(categoryId);
            if (c != null)
            {
                var catSnapshot = new CategorySnapshot(c);
                var destination = UrlRewriter.BuildUrlForCategory(catSnapshot);

                var imageUrl = DiskStorage.CategoryIconUrl(HccApp,
                    c.Bvin,
                    c.ImageUrl,
                    Request.IsSecureConnection);
                model.IconUrl = ImageHelper.SafeImage(imageUrl);

                model.LinkUrl = destination;
                model.AltText = c.MetaTitle;
                model.Name = c.Name;
                model.LocalCategory = catSnapshot;

                if (c.SourceType == CategorySourceType.CustomLink)
                {
                    model.OpenInNewWindow = c.CustomPageOpenInNewWindow;
                }
            }
        }
    }
}