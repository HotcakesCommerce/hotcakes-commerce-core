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
using System.Collections.Generic;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class PriceGroupsHandler : BaseRestHandler
    {
        public PriceGroupsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many price groups to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring.</param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     PriceGroupDTO or list of PriceGroupDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<PriceGroupDTO>>();

                var results = HccApp.ContactServices.PriceGroups.FindAll();
                var dto = new List<PriceGroupDTO>();

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
                var response = new ApiResponse<PriceGroupDTO>();
                var bvin = FirstParameter(parameters);
                var item = HccApp.ContactServices.PriceGroups.Find(bvin);
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
        ///     Allows the REST API to create or update a price group
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the price group ID (bvin) and that this is an update, otherwise it assumes to
        ///     create a price group.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the PriceGroupDTO object</param>
        /// <returns>PriceGroupDTO - Serialized (JSON) version of the price group</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<PriceGroupDTO>();

            PriceGroupDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<PriceGroupDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new PriceGroup();
            item.FromDto(postedItem);

            if (bvin == string.Empty)
            {
                if (HccApp.ContactServices.PriceGroups.Create(item))
                {
                    bvin = item.Bvin;
                }
            }
            else
            {
                HccApp.ContactServices.PriceGroups.Update(item);
            }
            var resultItem = HccApp.ContactServices.PriceGroups.Find(bvin);
            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a price group from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (price group ID/bvin) is
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
            var response = new ApiResponse<bool> {Content = HccApp.ContactServices.PriceGroups.Delete(bvin)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}