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
using Hotcakes.Commerce.Orders;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for order management
    /// </summary>
    [Serializable]
    public class OrdersHandler : BaseRestHandler
    {
        public OrdersHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many orders to the application that called it
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If a parameter is present, this method
        ///     expects the first parameter to be the Order ID or bvin.
        /// </param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. This method does not currently expect
        ///     any querystring values.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     OrderDTO or list of OrderSnapshotDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<OrderSnapshotDTO>>();

                var results = HccApp.OrderServices.Orders.FindAll();
                var dto = new List<OrderSnapshotDTO>();

                foreach (var item in results)
                {
                    dto.Add(item.ToDto());
                }
                response.Content = dto;
                data = response.ObjectToJson();
            }
            else
            {
                // Find One Specific Order
                var response = new ApiResponse<OrderDTO>();
                var bvin = FirstParameter(parameters);
                var item = HccApp.OrderServices.Orders.FindForCurrentStore(bvin);
                if (item == null)
                {
                    response.Errors.Add(new ApiError("NULL",
                        "Could not locate that order. Check the bvin and try again."));
                }
                else
                {
                    response.Content = item.ToDto();
                }
                data = response.ObjectToJson();
            }

            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update an order
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the order ID (bvin) and that this is an update, otherwise it assumes to create an
        ///     order.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the OrderDTO object</param>
        /// <returns>CategoryDTO - Serialized (JSON) version of the order</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var paramRecalc = querystring["recalc"];
            var isRecalc = paramRecalc != null && paramRecalc.Trim() == "1";

            OrderDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<OrderDTO>(postdata);
            }
            catch (Exception ex)
            {
                return JsonApiResponseException(ex);
            }

            var item = new Order();

            if (!string.IsNullOrEmpty(bvin))
            {
                item = HccApp.OrderServices.Orders.FindForCurrentStore(bvin);

                if (item != null)
                {
                    item.FromDTO(postedItem);

                    if (isRecalc)
                    {
                        HccApp.CalculateOrder(item);
                    }

                    HccApp.OrderServices.Orders.Update(item);
                }
            }
            else
            {
                item.FromDTO(postedItem);
                item.StoreId = HccApp.CurrentStore.Id;

                if (isRecalc)
                {
                    HccApp.CalculateOrder(item);
                }

                HccApp.OrderServices.Orders.Create(item);
            }

            return JsonApiResponse(item);
        }

        /// <summary>
        ///     Allows for the REST API to permanently delete an order from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (order ID/bvin) is
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
            var response = new ApiResponse<bool> {Content = HccApp.OrderServices.Orders.Delete(bvin)};

            data = response.ObjectToJson();
            return data;
        }
    }
}