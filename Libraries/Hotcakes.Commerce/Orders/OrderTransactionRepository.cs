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
using Hotcakes.Commerce.Reporting;
using Hotcakes.Payment;
using Hotcakes.Web;
using Hotcakes.Web.Cryptography;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Orders
{
	[Serializable]
	public class OrderTransactionRepository : HccSimpleRepoBase<hcc_OrderTransactions, OrderTransaction>
	{
		public OrderTransactionRepository(HccRequestContext c)
			: base(c)
		{
		}

		protected override void CopyDataToModel(hcc_OrderTransactions data, OrderTransaction model)
		{
            model.Action = (ActionType) data.Action;
			model.Amount = data.Amount;

			try
			{
				if (!string.IsNullOrWhiteSpace(data.GiftCard))
					model.GiftCard = Json.ObjectFromJson<GiftCardData>(data.GiftCard);
			}
            catch
            {
            }

			try
			{
                var key = KeyManager.GetKey(0);
				if (data.CreditCard.Trim().Length > 2)
				{
                    var json = AesEncryption.Decode(data.CreditCard, key);
					model.CreditCard = Json.ObjectFromJson<CardData>(json);
				}
			}
            catch
            {
            }

            var settingsJson = data.AdditionalSettings;
			model.AdditionalSettings = Json.ObjectFromJson<Dictionary<string, string>>(settingsJson);

			model.Id = data.Id;
			model.LinkedToTransaction = data.LinkedToTransaction;
			model.OrderId = DataTypeHelper.GuidToBvin(data.OrderId);
			model.OrderNumber = data.OrderNumber;
			model.RefNum1 = data.RefNum1;
			model.RefNum2 = data.RefNum2;
			model.StoreId = data.StoreId;
			model.Success = data.Success;
			model.TimeStampUtc = data.Timestamp;
			model.Voided = data.Voided;
			model.Messages = data.Messages;
			model.CheckNumber = data.CheckNumber;
			model.PurchaseOrderNumber = data.PurchaseOrderNumber;
			model.CompanyAccountNumber = data.CompanyAccountNumber;
			model.MethodId = data.MethodId;
			model.LineItemId = data.LineItemId;
		}

		protected override void CopyModelToData(hcc_OrderTransactions data, OrderTransaction model)
		{
            data.Action = (int) model.Action;
			data.Amount = model.Amount;

            data.GiftCard = model.GiftCard.ObjectToJson();

            var json = model.CreditCard.ObjectToJson();
            var key = KeyManager.GetKey(0);
			data.CreditCard = AesEncryption.Encode(json, key);

            var additionalSettings = model.AdditionalSettings.ObjectToJson();
			data.AdditionalSettings = additionalSettings;

			data.Id = model.Id;
			data.LinkedToTransaction = model.LinkedToTransaction;
			data.OrderId = DataTypeHelper.BvinToGuid(model.OrderId);
			data.OrderNumber = model.OrderNumber;
			data.RefNum1 = model.RefNum1;
			data.RefNum2 = model.RefNum2;
			data.StoreId = model.StoreId;
			data.Success = model.Success;
			data.Timestamp = model.TimeStampUtc;
			data.Voided = model.Voided;
			data.Messages = model.Messages;
            data.GiftCard = model.GiftCard.ObjectToJson();
			data.CheckNumber = model.CheckNumber;
			data.PurchaseOrderNumber = model.PurchaseOrderNumber;
			data.CompanyAccountNumber = model.CompanyAccountNumber;
			data.MethodId = model.MethodId;
			data.LineItemId = model.LineItemId;
		}

		public override bool Create(OrderTransaction item)
		{
			item.StoreId = Context.CurrentStore.Id;
			return base.Create(item);
		}

		public bool Update(OrderTransaction c)
		{
            return Update(c, y => y.Id == c.Id);
		}

		public bool Delete(Guid id)
		{
            var storeId = Context.CurrentStore.Id;
			return DeleteForStore(id, storeId);
		}

		internal bool DeleteForStore(Guid id, long storeId)
		{
			return Delete(y => y.Id == id && y.StoreId == storeId);
		}

		public OrderTransaction Find(Guid id)
		{
			return FindFirstPoco(y => y.Id == id && y.StoreId == Context.CurrentStore.Id);
		}

		public OrderTransaction FindForAllStores(Guid id)
		{
			return FindFirstPoco(y => y.Id == id);
		}

        public bool RecordPaymentTransaction(Transaction t, Order o)
		{
            var ot = new OrderTransaction(t);
			ot.OrderId = o.bvin;
			ot.StoreId = o.StoreId;
			return Create(ot);
		}

		public List<OrderTransaction> FindForOrder(string orderId, ActionType byAction = ActionType.Unknown)
		{
			var orderGuid = DataTypeHelper.BvinToGuid(orderId);
			return FindListPoco(q =>
			{
				var query = q.Where(y => y.OrderId == orderGuid);
				if (byAction != ActionType.Unknown)
				{
                    query = query.Where(i => i.Action == (int) byAction);
				}
				return query;
			});
        }

		public List<OrderTransaction> FindForOrderAndRma(string orderId, string rmaId)
		{
			using (var s = CreateStrategy())
			{
				var orderGuid = DataTypeHelper.BvinToGuid(orderId);
				var query = s.GetQuery().Where(y => y.OrderId == orderGuid)
					.OrderBy(y => y.Timestamp);

				var items = ListPoco(query);

				var result = items.Where(y => y.RMABvin == rmaId).ToList();
				return result;
			}
		}

		/// <summary>
        ///     Queries the store for all Transactions that match the given order.
		/// </summary>
		/// <param name="orderId">String - the unique ID of the order</param>
		/// <returns>A list of OrderTransaction for the given order.</returns>
		public List<OrderTransaction> FindAllTransactionsForOrder(string orderId)
		{
			var orderGuid = DataTypeHelper.BvinToGuid(orderId);
			return FindListPoco(q =>
			{
				return q.Where(y => y.OrderId == orderGuid)
											.Where(y => y.Success)
											.Where(y => !y.Voided)
											.OrderBy(y => y.Timestamp);
			});
		}

		/// <summary>
        ///     Queries the store for all authorizations that match the given order.
		/// </summary>
		/// <param name="orderId">String - the unique ID of the order</param>
		/// <returns>A list of OrderTransaction for the given order.</returns>
		public List<OrderTransaction> FindAllAuthorizationsForOrder(string orderId)
		{
			var orderGuid = DataTypeHelper.BvinToGuid(orderId);
			return FindListPoco(q =>
			{
				return q.Where(y => y.OrderId == orderGuid)
										.Where(y => y.Action == Convert.ToInt32(ActionType.CreditCardHold)
											|| y.Action == Convert.ToInt32(ActionType.GiftCardHold)
											|| y.Action == Convert.ToInt32(ActionType.PayPalHold)
											|| y.Action == Convert.ToInt32(ActionType.RewardPointsHold)
											|| y.Action == Convert.ToInt32(ActionType.ThirdPartyPayMethodHold))
										.Where(y => y.Success)
										.Where(y => !y.Voided)
										.OrderBy(y => y.Timestamp);
			});
		}

		public decimal TransactionsPotentialValue(List<OrderTransaction> transactions, ActionType actionType)
		{
			decimal amount = 0;
            foreach (var t in transactions)
			{
				if (t.Action == actionType)
				{
					amount += t.Amount;
				}
			}
			return amount;
		}

		public decimal TransactionsPotentialStoreCredits(List<OrderTransaction> transactions)
		{
			decimal amount = 0;
            foreach (var t in transactions)
			{
				if (t.Action == ActionType.GiftCardInfo ||
					t.Action == ActionType.RewardPointsInfo)
				{
					amount += t.Amount;
				}
			}
			return amount;
		}


		// Page Number is 1 based, NOT zero based
		public List<OrderTransaction> FindForReportByDateRange(DateTime startDateUtc, DateTime endDateUtc,
																long storeId, int pageSize,
																int pageNumber, ref int totalCount)
		{
			using (var s = CreateStrategy())
			{
                var actionCodes = ActionTypeUtils.BalanceChangingActions.Select(a => (int) a).ToList();

				var query = s.GetQuery().Where(y => y.StoreId == Context.CurrentStore.Id)
									.Where(y => y.Timestamp >= startDateUtc && y.Timestamp <= endDateUtc)
									.Where(y => y.Success)
									.Where(y => !y.Voided)
									.Where(y => actionCodes.Contains(y.Action))
									.OrderBy(y => y.Timestamp);

				totalCount = query.Count();
				var items = GetPagedItems(query, pageNumber, pageSize);

				return ListPoco(items);
			}
		}

        // Page Number is 1 based, NOT zero based
        public List<OrderTransaction> FindForCreditCardReportByDateRange(DateTime startDateUtc, DateTime endDateUtc,
                                                                long storeId, int pageSize,
                                                                int pageNumber, ref int totalCount)
        {
            using (var s = CreateStrategy())
            {
                List<int> actionCodes = ActionTypeUtils.BalanceChangingActionsForCreditCardReport.Select(a => (int)a).ToList();

                var query = s.GetQuery().Where(y => y.StoreId == Context.CurrentStore.Id)
                                    .Where(y => y.Timestamp >= startDateUtc && y.Timestamp <= endDateUtc)
                                    .Where(y => y.Success)
                                    .Where(y => !y.Voided)
                                    .Where(y => actionCodes.Contains(y.Action))
                                    .OrderBy(y => y.Timestamp);

                totalCount = query.Count();
                var items = GetPagedItems(query, pageNumber, pageSize);

                return ListPoco(items);
            }
        }

		public decimal FindBillableTransactionTotal(DateTime startDateUtc, DateTime endDateUtc, long storeId)
		{
			decimal result = 0;
			using (var s = CreateStrategy())
			{
				var query = s.GetQuery().Where(y => y.StoreId == Context.CurrentStore.Id)
										.Where(y => y.Timestamp >= startDateUtc && y.Timestamp <= endDateUtc);
				var items = ListPoco(query);
				result = items.Sum(y => y.AmountAppliedToOrder);
			}
			return result;
		}

		public decimal FindTotalTransactionsForever()
		{
			decimal result = 0;

			using (var s = CreateStrategy())
			{
                var actionCodes = ActionTypeUtils.BalanceChangingActions.Select(a => (int) a).ToList();

				var x = s.GetQuery()
								.Where(y => y.StoreId == Context.CurrentStore.Id)
								.Where(y => y.Success)
								.Where(y => !y.Voided)
								.Where(y => actionCodes.Contains(y.Action))
								.Select(y => y.Amount).Sum();

                if (x > 0) result = x;
			}

			return result;
		}

        public List<SalesSummaryData> FindTotalTransactionsByDateRange(DateTime startDateUtc, DateTime endDateUtc,
            Func<hcc_OrderTransactions, int> separationFunc)
		{
			using (var s = CreateStrategy())
			{
                var actionCodes = ActionTypeUtils.BalanceChangingActions.Select(a => (int) a).ToList();

				return s.GetQuery().Where(ot => ot.StoreId == Context.CurrentStore.Id)
                    .Where(ot => ot.hcc_Order.IsPlaced == 1 && ot.hcc_Order.StatusCode != OrderStatusCode.Cancelled)
					  .Where(y => y.Timestamp >= startDateUtc && y.Timestamp <= endDateUtc)
					  .Where(y => y.Success)
					  .Where(y => !y.Voided)
					  .Where(y => actionCodes.Contains(y.Action))
					  .GroupBy(separationFunc)
                    .Select(group => group.Select(item => new SalesSummaryData
					  {
						  Period = group.Key,
						  Sum = group.Sum(a => a.Amount)
					  }).FirstOrDefault()).ToList();
			}
		}

		internal void DestoryAllForStore(long storeId)
		{
			Delete(y => y.StoreId == storeId);
		}
	}
}
