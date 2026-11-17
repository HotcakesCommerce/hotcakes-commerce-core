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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Provides store-scoped persistence for <see cref="PriceGroup"/> records.</summary>
    public class PriceGroupRepository : HccSimpleRepoBase<hcc_PriceGroup, PriceGroup>
    {
        /// <summary>Initializes the repository with the request context used to resolve the current store.</summary>
        /// <param name="c">The Hotcakes request context.</param>
        public PriceGroupRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Copies a price-group domain model into its database row.</summary>
        /// <param name="data">The destination database row.</param>
        /// <param name="model">The source price group.</param>
        protected override void CopyModelToData(hcc_PriceGroup data, PriceGroup model)
        {
            data.AdjustmentAmount = model.AdjustmentAmount;
            data.bvin = model.Bvin;
            data.LastUpdated = model.LastUpdated;
            data.Name = model.Name;
            data.PricingType = (int) model.PricingType;
            data.StoreId = model.StoreId;
        }

        /// <summary>Copies a database price-group row into the domain model.</summary>
        /// <param name="data">The database row.</param>
        /// <param name="model">The destination price group.</param>
        /// <remarks>The persisted integer pricing mode is converted to <see cref="PricingTypes"/>.</remarks>
        protected override void CopyDataToModel(hcc_PriceGroup data, PriceGroup model)
        {
            model.AdjustmentAmount = data.AdjustmentAmount;
            model.Bvin = data.bvin;
            model.LastUpdated = data.LastUpdated;
            model.Name = data.Name;
            model.PricingType = (PricingTypes) data.PricingType;
            model.StoreId = data.StoreId;
        }


        /// <summary>Finds a price group by its identifier.</summary>
        /// <param name="bvin">The price-group identifier.</param>
        /// <returns>The matching price group, or <see langword="null"/> when no record matches.</returns>
        public PriceGroup Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        /// <summary>Creates a price group for the current store.</summary>
        /// <param name="item">The price group to persist.</param>
        /// <returns><see langword="true"/> when the base repository creates the record.</returns>
        /// <remarks>An empty identifier is generated, store ownership is assigned, and the UTC update timestamp is refreshed.</remarks>
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

        /// <summary>Updates a price group only when it belongs to the current store.</summary>
        /// <param name="item">The price group to update.</param>
        /// <returns><see langword="false"/> for a different store; otherwise the repository update result.</returns>
        public bool Update(PriceGroup item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            item.LastUpdated = DateTime.UtcNow;
            return Update(item, y => y.bvin == item.Bvin);
        }

        /// <summary>Deletes a price group by its identifier.</summary>
        /// <param name="bvin">The price-group identifier.</param>
        /// <returns>The repository delete result.</returns>
        public bool Delete(string bvin)
        {
            return Delete(y => y.bvin == bvin);
        }

        /// <summary>Returns all price groups for the current store, ordered by name.</summary>
        /// <returns>The current store's price groups.</returns>
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
