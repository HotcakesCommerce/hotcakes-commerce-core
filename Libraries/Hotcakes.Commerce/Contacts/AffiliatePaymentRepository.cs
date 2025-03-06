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
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Contacts
{
    public class AffiliatePaymentRepository : HccSimpleRepoBase<hcc_AffiliatePayments, AffiliatePayment>
    {
        public AffiliatePaymentRepository(HccRequestContext c)
            : base(c)
        {
        }

        #region Public methods

        public AffiliatePayment Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public List<AffiliatePayment> FindForAffiliate(long id)
        {
            return FindListPoco(q => { return q.Where(y => y.AffiliateId == id); });
        }

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

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public bool DeleteAllForAffiliate(long affiliateId)
        {
            return Delete(y => y.AffiliateId == affiliateId);
        }

        public bool Update(AffiliatePayment payment)
        {
            return base.Update(payment, y => y.Id == payment.Id);
        }

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

        protected override void CopyModelToData(hcc_AffiliatePayments data, AffiliatePayment model)
        {
            data.Id = model.Id;
            data.AffiliateId = model.AffiliateId;
            data.PaymentAmount = model.PaymentAmount;
            data.PaymentDate = model.PaymentDateUtc;
            data.Notes = model.Notes;
            data.FileName = model.FileName;
        }

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