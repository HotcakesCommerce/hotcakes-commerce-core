#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce.Contacts
{
    public abstract class AffiliateRepository : HccSimpleRepoBase<hcc_Affiliate, Affiliate>
    {
        public enum TotalsReturnType
        {
            ReturnAll,
            ReturnOrders,
            ReturnPayments,
            ReturnReferrals
        }

        public enum UpdateStatus
        {
            Success = 0,
            DuplicateAffiliateID = 1,
            UserCreateFailed = 2,
            UnknownError = 99
        }

        public const string AffiliateIDValidationExpression = @"[-\w]*";

        public AffiliateRepository(HccRequestContext context)
            : base(context)
        {
            _countryRepository = Factory.CreateRepo<CountryRepository>(context);
            _affiliatePaymentRepository = Factory.CreateRepo<AffiliatePaymentRepository>(context);
            _affiliateReferralRepository = Factory.CreateRepo<AffiliateReferralRepository>(context);
        }

        protected override void CopyDataToModel(hcc_Affiliate data, Affiliate model)
        {
            model.CommissionAmount = data.CommissionAmount;
            model.CommissionType = (AffiliateCommissionType) data.CommissionType;
            model.DriversLicenseNumber = data.DriversLicenseNumber;
            model.Approved = data.Approved;
            model.Enabled = data.Enabled;
            model.Id = data.Id;
            model.UserId = data.UserId;
            model.LastUpdatedUtc = data.LastUpdated;
            model.CreationDate = data.CreationDate;
            model.Notes = data.Notes;
            model.ReferralDays = data.ReferralDays;
            model.AffiliateId = data.AffiliateID;
            model.ReferralAffiliateId = data.ReferralID;
            model.StoreId = data.StoreId;
            model.TaxId = data.TaxID;
            model.WebSiteUrl = data.WebSiteURL;
        }

        protected override void CopyModelToData(hcc_Affiliate data, Affiliate model)
        {
            data.CommissionAmount = model.CommissionAmount;
            data.CommissionType = (int) model.CommissionType;
            data.DriversLicenseNumber = model.DriversLicenseNumber;
            data.Approved = model.Approved;
            data.Enabled = model.Enabled;
            data.Id = model.Id;
            data.UserId = model.UserId;
            data.LastUpdated = model.LastUpdatedUtc;
            data.CreationDate = model.CreationDate;
            data.Notes = model.Notes;
            data.ReferralDays = model.ReferralDays;
            data.AffiliateID = model.AffiliateId;
            data.ReferralID = model.ReferralAffiliateId;
            data.StoreId = model.StoreId;
            data.TaxID = model.TaxId;
            data.WebSiteURL = model.WebSiteUrl;
        }

        #region Fields

        protected CountryRepository _countryRepository;
        protected AffiliatePaymentRepository _affiliatePaymentRepository;
        protected AffiliateReferralRepository _affiliateReferralRepository;

        #endregion

        #region Public methods

        public Affiliate Find(long id)
        {
            var result = FindForAllStores(id);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        public Affiliate FindForAllStores(long id)
        {
            return FindFirstPoco(a => a.Id == id);
        }

        public Affiliate FindForByStoreId(long StoreId)
        {
            return FindFirstPoco(a => a.StoreId == StoreId);
        }

        public override bool Create(Affiliate item)
        {
            throw new NotImplementedException("Use Create(Affiliate item, ref CreateUserStatus userStatus) instead");
        }

        public virtual UpdateStatus Create(Affiliate aff, ref CreateUserStatus userStatus)
        {
            aff.CreationDate = DateTime.UtcNow;
            aff.LastUpdatedUtc = DateTime.UtcNow;
            aff.StoreId = Context.CurrentStore.Id;

            return base.Create(aff) ? UpdateStatus.Success : UpdateStatus.UnknownError;
        }

        public virtual UpdateStatus Update(Affiliate aff)
        {
            if (aff.StoreId != Context.CurrentStore.Id)
            {
                throw new ArgumentException("Argument 'aff.StoreId' has invalid value");
            }

            aff.LastUpdatedUtc = DateTime.UtcNow;
            return Update(aff, a => a.Id == aff.Id) ? UpdateStatus.Success : UpdateStatus.UnknownError;
        }

        public virtual bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;

            var existing = FindForAllStores(id);
            if (existing != null && existing.StoreId == storeId)
            {
                return Delete(a => a.Id == id);
            }
            return false;
        }

        public virtual bool DeleteByUserID(string UserID)
        {
            var storeId = Context.CurrentStore.Id;
            var intUserID = Convert.ToInt32(UserID);
            return Delete(a => a.UserId == intUserID && a.StoreId == storeId);
        }

        public Affiliate FindByAffiliateId(string affiliateId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(a => a.StoreId == storeId && a.AffiliateID == affiliateId);
        }

        public Affiliate FindByUserId(int userId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(a => a.StoreId == storeId && a.UserId == userId);
        }

        public abstract List<AffiliateReportData> FindAllWithFilter(AffiliateReportCriteria criteria, int pageNumber,
            int pageSize, ref int rowCount);

        public abstract AffiliateReportTotals GetTotalsByFilter(AffiliateReportCriteria criteria);

        public abstract AffiliateReportTotals GetTotalsByFilter(AffiliateReportCriteria criteria,
            TotalsReturnType returnType);

        public abstract AffiliateReportTotals GetAffiliateTotals(long affId, AffiliateReportCriteria criteria);

        public abstract AffiliateReportTotals GetAffiliateTotals(long affId, AffiliateReportCriteria criteria,
            TotalsReturnType returnType);

        internal void DestoryForStore(long storeId)
        {
            Delete(a => a.StoreId == storeId);
        }

        #endregion
    }
}