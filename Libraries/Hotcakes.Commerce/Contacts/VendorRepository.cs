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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Provides store-scoped persistence for <see cref="VendorManufacturer"/> records representing vendors.</summary>
    /// <remarks>Addresses are serialized to and from the vendor table's XML address column.</remarks>
    public class VendorRepository : HccSimpleRepoBase<hcc_Vendor, VendorManufacturer>
    {
        /// <summary>Initializes the repository with the request context used to resolve the current store.</summary>
        /// <param name="c">The Hotcakes request context.</param>
        public VendorRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Copies a database vendor row into the domain model.</summary>
        /// <param name="data">The database row.</param>
        /// <param name="model">The destination domain model.</param>
        /// <remarks>This mapping deserializes the address XML and forces <see cref="VendorManufacturerType.Vendor"/>.</remarks>
        protected override void CopyDataToModel(hcc_Vendor data, VendorManufacturer model)
        {
            model.Address.FromXmlString(data.Address);
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.DisplayName = data.DisplayName;
            model.DropShipEmailTemplateId = data.DropShipEmailTemplateId;
            model.EmailAddress = data.EmailAddress;
            model.LastUpdated = data.LastUpdated;
            model.StoreId = data.StoreId;
            model.ContactType = VendorManufacturerType.Vendor;
        }

        /// <summary>Copies a vendor domain model into its database row.</summary>
        /// <param name="data">The destination database row.</param>
        /// <param name="model">The source domain model.</param>
        /// <remarks>This mapping serializes the address to XML; the repository's vendor table determines the contact type.</remarks>
        protected override void CopyModelToData(hcc_Vendor data, VendorManufacturer model)
        {
            data.Address = model.Address.ToXml(true);
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.DisplayName = model.DisplayName;
            data.DropShipEmailTemplateId = model.DropShipEmailTemplateId;
            data.EmailAddress = model.EmailAddress;
            data.LastUpdated = model.LastUpdated;
            data.StoreId = model.StoreId;
        }

        /// <summary>Finds a vendor by identifier only when it belongs to the current store.</summary>
        /// <param name="bvin">The vendor identifier.</param>
        /// <returns>The matching vendor, or <see langword="null"/> when absent or owned by another store.</returns>
        public VendorManufacturer Find(string bvin)
        {
            var result = FindForAllStores(bvin);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>Finds a vendor by identifier without applying current-store filtering.</summary>
        /// <param name="bvin">The vendor identifier.</param>
        /// <returns>The matching vendor, or <see langword="null"/> when no row matches.</returns>
        /// <remarks>Use <see cref="Find(string)"/> when enforcing current-store ownership is required.</remarks>
        public VendorManufacturer FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        /// <summary>Creates a vendor in the current store.</summary>
        /// <param name="item">The vendor to persist.</param>
        /// <returns><see langword="true"/> when the base repository creates the record.</returns>
        /// <remarks>An empty identifier is generated, the update timestamp is refreshed, and store ownership is assigned from the current context.</remarks>
        public override bool Create(VendorManufacturer item)
        {
            if (item.Bvin == string.Empty)
                item.Bvin = Guid.NewGuid().ToString();
            item.LastUpdated = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>Updates a vendor only when its store matches the current store.</summary>
        /// <param name="c">The vendor to update.</param>
        /// <returns><see langword="false"/> for a different store; otherwise the base update result.</returns>
        public bool Update(VendorManufacturer c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }

            c.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        /// <summary>Deletes the vendor row identified by the supplied identifier.</summary>
        /// <param name="bvin">The vendor identifier.</param>
        /// <returns>The repository delete result.</returns>
        /// <remarks>The implementation delegates directly to the base predicate and does not add a separate store check.</remarks>
        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        /// <summary>Returns all vendors for the current store, ordered by display name.</summary>
        /// <returns>The current store's vendors.</returns>
        public List<VendorManufacturer> FindAll()
        {
            return
                FindListPoco(
                    q => { return q.Where(y => y.StoreId == Context.CurrentStore.Id).OrderBy(y => y.DisplayName); });
        }

        /// <summary>Returns a page of current-store vendors matching display name or e-mail address.</summary>
        /// <param name="filter">The substring to search for in display name or e-mail address.</param>
        /// <param name="pageNumber">The requested page number.</param>
        /// <param name="pageSize">The maximum number of records in the page.</param>
        /// <param name="rowCount">Receives the total matching row count before paging.</param>
        /// <returns>The requested page, ordered by display name.</returns>
        public List<VendorManufacturer> FindAllWithFilter(string filter, int pageNumber, int pageSize, ref int rowCount)
        {
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.DisplayName.Contains(filter) || y.EmailAddress.Contains(filter))
                    .OrderBy(y => y.DisplayName);

                rowCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
