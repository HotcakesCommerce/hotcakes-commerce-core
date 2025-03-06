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

using System.Linq;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Configuration
{
    /// <summary>
    ///     Summary description for ConfigHandler
    /// </summary>
    public class ConfigHandler : BaseHandler, IHttpHandler
    {
        protected override object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            if (request.RequestContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                // not found
                request.RequestContext.HttpContext.Response.StatusCode = 404;
                request.RequestContext.HttpContext.Response.End();
                return null;
            }

            var method = request.Params["method"];

            switch (method)
            {
                case "ResortShippingMethods":
                    return ResortShippingMethods(request, hccApp);
                case "VerifyAddress":
                    return VerifyAddress(request, hccApp);
                default:
                    break;
            }
            return true;
        }

        private object ResortShippingMethods(HttpRequest request, HotcakesApplication hccApp)
        {
            var offset = request.Params["offset"].ConvertTo(0);
            var itemIds = request.Params["itemIds"];
            var sortedItemIds = itemIds.Split(',').ToList();

            return hccApp.OrderServices.ShippingMethods.Resort(sortedItemIds, offset);
        }

        private object VerifyAddress(HttpRequest request, HotcakesApplication hccApp)
        {
            var addr = new Address
            {
                CountryBvin = request.Params["CountryBvin"],
                Line1 = request.Params["Line1"],
                Line2 = request.Params["Line2"],
                City = request.Params["City"],
                RegionBvin = request.Params["RegionBvin"],
                PostalCode = request.Params["PostalCode"]
            };

            var service = new AddressService(hccApp.CurrentStore);
            string message;
            Address nmAddr = null;
            var isValid = service.Validate(addr, out message, out nmAddr);

            string secMessage = null;
            Address nmSecAddr = null;
            Address secAddr = null;
            var secIsValid = false;
            if (!string.IsNullOrEmpty(request.Params["ContainsSecondary"]) &&
                bool.Parse(request.Params["ContainsSecondary"]))
            {
                secAddr = new Address
                {
                    CountryBvin = request.Params["SecCountryBvin"],
                    Line1 = request.Params["SecLine1"],
                    Line2 = request.Params["SecLine2"],
                    City = request.Params["SecCity"],
                    RegionBvin = request.Params["SecRegionBvin"],
                    PostalCode = request.Params["SecPostalCode"]
                };
                secIsValid = service.Validate(secAddr, out secMessage, out nmSecAddr);
            }

            return new
            {
                IsValid = isValid,
                Message = message,
                NormalizedAddress = nmAddr,
                NormalizedAddressHtml = nmAddr != null ? nmAddr.GetLinesHtml(false, false) : null,
                OriginalAddressHtml = addr.GetLinesHtml(false, false),
                SecIsValid = secIsValid,
                SecMessage = secMessage,
                SecNormalizedAddress = nmSecAddr,
                SecNormalizedAddressHtml = nmSecAddr != null ? nmSecAddr.GetLinesHtml(false, false) : null,
                SecOriginalAddressHtml = secAddr != null ? secAddr.GetLinesHtml(false, false) : null
            };
        }
    }
}