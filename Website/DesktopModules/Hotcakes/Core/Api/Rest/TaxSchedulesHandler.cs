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
using Hotcakes.Commerce.Taxes;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Taxes;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This is the class that handles the REST API end point calls for tax schedules.
    /// </summary>
    [Serializable]
    public class TaxSchedulesHandler : BaseRestHandler
    {
        public TaxSchedulesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many tax schedules to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected keys will be byproduct
        ///     and byslug.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     TaxScheduleDTO or list of TaxScheduleDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<TaxScheduleDTO>>();

                var results = HccApp.OrderServices.TaxSchedules.FindAll(HccApp.CurrentStore.Id);
                var dto = new List<TaxScheduleDTO>();

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
                var response = new ApiResponse<TaxScheduleDTO>();
                var ids = FirstParameter(parameters);
                long id = 0;
                long.TryParse(ids, out id);
                var item = HccApp.OrderServices.TaxSchedules.FindForThisStore(id);
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
        ///     Allows the REST API to create or update a tax schedule
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the tax schedule ID and that this is an update, otherwise it assumes to create a
        ///     tax schedule.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the TaxScheduleDTO object</param>
        /// <returns>TaxDTO - Serialized (JSON) version of the tax schedule</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);
            var response = new ApiResponse<TaxScheduleDTO>();

            TaxScheduleDTO postedCategory = null;
            try
            {
                postedCategory = Json.ObjectFromJson<TaxScheduleDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new TaxSchedule();
            item.FromDto(postedCategory);

            if (id < 1)
            {
                var existing = HccApp.OrderServices.TaxSchedules.FindByNameForThisStore(item.Name);
                if (existing == null)
                {
                    // Create
                    HccApp.OrderServices.TaxSchedules.Create(item);
                }
                else
                {
                    item.Id = existing.Id;
                }
            }
            else
            {
                HccApp.OrderServices.TaxSchedules.Update(item);
            }
            response.Content = item.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a tax schedule from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (tax schedule ID) is
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
            var ids = FirstParameter(parameters);
            long id = 0;
            long.TryParse(ids, out id);
            var response = new ApiResponse<bool> {Content = HccApp.OrderServices.TaxSchedules.Delete(id)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}