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
using Hotcakes.Web.Data;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for catalog role management
    /// </summary>
    public class CatalogRolesHandler : BaseRestHandler
    {
        public CatalogRolesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many catalog roles to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. The only expected keys are bytype and
        ///     bvin.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     CatalogRoleDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            // List
            var roleType = querystring["bytype"] ?? "product";
            var bvin = querystring["bvin"] ?? string.Empty;
            var guid = DataTypeHelper.BvinToGuid(bvin);

            List<CatalogRole> results = null;

            switch (roleType)
            {
                case "producttype":
                    results = HccApp.CatalogServices.CatalogRoles.FindByProductTypeId(guid);
                    break;
                case "category":
                    results = HccApp.CatalogServices.CatalogRoles.FindByCategoryId(guid);
                    break;
                default:
                    results = HccApp.CatalogServices.CatalogRoles.FindByProductId(guid);
                    break;
            }

            var dto = results.Select(r => r.ToDto()).ToList();
            return JsonApiResponse(dto);
        }

        /// <summary>
        ///     Allows the REST API to create a catalog role. There is no update.
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call. None are expected.</param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the CatalogRoleDTO object</param>
        /// <returns>CatalogRoleDTO - Serialized (JSON) version of the catalog role</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var errors = new List<ApiError>();
            var data = string.Empty;

            CatalogRoleDTO postedRole = null;
            try
            {
                postedRole = Json.ObjectFromJson<CatalogRoleDTO>(postdata);
            }
            catch (Exception ex)
            {
                return JsonApiResponseException(ex);
            }

            var cRole = new CatalogRole();
            cRole.FromDto(postedRole);

            HccApp.CatalogServices.CatalogRoles.Create(cRole);

            return JsonApiResponse(cRole.ToDto());
        }

        /// <summary>
        ///     Allows for the REST API to delete a catalog role from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (catalog role ID/bvin)
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
            var id = FirstLongParameter(parameters);
            var response = new ApiResponse<bool>();

            if (id.HasValue)
            {
                var res = HccApp.CatalogServices.CatalogRoles.Delete(id.Value);
                return JsonApiResponse(res);
            }

            return JsonApiResponse(false);
        }
    }
}