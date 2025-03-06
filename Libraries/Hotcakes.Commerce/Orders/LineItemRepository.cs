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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Payment;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Orders
{
    public class LineItemRepository : HccSimpleRepoBase<hcc_LineItem, LineItem>
    {
        private const string MARKEDFREESHIPPING = "ismarkedforfreeshipping";
        private const string ISTAXEXEMPT = "istaxexempt";
        private const string FREESHIPPINGIDS = "freeshippingmethodsids";

        public LineItemRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_LineItem, bool> MatchItems(LineItem item)
        {
            return li => li.Id == item.Id;
        }

        protected override Func<hcc_LineItem, bool> NotMatchItems(List<LineItem> items)
        {
            var itemIds = items.Select(i => i.Id).ToList();
            return li => !itemIds.Contains(li.Id);
        }

        protected override void CopyModelToData(hcc_LineItem data, LineItem model)
        {
            data.Id = model.Id;
            data.IsUserSuppliedPrice = model.IsUserSuppliedPrice;
            data.IsGiftCard = model.IsGiftCard;
            data.BasePrice = model.BasePricePerItem;
            data.AdjustedPrice = model.AdjustedPricePerItem;
            model.CustomPropertySet(Constants.HCC_KEY, MARKEDFREESHIPPING, model.IsMarkedForFreeShipping.ToString());
            model.CustomPropertySet(Constants.HCC_KEY, ISTAXEXEMPT, model.IsTaxExempt);
            model.CustomPropertySet(Constants.HCC_KEY, FREESHIPPINGIDS,
                string.Join(",", model.FreeShippingMethodIds.ToArray()));
            data.CustomProperties = model.CustomPropertiesToXml();
            data.DiscountDetails = DiscountDetail.ListToXml(model.DiscountDetails);
            data.LastUpdated = model.LastUpdatedUtc;
            data.LineTotal = model.LineTotal;
            data.OrderBvin = DataTypeHelper.BvinToGuid(model.OrderBvin);
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
            data.ProductName = model.ProductName;
            data.ProductShippingHeight = model.ProductShippingHeight;
            data.ProductShippingLength = model.ProductShippingLength;
            data.ProductShippingWeight = model.ProductShippingWeight;
            data.ProductShippingWidth = model.ProductShippingWidth;
            data.ProductShortDescription = model.ProductShortDescription;
            data.ProductSku = model.ProductSku;
            data.Quantity = model.Quantity;
            data.QuantityReturned = model.QuantityReturned;
            data.QuantityShipped = model.QuantityShipped;
            data.SelectionData = model.SelectionData.SerializeToXml();
            data.ShipSeparately = model.ShipSeparately;
            data.ShippingPortion = model.ShippingPortion;
            data.IsNonShipping = model.IsNonShipping ? 1 : 0;
            data.StatusCode = model.StatusCode;
            data.StatusName = model.StatusName;
            data.StoreId = model.StoreId;
            data.TaxRate = model.TaxRate;
            data.ShippingTaxRate = model.ShippingTaxRate;
            data.TaxPortion = model.TaxPortion;
            data.TaxScheduleId = model.TaxSchedule;
            data.VariantId = model.VariantId;
            data.ShipFromAddress = model.ShipFromAddress.ToXml(true);
            data.ShipFromMode = (int) model.ShipFromMode;
            data.ShipFromNotificationId = model.ShipFromNotificationId;
            data.ExtraShipCharge = model.ExtraShipCharge;
            data.ShippingCharge = (int) model.ShippingCharge;
            data.IsBundle = model.IsBundle;
            data.QuantityReserved = model.QuantityReserved;
            data.IsRecurring = model.IsRecurring;
            data.RecurringInterval = model.RecurringBilling.Interval;
            data.RecurringIntervalType = (int) model.RecurringBilling.IntervalType;
            data.IsRecurringCancelled = model.RecurringBilling.IsCancelled;
            data.PromotionIds = model.PromotionIds;
            data.FreeQuantity = model.FreeQuantity;
            data.IsUpchargeAllowed = model.IsUpchargeAllowed;
        }

        protected override void CopyDataToModel(hcc_LineItem data, LineItem model)
        {
            model.Id = data.Id;
            model.IsUserSuppliedPrice = data.IsUserSuppliedPrice;
            model.IsGiftCard = data.IsGiftCard;
            model.BasePricePerItem = data.BasePrice;
            model.AdjustedPricePerItem = data.AdjustedPrice;
            model.CustomPropertiesFromXml(data.CustomProperties);
            model.DiscountDetails = DiscountDetail.ListFromXml(data.DiscountDetails);
            model.LastUpdatedUtc = data.LastUpdated;
            model.LineTotal = data.LineTotal;
            model.OrderBvin = DataTypeHelper.GuidToBvin(data.OrderBvin);
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
            model.ProductName = data.ProductName;
            model.ProductShippingHeight = data.ProductShippingHeight;
            model.ProductShippingLength = data.ProductShippingLength;
            model.ProductShippingWeight = data.ProductShippingWeight;
            model.ProductShippingWidth = data.ProductShippingWidth;
            model.ProductShortDescription = data.ProductShortDescription;
            model.ProductSku = data.ProductSku;
            model.Quantity = data.Quantity;
            model.QuantityReturned = data.QuantityReturned;
            model.QuantityShipped = data.QuantityShipped;
            model.SelectionData.DeserializeFromXml(data.SelectionData);
            model.ShippingPortion = data.ShippingPortion;
            model.IsNonShipping = data.IsNonShipping == 1 ? true : false;
            model.StatusCode = data.StatusCode;
            model.StatusName = data.StatusName;
            model.StoreId = data.StoreId;
            model.TaxRate = data.TaxRate;
            model.ShippingTaxRate = data.ShippingTaxRate;
            model.TaxPortion = data.TaxPortion;
            model.TaxSchedule = data.TaxScheduleId;
            model.VariantId = data.VariantId;
            model.ShipFromAddress.FromXmlString(data.ShipFromAddress);
            model.ShipFromMode = (ShippingMode) data.ShipFromMode;
            model.ShipFromNotificationId = data.ShipFromNotificationId;
            model.ExtraShipCharge = data.ExtraShipCharge;
            model.ShippingCharge = (ShippingChargeType) data.ShippingCharge;
            model.ShipSeparately = data.ShipSeparately;
            model.IsBundle = data.IsBundle;
            model.QuantityReserved = data.QuantityReserved;
            model.IsRecurring = data.IsRecurring;
            model.RecurringBilling.Interval = data.RecurringInterval ?? 0;
            model.RecurringBilling.IntervalType = (RecurringIntervalType) (data.RecurringIntervalType ?? 0);
            model.RecurringBilling.IsCancelled = data.IsRecurringCancelled;
            model.PromotionIds = data.PromotionIds;
            model.FreeQuantity = data.FreeQuantity;
            model.IsUpchargeAllowed = data.IsUpchargeAllowed;

            if (model.CustomPropertyGet(Constants.HCC_KEY, MARKEDFREESHIPPING) == true.ToString())
            {
                model.IsMarkedForFreeShipping = true;
            }
            else
            {
                model.IsMarkedForFreeShipping = false;
            }
            model.IsTaxExempt = model.CustomPropertyGetAsBool(Constants.HCC_KEY, ISTAXEXEMPT);

            // Free Shipping Method Ids
            var freeshippingids = model.CustomPropertyGet(Constants.HCC_KEY, FREESHIPPINGIDS);
            if (freeshippingids.Trim().Length > 0)
            {
                var methods = freeshippingids.Split(',');
                foreach (var methodId in methods)
                {
                    //Always add method id as upper invariant to avoid issue on comparision
                    model.FreeShippingMethodIds.Add(methodId.ToUpperInvariant());
                }
            }
        }

        public bool Update(LineItem item)
        {
            item.LastUpdatedUtc = DateTime.UtcNow;
            return base.Update(item, y => y.Id == item.Id);
        }

        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        public List<LineItem> FindForOrder(string orderBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(orderBvin);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OrderBvin == guid)
                    .OrderBy(y => y.Id);
            });
        }

        public List<LineItem> FindForOrders(List<string> bvins)
        {
            var guids = bvins.Select(b => DataTypeHelper.BvinToGuid(b)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => guids.Contains(y.OrderBvin))
                    .OrderBy(y => y.OrderBvin)
                    .ThenBy(y => y.Id);
            });
        }

        public void DeleteForOrder(string orderBvin)
        {
            var guid = DataTypeHelper.BvinToGuid(orderBvin);
            Delete(y => y.OrderBvin == guid);
        }

        public void MergeList(string orderBvin, long storeId, List<LineItem> subitems)
        {
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.OrderBvin = orderBvin;
                item.StoreId = storeId;
                item.LastUpdatedUtc = DateTime.UtcNow;
            }

            var orderGuid = DataTypeHelper.BvinToGuid(orderBvin);
            MergeList(subitems, o => o.OrderBvin == orderGuid && o.StoreId == storeId);
        }

        public Dictionary<string, int> FindPopularItems(DateTime startDateUtc, DateTime endDateUtc, int maxItems)
        {
            var result = new Dictionary<string, int>();
            var storeId = Context.CurrentStore.Id;

            using (var s = CreateReadStrategy())
            {
                var query = from lineitems in s.GetQuery()
                    where lineitems.StoreId == storeId &&
                          lineitems.hcc_Order.TimeOfOrder >= startDateUtc &&
                          lineitems.hcc_Order.TimeOfOrder <= endDateUtc
                    group lineitems by lineitems.ProductId
                    into groupedItems
                    orderby groupedItems.Sum(y => y.Quantity) descending
                    select new {ProductId = groupedItems.Key, Quantity = groupedItems.Sum(y => y.Quantity)};

                var query2 = query.Take(maxItems).AsNoTracking().ToList();
                foreach (var popular in query2)
                {
                    var productId = DataTypeHelper.GuidToBvin(popular.ProductId);
                    result.Add(productId, popular.Quantity);
                }
            }

            return result;
        }
    }
}