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
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for product review management
    /// </summary>
    [Serializable]
    public class ProductReviewsHandler : BaseRestHandler
    {
        public ProductReviewsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many product reviews to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected key will be
        ///     productbvin.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     ProductReviewDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            // check to see if the query is for one or many product reviews
            if (string.IsNullOrEmpty(parameters))
            {
                var byProduct = querystring["productbvin"] ?? string.Empty;

                // instantiate a response
                var response = new ApiResponse<List<ProductReviewDTO>>();

                // instantiate a collection of reviews
                var items = new List<ProductReview>();

                if (byProduct != string.Empty)
                {
                    // find a single product review
                    items = HccApp.CatalogServices.ProductReviews.FindByProductId(byProduct);
                }
                else
                {
                    // find all product reviews
                    items = HccApp.CatalogServices.ProductReviews.FindAllPaged(1, int.MaxValue);
                }

                if (items == null)
                {
                    // let the calling application know that there aren't any reviews found
                    response.Errors.Add(new ApiError("NULL", "No product reviews found."));
                }
                else
                {
                    // ensure that the Content property is instantiated
                    response.Content = new List<ProductReviewDTO>();

                    // iterate through each item and add it to the response
                    foreach (var r in items)
                    {
                        response.Content.Add(r.ToDto());
                    }
                }

                // serialize the response to send back to the calling application
                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find one specific product review
                var response = new ApiResponse<ProductReviewDTO>();

                // get the product review id
                var bvin = FirstParameter(parameters);

                // get the product review from the data source
                var item = HccApp.CatalogServices.ProductReviews.Find(bvin);

                if (item == null)
                {
                    // add the error to the response
                    response.Errors.Add(new ApiError("NULL", "Could not locate that item. Check bvin and try again."));
                }
                else
                {
                    // add the product review to the response
                    response.Content = item.ToDto();
                }

                // serialize the response to send back to the calling application
                data = Json.ObjectToJson(response);
            }

            // send the response back to the calling application
            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update a product review
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product review ID (bvin) and that this is an update, otherwise it assumes to
        ///     create a product review.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductReviewDTO object</param>
        /// <returns>ProductReviewDTO - Serialized (JSON) version of the product review</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            // an object used to return the response
            var data = string.Empty;

            // the ID of the product review to update
            var bvin = FirstParameter(parameters);

            // a response object used to send back the new/updated review
            var response = new ApiResponse<ProductReviewDTO>();

            // an object used to house the posted review for parsing
            ProductReviewDTO postedItem = null;

            try
            {
                // load the posted review object to the local REST API object
                postedItem = Json.ObjectFromJson<ProductReviewDTO>(postdata);
            }
            catch (Exception ex)
            {
                // add an error to send back to the calling application
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            // create a new review object
            var item = new ProductReview();

            // convert the posted review object for parsing
            item.FromDto(postedItem);

            // if bvin is empty this is a new review
            if (bvin == string.Empty)
            {
                // create the new review 
                if (HccApp.CatalogServices.ProductReviews.Create(item))
                {
                    // populate the local bvin object for use in querying for the update later
                    bvin = item.Bvin;
                }
            }
            else
            {
                item.StoreId = HccApp.CurrentRequestContext.CurrentStore.Id;

                // update the review in the data source
                HccApp.CatalogServices.ProductReviews.Update(item);
            }


            // find a fresh instance of the new/updated product review
            var resultItem = HccApp.CatalogServices.ProductReviews.Find(bvin);

            // add the review object to the response
            if (resultItem != null) response.Content = resultItem.ToDto();

            // serialize the response for return
            data = Json.ObjectToJson(response);

            // send the response back to the calling application
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a product review from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product review ID/bvin)
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
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.ProductReviews.Delete(bvin)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}