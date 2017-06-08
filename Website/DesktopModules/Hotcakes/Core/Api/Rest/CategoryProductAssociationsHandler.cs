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

using System;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for category/product association management
    /// </summary>
    [Serializable]
    public class CategoryProductAssociationsHandler : BaseRestHandler
    {
        public CategoryProductAssociationsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return a single category/product associations to the application that called
        ///     it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected querystring
        ///     keys/values is the category/product association ID or Bvin.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     CategoryProductAssociationDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            // Find One Specific category/product association
            var response = new ApiResponse<CategoryProductAssociationDTO>();

            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);

            var item = HccApp.CatalogServices.CategoriesXProducts.Find(id);
            if (item == null)
            {
                response.Errors.Add(new ApiError("NULL", "Could not locate that item. Check bvin and try again."));
            }
            else
            {
                response.Content = item.ToDto();
            }
            data = Json.ObjectToJson(response);

            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update a category/product association.
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. Only one parameter is expected, which is
        ///     the ID of the category/product association, but only when updating.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the CategoryProductAssociationDTO object</param>
        /// <returns>CategoryProductAssociationDTO - Serialized (JSON) version of the category/product association</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);

            var response = new ApiResponse<CategoryProductAssociationDTO>();

            CategoryProductAssociationDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<CategoryProductAssociationDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new CategoryProductAssociation();
            item.FromDto(postedItem);

            if (id < 1)
            {
                item = HccApp.CatalogServices.AddProductToCategory(item.ProductId, item.CategoryId);
            }
            else
            {
                HccApp.CatalogServices.CategoriesXProducts.Update(item);
            }

            if (item != null)
                response.Content = item.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a category/product association from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (category/product
        ///     association ID/bvin) is expected.
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
            var categoryBvin = GetParameterByIndex(1, parameters);
            var response = new ApiResponse<bool>
            {
                Content = HccApp.CatalogServices.RemoveProductFromCategory(bvin, categoryBvin)
            };

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}