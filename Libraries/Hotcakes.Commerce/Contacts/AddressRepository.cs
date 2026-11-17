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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>
    /// Provides store-scoped persistence operations for <see cref="Address"/> records.
    /// </summary>
    public class AddressRepository : HccSimpleRepoBase<hcc_Address, Address>
    {
        /// <summary>
        /// Initializes the repository with the commerce request context used for store scoping.
        /// </summary>
        /// <param name="context">The current commerce request context.</param>
        public AddressRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Copies persisted address data into the domain model.
        /// </summary>
        /// <param name="data">The persisted address record.</param>
        /// <param name="model">The domain address to populate.</param>
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

        /// <summary>
        /// Copies the domain address into its persisted data record.
        /// </summary>
        /// <param name="data">The persisted address record to populate.</param>
        /// <param name="model">The domain address containing the values to persist.</param>
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


        /// <summary>
        /// Finds an address by its unique bvin.
        /// </summary>
        /// <param name="bvin">The address bvin.</param>
        /// <returns>The matching address, or <see langword="null"/> when no record is found.</returns>
        public Address Find(string bvin)
        {
            return FindFirstPoco(y => y.bvin == bvin);
        }

        /// <summary>
        /// Creates an address in the current store, assigning its store, timestamp, and bvin.
        /// </summary>
        /// <param name="item">The address to create.</param>
        /// <returns><see langword="true"/> when persistence succeeds; otherwise, <see langword="false"/>.</returns>
        public override bool Create(Address item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdatedUtc = DateTime.UtcNow;
            item.Bvin = Guid.NewGuid().ToString();
            return base.Create(item);
        }

        /// <summary>
        /// Updates an address only when it belongs to the current store.
        /// </summary>
        /// <param name="item">The address to update.</param>
        /// <returns><see langword="true"/> when the update succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Update(Address item)
        {
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            item.LastUpdatedUtc = DateTime.UtcNow;
            return Update(item, y => y.bvin == item.Bvin);
        }

        /// <summary>
        /// Deletes the address identified by the bvin.
        /// </summary>
        /// <param name="bvin">The address bvin.</param>
        /// <returns><see langword="true"/> when the delete operation succeeds; otherwise, <see langword="false"/>.</returns>
        public bool Delete(string bvin)
        {
            return Delete(y => y.bvin == bvin);
        }

        /// <summary>
        /// Returns all addresses belonging to the current store in repository order.
        /// </summary>
        /// <returns>The current store's addresses.</returns>
        public List<Address> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.Where(y => y.StoreId == storeId).OrderBy(y => y.Id); });
        }

        /// <summary>
        /// Finds all current-store addresses having the specified address type.
        /// </summary>
        /// <param name="t">The address type to match.</param>
        /// <returns>Matching addresses ordered by their persisted identifier.</returns>
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

        /// <summary>
        /// Finds all current-store addresses associated with a user bvin.
        /// </summary>
        /// <param name="userBvin">The user bvin to match.</param>
        /// <returns>Matching addresses ordered by their persisted identifier.</returns>
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

        /// <summary>
        /// Returns the first store-contact address for the current store, creating a default one when none exists.
        /// </summary>
        /// <returns>The existing or newly created store-contact address.</returns>
        /// <remarks>The default address retains the historical United States and Virginia values.</remarks>
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
