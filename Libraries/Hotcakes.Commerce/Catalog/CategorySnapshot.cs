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

using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    public class CategorySnapshot
    {
        private string _RewriteUrl = string.Empty;

        public CategorySnapshot()
        {
            Bvin = string.Empty;
            StoreId = 0;
            ParentId = string.Empty;
            Name = string.Empty;
            SourceType = CategorySourceType.Manual;
            ImageUrl = string.Empty;
            CustomPageUrl = string.Empty;
            ShowInTopMenu = false;
            Hidden = false;
            SortOrder = 0;
            MetaTitle = string.Empty;
        }

        public CategorySnapshot(Category cat)
        {
            Bvin = cat.Bvin;
            StoreId = cat.StoreId;
            ParentId = cat.ParentId;
            Name = cat.Name;
            Description = cat.Description;
            SourceType = cat.SourceType;
            ImageUrl = cat.ImageUrl;
            BannerImageUrl = cat.BannerImageUrl;
            CustomPageUrl = cat.CustomPageUrl;
            ShowInTopMenu = cat.ShowInTopMenu;
            Hidden = cat.Hidden;
            SortOrder = cat.SortOrder;
            MetaTitle = cat.MetaTitle;
            RewriteUrl = cat.RewriteUrl;
        }

        /// <summary>
        ///     This is the ID of the category.
        /// </summary>
        public string Bvin { get; set; }

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
        ///     Allows you to define whether your category is a typical category or a placeholder link to another resource.
        /// </summary>
        /// <remarks>Always use the CategorySourceType enum for this property.</remarks>
        public CategorySourceType SourceType { get; set; }

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
        ///     This is the slug of the URL, or the last part of the URL to be used to get to this category's landing page. It must
        ///     be unique. If empty, the application will create one based upon the name of the category.
        /// </summary>
        public string RewriteUrl
        {
            get { return _RewriteUrl; }
            set { _RewriteUrl = value.Trim().ToLowerInvariant(); }
        }

        /// <summary>
        ///     Allows you to define how the products in this category will be ordered.
        /// </summary>
        /// <remarks>Always use the CategorySortOrderDTO enum for this property.</remarks>
        public int SortOrder { get; set; }

        /// <summary>
        ///     This title is used to adjust the title in the source code of the category landing page for SEO.
        /// </summary>
        public string MetaTitle { get; set; }


        /// <summary>
        ///     Allows you to convert the current CategorySnapshot object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of CategorySnapshotDTO</returns>
        public CategorySnapshotDTO ToDto()
        {
            var dto = new CategorySnapshotDTO();

            dto.Bvin = Bvin;
            dto.CustomPageOpenInNewWindow = CustomPageOpenInNewWindow;
            dto.CustomPageUrl = CustomPageUrl;
            dto.Hidden = Hidden;
            dto.ImageUrl = ImageUrl;
            dto.BannerImageUrl = BannerImageUrl;
            dto.Name = Name;
            dto.Description = Description;
            dto.ParentId = ParentId;
            dto.RewriteUrl = RewriteUrl;
            dto.ShowInTopMenu = ShowInTopMenu;
            dto.SourceType = (CategorySourceTypeDTO) (int) SourceType;
            dto.StoreId = StoreId;
            dto.SortOrder = SortOrder;
            dto.MetaTitle = MetaTitle;
            return dto;
        }
    }
}