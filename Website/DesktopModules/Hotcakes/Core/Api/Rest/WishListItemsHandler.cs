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
    ///     This class handles all of the REST API calls for wish list item management
    /// </summary>
    [Serializable]
    public class WishListItemsHandler : BaseRestHandler
    {
        public WishListItemsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many wish list item to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. There are no expected querystring
        ///     keys/values.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     WishListItemDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            long wishListItemId = 0;
            long.TryParse(FirstParameter(parameters), out wishListItemId);

            if (wishListItemId > 0)
            {
                // this was a request for a specific wish list item
                var item = HccApp.CatalogServices.WishListItems.Find(wishListItemId);

                if (item == null)
                {
                    return JsonApiResponseError("NULL", "No wish list items were found matching the given ID.");
                }
                return JsonApiResponse(item.ToDto());
            }
            // this was a request for all wish list items OR those matching a specific customer
            var customerId = querystring["id"];
            List<WishListItem> items = null;

            if (!string.IsNullOrEmpty(customerId))
            {
                // this is a request for wish list items matching a customer
                items = HccApp.CatalogServices.WishListItems.FindByCustomerIdPaged(customerId, 1, int.MaxValue);
            }
            else
            {
                // this is a request for all wish list items in the store
                items = HccApp.CatalogServices.WishListItems.FindAllPaged(1, int.MaxValue);
            }

            if (items == null || items.Count == 0)
            {
                return JsonApiResponseError("NULL", "No wish list items were found for the specified customer or store.");
            }
            var dto = items.Select(i => i.ToDto()).ToList();
            return JsonApiResponse(dto);
        }

        /// <summary>
        ///     Allows the REST API to create or update a wish list item.
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. Only one parameter is expected, which is
        ///     the ID of the wish list item, but only when updating.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the WishListItemDTO object</param>
        /// <returns>WishListItemDTO - Serialized (JSON) version of the wish list item</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            long id = 0;
            long.TryParse(FirstParameter(parameters), out id);
            var response = new ApiResponse<WishListItemDTO>();
            WishListItemDTO postedItem = null;

            try
            {
                postedItem = Json.ObjectFromJson<WishListItemDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new WishListItem();
            item.FromDto(postedItem);

            if (id < 1)
            {
                if (HccApp.CatalogServices.WishListItems.Create(item))
                {
                    id = item.Id;
                }
            }
            else
            {
                HccApp.CatalogServices.WishListItems.Update(item);
            }

            var resultItem = HccApp.CatalogServices.WishListItems.Find(id);
            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a wish list item from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (wish list item ID/bvin)
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
            long id = 0;
            long.TryParse(FirstParameter(parameters), out id);
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.WishListItems.Delete(id)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}