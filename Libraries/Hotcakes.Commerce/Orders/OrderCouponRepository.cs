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

namespace Hotcakes.Commerce.Orders
{
    public class OrderCouponRepository : HccSimpleRepoBase<hcc_OrderCoupon, OrderCoupon>
    {
        public OrderCouponRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_OrderCoupon, bool> MatchItems(OrderCoupon item)
        {
            return oc => oc.Id == item.Id;
        }

        protected override Func<hcc_OrderCoupon, bool> NotMatchItems(List<OrderCoupon> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return oc => !itemIds.Contains(oc.Id);
        }

        protected override void CopyModelToData(hcc_OrderCoupon data, OrderCoupon model)
        {
            data.Id = model.Id;
            data.CouponCode = model.CouponCode;
            data.IsUsed = model.IsUsed;
            data.LastUpdatedUtc = model.LastUpdatedUtc;
            data.OrderBvin = DataTypeHelper.BvinToGuid(model.OrderBvin);
            data.StoreId = model.StoreId;
            data.UserId = model.UserId;
        }

        protected override void CopyDataToModel(hcc_OrderCoupon data, OrderCoupon model)
        {
            model.Id = data.Id;
            model.CouponCode = data.CouponCode;
            model.IsUsed = data.IsUsed;
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.OrderBvin = DataTypeHelper.GuidToBvin(data.OrderBvin);
            model.StoreId = data.StoreId;
            model.UserId = data.UserId;
        }

        public bool Update(OrderCoupon item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<OrderCoupon> FindForOrders(List<string> orderBvins)
        {
            var orderGuids = orderBvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => orderGuids.Contains(y.OrderBvin))
                    .OrderBy(y => y.OrderBvin)
                    .ThenBy(y => y.Id);
            });
        }

        public List<OrderCoupon> FindForOrder(string orderBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(orderBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OrderBvin == guid)
                    .OrderBy(y => y.Id);
            });
        }

        public void DeleteForOrder(string orderBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(orderBvin);
            Delete(y => y.OrderBvin == guid);
        }

        public void MergeList(string orderBvin, long storeId, List<OrderCoupon> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.OrderBvin = orderBvin;
                item.StoreId = storeId;
                item.LastUpdatedUtc = DateTime.UtcNow;
            }

            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            MergeList(subitems, oc => oc.OrderBvin == orderGuid && oc.StoreId == storeId);
        }

        public int GetUseTimesByUserId(string couponCode, string UserId, long storeId)
        {
            using (var s = CreateStrategy())
            {
                return s.GetQuery().Where(y => y.StoreId == storeId)
                    .Where(y => y.CouponCode == couponCode)
                    .Where(y => y.UserId == UserId)
                    .Count();
            }
        }

        public int GetUseTimesForStore(string couponCode, long storeId)
        {
            using (var s = CreateStrategy())
            {
                return s.GetQuery().Where(y => y.StoreId == storeId)
                    .Where(y => y.CouponCode == couponCode)
                    .Count();
            }
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static OrderCouponRepository InstantiateForMemory(HccRequestContext c)
        {
            return new OrderCouponRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static OrderCouponRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new OrderCouponRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public OrderCouponRepository(IRepositoryStrategy<hcc_OrderCoupon> strategy, ILogger log)
            : this(HccRequestContext.Current)
        {
        }

        #endregion
    }
}