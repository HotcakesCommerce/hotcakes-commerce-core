#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Linq;
using System.Web.Mvc;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Urls;
using Hotcakes.Modules.Core.Controllers;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Modules.Core.Models.Json;

namespace Hotcakes.Modules.Core.Areas.Account.Controllers
{
/// <summary>Manages the signed-in customer's saved addresses.</summary>
    [CustomerSignedInFilter]
    [Serializable]
    public class AddressBookController : BaseStoreController
    {
        /// <summary>Displays the customer's saved addresses.</summary>
        /// <returns>The address book view.</returns>
        public ActionResult Index()
        {
            var addresses = LoadAddresses();

            return View(addresses);
        }

        /// <summary>Deletes a saved address and returns to the address book.</summary>
        /// <param name="id">The address business identifier.</param>
        /// <returns>A redirect to the address book.</returns>
        [HccHttpPost]
        public ActionResult Delete(string id)
        {
            var user = HccApp.CurrentCustomer;
            if (user != null)
            {
                user.DeleteAddress(id);
                HccApp.MembershipServices.UpdateCustomer(user);
            }

            return Redirect(Url.RouteHccUrl(HccRoute.AddressBook));
        }

        /// <summary>Saves a new or existing customer address.</summary>
        /// <param name="id">The address identifier, or <c>new</c> for a new address.</param>
        /// <param name="posted">The submitted address form values.</param>
        /// <returns>The address view on validation failure or a redirect after saving.</returns>
        [ActionName("Edit")]
        [HccHttpPost]
        public ActionResult EditPosted(string id, FormCollection posted)
        {
            var model = new AddressViewModel();
            if (TryUpdateModel(model))
            {
                var u = HccApp.MembershipServices.Customers.Find(HccApp.CurrentCustomerId);
                if (u == null) return View(model);

                if (model.RegionBvin == "_") model.RegionBvin = string.Empty;

                var addr = LoadAddress(id);
                model.CopyTo(addr);

                var slug = id;
                switch (slug.ToLower())
                {
                    case "new":
                        HccApp.MembershipServices.CheckIfNewAddressAndAddWithUpdate(u, addr);
                        HccApp.MembershipServices.UpdateCustomer(u);
                        break;
                    default:
                        u.UpdateAddress(addr);
                        HccApp.MembershipServices.UpdateCustomer(u);
                        break;
                }
                return Redirect(Url.RouteHccUrl(HccRoute.AddressBook));
            }

            model.Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            var country = HccApp.GlobalizationServices.Countries.Find(model.CountryBvin);
            model.Regions = country.Regions;

            return View(model);
        }

        /// <summary>Validates and optionally normalizes a posted address.</summary>
        /// <param name="form">The submitted address fields.</param>
        /// <returns>A serialized address validation response.</returns>
        [HttpPost]
        public ActionResult ValidateAddress(FormCollection form)
        {
            var model = new AddressValidationJsonModel();

            var address = new Address
            {
                CountryBvin = form["country"] ?? string.Empty,
                Line1 = form["address"] ?? string.Empty,
                Line2 = form["address2"] ?? string.Empty,
                City = form["city"] ?? string.Empty,
                PostalCode = form["zip"] ?? string.Empty,
                RegionBvin = form["state"] ?? string.Empty
            };

            var service = new AddressService(HccApp.CurrentStore);
            string message;
            Address nmAddr = null;
            model.IsValid = service.Validate(address, out message, out nmAddr);
            var isNormalized = nmAddr != null;

            model.Message = message;
            model.NormalizedAddress = nmAddr;
            model.NormalizedAddressHtml = isNormalized ? nmAddr.GetLinesHtml(false, false) : null;
            model.OriginalAddressHtml = address.GetLinesHtml(false, false);

            return new PreJsonResult(Web.Json.ObjectToJson(model));
        }

        /// <summary>Displays the edit form for a saved or new address.</summary>
        /// <param name="id">The address identifier, or <c>new</c>.</param>
        /// <returns>The address edit view.</returns>
        public ActionResult Edit(string id)
        {
            var a = LoadAddress(id);
            var model = new AddressViewModel(a)
            {
                Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries()
            };

            var countryCode = string.Empty;
            if (!string.IsNullOrEmpty(model.CountryBvin))
            {
                countryCode = model.CountryBvin;
            }
            else if (model.Countries != null && model.Countries.Any())
            {
                countryCode = model.Countries.FirstOrDefault().Bvin;
            }

            if (!string.IsNullOrEmpty(countryCode))
            {
                var country = HccApp.GlobalizationServices.Countries.Find(countryCode);
                model.Regions = new List<Region>(country.Regions);

                if (model.Regions == null || !model.Regions.Any())
                {
                    ModelState.Remove("RegionBvin");
                }
            }

            var notSelectedRegion = new Region
            {
                DisplayName = Localization.GetString("NotSelectedItem"),
                Abbreviation = model.Regions.Count == 0 ? "_" : string.Empty
            };

            model.Regions.Insert(0, notSelectedRegion);

            return View(model);
        }

        private List<Address> LoadAddresses()
        {
            var user = HccApp.CurrentCustomer;

            if (user != null)
            {
                return user.Addresses;
            }

            return new List<Address>();
        }

        private Address LoadAddress(string bvin)
        {
            var user = HccApp.CurrentCustomer;

            if (user != null)
            {
                switch (bvin.ToLower())
                {
                    case "new":
                        var a = new Address
                        {
                            Bvin = Guid.NewGuid().ToString(),
                            RegionBvin = string.Empty
                        };
                        return a;
                    default:
                        foreach (var a2 in user.Addresses)
                        {
                            if (a2.Bvin == bvin)
                            {
                                return a2;
                            }
                        }
                        break;
                }
            }

            return new Address();
        }
    }
}
