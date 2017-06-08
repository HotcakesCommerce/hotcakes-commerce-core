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

using System.Web.UI;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Controls
{
    partial class CategoryBreadCrumbTrail : HccUserControl
    {
        private string _Spacer = "&nbsp;::&nbsp;";

        public CategoryBreadCrumbTrail()
        {
            DisplayLinks = false;
        }

        public string Spacer
        {
            get { return _Spacer; }
            set { _Spacer = value; }
        }

        public bool DisplayLinks { get; set; }

        public void LoadTrail(string categoryId)
        {
            TrailPlaceholder.Controls.Clear();

            var trail = Category.BuildTrailToRoot(categoryId, HccApp.CatalogServices.Categories);

            if (DisplayLinks)
            {
                var m = new HyperLink();
                m.CssClass = "root";
                m.ToolTip = GlobalLocalization.GetString("Home");
                m.Text = GlobalLocalization.GetString("Home");
                m.NavigateUrl = ResolveUrl("~/");
                m.EnableViewState = false;
                TrailPlaceholder.Controls.Add(m);
            }
            else
            {
                TrailPlaceholder.Controls.Add(new LiteralControl("<span class=\"current\">Home</span>"));
            }

            TrailPlaceholder.Controls.Add(new LiteralControl("<span class=\"spacer\">" + _Spacer + "</span>"));
            if (trail != null)
            {
                // Walk list backwards
                for (var i = trail.Count - 1; i >= 0; i += -1)
                {
                    if (i != trail.Count - 1)
                    {
                        TrailPlaceholder.Controls.Add(new LiteralControl("<span class=\"spacer\">" + _Spacer + "</span>"));
                    }
                    if (i != 0)
                    {
                        if (DisplayLinks)
                        {
                            AddCategoryLink(trail[i]);
                        }
                        else
                        {
                            AddCategoryName(trail[i]);
                        }
                    }

                    else
                    {
                        AddCategoryName(trail[i]);
                    }
                }
            }
        }

        private void AddCategoryLink(CategorySnapshot c)
        {
            var m = new HyperLink
            {
                ToolTip = c.MetaTitle,
                Text = c.Name,
                NavigateUrl = UrlRewriter.BuildUrlForCategory(c)
            };

            if (c.SourceType == CategorySourceType.CustomLink)
            {
                if (c.CustomPageOpenInNewWindow)
                {
                    m.Target = "_blank";
                }
            }

            m.EnableViewState = false;
            TrailPlaceholder.Controls.Add(m);
        }

        private void AddCategoryName(CategorySnapshot c)
        {
            TrailPlaceholder.Controls.Add(new LiteralControl("<span class=\"current\">" + c.Name + "</span>"));
        }
    }
}