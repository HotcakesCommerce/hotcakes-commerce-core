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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for product type management
    /// </summary>
    [Serializable]
    public class ProductTypesHandler : BaseRestHandler
    {
        public ProductTypesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many product types to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring.</param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     ProductTypeDTO.
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<ProductTypeDTO>>();

                var results = HccApp.CatalogServices.ProductTypes.FindAll();
                var dto = new List<ProductTypeDTO>();

                foreach (var item in results)
                {
                    dto.Add(item.ToDto());
                }
                response.Content = dto;
                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find One Specific Category
                var response = new ApiResponse<ProductTypeDTO>();
                var bvin = FirstParameter(parameters);
                var item = HccApp.CatalogServices.ProductTypes.Find(bvin);
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
        ///     Allows the REST API to create or update a product type
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product type ID (bvin) and that this is an update, otherwise it assumes to
        ///     create a product type.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductTypeDTO object</param>
        /// <returns>ProductTypeDTO - Serialized (JSON) version of the ProductType</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);

            //
            // <site Url>/producttypes/<guid>/properties/<propertyid>/<sortOrder>
            //
            var isProperty = GetParameterByIndex(1, parameters);
            if (isProperty.Trim().ToLowerInvariant() == "properties")
            {
                var response2 = new ApiResponse<bool>();

                var propertyIds = GetParameterByIndex(2, parameters);
                long propertyId = 0;
                long.TryParse(propertyIds, out propertyId);

                response2.Content = HccApp.CatalogServices.ProductTypeAddProperty(bvin, propertyId);
                data = Json.ObjectToJson(response2);
            }
            else
            {
                var response = new ApiResponse<ProductTypeDTO>();
                ProductTypeDTO postedItem = null;
                try
                {
                    postedItem = Json.ObjectFromJson<ProductTypeDTO>(postdata);
                }
                catch (Exception ex)
                {
                    response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                    return Json.ObjectToJson(response);
                }

                var item = new ProductType();
                item.FromDto(postedItem);

                if (bvin == string.Empty)
                {
                    if (HccApp.CatalogServices.ProductTypes.Create(item))
                    {
                        bvin = item.Bvin;
                    }
                }
                else
                {
                    HccApp.CatalogServices.ProductTypes.Update(item);
                }
                var resultItem = HccApp.CatalogServices.ProductTypes.Find(bvin);
                if (resultItem != null) response.Content = resultItem.ToDto();
                data = Json.ObjectToJson(response);
            }

            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a product type from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product type ID/bvin)
        ///     is expected.
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

            // 
            // <site url>/producttypes/<guid>/properties/<propertyid>
            //
            var isProperty = GetParameterByIndex(1, parameters);
            if (isProperty.Trim().ToLowerInvariant() == "properties")
            {
                // Properties Delete
                var response2 = new ApiResponse<bool>();

                var propertyIds = GetParameterByIndex(2, parameters);
                long propertyId = 0;
                long.TryParse(propertyIds, out propertyId);

                response2.Content = HccApp.CatalogServices.ProductTypeRemoveProperty(bvin, propertyId);
                data = Json.ObjectToJson(response2);
            }
            else
            {
                if (HccApp.CatalogServices.Products.FindCountByProductType(bvin) > 0)
                {
                    response.Errors.Add(new ApiError("EXCEPTION",
                        "Cannot delete product types while products are assigned to it."));
                }
                else
                {
                    // Single Item Delete
                    response.Content = HccApp.CatalogServices.ProductTypeDestroy(bvin);
                }

                data = Json.ObjectToJson(response);
            }
            return data;
        }
    }
}