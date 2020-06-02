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
    public class PriceGroupRepository : HccSimpleRepoBase<hcc_PriceGroup, PriceGroup>
    {
        public PriceGroupRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyModelToData(hcc_PriceGroup data, PriceGroup model)
        {
            data.AdjustmentAmount = model.AdjustmentAmount;
            data.bvin = model.Bvin;
            data.LastUpdated = model.LastUpdated;
            data.Name = model.Name;
            data.PricingType = (int) model.PricingType;
            data.StoreId = model.StoreId;
        }

        protected override void CopyDataToModel(hcc_PriceGroup data, PriceGroup model)
        {
            model.AdjustmentAmount = data.AdjustmentAmount;
            model.Bvin = data.bvin;
            model.LastUpdated = data.LastUpdated;
            model.Name = data.Name;
            model.PricingType = (PricingTypes) data.PricingType;
            model.StoreId = data.StoreId;
        }


        public PriceGroup Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public override bool Create(PriceGroup item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;
            return base.Create(item);
        }

        public bool Update(PriceGroup item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            item.LastUpdated = DateTime.UtcNow;
            return Update(item, y => y.bvin == item.Bvin);
        }

        public bool Delete(string bvin)
        {
            return Delete(y => y.bvin == bvin);
        }

        public List<PriceGroup> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderBy(y => y.Name); });
        }


        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}