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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Geography;

namespace Hotcakes.Commerce.Taxes
{
    /// <summary>Persists and queries store-scoped <see cref="Tax"/> records.</summary>
    /// <remarks>Queries use the repository's current commerce context where the method does not explicitly receive a store identifier.</remarks>
    public class TaxRepository : HccSimpleRepoBase<hcc_Taxes, Tax>, ITaxRateRepository
    {
        /// <summary>Initializes the repository with the supplied commerce request context.</summary>
        /// <param name="c">The request context used for the repository data operations.</param>
        public TaxRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Gets all rates for a store and tax schedule.</summary>
        /// <param name="storeId">The store to query.</param>
        /// <param name="scheduleId">The tax schedule to query.</param>
        /// <returns>Matching rates ordered by country, region, and postal code.</returns>
        /// <remarks>This is the <see cref="ITaxRateRepository.GetRates"/> implementation and preserves both supplied filters.</remarks>
        public List<Tax> GetRates(long storeId, long scheduleId)
        {
            var rates = new List<Tax>();
            foreach (var r in FindByTaxSchedule(storeId, scheduleId))
            {
                rates.Add(r);
            }
            return rates;
        }

        /// <summary>Maps a database tax row to the commerce model.</summary>
        /// <param name="data">The database row.</param>
        /// <param name="model">The model receiving the row values.</param>
        protected override void CopyDataToModel(hcc_Taxes data, Tax model)
        {
            model.ApplyToShipping = data.ApplyToShipping;
            model.CountryIsoAlpha3 = data.CountryName;
            model.Id = data.Id;
            model.PostalCode = data.PostalCode;
            model.Rate = data.Rate;
            model.ShippingRate = data.ShippingRate;
            model.RegionAbbreviation = data.RegionName;
            model.StoreId = data.StoreId;
            model.TaxScheduleId = data.TaxScheduleId;
        }

        /// <summary>Maps a commerce tax model to a database row.</summary>
        /// <param name="data">The database row receiving the model values.</param>
        /// <param name="model">The model to persist.</param>
        protected override void CopyModelToData(hcc_Taxes data, Tax model)
        {
            data.ApplyToShipping = model.ApplyToShipping;
            data.CountryName = model.CountryIsoAlpha3;
            data.Id = model.Id;
            data.PostalCode = model.PostalCode;
            data.Rate = model.Rate;
            data.ShippingRate = model.ShippingRate;
            data.RegionName = model.RegionAbbreviation;
            data.StoreId = model.StoreId;
            data.TaxScheduleId = model.TaxScheduleId;
        }

        /// <summary>Determines whether an identical tax record already exists in the current store and schedule.</summary>
        /// <param name="item">The tax record whose persisted values are compared.</param>
        /// <returns><see langword="true"/> when every compared location, rate, shipping, store, and schedule value matches; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The lookup uses the current store for its initial schedule query and compares values exactly, including empty strings and decimal rates.</remarks>
        public bool ExactMatchExists(Tax item)
        {
            var result = false;

            var all = FindByTaxSchedule(Context.CurrentStore.Id, item.TaxScheduleId);
            if (all == null) return false;
            var t = all.Where(y => y.ApplyToShipping == item.ApplyToShipping)
                .Where(y => y.CountryIsoAlpha3 == item.CountryIsoAlpha3)
                .Where(y => y.PostalCode == item.PostalCode)
                .Where(y => y.Rate == item.Rate)
                .Where(y => y.ShippingRate == item.ShippingRate)
                .Where(y => y.RegionAbbreviation == item.RegionAbbreviation)
                .Where(y => y.StoreId == item.StoreId)
                .Where(y => y.TaxScheduleId == item.TaxScheduleId).FirstOrDefault();
            if (t != null) return true;

            return result;
        }

