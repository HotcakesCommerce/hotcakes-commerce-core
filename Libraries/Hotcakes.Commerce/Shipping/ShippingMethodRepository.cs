#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Shipping;
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Shipping
{
    public class ShippingMethodRepository :
        HccLocalizationRepoBase<hcc_ShippingMethod, hcc_ShippingMethodTranslation, ShippingMethod, Guid>
    {
        public ShippingMethodRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Expression<Func<hcc_ShippingMethod, Guid>> ItemKeyExp
        {
            get { return sm => sm.bvin; }
        }

        protected override Expression<Func<hcc_ShippingMethodTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return pt => pt.ShippingMethodId; }
        }

        protected override void CopyItemToModel(hcc_ShippingMethod data, ShippingMethod model)
        {
            model.Adjustment = data.Adjustment;
            model.AdjustmentType = (ShippingMethodAdjustmentType) data.AdjustmentType;
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.LastUpdated = data.LastUpdated;
            model.Settings = Json.ObjectFromJson<ServiceSettings>(data.Settings);
            model.ShippingProviderId = data.ShippingProviderId;
            model.StoreId = data.StoreId;
            model.ZoneId = data.ZoneId;
            model.VisibilityMode = (ShippingVisibilityMode) data.VisibilityMode;
            model.VisibilityAmount = data.VisibilityAmount;
            model.SortOrder = data.SortOrder;
        }

        protected override void CopyTransToModel(hcc_ShippingMethodTranslation data, ShippingMethod model)
        {
            model.Name = data.Name;
        }

        protected override void CopyModelToItem(JoinedItem<hcc_ShippingMethod, hcc_ShippingMethodTranslation> data,
            ShippingMethod model)
        {
            data.Item.Adjustment = model.Adjustment;
            data.Item.AdjustmentType = (int) model.AdjustmentType;
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.LastUpdated = model.LastUpdated;
            data.Item.Settings = Json.ObjectToJson(model.Settings);
            data.Item.ShippingProviderId = model.ShippingProviderId;
            data.Item.StoreId = model.StoreId;
            data.Item.ZoneId = model.ZoneId;
            data.Item.VisibilityMode = (int) model.VisibilityMode;
            data.Item.VisibilityAmount = model.VisibilityAmount;
            data.Item.SortOrder = model.SortOrder;
        }

        protected override void CopyModelToTrans(JoinedItem<hcc_ShippingMethod, hcc_ShippingMethodTranslation> data,
            ShippingMethod model)
        {
            data.ItemTranslation.ShippingMethodId = data.Item.bvin;

            data.ItemTranslation.Name = model.Name;
        }

        public override bool Create(ShippingMethod item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.LastUpdated = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            item.VisibilityMode = ShippingVisibilityMode.Always;
            return base.Create(item);
        }

        public bool Update(ShippingMethod item)
        {
            item.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return Update(item, y => y.bvin == guid);
        }

        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid && y.StoreId == Context.CurrentStore.Id);
        }

        public ShippingMethod Find(string bvin)
        {
            string[] virtualMethods =
            {
                ShippingMethod.MethodUnknown, ShippingMethod.MethodToBeDetermined,
                ShippingMethod.MethodFreeShipping
            };
            if (virtualMethods.Contains(bvin))
                return null;
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid && y.Item.StoreId == Context.CurrentStore.Id);
        }

        internal ShippingMethod FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid);
        }

        public List<ShippingMethod> FindAll(long storeId)
        {
            return
                FindListPoco(
                    q => { return q.Where(y => y.Item.StoreId == storeId).OrderBy(y => y.ItemTranslation.Name); });
        }

        public List<ShippingMethod> FindForZones(List<Zone> zones)
        {
            var storeId = Context.CurrentStore.Id;
            var all = FindAll(storeId);

            var result = new List<ShippingMethod>();

            foreach (var m in all)
            {
                if (m.VisibilityMode == ShippingVisibilityMode.Never)
                {
                    continue;
                }

                foreach (var z in zones)
                {
                    if (m.ZoneId == z.Id)
                    {
                        result.Add(m);
                    }
                }
            }
            return result;
        }

        public bool Resort(List<string> sortedIds, int orderOffset = 0)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrder(sortedIds[i - 1], orderOffset + i);
                }
            }
            return true;
        }

        private bool UpdateSortOrder(string id, int newSortOrder)
        {
            var item = Find(id);
            if (item == null) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}