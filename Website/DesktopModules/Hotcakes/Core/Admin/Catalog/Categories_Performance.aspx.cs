#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Categories_Performance : BaseCategoryPage
    {
        #region Fields

        #endregion

        #region Event handlers

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            PageTitle = Localization.GetString("PageTitle");
            CurrentTab = AdminTabType.Catalog;
            ValidateCurrentUserHasPermission(SystemPermissions.CatalogView);
        }

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            InitNavMenu(ucNavMenu);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            var category = HccApp.CatalogServices.Categories.Find(CategoryId);
            lnkViewInStore.NavigateUrl = UrlRewriter.BuildUrlForCategory(new CategorySnapshot(category));

            ucCategoryPerformance.CategoryId = CategoryId;

            PageMessageBox = ucMessageBox;
        }

        #endregion
    }
}