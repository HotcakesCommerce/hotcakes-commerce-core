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
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for gift card management
    /// </summary>
    [Serializable]
    public class GiftCardsHandler : BaseRestHandler
    {
        public GiftCardsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many gift cards to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. Querystrings are not currently used by
        ///     this method.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     GiftCardDTO or list of GiftCardDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            if (string.IsNullOrEmpty(parameters))
            {
                var page = querystring["page"] ?? "1";
                var pageInt = 1;
                int.TryParse(page, out pageInt);
                var pageSize = querystring["pagesize"] ?? "9";
                var pageSizeInt = 9;
                int.TryParse(pageSize, out pageSizeInt);

                if (querystring["countonly"] != null)
                {
                    // Count of pages
                    var items = HccApp.CatalogServices.GiftCards.FindAllPaged(1, int.MaxValue);
                    var results = items.Count;
                    return JsonApiResponse(results);
                }
                if (querystring["page"] != null)
                {
                    // List by page
                    var items = HccApp.CatalogServices.GiftCards.FindAllPaged(pageInt, pageSizeInt);
                    var dto = items.Select(i => i.ToDto()).ToList();
                    return JsonApiResponse(dto);
                }
                else
                {
                    // List of all
                    var items = HccApp.CatalogServices.GiftCards.FindAllPaged(1, int.MaxValue);
                    var dto = items.Select(i => i.ToDto()).ToList();
                    return JsonApiResponse(dto);
                }
            }
            // Get just one gift card
            var id = FirstLongParameter(parameters);
            GiftCard item = null;
            if (id.HasValue)
            {
                item = HccApp.CatalogServices.GiftCards.Find(id.Value);
            }

            if (item == null)
            {
                return JsonApiResponseError("NULL", "Could not locate that gift card. Check the id and try again.");
            }

            return JsonApiResponse(item.ToDto());
        }

        /// <summary>
        ///     Allows the REST API to create or update a gift card
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call. No parameters are expected at this time.</param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the GiftCardDTO object</param>
        /// <returns>GiftCardDTO - Serialized (JSON) version of the gift card</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var errors = new List<ApiError>();

            GiftCardDTO postedCard = null;
            try
            {
                postedCard = Json.ObjectFromJson<GiftCardDTO>(postdata);
            }
            catch (Exception ex)
            {
                return JsonApiResponseException(ex);
            }

            var giftCard = new GiftCard();
            giftCard.FromDto(postedCard);

            var existing = HccApp.CatalogServices.GiftCards.Find(giftCard.GiftCardId);

            if (existing == null)
            {
                HccApp.CatalogServices.GiftCards.Create(giftCard);
            }
            else
            {
                HccApp.CatalogServices.GiftCards.Update(giftCard);
            }

            return JsonApiResponse(giftCard.ToDto());
        }

        /// <summary>
        ///     Allows for the REST API to delete a gift card from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (gift card ID/bvin) is
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
            var id = FirstLongParameter(parameters);
            var response = new ApiResponse<bool>();

            if (id.HasValue)
            {
                var res = HccApp.CatalogServices.GiftCards.Delete(id.Value);
                return JsonApiResponse(res);
            }

            return JsonApiResponse(false);
        }
    }
}