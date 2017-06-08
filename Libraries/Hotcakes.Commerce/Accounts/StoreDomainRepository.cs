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

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>
    ///     This class used to perform different database operation against hcc_StoreDomains table.
    /// </summary>
    [Serializable]
    public class StoreDomainRepository : HccSimpleRepoBase<hcc_StoreDomains, StoreDomain>
    {
        public StoreDomainRepository(HccRequestContext c)
            : base(c)
        {
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public StoreDomainRepository(HccRequestContext c, IRepositoryStrategy<hcc_StoreDomains> r)
            : this(c)
        {
        }

        #endregion

        /// <summary>
        ///     Copy data from database table instance to model instance.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        protected override void CopyModelToData(hcc_StoreDomains data, StoreDomain model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.DomainName = model.DomainName.Trim().ToLowerInvariant();
        }

        /// <summary>
        ///     Copy
        /// </summary>
        /// <param name="data"></param>
        /// <param name="model"></param>
        protected override void CopyDataToModel(hcc_StoreDomains data, StoreDomain model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.DomainName = data.DomainName;
        }

        public StoreDomain Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public StoreDomain FindForAnyStoreByDomain(string domain)
        {
            var domainMatch = domain.Trim().ToLowerInvariant();
            return FindFirstPoco(y => y.DomainName == domainMatch);
        }

        public override bool Create(StoreDomain item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(StoreDomain item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<StoreDomain> FindForStore(long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.DomainName);
            });
        }

        public List<StoreDomain> FindForCurrentStore()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId); });
        }
    }
}