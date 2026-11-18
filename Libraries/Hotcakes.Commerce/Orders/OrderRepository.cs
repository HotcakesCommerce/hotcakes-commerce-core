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
using Hotcakes.Commerce.Marketing;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     Provides persistence and query operations for <see cref="Order"/> records and their related order entities.
    /// </summary>
    /// <remarks>
    ///     Entity mapping preserves the repository's GUID/bvin conversion, serialized address and custom-property storage,
    ///     child-collection merging, and current-store filtering conventions.
    /// </remarks>
        [Serializable]
    public class OrderRepository : HccSimpleRepoBase<hcc_Order, Order>
    {
        /// <summary>
        ///     Initializes the order repository and related child repositories using the supplied request context.
        /// </summary>
        /// <param name="context">The supplied context value.</param>
        public OrderRepository(HccRequestContext context)
            : base(context)
        {
            ItemRepository = Factory.CreateRepo<LineItemRepository>(context);
            NotesRepository = Factory.CreateRepo<OrderNoteRepository>(context);
            CouponRepository = Factory.CreateRepo<OrderCouponRepository>(context);
            PackageRepository = Factory.CreateRepo<OrderPackageRepository>(context);
            ReturnsRepository = Factory.CreateRepo<RMARepository>(context);
        }

        private LineItemRepository ItemRepository { get; set; }
        private OrderNoteRepository NotesRepository { get; set; }
        private OrderCouponRepository CouponRepository { get; set; }
        private OrderPackageRepository PackageRepository { get; set; }
        private RMARepository ReturnsRepository { get; set; }

        /// <summary>
        ///     Performs the copy data to model operation using the supplied arguments and current commerce context.
        ///     Serialized addresses, custom properties, discounts, status values, and totals are mapped into the model.
        /// </summary>
        /// <param name="data">The supplied data value.</param>
        /// <param name="model">The supplied model value.</param>
        protected override void CopyDataToModel(hcc_Order data, Order model)
        {
            model.AffiliateID = data.AffiliateId;
            model.BillingAddress.FromXmlString(data.BillingAddress);
            model.bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.CustomProperties = CustomPropertyCollection.FromXml(data.CustomProperties);
            model.FraudScore = data.FraudScore;
            model.TotalHandling = data.HandlingTotal;
            model.Id = data.Id;
            model.Instructions = data.Instructions;
            model.IsPlaced = data.IsPlaced == 1;
            model.LastUpdatedUtc = data.LastUpdated;
            model.OrderDiscountDetails = DiscountDetail.ListFromXml(data.OrderDiscountDetails);
            model.OrderNumber = data.OrderNumber;
            model.PaymentStatus = (OrderPaymentStatus)data.PaymentStatus;
            model.ShippingDiscountDetails = DiscountDetail.ListFromXml(data.ShippingDiscountDetails);
            model.ShippingAddress.FromXmlString(data.ShippingAddress);
            model.ShippingMethodDisplayName = data.ShippingMethodDisplayName;
            model.ShippingMethodId = data.ShippingMethodId;
            model.ShippingProviderId = data.ShippingProviderId;
            model.ShippingProviderServiceCode = data.ShippingProviderServiceCode;
            model.ShippingStatus = (OrderShippingStatus)data.ShippingStatus;
            model.TotalShippingBeforeDiscounts = data.ShippingTotal;
            model.TotalShippingAfterDiscounts = data.AdjustedShippingTotal;
            model.StatusCode = data.StatusCode;
            model.StatusName = data.StatusName;
            model.StoreId = data.StoreId;
            model.ItemsTax = data.ItemsTax;
            model.ShippingTax = data.ShippingTax;
            model.ShippingTaxRate = data.ShippingTaxRate;
            model.TotalTax = data.TaxTotal;
            model.ThirdPartyOrderId = data.ThirdPartyOrderId;
            model.TimeOfOrderUtc = data.TimeOfOrder;
            model.UserEmail = data.UserEmail;
            model.UserID = data.UserId;
            model.UserDeviceType = (DeviceType)data.UserDeviceType;
            model.IsAbandonedEmailSent = data.IsAbandonedEmailSent;
            model.UsedCulture = data.UsedCulture;
        }

        /// <summary>
        ///     Performs the copy model to data operation using the supplied arguments and current commerce context.
        ///     Addresses, custom properties, discounts, status values, and totals are mapped into the persistence record.
        /// </summary>
        /// <param name="data">The supplied data value.</param>
        /// <param name="model">The supplied model value.</param>
        protected override void CopyModelToData(hcc_Order data, Order model)
        {
            data.AffiliateId = model.AffiliateID;
            data.BillingAddress = model.BillingAddress.ToXml(true);
            data.bvin = DataTypeHelper.BvinToGuid(model.bvin);
            data.CustomProperties = model.CustomProperties.ToXml();
            data.FraudScore = model.FraudScore;
            data.OrderDiscountDetails = DiscountDetail.ListToXml(model.OrderDiscountDetails);
            data.OrderDiscounts = model.TotalOrderDiscounts;
            data.SubTotal = model.TotalOrderBeforeDiscounts;
            data.GrandTotal = model.TotalGrand;
            data.HandlingTotal = model.TotalHandling;
            data.Id = model.Id;
            data.Instructions = model.Instructions;
            data.IsPlaced = model.IsPlaced ? 1 : 0;
            data.LastUpdated = model.LastUpdatedUtc;
            data.OrderNumber = model.OrderNumber;
            data.PaymentStatus = (int)model.PaymentStatus;
            data.ShippingAddress = model.ShippingAddress.ToXml(true);
            data.ShippingDiscounts = model.TotalShippingDiscounts;
            data.ShippingDiscountDetails = DiscountDetail.ListToXml(model.ShippingDiscountDetails);
            data.ShippingMethodDisplayName = model.ShippingMethodDisplayName;
            data.ShippingMethodId = model.ShippingMethodId;
            data.ShippingProviderId = model.ShippingProviderId;
            data.ShippingProviderServiceCode = model.ShippingProviderServiceCode;
            data.ShippingStatus = (int)model.ShippingStatus;
            data.ShippingTotal = model.TotalShippingBeforeDiscounts;
            data.AdjustedShippingTotal = model.TotalShippingAfterDiscounts;
            data.StatusCode = model.StatusCode;
            data.StatusName = model.StatusName;
            data.StoreId = model.StoreId;
            data.ItemsTax = model.ItemsTax;
            data.ShippingTaxRate = model.ShippingTaxRate;
            data.ShippingTax = model.ShippingTax;
            data.TaxTotal = model.TotalTax;
            data.ThirdPartyOrderId = model.ThirdPartyOrderId;
            data.TimeOfOrder = model.TimeOfOrderUtc;
            data.UserEmail = model.UserEmail;
            data.UserId = model.UserID;
            data.UserDeviceType = (int)model.UserDeviceType;
            data.IsAbandonedEmailSent = model.IsAbandonedEmailSent;
            data.IsRecurring = model.IsRecurring;
            data.UsedCulture = model.UsedCulture;
        }

        /// <summary>
        ///     Performs the get sub items operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="models">The supplied models value.</param>
        protected override void GetSubItems(List<Order> models)
        {
            var orderIds = models.Select(o => o.bvin).ToList();
            var allItems = ItemRepository.FindForOrders(orderIds);
            var allNotes = NotesRepository.FindForOrders(orderIds);
            var allCoupons = CouponRepository.FindForOrders(orderIds);
            var allPackages = PackageRepository.FindForOrders(orderIds);
            var allReturns = ReturnsRepository.FindForOrders(orderIds);

            foreach (var model in models)
            {
                model.Items = allItems.Where(i => i.OrderBvin == model.bvin).ToList();
                model.Notes = allNotes.Where(i => i.OrderID == model.bvin).ToList();
                model.Coupons = allCoupons.Where(i => i.OrderBvin == model.bvin).ToList();
                model.Packages = allPackages.Where(i => i.OrderId == model.bvin).ToList();
                model.Returns = allReturns.Where(i => i.OrderBvin == model.bvin).ToList();
            }
        }

        /// <summary>
        ///     Performs the merge sub items operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="model">The supplied model value.</param>
        protected override void MergeSubItems(Order model)
        {
            ItemRepository.MergeList(model.bvin, model.StoreId, model.Items);
            NotesRepository.MergeList(model.bvin, model.StoreId, model.Notes);
            CouponRepository.MergeList(model.bvin, model.StoreId, model.Coupons);
            PackageRepository.MergeList(model.bvin, model.StoreId, model.Packages);
            ReturnsRepository.MergeList(model.bvin, model.StoreId, model.Returns);
        }

        /// <summary>
        ///     Performs the find for current store operation using the supplied arguments and current commerce context.
        ///     The result is restricted to the current store.
        /// </summary>
        /// <param name="bvin">The supplied bvin value.</param>
        /// <returns>The value produced by the operation.</returns>
        public Order FindForCurrentStore(string bvin)
        {
            var result = FindForAllStores(bvin);
            return result?.StoreId == Context.CurrentStore.Id ? result : null;
        }

        /// <summary>
        ///     Performs the find for all stores operation using the supplied arguments and current commerce context.
        ///     The query is not restricted to the current store.
        /// </summary>
        /// <param name="bvin">The supplied bvin value.</param>
        /// <returns>The value produced by the operation.</returns>
        public Order FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(o => o.bvin == guid);
        }

        /// <summary>
        ///     Performs the find by third party order id operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="thirdPartyOrderId">The supplied third party order id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public Order FindByThirdPartyOrderId(string thirdPartyOrderId)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(o => o.ThirdPartyOrderId == thirdPartyOrderId && o.StoreId == storeId);
        }

        /// <summary>
        ///     Performs the find by order number operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="orderNumber">The supplied order number value.</param>
        /// <returns>The value produced by the operation.</returns>
        public Order FindByOrderNumber(string orderNumber)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(o => o.OrderNumber == orderNumber && o.StoreId == storeId);
        }

        /// <summary>
        ///     Performs the find many operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="bvins">The supplied bvins value.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<Order> FindMany(List<string> bvins)
        {
            var storeId = Context.CurrentStore.Id;
            var guids = bvins.Select(b => DataTypeHelper.BvinToGuid(b)).ToList();

            return FindListPoco(q =>
            {
                return q.Where(y => guids.Contains(y.bvin))
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Id);
            });
        }

        /// <summary>
        ///     Performs the create operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="item">The supplied item value.</param>
        /// <returns>The value produced by the operation.</returns>
        public override bool Create(Order item)
        {
            if (item == null) return false;

            item.LastUpdatedUtc = DateTime.UtcNow;
            if (string.IsNullOrEmpty(item.bvin))
            {
                item.bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.ApplyVATRules = Context.CurrentStore.Settings.ApplyVATRules;
            return base.Create(item);
        }

        /// <summary>
        ///     Performs the upsert operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="item">The supplied item value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool Upsert(Order item)
        {
            if (item == null) return false;
            if (string.IsNullOrEmpty(item.bvin))
            {
                return Create(item);
            }

            return Update(item);
        }

        /// <summary>
        ///     Performs the update operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="item">The supplied item value.</param>
        /// <param name="mergeSubItems">The supplied merge sub items value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool Update(Order item, bool mergeSubItems = true)
        {
            if (item == null) return false;
            if (item.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }

            item.LastUpdatedUtc = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(item.bvin);
            return Update(item, o => o.bvin == guid, mergeSubItems);
        }

        /// <summary>
        ///     Performs the delete operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="bvin">The supplied bvin value.</param>
        /// <returns>The value produced by the operation.</returns>
        public bool Delete(string bvin)
        {
            if (string.IsNullOrEmpty(bvin)) return false;
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(o => o.bvin == guid);
        }

        /// <summary>
        ///     Performs the list poco snapshot operation using the supplied arguments and current commerce context.
        ///     The projection contains stored totals and recurring-item state without loading mutable child collections.
        /// </summary>
        /// <param name="items">The supplied items value.</param>
        /// <returns>The value produced by the operation.</returns>
        protected virtual List<OrderSnapshot> ListPocoSnapshot(IQueryable<hcc_Order> items)
        {
            var result = new List<OrderSnapshot>();

            var extendedItems = items.Select(i => new { item = i, isRecurring = i.hcc_LineItem.Any(l => l.IsRecurring) });

            foreach (var extendedItem in extendedItems)
            {
                var item = extendedItem.item;
                var temp = new OrderSnapshot();

                temp.AffiliateID = item.AffiliateId;
                temp.BillingAddress.FromXmlString(item.BillingAddress);
                temp.bvin = DataTypeHelper.GuidToBvin(item.bvin);
                temp.CustomProperties = CustomPropertyCollection.FromXml(item.CustomProperties);
                temp.FraudScore = item.FraudScore;
                temp.TotalGrand = item.GrandTotal;
                temp.TotalHandling = item.HandlingTotal;
                temp.Id = item.Id;
                temp.Instructions = item.Instructions;
                temp.IsPlaced = item.IsPlaced == 1;
                temp.LastUpdatedUtc = item.LastUpdated;
                temp.TotalOrderDiscounts = item.OrderDiscounts;
                temp.OrderNumber = item.OrderNumber;
                temp.PaymentStatus = (OrderPaymentStatus)item.PaymentStatus;
                temp.TotalShippingDiscounts = item.ShippingDiscounts;
                temp.ShippingAddress.FromXmlString(item.ShippingAddress);
                temp.ShippingMethodDisplayName = item.ShippingMethodDisplayName;
                temp.ShippingMethodId = item.ShippingMethodId;
                temp.ShippingProviderId = item.ShippingProviderId;
                temp.ShippingProviderServiceCode = item.ShippingProviderServiceCode;
                temp.ShippingStatus = (OrderShippingStatus)item.ShippingStatus;
                temp.TotalShippingBeforeDiscounts = item.ShippingTotal;
                temp.StatusCode = item.StatusCode;
                temp.StatusName = item.StatusName;
                temp.StoreId = item.StoreId;
                temp.TotalOrderBeforeDiscounts = item.SubTotal;
                temp.TotalOrderAfterDiscounts = item.GrandTotal - item.ShippingTotal - item.TaxTotal;
                temp.TotalTax = item.TaxTotal;
                temp.ItemsTax = item.ItemsTax;
                temp.ShippingTax = item.ShippingTax;
                temp.ThirdPartyOrderId = item.ThirdPartyOrderId;
                temp.TimeOfOrderUtc = item.TimeOfOrder;
                temp.UserEmail = item.UserEmail;
                temp.UserID = item.UserId;
                temp.UsedCulture = item.UsedCulture;
                temp.IsRecurring = extendedItem.isRecurring;

                result.Add(temp);
            }

            return result;
        }

        /// <summary>
        ///     Performs the find all operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Id);

                return ListPocoSnapshot(query);
            }
        }

        /// <summary>
        ///     Performs the find all for all stores operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindAllForAllStores()
        {
            return FindAllPagedForAllStores(1, int.MaxValue);
        }

        /// <summary>
        ///     Performs the find all paged operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <returns>The value produced by the operation.</returns>
        public new List<OrderSnapshot> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;

            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Id);

                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);

                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the find all paged for all stores operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindAllPagedForAllStores(int pageNumber, int pageSize)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .OrderBy(y => y.Id);

                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);

                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the find many snapshots operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="bvins">The supplied bvins value.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindManySnapshots(List<string> bvins)
        {
            var storeId = Context.CurrentStore.Id;
            var guids = bvins.Select(b => DataTypeHelper.BvinToGuid(b)).ToList();

            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => guids.Contains(y.bvin))
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.Id);

                return ListPocoSnapshot(query);
            }
        }

        /// <summary>
        ///     Performs the find by criteria operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="criteria">The supplied criteria value.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindByCriteria(OrderSearchCriteria criteria)
        {
            var temp = -1;
            return FindByCriteriaPaged(criteria, 1, int.MaxValue, ref temp);
        }

        /// <summary>
        ///     Performs the get ready for payment paged operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="StartDate">The supplied start date value.</param>
        /// <param name="endDate">The supplied end date value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The maximum number of rows in the page.</param>
        /// <param name="rowCount">Receives the total number of matching rows.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> GetReadyForPaymentPaged(DateTime StartDate, DateTime endDate, int pageNumber,
            int pageSize, ref int rowCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.TimeOfOrder >= StartDate && y.TimeOfOrder <= endDate);

                // Is Placed
                query = query.Where(y => y.IsPlaced == 1);

                // Order is not Cancelled
                query = query.Where(y => y.StatusCode != OrderStatusCode.Cancelled);

                // Payment State is not Paid
                query = query.Where(y => y.PaymentStatus != (int)OrderPaymentStatus.Paid);

                // Payment State is not overpaid Paid
                query = query.Where(y => y.PaymentStatus != (int)OrderPaymentStatus.Overpaid);

                query = query.OrderByDescending(y => y.TimeOfOrder);

                // return total item count;
                rowCount = query.Count();

                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the get ready for shipping paged operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="StartDate">The supplied start date value.</param>
        /// <param name="endDate">The supplied end date value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The maximum number of rows in the page.</param>
        /// <param name="rowCount">Receives the total number of matching rows.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> GetReadyForShippingPaged(DateTime StartDate, DateTime endDate, int pageNumber,
            int pageSize, ref int rowCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.TimeOfOrder >= StartDate && y.TimeOfOrder <= endDate);

                // Is Placed
                query = query.Where(y => y.IsPlaced == 1);

                // Order is not Cancelled
                query = query.Where(y => y.StatusCode != OrderStatusCode.Cancelled);

                // Shipping State is not non shipping
                query = query.Where(y => y.ShippingStatus != (int)OrderShippingStatus.NonShipping);

                // Shipping State is not fully shipped
                query = query.Where(y => y.ShippingStatus != (int)OrderShippingStatus.FullyShipped);

                query = query.OrderByDescending(y => y.TimeOfOrder);

                // return total item count;
                rowCount = query.Count();


                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the find by criteria paged operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="criteria">The supplied criteria value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <param name="rowCount">Receives the total number of matching rows.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindByCriteriaPaged(OrderSearchCriteria criteria, int pageNumber, int pageSize,
            ref int rowCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.TimeOfOrder >= criteria.StartDateUtc && y.TimeOfOrder <= criteria.EndDateUtc);

                if (!criteria.IsIncludeCanceledOrder)
                {
                    // Order is not Cancelled
                    query = query.Where(y => y.StatusCode != OrderStatusCode.Cancelled);
                }

                // Order Number
                if (!string.IsNullOrEmpty(criteria.OrderNumber))
                {
                    query = query.Where(y => y.OrderNumber == criteria.OrderNumber);
                }

                // Is Recurring
                if (criteria.IsRecurring.HasValue)
                    query = query.Where(y => y.IsRecurring == criteria.IsRecurring.Value);

                // Status Code
                if (!string.IsNullOrEmpty(criteria.StatusCode))
                {
                    query = query.Where(y => y.StatusCode == criteria.StatusCode);
                }

                // Affiliate Id
                if (criteria.AffiliateId.HasValue)
                {
                    query = query.Where(y => y.AffiliateId == criteria.AffiliateId);
                }

                // Payment Status
                if (criteria.PaymentStatus != OrderPaymentStatus.Unknown)
                {
                    var tempPay = (int)criteria.PaymentStatus;
                    query = query.Where(y => y.PaymentStatus == tempPay);

                    //If payment is done then include orders even if they are placed or not
                    if (criteria.PaymentStatus == OrderPaymentStatus.Paid || criteria.PaymentStatus == OrderPaymentStatus.PartiallyPaid || criteria.PaymentStatus == OrderPaymentStatus.Overpaid)
                    {
                        criteria.IncludeUnplaced = true;
                    }
                }

                // Shipping Status
                if (criteria.ShippingStatus != OrderShippingStatus.Unknown)
                {
                    var tempShip = (int)criteria.ShippingStatus;
                    query = query.Where(y => y.ShippingStatus == tempShip);
                }

                // Is Placed
                if (!criteria.IncludeUnplaced)
                {
                    query = query.Where(y => y.IsPlaced == (criteria.IsPlaced ? 1 : 0) || string.IsNullOrEmpty(y.OrderNumber) == false);
                }

                // Keyword (most expensive operation)
                if (!string.IsNullOrEmpty(criteria.Keyword))
                {
                    var orderNumber = 0;
                    var firstChar = 0;
                    if (int.TryParse(criteria.Keyword, out orderNumber))
                    {
                        query = query.Where(y => y.OrderNumber.Contains(criteria.Keyword));
                    }
                    else if (criteria.Keyword.IndexOf("@") >= 0)
                    {
                        query = query.Where(y => y.UserEmail.Contains(criteria.Keyword));
                    }
                    else if (int.TryParse(criteria.Keyword.Substring(0, 1), out firstChar))
                    {
                        query = query.Where(y => y.BillingAddress.Contains(criteria.Keyword)
                                              || y.ShippingAddress.Contains(criteria.Keyword));
                    }
                    else
                    {
                        query = query.Where(y => y.UserEmail.Contains(criteria.Keyword)
                                              || y.BillingAddress.Contains(criteria.Keyword)
                                              || y.ShippingAddress.Contains(criteria.Keyword));
                    }

                    //
                    // TODO: 
                    // From Will:
                    // We need to populate the return results properly with all orders that partially match a product name.
                    // Probably need to add a new "LineItemSnapshot" property & class to OrderSnapshot to make this feasible.
                    // The code below was a POC that clearly won't work, and if fixed, might not be performant enough.
                    //

                    // get a collection of line items for the remaining filtered orders
                    //var lineItems = itemRepository.FindForOrders(items.Select(y => y.bvin).ToList());

                    // find all of the orders that contain the product name
                    //List<string> orderIds =
                    //  (from li in lineItems where li.ProductName.Contains(criteria.Keyword) select li.OrderBvin).Distinct()
                    //      .ToList();

                    // merge the results with the filtered order items
                    // (code never written)
                }

                // return total item count;
                rowCount = query.Count();

                if (criteria.SortDescending)
                    query = query.OrderByDescending(y => y.TimeOfOrder);
                else
                    query = query.OrderBy(y => y.TimeOfOrder);

                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the get orders ready for payment count operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="startDate">The supplied start date value.</param>
        /// <param name="endDate">The supplied end date value.</param>
        /// <returns>The value produced by the operation.</returns>
        public int GetOrdersReadyForPaymentCount(DateTime startDate, DateTime endDate)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.TimeOfOrder >= startDate && y.TimeOfOrder <= endDate);

                // Is Placed
                query = query.Where(y => y.IsPlaced == 1);

                // Order is not Cancelled
                query = query.Where(y => y.StatusCode != OrderStatusCode.Cancelled);

                // Payment State is not Paid
                query = query.Where(y => y.PaymentStatus != (int)OrderPaymentStatus.Paid);

                // Payment State is not overpaid
                query = query.Where(y => y.PaymentStatus != (int)OrderPaymentStatus.Overpaid);

                return query.Count();
            }
        }

        /// <summary>
        ///     Performs the get orders ready for shipping count operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="startDate">The supplied start date value.</param>
        /// <param name="endDate">The supplied end date value.</param>
        /// <returns>The value produced by the operation.</returns>
        public int GetOrdersReadyForShippingCount(DateTime startDate, DateTime endDate)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.TimeOfOrder >= startDate && y.TimeOfOrder <= endDate);

                // Is Placed
                query = query.Where(y => y.IsPlaced == 1);

                // Order is not Cancelled
                query = query.Where(y => y.StatusCode != OrderStatusCode.Cancelled);

                // Shipping State is not non shipping
                query = query.Where(y => y.ShippingStatus != (int)OrderShippingStatus.NonShipping);

                // Shipping State is not fully shipped
                query = query.Where(y => y.ShippingStatus != (int)OrderShippingStatus.FullyShipped);

                return query.Count();
            }
        }

        /// <summary>
        ///     Performs the count by criteria operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="criteria">The supplied criteria value.</param>
        /// <returns>The value produced by the operation.</returns>
        public int CountByCriteria(OrderSearchCriteria criteria)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.TimeOfOrder >= criteria.StartDateUtc && y.TimeOfOrder <= criteria.EndDateUtc);

                // Order Number
                if (criteria.OrderNumber != string.Empty)
                {
                    query = query.Where(y => y.OrderNumber == criteria.OrderNumber);
                }

                // Is Placed
                query = query.Where(y => y.IsPlaced == (criteria.IsPlaced ? 1 : 0));

                // Is Recurring
                if (criteria.IsRecurring.HasValue)
                    query = query.Where(y => y.IsRecurring == criteria.IsRecurring.Value);

                // Status Code
                if (!string.IsNullOrEmpty(criteria.StatusCode))
                {
                    query = query.Where(y => y.StatusCode == criteria.StatusCode);
                }

                // Affiliate Id
                if (criteria.AffiliateId.HasValue)
                {
                    query = query.Where(y => y.AffiliateId == criteria.AffiliateId);
                }

                // Payment Status
                if (criteria.PaymentStatus != OrderPaymentStatus.Unknown)
                {
                    var tempPay = (int)criteria.PaymentStatus;
                    query = query.Where(y => y.PaymentStatus == tempPay);
                }

                // Shipping Status
                if (criteria.ShippingStatus != OrderShippingStatus.Unknown)
                {
                    var tempShip = (int)criteria.ShippingStatus;
                    query = query.Where(y => y.ShippingStatus == tempShip);
                }

                // Keyword (most expensive operation)
                if (criteria.Keyword != string.Empty)
                {
                    query = query.Where(y => y.OrderNumber.Contains(criteria.Keyword)
                                          || y.UserEmail.Contains(criteria.Keyword)
                                          || y.BillingAddress.Contains(criteria.Keyword)
                                          || y.ShippingAddress.Contains(criteria.Keyword));
                }

                // return total item count;
                return query.Count();
            }
        }

        /// <summary>
        ///     Performs the find total spent by user operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="userId">The supplied user id value.</param>
        /// <param name="storeId">The supplied store id value.</param>
        /// <returns>The value produced by the operation.</returns>
        public decimal FindTotalSpentByUser(string userId, long storeId)
        {
            using (var strategy = CreateReadStrategy())
            {
                return strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.UserId == userId)
                    .Where(y => y.StoreId == storeId)
                    .Where(y => y.IsPlaced == 1)
                    .Sum(y => y.GrandTotal);
            }
        }

        /// <summary>
        ///     Performs the find by user id operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="userId">The supplied user id value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <param name="totalCount">The supplied total count value.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindByUserId(string userId, int pageNumber, int pageSize, ref int totalCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.UserId == userId)
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    //If order has number assigned then independent of its status or IsPlaced it should appear in list
                    .Where(y => y.OrderNumber.Trim().Length > 0)

                    .OrderByDescending(y => y.TimeOfOrder);


                totalCount = query.Count();

                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the find by id range operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="startId">The supplied start id value.</param>
        /// <param name="endId">The supplied end id value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <param name="totalCount">Receives the total number of matching rows.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<OrderSnapshot> FindByIdRange(int startId, int endId, int pageNumber, int pageSize,
            ref int totalCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
                    .Where(y => y.Id >= startId && y.Id <= endId)
                    .OrderBy(y => y.Id);

                totalCount = query.Count();

                var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
                return ListPocoSnapshot(pagedQuery);
            }
        }

        /// <summary>
        ///     Performs the find abandoned carts operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <returns>The value produced by the operation.</returns>
        public List<Order> FindAbandonedCarts()
        {
            var daysInterval = Context.CurrentStore.Settings.SendAbandonedEmailIn;
            var endTouchDate = DateTime.UtcNow.AddDays(-1 * daysInterval);

            return FindListPoco(q =>
            {
                return q.Where(o => o.StoreId == Context.CurrentStore.Id)
                        .Where(o => o.IsPlaced == 0)
                        .Where(o => o.hcc_LineItem.Any())
                        .Where(o => !string.IsNullOrEmpty(o.UserId))
                        .Where(o => !o.IsAbandonedEmailSent)
                        .Where(o => o.TimeOfOrder < endTouchDate);
            });
        }

        /// <summary>
        ///     Performs the find abandoned carts operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="startDate">The supplied start date value.</param>
        /// <param name="endDate">The supplied end date value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <param name="totalCount">Receives the total number of matching rows.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<Order> FindAbandonedCarts(DateTime startDate, DateTime endDate, int pageNumber, int pageSize,
            out int totalCount)
        {
            using (var strategy = CreateReadStrategy())
            {
                var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(o => o.StoreId == Context.CurrentStore.Id)
                    .Where(o => o.IsPlaced == 0)
                    .Where(o => o.hcc_LineItem.Any())
                    .Where(o => o.TimeOfOrder > startDate)
                    .Where(o => o.TimeOfOrder < endDate)
                    .OrderByDescending(o => o.TimeOfOrder);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize);
                return ListPoco(items);
            }
        }

        /// <summary>
        ///     Performs the find payment failure operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <returns>The value produced by the operation.</returns>
        public List<Order> FindPaymentFailure()
        {
            var endTouchDate = DateTime.UtcNow;

            return FindListPoco(q =>
            {
                return q.Where(o => o.StoreId == Context.CurrentStore.Id)
                        .Where(o => o.hcc_LineItem.Any())
                        .Where(o => !string.IsNullOrEmpty(o.UserId))
                        .Where(o => o.TimeOfOrder < endTouchDate)
                        .Where(o => o.hcc_OrderTransactions.Any(t => !t.Success))
                        .Where(o => o.PaymentStatus == (int)OrderPaymentStatus.Unknown || o.PaymentStatus == (int)OrderPaymentStatus.Unpaid);
            });
        }

        /// <summary>
        ///     Performs the find payment failure operation using the supplied arguments and current commerce context.
        /// </summary>
        /// <param name="startDate">The supplied start date value.</param>
        /// <param name="endDate">The supplied end date value.</param>
        /// <param name="pageNumber">The supplied page number value.</param>
        /// <param name="pageSize">The supplied page size value.</param>
        /// <param name="totalCount">Receives the total number of matching rows.</param>
        /// <returns>The value produced by the operation.</returns>
        public List<Order> FindPaymentFailure(DateTime startDate, DateTime endDate, int pageNumber, int pageSize,
            out int totalCount)
		{
			using (var strategy = CreateReadStrategy())
			{
				var query = strategy.GetQuery()
                        .AsNoTracking()
                        .Where(o => o.StoreId == Context.CurrentStore.Id)
                        .Where(o => o.hcc_LineItem.Any())
                        .Where(o => o.TimeOfOrder > startDate)
                        .Where(o => o.TimeOfOrder < endDate)
                        .Where(o => o.hcc_OrderTransactions.Any(t => !t.Success))
                        .Where(o => o.PaymentStatus == (int)OrderPaymentStatus.Unknown || o.PaymentStatus == (int)OrderPaymentStatus.Unpaid)
                        .OrderByDescending(o => o.TimeOfOrder);

				totalCount = query.Count();

				var items = GetPagedItems(query, pageNumber, pageSize);
				return ListPoco(items);
			}
		}

		/// <summary>
		///     Performs the find abandoned carts users operation using the supplied arguments and current commerce context.
		/// </summary>
		/// <param name="startDate">The supplied start date value.</param>
		/// <param name="endDate">The supplied end date value.</param>
		/// <returns>The value produced by the operation.</returns>
		public List<string> FindAbandonedCartsUsers(DateTime startDate, DateTime endDate)
		{
			using (var strategy = CreateReadStrategy())
			{
				return strategy.GetQuery()
                        .AsNoTracking()
                        .Where(o => o.StoreId == Context.CurrentStore.Id)
						.Where(o => o.IsPlaced == 0)
                    .Where(o => o.hcc_LineItem.Any())
						.Where(o => o.TimeOfOrder > startDate)
						.Where(o => o.TimeOfOrder < endDate)
						.Where(o => o.UserId != null && o.UserId != string.Empty)
						.Select(o => o.UserId)
						.Distinct()
						.ToList();
			}
		}

		/// <summary>
		///     Performs the find abandoned carts users operation using the supplied arguments and current commerce context.
		/// </summary>
		/// <param name="startDate">The supplied start date value.</param>
		/// <param name="endDate">The supplied end date value.</param>
		/// <param name="productId">The supplied product id value.</param>
		/// <returns>The value produced by the operation.</returns>
		public List<string> FindAbandonedCartsUsers(DateTime startDate, DateTime endDate, string productId)
		{
			var productGuid = DataTypeHelper.BvinToGuid(productId);
			using (var strategy = CreateReadStrategy())
			{
				return strategy.GetQuery()
                        .AsNoTracking()
                        .Where(o => o.StoreId == Context.CurrentStore.Id)
						.Where(o => o.IsPlaced == 0)
						.Where(o => o.hcc_LineItem.Any(li => li.ProductId == productGuid))
						.Where(o => o.TimeOfOrder > startDate)
						.Where(o => o.TimeOfOrder < endDate)
						.Where(o => o.UserId != null && o.UserId != string.Empty)
						.Select(o => o.UserId)
						.Distinct()
						.ToList();
			}
		}

		/// <summary>
		///     Performs the find line items for orders operation using the supplied arguments and current commerce context.
		/// </summary>
		/// <param name="snaps">The supplied snaps value.</param>
		/// <returns>The value produced by the operation.</returns>
		public List<LineItem> FindLineItemsForOrders(List<OrderSnapshot> snaps)
		{
			return ItemRepository.FindForOrders(snaps.Select(y => y.bvin).ToList());
		}

		/// <summary>
		///     Performs the find popular items operation using the supplied arguments and current commerce context.
		/// </summary>
		/// <param name="startDateUtc">The supplied start date utc value.</param>
		/// <param name="endDateUtc">The supplied end date utc value.</param>
		/// <param name="maxItems">The supplied max items value.</param>
		/// <returns>The value produced by the operation.</returns>
		public Dictionary<string, int> FindPopularItems(DateTime startDateUtc, DateTime endDateUtc, int maxItems)
		{
			return ItemRepository.FindPopularItems(startDateUtc, endDateUtc, maxItems);
		}

		/// <summary>
		///     Performs the find by coupon code operation using the supplied arguments and current commerce context.
		/// </summary>
		/// <param name="couponCode">The supplied coupon code value.</param>
		/// <param name="startDateUtc">The supplied start date utc value.</param>
		/// <param name="endDateUtc">The supplied end date utc value.</param>
		/// <returns>The value produced by the operation.</returns>
		public List<OrderSnapshot> FindByCouponCode(string couponCode, DateTime startDateUtc, DateTime endDateUtc)
		{
            var matchCode = couponCode.Trim().ToUpperInvariant();

			using (var strategy = CreateReadStrategy())
			{
				var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
					.Where(y => y.TimeOfOrder >= startDateUtc && y.TimeOfOrder <= endDateUtc)
                    .Where(y => y.hcc_OrderCoupon.Any(x => x.CouponCode == matchCode));

				return ListPocoSnapshot(query);
			}
		}

		internal void DestoryAllForStore(long storeId)
		{
			Delete(o => o.StoreId == storeId);
		}
	}
}
