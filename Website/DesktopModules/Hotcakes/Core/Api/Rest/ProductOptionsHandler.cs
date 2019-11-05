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
    ///     This class handles all of the REST API calls for product options management
    /// </summary>
    [Serializable]
    public class ProductOptionsHandler : BaseRestHandler
    {
        public ProductOptionsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many product options to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only additional querystring expected
        ///     is "productbvin" for returning options assigned to the specified product.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     OptionDTO.
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<OptionDTO>>();

                var productBvin = querystring["productbvin"] ?? string.Empty;
                List<Option> results;
                if (string.IsNullOrEmpty(productBvin))
                {
                    results = HccApp.CatalogServices.ProductOptions.FindAll(1, int.MaxValue);
                }
                else
                {
                    results = HccApp.CatalogServices.ProductOptions.FindByProductId(productBvin);
                }
                var dto = new List<OptionDTO>();

                if (results.Any())
                {
                    foreach (var item in results)
                    {
                        dto.Add(item.ToDto());
                    }
                }
                else
                {
                    response.Errors.Add(new ApiError("NULL", "No options were found for the given BVIN."));
                }
                response.Content = dto;
                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find One Specific Category
                var response = new ApiResponse<OptionDTO>();
                var bvin = FirstParameter(parameters);
                var item = HccApp.CatalogServices.ProductOptions.Find(bvin);
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
        ///     Allows the REST API to create or update a product option
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product option ID (bvin) and that this is an update, otherwise it assumes to
        ///     create a product option.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the OptionDTO object</param>
        /// <returns>OptionDTO - Serialized (JSON) version of the product option</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);

            var isProducts = GetParameterByIndex(1, parameters);
            var productBvin = GetParameterByIndex(2, parameters);

            if (isProducts.Trim().ToLower() == "generateonly")
            {
                // Generate Variants Only
                var response = new ApiResponse<bool>();
                var p = HccApp.CatalogServices.Products.Find(productBvin);
                if (p == null | p.Bvin == string.Empty)
                {
                    data = Json.ObjectToJson(response);
                    return data;
                }
				int possibleCount;
				HccApp.CatalogServices.VariantsGenerateAllPossible(p, out possibleCount);
                response.Content = true;
                data = Json.ObjectToJson(response);
            }
            else if (isProducts.Trim().ToLower() == "products")
            {
                
                string generatevariants = querystring["generatevariants"];
                string unassign = querystring["unassign"];

                // Assign to Products
                var response = new ApiResponse<bool>();
                var p = HccApp.CatalogServices.Products.Find(productBvin);

                if (p == null || p.Bvin == string.Empty)
                {
                    data = Json.ObjectToJson(response);
                    return data;
                }

                if (!string.IsNullOrEmpty(unassign) && unassign == "1")
                {
                    HccApp.CatalogServices.ProductsRemoveOption(p, bvin);
                }
                else
                {
                    HccApp.CatalogServices.ProductsAddOption(p, bvin);

                    if (generatevariants.Trim() == "1")
                    {
                        int possibleCount;
                        HccApp.CatalogServices.VariantsGenerateAllPossible(p, out possibleCount);
                    }
                }

                response.Content = true;
                data = Json.ObjectToJson(response);
            }
            else
            {
                var response = new ApiResponse<OptionDTO>();

                OptionDTO postedItem = null;
                try
                {
                    postedItem = Json.ObjectFromJson<OptionDTO>(postdata);
                }
                catch (Exception ex)
                {
                    response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                    return Json.ObjectToJson(response);
                }

                if (postedItem == null)
                {
                    response.Errors.Add(new ApiError("EXCEPTION", "No postdata was found in your request."));
                    return Json.ObjectToJson(response);
                }

                // convert the option from DTO
                var op = new Option();
                op.FromDto(postedItem);

                // attempt to get the option from the DB to see if it exists already
                var existing = HccApp.CatalogServices.ProductOptions.Find(postedItem.Bvin);

                if (existing == null || existing.Bvin == string.Empty)
                {
                    postedItem.StoreId = HccApp.CurrentStore.Id;

                    // create a new option
                    HccApp.CatalogServices.ProductOptions.Create(op);
                }
                else
                {
                    // update the existing option
                    HccApp.CatalogServices.ProductOptions.Update(op);
                }

                // grab a fresh instance of the product option
                var resultOption = HccApp.CatalogServices.ProductOptions.Find(op.Bvin);

                // add the option to the response
                if (resultOption != null) response.Content = resultOption.ToDto();

                data = Json.ObjectToJson(response);
            }


            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a product option from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product option ID/bvin)
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
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.ProductOptions.Delete(bvin)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}