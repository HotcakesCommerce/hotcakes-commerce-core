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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Geography;

namespace Hotcakes.Commerce.Taxes
{
    public class TaxRepository : HccSimpleRepoBase<hcc_Taxes, Tax>, ITaxRateRepository
    {
        public TaxRepository(HccRequestContext c)
            : base(c)
        {
        }

        public List<Tax> GetRates(long storeId, long scheduleId)
        {
            var rates = new List<Tax>();
            foreach (var r in FindByTaxSchedule(storeId, scheduleId))
            {
                rates.Add(r);
            }
            return rates;
        }

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

        public override bool Create(Tax item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(Tax c)
        {
            return Update(c, y => y.Id == c.Id);
        }

        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            return Delete(y => y.Id == id && y.StoreId == storeId);
        }

        public Tax Find(long id)
        {
            return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
        }

        public Tax FindForAllStores(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

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

        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}