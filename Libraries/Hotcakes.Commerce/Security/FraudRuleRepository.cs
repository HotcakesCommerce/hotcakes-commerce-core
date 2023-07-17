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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Security
{
    public class FraudRuleRepository : HccSimpleRepoBase<hcc_Fraud, FraudRule>
    {
        public FraudRuleRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_Fraud data, FraudRule model)
        {
            model.Bvin = data.bvin;
            model.LastUpdated = data.LastUpdated;
            model.RuleType = (FraudRuleType) data.RuleType;
            model.RuleValue = data.RuleValue;
            model.StoreId = data.StoreId;
        }

        protected override void CopyModelToData(hcc_Fraud data, FraudRule model)
        {
            data.bvin = model.Bvin;
            data.LastUpdated = model.LastUpdated;
            data.RuleType = (int) model.RuleType;
            data.RuleValue = model.RuleValue;
            data.StoreId = model.StoreId;
        }

        public FraudRule Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin && y.StoreId == Context.CurrentStore.Id);
        }

        public FraudRule FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public override bool Create(FraudRule item)
        {
            item.LastUpdated = DateTime.UtcNow;
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;

            return base.Create(item);
        }

        public bool Update(FraudRule c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        public bool Delete(string bvin)
        {
            return Delete(y => y.bvin == bvin && y.StoreId == Context.CurrentStore.Id);
        }

        public List<FraudRule> FindForStore(long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.RuleValue);
            });
        }
    }
}