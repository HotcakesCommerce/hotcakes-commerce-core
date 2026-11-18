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
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Security
{
    /// <summary>
    /// Persists and retrieves <see cref="FraudRule"/> records for the current commerce context.
    /// </summary>
    /// <remarks>
    /// The repository maps to the <c>hcc_Fraud</c> data record. Create, update, find,
    /// and delete operations use the current store where their implementation requires
    /// it; <see cref="FindForAllStores"/> is the explicit exception for lookup by BVIN.
    /// </remarks>
    public class FraudRuleRepository : HccSimpleRepoBase<hcc_Fraud, FraudRule>
    {
        /// <summary>
        /// Initializes a repository using the supplied Hotcakes request context.
        /// </summary>
        /// <param name="c">The request context that supplies the current store and data access context.</param>
        public FraudRuleRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        /// Copies persisted fraud data into its domain model, including the stored
        /// numeric rule type and store identifier.
        /// </summary>
        /// <param name="data">The persistence record to read.</param>
        /// <param name="model">The domain model to populate.</param>
        protected override void CopyDataToModel(hcc_Fraud data, FraudRule model)
        {
            model.Bvin = data.bvin;
            model.LastUpdated = data.LastUpdated;
            model.RuleType = (FraudRuleType) data.RuleType;
            model.RuleValue = data.RuleValue;
            model.StoreId = data.StoreId;
        }

        /// <summary>
        /// Copies a fraud-rule model into its persistence record, storing the enum as
        /// its compatibility-sensitive integer value.
        /// </summary>
        /// <param name="data">The persistence record to populate.</param>
        /// <param name="model">The fraud-rule model to write.</param>
        protected override void CopyModelToData(hcc_Fraud data, FraudRule model)
        {
            data.bvin = model.Bvin;
            data.LastUpdated = model.LastUpdated;
            data.RuleType = (int) model.RuleType;
            data.RuleValue = model.RuleValue;
            data.StoreId = model.StoreId;
        }

        /// <summary>
        /// Finds a rule by BVIN within the current store.
        /// </summary>
        /// <param name="bvin">The rule BVIN.</param>
        /// <returns>The matching rule, or <see langword="null"/> when no current-store rule matches.</returns>
        public FraudRule Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>
        /// Finds a rule by BVIN without applying a store filter.
        /// </summary>
        /// <param name="bvin">The rule BVIN.</param>
        /// <returns>The first matching rule, or <see langword="null"/> when none matches.</returns>
        /// <remarks>
        /// This method intentionally differs from <see cref="Find(string)"/> and is the
        /// repository's all-store lookup.
        /// </remarks>
        public FraudRule FindForAllStores(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        /// <summary>
        /// Creates a rule for the current store.
        /// </summary>
        /// <param name="item">The rule to create.</param>
        /// <returns><see langword="true"/> when the base repository creates the record.</returns>
        /// <remarks>
        /// The current UTC timestamp is assigned, an empty BVIN is replaced with a new
        /// GUID string, and <see cref="FraudRule.StoreId"/> is set to the current store.
        /// </remarks>
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

        /// <summary>
        /// Updates a rule when its store matches the current store.
        /// </summary>
        /// <param name="c">The rule to update.</param>
        /// <returns>
        /// <see langword="false"/> when the rule belongs to another store; otherwise,
        /// the result returned by the base update operation.
        /// </returns>
        /// <remarks>The last-updated timestamp is replaced with the current UTC time.</remarks>
        public bool Update(FraudRule c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            return Update(c, y => y.bvin == c.Bvin);
        }

        /// <summary>
        /// Deletes the current-store rule identified by the supplied BVIN.
        /// </summary>
        /// <param name="bvin">The rule BVIN.</param>
        /// <returns><see langword="true"/> when the base repository deletes a matching record.</returns>
        public bool Delete(string bvin)
        {
            return Delete(y => y.bvin == bvin && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>
        /// Retrieves rules for a store, ordered by their rule values.
        /// </summary>
        /// <param name="storeId">The store identifier to filter by.</param>
        /// <returns>The matching rules ordered ascending by <see cref="FraudRule.RuleValue"/>.</returns>
        /// <remarks>
        /// Unlike <see cref="Find(string)"/>, this method accepts an explicit store ID
        /// rather than implicitly using the current store.
        /// </remarks>
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
