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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Contacts
{
    [Obsolete("Obsolete in 1.8.0. VendorManufacturer contacts are not used")]
    public class VendorManufacturerContactRepository : HccSimpleRepoBase<hcc_UserXContact, VendorManufacturerContact>
    {
        public VendorManufacturerContactRepository(HccRequestContext context)
            : base(context)
        {
        }

        protected override Func<hcc_UserXContact, bool> MatchItems(VendorManufacturerContact item)
        {
            return vmc => vmc.ContactId == item.VendorManufacturerId;
        }

        protected override Func<hcc_UserXContact, bool> NotMatchItems(List<VendorManufacturerContact> items)
        {
            var itemIds = items.Select(i => i.VendorManufacturerId).ToList();
            return vmc => !itemIds.Contains(vmc.ContactId);
        }

        protected override void CopyDataToModel(hcc_UserXContact data, VendorManufacturerContact model)
        {
            model.Id = data.Id;
            model.VendorManufacturerId = data.ContactId;
            model.StoreId = data.StoreId;
            model.UserId = data.UserId;
        }

        protected override void CopyModelToData(hcc_UserXContact data, VendorManufacturerContact model)
        {
            data.Id = model.Id;
            data.ContactId = model.VendorManufacturerId;
            data.StoreId = model.StoreId;
            data.UserId = model.UserId;
        }

        public VendorManufacturerContact Find(long id)
        {
            var result = FindForAllStores(id);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        public VendorManufacturerContact FindForAllStores(long id)
        {
            return FindFirstPoco(c => c.Id == id);
        }

        public override bool Create(VendorManufacturerContact item)
        {
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(VendorManufacturerContact contact)
        {
            if (contact.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(contact, c => c.Id == contact.Id);
        }

        public bool Delete(long id)
        {
            return Delete(c => c.Id == id);
        }

        public List<VendorManufacturerContact> FindForVendorManufacturer(string bvin)
        {
            return FindListPoco(q => q.Where(y => y.ContactId == bvin));
        }

        public void DeleteForVendorManufacturer(string bvin)
        {
            var existing = FindForVendorManufacturer(bvin);
            foreach (var sub in existing)
            {
                Delete(sub.Id);
            }
        }

        public void MergeList(string bvin, List<VendorManufacturerContact> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.VendorManufacturerId = bvin;
            }

            MergeList(subitems, vmc => vmc.ContactId == bvin);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static VendorManufacturerContactRepository InstantiateForMemory(HccRequestContext c)
        {
            return new VendorManufacturerContactRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static VendorManufacturerContactRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new VendorManufacturerContactRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public VendorManufacturerContactRepository(HccRequestContext c, IRepositoryStrategy<hcc_UserXContact> r,
            ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}