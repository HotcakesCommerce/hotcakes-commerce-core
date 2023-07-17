#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Data.Entity;
using System.Linq;
using DotNetNuke.Entities.Users;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Dnn.Data;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Common.Dnn;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnAffiliateRepository : AffiliateRepository
    {
        public DnnAffiliateRepository(HccRequestContext context)
            : base(context)
        {
            _userCtl = DnnUserController.Instance;
            _portalId = DnnGlobal.Instance.GetPortalId();
        }

        #region Internal declaration

        internal class AffiliateReportHelper
        {
            private readonly bool _applyVATRules;
            private readonly AffiliateReportCriteria _criteria;
            private readonly int _pageNumber;
            private readonly int _pageSize;
            private readonly DnnAffiliateRepository _rep;
            private readonly long _storeId;
            private readonly IRepoStrategy<hcc_Affiliate> _strategy;
            internal int RowCount;

            internal AffiliateReportHelper(DnnAffiliateRepository rep, IRepoStrategy<hcc_Affiliate> strategy,
                AffiliateReportCriteria criteria, int pageNumber, int pageSize)
            {
                _rep = rep;
                _strategy = strategy;
                _criteria = criteria;
                _pageNumber = pageNumber;
                _pageSize = pageSize;
                _storeId = _rep.Context.CurrentStore.Id;
                _applyVATRules = _rep.Context.CurrentStore.Settings.ApplyVATRules;
            }

            internal List<AffiliateReportData> GetReport()
            {
                List<AffiliateReportData> result = null;
                IEnumerable<DnnUser> users = null;

                var query = BuildReportQuery();
                query = FilterQueryBySearchText(query, ref users);

                RowCount = query.Count();

                query = ApplySorting(query);
                result = ApplyPaging(query);

                if (users == null)
                {
                    users = GetDnnUsers(result);
                }

                result.ForEach(i =>
                {
                    var user = users.FirstOrDefault(u => u.UserID == i.UserId);

                    if (user != null)
                    {
                        i.Company = user.ProfileCompany;
                        i.FirstName = user.FirstName;
                        i.LastName = user.LastName;
                        i.Email = user.Email;

                        if (string.IsNullOrEmpty(i.FirstName) && string.IsNullOrEmpty(i.LastName))
                        {
                            i.LastName = user.DisplayName;
                        }
                    }
                    else
                    {
                        i.UserId = -1;
                        i.LastName = "[USER ACCOUNT DELETED]";
                    }
                });

                return result;
            }

            internal AffiliateReportTotals GetTotals()
            {
                IEnumerable<DnnUser> users = null;

                var query = BuildReportQuery();
                query = FilterQueryBySearchText(query, ref users);

                return new AffiliateReportTotals
                {
                    OrdersCount = query.Sum(i => (int?) i.OrdersCount) ?? 0,
                    SalesAmount = query.Sum(i => (decimal?) i.SalesAmount) ?? 0,
                    Commission = query.Sum(i => (decimal?) i.Commission) ?? 0,
                    CommissionPaid = query.Sum(i => (decimal?) i.CommissionPayed) ?? 0,
                    PaymentsCount = query.Sum(i => (int?) i.PaymentsCount) ?? 0
                };
            }

            internal AffiliateReportTotals GetTotals(TotalsReturnType returnType)
            {
                IEnumerable<DnnUser> users = null;

                var query = BuildReportQuery();
                query = FilterQueryBySearchText(query, ref users);

                var totals = new AffiliateReportTotals();

                if (returnType == TotalsReturnType.ReturnAll || returnType == TotalsReturnType.ReturnReferrals)
                {
                    totals.ReferralsCount = query.Sum(i => (int?) i.SignupsCount) ?? 0;
                }

                if (returnType == TotalsReturnType.ReturnAll || returnType == TotalsReturnType.ReturnOrders)
                {
                    totals.OrdersCount = query.Sum(i => (int?) i.OrdersCount) ?? 0;
                    totals.SalesAmount = query.Sum(i => (decimal?) i.SalesAmount) ?? 0;
                }

                if (returnType == TotalsReturnType.ReturnAll || returnType == TotalsReturnType.ReturnPayments)
                {
                    totals.Commission = query.Sum(i => (decimal?) i.Commission) ?? 0;
                    totals.CommissionPaid = query.Sum(i => (decimal?) i.CommissionPayed) ?? 0;
                    totals.PaymentsCount = query.Sum(i => (int?) i.PaymentsCount) ?? 0;
                }

                return totals;
            }

            internal AffiliateReportTotals GetAffiliateTotals(long affId)
            {
                IEnumerable<DnnUser> users = null;

                var query = BuildReportQuery().Where(a => a.Id == affId);
                query = FilterQueryBySearchText(query, ref users);

                return new AffiliateReportTotals
                {
                    ReferralsCount = query.Sum(i => (int?) i.SignupsCount) ?? 0,
                    OrdersCount = query.Sum(i => (int?) i.OrdersCount) ?? 0,
                    SalesAmount = query.Sum(i => (decimal?) i.SalesAmount) ?? 0,
                    Commission = query.Sum(i => (decimal?) i.Commission) ?? 0,
                    CommissionPaid = query.Sum(i => (decimal?) i.CommissionPayed) ?? 0,
                    PaymentsCount = query.Sum(i => (int?) i.PaymentsCount) ?? 0
                };
            }

            internal AffiliateReportTotals GetAffiliateTotals(long affId, TotalsReturnType returnType)
            {
                IEnumerable<DnnUser> users = null;

                var query = BuildReportQuery().Where(a => a.Id == affId);
                query = FilterQueryBySearchText(query, ref users);

                var totals = new AffiliateReportTotals();

                if (returnType == TotalsReturnType.ReturnAll || returnType == TotalsReturnType.ReturnReferrals)
                {
                    totals.ReferralsCount = query.Sum(i => (int?) i.SignupsCount) ?? 0;
                }

                if (returnType == TotalsReturnType.ReturnAll || returnType == TotalsReturnType.ReturnOrders)
                {
                    totals.OrdersCount = query.Sum(i => (int?) i.OrdersCount) ?? 0;
                    totals.SalesAmount = query.Sum(i => (decimal?) i.SalesAmount) ?? 0;
                }

                if (returnType == TotalsReturnType.ReturnAll || returnType == TotalsReturnType.ReturnPayments)
                {
                    totals.Commission = query.Sum(i => (decimal?) i.Commission) ?? 0;
                    totals.CommissionPaid = query.Sum(i => (decimal?) i.CommissionPayed) ?? 0;
                    totals.PaymentsCount = query.Sum(i => (int?) i.PaymentsCount) ?? 0;
                }
                return totals;
            }

            #region Implementation

            private IQueryable<AffiliateReportData> BuildReportQuery()
            {
                var orders = _strategy.GetQuery<hcc_Order>()
                    .AsNoTracking()
                    .Where(o => o.StoreId == _storeId)
                    .Where(o => o.TimeOfOrder >= _criteria.StartDateUtc && o.TimeOfOrder <= _criteria.EndDateUtc);

                var signups = _strategy.GetQuery()
                    .AsNoTracking()
                    .Where(i => i.CreationDate >= _criteria.StartDateUtc && i.CreationDate <= _criteria.EndDateUtc);

                var payments = _strategy.GetQuery<hcc_AffiliatePayments>()
                    .AsNoTracking()
                    .Where(i => i.PaymentDate >= _criteria.StartDateUtc && i.PaymentDate <= _criteria.EndDateUtc);

                var affiliates = _strategy.GetQuery().AsNoTracking().Where(a => a.StoreId == _storeId);

                if (!string.IsNullOrEmpty(_criteria.ReferralAffiliateID))
                    affiliates = affiliates.Where(a => a.ReferralID == _criteria.ReferralAffiliateID);

                if (_criteria.ShowOnlyNonApproved)
                    affiliates = affiliates.Where(a => !a.Approved);

                var query = affiliates.GroupJoin(orders, a => a.Id, o => o.AffiliateId,
                    (a, oColl) => new {Affiliate = a, Orders = oColl})
                    .GroupJoin(signups, ao => ao.Affiliate.AffiliateID, s => s.ReferralID,
                        (ao, sColl) => new {ao.Affiliate, ao.Orders, Signups = sColl})
                    .GroupJoin(payments, aos => aos.Affiliate.Id, p => p.AffiliateId,
                        (aos, pColl) => new {aos.Affiliate, aos.Orders, aos.Signups, Payments = pColl})
                    .Select(aosp => new AffiliateReportData
                    {
                        Id = aosp.Affiliate.Id,
                        AffiliateId = aosp.Affiliate.AffiliateID,
                        UserId = aosp.Affiliate.UserId,
                        OrdersCount = aosp.Orders.Count(),
                        SignupsCount = aosp.Signups.Count(),
                        SalesAmount =
                            _applyVATRules
                                ? aosp.Orders.Sum(o => (decimal?) o.SubTotal - (decimal?) o.ItemsTax) ?? 0
                                : aosp.Orders.Sum(o => (decimal?) o.SubTotal + (decimal?) o.OrderDiscounts) ?? 0,
                        // TODO: Old Affiliate Report use TotalOrderBeforeDiscounts
                        Commission = aosp.Affiliate.CommissionType == 2
                            ? Math.Round(aosp.Affiliate.CommissionAmount*aosp.Orders.Count(), 2)
                            : Math.Round(
                                aosp.Affiliate.CommissionAmount/100*
                                (_applyVATRules
                                    ? aosp.Orders.Sum(o => (decimal?) o.SubTotal - (decimal?) o.ItemsTax) ?? 0
                                    : aosp.Orders.Sum(o => (decimal?) o.SubTotal + (decimal?) o.OrderDiscounts) ?? 0), 2),
                        CommissionPayed = aosp.Payments.Sum(p => (decimal?) p.PaymentAmount) ?? 0,
                        PaymentsCount = aosp.Payments.Count()
                    });

                if (_criteria.ShowCommissionOwed)
                    query = query.Where(i => ((decimal?) (i.Commission - i.CommissionPayed) ?? 0) > 0);

                return query;
            }

            private IQueryable<AffiliateReportData> FilterQueryBySearchText(IQueryable<AffiliateReportData> query,
                ref IEnumerable<DnnUser> users)
            {
                if (!string.IsNullOrEmpty(_criteria.SearchText))
                {
                    if (_criteria.SearchBy == AffiliateReportCriteria.SearchType.AffiliateId)
                    {
                        query = query.Where(i => i.AffiliateId.Contains(_criteria.SearchText));
                    }
                    else
                    {
                        users = _rep._userCtl.GetUsersFromDb(_rep._portalId, query.Select(i => i.UserId).ToList());

                        switch (_criteria.SearchBy)
                        {
                            case AffiliateReportCriteria.SearchType.LastName:
                                users = users.Where(u => u.LastName.ToLower().Contains(_criteria.SearchText.ToLower()));
                                break;
                            case AffiliateReportCriteria.SearchType.Email:
                                users = users.Where(u => u.Email.ToLower().Contains(_criteria.SearchText.ToLower()));
                                break;
                            case AffiliateReportCriteria.SearchType.CompanyName:
                                users =
                                    users.Where(
                                        u =>
                                            u.ProfileCompany != null &&
                                            u.ProfileCompany.ToLower().Contains(_criteria.SearchText.ToLower()));
                                break;
                            default:
                                break;
                        }

                        var userIds = users.Select(u => u.UserID).ToList();
                        query = query.Where(i => userIds.Contains(i.UserId));
                    }
                }

                return query;
            }

            private IQueryable<AffiliateReportData> ApplySorting(IQueryable<AffiliateReportData> query)
            {
                switch (_criteria.SortBy)
                {
                    case AffiliateReportCriteria.SortingType.Sales:
                        query = query.OrderByDescending(i => i.SalesAmount);
                        break;
                    case AffiliateReportCriteria.SortingType.Orders:
                        query = query.OrderByDescending(i => i.OrdersCount);
                        break;
                    case AffiliateReportCriteria.SortingType.Commission:
                        query = query.OrderByDescending(i => i.Commission);
                        break;
                    case AffiliateReportCriteria.SortingType.Signups:
                        query = query.OrderByDescending(i => i.SignupsCount);
                        break;
                    default:
                        break;
                }
                return query;
            }

            private List<AffiliateReportData> ApplyPaging(IQueryable<AffiliateReportData> query)
            {
                return query.Skip((_pageNumber - 1)*_pageSize).Take(_pageSize).ToList();
            }

            private IEnumerable<DnnUser> GetDnnUsers(List<AffiliateReportData> result)
            {
                return _rep._userCtl.GetUsersFromDb(_rep._portalId, result.Select(i => i.UserId).ToList());
            }

            #endregion
        }

        #endregion

        #region Fields

        private readonly IUserController _userCtl;
        private readonly int _portalId;

        #endregion

        #region Public methods

        public override UpdateStatus Create(Affiliate aff, ref CreateUserStatus userStatus)
        {
            if (string.IsNullOrEmpty(aff.AffiliateId))
                throw new ArgumentException("Property aff.AffiliateId can not be empty or null");

            if (FindByAffiliateId(aff.AffiliateId) != null)
            {
                return UpdateStatus.DuplicateAffiliateID;
            }

            var uInfo = _userCtl.GetUser(_portalId, aff.UserId);

            if (uInfo == null)
            {
                if (FindByUserId(aff.UserId) != null)
                {
                    throw new ArgumentException("Value aff.UserId already exists in database. ID:" + aff.UserId);
                }

                uInfo = _userCtl.BuildUserInfo(
                    aff.Username,
                    aff.Address.FirstName,
                    aff.Address.LastName,
                    aff.Email,
                    aff.Password,
                    _portalId);

                if (string.IsNullOrEmpty(uInfo.Profile["UsedCulture"] as string))
                {
                    uInfo.Profile.EnsureProfileProperty("UsedCulture", "Hotcakes", _portalId, false);
                    uInfo.Profile.SetProfileProperty("UsedCulture", Context.MainContentCulture);
                }

                UpdateUserInfo(uInfo, aff);
                userStatus = _userCtl.CreateUser(ref uInfo);
                aff.UserId = uInfo.UserID;
            }
            else
            {
                UpdateUserInfo(uInfo, aff);
                _userCtl.UpdateUser(_portalId, uInfo);
                userStatus = CreateUserStatus.Success;
            }

            if (userStatus == CreateUserStatus.Success)
            {
                return base.Create(aff, ref userStatus);
            }

            return UpdateStatus.UserCreateFailed;
        }

        public override UpdateStatus Update(Affiliate aff)
        {
            if (string.IsNullOrEmpty(aff.AffiliateId))
                throw new ArgumentException("Property aff.AffiliateId can not be empty or null");

            var dupAff = FindByAffiliateId(aff.AffiliateId);
            if (dupAff != null && dupAff.Id != aff.Id)
            {
                return UpdateStatus.DuplicateAffiliateID;
            }

            var uInfo = _userCtl.GetUser(_portalId, dupAff.UserId);
            if (uInfo == null)
                throw new ArgumentException("aff.UserId has invalid value");

            UpdateUserInfo(uInfo, aff);
            _userCtl.UpdateUser(_portalId, uInfo);

            return base.Update(aff);
        }

        public override bool Delete(long id)
        {
            if (base.Delete(id))
            {
                return true;
            }
            return false;
        }

        public override List<AffiliateReportData> FindAllWithFilter(AffiliateReportCriteria criteria, int pageNumber,
            int pageSize, ref int rowCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var helper = new AffiliateReportHelper(this, strategy, criteria, pageNumber, pageSize);
                var list = helper.GetReport();
                rowCount = helper.RowCount;
                return list;
            }
        }

        public override AffiliateReportTotals GetTotalsByFilter(AffiliateReportCriteria criteria)
        {
            using (var strategy = CreateReadStrategy())
            {
                var helper = new AffiliateReportHelper(this, strategy, criteria, 1, int.MaxValue);
                return helper.GetTotals();
            }
        }

        public override AffiliateReportTotals GetTotalsByFilter(AffiliateReportCriteria criteria,
            TotalsReturnType returnType)
        {
            using (var strategy = CreateReadStrategy())
            {
                var helper = new AffiliateReportHelper(this, strategy, criteria, 1, int.MaxValue);
                return helper.GetTotals(returnType);
            }
        }

        public override AffiliateReportTotals GetAffiliateTotals(long affId, AffiliateReportCriteria criteria)
        {
            using (var strategy = CreateReadStrategy())
            {
                var helper = new AffiliateReportHelper(this, strategy, criteria, 1, int.MaxValue);
                return helper.GetAffiliateTotals(affId);
            }
        }

        public override AffiliateReportTotals GetAffiliateTotals(long affId, AffiliateReportCriteria criteria,
            TotalsReturnType returnType)
        {
            using (var strategy = CreateReadStrategy())
            {
                var helper = new AffiliateReportHelper(this, strategy, criteria, 1, int.MaxValue);
                return helper.GetAffiliateTotals(affId, returnType);
            }
        }

        #endregion

        #region Implementation

        protected override void CopyDataToModel(hcc_Affiliate data, Affiliate model)
        {
            base.CopyDataToModel(data, model);
            var user = _userCtl.GetUser(_portalId, data.UserId);
            MergeAffiliate(model, user);
        }

        protected override List<Affiliate> ListPoco(IEnumerable<hcc_Affiliate> items)
        {
            var result = new List<Affiliate>();

            if (items != null)
            {
                var users = _userCtl.GetUsersFromDb(_portalId, items.Select(i => i.UserId).ToList());

                foreach (var item in items)
                {
                    var temp = new Affiliate();
                    base.CopyDataToModel(item, temp);
                    MergeAffiliate(temp, users.FirstOrDefault(u => u.UserID == item.UserId));
                    GetSubItems(temp);
                    result.Add(temp);
                }
            }

            return result;
        }

        private void UpdateUserInfo(UserInfo user, Affiliate aff)
        {
            user.Username = aff.Username;
            user.FirstName = aff.Address.FirstName;
            user.LastName = aff.Address.LastName;
            user.Email = aff.Email;

            user.Profile.Country = aff.Address.CountryDisplayName;

            user.Profile.EnsureProfileProperty("Company", "Contact Info", _portalId, true);
            user.Profile.SetProfileProperty("Company", aff.Address.Company);
            user.Profile.Street = aff.Address.Street;
            user.Profile.Region = aff.Address.RegionDisplayName;
            user.Profile.City = aff.Address.City;
            user.Profile.PostalCode = aff.Address.PostalCode;
            user.Profile.Telephone = aff.Address.Phone;
        }

        private void MergeAffiliate(Affiliate aff, UserInfo user)
        {
            if (user != null)
            {
                aff.Username = user.Username;
                aff.Email = user.Email;
                aff.Address.FirstName = user.FirstName;
                aff.Address.LastName = user.LastName;

                UpdateCountryAndRegion(aff.Address, user.Profile.Country, user.Profile.Region);
                aff.Address.Company = user.Profile["Company"] as string ?? string.Empty;
                aff.Address.Line1 = user.Profile.Street;
                aff.Address.City = user.Profile.City;
                aff.Address.PostalCode = user.Profile.PostalCode;
                aff.Address.Phone = user.Profile.Telephone;
            }
            else
            {
                aff.UserId = -1;
                aff.Username = "[USER ACCOUNT DELETED]";
                aff.Address.LastName = "[USER ACCOUNT DELETED]";
            }
        }

        private void MergeAffiliate(Affiliate aff, DnnUser user)
        {
            if (user == null) return;

            aff.Username = user.Username;
            aff.Address.FirstName = user.FirstName;
            aff.Address.LastName = user.LastName;

            aff.Address.Company = user.ProfileCompany;
            aff.Address.Line1 = user.ProfileStreet;
            aff.Address.City = user.ProfileCity;
            aff.Address.PostalCode = user.ProfilePostalCode;
            aff.Address.Phone = user.ProfileTelephone;
        }

        private void UpdateCountryAndRegion(Address addr, string country, string region)
        {
            var gCountry = _countryRepository.FindByDisplayName(country);

            if (gCountry != null)
            {
                addr.CountryBvin = gCountry.Bvin;
                var gRegion = gCountry.Regions.FirstOrDefault(r => r.DisplayName == region);
                if (gRegion != null)
                {
                    addr.RegionBvin = gRegion.Abbreviation;
                }
            }
            else
            {
                addr.CountryBvin = Country.UnitedStatesCountryBvin;
            }
        }

        #endregion
    }
}