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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Provides store-scoped persistence and reporting operations for <see cref="Affiliate"/> records.</summary>
    /// <remarks>Most lookups use <see cref="HccRequestContext.CurrentStore"/>; <see cref="FindForAllStores(long)"/> is the explicit cross-store lookup.</remarks>
    public abstract class AffiliateRepository : HccSimpleRepoBase<hcc_Affiliate, Affiliate>
    {
        /// <summary>Specifies which related affiliate totals are included in a totals query.</summary>
        public enum TotalsReturnType
        {
            /// <summary>Returns all available totals.</summary>
            ReturnAll,
            /// <summary>Returns order-related totals.</summary>
            ReturnOrders,
            /// <summary>Returns payment-related totals.</summary>
            ReturnPayments,
            /// <summary>Returns referral-related totals.</summary>
            ReturnReferrals
        }

        /// <summary>Reports the outcome of an affiliate create or update operation.</summary>
        public enum UpdateStatus
        {
            /// <summary>The operation completed successfully.</summary>
            Success = 0,
            /// <summary>The affiliate identifier is already in use.</summary>
            DuplicateAffiliateID = 1,
            /// <summary>The associated CMS user could not be created.</summary>
            UserCreateFailed = 2,
            /// <summary>The operation failed for an unspecified reason.</summary>
            UnknownError = 99
        }

        /// <summary>Regular expression used to validate compatibility affiliate identifiers.</summary>
        /// <remarks>The <c>ID</c> spelling is part of the public compatibility surface.</remarks>
        public const string AffiliateIDValidationExpression = @"[-\w]*";

        /// <summary>Initializes the repository and its related country, payment, and referral repositories.</summary>
        /// <param name="context">The commerce request context that supplies store scope.</param>
        public AffiliateRepository(HccRequestContext context)
            : base(context)
        {
            _countryRepository = Factory.CreateRepo<CountryRepository>(context);
            _affiliatePaymentRepository = Factory.CreateRepo<AffiliatePaymentRepository>(context);
            _affiliateReferralRepository = Factory.CreateRepo<AffiliateReferralRepository>(context);
        }

        /// <summary>Copies persistence values into an affiliate model.</summary>
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

        /// <summary>Copies an affiliate model into its persistence representation.</summary>
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

        /// <summary>Repository used for country-related affiliate operations.</summary>
        protected CountryRepository _countryRepository;
        /// <summary>Repository used for affiliate payment operations.</summary>
        protected AffiliatePaymentRepository _affiliatePaymentRepository;
        /// <summary>Repository used for affiliate referral operations.</summary>
        protected AffiliateReferralRepository _affiliateReferralRepository;

        #endregion

        #region Public methods

        /// <summary>Finds an affiliate only when it belongs to the current store.</summary>
        /// <param name="id">The affiliate database identifier.</param>
        /// <returns>The matching current-store affiliate, or <see langword="null"/>.</returns>
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

        /// <summary>Finds an affiliate without applying current-store filtering.</summary>
        /// <param name="id">The affiliate database identifier.</param>
        public Affiliate FindForAllStores(long id)
        {
            return FindFirstPoco(a => a.Id == id);
        }

        /// <summary>Finds the first affiliate associated with the specified store identifier.</summary>
        /// <param name="StoreId">The store identifier.</param>
        /// <remarks>The parameter spelling is retained for compatibility.</remarks>
        public Affiliate FindForByStoreId(long StoreId)
        {
            return FindFirstPoco(a => a.StoreId == StoreId);
        }

        /// <summary>Not supported; callers must use the overload that returns <see cref="UpdateStatus"/>.</summary>
        public override bool Create(Affiliate item)
        {
            throw new NotImplementedException("Use Create(Affiliate item, ref CreateUserStatus userStatus) instead");
        }

        /// <summary>Creates an affiliate in the current store and initializes its UTC audit timestamps.</summary>
        /// <param name="aff">The affiliate to create.</param>
        /// <param name="userStatus">Receives the CMS user creation status.</param>
        /// <returns>The create outcome.</returns>
        public virtual UpdateStatus Create(Affiliate aff, ref CreateUserStatus userStatus)
        {
            aff.CreationDate = DateTime.UtcNow;
            aff.LastUpdatedUtc = DateTime.UtcNow;
            aff.StoreId = Context.CurrentStore.Id;

            return base.Create(aff) ? UpdateStatus.Success : UpdateStatus.UnknownError;
        }

        /// <summary>Updates an affiliate belonging to the current store.</summary>
        /// <param name="aff">The affiliate to update.</param>
        /// <returns>The update outcome.</returns>
        public virtual UpdateStatus Update(Affiliate aff)
        {
            if (aff.StoreId != Context.CurrentStore.Id)
            {
                throw new ArgumentException("Argument 'aff.StoreId' has invalid value");
            }

            aff.LastUpdatedUtc = DateTime.UtcNow;
            return Update(aff, a => a.Id == aff.Id) ? UpdateStatus.Success : UpdateStatus.UnknownError;
        }

        /// <summary>Deletes an affiliate when its record belongs to the current store.</summary>
        /// <param name="id">The affiliate database identifier.</param>
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

        /// <summary>Deletes the current-store affiliate associated with a CMS user identifier.</summary>
        /// <param name="UserID">The numeric user identifier represented as text.</param>
        /// <remarks>The <c>ID</c> spelling is retained for compatibility.</remarks>
        public virtual bool DeleteByUserID(string UserID)
        {
            var storeId = Context.CurrentStore.Id;
            var intUserID = Convert.ToInt32(UserID);
            return Delete(a => a.UserId == intUserID && a.StoreId == storeId);
        }

        /// <summary>Finds a current-store affiliate by its referral identifier.</summary>
        public Affiliate FindByAffiliateId(string affiliateId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(a => a.StoreId == storeId && a.AffiliateID == affiliateId);
        }

        /// <summary>Finds a current-store affiliate by its CMS user identifier.</summary>
        public Affiliate FindByUserId(int userId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(a => a.StoreId == storeId && a.UserId == userId);
        }

        /// <summary>Returns a page of affiliate report projections matching the supplied criteria.</summary>
        /// <param name="criteria">The date, search, approval, commission, and referral filters.</param>
        /// <param name="pageNumber">The page number used by the repository paging helper.</param>
        /// <param name="pageSize">The maximum number of rows in the page.</param>
        /// <param name="rowCount">Receives the total matching row count before paging.</param>
        public abstract List<AffiliateReportData> FindAllWithFilter(AffiliateReportCriteria criteria, int pageNumber,
            int pageSize, ref int rowCount);

        /// <summary>Gets aggregate affiliate totals for the supplied report criteria.</summary>
        public abstract AffiliateReportTotals GetTotalsByFilter(AffiliateReportCriteria criteria);

        /// <summary>Gets aggregate affiliate totals for the supplied criteria and related-data selection.</summary>
        public abstract AffiliateReportTotals GetTotalsByFilter(AffiliateReportCriteria criteria,
            TotalsReturnType returnType);

        /// <summary>Gets totals for one affiliate using the supplied report criteria.</summary>
        public abstract AffiliateReportTotals GetAffiliateTotals(long affId, AffiliateReportCriteria criteria);

        /// <summary>Gets one affiliate's totals with the requested related-data selection.</summary>
        public abstract AffiliateReportTotals GetAffiliateTotals(long affId, AffiliateReportCriteria criteria,
            TotalsReturnType returnType);

        /// <summary>Removes all affiliate records for a store.</summary>
        /// <param name="storeId">The store whose affiliate records are removed.</param>
        /// <remarks>The misspelled <c>DestoryForStore</c> name is retained for compatibility.</remarks>
        internal void DestoryForStore(long storeId)
        {
            Delete(a => a.StoreId == storeId);
        }

        #endregion
    }
}
