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
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for the search manager
    /// </summary>
    [Serializable]
    public class SearchManagerHandler : BaseRestHandler
    {
        public SearchManagerHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method is not suppported at this time.
        /// </summary>
        /// <param name="parameters">No parameters are expected</param>
        /// <param name="querystring">No values are expected</param>
        /// <returns>ApiError - Not Supported</returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var response = new ApiResponse<bool>();
            response.Errors.Add(new ApiError("NOTSUPPORTED", "GET method is not supported for this object."));
            response.Content = false;
            var data = string.Empty;
            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Informs the REST API to index the specified product.
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. Two parameters are expected. First,
        ///     "products" and then the second parameter is the product ID/Bvin.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">No postdata is expected at this time</param>
        /// <returns>Boolean - Serialized (JSON) version of the ApiResponse object. If true, the product index was successful.</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var objecttype = FirstParameter(parameters);
            var objectid = GetParameterByIndex(1, parameters);
            var response = new ApiResponse<bool>();

            try
            {
                if (objecttype.Trim().ToLowerInvariant() == "products")
                {
                    var m = new SearchManager(HccApp.CurrentRequestContext);
                    var p = HccApp.CatalogServices.Products.Find(objectid);
                    if (p != null)
                    {
                        if (p.Bvin.Length > 0)
                        {
                            m.IndexSingleProduct(p);
                            response.Content = true;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     The DELETE method is not suppported at this time.
        /// </summary>
        /// <param name="parameters">No parameters are expected</param>
        /// <param name="querystring">No values are expected</param>
        /// <param name="postdata">The postdata.</param>
        /// <returns>
        ///     ApiError - Not Supported
        /// </returns>
        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var response = new ApiResponse<bool>();
            response.Errors.Add(new ApiError("NOTSUPPORTED", "Delete method is not supported for this object."));
            response.Content = false;
            var data = string.Empty;
            data = Json.ObjectToJson(response);
            return data;
        }
    }
}