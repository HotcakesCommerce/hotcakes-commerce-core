#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
    [Obsolete("Obsolete in 1.8.0. Affiliate contacts are not used")]
    public class AffiliateContactRepository : HccSimpleRepoBase<hcc_UserXContact, AffiliateContact>
    {
        public AffiliateContactRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override Func<hcc_UserXContact, bool> MatchItems(AffiliateContact item)
        {
            return ac => ac.ContactId == item.AffiliateId;
        }

        protected override Func<hcc_UserXContact, bool> NotMatchItems(List<AffiliateContact> items)
        {
            var itemIds = items.Select(i => i.AffiliateId).ToList();
            return ac => !itemIds.Contains(ac.ContactId);
        }

        protected override void CopyDataToModel(hcc_UserXContact data, AffiliateContact model)
        {
            model.Id = data.Id;
            model.AffiliateId = data.ContactId;
            model.StoreId = data.StoreId;
            model.UserId = data.UserId;
        }

        protected override void CopyModelToData(hcc_UserXContact data, AffiliateContact model)
        {
            data.Id = model.Id;
            data.ContactId = model.AffiliateId;
            data.StoreId = model.StoreId;
            data.UserId = model.UserId;
        }

        public AffiliateContact Find(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        public AffiliateContact FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public override bool Create(AffiliateContact item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(AffiliateContact c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<AffiliateContact> FindForAffiliate(long affiliateId)
        {
            var affId = affiliateId.ToString();
            return FindListPoco(q => { return q.Where(y => y.ContactId == affId); });
        }

        public List<AffiliateContact> FindForCustomerId(string customerId)
        {
            return FindListPoco(q => { return q.Where(y => y.UserId == customerId); });
        }

        public void DeleteForAffiliate(long affiliateId)
        {
            var existing = FindForAffiliate(affiliateId);
            foreach (var sub in existing)
            {
                Delete(sub.Id);
            }
        }

        public bool DeleteForCustomerId(string customerId)
        {
            var existing = FindForCustomerId(customerId);
            foreach (var sub in existing)
            {
                Delete(sub.Id);
            }
            return true;
        }

        public void MergeList(long affiliateId, List<AffiliateContact> subitems)
        {
            // Set Base Key Field
            var affiliateString = affiliateId.ToString();
            foreach (var item in subitems)
            {
                item.AffiliateId = affiliateString;
            }

            MergeList(subitems, ac => ac.ContactId == affiliateString);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static AffiliateContactRepository InstantiateForMemory(HccRequestContext c)
        {
            return new AffiliateContactRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static AffiliateContactRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new AffiliateContactRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public AffiliateContactRepository(HccRequestContext c, IRepositoryStrategy<hcc_UserXContact> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}