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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Persists store-owned <see cref="Zone"/> records and evaluates shipping addresses against their areas.
    /// Built-in zones use negative identifiers and are constructed in memory; persisted zones are scoped by store.
    /// </summary>
    public class ZoneRepository : HccSimpleRepoBase<hcc_ShippingZones, Zone>, IZoneRepository
    {
        private readonly CountryRepository _countryRepository;

        /// <summary>
        /// Initializes the repository and its country lookup repository with the same request context.
        /// </summary>
        /// <param name="context">The commerce request context used for repository operations and store scope.</param>
        public ZoneRepository(HccRequestContext context)
            : base(context)
        {
            _countryRepository = new CountryRepository(context);
        }

        /// <summary>
        /// Gets the zones belonging to a store, in repository name order.
        /// </summary>
        /// <param name="storeId">The store identifier whose zones are requested.</param>
        /// <returns>The zones for the store.</returns>
        public List<Zone> GetZones(long storeId)
        {
            return FindForStore(storeId);
        }

        /// <summary>
        /// Maps a persisted zone row to a model, deserializing its area list from JSON.
        /// </summary>
        /// <param name="data">The persisted zone row.</param>
        /// <param name="model">The zone model receiving the values.</param>
        protected override void CopyDataToModel(hcc_ShippingZones data, Zone model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Name = data.Name;
            model.Areas = Json.ObjectFromJson<List<ZoneArea>>(data.Areas);
        }

        /// <summary>
        /// Maps a zone model to a persisted row, serializing its area list as JSON.
        /// </summary>
        /// <param name="data">The persisted row receiving the values.</param>
        /// <param name="model">The zone model to persist.</param>
        protected override void CopyModelToData(hcc_ShippingZones data, Zone model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.Name = model.Name;
            data.Areas = Json.ObjectToJson(model.Areas);
        }

        /// <summary>
        /// Creates a zone in the current store, overriding the model store identifier with the current store.
        /// </summary>
        /// <param name="item">The zone to create.</param>
        /// <returns><see langword="true"/> when the base repository creates the zone.</returns>
        public override bool Create(Zone item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        /// Updates the zone identified by its model identifier.
        /// </summary>
        /// <param name="zone">The zone values to persist.</param>
        /// <returns><see langword="true"/> when the update succeeds.</returns>
        public bool Update(Zone zone)
        {
            return Update(zone, z => z.Id == zone.Id);
        }

        /// <summary>
        /// Deletes a zone belonging to the current store.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <returns><see langword="true"/> when the zone is deleted.</returns>
        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        /// <summary>
        /// Deletes a zone only when the located zone belongs to the supplied store.
        /// The final repository predicate is intentionally not store-qualified; ownership is checked before deletion.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <param name="storeId">The store identifier that must own the zone.</param>
        /// <returns><see langword="false"/> when no zone is found or ownership does not match; otherwise the delete result.</returns>
        internal bool DeleteForStore(long id, long storeId)
        {
            var item = FindForAllStores(id);
            if (item == null) return false;
            if (item.StoreId != storeId) return false;
            return Delete(z => z.Id == id);
        }

        /// <summary>
        /// Finds a zone by identifier only when it belongs to the current store.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <returns>The current-store zone, or <see langword="null"/> for another store or a missing identifier.</returns>
        public Zone Find(long id)
        {
            var result = FindForAllStores(id);
            if (result != null && result.StoreId == Context.CurrentStore.Id)
            {
                return result;
            }
            return null;
        }

        /// <summary>
        /// Finds a zone by identifier without applying current-store filtering.
        /// </summary>
        /// <param name="id">The zone identifier.</param>
        /// <returns>The first matching zone, or <see langword="null"/>.</returns>
        public Zone FindForAllStores(long id)
        {
            return FindFirstPoco(z => z.Id == id);
        }

        /// <summary>
        /// Determines whether another zone in a store has the supplied name, using trimmed,
        /// case-insensitive comparison.
        /// </summary>
        /// <param name="name">The name to compare.</param>
        /// <param name="currentId">The identifier excluded from the duplicate check.</param>
        /// <param name="storeId">The store identifier to inspect.</param>
        /// <returns><see langword="true"/> when another zone has the same normalized name.</returns>
        public bool NameExists(string name, long currentId, long storeId)
        {
            var result = false;

            var zones = FindForStore(storeId);
            if (zones != null)
            {
                foreach (var z in zones)
                {
                    if (z.Name.Trim().ToLowerInvariant() == name.Trim().ToLowerInvariant())
                    {
                        if (z.Id != currentId)
                        {
                            return true;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Gets all persisted zones for a store, ordered by name.
        /// </summary>
        /// <param name="storeId">The store identifier whose zones are requested.</param>
        /// <returns>The store's zones ordered by name.</returns>
        public List<Zone> FindForStore(long storeId)
        {
            return FindListPoco(q => q.Where(y => y.StoreId == storeId).OrderBy(y => y.Name));
        }

        /// <summary>
        /// Finds every zone for a store whose areas match an address.
        /// Country codes and region abbreviations are compared case-insensitively after trimming;
        /// an area with an empty region abbreviation matches every region in its country.
        /// </summary>
        /// <param name="address">The address to test.</param>
        /// <param name="storeId">The store identifier whose zones are tested.</param>
        /// <returns>All matching zones in the store's name order.</returns>
        public List<Zone> FindAllZonesForAddress(Address address, long storeId)
        {
            var all = FindForStore(storeId);

            var result = new List<Zone>();

            foreach (var z in all)
            {
                if (AddressIsInZone(z, address))
                {
                    result.Add(z);
                }
            }
            return result;
        }


        /// <summary>
        /// Deletes all persisted zones whose store identifier equals <paramref name="storeId"/>.
        /// The method name is compatibility-sensitive and intentionally retains its existing spelling.
        /// </summary>
        /// <param name="storeId">The store identifier whose zones are deleted.</param>
        internal void DestoryAllForStore(long storeId)
        {
            Delete(z => z.StoreId == storeId);
        }

        private bool AddressIsInZone(Zone zone, Address address)
        {
            var result = false;

            var c = _countryRepository.Find(address.CountryBvin);
            if (c != null)
            {
                foreach (var a in zone.Areas)
                {
                    if (a.CountryIsoAlpha3.Trim().ToLowerInvariant() ==
                        c.IsoAlpha3.Trim().ToLowerInvariant())
                    {
                        // Country matches
                        if (string.IsNullOrWhiteSpace(a.RegionAbbreviation))
                        {
                            // empty region abbreviation means match all
                            return true;
                        }
                        if (address.RegionBvin.Trim().ToLowerInvariant() ==
                            a.RegionAbbreviation.Trim().ToLowerInvariant())
                        {
                            return true;
                        }
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// Creates the built-in United States zone that matches every United States region.
        /// Its negative identifier and literal name are part of the built-in zone contract.
        /// </summary>
        /// <returns>A non-persisted built-in zone with identifier -100.</returns>
        internal Zone UnitedStatesAll()
        {
            var usAll = new Zone();
            usAll.Id = -100;
            usAll.Name = "United States - All";
            usAll.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = string.Empty});
            return usAll;
        }

        /// <summary>
        /// Creates the built-in contiguous United States zone with identifier -101.
        /// The established area data, including AE, AA, AP, and the duplicate AE entry, is preserved exactly.
        /// </summary>
        /// <returns>A non-persisted built-in zone containing the established contiguous-US area list.</returns>
        internal Zone UnitedStates48Contiguous()
        {
            var us48 = new Zone();
            us48.Id = -101;
            us48.Name = "United States - 48 contiguous states";

            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AL"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AZ"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AR"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AE"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AE"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AP"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "CA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "CO"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "CT"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "DE"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "DC"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "FL"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "GA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "ID"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "IL"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "IN"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "IA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "KS"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "KY"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "LA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "ME"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MD"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MI"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MN"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MS"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MO"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "MT"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NE"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NV"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NH"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NJ"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NM"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NY"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "NC"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "ND"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "OH"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "OK"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "OR"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "PA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "RI"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "SC"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "SD"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "TN"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "TX"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "UT"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "VT"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "VA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "WA"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "WV"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "WI"});
            us48.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "WY"});

            return us48;
        }

        /// <summary>
        /// Creates the built-in United States Alaska and Hawaii zone with identifier -102.
        /// </summary>
        /// <returns>A non-persisted built-in zone containing AK and HI.</returns>
        internal Zone UnitedStatesAlaskaAndHawaii()
        {
            var z = new Zone();
            z.Id = -102;
            z.Name = "United States - Alaska and Hawaii";

            z.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AK"});
            z.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "HI"});

            return z;
        }

        /// <summary>
        /// Creates the built-in international zone with identifier -103 by adding every country except
        /// the supplied home country. Areas use empty regions, so each included country matches all regions.
        /// </summary>
        /// <param name="homeCountryIsoAlpha3">The home country's ISO alpha-3 code to exclude.</param>
        /// <returns>A non-persisted built-in international zone.</returns>
        internal Zone International(string homeCountryIsoAlpha3)
        {
            var international = new Zone();
            international.Id = -103;
            international.Name = "International";
            foreach (var c in _countryRepository.FindAll())
            {
                if (c.IsoAlpha3.Trim().ToLowerInvariant() != homeCountryIsoAlpha3.Trim().ToLowerInvariant())
                {
                    international.Areas.Add(new ZoneArea {CountryIsoAlpha3 = c.IsoAlpha3, RegionAbbreviation = string.Empty});
                }
            }

            return international;
        }
    }
}
