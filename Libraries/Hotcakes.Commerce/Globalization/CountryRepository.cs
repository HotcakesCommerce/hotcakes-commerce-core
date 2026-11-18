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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Globalization
{
    /// <summary>
    ///     Provides store-scoped persistence and localized lookup operations for <see cref="Country"/> records.
    /// </summary>
    /// <remarks>
    ///     The base localization repository selects the translation for the request context's main content culture and
    ///     applies its established fallback behavior. Country-list caching is keyed by that culture. Country mutations
    ///     invalidate the corresponding country cache; loading countries also loads their regions in one bulk operation.
    /// </remarks>
    public class CountryRepository : HccLocalizationRepoBase<hcc_Country, hcc_CountryTranslation, Country, Guid>
    {
        /// <summary>
        ///     Gets the repository used to load regions for countries returned by this repository.
        /// </summary>
        /// <remarks>The field is public for compatibility and is initialized with the same request context as this repository.</remarks>
        public readonly RegionRepository regionRepository;

        /// <summary>
        ///     Initializes a country repository for the supplied commerce request context.
        /// </summary>
        /// <param name="context">The request context that supplies store scope and the main content culture.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
        public CountryRepository(HccRequestContext context)
            : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
            regionRepository = new RegionRepository(context);
        }

        /// <summary>Gets the expression that maps a country entity to its persisted identifier.</summary>
        protected override Expression<Func<hcc_Country, Guid>> ItemKeyExp => c => c.CountryId;

        /// <summary>Gets the expression that maps a country translation to its country identifier.</summary>
        protected override Expression<Func<hcc_CountryTranslation, Guid>> ItemTranslationKeyExp => ct => ct.CountryId;

        /// <summary>Copies non-localized country fields from the data entity to the model.</summary>
        /// <param name="data">The country data entity.</param>
        /// <param name="model">The country model to populate.</param>
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

        /// <summary>Copies the selected localized country display name to the model.</summary>
        /// <param name="data">The country translation selected by the base localization query.</param>
        /// <param name="model">The country model to populate.</param>
        protected override void CopyTransToModel(hcc_CountryTranslation data, Country model)
        {
            model.DisplayName = data.DisplayName;
        }

        /// <summary>Copies non-localized country fields from the model to the joined persistence item.</summary>
        /// <param name="data">The joined country and translation entities to update.</param>
        /// <param name="model">The country model to persist.</param>
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

        /// <summary>Copies the model's localized display name to the joined translation entity.</summary>
        /// <param name="data">The joined country and translation entities to update.</param>
        /// <param name="model">The country model to persist.</param>
        protected override void CopyModelToTrans(JoinedItem<hcc_Country, hcc_CountryTranslation> data, Country model)
        {
            data.ItemTranslation.CountryId = DataTypeHelper.BvinToGuid(model.Bvin);
            data.ItemTranslation.DisplayName = model.DisplayName;
        }

        /// <summary>
        ///     Loads regions for all supplied countries and assigns each list ordered by localized display name.
        /// </summary>
        /// <param name="models">The country models whose region collections should be populated.</param>
        /// <remarks>
        ///     Regions are queried in bulk through <see cref="RegionRepository.FindAll(List{Guid})"/> rather than once per
        ///     country. A null or empty model list is ignored. The bulk region query itself does not impose an order; this
        ///     method orders each country's assigned collection by <see cref="Region.DisplayName"/>.
        /// </remarks>
        protected override void GetSubItems(List<Country> models)
        {
            if (models == null || models.Count == 0) return;

            var countryIds = models.Select(s => DataTypeHelper.BvinToGuid(s.Bvin)).ToList();
            var allRegions = regionRepository.FindAll(countryIds);

            var regionsByCountry = allRegions.ToLookup(r => r.CountryId);

            foreach (var model in models)
            {
                var countryGuid = DataTypeHelper.BvinToGuid(model.Bvin);
                var regionList = regionsByCountry[countryGuid]
                    .OrderBy(r => r.DisplayName)
                    .ToList();
                model.Regions = regionList;
            }
        }

        /// <summary>
        ///     Finds all countries for the request context's main content culture.
        /// </summary>
        /// <returns>An alphabetically ordered list of countries, including their loaded region collections.</returns>
        /// <remarks>
        ///     Results are read from or written to the culture-specific country cache. Localization selection and fallback
        ///     are performed by the inherited repository query.
        /// </remarks>
        public List<Country> FindAll()
        {
            var result = CacheManager.GetCountries(Context.MainContentCulture);
            if (result == null)
            {
                result = FindListPoco(q => q)
                    .OrderBy(c => c.DisplayName)
                    .ToList();

                CacheManager.AddCountries(Context.MainContentCulture, result);
            }
            return result;
        }

        /// <summary>Finds a country by its bvin.</summary>
        /// <param name="bvin">The country identifier in bvin format.</param>
        /// <returns>The matching country, or <see langword="null"/> when it is not found.</returns>
        public Country Find(string bvin)
        {
            return FindAll().FirstOrDefault(c => c.Bvin == bvin);
        }

        /// <summary>Finds a country by its stable system name.</summary>
        /// <param name="systemName">The non-localized system name to compare.</param>
        /// <returns>The matching country, or <see langword="null"/> when it is not found.</returns>
        public Country FindBySystemName(string systemName)
        {
            return FindAll().FirstOrDefault(c => c.SystemName == systemName);
        }

        /// <summary>Finds a country by the display name selected for the current content culture.</summary>
        /// <param name="displayName">The localized display name to compare.</param>
        /// <returns>The matching country, or <see langword="null"/> when it is not found.</returns>
        public Country FindByDisplayName(string displayName)
        {
            return FindAll().FirstOrDefault(c => c.DisplayName == displayName);
        }

        /// <summary>Finds a country by its two-letter, three-letter, or numeric ISO code.</summary>
        /// <param name="isoCode">A value compared with <see cref="Country.IsoCode"/>, <see cref="Country.IsoAlpha3"/>, and <see cref="Country.IsoNumeric"/>.</param>
        /// <returns>The first matching country, or <see langword="null"/> when no code matches.</returns>
        public Country FindByISOCode(string isoCode)
        {
            return FindAll().FirstOrDefault(c => c.IsoCode == isoCode
                            || c.IsoAlpha3 == isoCode
                            || c.IsoNumeric == isoCode);
        }

        /// <summary>Finds all countries except those whose three-letter ISO codes are listed.</summary>
        /// <param name="disabledIso3Codes">The ISO alpha-3 codes to exclude.</param>
        /// <returns>The remaining countries ordered by <see cref="Country.DisplayName"/>.</returns>
        public List<Country> FindAllExceptList(List<string> disabledIso3Codes)
        {
            return FindAll()
                .Where(c => !disabledIso3Codes.Contains(c.IsoAlpha3))
                .OrderBy(c => c.DisplayName)
                .ToList();
        }

        /// <summary>Finds all countries whose three-letter ISO codes are listed.</summary>
        /// <param name="matchingIso3Codes">The ISO alpha-3 codes to include.</param>
        /// <returns>The matching countries in the order returned by <see cref="FindAll"/>.</returns>
        public List<Country> FindAllInList(List<string> matchingIso3Codes)
        {
            return FindAll()
                .Where(c => matchingIso3Codes.Contains(c.IsoAlpha3))
                .ToList();
        }

        /// <summary>Finds all countries not disabled by the current store's ISO alpha-3 country settings.</summary>
        /// <returns>The active countries ordered by <see cref="Country.DisplayName"/>.</returns>
        public List<Country> FindActiveCountries()
        {
            return FindAllExceptList(Context.CurrentStore.Settings.DisabledCountryIso3Codes);
        }

        /// <summary>Finds all countries that have a non-empty culture code suitable for currency formatting.</summary>
        /// <returns>The countries with configured <see cref="Country.CultureCode"/> values.</returns>
        public List<Country> FindAllForCurrency()
        {
            return FindAll()
                .Where(c => !string.IsNullOrEmpty(c.CultureCode))
                .ToList();
        }

        /// <summary>Creates a country and clears the current culture's country cache.</summary>
        /// <param name="country">The country to persist.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        /// <remarks>A missing bvin is replaced with a new GUID before the inherited create operation.</remarks>
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

        /// <summary>Updates a country by its bvin and clears the current culture's country cache.</summary>
        /// <param name="country">The country containing the updated values and bvin.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Update(Country country)
        {
            var countryGuid = DataTypeHelper.BvinToGuid(country.Bvin);
            var result = Update(country, c => c.CountryId == countryGuid);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        /// <summary>Deletes a country by bvin and clears the current culture's country cache.</summary>
        /// <param name="countryId">The country bvin to delete.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Delete(string countryId)
        {
            var countryGuid = DataTypeHelper.BvinToGuid(countryId);
            var result = Delete(c => c.CountryId == countryGuid);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }
    }
}