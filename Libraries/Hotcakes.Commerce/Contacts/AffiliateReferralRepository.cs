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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Contacts
{
    public class AffiliateReferralRepository : HccSimpleRepoBase<hcc_AffiliateReferral, AffiliateReferral>
    {
        public AffiliateReferralRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override void CopyDataToModel(hcc_AffiliateReferral data, AffiliateReferral model)
        {
            model.AffiliateId = data.AffiliateId;
            model.Id = data.Id;
            model.TimeOfReferralUtc = data.TimeOfReferralUtc;
            model.ReferrerUrl = data.referrerurl;
            model.StoreId = data.StoreId;
        }

        protected override void CopyModelToData(hcc_AffiliateReferral data, AffiliateReferral model)
        {
            data.AffiliateId = model.AffiliateId;
            data.Id = model.Id;
            data.TimeOfReferralUtc = model.TimeOfReferralUtc;
            data.referrerurl = model.ReferrerUrl;
            data.StoreId = model.StoreId;
        }

        public AffiliateReferral Find(long id)
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

        public AffiliateReferral FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public override bool Create(AffiliateReferral item)
        {
            return Create2(item, false);
        }

        public bool Create2(AffiliateReferral item, bool timeStampIsSet)
        {
            if (!timeStampIsSet)
            {
                item.TimeOfReferralUtc = DateTime.UtcNow;
            }
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(AffiliateReferral c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        public List<AffiliateReferral> FindByCriteria(AffiliateReferralSearchCriteria criteria, int pageNumber,
            int pageSize, ref int rowCount)
        {
            using (var strategy = CreateStrategy())
            {
                var query = strategy.GetQuery().Where(y => y.StoreId == Context.CurrentStore.Id);
                if (criteria.AffiliateId > 0)
                {
                    query = query.Where(y => y.AffiliateId == criteria.AffiliateId);
                }
                if (criteria.StartDateUtc.HasValue && criteria.EndDateUtc.HasValue)
                {
                    query =
                        query.Where(
                            y =>
                                y.TimeOfReferralUtc >= criteria.StartDateUtc.Value &&
                                y.TimeOfReferralUtc <= criteria.EndDateUtc.Value);
                }
                query = query.OrderBy(y => y.Id);
                rowCount = query.Count();
                var items = GetPagedItems(query, pageNumber, pageSize);
                return ListPoco(items);
            }
        }

        public bool DeleteAllForAffiliate(long affiliateId)
        {
            return Delete(y => y.AffiliateId == affiliateId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static AffiliateReferralRepository InstantiateForMemory(HccRequestContext c)
        {
            return new AffiliateReferralRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static AffiliateReferralRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new AffiliateReferralRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public AffiliateReferralRepository(HccRequestContext c, IRepositoryStrategy<hcc_AffiliateReferral> r,
            ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}