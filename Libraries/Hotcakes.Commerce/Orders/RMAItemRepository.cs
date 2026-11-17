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
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    /// Represents the RMAItemRepository order-domain type and preserves the established public compatibility surface.
    /// </summary>
    public class RMAItemRepository : HccSimpleRepoBase<hcc_RMAItem, RMAItem>
    {
        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
        public RMAItemRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
        protected override Func<hcc_RMAItem, bool> MatchItems(RMAItem item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return ri => ri.bvin == guid;
        }

        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
        protected override Func<hcc_RMAItem, bool> NotMatchItems(List<RMAItem> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return ri => !itemGuids.Contains(ri.bvin);
        }

        /// <summary>
        /// Provides the CopyModelToData operation while preserving the established order-domain behavior.
        /// </summary>
        protected override void CopyModelToData(hcc_RMAItem data, RMAItem model)
        {
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.ItemDescription = model.ItemDescription;
            data.ItemName = model.ItemName;
            data.LastUpdated = model.LastUpdated;
            data.LineItemBvin = model.LineItemId;
            data.Note = model.Note;
            data.ProductClass = model.ProductClass;
            data.Quantity = model.Quantity;
            data.QuantityReceived = model.QuantityReceived;
            data.QuantityReturnedToInventory = model.QuantityReturnedToInventory;
            data.Reason = model.Reason;
            data.RefundAmount = model.RefundAmount;
            data.RefundGiftWrapAmount = model.RefundGiftWrapAmount;
            data.RefundShippingAmount = model.RefundShippingAmount;
            data.RefundTaxAmount = model.RefundTaxAmount;
            data.Replace = model.Replace;
            data.RMABvin = DataTypeHelper.BvinToGuid(model.RMABvin);
            data.StoreId = model.StoreId;
        }

        /// <summary>
        /// Provides the CopyDataToModel operation while preserving the established order-domain behavior.
        /// </summary>
        protected override void CopyDataToModel(hcc_RMAItem data, RMAItem model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.ItemDescription = data.ItemDescription;
            model.ItemName = data.ItemName;
            model.LastUpdated = data.LastUpdated;
            model.LineItemId = data.LineItemBvin;
            model.Note = data.Note;
            model.ProductClass = data.ProductClass;
            model.Quantity = data.Quantity;
            model.QuantityReceived = data.QuantityReceived;
            model.QuantityReturnedToInventory = data.QuantityReturnedToInventory;
            model.Reason = data.Reason;
            model.RefundAmount = data.RefundAmount;
            model.RefundGiftWrapAmount = data.RefundGiftWrapAmount;
            model.RefundShippingAmount = data.RefundShippingAmount;
            model.RefundTaxAmount = data.RefundTaxAmount;
            model.Replace = data.Replace;
            model.RMABvin = DataTypeHelper.GuidToBvin(data.RMABvin);
            model.StoreId = data.StoreId;
        }

        /// <summary>
        /// Provides the Find operation while preserving the established order-domain behavior.
        /// </summary>
        public RMAItem Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        /// <summary>
        /// Provides the Create operation while preserving the established order-domain behavior.
        /// </summary>
        public override bool Create(RMAItem item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;

            return base.Create(item);
        }

        /// <summary>
        /// Provides the Update operation while preserving the established order-domain behavior.
        /// </summary>
        public bool Update(RMAItem item)
        {
            var rmaItemId = DataTypeHelper.BvinToGuid(item.Bvin);
            return base.Update(item, y => y.bvin == rmaItemId);
        }

        /// <summary>
        /// Provides the Delete operation while preserving the established order-domain behavior.
        /// </summary>
        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        /// <summary>
        /// Finds the requested records within the established repository store scope and ordering.
        /// </summary>
        public List<RMAItem> FindForRMA(string rmaBvin)
        {
            var rmaId = DataTypeHelper.BvinToGuid(rmaBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.RMABvin == rmaId)
                    .OrderBy(y => y.LastUpdated);
            });
        }

        /// <summary>
        /// Finds the requested records within the established repository store scope and ordering.
        /// </summary>
        public List<RMAItem> FindForRMA(List<string> bvinIds)
        {
            var rmaIds = bvinIds.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return FindListPoco(q => q.Where(cb => rmaIds.Contains(cb.bvin)));
        }

        /// <summary>
        /// Deletes all records associated with the specified parent identifier.
        /// </summary>
        public void DeleteForRMA(string rmaBvin)
        {
            var rmaId = DataTypeHelper.BvinToGuid(rmaBvin);
            Delete(y => y.RMABvin == rmaId);
        }

        /// <summary>
        /// Merges the supplied records after assigning their established parent and store scope.
        /// </summary>
        public void MergeList(string rmaBvin, List<RMAItem> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.RMABvin = rmaBvin;
                item.StoreId = Context.CurrentStore.Id;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var rmaGuid = DataTypeHelper.BvinToGuid(rmaBvin);
            MergeList(subitems, ri => ri.RMABvin == rmaGuid);
        }
    }
}
