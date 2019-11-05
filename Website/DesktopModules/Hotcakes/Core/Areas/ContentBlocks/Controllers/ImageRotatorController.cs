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
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Areas.ContentBlocks.Models;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Areas.ContentBlocks.Controllers
{
    [Serializable]
    public class ImageRotatorController : BaseAppController
    {
        public ActionResult Index(ContentBlock block)
        {
            var model = new ImageRotatorViewModel();

            if (block != null)
            {
                var imageList = block.Lists.FindList("Images");
                foreach (var listItem in imageList)
                {
                    var img = new ImageRotatorImageViewModel
                    {
                        ImageUrl = ResolveUrl(listItem.Setting1),
                        Url = listItem.Setting2
                    };
                    if (img.Url.StartsWith("~"))
                    {
                        img.Url = Url.Content(img.Url);
                    }
                    img.NewWindow = listItem.Setting3 == "1";
                    img.Caption = listItem.Setting4;
                    model.Images.Add(img);
                }
                var cleanId = Text.ForceAlphaNumericOnly(block.Bvin);
                model.CssId = "rotator" + cleanId;
                model.CssClass = block.BaseSettings.GetSettingOrEmpty("cssclass");

                model.Height = block.BaseSettings.GetIntegerSetting("Height");
                model.Width = block.BaseSettings.GetIntegerSetting("Width");

                if (block.BaseSettings.GetBoolSetting("ShowInOrder") == false)
                {
                    RandomizeList(model.Images);
                }
            }

            return View(model);
        }

        private string ResolveUrl(string raw)
        {
            if (raw.Trim().ToLowerInvariant().StartsWith("http"))
                return raw;
            return DiskStorage.StoreUrl(
                HccApp,
                raw,
                Request.IsSecureConnection);
        }

        private void RandomizeList(List<ImageRotatorImageViewModel> list)
        {
            var rand = new Random();
            for (var i = list.Count - 1; i >= 0; i--)
            {
                var n = rand.Next(i + 1);
                // Swap
                var temp = list[i];
                list[i] = list[n];
                list[n] = temp;
            }
        }
    }
}