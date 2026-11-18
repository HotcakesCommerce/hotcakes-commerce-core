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

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    /// Represents the RMARepository order-domain type and preserves the established public compatibility surface.
    /// </summary>
    public class RMARepository : HccSimpleRepoBase<hcc_RMA, RMA>
    {
        private readonly RMAItemRepository itemRepository;

        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
        public RMARepository(HccRequestContext c)
            : base(c)
        {
            itemRepository = new RMAItemRepository(c);
        }

        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
        protected override Func<hcc_RMA, bool> MatchItems(RMA item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return rma => rma.bvin == guid;
        }

        /// <summary>
        /// Provides the member operation while preserving the established order-domain behavior.
        /// </summary>
        protected override Func<hcc_RMA, bool> NotMatchItems(List<RMA> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return rma => !itemGuids.Contains(rma.bvin);
        }

        /// <summary>
        /// Provides the CopyModelToData operation while preserving the established order-domain behavior.
        /// </summary>
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

        /// <summary>
        /// Provides the CopyDataToModel operation while preserving the established order-domain behavior.
        /// </summary>
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

        /// <summary>
        /// Provides the GetSubItems operation while preserving the established order-domain behavior.
        /// </summary>
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

        /// <summary>
        /// Provides the MergeSubItems operation while preserving the established order-domain behavior.
        /// </summary>
        protected override void MergeSubItems(RMA model)
        {
            itemRepository.MergeList(model.Bvin, model.Items);
        }

        /// <summary>
        /// Provides the Find operation while preserving the established order-domain behavior.
        /// </summary>
        public RMA Find(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        /// <summary>
        /// Provides the Create operation while preserving the established order-domain behavior.
        /// </summary>
        public override bool Create(RMA item)
        {
            if (item.Bvin.Trim().Length < 1)
                item.Bvin = Guid.NewGuid().ToString();
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>
        /// Provides the Update operation while preserving the established order-domain behavior.
        /// </summary>
        public bool Update(RMA item)
        {
            var rmaId = DataTypeHelper.BvinToGuid(item.Bvin);
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.bvin == rmaId);
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

        /// <summary>
        /// Finds the requested records within the established repository store scope and ordering.
        /// </summary>
        public List<RMA> FindForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OrderBvin == orderGuid)
                    .OrderBy(y => y.DateOfReturn);
            });
        }

        /// <summary>
        /// Deletes all records associated with the specified parent identifier.
        /// </summary>
        public void DeleteForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            Delete(y => y.OrderBvin == orderGuid);
        }

        /// <summary>
        /// Merges the supplied records after assigning their established parent and store scope.
        /// </summary>
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

        /// <summary>
        /// Provides the DeleteItem operation while preserving the established order-domain behavior.
        /// </summary>
        public bool DeleteItem(string rmaItemBvin)
        {
            return itemRepository.Delete(rmaItemBvin);
        }

        /// <summary>
        /// Provides the RMAItemFind operation while preserving the established order-domain behavior.
        /// </summary>
        public RMAItem RMAItemFind(string bvin)
        {
            return itemRepository.Find(bvin);
        }

        /// <summary>
        /// Provides the RMAItemUpdate operation while preserving the established order-domain behavior.
        /// </summary>
        public bool RMAItemUpdate(RMAItem item)
        {
            return itemRepository.Update(item);
        }

        /// <summary>
        /// Provides the FindMany operation while preserving the established order-domain behavior.
        /// </summary>
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

        /// <summary>
        /// Returns a status-filtered page and reports the unpaged count through the reference parameter.
        /// </summary>
        public List<RMA> FindByStatusPaged(int pageNumber, int pageSize, ref int totalItems, RMAStatus status)
        {
            using (var s = CreateReadStrategy())
            {
                var query = s.GetQuery().AsNoTracking();

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
