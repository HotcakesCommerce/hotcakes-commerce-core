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
using Hotcakes.Shipping;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Orders
{
    public class OrderPackageRepository : HccSimpleRepoBase<hcc_OrderPackage, OrderPackage>
    {
        public OrderPackageRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_OrderPackage, bool> MatchItems(OrderPackage item)
        {
            return op => op.Id == item.Id;
        }

        protected override Func<hcc_OrderPackage, bool> NotMatchItems(List<OrderPackage> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return op => !itemIds.Contains(op.Id);
        }

        protected override void CopyModelToData(hcc_OrderPackage data, OrderPackage model)
        {
            data.CustomProperties = model.CustomProperties.ToXml();
            data.Description = model.Description;
            data.EstimatedShippingCost = model.EstimatedShippingCost;
            data.HasShipped = model.HasShipped ? 1 : 0;
            data.Height = model.Height;
            data.Id = model.Id;
            data.Items = model.ItemsToXml();
            data.LastUpdatedUtc = model.LastUpdatedUtc;
            data.Length = model.Length;
            data.OrderId = DataTypeHelper.BvinToGuid(model.OrderId);
            data.ShipDateUtc = model.ShipDateUtc;
            data.ShippingProviderId = model.ShippingProviderId;
            data.ShippingProviderServiceCode = model.ShippingProviderServiceCode;
            data.SizeUnits = (int) model.SizeUnits;
            data.StoreId = model.StoreId;
            data.TrackingNumber = model.TrackingNumber;
            data.Weight = model.Weight;
            data.WeightUnits = (int) model.WeightUnits;
            data.Width = model.Width;
            data.ShippingMethodId = model.ShippingMethodId;
        }

        protected override void CopyDataToModel(hcc_OrderPackage data, OrderPackage model)
        {
            model.CustomProperties = CustomPropertyCollection.FromXml(data.CustomProperties);
            model.Description = data.Description;
            model.EstimatedShippingCost = data.EstimatedShippingCost;
            model.HasShipped = data.HasShipped == 1;
            model.Height = data.Height;
            model.Id = data.Id;
            model.ItemsFromXml(data.Items);
            model.LastUpdatedUtc = data.LastUpdatedUtc;
            model.Length = data.Length;
            model.OrderId = DataTypeHelper.GuidToBvin(data.OrderId);
            model.ShipDateUtc = data.ShipDateUtc;
            model.ShippingProviderId = data.ShippingProviderId;
            model.ShippingProviderServiceCode = data.ShippingProviderServiceCode;
            model.SizeUnits = (LengthType) data.SizeUnits;
            model.StoreId = data.StoreId;
            model.TrackingNumber = data.TrackingNumber;
            model.Weight = data.Weight;
            model.WeightUnits = (WeightType) data.WeightUnits;
            model.Width = data.Width;
            model.ShippingMethodId = data.ShippingMethodId;
        }

        public bool Update(OrderPackage item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<OrderPackage> FindForOrders(List<string> orderBvins)
        {
            var orderGuids = orderBvins.Select(bvin => DataTypeHelper.BvinToGuid(bvin)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => orderGuids.Contains(y.OrderId))
                    .OrderBy(y => y.OrderId)
                    .ThenBy(y => y.Id);
            });
        }

        public List<OrderPackage> FindForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OrderId == orderGuid)
                    .OrderBy(y => y.Id);
            });
        }

        public void DeleteForOrder(string orderBvin)
        {
            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            Delete(y => y.OrderId == orderGuid);
        }

        public void MergeList(string orderBvin, long storeId, List<OrderPackage> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.OrderId = orderBvin;
                item.StoreId = storeId;
                item.LastUpdatedUtc = DateTime.UtcNow;
            }

            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            MergeList(subitems, op => op.OrderId == orderGuid && op.StoreId == storeId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static OrderPackageRepository InstantiateForMemory(HccRequestContext c)
        {
            return new OrderPackageRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static OrderPackageRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new OrderPackageRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public OrderPackageRepository(IRepositoryStrategy<hcc_OrderPackage> strategy, ILogger log)
            : this(HccRequestContext.Current)
        {
        }

        #endregion
    }
}