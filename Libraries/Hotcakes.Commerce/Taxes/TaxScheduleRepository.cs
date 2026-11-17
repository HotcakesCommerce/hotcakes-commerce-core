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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>Persists and queries store-scoped <see cref="TaxSchedule"/> records.</summary>
    /// <remarks>Methods that include a store identifier use that value for list queries; methods named for the current store use the repository's commerce context.</remarks>
    public class TaxScheduleRepository : HccSimpleRepoBase<hcc_TaxSchedules, TaxSchedule>
    {
        /// <summary>Initializes the repository with the supplied commerce request context.</summary>
        /// <param name="c">The request context used for repository data operations.</param>
        public TaxScheduleRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Maps a database schedule row to the commerce model.</summary>
        /// <param name="data">The database row.</param>
        /// <param name="model">The model receiving the row values.</param>
        protected override void CopyDataToModel(hcc_TaxSchedules data, TaxSchedule model)
        {
            model.Id = data.Id;
            model.Name = data.Name;
            model.StoreId = data.StoreId;
            model.DefaultRate = data.DefaultRate;
            model.DefaultShippingRate = data.DefaultShippingRate;
        }

        /// <summary>Maps a commerce schedule model to a database row.</summary>
        /// <param name="data">The database row receiving the model values.</param>
        /// <param name="model">The model to persist.</param>
        protected override void CopyModelToData(hcc_TaxSchedules data, TaxSchedule model)
        {
            data.Id = model.Id;
            data.Name = model.Name;
            data.StoreId = model.StoreId;
            data.DefaultRate = model.DefaultRate;
            data.DefaultShippingRate = model.DefaultShippingRate;
        }

        /// <summary>Creates a tax schedule in the current store.</summary>
        /// <param name="item">The schedule to create.</param>
        /// <returns><see langword="true"/> when the base repository creates the schedule; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The item's <see cref="TaxSchedule.StoreId"/> is overwritten with the current store identifier before persistence.</remarks>
        public override bool Create(TaxSchedule item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>Updates the schedule identified by <see cref="TaxSchedule.Id"/>.</summary>
        /// <param name="c">The schedule to update.</param>
        /// <returns>The result returned by the base repository update operation.</returns>
        /// <remarks>The existing implementation uses only the identifier predicate.</remarks>
        public bool Update(TaxSchedule c)
        {
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>Deletes a schedule by identifier.</summary>
        /// <param name="id">The schedule identifier.</param>
        /// <returns>The result returned by the repository delete operation.</returns>
        /// <remarks>This public overload does not add a store predicate.</remarks>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>Deletes a schedule by identifier using the current store context.</summary>
        /// <param name="id">The schedule identifier.</param>
        /// <param name="storeId">An accepted compatibility parameter; the implementation uses the current context store instead.</param>
        /// <returns>The result returned by the base repository delete operation.</returns>
        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>Finds a schedule by identifier within the current store.</summary>
        /// <param name="id">The schedule identifier.</param>
        /// <returns>The matching schedule, or <see langword="null"/> when it is not in the current store.</returns>
        public TaxSchedule FindForThisStore(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>Finds a schedule by identifier without a store filter.</summary>
        /// <param name="id">The schedule identifier.</param>
        /// <returns>The first matching schedule across stores, or <see langword="null"/> when none exists.</returns>
        public TaxSchedule FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Gets all schedules for a store.</summary>
        /// <param name="storeId">The store to query.</param>
        /// <returns>Matching schedules ordered by name.</returns>
        public List<TaxSchedule> FindAll(long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Name);
            });
        }

        /// <summary>Gets all schedules for a store through the <see cref="ITaxSchedule"/> abstraction.</summary>
        /// <param name="storeId">The store to query.</param>
        /// <returns>A new list containing the schedules returned by <see cref="FindAll"/> in the same order.</returns>
        public List<ITaxSchedule> FindAllAsInterface(long storeId)
        {
            var result = new List<ITaxSchedule>();
            foreach (ITaxSchedule ts in FindAll(storeId))
            {
                result.Add(ts);
            }
            return result;
        }

        /// <summary>Finds a schedule by exact name within the current store.</summary>
        /// <param name="name">The schedule name to compare.</param>
        /// <returns>The first matching schedule, or <see langword="null"/> when no exact match exists.</returns>
        public TaxSchedule FindByNameForThisStore(string name)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(y => y.StoreId == storeId && y.Name == name);
        }

        /// <summary>Deletes all schedules for a store.</summary>
        /// <param name="storeId">The store whose schedules are deleted.</param>
        /// <remarks>The existing misspelling <c>DestoryAllForStore</c> is retained for compatibility.</remarks>
        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
