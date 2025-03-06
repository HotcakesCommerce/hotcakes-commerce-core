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
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for product property management
    /// </summary>
    [Serializable]
    public class ProductPropertiesHandler : BaseRestHandler
    {
        public ProductPropertiesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many product properties to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring.</param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     ProductPropertyDTO.
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var productBvin = querystring["byproduct"] ?? string.Empty;
                var response = new ApiResponse<List<ProductPropertyDTO>>();

                List<ProductProperty> results = null;

                if (string.IsNullOrEmpty(productBvin))
                {
                    results = HccApp.CatalogServices.ProductProperties.FindAll();
                }
                else
                {
                    var product = HccApp.CatalogServices.Products.Find(productBvin);
                    results = product.GetProductTypeProperties();
                }

                if (results.Any())
                {
                    var dto = new List<ProductPropertyDTO>();

                    foreach (var item in results)
                    {
                        dto.Add(item.ToDto());
                    }

                    response.Content = dto;
                }
                else
                {
                    response.Errors.Add(new ApiError("NULL", "No product properties found."));
                }

                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find One Specific Category
                var response = new ApiResponse<ProductPropertyDTO>();
                var ids = FirstParameter(parameters);
                long id = 0;
                long.TryParse(ids, out id);
                var item = HccApp.CatalogServices.ProductProperties.Find(id);
                if (item == null)
                {
                    response.Errors.Add(new ApiError("NULL", "Could not locate that item. Check id and try again."));
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
        ///     Allows the REST API to create or update a product property
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product property ID (bvin) and that this is an update, otherwise it assumes
        ///     to create a product property.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductPropertyDTO object</param>
        /// <returns>ProductPropertyDTO - Serialized (JSON) version of the ProductProperty</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);

            var isValueSetRequest = GetParameterByIndex(1, parameters);


            if (isValueSetRequest.Trim().ToLowerInvariant() == "valuesforproduct")
            {
                // Set Property value Request
                var response = new ApiResponse<bool>();

                var productBvin = GetParameterByIndex(2, parameters);
                var propertyValue = postdata;
                var property = HccApp.CatalogServices.ProductProperties.Find(id);
                response.Content = HccApp.CatalogServices.ProductPropertyValues.SetPropertyValue(productBvin, property,
                    propertyValue);

                data = Json.ObjectToJson(response);
            }
            else
            {
                // Regular Create or Update Request

                var response = new ApiResponse<ProductPropertyDTO>();
                ProductPropertyDTO postedItem = null;
                try
                {
                    postedItem = Json.ObjectFromJson<ProductPropertyDTO>(postdata);
                }
                catch (Exception ex)
                {
                    response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                    return Json.ObjectToJson(response);
                }

                var item = new ProductProperty();
                item.FromDto(postedItem);


                // Check for existing and create base property
                var existing = HccApp.CatalogServices.ProductProperties.FindByName(item.DisplayName);
                if (existing == null)
                {
                    // Create
                    HccApp.CatalogServices.ProductProperties.Create(item);
                    id = item.Id;
                }
                else
                {
                    // Update
                    HccApp.CatalogServices.ProductProperties.Update(item);
                    id = existing.Id;
                }

                var resultItem = HccApp.CatalogServices.ProductProperties.Find(id);
                if (resultItem != null) response.Content = resultItem.ToDto();

                data = Json.ObjectToJson(response);
            }

            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a product property from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product property
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
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.ProductPropertiesDestroy(id)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}