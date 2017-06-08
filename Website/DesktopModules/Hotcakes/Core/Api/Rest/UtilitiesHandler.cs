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
using Hotcakes.CommerceDTO.v1;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the utilitarian REST API calls
    /// </summary>
    [Serializable]
    public class UtilitiesHandler : BaseRestHandler
    {
        public UtilitiesHandler(HotcakesApplication app)
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
            var data = string.Empty;
            var response = new ApiResponse<string>();
            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Informs the REST API to return a URL safe slug to use in a URL, using the given slug parameter.
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call. Only 1 parameter is expected, "slugify."</param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">String - The value to return as a URL-safe slug</param>
        /// <returns>
        ///     Boolean - Serialized (JSON) version of the ApiResponse object. If true, the product index was successful.
        /// </returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;

            var action = FirstParameter(parameters);
            switch (action.ToLowerInvariant().Trim())
            {
                case "slugify":
                    var response = new ApiResponse<string>();
                    response.Content = Text.Slugify(postdata, true);
                    data = Json.ObjectToJson(response);
                    break;
            }

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
            var data = string.Empty;
            var response = new ApiResponse<bool> {Content = false};
            data = Json.ObjectToJson(response);
            return data;
        }
    }
}