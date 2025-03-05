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
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Dnn.Utils;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    partial class Categories_Edit : BaseCategoryPage
    {
        #region Private Members

        private const string EMPTY_CATEGORY_PATTERN = @"(^$)|(^\.+$)";

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = Localization.GetString("PageTitle");
            InitNavMenu(ucNavMenu);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // TODO: troubleshoot taxonomy to see why it's not saving and loading
            TaxonomyBlock.Visible = false;

            if (!Page.IsPostBack)
            {
                PopulateCategories();
                PopulateTemplates();
                PopulateColumns();

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
                    ParentCategoryDropDownList.SelectedValue = parentId;
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
                ucMessageBox.ShowOk(Localization.GetString("SaveSuccess"));
                Response.Redirect(HccApp.CatalogServices.EditorRouteForCategory(CategorySourceType.Manual, CategoryId,
                    null));
            }
        }

        #endregion

        #region Implementation

        private void PopulateTemplates()
        {
            TemplateList.Items.Clear();
            TemplateList.Items.Add(new ListItem(Localization.GetString("NotSelected"), string.Empty));
            TemplateList.AppendDataBoundItems = true;
            TemplateList.DataSource = DnnPathHelper.GetViewNames("Category");
            TemplateList.DataBind();
        }

        private void PopulateColumns()
        {
            var columns = HccApp.ContentServices.Columns.FindAll();
            foreach (var col in columns)
            {
                PreContentColumnIdField.Items.Add(new ListItem(col.DisplayName, col.Bvin));
                PostContentColumnIdField.Items.Add(new ListItem(col.DisplayName, col.Bvin));
            }
        }

        private void PopulateCategories(string currentBvin = null)
        {
            // get a collection of categories to bind to the DLL (filtered to not allow assignment to children of current parent)
            var categories =
                HccApp.CatalogServices.Categories.FindAllSnapshotsPaged(1, int.MaxValue)
                    .Where(x => currentBvin == null || x.ParentId != currentBvin)
                    .ToList();
            var parents = CategoriesHelper.ListFullTreeWithIndents(categories, true);

            ParentCategoryDropDownList.Items.Clear();
            // iterate through each category and add to the DDL
            foreach (var category in parents)
            {
                // update the category name if it's empty
                category.Text = Regex.IsMatch(category.Text, EMPTY_CATEGORY_PATTERN)
                    ? string.Concat(category.Text, Localization.GetString("NoName"))
                    : category.Text;

                // let the merchant know visually that this is the current category they're editing
                if (category.Value == currentBvin)
                {
                    // change the name to reflect this
                    category.Text = string.Concat(Localization.GetString("CurrentCategory"), category.Text);
                }

                // create a new DLL item
                ParentCategoryDropDownList.Items.Add(category);
            }

            // add a default option for making the category a top-level category
            ParentCategoryDropDownList.Items.Insert(0, new ListItem(Localization.GetString("Parent_Default"), string.Empty));
        }

        private void LoadCategory(Category c)
        {
            // load the parent categories DDL
            PopulateCategories(c.Bvin);

            NameField.Text = c.Name;
            DescriptionField.Text = c.Description;
            MetaDescriptionField.Text = c.MetaDescription;
            MetaKeywordsField.Text = c.MetaKeywords;
            MetaTitleField.Text = c.MetaTitle;
            chkHidden.Checked = c.Hidden;

            if (ParentCategoryDropDownList.Items.FindByValue(c.ParentId) != null)
            {
                ParentCategoryDropDownList.ClearSelection();
                ParentCategoryDropDownList.Items.FindByValue(c.ParentId).Selected = true;
            }

            if (TemplateList.Items.FindByValue(c.TemplateName) != null)
            {
                TemplateList.ClearSelection();
                TemplateList.Items.FindByValue(c.TemplateName).Selected = true;
            }

            if (!string.IsNullOrWhiteSpace(c.PreContentColumnId))
            {
                if (PreContentColumnIdField.Items.FindByValue(c.PreContentColumnId) != null)
                {
                    PreContentColumnIdField.Items.FindByValue(c.PreContentColumnId).Selected = true;
                }
            }
            if (!string.IsNullOrWhiteSpace(c.PostContentColumnId))
            {
                if (PostContentColumnIdField.Items.FindByValue(c.PostContentColumnId) != null)
                {
                    PostContentColumnIdField.Items.FindByValue(c.PostContentColumnId).Selected = true;
                }
            }

            if (Enum.IsDefined(typeof (CategorySortOrder), c.DisplaySortOrder) &&
                c.DisplaySortOrder != CategorySortOrder.None)
            {
                SortOrderDropDownList.SelectedValue = ((int) c.DisplaySortOrder).ToString();
            }
            else
            {
                SortOrderDropDownList.SelectedValue = ((int) CategorySortOrder.ManualOrder).ToString();
            }

            RewriteUrlField.Text = c.RewriteUrl;
            chkShowTitle.Checked = c.ShowTitle;
            keywords.Text = c.Keywords;
            txtTaxonomyTags.Text = string.Join(",", HccApp.SocialService.GetTaxonomyTerms(c));

            CategoryBreadCrumbTrail1.LoadTrail(c.Bvin);
            UrlsAssociated1.ObjectId = c.Bvin;
            UrlsAssociated1.LoadUrls();

            lnkViewInStore.NavigateUrl = UrlRewriter.BuildUrlForCategory(new CategorySnapshot(c));
            ucIconImage.ImageUrl = DiskStorage.CategoryIconUrl(HccApp, c.Bvin, c.ImageUrl,
                HccApp.IsCurrentRequestSecure());
            ucBannerImage.ImageUrl = DiskStorage.CategoryBannerUrl(HccApp, c.Bvin, c.BannerImageUrl,
                HccApp.IsCurrentRequestSecure());
        }

        private bool Save()
        {
            var result = false;

            var c = HccApp.CatalogServices.Categories.Find(CategoryId) ?? new Category {ParentId = ParentCategoryId};
            var isNewCategory = string.IsNullOrEmpty(CategoryId);

            // ensure that the current category is not a parent of itself
            if (ParentCategoryDropDownList.SelectedValue != c.Bvin || isNewCategory)
            {
                // only update the sort if this is not a new category & the parent is different
                if (!isNewCategory && c.ParentId != ParentCategoryDropDownList.SelectedValue)
                {
                    // update the sort to be at the bottom of the existing child categories
                    c.SortOrder = HccApp.CatalogServices.Categories.FindMaxSort(ParentCategoryDropDownList.SelectedValue);
                }

                // update the category to match the selected parent category
                c.ParentId = ParentCategoryDropDownList.SelectedValue;
            }
            else
            {
                ucMessageBox.ShowWarning(Localization.GetString("SelfSaveError"));
                return false;
            }

            c.Name = NameField.Text.Trim();
            c.Description = DescriptionField.Text.Trim();
            c.MetaDescription = MetaDescriptionField.Text.Trim();
            c.MetaTitle = MetaTitleField.Text.Trim();
            c.MetaKeywords = MetaKeywordsField.Text.Trim();
            c.ShowInTopMenu = false;
            c.Hidden = chkHidden.Checked;

            c.SourceType = CategorySourceType.Manual;
            c.TemplateName = TemplateList.SelectedItem.Value;
            c.PreContentColumnId = PreContentColumnIdField.SelectedValue;
            c.PostContentColumnId = PostContentColumnIdField.SelectedValue;
            c.DisplaySortOrder = (CategorySortOrder) int.Parse(SortOrderDropDownList.SelectedValue);

            var oldUrl = c.RewriteUrl;

            // no entry, generate one
            if (string.IsNullOrWhiteSpace(RewriteUrlField.Text))
            {
                c.RewriteUrl = Text.Slugify(c.Name, true);
            }
            else
            {
                c.RewriteUrl = Text.Slugify(RewriteUrlField.Text, true);
            }

            RewriteUrlField.Text = c.RewriteUrl;

            if (UrlRewriter.IsCategorySlugInUse(c.RewriteUrl, c.Bvin, HccApp))
            {
                ucMessageBox.ShowWarning(Localization.GetString("DuplicateUrlError"));
                return false;
            }

            c.ShowTitle = chkShowTitle.Checked;
            c.Keywords = keywords.Text.Trim();

            if (isNewCategory)
            {
                result = HccApp.CatalogServices.Categories.Create(c);
            }

            if (!SaveImages(c))
                return false;

            result = HccApp.CatalogServices.CategoryUpdate(c);

            var taxonomyTags = txtTaxonomyTags.Text.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries);
            HccApp.SocialService.UpdateCategoryTaxonomy(c, taxonomyTags);

            if (result)
            {
                // Update bvin field so that next save will call updated instead of create
                CategoryId = c.Bvin;

                if (oldUrl != string.Empty)
                {
                    if (oldUrl != c.RewriteUrl)
                    {
                        HccApp.ContentServices.CustomUrls.Register301(oldUrl, c.RewriteUrl,
                            c.Bvin, CustomUrlType.Category, HccApp.CurrentRequestContext, HccApp);
                        UrlsAssociated1.LoadUrls();
                    }
                }
            }
            else
            {
                ucMessageBox.ShowError(Localization.GetString("CategorySaveFailure"));
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
                    ucMessageBox.ShowError(Localization.GetString("ImageSaveFailure"));
                }
            }
            else if (ucIconImage.Removed)
            {
                c.ImageUrl = string.Empty;
            }

            // Banner Image Upload
            if (ucBannerImage.HasFile)
            {
                var fileName = Text.CleanFileName(Path.GetFileName(ucBannerImage.FileName));

                if (DiskStorage.CopyCategoryBanner(HccApp.CurrentStore.Id, c.Bvin, ucBannerImage.TempImagePath, fileName))
                {
                    c.BannerImageUrl = fileName;
                }
                else
                {
                    result = false;
                    ucMessageBox.ShowError(Localization.GetString("ImageSaveFailure"));
                }
            }
            else if (ucBannerImage.Removed)
            {
                c.BannerImageUrl = string.Empty;
            }

            return result;
        }

        #endregion
    }
}