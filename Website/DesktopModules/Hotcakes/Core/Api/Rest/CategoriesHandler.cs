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
using System.Collections.Generic;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for category management
    /// </summary>
    [Serializable]
    public class CategoriesHandler : BaseRestHandler
    {
        public CategoriesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many categories to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected keys will be byproduct
        ///     and byslug.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     CategoryDTO or list of CategorySnapshotDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var byproduct = querystring["byproduct"] ?? string.Empty;
                var response = new ApiResponse<List<CategorySnapshotDTO>>();

                var results = new List<CategorySnapshot>();

                if (byproduct.Trim().Length > 0)
                {
                    results = HccApp.CatalogServices.FindCategoriesForProduct(byproduct);
                }
                else
                {
                    results = HccApp.CatalogServices.Categories.FindAll();
                }

                var dto = new List<CategorySnapshotDTO>();

                foreach (var cat in results)
                {
                    if (string.IsNullOrEmpty(cat.CustomPageUrl))
                        cat.CustomPageUrl = UrlRewriter.BuildUrlForCategory(cat);
                    dto.Add(cat.ToDto());
                }
                response.Content = dto;
                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find One Specific Category
                var response = new ApiResponse<CategoryDTO>();
                var bvin = FirstParameter(parameters);
                var byslug = querystring["byslug"] ?? string.Empty;

                Category c = null;

                if (byslug.Trim().Length > 0)
                {
                    c = HccApp.CatalogServices.Categories.FindBySlug(byslug);
                }
                else
                {
                    c = HccApp.CatalogServices.Categories.Find(bvin);
                }

                if (c == null)
                {
                    response.Errors.Add(new ApiError("NULL", "Could not locate that category. Check bvin and try again."));
                }
                else
                {
                    if (string.IsNullOrEmpty(c.CustomPageUrl))
                        c.CustomPageUrl = UrlRewriter.BuildUrlForCategory(new CategorySnapshot(c));
                    response.Content = c.ToDto();
                }
                data = Json.ObjectToJson(response);
            }

            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update a category
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the category ID (bvin) and that this is an update, otherwise it assumes to create
        ///     a category.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the CategoryDTO object</param>
        /// <returns>CategoryDTO - Serialized (JSON) version of the category</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<CategoryDTO>();

            CategoryDTO postedCategory = null;
            try
            {
                postedCategory = Json.ObjectFromJson<CategoryDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var c = new Category();
            c.FromDto(postedCategory);
            Category resultCategory = null;
            if (bvin == string.Empty)
            {
                if (HccApp.CatalogServices.Categories.Create(c))
                {
                    bvin = c.Bvin;
                }
            }
            else
            {
                resultCategory = HccApp.CatalogServices.Categories.Find(bvin);
                c.CreationDateUtc = resultCategory.CreationDateUtc;
                HccApp.CatalogServices.CategoryUpdate(c);
            }

            resultCategory = HccApp.CatalogServices.Categories.Find(bvin);
            if (resultCategory != null) response.Content = resultCategory.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a category from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (category ID/bvin) is
        ///     expected.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. Not used in this method.</param>
        /// <param name="postdata">This parameter is not used in this method</param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain either True or False,
        ///     depending on success of the deletion
        /// </returns>
        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<bool>();

            if (bvin == string.Empty)
            {
                // Clear Requested
                response.Content = HccApp.DestroyAllCategories();
            }
            else
            {
                // Single Item Delete
                response.Content = HccApp.CatalogServices.Categories.Delete(bvin);
            }

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}