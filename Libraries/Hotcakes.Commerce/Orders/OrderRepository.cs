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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Orders
{
	[Serializable]
	public class OrderRepository : HccSimpleRepoBase<hcc_Order, Order>
	{
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
            model.PaymentStatus = (OrderPaymentStatus) data.PaymentStatus;
            model.ShippingDiscountDetails = DiscountDetail.ListFromXml(data.ShippingDiscountDetails);
			model.ShippingAddress.FromXmlString(data.ShippingAddress);
			model.ShippingMethodDisplayName = data.ShippingMethodDisplayName;
			model.ShippingMethodId = data.ShippingMethodId;
			model.ShippingProviderId = data.ShippingProviderId;
			model.ShippingProviderServiceCode = data.ShippingProviderServiceCode;
            model.ShippingStatus = (OrderShippingStatus) data.ShippingStatus;
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
            model.UserDeviceType = (DeviceType) data.UserDeviceType;
			model.IsAbandonedEmailSent = data.IsAbandonedEmailSent;
			model.UsedCulture = data.UsedCulture;
		}

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
            data.PaymentStatus = (int) model.PaymentStatus;
			data.ShippingAddress = model.ShippingAddress.ToXml(true);
			data.ShippingDiscounts = model.TotalShippingDiscounts;
            data.ShippingDiscountDetails = DiscountDetail.ListToXml(model.ShippingDiscountDetails);
			data.ShippingMethodDisplayName = model.ShippingMethodDisplayName;
			data.ShippingMethodId = model.ShippingMethodId;
			data.ShippingProviderId = model.ShippingProviderId;
			data.ShippingProviderServiceCode = model.ShippingProviderServiceCode;
            data.ShippingStatus = (int) model.ShippingStatus;
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
            data.UserDeviceType = (int) model.UserDeviceType;
			data.IsAbandonedEmailSent = model.IsAbandonedEmailSent;
			data.IsRecurring = model.IsRecurring;
			data.UsedCulture = model.UsedCulture;
		}

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

		protected override void MergeSubItems(Order model)
		{
			ItemRepository.MergeList(model.bvin, model.StoreId, model.Items);
			NotesRepository.MergeList(model.bvin, model.StoreId, model.Notes);
			CouponRepository.MergeList(model.bvin, model.StoreId, model.Coupons);
			PackageRepository.MergeList(model.bvin, model.StoreId, model.Packages);
			ReturnsRepository.MergeList(model.bvin, model.StoreId, model.Returns);
		}

		public Order FindForCurrentStore(string bvin)
		{
            var result = FindForAllStores(bvin);
			if (result != null)
			{
				if (result.StoreId == Context.CurrentStore.Id)
				{
					return result;
				}
			}
			return null;
		}

		public Order FindForAllStores(string bvin)
		{
			var guid = DataTypeHelper.BvinToGuid(bvin);
			return FindFirstPoco(o => o.bvin == guid);
		}

		public Order FindByThirdPartyOrderId(string thirdPartyOrderId)
		{
			var storeId = Context.CurrentStore.Id;
			return FindFirstPoco(o => o.ThirdPartyOrderId == thirdPartyOrderId && o.StoreId == storeId);
		}

		public Order FindByOrderNumber(string orderNumber)
		{
			var storeId = Context.CurrentStore.Id;
			return FindFirstPoco(o => o.OrderNumber == orderNumber && o.StoreId == storeId);
		}

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

		public override bool Create(Order item)
		{
			item.LastUpdatedUtc = DateTime.UtcNow;
			if (item.bvin == string.Empty)
			{
				item.bvin = Guid.NewGuid().ToString();
			}
			item.StoreId = Context.CurrentStore.Id;
			item.ApplyVATRules = Context.CurrentStore.Settings.ApplyVATRules;
			return base.Create(item);
		}

		public bool Upsert(Order item)
		{
			if (item.bvin == string.Empty)
			{
				return Create(item);
			}
				return Update(item);
			}

		public bool Update(Order item, bool mergeSubItems = true)
		{
			if (item.StoreId != Context.CurrentStore.Id)
			{
				return false;
			}

			item.LastUpdatedUtc = DateTime.UtcNow;
			var guid = DataTypeHelper.BvinToGuid(item.bvin);
			return Update(item, o => o.bvin == guid, mergeSubItems);
		}

		public bool Delete(string bvin)
		{
			var guid = DataTypeHelper.BvinToGuid(bvin);
			return Delete(o => o.bvin == guid);
		}

		protected virtual List<OrderSnapshot> ListPocoSnapshot(IQueryable<hcc_Order> items)
		{
            var result = new List<OrderSnapshot>();

            var extendedItems = items.Select(i => new {item = i, isRecurring = i.hcc_LineItem.Any(l => l.IsRecurring)});

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
                temp.PaymentStatus = (OrderPaymentStatus) item.PaymentStatus;
				temp.TotalShippingDiscounts = item.ShippingDiscounts;
				temp.ShippingAddress.FromXmlString(item.ShippingAddress);
				temp.ShippingMethodDisplayName = item.ShippingMethodDisplayName;
				temp.ShippingMethodId = item.ShippingMethodId;
				temp.ShippingProviderId = item.ShippingProviderId;
				temp.ShippingProviderServiceCode = item.ShippingProviderServiceCode;
                temp.ShippingStatus = (OrderShippingStatus) item.ShippingStatus;
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

		public List<OrderSnapshot> FindAllForAllStores()
		{
			return FindAllPagedForAllStores(1, int.MaxValue);
		}

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

		public List<OrderSnapshot> FindByCriteria(OrderSearchCriteria criteria)
		{
            var temp = -1;
			return FindByCriteriaPaged(criteria, 1, int.MaxValue, ref temp);
		}

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
                query = query.Where(y => y.PaymentStatus != (int) OrderPaymentStatus.Paid);

				// Payment State is not overpaid Paid
                query = query.Where(y => y.PaymentStatus != (int) OrderPaymentStatus.Overpaid);

				query = query.OrderByDescending(y => y.TimeOfOrder);

				// return total item count;
				rowCount = query.Count();

				var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
				return ListPocoSnapshot(pagedQuery);
			}
		}

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
                query = query.Where(y => y.ShippingStatus != (int) OrderShippingStatus.NonShipping);

				// Shipping State is not fully shipped
                query = query.Where(y => y.ShippingStatus != (int) OrderShippingStatus.FullyShipped);

				query = query.OrderByDescending(y => y.TimeOfOrder);

				// return total item count;
				rowCount = query.Count();


				var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
				return ListPocoSnapshot(pagedQuery);
			}
		}

        public List<OrderSnapshot> FindByCriteriaPaged(OrderSearchCriteria criteria, int pageNumber, int pageSize,
            ref int rowCount)
		{
			using (var strategy = CreateReadStrategy())
			{
				var query = strategy.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.StoreId == Context.CurrentStore.Id)
					.Where(y => y.TimeOfOrder >= criteria.StartDateUtc && y.TimeOfOrder <= criteria.EndDateUtc);

				//if (criteria.OrderNumber != string.Empty)
				//{
				//	query = query.Where(y => y.OrderNumber == criteria.OrderNumber);
				//}

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
                    var tempPay = (int) criteria.PaymentStatus;
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
                    var tempShip = (int) criteria.ShippingStatus;
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
                query = query.Where(y => y.PaymentStatus != (int) OrderPaymentStatus.Paid);

				// Payment State is not overpaid
                query = query.Where(y => y.PaymentStatus != (int) OrderPaymentStatus.Overpaid);

				return query.Count();
			}
		}

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
                query = query.Where(y => y.ShippingStatus != (int) OrderShippingStatus.NonShipping);

				// Shipping State is not fully shipped
                query = query.Where(y => y.ShippingStatus != (int) OrderShippingStatus.FullyShipped);

				return query.Count();
			}
		}

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
                    var tempPay = (int) criteria.PaymentStatus;
					query = query.Where(y => y.PaymentStatus == tempPay);
				}

				// Shipping Status
				if (criteria.ShippingStatus != OrderShippingStatus.Unknown)
				{
                    var tempShip = (int) criteria.ShippingStatus;
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

		
		public List<OrderSnapshot> FindByUserId(string userId, int pageNumber, int pageSize, ref int totalCount)
		{
			using (var strategy = CreateReadStrategy())
			{
				var query = strategy.GetQuery()
                        .AsNoTracking()
                        .Where(y => y.UserId == userId)
						.Where(y => y.StoreId == Context.CurrentStore.Id)
						//If order has number assigned then independent of its status or IsPlaced it should appear in list
						.Where( y => y.OrderNumber.Trim().Length > 0)

						// If Order is complete
						//.Where( y => y.StatusCode == OrderStatusCode.Completed)

						// If Order is cancelled but has order id
						//.Where( y => y.StatusCode == OrderStatusCode.Cancelled)

						// If Order is received
						//.Where( y => y.StatusCode == OrderStatusCode.Received )

						// If Order is on hold
						//.Where( y => y.StatusCode == OrderStatusCode.OnHold)

						//.Where(y => y.IsPlaced == 1)
						.OrderByDescending(y => y.TimeOfOrder);


				totalCount = query.Count();

				var pagedQuery = GetPagedItems(query, pageNumber, pageSize);
				return ListPocoSnapshot(pagedQuery);
			}
		}

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

		public List<Order> FindAbandonedCarts()
		{
			var daysInterval = Context.CurrentStore.Settings.SendAbandonedEmailIn;
            var endTouchDate = DateTime.UtcNow.AddDays(-1*daysInterval);

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

		public List<LineItem> FindLineItemsForOrders(List<OrderSnapshot> snaps)
		{
			return ItemRepository.FindForOrders(snaps.Select(y => y.bvin).ToList());
		}

		public Dictionary<string, int> FindPopularItems(DateTime startDateUtc, DateTime endDateUtc, int maxItems)
		{
			return ItemRepository.FindPopularItems(startDateUtc, endDateUtc, maxItems);
		}

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
