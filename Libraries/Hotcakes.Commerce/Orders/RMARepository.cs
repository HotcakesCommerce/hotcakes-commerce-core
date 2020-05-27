#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

namespace Hotcakes.Commerce.Orders
{
    public class RMARepository : HccSimpleRepoBase<hcc_RMA, RMA>
    {
        private readonly RMAItemRepository itemRepository;

        public RMARepository(HccRequestContext c)
            : base(c)
        {
            itemRepository = new RMAItemRepository(c);
        }

        protected override Func<hcc_RMA, bool> MatchItems(RMA item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return rma => rma.bvin == guid;
        }

        protected override Func<hcc_RMA, bool> NotMatchItems(List<RMA> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return rma => !itemGuids.Contains(rma.bvin);
        }

        protected override void CopyModelToData(hcc_RMA data, RMA model)
        {
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Comments = model.Comments;
            data.DateOfReturn = model.DateOfReturnUtc;
            data.EmailAddress = model.EmailAddress;
            data.LastUpdated = model.LastUpdatedUtc;
            data.Name = model.Name;
            data.Number = model.Number;
            data.OrderBvin = DataTypeHelper.BvinToGuid(model.OrderBvin);
            data.PhoneNumber = model.PhoneNumber;
            data.Status = (int) model.Status;
            data.StoreId = model.StoreId;
        }

        protected override void CopyDataToModel(hcc_RMA data, RMA model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.Comments = data.Comments;
            model.DateOfReturnUtc = data.DateOfReturn;
            model.EmailAddress = data.EmailAddress;
            model.LastUpdatedUtc = data.LastUpdated;
            model.Name = data.Name;
            model.Number = data.Number;
            model.OrderBvin = DataTypeHelper.GuidToBvin(data.OrderBvin);
            model.PhoneNumber = data.PhoneNumber;
            model.Status = (RMAStatus) data.Status;
            model.StoreId = data.StoreId;
        }

        protected override void GetSubItems(List<RMA> models)
        {
            var bvinIds = models.Select(s => s.Bvin).ToList();
            var items = itemRepository.FindForRMA(bvinIds);

            foreach (var model in models)
            {
                var item = items.Where(s => s.Bvin == model.Bvin).ToList();
                model.Items = item;
            }
        }

        protected override void MergeSubItems(RMA model)
        {
            itemRepository.MergeList(model.Bvin, model.Items);
        }

        public RMA Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        public override bool Create(RMA item)
        {
            if (item.Bvin.Trim().Length < 1)
                item.Bvin = Guid.NewGuid().ToString();
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(RMA item)
        {
            var rmaId = DataTypeHelper.BvinToGuid(item.Bvin);
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.bvin == rmaId);
        }

        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        public List<RMA> FindForOrders(List<string> orderBvins)
        {
            var orderGuids = orderBvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => orderGuids.Contains(y.OrderBvin))
                    .OrderBy(y => y.OrderBvin)
                    .ThenBy(y => y.DateOfReturn);
            });
        }

        public List<RMA> FindForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OrderBvin == orderGuid)
                    .OrderBy(y => y.DateOfReturn);
            });
        }

        public void DeleteForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            Delete(y => y.OrderBvin == orderGuid);
        }

        public void MergeList(string orderBvin, long storeId, List<RMA> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.OrderBvin = orderBvin;
                item.StoreId = storeId;
                item.LastUpdatedUtc = DateTime.UtcNow;
            }

            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            MergeList(subitems, rma => rma.OrderBvin == orderGuid && rma.StoreId == storeId);
        }

        public bool DeleteItem(string rmaItemBvin)
        {
            return itemRepository.Delete(rmaItemBvin);
        }

        public RMAItem RMAItemFind(string bvin)
        {
            return itemRepository.Find(bvin);
        }

        public bool RMAItemUpdate(RMAItem item)
        {
            return itemRepository.Update(item);
        }

        public List<RMA> FindMany(List<string> bvins)
        {
            var storeId = Context.CurrentStore.Id;
            var guids = bvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => guids.Contains(y.bvin))
                    .Where(y => y.StoreId == storeId).OrderBy(y => y.LastUpdated);
            });
        }

        public List<RMA> FindByStatusPaged(int pageNumber, int pageSize, ref int totalItems, RMAStatus status)
        {
            using (var s = CreateStrategy())
            {
                var query = s.GetQuery();

                if (status != RMAStatus.None)
                {
                    query = query.Where(y => y.Status == (int) status);
                }

                totalItems = query.Count();
                var items = GetPagedItems(query.OrderByDescending(y => y.LastUpdated), pageNumber, pageSize);

                return ListPoco(items);
            }
        }
    }
}