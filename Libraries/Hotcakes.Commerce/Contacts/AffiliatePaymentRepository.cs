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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Provides persistence, paging, and reporting operations for <see cref="AffiliatePayment"/> records.</summary>
    /// <remarks>Payment queries use the repository's underlying payment data; unlike referral searches, these methods do not add a current-store predicate.</remarks>
    public class AffiliatePaymentRepository : HccSimpleRepoBase<hcc_AffiliatePayments, AffiliatePayment>
    {
        /// <summary>Initializes the payment repository for the supplied commerce context.</summary>
        public AffiliatePaymentRepository(HccRequestContext c)
            : base(c)
        {
        }

        #region Public methods

        /// <summary>Finds a payment by its identifier.</summary>
        public AffiliatePayment Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Returns all payments associated with an affiliate.</summary>
        public List<AffiliatePayment> FindForAffiliate(long id)
        {
            return FindListPoco(q => { return q.Where(y => y.AffiliateId == id); });
        }

        /// <summary>Returns a page of an affiliate's payments within inclusive UTC date bounds.</summary>
        /// <param name="affiliateId">The numeric affiliate identifier.</param>
        /// <param name="startUtc">The inclusive UTC start date.</param>
        /// <param name="endUtc">The inclusive UTC end date.</param>
        /// <param name="pageNumber">The page number used by the repository paging helper.</param>
        /// <param name="pageSize">The maximum number of rows in the page.</param>
        /// <param name="totalRowCount">Receives the total matching row count before paging.</param>
        /// <returns>Payments ordered by ascending payment date.</returns>
        public List<AffiliatePayment> FindAllPaged(long affiliateId, DateTime startUtc, DateTime endUtc, int pageNumber,
            int pageSize, out int totalRowCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy
                    .GetQuery(i => i.AffiliateId == affiliateId && i.PaymentDate >= startUtc && i.PaymentDate <= endUtc)
                    .AsNoTracking()
                    .OrderBy(i => i.PaymentDate);

                totalRowCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        /// <summary>Returns a projected, paged payment report filtered by search text and inclusive UTC dates.</summary>
        /// <param name="criteria">The report search and date criteria.</param>
        /// <param name="pageNumber">The page number used by the repository paging helper.</param>
        /// <param name="pageSize">The maximum number of rows in the page.</param>
        /// <param name="totalRowCount">Receives the total matching row count before paging.</param>
        /// <returns>Payment projections ordered by ascending payment date.</returns>
        /// <remarks>Affiliate searches use the affiliate referral identifier; invalid numeric payment searches match no rows.</remarks>
        public List<AffiliatePaymentReportData> FindAllWithFilter(AffiliatePaymentReportCriteria criteria,
            int pageNumber, int pageSize, out int totalRowCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery().AsNoTracking();

                //IQueryable<hcc_AffiliatePayments> items = repository.Find();
                if (!string.IsNullOrEmpty(criteria.SearchText))
                {
                    switch (criteria.SearchBy)
                    {
                        case AffiliatePaymentReportCriteria.SearchType.AffiliateId:
                            query = query.Where(i => i.hcc_Affiliate.AffiliateID.Contains(criteria.SearchText));
                            break;
                        case AffiliatePaymentReportCriteria.SearchType.PaymentId:
                            long paymentId;
                            if (!long.TryParse(criteria.SearchText, out paymentId))
                                paymentId = -1;
                            query = query.Where(i => i.Id == paymentId);
                            break;
                    }
                }

                query = query.Where(i => i.PaymentDate >= criteria.StartDateUtc && i.PaymentDate <= criteria.EndDateUtc)
                    .OrderBy(i => i.PaymentDate);

                totalRowCount = query.Count();

                return GetPagedItems(query, pageNumber, pageSize).Select(i => new AffiliatePaymentReportData
                {
                    Id = i.Id,
                    AffiliateId = i.hcc_Affiliate.AffiliateID,
                    Notes = i.Notes,
                    PaymentAmount = i.PaymentAmount,
                    PaymentDateUtc = i.PaymentDate,
                    FileName = i.FileName
                })
                    .ToList();
            }
        }

        /// <summary>Deletes a payment by its identifier.</summary>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>Deletes all payments associated with an affiliate.</summary>
        public bool DeleteAllForAffiliate(long affiliateId)
        {
            return Delete(y => y.AffiliateId == affiliateId);
        }

        /// <summary>Updates a payment by its identifier.</summary>
        public bool Update(AffiliatePayment payment)
        {
            return base.Update(payment, y => y.Id == payment.Id);
        }

        /// <summary>Associates every supplied payment with an affiliate and merges the list.</summary>
        /// <param name="affiliateId">The numeric affiliate identifier assigned to each item.</param>
        /// <param name="subitems">The payment items to merge.</param>
        public void MergeList(long affiliateId, List<AffiliatePayment> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.AffiliateId = affiliateId;
            }

            MergeList(subitems, ap => ap.AffiliateId == affiliateId);
        }

        #endregion

        #region Implementation

        /// <summary>Copies a payment model into its persistence representation.</summary>
        protected override void CopyModelToData(hcc_AffiliatePayments data, AffiliatePayment model)
        {
            data.Id = model.Id;
            data.AffiliateId = model.AffiliateId;
            data.PaymentAmount = model.PaymentAmount;
            data.PaymentDate = model.PaymentDateUtc;
            data.Notes = model.Notes;
            data.FileName = model.FileName;
        }

        /// <summary>Copies persistence values into a payment model.</summary>
        protected override void CopyDataToModel(hcc_AffiliatePayments data, AffiliatePayment model)
        {
            model.Id = data.Id;
            model.AffiliateId = data.AffiliateId;
            model.PaymentAmount = data.PaymentAmount;
            model.PaymentDateUtc = data.PaymentDate;
            model.Notes = data.Notes;
            model.FileName = data.FileName;
        }

        #endregion
    }
}
