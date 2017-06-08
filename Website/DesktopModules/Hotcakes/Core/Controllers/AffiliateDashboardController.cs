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
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;
using Newtonsoft.Json;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class AffiliateDashboardController : BaseAppController
    {
        private const DateRangeType DefaultDateRange = DateRangeType.ThisMonth;

        public ActionResult Index()
        {
            var model = new AffiliateDashboardViewModel();
            var aff = HccApp.ContactServices.Affiliates.FindByUserId(HccApp.CurrentCustomerId.ConvertTo<int>());
            if (aff == null)
                return View((AffiliateDashboardViewModel) null);

            var affModel = new AffiliateViewModel(aff);
            LoadModel(affModel);
            FillRequiredFields(affModel);
            model.Affiliate = affModel;

            var range = new DateRange {RangeType = DefaultDateRange};
            range.CalculateDatesFromType(DateTime.Now);

            var totals = HccApp.ContactServices.Affiliates.GetAffiliateTotals(aff.Id, new AffiliateReportCriteria
            {
                StartDateUtc = range.StartDate,
                EndDateUtc = range.EndDate
            });

            var totalRowCount = 0;
            model.Orders = new AffiliateOrdersViewModel
            {
                Orders = GetOrders(aff, range.StartDate, range.EndDate, 1, 5, ref totalRowCount),
                TotalCount = totals.OrdersCount,
                TotalAmount = totals.SalesAmount.ToString("C")
            };

            int totalPayments;
            model.Payments = new AffiliatePaymentsViewModel
            {
                Payments = GetPayments(aff, range.StartDate, range.EndDate, 1, 5, out totalPayments),
                TotalCount = totals.PaymentsCount,
                TotalAmount = totals.CommissionPaid.ToString("C")
            };

            if (HccApp.CurrentStore.Settings.AffiliateDisplayChildren)
            {
                model.Affiliate.AllowReferral = true;
                var totalsReferrals = HccApp.ContactServices.Affiliates.GetTotalsByFilter(new AffiliateReportCriteria
                {
                    ReferralAffiliateID = aff.AffiliateId,
                    SearchBy = AffiliateReportCriteria.SearchType.AffiliateId,
                    SearchText = null
                }, AffiliateRepository.TotalsReturnType.ReturnPayments);

                int totalReferals;
                model.Referrals = new AffiliateReferralsViewModel
                {
                    Referrals =
                        GetReferrals(aff, AffiliateReportCriteria.SearchType.AffiliateId, null, 1, 5, out totalReferals),
                    TotalCount = totalReferals,
                    TotalAmount = totalsReferrals.Commission.ToString("C")
                };
            }

#pragma warning disable 0612, 0618
            model.TimeRangesLocalized = GetTimeRangesLocalized();
#pragma warning restore 0612, 0618

            //model.DefaultDateRange = (int)DefaultDateRange;
            model.UrlBuilder = new AffiliateUrlBuilderViewModel
            {
                AffiliateId = model.Affiliate.MyAffiliateId,
                Categories = GetCategoriesList(),
                RegistrationUrl = Url.RouteHccUrl(HccRoute.AffiliateRegistration)
            };

            return View(model);
        }

        [HccHttpPost]
        public ActionResult Update(AffiliateViewModel model)
        {
            if (ModelState.IsValid)
            {
                JsonResult result = null;
                var status = AffiliateRepository.UpdateStatus.UnknownError;

                var affiliates = HccApp.ContactServices.Affiliates;
                var aff = affiliates.Find(model.Id);

                if (aff != null)
                {
                    aff.AffiliateId = model.MyAffiliateId;
                    aff.Address.FirstName = model.FirstName;
                    aff.Address.LastName = model.LastName;
                    aff.Address.CountryBvin = model.CountryId;
                    aff.Address.Company = model.Company;
                    aff.Address.Line1 = model.AddressLine;
                    aff.Address.City = model.City;
                    aff.Address.RegionBvin = model.State;
                    aff.Address.PostalCode = model.PostalCode;
                    aff.Address.Phone = model.Phone;

                    if (!aff.Approved && HccApp.CurrentStore.Settings.AffiliateDisplayChildren)
                        aff.ReferralAffiliateId = model.ReferralAffiliateId;

                    status = affiliates.Update(aff);
                }

                switch (status)
                {
                    case AffiliateRepository.UpdateStatus.Success:
                        result = GetStatusMessage(Localization.GetString("msgAffiliateSuccessfullyUpdated"), true);
                        break;
                    case AffiliateRepository.UpdateStatus.DuplicateAffiliateID:
                        result = GetStatusMessage(Localization.GetString("msgAffiliateIdAlreadyExists"), false);
                        break;
                    default:
                        result = GetStatusMessage(Localization.GetString("msgUnknownError"), false);
                        break;
                }

                return result;
            }

            return Json(new {Status = "Invalid", Message = GetValidationSummaryMessage()});
        }

        [HccHttpPost]
        public string GetOrdersReport(int dateRange, int pageNumber, int pageSize)
        {
            var range = new DateRange {RangeType = (DateRangeType) dateRange};
            range.CalculateDatesFromType(DateTime.Now);

            var aff = HccApp.ContactServices.Affiliates.FindByUserId(HccApp.CurrentCustomerId.ConvertTo<int>());

            var totals = HccApp.ContactServices.Affiliates.GetAffiliateTotals(aff.Id, new AffiliateReportCriteria
            {
                StartDateUtc = range.StartDate,
                EndDateUtc = range.EndDate
            }, AffiliateRepository.TotalsReturnType.ReturnOrders);

            var totalRowCount = 0;
            var orders = GetOrders(aff, range.StartDate, range.EndDate, pageNumber, pageSize, ref totalRowCount);

            return
                JsonConvert.SerializeObject(new AffiliateOrdersViewModel
                {
                    Orders = orders,
                    TotalCount = totals.OrdersCount,
                    TotalAmount = totals.SalesAmount.ToString("C")
                });
        }

        [HccHttpPost]
        public string GetPaymentsReport(int dateRange, int pageNumber, int pageSize)
        {
            var range = new DateRange {RangeType = (DateRangeType) dateRange};
            range.CalculateDatesFromType(DateTime.Now);
            var aff = HccApp.ContactServices.Affiliates.FindByUserId(HccApp.CurrentCustomerId.ConvertTo<int>());

            var totals = HccApp.ContactServices.Affiliates.GetAffiliateTotals(aff.Id, new AffiliateReportCriteria
            {
                StartDateUtc = range.StartDate,
                EndDateUtc = range.EndDate
            }, AffiliateRepository.TotalsReturnType.ReturnPayments);

            int total;
            var payments = GetPayments(aff, range.StartDate, range.EndDate, pageNumber, pageSize, out total);

            return
                JsonConvert.SerializeObject(new AffiliatePaymentsViewModel
                {
                    Payments = payments,
                    TotalCount = totals.PaymentsCount,
                    TotalAmount = totals.Commission.ToString("C")
                });
        }


        [HccHttpPost]
        public string GetReferralsReport(int searchBy, string searchText, int pageNumber, int pageSize)
        {
            var aff = HccApp.ContactServices.Affiliates.FindByUserId(HccApp.CurrentCustomerId.ConvertTo<int>());

            var totalsReferrals = HccApp.ContactServices.Affiliates.GetTotalsByFilter(new AffiliateReportCriteria
            {
                ReferralAffiliateID = aff.AffiliateId,
                SearchBy = (AffiliateReportCriteria.SearchType) searchBy,
                SearchText = searchText
            }, AffiliateRepository.TotalsReturnType.ReturnPayments);

            int total;
            var referrals = GetReferrals(aff, (AffiliateReportCriteria.SearchType) searchBy, searchText, pageNumber,
                pageSize, out total);

            return
                JsonConvert.SerializeObject(new AffiliateReferralsViewModel
                {
                    Referrals = referrals,
                    TotalCount = total,
                    TotalAmount = totalsReferrals.Commission.ToString("C")
                });
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
        public ActionResult GetProducts(string categoryId)
        {
            var totalResults = 0;
            var aff = HccApp.ContactServices.Affiliates.FindByUserId(HccApp.CurrentCustomerId.ConvertTo<int>());

            List<Product> products = null;
            if (string.IsNullOrEmpty(categoryId))
            {
                products =
                    HccApp.CatalogServices.Products.FindByCriteria(new ProductSearchCriteria {NotCategorized = true}, 1,
                        1000, ref totalResults);
            }
            else
            {
                products = HccApp.CatalogServices.FindProductForCategoryWithSort(categoryId,
                    CategorySortOrder.ProductName, false, 1, 1000, ref totalResults);
            }

            var items = products.Select(p => new ListItem {Value = p.Bvin, Text = p.ProductName}).ToList();
            return Json(items);
        }

        [HccHttpPost]
        public ActionResult GenerateUrl(string id, string mode)
        {
            var url = string.Empty;
            var aff = HccApp.ContactServices.Affiliates.FindByUserId(HccApp.CurrentCustomerId.ConvertTo<int>());

            switch (mode)
            {
                case "Category":
                    var cat = HccApp.CatalogServices.Categories.FindManySnapshots(new List<string> {id}).First();
                    url = UrlRewriter.BuildUrlForCategory(cat);
                    url += "?" + WebAppSettings.AffiliateQueryStringName + "=" + aff.AffiliateId;
                    break;
                case "Product":
                    var prod = HccApp.CatalogServices.Products.FindWithCache(id);
                    url = UrlRewriter.BuildUrlForProduct(prod);
                    url += "?" + WebAppSettings.AffiliateQueryStringName + "=" + aff.AffiliateId;
                    break;
                default:
                    url = id + "?" + WebAppSettings.AffiliateQueryStringName + "=" + aff.AffiliateId;
                    break;
            }

            return Json(url);
        }

        #region Implementation

        private void FillRequiredFields(AffiliateViewModel model)
        {
            model.Password = "*";
            model.ConfirmPassword = "*";
        }

        protected decimal GetCommission(Affiliate affiliate, decimal orderTotal)
        {
            var comm = affiliate.CommissionType == AffiliateCommissionType.FlatRateCommission
                ? affiliate.CommissionAmount
                : Math.Round(affiliate.CommissionAmount/100*orderTotal, 2);

            return comm;
        }

        private void LoadModel(AffiliateViewModel model)
        {
            if (string.IsNullOrEmpty(model.CountryId))
                model.CountryId = Country.UnitedStatesCountryBvin;
            var country = HccApp.GlobalizationServices.Countries.Find(model.CountryId);

            model.Countries = HccApp.GlobalizationServices.Countries.FindAll();
            model.Regions = country.Regions;
            model.AgreementText = HccApp.CurrentStore.Settings.AffiliateAgreementText;
        }

        private List<AffiliatePaymentsViewModel.PaymentViewModel> GetPayments(Affiliate aff, DateTime start,
            DateTime end, int pageNumber,
            int pageSize, out int totalRowCount)
        {
            var payments =
                HccApp.ContactServices.AffiliatePayments.FindAllPaged(aff.Id, start, end, pageNumber, pageSize,
                    out totalRowCount)
                    .Select(p => new AffiliatePaymentsViewModel.PaymentViewModel
                    {
                        Amount = p.PaymentAmount.ToString("c"),
                        AttachmentUrl =
                            string.IsNullOrEmpty(p.FileName)
                                ? string.Empty
                                : DiskStorage.PaymentsAttachmentUrl(HccApp.CurrentStore.Id, p.FileName),
                        Notes = p.Notes
                    })
                    .ToList();

            return payments;
        }

        private List<AffiliateReferralsViewModel.ReferalViewModel> GetReferrals(Affiliate aff,
            AffiliateReportCriteria.SearchType searchBy, string searchText, int pageNumber, int pageSize,
            out int totalRowCount)
        {
            var criteria = new AffiliateReportCriteria
            {
                SearchBy = searchBy,
                SearchText = searchText,
                ReferralAffiliateID = aff.AffiliateId
            };
            var total = 0;
            var referrals = HccApp.ContactServices.Affiliates.FindAllWithFilter(criteria, pageNumber, pageSize,
                ref total);
            totalRowCount = total;

            return referrals
                .Select(r => new AffiliateReferralsViewModel.ReferalViewModel
                {
                    Id = r.AffiliateId,
                    Name = r.FullName,
                    Revenue = r.Commission.ToString("C"),
                    Email = r.Email
                })
                .ToList();
        }


        private List<AffiliateOrdersViewModel.OrderViewModel> GetOrders(Affiliate aff, DateTime start, DateTime end,
            int pageNumber, int pageSize, ref int totalRowCount)
        {
            var criteria = new OrderSearchCriteria
            {
                AffiliateId = aff.Id,
                StartDateUtc = start,
                EndDateUtc = end
            };

            return HccApp.OrderServices.Orders.FindByCriteriaPaged(criteria, pageNumber, pageSize, ref totalRowCount)
                .Select(o => new AffiliateOrdersViewModel.OrderViewModel
                {
                    OrderNumber = o.OrderNumber,
                    OrderDate = DateHelper.ConvertUtcToStoreTime(HccApp, o.TimeOfOrderUtc),
                    Amount = o.TotalGrand.ToString("c"),
                    Commission = GetCommission(aff, o.TotalGrand).ToString("c")
                })
                .ToList();
        }

        private JsonResult GetStatusMessage(string message, bool isSuccess)
        {
            return Json(new {Status = isSuccess ? "OK" : "Failed", Message = message});
        }

        private List<ListItem> GetCategoriesList()
        {
            var allCats = HccApp.CatalogServices.Categories.FindAllSnapshotsPaged(1, 5000);
            var items = new List<ListItem> { new ListItem { Text = Localization.GetString("NotCategorized"), Value = string.Empty } };
            AddChildrenItems(items, string.Empty, 0, allCats);
            return items;
        }

        private Dictionary<int, string> GetTimeRangesLocalized()
        {
            var result = new Dictionary<int, string>();

            foreach (var t in Enum.GetValues(typeof (DateRangeType)).OfType<DateRangeType>())
            {
                if (t != DateRangeType.Custom && t != DateRangeType.None)
                {
                    result.Add((int) t, Localization.GetString(t.ToString()));
                }
            }
            return result;
        }

        private void AddChildrenItems(List<ListItem> items, string parentId, int indentLevel,
            List<CategorySnapshot> allCats)
        {
            var cats = allCats.Where(c => !c.Hidden && c.ParentId == parentId).OrderBy(c => c.Name);
            foreach (var cat in cats)
            {
                items.Add(new ListItem {Value = cat.Bvin, Text = new string('.', indentLevel*3) + cat.Name});
                AddChildrenItems(items, cat.Bvin, indentLevel + 1, allCats);
            }
        }

        #endregion
    }
}