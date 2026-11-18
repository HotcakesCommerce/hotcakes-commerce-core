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
    ///     Provides store-scoped persistence and localized lookup operations for <see cref="Region"/> records.
    /// </summary>
    /// <remarks>
    ///     The base localization repository selects translations using the request context's main content culture and its
    ///     established fallback behavior. Region mutations clear the country cache because countries load regions as
    ///     sub-items, even though region queries themselves are not cached here.
    /// </remarks>
    public class RegionRepository : HccLocalizationRepoBase<hcc_Region, hcc_RegionTranslation, Region, Guid>
    {
        /// <summary>
        ///     Initializes a region repository for the supplied commerce request context.
        /// </summary>
        /// <param name="context">The request context that supplies store scope and the main content culture.</param>
        /// <exception cref="ArgumentNullException"><paramref name="context"/> is <see langword="null"/>.</exception>
            public RegionRepository(HccRequestContext context)
            : base(context)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));
        }

        /// <summary>Gets the expression that maps a region entity to its persisted identifier.</summary>
        protected override Expression<Func<hcc_Region, Guid>> ItemKeyExp => r => r.RegionId;

        /// <summary>Gets the expression that maps a region translation to its region identifier.</summary>
        protected override Expression<Func<hcc_RegionTranslation, Guid>> ItemTranslationKeyExp => rt => rt.RegionId;

        /// <summary>Copies non-localized region fields from the data entity to the model.</summary>
        /// <param name="data">The region data entity.</param>
        /// <param name="model">The region model to populate.</param>
        protected override void CopyItemToModel(hcc_Region data, Region model)
        {
            model.RegionId = data.RegionId;
            model.CountryId = data.CountryId;
            model.Abbreviation = data.Abbreviation;
            model.SystemName = data.SystemName;
        }

        /// <summary>Copies the selected localized region display name to the model.</summary>
        /// <param name="data">The region translation selected by the base localization query.</param>
        /// <param name="model">The region model to populate.</param>
        protected override void CopyTransToModel(hcc_RegionTranslation data, Region model)
        {
            model.DisplayName = data.DisplayName;
        }

        /// <summary>Copies non-localized region fields from the model to the joined persistence item.</summary>
        /// <param name="data">The joined region and translation entities to update.</param>
        /// <param name="model">The region model to persist.</param>
        protected override void CopyModelToItem(JoinedItem<hcc_Region, hcc_RegionTranslation> data, Region model)
        {
            data.Item.RegionId = model.RegionId;
            data.Item.CountryId = model.CountryId;
            data.Item.Abbreviation = model.Abbreviation;
            data.Item.SystemName = model.SystemName;
        }

        /// <summary>Copies the model's localized display name to the joined translation entity.</summary>
        /// <param name="data">The joined region and translation entities to update.</param>
        /// <param name="model">The region model to persist.</param>
        protected override void CopyModelToTrans(JoinedItem<hcc_Region, hcc_RegionTranslation> data, Region model)
        {
            data.ItemTranslation.RegionId = model.RegionId;
            data.ItemTranslation.DisplayName = model.DisplayName;
        }

        /// <summary>Finds all regions for the current request context's main content culture.</summary>
        /// <returns>All regions ordered by localized display name.</returns>
        public List<Region> FindAll()
        {
            return FindListPoco(q => q.OrderBy(r => r.ItemTranslation.DisplayName));
        }

        /// <summary>Finds all regions belonging to one country.</summary>
        /// <param name="countryId">The country identifier used to filter regions.</param>
        /// <returns>The country's regions ordered by localized display name.</returns>
        public List<Region> FindAll(Guid countryId)
        {
            return FindListPoco(q =>
                q.Where(r => r.Item.CountryId == countryId)
                 .OrderBy(r => r.ItemTranslation.DisplayName));
        }

        /// <summary>Finds regions belonging to any country in a supplied identifier list.</summary>
        /// <param name="countryIds">The country identifiers used to filter regions.</param>
        /// <returns>The matching regions, or an empty list when <paramref name="countryIds"/> is null or empty.</returns>
        /// <remarks>The bulk query does not apply an explicit display-name ordering; callers that require ordering should sort the result.</remarks>
        public List<Region> FindAll(List<Guid> countryIds)
        {
            if (countryIds == null || countryIds.Count == 0) return new List<Region>();
            return FindListPoco(q => q.Where(cb => countryIds.Contains(cb.Item.CountryId)));
        }

        /// <summary>Creates a region and clears the country cache for the current content culture.</summary>
        /// <param name="region">The region to persist.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        /// <exception cref="ArgumentNullException"><paramref name="region"/> is <see langword="null"/>.</exception>
        /// <remarks>A missing region identifier is replaced with a new GUID before the inherited create operation.</remarks>
        public override bool Create(Region region)
        {
            if (region == null) throw new ArgumentNullException(nameof(region));
            if (region.RegionId == Guid.Empty)
            {
                region.RegionId = Guid.NewGuid();
            }
            var result = base.Create(region);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        /// <summary>Updates a region by identifier and clears the country cache for the current content culture.</summary>
        /// <param name="region">The region containing the updated values.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Update(Region region)
        {
            var result = Update(region, r => r.RegionId == region.RegionId);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        /// <summary>Deletes a region by identifier and clears the country cache for the current content culture.</summary>
        /// <param name="regionId">The identifier of the region to delete.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Delete(Guid regionId)
        {
            var result = Delete(r => r.RegionId == regionId);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }
    }
}