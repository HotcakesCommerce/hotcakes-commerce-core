#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Contacts
{
    public class AddressRepository : HccSimpleRepoBase<hcc_Address, Address>
    {
        public AddressRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override void CopyDataToModel(hcc_Address data, Address model)
        {
            model.StoreId = data.StoreId;
            model.Bvin = data.bvin;
            model.AddressType = (AddressTypes) data.AddressType;
            model.City = data.City;
            model.Company = data.Company;
            model.CountryBvin = data.CountryBvin;
            model.Fax = data.Fax;
            model.FirstName = data.FirstName;
            model.LastName = data.LastName;
            model.LastUpdatedUtc = data.LastUpdated;
            model.Line1 = data.Line1;
            model.Line2 = data.Line2;
            model.Line3 = data.Line3;
            model.MiddleInitial = data.MiddleInitial;
            model.NickName = data.NickName;
            model.Phone = data.Phone;
            model.PostalCode = data.PostalCode;
            model.RegionBvin = data.RegionBvin;
            model.UserBvin = data.UserBvin;
            model.WebSiteUrl = data.WebSiteUrl;
        }

        protected override void CopyModelToData(hcc_Address data, Address model)
        {
            data.StoreId = model.StoreId;
            data.bvin = model.Bvin;
            data.AddressType = (int) model.AddressType;
            data.City = model.City;
            data.Company = model.Company;
            data.CountryBvin = model.CountryBvin;
            data.CountryName = model.CountrySystemName;
            data.Fax = model.Fax;
            data.FirstName = model.FirstName;
            data.LastName = model.LastName;
            data.LastUpdated = model.LastUpdatedUtc;
            data.Line1 = model.Line1;
            data.Line2 = model.Line2;
            data.Line3 = model.Line3;
            data.MiddleInitial = model.MiddleInitial;
            data.NickName = model.NickName;
            data.Phone = model.Phone;
            data.PostalCode = model.PostalCode;
            data.RegionBvin = model.RegionBvin;
            data.RegionName = model.RegionSystemName;
            data.UserBvin = model.UserBvin;
            data.WebSiteUrl = model.WebSiteUrl;
        }


        public Address Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        public override bool Create(Address item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.Bvin = Guid.NewGuid().ToString();
            return base.Create(item);
        }

        public bool Update(Address item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            item.LastUpdatedUtc = DateTime.UtcNow;
            return Update(item, y => y.bvin == item.Bvin);
        }

        public bool Delete(string bvin)
        {
            return Delete(y => y.bvin == bvin);
        }

        public List<Address> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderBy(y => y.Id); });
        }

        public List<Address> FindByType(AddressTypes t)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.AddressType == (int) t)
                    .OrderBy(y => y.Id);
            });
        }

        public List<Address> FindByUserBvin(string userBvin)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .Where(y => y.UserBvin == userBvin)
                    .OrderBy(y => y.Id);
            });
        }

        public Address FindStoreContactAddress()
        {
            var result = new Address();
            result.CountryBvin = Country.UnitedStatesCountryBvin;
            result.RegionBvin = "VA";
            result.AddressType = AddressTypes.StoreContact;
            var possible = FindByType(AddressTypes.StoreContact);

            if (possible == null || possible.Count < 1)
            {
                Create(result);
                return result;
            }

            result = possible[0];
            return result;
        }
    }
}