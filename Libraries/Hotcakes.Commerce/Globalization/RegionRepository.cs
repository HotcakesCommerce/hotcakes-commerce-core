#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
    public class RegionRepository : HccLocalizationRepoBase<hcc_Region, hcc_RegionTranslation, Region, Guid>
    {
        public RegionRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Expression<Func<hcc_Region, Guid>> ItemKeyExp
        {
            get { return r => r.RegionId; }
        }

        protected override Expression<Func<hcc_RegionTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return rt => rt.RegionId; }
        }

        protected override void CopyItemToModel(hcc_Region data, Region model)
        {
            model.RegionId = data.RegionId;
            model.CountryId = data.CountryId;
            model.Abbreviation = data.Abbreviation;
            model.SystemName = data.SystemName;
        }

        protected override void CopyTransToModel(hcc_RegionTranslation data, Region model)
        {
            model.DisplayName = data.DisplayName;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_Region, hcc_RegionTranslation> data, Region model)
        {
            data.Item.RegionId = model.RegionId;
            data.Item.CountryId = model.CountryId;
            data.Item.Abbreviation = model.Abbreviation;
            data.Item.SystemName = model.SystemName;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_Region, hcc_RegionTranslation> data, Region model)
        {
            data.ItemTranslation.RegionId = model.RegionId;

            data.ItemTranslation.DisplayName = model.DisplayName;
        }

        public List<Region> FindAll()
        {
            return FindListPoco(q => { return q.OrderBy(r => r.ItemTranslation.DisplayName); });
        }

        public List<Region> FindAll(Guid countryId)
        {
            return FindListPoco(q =>
            {
                return q.Where(r => r.Item.CountryId == countryId)
                    .OrderBy(r => r.ItemTranslation.DisplayName);
            });
        }

        public List<Region> FindAll(List<Guid> countryIds)
        {
            return FindListPoco(q => q.Where(cb => countryIds.Contains(cb.Item.CountryId)));
        }

        public override bool Create(Region region)
        {
            if (region.RegionId == Guid.Empty)
            {
                region.RegionId = Guid.NewGuid();
            }
            var result = base.Create(region);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        public bool Update(Region region)
        {
            var result = Update(region, r => r.RegionId == region.RegionId);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }

        public bool Delete(Guid regionId)
        {
            var result = Delete(r => r.RegionId == regionId);
            CacheManager.ClearCountries(Context.MainContentCulture);
            return result;
        }
    }
}