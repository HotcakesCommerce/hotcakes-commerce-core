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

namespace Hotcakes.Commerce.Taxes
{
    public class TaxScheduleRepository : HccSimpleRepoBase<hcc_TaxSchedules, TaxSchedule>
    {
        public TaxScheduleRepository(HccRequestContext c)
            : base(c)
        {
        }


        protected override void CopyDataToModel(hcc_TaxSchedules data, TaxSchedule model)
        {
            model.Id = data.Id;
            model.Name = data.Name;
            model.StoreId = data.StoreId;
            model.DefaultRate = data.DefaultRate;
            model.DefaultShippingRate = data.DefaultShippingRate;
        }

        protected override void CopyModelToData(hcc_TaxSchedules data, TaxSchedule model)
        {
            data.Id = model.Id;
            data.Name = model.Name;
            data.StoreId = model.StoreId;
            data.DefaultRate = model.DefaultRate;
            data.DefaultShippingRate = model.DefaultShippingRate;
        }

        public override bool Create(TaxSchedule item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(TaxSchedule c)
        {
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        public TaxSchedule FindForThisStore(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        public TaxSchedule FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public List<TaxSchedule> FindAll(long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Name);
            });
        }

        public List<ITaxSchedule> FindAllAsInterface(long storeId)
        {
            var result = new List<ITaxSchedule>();
            foreach (ITaxSchedule ts in FindAll(storeId))
            {
                result.Add(ts);
            }
            return result;
        }

        public TaxSchedule FindByNameForThisStore(string name)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(y => y.StoreId == storeId && y.Name == name);
        }

        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static TaxScheduleRepository InstantiateForMemory(HccRequestContext c)
        {
            return new TaxScheduleRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static TaxScheduleRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new TaxScheduleRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public TaxScheduleRepository(HccRequestContext c, IRepositoryStrategy<hcc_TaxSchedules> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}