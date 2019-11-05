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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary object that is used in the REST API to manage categories.
    /// </summary>
    /// <remarks>This object is used to populate the Category object.</remarks>
    [DataContract]
    [Serializable]
    public class CategoryDTO
    {
        //public List<CategoryPageVersionDTO> Versions { get; set; }

        public CategoryDTO()
        {
            Bvin = string.Empty;
            StoreId = 0;
            LastUpdatedUtc = DateTime.UtcNow;
            ParentId = string.Empty;
            Name = string.Empty;
            Description = string.Empty;
            DisplaySortOrder = CategorySortOrderDTO.ManualOrder;
            SourceType = CategorySourceTypeDTO.Manual;
            SortOrder = 0;
            MetaKeywords = string.Empty;
            MetaDescription = string.Empty;
            MetaTitle = string.Empty;
            ImageUrl = string.Empty;
            BannerImageUrl = string.Empty;
            CustomPageUrl = string.Empty;
            ShowInTopMenu = false;
            Hidden = false;
            TemplateName = "Index";
            PreContentColumnId = string.Empty;
            PostContentColumnId = string.Empty;
            ShowTitle = true;
            Keywords = string.Empty;
            RewriteUrl = string.Empty;

            //this.Versions = new List<CategoryPageVersionDTO>();

            Operations = new List<ApiOperation>();
        }

        /// <summary>
        ///     This is the ID of the category.
        /// </summary>
        [DataMember]
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the category was last updated.
        /// </summary>
        [DataMember]
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     Having an ID here will make this category a child or nested category of the category that matches this ID. This
        ///     helps to create nested navigation and other features.
        /// </summary>
        [DataMember]
        public string ParentId { get; set; }

        /// <summary>
        ///     This is the name of the category that the customers will see in their views.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        ///     If the description exists, it will be placed below the category banner.
        /// </summary>
        [DataMember]
        public string Description { get; set; }

        /// <summary>
        ///     Except when overridden by a “sort” querystring parameter, this value determines how the products in the category
        ///     will be sorted.
        /// </summary>
        [DataMember]
        public CategorySortOrderDTO DisplaySortOrder { get; set; }

        /// <summary>
        ///     Allows you to define whether your category is a typical category or a placeholder link to another resource.
        /// </summary>
        /// <remarks>Always use the CategorySourceType enum for this property.</remarks>
        [DataMember]
        public CategorySourceTypeDTO SourceType { get; set; }

        /// <summary>
        ///     Allows you to define how the products in this category will be ordered.
        /// </summary>
        /// <remarks>Always use the CategorySortOrderDTO enum for this property.</remarks>
        [DataMember]
        public int SortOrder { get; set; }

        /// <summary>
        ///     These keywords are used to adjust the keywords in the source code of the category landing page for SEO.
        /// </summary>
        [DataMember]
        public string MetaKeywords { get; set; }

        /// <summary>
        ///     This description is used to adjust the description in the source code of the category landing page for SEO.
        /// </summary>
        [DataMember]
        public string MetaDescription { get; set; }

        /// <summary>
        ///     This title is used to adjust the title in the source code of the category landing page for SEO.
        /// </summary>
        [DataMember]
        public string MetaTitle { get; set; }

        /// <summary>
        ///     This is the image of the category that you want associated with it in the various views. It also is used to
        ///     generate the category thumbnail.
        /// </summary>
        [DataMember]
        public string ImageUrl { get; set; }

        /// <summary>
        ///     If populated with a URL, the specified banner will be displayed in the category header.
        /// </summary>
        [DataMember]
        public string BannerImageUrl { get; set; }

        /// <summary>
        ///     If populated with a URL, this value will be used as the URL for the category when clicked.
        /// </summary>
        [DataMember]
        public string CustomPageUrl { get; set; }

        /// <summary>
        ///     If true and if using a category as a custom link, this will cause the link to be opened in a new window when the
        ///     customer clicks on it. This is only used in the CategoryRotator content block.
        /// </summary>
        [DataMember]
        public bool CustomPageOpenInNewWindow { get; set; }

        /// <summary>
        ///     If true, this category will be shown in the initial list of categories in category lists.
        /// </summary>
        [DataMember]
        public bool ShowInTopMenu { get; set; }

        /// <summary>
        ///     Except when overridden by a “sort” querystring parameter, this value determines how the products in the category
        ///     will be sorted.
        /// </summary>
        /// <remarks>This needs to be tested. It doesn't appear to be referenced anywhere.</remarks>
        [DataMember]
        public bool Hidden { get; set; }

        /// <summary>
        ///     Allows you to specify a specify view in your viewset to be associated with this category when customers see it.
        /// </summary>
        [DataMember]
        public string TemplateName { get; set; }

        /// <summary>
        ///     Contains the ID of the content block that you want to use in the header area of the category.
        /// </summary>
        [DataMember]
        public string PreContentColumnId { get; set; }

        /// <summary>
        ///     Contains the ID of the content block that you want to use in the footer area of the category.
        /// </summary>
        [DataMember]
        public string PostContentColumnId { get; set; }

        /// <summary>
        ///     If true, the category name is shown in the customer-facing views.
        /// </summary>
        [DataMember]
        public bool ShowTitle { get; set; }

        /// <summary>
        ///     These keywords are additional keywords that can be used to further enhance your onsite search to find products in
        ///     this category.
        /// </summary>
        /// <remarks>These keywords are currently not being used anywhere</remarks>
        [DataMember]
        public string Keywords { get; set; }

        /// <summary>
        ///     This is the slug of the URL, or the last part of the URL to be used to get to this category's landing page. It must
        ///     be unique. If empty, the application will create one based upon the name of the category.
        /// </summary>
        [DataMember]
        public string RewriteUrl { get; set; }

        /// <summary>
        ///     Operations allow you to define external API end points that can be used to manage the category.
        /// </summary>
        /// <remarks>This is not currently used at all.</remarks>
        [DataMember]
        public List<ApiOperation> Operations { get; set; }
    }
}