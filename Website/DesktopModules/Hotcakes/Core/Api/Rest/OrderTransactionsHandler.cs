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
using Hotcakes.Commerce.Orders;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for order transaction management
    /// </summary>
    [Serializable]
    public class OrderTransactionsHandler : BaseRestHandler
    {
        public OrderTransactionsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many transactions to the application that called it
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. Only one (optional) parameter is expected,
        ///     which is the transaction ID or bvin.
        /// </param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected (optional) key is the
        ///     Order ID or bvin.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     OrderTransactionDTO.
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<OrderTransactionDTO>>();

                var orderBvin = querystring["orderbvin"] ?? string.Empty;
                var results = new List<OrderTransaction>();
                if (orderBvin.Trim().Length > 0)
                {
                    results = HccApp.OrderServices.Transactions.FindForOrder(orderBvin);
                }
                var dto = new List<OrderTransactionDTO>();

                foreach (var item in results)
                {
                    dto.Add(item.ToDto());
                }
                response.Content = dto;
                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find One Specific Order Transaction
                var response = new ApiResponse<OrderTransactionDTO>();
                var bvin = FirstParameter(parameters);
                var tempId = new Guid();
                Guid.TryParse(bvin, out tempId);
                var item = HccApp.OrderServices.Transactions.Find(tempId);
                if (item == null)
                {
                    response.Errors.Add(new ApiError("NULL",
                        "Could not locate that transaction. Check bvin and try again."));
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
        ///     Allows the REST API to create or update a transaction.
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the transaction ID (bvin) and that this is an update, otherwise it assumes to
        ///     create a new transaction.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the OrderTransacionDTO object</param>
        /// <returns>CategoryDTO - Serialized (JSON) version of the transaction</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<OrderTransactionDTO>();

            OrderTransactionDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<OrderTransactionDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new OrderTransaction();
            item.FromDto(postedItem);

            var existing = HccApp.OrderServices.Transactions.Find(item.Id);
            if (existing == null || item.Id == Guid.Empty)
            {
                item.StoreId = HccApp.CurrentStore.Id;
                item.Id = Guid.NewGuid();
                HccApp.OrderServices.Transactions.Create(item);
            }
            else
            {
                item.StoreId = HccApp.CurrentStore.Id;
                HccApp.OrderServices.Transactions.Update(item);
            }
            var resultItem = HccApp.OrderServices.Transactions.Find(item.Id);
            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a transaction from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (transaction ID/bvin) is
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
            var tempId = new Guid();
            Guid.TryParse(bvin, out tempId);
            var response = new ApiResponse<bool> {Content = HccApp.OrderServices.Transactions.Delete(tempId)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}