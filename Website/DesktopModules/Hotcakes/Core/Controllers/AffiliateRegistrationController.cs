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
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;
using DotNetNuke.Common.Utilities;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Urls;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Web.Logging;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class AffiliateRegistrationController : BaseAppController
    {
        private const string AffiliateRoleName = "Hotcakes Affiliate";

        public ActionResult Index()
        {
            var userRegisteredAsAffiliate = false;
            var model = new AffiliateViewModel();

            LoadModel(model);

            if (HccApp.MembershipServices.IsUserLoggedIn())
            {
                var acc = HccApp.CurrentCustomer;
                userRegisteredAsAffiliate = IsUserAlreadyRegisteredAsAffiliate(acc);

                if (!userRegisteredAsAffiliate)
                {
                    LoadModelUserInfo(model, acc);
                }
            }

            if (userRegisteredAsAffiliate)
            {
                return Redirect(Url.RouteHccUrl(HccRoute.AffiliateDashboard));
            }
            return View(model);
        }

        [HccHttpPost]
        public ActionResult Register(AffiliateViewModel model)
        {
            if (ModelState.IsValid)
            {
                JsonResult result = null;
                var aff = LoadAffiliateFromModel(model);
                var createStatus = CreateUserStatus.None;
                var status = HccApp.ContactServices.Affiliates.Create(aff, ref createStatus);

                switch (status)
                {
                    case AffiliateRepository.UpdateStatus.Success:
                        result = GetStatusMessage(Localization.GetString("msgAffiliateSuccessfullyRegistered"), true,
                            Url.RouteHccUrl(HccRoute.AffiliateDashboard));

                        var ui = DnnUserController.Instance.GetUser(DnnGlobal.Instance.GetPortalId(), aff.UserId);
                        var culture = ui.Profile["UsedCulture"] as string;
                        if (string.IsNullOrEmpty(culture))
                        {
                            culture = "en-US";
                        }

                        HccApp.ContactServices.SendAffiliateConfirmationEmail(aff, culture);
                        if (HccApp.CurrentStore.Settings.AffiliateReview)
                        {
                            HccApp.ContactServices.SendAffiliateReviewEmail(aff);
                        }
                        if (aff.Approved)
                        {
                            HccApp.ContactServices.AffiliateWasApproved(aff, culture);
                        }
                        AssignAffiliateRole(aff.UserId);
                        LoginAffiliateUser(aff);
                        break;
                    case AffiliateRepository.UpdateStatus.DuplicateAffiliateID:
                        result = GetStatusMessage(Localization.GetString("msgAffiliateIdAlreadyExists"), false);
                        break;
                    case AffiliateRepository.UpdateStatus.UserCreateFailed:
                        result = GetUserCreateFailedMessage(createStatus);
                        break;
                    default:
                        result = GetStatusMessage(Localization.GetString("msgUnknownError"), false);
                        break;
                }

                return result;
            }

            return Json(new {Status = "Invalid", Message = GetValidationSummaryMessage()});
        }

        private void LoginAffiliateUser(Affiliate aff)
        {
            var errorMessage = string.Empty;
            string userId = null;

            if (!HccApp.MembershipServices.LoginUser(aff.Username, aff.Password,
                out errorMessage, out userId))
            {
                EventLog.LogEvent(string.Empty, errorMessage, EventLogSeverity.Warning);
            }
        }

        [HccHttpPost]
        public ActionResult GetRegions(string countryId)
        {
            var country = HccApp.GlobalizationServices.Countries.Find(countryId);

            if (country != null)
            {
                return Json(country.Regions);
            }

            return Json(new List<Region>());
        }

        [HccHttpPost]
        public ActionResult IsAffiliateValid(string affiliateId)
        {
            var aff = HccApp.ContactServices.Affiliates.FindByAffiliateId(affiliateId);
            var success = aff != null && aff.Enabled && aff.Approved;

            return Json(success);
        }

        #region Implementation

        private void LoadModel(AffiliateViewModel model)
        {
            var countryId = Country.UnitedStatesCountryBvin;
            var country = HccApp.GlobalizationServices.Countries.Find(countryId);

            model.Countries = HccApp.GlobalizationServices.Countries.FindAll();
            model.CountryId = countryId;
            model.Regions = country.Regions;
            model.ReferralAffiliateId = Request.QueryString[WebAppSettings.AffiliateQueryStringName];
            model.AgreementText = HccApp.CurrentStore.Settings.AffiliateAgreementText;
            model.AllowReferral = HccApp.CurrentStore.Settings.AffiliateDisplayChildren;
        }

        private bool IsUserAlreadyRegisteredAsAffiliate(CustomerAccount acc)
        {
            var aff = HccApp.ContactServices.Affiliates.FindByUserId(acc.Bvin.ConvertTo<int>());
            return aff != null;
        }

        private void LoadModelUserInfo(AffiliateViewModel model, CustomerAccount acc)
        {
            model.UserId = HccApp.CurrentCustomerId.ConvertTo<int>();
            model.Username = acc.Username;
            model.FirstName = HccApp.CurrentCustomer.FirstName;
            model.LastName = HccApp.CurrentCustomer.LastName;
            model.Email = "hotcakes@example.com"; // Fake data. For skipping validation only.
            model.Password = "*";
            model.ConfirmPassword = "*";

            LoadModelAddressData(model);

            var aff = HccApp.ContactServices.Affiliates.FindByUserId(model.UserId.Value);

            if (aff != null)
            {
                model.MyAffiliateId = aff.AffiliateId;
            }
        }

        private void LoadModelAddressData(AffiliateViewModel model)
        {
            var ui = DnnUserController.Instance.GetUser(DnnGlobal.Instance.GetPortalId(), model.UserId.Value);

            UpdateCountryAndRegion(model, ui.Profile.Country, ui.Profile.Region);
            model.Company = ui.Profile["Company"] as string;
            model.AddressLine = ui.Profile.Street;
            model.City = ui.Profile.City;
            model.PostalCode = ui.Profile.PostalCode;
            model.Phone = ui.Profile.Telephone;
        }

        private void UpdateCountryAndRegion(AffiliateViewModel model, string country, string region)
        {
            var gCountry = HccApp.GlobalizationServices.Countries.FindByDisplayName(country);

            if (gCountry != null)
            {
                model.CountryId = gCountry.Bvin;
                model.Regions = gCountry.Regions;
                var gRegion = gCountry.Regions.FirstOrDefault(r => r.DisplayName == region);
                if (gRegion != null)
                {
                    model.State = gRegion.Abbreviation;
                }
            }
            else
            {
                model.CountryId = Country.UnitedStatesCountryBvin;
            }
        }

        private void AssignAffiliateRole(int userId)
        {
            DnnUserController.Instance.AddUserRole(DnnGlobal.Instance.GetPortalId(), userId, AffiliateRoleName);
        }

        private Affiliate LoadAffiliateFromModel(AffiliateViewModel model)
        {
            var sett = HccApp.CurrentStore.Settings;
            var aff = new Affiliate
            {
                UserId = model.UserId ?? Null.NullInteger,
                Username = model.Username,
                Password = model.Password,
                StoreId = HccApp.CurrentStore.Id,
                Enabled = true,
                AffiliateId = model.MyAffiliateId,
                Email = model.Email,
                CommissionAmount = sett.AffiliateCommissionAmount,
                CommissionType = sett.AffiliateCommissionType,
                ReferralDays = sett.AffiliateReferralDays,
                Approved = !sett.AffiliateRequireApproval
            };

            if (sett.AffiliateDisplayChildren)
                aff.ReferralAffiliateId = model.ReferralAffiliateId;

            aff.Address.FirstName = model.FirstName;
            aff.Address.LastName = model.LastName;
            aff.Address.CountryBvin = model.CountryId;
            aff.Address.Company = model.Company;
            aff.Address.Line1 = model.AddressLine;
            aff.Address.City = model.City;
            aff.Address.RegionBvin = model.State;
            aff.Address.PostalCode = model.PostalCode;
            aff.Address.Phone = model.Phone;

            if (model.UserId.HasValue)
            {
                var acc = HccApp.CurrentCustomer;
                aff.Username = acc.Username;
                aff.Email = acc.Email;
                aff.Address.FirstName = acc.FirstName;
                aff.Address.LastName = acc.LastName;
            }

            return aff;
        }

        private JsonResult GetUserCreateFailedMessage(CreateUserStatus createStatus)
        {
            JsonResult result = null;

            switch (createStatus)
            {
                case CreateUserStatus.InvalidPassword:
                    var msg = Localization.GetString("msgInvalidPassword")
                        .Replace("{MinLength}", Membership.Provider.MinRequiredPasswordLength.ToString());
                    result = GetStatusMessage(msg, false);
                    break;
                case CreateUserStatus.DuplicateUsername:
                    result = GetStatusMessage(Localization.GetString("msgUsernameAlreadyExists"), false);
                    break;
                case CreateUserStatus.DuplicateEmail:
                    result = GetStatusMessage(Localization.GetString("msgEmailAlreadyExists"), false);
                    break;
                default:
                    result = GetStatusMessage(Localization.GetString("msgUnknownDatabaseError"), false);
                    break;
            }
            return result;
        }

        #endregion
    }
}