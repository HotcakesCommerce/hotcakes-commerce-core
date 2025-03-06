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
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Modules.Core.Admin.Controls;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    public class BaseCategoryPage : BaseAdminPage
    {
        #region Event Handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        #endregion

        #region Implementation

        protected void InitNavMenu(NavMenu navMenu, bool isCustomLink = false)
        {
            if (!string.IsNullOrEmpty(CategoryId))
            {
                var cat = HccApp.CatalogServices.Categories.Find(CategoryId);
                isCustomLink = cat.SourceType == CategorySourceType.CustomLink;
            }

            // 1. If CUSTOM LINK Category -> remove Manual Selection page
            // 2. If CUSTOM LINK Category -> replace _Edit.aspx page to _EditCustomLink.aspx
            // 3. Append url with 'id' parameter
            navMenu.ProcessItems = items => items
                .Where(i => !isCustomLink || i.Name != "SelectProducts")
                .Select(i =>
                {
                    var mi = new MenuItem {Text = i.Text};

                    if (string.IsNullOrEmpty(CategoryId) && i.Name != "Name")
                    {
                        mi.Url = null;
                    }
                    else if (isCustomLink && i.Name == "Name")
                    {
                        mi.Url = i.Url.Replace("_Edit.aspx", "_EditCustomLink.aspx?id=" + CategoryId);
                    }
                    else
                    {
                        mi.Url = i.Url + "?id=" + CategoryId;
                    }

                    return mi;
                }).ToList();
        }

        #endregion

        #region Properties

        public string CategoryId
        {
            get
            {
                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                    return id;
                return (string) ViewState["CategoryId"] ?? string.Empty;
            }
            set { ViewState["CategoryId"] = value; }
        }

        public string ParentCategoryId
        {
            get { return Request.QueryString["ParentID"]; }
        }

        public string CategoryType
        {
            get { return Request.QueryString["type"]; }
        }

        #endregion
    }
}