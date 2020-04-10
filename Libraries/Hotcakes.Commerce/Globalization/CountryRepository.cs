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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Globalization
{
    public class CountryRepository : HccLocalizationRepoBase<hcc_Country, hcc_CountryTranslation, Country, Guid>
    {
        public RegionRepository regionRepository;

        public CountryRepository(HccRequestContext context)
            : base(context)
        {
            regionRepository = new RegionRepository(context);
        }

        protected override Expression<Func<hcc_Country, Guid>> ItemKeyExp
        {
            get { return c => c.CountryId; }
        }

        protected override Expression<Func<hcc_CountryTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return ct => ct.CountryId; }
        }

        protected override void CopyItemToModel(hcc_Country data, Country model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.CountryId);
            model.CultureCode = data.CultureCode;
            model.SystemName = data.SystemName;
            model.IsoCode = data.IsoCode;
            model.IsoAlpha3 = data.IsoAlpha3;
            model.IsoNumeric = data.IsoNumeric;
            model.PostalCodeValidationRegex = data.PostalCodeValidationRegex;
        }

        protected override void CopyTransToModel(hcc_CountryTranslation data, Country model)
        {
            model.DisplayName = data.DisplayName;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_Country, hcc_CountryTranslation> data, Country model)
        {
            data.Item.CountryId = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.CultureCode = model.CultureCode;
            data.Item.SystemName = model.SystemName;
            data.Item.IsoCode = model.IsoCode;
            data.Item.IsoAlpha3 = model.IsoAlpha3;
            data.Item.IsoNumeric = model.IsoNumeric;
            data.Item.PostalCodeValidationRegex = model.PostalCodeValidationRegex;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_Country, hcc_CountryTranslation> data, Country model)
        {
            data.ItemTranslation.CountryId = DataTypeHelper.BvinToGuid(model.Bvin);

            data.ItemTranslation.DisplayName = model.DisplayName;
        }

        protected override void GetSubItems(List<Country> models)
        {
            var countryIds = models.Select(s => DataTypeHelper.BvinToGuid(s.Bvin)).ToList();
            var allRegions = regionRepository.FindAll(countryIds);

            foreach (var model in models)
            {
                var countryGuid = DataTypeHelper.BvinToGuid(model.Bvin);
                var region = allRegions.Where(r => r.CountryId == countryGuid).OrderBy(r => r.DisplayName).ToList();
                model.Regions = region;
            }
        }

        public List<Country> FindAll()
        {
            var result = CacheManager.GetCountries(Context.MainContentCulture);
            if (result == null)
            {
                result = FindListPoco(q => q);

                result = result.OrderBy(c => c.DisplayName).ToList();

                CacheManager.AddCountries(Context.MainContentCulture, result);
            }
            return result;
        }

        public Country Find(string bvin)
        {
            return FindAll().FirstOrDefault(c => c.Bvin == bvin);
        }

        public Country FindBySystemName(string systemName)
        {
            return FindAll().FirstOrDefault(c => c.SystemName == systemName);
        }

        public Country FindByDisplayName(string displayName)
        {
            return FindAll().FirstOrDefault(c => c.DisplayName == displayName);
        }


        public Country FindByISOCode(string isoCode)
        {
            return FindAll().FirstOrDefault(c => c.IsoCode == isoCode
                            || c.IsoAlpha3 == isoCode
                            || c.IsoNumeric == isoCode);
        }

        public List<Country> FindAllExceptList(List<string> disabledIso3Codes)
        {
            return FindAll()
                .Where(c => !disabledIso3Codes.Contains(c.IsoAlpha3))
                .OrderBy(c => c.DisplayName)
                .ToList();
        }

        public List<Country> FindAllInList(List<string> matchingIso3Codes)
        {
            return FindAll()
                .Where(c => matchingIso3Codes.Contains(c.IsoAlpha3))
                .ToList();
        }

        public List<Country> FindActiveCountries()
        {
            return FindAllExceptList(Context.CurrentStore.Settings.DisabledCountryIso3Codes);
        }

        public List<Country> FindAllForCurrency()
        {
            return FindAll()
                .Where(c => !string.IsNullOrEmpty(c.CultureCode))
                .ToList();
        }

        public override bool Create(Country country)
        {
            if (string.IsNullOrEmpty(country.Bvin))
            {
                country.Bvin = Guid.NewGuid().ToString();
            }
            var result = base.Create(country);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        public bool Update(Country country)
        {
            var countryGuid = DataTypeHelper.BvinToGuid(country.Bvin);
            var result = Update(country, c => c.CountryId == countryGuid);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        public bool Delete(string countryId)
        {
            var countryGuid = DataTypeHelper.BvinToGuid(countryId);
            var result = Delete(c => c.CountryId == countryGuid);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }
    }
}