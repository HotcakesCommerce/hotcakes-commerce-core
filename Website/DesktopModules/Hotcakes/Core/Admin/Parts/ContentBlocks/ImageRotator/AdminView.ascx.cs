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
using System.Web.UI.HtmlControls;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Parts.ContentBlocks.ImageRotator
{
    partial class AdminView : HccContentBlockPart
    {
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            LoadImage();
        }

        private void LoadImage()
        {
            var b = HccApp.ContentServices.Columns.FindBlock(BlockId);
            var images = b.Lists.FindList("Images");

            if (images != null && images.Count > 0)
            {
                litCount.Text = images.Count.ToString();
                var settItem = images[0];

                RenderImage(settItem.Setting1);
            }
        }

        private void RenderImage(string imgUrl)
        {
            phProduct.Controls.Clear();
            phProduct.Controls.Add(new HtmlImage
            {
                Src = ResolveSpecialUrl(imgUrl)
            });
        }

        private string ResolveSpecialUrl(string raw)
        {
            // full url
            var tester = raw.Trim().ToLowerInvariant();
            if (tester.StartsWith("http:") || tester.StartsWith("https:")
                || tester.StartsWith("//")) return raw;

            // tag replaced url {{img}} or {{assets}
            if (tester.StartsWith("{{"))
            {
                return TagReplacer.ReplaceContentTags(raw, HccApp);
            }

            // app relative url
            if (tester.StartsWith("~"))
            {
                return ResolveUrl(raw);
            }

            // old style asset
            return DiskStorage.StoreUrl(
                HccApp,
                raw,
                HccApp.IsCurrentRequestSecure());
        }
    }
}