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
using System.Collections.Specialized;
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for product relationship management
    /// </summary>
    [Serializable]
    public class ProductRelationshipsHandler : BaseRestHandler
    {
        public ProductRelationshipsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many product relationships to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. This end point does not expect any
        ///     querystring values.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     the ProductRelationshipDTO object.
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                var productBvin = querystring["byproduct"] ?? string.Empty;
                // List
                var response = new ApiResponse<List<ProductRelationshipDTO>>();

                var items = HccApp.CatalogServices.ProductRelationships.FindForProduct(productBvin);
                response.Content = items.Select(r => r.ToDto()).ToList();

                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find One Specific Category
                var response = new ApiResponse<ProductRelationshipDTO>();
                var ids = FirstParameter(parameters);
                long id = 0;
                long.TryParse(ids, out id);

                var item = HccApp.CatalogServices.ProductRelationships.Find(id);

                if (item == null)
                {
                    response.Errors.Add(new ApiError("NULL", "Could not locate that item. Check bvin and try again."));
                }
                else
                {
                    response.Content = item.ToDto();
                }

                data = Json.ObjectToJson(response);
            }
            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update a product relationship
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product relationship ID (bvin) and that this is an update, otherwise it
        ///     assumes to create a product relationship.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductRelationshipDTO object</param>
        /// <returns>ProductRelationshipDTO - Serialized (JSON) version of the product relationship</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);

            var response = new ApiResponse<ProductRelationshipDTO>();

            ProductRelationshipDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<ProductRelationshipDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new ProductRelationship();
            item.FromDto(postedItem);

            if (id < 1)
            {
                if (HccApp.CatalogServices.ProductRelationships.Create(item))
                {
                    id = item.Id;
                }
            }
            else
            {
                HccApp.CatalogServices.ProductRelationships.Update(item);
            }

            var resultItem = HccApp.CatalogServices.ProductRelationships.Find(id);

            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a product relationship from the store catalog
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product relationship
        ///     ID/bvin) is expected.
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
            var bvin = FirstParameter(parameters); // the primary product id
            var otherBvin = GetParameterByIndex(1, parameters); // the related product id
            var response = new ApiResponse<bool>();

            var existing = HccApp.CatalogServices.ProductRelationships.FindByProductAndRelated(bvin, otherBvin);

            // check to see if the relationship exists already
            if (existing != null && existing.Id > 0)
            {
                // it exists, delete it
                response.Content = HccApp.CatalogServices.ProductRelationships.UnrelateProducts(bvin, otherBvin);
            }
            else
            {
                // it doesn't exist, report it to the calling application
                response.Content = false;
                response.Errors.Add(new ApiError("EXCEPTION", "Cannot delete a product relationship that does not exist"));
            }

            // serialize the response
            data = Json.ObjectToJson(response);

            // return the response
            return data;
        }
    }
}