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
using System.IO;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Storage;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    public partial class Categories_EditCustomLink : BaseCategoryPage
    {
        #region Event handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = "Edit Custom Link Category";
            InitNavMenu(ucNavMenu, true);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                NameField.Focus();

                var categoryId = CategoryId;
                var parentId = ParentCategoryId;

                if (!string.IsNullOrEmpty(categoryId))
                {
                    var category = HccApp.CatalogServices.Categories.Find(categoryId);
                    if (category == null)
                    {
                        EventLog.LogEvent("Edit Category Page", "Could not find category with bvin " + categoryId,
                            EventLogSeverity.Warning);
                        Response.Redirect("Categories.aspx");
                    }

                    LoadCategory(category);
                }
                else if (!string.IsNullOrEmpty(parentId))
                {
                    CategoryBreadCrumbTrail1.LoadTrail(parentId);
                }
            }
        }

        protected void btnCancel_Click(object sender, EventArgs e)
        {
            Response.Redirect("Categories.aspx?id=" + ParentCategoryId);
        }

        protected void btnSaveChanges_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                Response.Redirect("Categories.aspx?id=" + ParentCategoryId);
            }
        }

        protected void UpdateButton_Click(object sender, EventArgs e)
        {
            if (Save())
            {
                ucMessageBox.ShowOk("Category Updated Successfully.");
                Response.Redirect(HccApp.CatalogServices.EditorRouteForCategory(CategorySourceType.CustomLink,
                    CategoryId, null));
            }
            else
            {
                ucMessageBox.ShowError("Error during update. Please check event log.");
            }
        }

        #endregion

        #region Implementation

        private void LoadCategory(Category c)
        {
            NameField.Text = c.Name;
            LinkToField.Text = c.CustomPageUrl;
            MetaTitleField.Text = c.MetaTitle;
            chkHidden.Checked = c.Hidden;

            ucIconImage.ImageUrl = DiskStorage.CategoryIconUrl(HccApp, c.Bvin, c.ImageUrl,
                HccApp.IsCurrentRequestSecure());
        }

        private bool Save()
        {
            var c = HccApp.CatalogServices.Categories.Find(CategoryId)
                    ?? new Category {ParentId = ParentCategoryId};
            return Save(c);
        }

        private bool Save(Category c)
        {
            var result = false;

            if (c != null)
            {
                c.Name = NameField.Text.Trim();
                c.MetaTitle = MetaTitleField.Text.Trim();
                c.CustomPageUrl = LinkToField.Text.Trim();
                c.ShowInTopMenu = false;
                c.Hidden = chkHidden.Checked;

                c.SourceType = CategorySourceType.CustomLink;

                if (string.IsNullOrEmpty(CategoryId))
                {
                    c.ParentId = ParentCategoryId;
                    result = HccApp.CatalogServices.Categories.Create(c);
                }

                if (!SaveImages(c))
                    return false;

                result = HccApp.CatalogServices.CategoryUpdate(c);

                if (result)
                {
                    // Update bvin field so that next save will call updated instead of create
                    CategoryId = c.Bvin;
                }
                else
                {
                    ucMessageBox.ShowError("Unable to save category. Unknown error.");
                }
            }

            return result;
        }

        private bool SaveImages(Category c)
        {
            var result = true;

            // Icon Image Upload
            if (ucIconImage.HasFile)
            {
                var fileName = Text.CleanFileName(Path.GetFileName(ucIconImage.FileName));

                if (DiskStorage.CopyCategoryIcon(HccApp.CurrentStore.Id, c.Bvin, ucIconImage.TempImagePath, fileName))
                {
                    c.ImageUrl = fileName;
                }
                else
                {
                    result = false;
                    ucMessageBox.ShowError("Only .PNG, .JPG, .GIF file types are allowed for icon images");
                }
            }
            else if (ucIconImage.Removed)
            {
                c.ImageUrl = string.Empty;
            }

            return result;
        }

        #endregion
    }
}