        /// <summary>Creates a tax record in the current store.</summary>
        /// <param name="item">The tax record to create.</param>
        /// <returns><see langword="true"/> when the base repository creates the record; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The item's <see cref="Tax.StoreId"/> is overwritten with the current store identifier before persistence.</remarks>
        public override bool Create(Tax item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>Updates the tax record identified by <see cref="Tax.Id"/>.</summary>
        /// <param name="c">The tax record to update.</param>
        /// <returns>The result returned by the base repository update operation.</returns>
        /// <remarks>The existing implementation uses the identifier predicate and does not add a separate store predicate here.</remarks>
        public bool Update(Tax c)
        {
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>Deletes a tax record by identifier within the current store.</summary>
        /// <param name="id">The tax record identifier.</param>
        /// <returns><see langword="true"/> when the store-scoped delete succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        /// <summary>Deletes a tax record when both its identifier and store identifier match.</summary>
        /// <param name="id">The tax record identifier.</param>
        /// <param name="storeId">The store identifier used by this internal helper.</param>
        /// <returns>The result returned by the base repository delete operation.</returns>
        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        /// <summary>Finds a tax record by identifier within the current store.</summary>
        /// <param name="id">The tax record identifier.</param>
        /// <returns>The matching tax record, or <see langword="null"/> when it is not in the current store.</returns>
        public Tax Find(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>Finds a tax record by identifier without a store filter.</summary>
        /// <param name="id">The tax record identifier.</param>
        /// <returns>The first matching tax record across stores, or <see langword="null"/> when none exists.</returns>
        public Tax FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>Gets every tax record for a store.</summary>
        /// <param name="storeId">The store to query.</param>
        /// <returns>Matching records ordered by country, region, and postal code.</returns>
        public List<Tax> FindByStoreId(long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.CountryName)
                    .ThenBy(y => y.RegionName)
                    .ThenBy(y => y.PostalCode);
            });
        }

        /// <summary>Gets every tax record for a store and tax schedule.</summary>
        /// <param name="storeId">The store to query.</param>
        /// <param name="scheduleId">The tax schedule to query.</param>
        /// <returns>Matching records ordered by country, region, and postal code.</returns>
        public List<Tax> FindByTaxSchedule(long storeId, long scheduleId)
        {
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.TaxScheduleId == scheduleId)
                    .OrderBy(y => y.CountryName)
                    .ThenBy(y => y.RegionName)
                    .ThenBy(y => y.PostalCode);
            });
        }

        /// <summary>Finds the first tax record matching an address for a store and schedule.</summary>
        /// <param name="storeId">The store to query.</param>
        /// <param name="scheduleId">The tax schedule to query.</param>
        /// <param name="address">The address to match.</param>
        /// <returns>The first matching tax record, or <see langword="null"/> when no record matches.</returns>
        /// <remarks>The misspelling in <c>FindByAdress</c> is compatibility-sensitive. Country must match case-insensitively; an empty region or postal code in a stored record is a wildcard. Candidates are evaluated in country, region, and postal-code order.</remarks>
        public Tax FindByAdress(long storeId, long scheduleId, IAddress address)
        {
            using (var s = CreateReadStrategy())
            {
                var taxes = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .Where(y => y.TaxScheduleId == scheduleId)
                    .OrderBy(y => y.CountryName)
                    .ThenBy(y => y.RegionName)
                    .ThenBy(y => y.PostalCode)
                    .ToList();

                foreach (var hccTax in taxes)
                {
                    if (address.CountryData != null)
                    {
                        if (string.Compare(address.CountryData.IsoAlpha3, hccTax.CountryName, true) == 0)
                        {
                            if (string.IsNullOrEmpty(hccTax.RegionName) ||
                                string.Compare(address.RegionBvin, hccTax.RegionName, true) == 0)
                            {
                                if (string.IsNullOrEmpty(hccTax.PostalCode) ||
                                    string.Compare(address.PostalCode, hccTax.PostalCode, true) == 0)
                                {
                                    var tax = new Tax();
                                    CopyDataToModel(hccTax, tax);
                                    return tax;
                                }
                            }
                        }
                    }
                }
            }

            return null;
        }

        /// <summary>Deletes all tax records for a store.</summary>
        /// <param name="storeId">The store whose tax records are deleted.</param>
        /// <remarks>The existing misspelling <c>DestoryAllForStore</c> is retained for compatibility.</remarks>
        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
