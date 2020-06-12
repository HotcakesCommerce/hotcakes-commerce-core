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

namespace Hotcakes.Commerce.Orders
{
    public class OrderNoteRepository : HccSimpleRepoBase<hcc_OrderNote, OrderNote>
    {
        public OrderNoteRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_OrderNote, bool> MatchItems(OrderNote item)
        {
            return on => on.Id == item.Id;
        }

        protected override Func<hcc_OrderNote, bool> NotMatchItems(List<OrderNote> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return on => !itemIds.Contains(on.Id);
        }

        protected override void CopyModelToData(hcc_OrderNote data, OrderNote model)
        {
            data.AuditDate = model.AuditDate;
            data.Id = model.Id;
            data.IsPublic = model.IsPublic;
            data.LastUpdatedUtc = model.LastUpdatedUtc;
            data.Note = model.Note;
            data.OrderId = DataTypeHelper.BvinToGuid(model.OrderID);
            data.StoreId = model.StoreId;
        }

        protected override void CopyDataToModel(hcc_OrderNote data, OrderNote model)
        {
            model.AuditDate = data.AuditDate;
            model.Id = data.Id;
            model.IsPublic = data.IsPublic;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.Note = data.Note;
            model.OrderID = DataTypeHelper.GuidToBvin(data.OrderId);
            model.StoreId = data.StoreId;
        }

        public bool Update(OrderNote item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<OrderNote> FindForOrders(List<string> orderBvins)
        {
            var orderGuids = orderBvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => orderGuids.Contains(y.OrderId))
                    .OrderBy(y => y.OrderId)
                    .ThenBy(y => y.Id);
            });
        }

        public List<OrderNote> FindForOrder(string orderBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(orderBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OrderId == guid)
                    .OrderBy(y => y.Id);
            });
        }

        public void DeleteForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            Delete(on => on.OrderId == orderGuid);
        }

        public void MergeList(string orderBvin, long storeId, List<OrderNote> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.OrderID = orderBvin;
                item.StoreId = storeId;
                item.LastUpdatedUtc = DateTime.UtcNow;
            }

            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            MergeList(subitems, on => on.OrderId == orderGuid && on.StoreId == storeId);
        }
    }
}