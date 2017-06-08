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
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Web;

namespace Hotcakes.Commerce.Shipping
{
    public class ZoneRepository : HccSimpleRepoBase<hcc_ShippingZones, Zone>, IZoneRepository
    {
        private readonly CountryRepository _countryRepository;

        public ZoneRepository(HccRequestContext context)
            : base(context)
        {
            _countryRepository = new CountryRepository(context);
        }

        public List<Zone> GetZones(long storeId)
        {
            return FindForStore(storeId);
        }

        protected override void CopyDataToModel(hcc_ShippingZones data, Zone model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Name = data.Name;
            model.Areas = Json.ObjectFromJson<List<ZoneArea>>(data.Areas);
        }

        protected override void CopyModelToData(hcc_ShippingZones data, Zone model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.Name = model.Name;
            data.Areas = Json.ObjectToJson(model.Areas);
        }

        public override bool Create(Zone item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(Zone zone)
        {
            return Update(zone, z => z.Id == zone.Id);
        }

        public bool Delete(long id)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(id, storeId);
        }

        internal bool DeleteForStore(long id, long storeId)
        {
            var item = FindForAllStores(id);
            if (item == null) return false;
            if (item.StoreId != storeId) return false;
            return Delete(z => z.Id == id);
        }

        public Zone Find(long id)
        {
            var result = FindForAllStores(id);
            if (result != null && result.StoreId == Context.CurrentStore.Id)
            {
                return result;
            }
            return null;
        }

        public Zone FindForAllStores(long id)
        {
            return FindFirstPoco(z => z.Id == id);
        }

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

        public List<Zone> FindForStore(long storeId)
        {
            return FindListPoco(q => q.Where(y => y.StoreId == storeId).OrderBy(y => y.Name));
        }

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

        internal Zone UnitedStatesAll()
        {
            var usAll = new Zone();
            usAll.Id = -100;
            usAll.Name = "United States - All";
            usAll.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = string.Empty});
            return usAll;
        }

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

        internal Zone UnitedStatesAlaskaAndHawaii()
        {
            var z = new Zone();
            z.Id = -102;
            z.Name = "United States - Alaska and Hawaii";

            z.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "AK"});
            z.Areas.Add(new ZoneArea {CountryIsoAlpha3 = "USA", RegionAbbreviation = "HI"});

            return z;
        }

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

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ZoneRepository(HccRequestContext context, bool isForMemoryOnly)
            : this(context)
        {
        }

        [Obsolete(
            "Obsolete in 2.0.0. Method violate concept of 3 tier applications. DAL/BL layers are mixed. Use EnsureDefaultZones of OrderService"
            )]
        public void EnsureDefaultZones(long storeId, HotcakesApplication hccApp)
        {
            CreateAndReassign(UnitedStatesAll(), hccApp);
            CreateAndReassign(UnitedStates48Contiguous(), hccApp);
            CreateAndReassign(UnitedStatesAlaskaAndHawaii(), hccApp);
            CreateAndReassign(International("USA"), hccApp);
        }

        [Obsolete("Obsolete in 2.0.0. Method violate concept of 3 tier applications. DAL/BL layers are mixed.")]
        private void CreateAndReassign(Zone zone, HotcakesApplication hccApp)
        {
            var methods = hccApp.OrderServices.ShippingMethods.FindForZones(new List<Zone> {zone});

            if (Create(zone))
            {
                methods.ForEach(m =>
                {
                    m.ZoneId = zone.Id;
                    hccApp.OrderServices.ShippingMethods.Update(m);
                });
            }
        }

        #endregion
    }
}