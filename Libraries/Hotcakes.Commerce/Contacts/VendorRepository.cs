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
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Contacts
{
    public class VendorRepository : HccSimpleRepoBase<hcc_Vendor, VendorManufacturer>
    {
        public VendorRepository(HccRequestContext c)
            : base(c)
        {
        }

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

        public VendorManufacturer FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        public override bool Create(VendorManufacturer item)
        {
            if (item.Bvin == string.Empty)
                item.Bvin = Guid.NewGuid().ToString();
            item.LastUpdated = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

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

        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        public List<VendorManufacturer> FindAll()
        {
            return
                FindListPoco(
                    q => { return q.Where(y => y.StoreId == Context.CurrentStore.Id).OrderBy(y => y.DisplayName); });
        }

        public List<VendorManufacturer> FindAllWithFilter(string filter, int pageNumber, int pageSize, ref int rowCount)
        {
            using (var s = CreateStrategy())
            {
                var query = s.GetQuery()
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