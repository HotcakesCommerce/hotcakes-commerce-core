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
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of Category
    /// </summary>
    /// <remarks>The REST API equivalent is CategoryDTO. There is also a CategorySnapshot that is used for better performance.</remarks>
    [Serializable]
    public class Category
    {
        public Category()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            ParentId = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            DisplaySortOrder = CategorySortOrder.ManualOrder;
            SourceType = CategorySourceType.Manual;
            SortOrder = 0;
            MetaKeywords = string.Empty;
            MetaDescription = string.Empty;
            MetaTitle = string.Empty;
            ImageUrl = string.Empty;
            BannerImageUrl = string.Empty;
            CustomPageUrl = string.Empty;
            ShowInTopMenu = false;
            Hidden = false;
            TemplateName = string.Empty;
            PreContentColumnId = string.Empty;
            PostContentColumnId = string.Empty;
            ShowTitle = true;
            Keywords = string.Empty;
        }

        #region Properties

        /// <summary>
        ///     This is the ID of the category.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     The creation date is used for auditing purposes to know when the category was created.
        /// </summary>
        public DateTime CreationDateUtc { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the category was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     Having an ID here will make this category a child or nested category of the category that matches this ID. This
        ///     helps to create nested navigation and other features.
        /// </summary>
        public string ParentId { get; set; }

        /// <summary>
        ///     This is the name of the category that the customers will see in their views.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        ///     If the description exists, it will be placed below the category banner.
        /// </summary>
        public string Description { get; set; }

        /// <summary>
        ///     Except when overridden by a “sort” querystring parameter, this value determines how the products in the category
        ///     will be sorted.
        /// </summary>
        public CategorySortOrder DisplaySortOrder { get; set; }

        /// <summary>
        ///     Allows you to define whether your category is a typical category or a placeholder link to another resource.
        /// </summary>
        /// <remarks>Always use the CategorySourceType enum for this property.</remarks>
        public CategorySourceType SourceType { get; set; }

        /// <summary>
        ///     Allows you to define how the products in this category will be ordered.
        /// </summary>
        /// <remarks>Always use the CategorySortOrderDTO enum for this property.</remarks>
        public int SortOrder { get; set; }

        /// <summary>
        ///     These keywords are used to adjust the keywords in the source code of the category landing page for SEO.
        /// </summary>
        public string MetaKeywords { get; set; }

        /// <summary>
        ///     This description is used to adjust the description in the source code of the category landing page for SEO.
        /// </summary>
        public string MetaDescription { get; set; }

        /// <summary>
        ///     This title is used to adjust the title in the source code of the category landing page for SEO.
        /// </summary>
        public string MetaTitle { get; set; }

        /// <summary>
        ///     This is the image of the category that you want associated with it in the various views. It also is used to
        ///     generate the category thumbnail.
        /// </summary>
        public string ImageUrl { get; set; }

        /// <summary>
        ///     If populated with a URL, the specified banner will be displayed in the category header.
        /// </summary>
        public string BannerImageUrl { get; set; }

        /// <summary>
        ///     If populated with a URL, this value will be used as the URL for the category when clicked.
        /// </summary>
        public string CustomPageUrl { get; set; }

        /// <summary>
        ///     If true and if using a category as a custom link, this will cause the link to be opened in a new window when the
        ///     customer clicks on it. This is only used in the CategoryRotator content block.
        /// </summary>
        public bool CustomPageOpenInNewWindow { get; set; }

        /// <summary>
        ///     If true, this category will be shown in the initial list of categories in category lists.
        /// </summary>
        public bool ShowInTopMenu { get; set; }

        /// <summary>
        ///     Except when overridden by a “sort” querystring parameter, this value determines how the products in the category
        ///     will be sorted.
        /// </summary>
        /// <remarks>This needs to be tested. It doesn't appear to be referenced anywhere.</remarks>
        public bool Hidden { get; set; }

        /// <summary>
        ///     Allows you to specify a specify view in your viewset to be associated with this category when customers see it.
        /// </summary>
        public string TemplateName { get; set; }

        /// <summary>
        ///     Contains the ID of the content block that you want to use in the header area of the category.
        /// </summary>
        public string PreContentColumnId { get; set; }

        /// <summary>
        ///     Contains the ID of the content block that you want to use in the footer area of the category.
        /// </summary>
        public string PostContentColumnId { get; set; }

        /// <summary>
        ///     If true, the category name is shown in the customer-facing views.
        /// </summary>
        public bool ShowTitle { get; set; }

        /// <summary>
        ///     These keywords are additional keywords that can be used to further enhance your onsite search to find products in
        ///     this category.
        /// </summary>
        /// <remarks>These keywords are currently not being used anywhere</remarks>
        public string Keywords { get; set; }

        private string _RewriteUrl = string.Empty;

        /// <summary>
        ///     This is the slug of the URL, or the last part of the URL to be used to get to this category's landing page. It must
        ///     be unique. If empty, the application will create one based upon the name of the category.
        /// </summary>
        public string RewriteUrl
        {
            get { return _RewriteUrl; }
            set { _RewriteUrl = value.Trim().ToLowerInvariant(); }
        }

        #endregion

        #region  Search Within List Functions

        /// <summary>
        ///     Allows you to get a listing of all of the categories that are children of the specified parent category.
        /// </summary>
        /// <param name="allCats">A list of CategorySnapshot that we want to search through</param>
        /// <param name="parentId">The GUID (bvin) of the parent category that we want to find children for</param>
        /// <returns>A list of CategorySnapshot</returns>
        /// <remarks>Assumes that you do not want to show categories that have been marked as hidden.</remarks>
        public static List<CategorySnapshot> FindChildrenInList(List<CategorySnapshot> allCats, string parentId)
        {
            return FindChildrenInList(allCats, parentId, false);
        }

        /// <summary>
        ///     Allows you to get a listing of all of the categories that are children of the specified parent category.
        /// </summary>
        /// <param name="allCats">A list of CategorySnapshot that we want to search through</param>
        /// <param name="parentId">The GUID (bvin) of the parent category that we want to find children for</param>
        /// <param name="showHidden"></param>
        /// <returns></returns>
        public static List<CategorySnapshot> FindChildrenInList(List<CategorySnapshot> allCats, string parentId,
            bool showHidden)
        {
            var results = new List<CategorySnapshot>();

            foreach (var c in allCats)
            {
                if (c.ParentId == parentId)
                {
                    if (c.Hidden == false || showHidden)
                    {
                        results.Add(c);
                    }
                }
            }

            results = results.OrderBy(y => y.SortOrder).ToList();
            return results;
        }

        /// <summary>
        ///     Allows you to find a specific category in the list that matches the category ID (bvin)
        /// </summary>
        /// <param name="allCats">A list of CategorySnapshot that we want to search through</param>
        /// <param name="bvin">The category ID to look for</param>
        /// <returns>A single instance of CategorySnapshot</returns>
        public static CategorySnapshot FindInList(List<CategorySnapshot> allCats, string bvin)
        {
            foreach (var cat in allCats)
            {
                if (cat.Bvin == bvin)
                {
                    return cat;
                }
            }
            return null;
        }

        #endregion

        #region Breadcrumb Functions

        /// <summary>
        ///     Builds a breadscrumb trail for all child categories related to the given category ID
        /// </summary>
        /// <param name="categoryId">The category ID (bvin) that we want to build a breadcrumb off of</param>
        /// <param name="repo">An instance of the data that contains the categories</param>
        /// <returns></returns>
        public static List<CategorySnapshot> BuildTrailToRoot(string categoryId, ICategoryRepository repo)
        {
            var result = new List<CategorySnapshot>();

            var allCats = repo.FindAllSnapshotsPaged(1, int.MaxValue);

            BuildParentTrail(ref allCats, categoryId, ref result);

            return result;
        }

        /// <summary>
        ///     Builds a breadcrumb of categories that match the specified category ID
        /// </summary>
        /// <param name="allCats">A list of CategorySnapshot that we want to search through</param>
        /// <param name="currentId">The category ID (bvin) that we want to build a breadcrumb off of</param>
        /// <param name="trail">The list of the categories that make up the breadcrumb trail</param>
        public static void BuildParentTrail(ref List<CategorySnapshot> allCats, string currentId,
            ref List<CategorySnapshot> trail)
        {
            if (string.IsNullOrEmpty(currentId))
                return;

            var current = FindInList(allCats, currentId);

            if (current != null)
            {
                trail.Add(current);
                if (string.IsNullOrEmpty(current.ParentId))
                    return;

                BuildParentTrail(ref allCats, current.ParentId, ref trail);
            }
        }

        #endregion

        #region DTO

        /// <summary>
        ///     Allows you to populate the current category object using a CategoryDTO instance
        /// </summary>
        /// <param name="dto">An instance of the category from the REST API</param>
        public void FromDto(CategoryDTO dto)
        {
            if (dto == null) return;

            BannerImageUrl = dto.BannerImageUrl ?? string.Empty;
            Bvin = dto.Bvin ?? string.Empty;
            CustomPageOpenInNewWindow = dto.CustomPageOpenInNewWindow;
            CustomPageUrl = dto.CustomPageUrl ?? string.Empty;
            Description = dto.Description ?? string.Empty;
            DisplaySortOrder = (CategorySortOrder) (int) dto.DisplaySortOrder;
            Hidden = dto.Hidden;
            ImageUrl = dto.ImageUrl ?? string.Empty;
            Keywords = dto.Keywords ?? string.Empty;
            LastUpdatedUtc = dto.LastUpdatedUtc;
            MetaDescription = dto.MetaDescription ?? string.Empty;
            MetaKeywords = dto.MetaKeywords ?? string.Empty;
            MetaTitle = dto.MetaTitle ?? string.Empty;
            Name = dto.Name ?? string.Empty;
            ParentId = dto.ParentId ?? string.Empty;
            PostContentColumnId = dto.PostContentColumnId ?? string.Empty;
            PreContentColumnId = dto.PreContentColumnId ?? string.Empty;
            RewriteUrl = dto.RewriteUrl ?? string.Empty;
            ShowInTopMenu = dto.ShowInTopMenu;
            ShowTitle = dto.ShowTitle;
            SortOrder = dto.SortOrder;
            SourceType = (CategorySourceType) (int) dto.SourceType;
            StoreId = dto.StoreId;
            TemplateName = dto.TemplateName ?? string.Empty;
        }

        /// <summary>
        ///     Allows you to convert the current category object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of CategoryDTO</returns>
        public CategoryDTO ToDto()
        {
            var dto = new CategoryDTO();

            dto.BannerImageUrl = BannerImageUrl;
            dto.Bvin = Bvin;
            dto.CustomPageOpenInNewWindow = CustomPageOpenInNewWindow;
            dto.CustomPageUrl = CustomPageUrl;
            dto.Description = Description;
            dto.DisplaySortOrder = (CategorySortOrderDTO) (int) DisplaySortOrder;
            dto.Hidden = Hidden;
            dto.ImageUrl = ImageUrl;
            dto.Keywords = Keywords;
            dto.LastUpdatedUtc = LastUpdatedUtc;
            dto.MetaDescription = MetaDescription;
            dto.MetaKeywords = MetaKeywords;
            dto.MetaTitle = MetaTitle;
            dto.Name = Name;
            dto.ParentId = ParentId;
            dto.PostContentColumnId = PostContentColumnId;
            dto.PreContentColumnId = PreContentColumnId;
            dto.RewriteUrl = RewriteUrl;
            dto.ShowInTopMenu = ShowInTopMenu;
            dto.ShowTitle = ShowTitle;
            dto.SortOrder = SortOrder;
            dto.SourceType = (CategorySourceTypeDTO) (int) SourceType;
            dto.StoreId = StoreId;
            dto.TemplateName = TemplateName;

            return dto;
        }

        #endregion
    }
}