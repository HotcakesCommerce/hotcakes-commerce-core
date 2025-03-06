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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Controls
{
    public partial class CategoryPerformance : HccUserControl
    {
        public string CategoryId { get; set; }

        public bool EditMode { get; set; }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(CategoryId))
            {
                var categoryRepo = Factory.CreateRepo<CategoryRepository>();
                var category = categoryRepo.Find(CategoryId);
                if (category != null)
                {
                    var currentStore = HccRequestContext.Current.CurrentStore;

                    lblCategoryName.Text = category.Name;
                    lblLastModifiedOn.Text =
                        DateHelper.ConvertUtcToStoreTime(currentStore, category.LastUpdatedUtc).ToString("MMM dd, yyyy");
                    var performanceUserSelections = new PerformanceUserSelections();
                    ddlRowPeriod.SelectedValue = performanceUserSelections.CategoriesPerformacePeriod.ToString();
                }
            }
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            txtCategorytId.Value = CategoryId;

            var urlTemplate = "~/DesktopModules/Hotcakes/Core/Admin/Catalog/Categories_Edit.aspx";

            btnAddCategory.NavigateUrl = urlTemplate;
            btnEditCategory.NavigateUrl = string.Concat(urlTemplate, "?id=", CategoryId);

            btnAddCategory.Visible = !EditMode;
            btnEditCategory.Visible = EditMode;
        }
    }
}