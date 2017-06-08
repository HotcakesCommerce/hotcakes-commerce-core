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
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Membership;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Membership;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class CustomerAccountHandler : BaseRestHandler
    {
        public CustomerAccountHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many categories to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected keys will be byproduct
        ///     and byslug.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     CategoryDTO or list of CategorySnapshotDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;
            var email = querystring["email"];

            if (string.IsNullOrEmpty(parameters) && string.IsNullOrEmpty(email))
            {
                var countonly = querystring["countonly"] ?? string.Empty;
                var page = querystring["page"] ?? "1";
                var pageInt = 1;
                int.TryParse(page, out pageInt);
                var pageSize = querystring["pagesize"] ?? "9";
                var pageSizeInt = 9;
                int.TryParse(pageSize, out pageSizeInt);

                if (querystring["countonly"] != null)
                {
                    // List by Page
                    var response = new ApiResponse<long>();
                    var results = HccApp.MembershipServices.Customers.CountOfAll();
                    response.Content = results;
                    data = Json.ObjectToJson(response);
                }
                else if (querystring["page"] != null)
                {
                    // List by Page
                    var response = new ApiResponse<List<CustomerAccountDTO>>();
                    var results = HccApp.MembershipServices.Customers.FindAllPaged(pageInt, pageSizeInt);
                    var dto = new List<CustomerAccountDTO>();
                    foreach (var item in results)
                    {
                        dto.Add(item.ToDto());
                    }
                    response.Content = dto;
                    data = Json.ObjectToJson(response);
                }
                else
                {
                    // List
                    var response = new ApiResponse<List<CustomerAccountDTO>>();
                    var results = HccApp.MembershipServices.Customers.FindAll();
                    var dto = new List<CustomerAccountDTO>();

                    foreach (var item in results)
                    {
                        dto.Add(item.ToDto());
                    }
                    response.Content = dto;
                    data = Json.ObjectToJson(response);
                }
            }
            else
            {
                // Find One Specific Customer
                var response = new ApiResponse<CustomerAccountDTO>();
                var bvin = FirstParameter(parameters);

                CustomerAccount item;
                if (string.IsNullOrWhiteSpace(email))
                {
                    item = HccApp.MembershipServices.Customers.Find(bvin);
                }
                else
                {
                    item = HccApp.MembershipServices.Customers.FindByEmail(email).FirstOrDefault();
                }
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
        ///     Allows the REST API to create or update a category
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the category ID (bvin) and that this is an update, otherwise it assumes to create
        ///     a category.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the CategoryDTO object</param>
        /// <returns>CategoryDTO - Serialized (JSON) version of the category</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<CustomerAccountDTO>();

            CustomerAccountDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<CustomerAccountDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new CustomerAccount();
            item.FromDto(postedItem);

            if (bvin == string.Empty)
            {
                var existing = HccApp.MembershipServices.Customers.FindByEmail(item.Email).FirstOrDefault();
                if (existing == null || existing.Bvin == string.Empty)
                {
                    var clearPassword = item.Password;
                    if (string.IsNullOrWhiteSpace(clearPassword))
                    {
                        clearPassword = PasswordGenerator.GeneratePassword(10);
                    }
                    // Create
                    var result = HccApp.MembershipServices.CreateCustomer(item, clearPassword);
                    bvin = item.Bvin;
                }
                else
                {
                    bvin = existing.Bvin;
                }
            }
            else
            {
                HccApp.MembershipServices.UpdateCustomer(item);
            }
            var resultItem = HccApp.MembershipServices.Customers.Find(bvin);
            if (resultItem != null)
            {
                response.Content = resultItem.ToDto();
                // Address Import
                foreach (var a in postedItem.Addresses)
                {
                    var addr = new Address();
                    addr.FromDto(a);
                    HccApp.MembershipServices.CheckIfNewAddressAndAddWithUpdate(resultItem, addr);
                }
            }

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a category from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (category ID/bvin) is
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
            var response = new ApiResponse<bool>();

            if (bvin == string.Empty)
            {
                // Clear All Requested
                response.Content = HccApp.MembershipServices.DestroyAllCustomers(HccApp);
            }
            else
            {
                // Delete Single Customer
                response.Content = HccApp.DestroyCustomerAccount(bvin);
            }

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}