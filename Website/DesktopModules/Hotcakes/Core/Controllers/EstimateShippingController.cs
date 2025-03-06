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
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Modules.Core.Models.Json;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class EstimateShippingController : BaseStoreController
    {
        private EstimateShippingViewModel BuildViewModel()
        {
            ViewBag.Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries();

            var model = new EstimateShippingViewModel();
            if (!string.IsNullOrEmpty(CurrentCart.ShippingAddress.CountryBvin))
            {
                model.CountryId = CurrentCart.ShippingAddress.CountryBvin;
            }
            if (!string.IsNullOrEmpty(CurrentCart.ShippingAddress.RegionBvin))
            {
                model.RegionId = CurrentCart.ShippingAddress.RegionBvin;
            }
            model.City = CurrentCart.ShippingAddress.City;
            model.PostalCode = CurrentCart.ShippingAddress.PostalCode;

            return model;
        }

        public ActionResult Index()
        {
            var model = BuildViewModel();

            // Populate Data for DropDownLists
            var currentCountry = HccApp.GlobalizationServices.Countries.Find(model.CountryId);
            if (currentCountry != null)
                ViewBag.Regions = currentCountry.Regions;

            return View(model);
        }

        [HccHttpPost]
        [ActionName("Index")]
        public ActionResult IndexPost(EstimateShippingViewModel posted)
        {
            var model = BuildViewModel();
            if (posted != null)
            {
                model.PostalCode = posted.PostalCode;
                model.CountryId = posted.CountryId;
                model.RegionId = posted.RegionId;
            }
            GetRatesImpl(model);

            return View(model);
        }

        [HccHttpPost]
        public ActionResult GetRates(string countryId, string regionId, string postalCode)
        {
            var model = BuildViewModel();
            model.PostalCode = postalCode;
            model.CountryId = countryId;
            model.RegionId = regionId;
            GetRatesImpl(model);

            return new JsonResult {Data = model.Rates};
        }

        [HccHttpPost]
        public ActionResult GetRegions(string id)
        {
            var regionid = Request.Form["regionid"];

            var c = HccApp.GlobalizationServices.Countries.Find(id);
            var result = new RegionsJsonModel();
            if (c != null)
            {
                var notSelectedValue = c.Regions.Count > 0 ? string.Empty : "_";
                    // Set underscore to pass required validator in case no regions can be selected 
                result.Regions = string.Format("<option value=\"{0}\">{1}</option>", notSelectedValue,
                    Localization.GetString("NotSelectedItem"));
                var sb = new StringBuilder();

                foreach (var r in c.Regions.OrderBy(x => x.DisplayName))
                {
                    sb.Append("<option ");
                    if (r.Abbreviation == regionid)
                    {
                        sb.Append(" selected=\"selected\" ");
                    }
                    sb.AppendFormat(" value=\"{0}\">{1}</option>", r.Abbreviation, r.DisplayName);
                }

                result.Regions += sb.ToString();
            }

            return new JsonResult {Data = result};
        }

        private void GetRatesImpl(EstimateShippingViewModel model)
        {
            CurrentCart.ShippingAddress.PostalCode = model.PostalCode;

            if (model.CountryId != string.Empty)
            {
                var current = HccApp.GlobalizationServices.Countries.Find(model.CountryId);
                if (current != null)
                {
                    CurrentCart.ShippingAddress.CountryBvin = model.CountryId;
                    CurrentCart.ShippingAddress.RegionBvin = model.RegionId;
                }
            }

            HccApp.OrderServices.Orders.Update(CurrentCart);

            var rates = HccApp.OrderServices.FindAvailableShippingRates(CurrentCart);

            if (rates.Count < 1)
            {
                TempData["message"] = "Unable to estimate at this time";
            }
            model.Rates = rates.ToList();
        }

        [HccHttpPost]
        public ActionResult GetRatesAsRadioButtons(FormCollection form)
        {
            var result = new ShippingRatesJsonModel();

            var country = form["country"] ?? string.Empty;
            var firstname = form["firstname"] ?? string.Empty;
            var lastname = form["lastname"] ?? string.Empty;
            var address = form["address"] ?? string.Empty;
            var city = form["city"] ?? string.Empty;
            var state = form["state"] ?? string.Empty;
            var zip = form["zip"] ?? string.Empty;
            var orderid = form["orderid"] ?? string.Empty;

            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderid);
            order.ShippingAddress.FirstName = firstname;
            order.ShippingAddress.LastName = lastname;
            order.ShippingAddress.Line1 = address;
            order.ShippingAddress.City = city;
            order.ShippingAddress.PostalCode = zip;
            var c = HccApp.GlobalizationServices.Countries.Find(country);
            if (c != null)
            {
                order.ShippingAddress.CountryBvin = country;
                var region = c.Regions.
                    FirstOrDefault(r => r.Abbreviation == state);
                if (region != null)
                    order.ShippingAddress.RegionBvin = region.Abbreviation;
            }

            var rates = HccApp.OrderServices.FindAvailableShippingRates(order);
            if (rates != null && rates.Count > 0)
            {
                var selectedRate = rates.
                    OfType<ShippingRateDisplay>().
                    FirstOrDefault(r => r.UniqueKey == order.ShippingMethodUniqueKey);
                if (selectedRate == null)
                    selectedRate = rates[0];

                HccApp.OrderServices.OrdersRequestShippingMethod(selectedRate, order);
            }
            else
            {
                order.ClearShippingPricesAndMethod();
            }

            result.rates = HtmlRendering.ShippingRatesToRadioButtons(rates, 300, order.ShippingMethodUniqueKey);

            HccApp.CalculateOrderAndSave(order);

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }
    }
}