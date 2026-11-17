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
using Hotcakes.Shipping;
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Shipping
{
    /// <summary>
    /// Persists and retrieves <see cref="ShippingMethod"/> records and their localized names.
    /// Store-scoped operations use the current store from the supplied <see cref="HccRequestContext"/>;
    /// <see cref="FindAll(long)"/> is available when an explicit store is required.
    /// </summary>
    public class ShippingMethodRepository :
        HccLocalizationRepoBase<hcc_ShippingMethod, hcc_ShippingMethodTranslation, ShippingMethod, Guid>
    {
        /// <summary>
        /// Initializes the repository with the commerce request context used for store scoping.
        /// </summary>
        /// <param name="c">The request context for repository operations.</param>
        public ShippingMethodRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        /// Gets the expression that identifies a persisted shipping method by its BVIN GUID.
        /// </summary>
        protected override Expression<Func<hcc_ShippingMethod, Guid>> ItemKeyExp
        {
            get { return sm => sm.bvin; }
        }

        /// <summary>
        /// Gets the expression that associates a localized shipping method name with its method.
        /// </summary>
        protected override Expression<Func<hcc_ShippingMethodTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return pt => pt.ShippingMethodId; }
        }

        /// <summary>
        /// Maps persisted shipping method values, including enum values and JSON settings, to a model.
        /// </summary>
        /// <param name="data">The persisted shipping method row.</param>
        /// <param name="model">The model receiving the row values.</param>
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

        /// <summary>
        /// Maps the persisted localized name to a shipping method model.
        /// </summary>
        /// <param name="data">The persisted translation row.</param>
        /// <param name="model">The model receiving the translated name.</param>
        protected override void CopyTransToModel(hcc_ShippingMethodTranslation data, ShippingMethod model)
        {
            model.Name = data.Name;
        }

        /// <summary>
        /// Maps shipping method model values to the persisted row, serializing settings as JSON.
        /// </summary>
        /// <param name="data">The joined persisted item receiving the values.</param>
        /// <param name="model">The shipping method model to persist.</param>
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

        /// <summary>
        /// Maps the model's localized name to the joined persisted translation row.
        /// </summary>
        /// <param name="data">The joined persisted item receiving the translation.</param>
        /// <param name="model">The shipping method model to persist.</param>
        protected override void CopyModelToTrans(JoinedItem<hcc_ShippingMethod, hcc_ShippingMethodTranslation> data,
            ShippingMethod model)
        {
            data.ItemTranslation.ShippingMethodId = data.Item.bvin;

            data.ItemTranslation.Name = model.Name;
        }

        /// <summary>
        /// Creates a shipping method for the current store, assigning a BVIN when needed and
        /// initializing its update timestamp and default visibility.
        /// </summary>
        /// <param name="item">The shipping method to create.</param>
        /// <returns><see langword="true"/> when the base repository creates the method.</returns>
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

        /// <summary>
        /// Updates a shipping method selected by its BVIN and refreshes its UTC update timestamp.
        /// </summary>
        /// <param name="item">The shipping method values to persist.</param>
        /// <returns><see langword="true"/> when the update succeeds.</returns>
        public bool Update(ShippingMethod item)
        {
            item.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return Update(item, y => y.bvin == guid);
        }

        /// <summary>
        /// Deletes the shipping method with the specified BVIN from the current store.
        /// </summary>
        /// <param name="bvin">The shipping method BVIN.</param>
        /// <returns><see langword="true"/> when the repository deletes the method.</returns>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        /// <summary>
        /// Deletes a shipping method for a store identifier. The underlying predicate uses the current
        /// request store context rather than the supplied <paramref name="storeId"/> value.
        /// </summary>
        /// <param name="bvin">The shipping method BVIN.</param>
        /// <param name="storeId">The store identifier checked before invoking the repository delete operation.</param>
        /// <returns><see langword="true"/> when the repository delete operation succeeds.</returns>
        internal bool DeleteForStore(string bvin, long storeId)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid && y.StoreId == Context.CurrentStore.Id);
        }

        /// <summary>
        /// Finds a shipping method in the current store by BVIN. The virtual method identifiers
        /// <see cref="ShippingMethod.MethodUnknown"/>, <see cref="ShippingMethod.MethodToBeDetermined"/>,
        /// and <see cref="ShippingMethod.MethodFreeShipping"/> do not represent persisted methods and return
        /// <see langword="null"/>.
        /// </summary>
        /// <param name="bvin">The persisted shipping method BVIN or a virtual method identifier.</param>
        /// <returns>The matching method, or <see langword="null"/> when none is persisted for the current store.</returns>
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

        /// <summary>
        /// Finds a shipping method by BVIN without restricting the query to the current store.
        /// </summary>
        /// <param name="bvin">The shipping method BVIN.</param>
        /// <returns>The first matching method across stores, or <see langword="null"/>.</returns>
        internal ShippingMethod FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid);
        }

        /// <summary>
        /// Gets all shipping methods for a store, ordered by localized name.
        /// </summary>
        /// <param name="storeId">The store identifier whose methods are requested.</param>
        /// <returns>The store's shipping methods ordered by name.</returns>
        public List<ShippingMethod> FindAll(long storeId)
        {
            return
                FindListPoco(
                    q => { return q.Where(y => y.Item.StoreId == storeId).OrderBy(y => y.ItemTranslation.Name); });
        }

        /// <summary>
        /// Gets visible shipping methods in the current store whose zone identifier matches one of the supplied zones.
        /// Methods configured with <see cref="ShippingVisibilityMode.Never"/> are excluded.
        /// </summary>
        /// <param name="zones">The zones eligible for the address or shipping request.</param>
        /// <returns>The matching visible shipping methods.</returns>
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

        /// <summary>
        /// Assigns sequential sort orders to shipping methods in the supplied BVIN order.
        /// </summary>
        /// <param name="sortedIds">The ordered shipping method BVINs.</param>
        /// <param name="orderOffset">The value added before the one-based position of each method.</param>
        /// <returns>Always <see langword="true"/> after processing the supplied list.</returns>
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

        /// <summary>
        /// Deletes shipping methods whose persisted store identifier equals <paramref name="storeId"/>.
        /// The method name is compatibility-sensitive and intentionally retains its existing spelling.
        /// </summary>
        /// <param name="storeId">The store identifier whose shipping methods are deleted.</param>
        internal void DestoryAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
