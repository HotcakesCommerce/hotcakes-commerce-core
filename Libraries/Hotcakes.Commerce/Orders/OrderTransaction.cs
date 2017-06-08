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
using Hotcakes.CommerceDTO.v1.Orders;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of transactions in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is OrderTransactionDTO.</remarks>
    [Serializable]
    public class OrderTransaction
    {
		#region Properties

		/// <summary>
        ///     The unique ID of the transaction.
		/// </summary>
		public Guid Id { get; set; }

		/// <summary>
        ///     A string version of the Id property.
		/// </summary>
		public string IdAsString
		{
            get { return Id.ToString(); }
			}

		/// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
		/// </summary>
		public long StoreId { get; set; }

		/// <summary>
        ///     The unique ID or bvin of the order that this transaction belongs to.
		/// </summary>
		public string OrderId { get; set; }

		/// <summary>
        ///     The display number of the order that this transaction belongs to.
		/// </summary>
		public string OrderNumber { get; set; }

		/// <summary>
        ///     The unique ID of the order line item that this transaction belongs to.
		/// </summary>
		public long? LineItemId { get; set; }

		/// <summary>
        ///     A date/time stamp used for auditing purposes to know when the transaction was created.
		/// </summary>
		public DateTime TimeStampUtc { get; set; }

		/// <summary>
        ///     Defines what kind of transaction this is.
		/// </summary>
		public ActionType Action { get; set; }

		/// <summary>
        ///     The total amount of the transaction.
		/// </summary>
		public decimal Amount { get; set; }

		/// <summary>
        ///     Information to define the associated gift card, when necessary.
		/// </summary>
		public GiftCardData GiftCard { get; set; }

		/// <summary>
        ///     Information to define the associated credit card, when necessary.
		/// </summary>
		public CardData CreditCard { get; set; }

		/// <summary>
        ///     Defines whether the transaction was successful or not.
		/// </summary>
		public bool Success { get; set; }

		/// <summary>
        ///     Notes that this transaction is a void for another transaction.
		/// </summary>
		public bool Voided { get; set; }

		/// <summary>
        ///     The primary reference number or token returned by the payment processor.
		/// </summary>
		public string RefNum1 { get; set; }

		/// <summary>
        ///     The secondary reference number or token returned by the payment processor.
		/// </summary>
		public string RefNum2 { get; set; }

		/// <summary>
        ///     When necessary, the unique ID of another transaction that this one is linked to, such as a credit or gift card
        ///     payment.
		/// </summary>
		public string LinkedToTransaction { get; set; }

		/// <summary>
        ///     Useful information about the transaction, such as errors or warnings.
		/// </summary>
		public string Messages { get; set; }

		/// <summary>
        ///     When the transaction is a check payment, the check number should be here.
		/// </summary>
		public string CheckNumber { get; set; }

		/// <summary>
        ///     When the transaction is a purchase order payment, the PO number should be here.
		/// </summary>
		public string PurchaseOrderNumber { get; set; }

		/// <summary>
        ///     When the transaction uses company accounts for payment, the company's account number should be here.
		/// </summary>
		public string CompanyAccountNumber { get; set; }

		/// <summary>
        ///     Contains a collection of optional settings or meta data for the transaction that do not have a property.
		/// </summary>
		public Dictionary<string, string> AdditionalSettings { get; set; }

		/// <summary>
        ///     Determines what payment mehtod is used. Currently is being used for ThirdParty Payment Methods only
		/// </summary>
		public string MethodId { get; set; }

		/// <summary>
        ///     Used for returns to store a return reference ID or bvin.
		/// </summary>
		public string RMABvin
		{
			get { return GetAdditionalSettingAsString("RMABvin"); }
			set { SetAdditionalSetting("RMABvin", value); }
		}

		#endregion

        #region Temp Properties

        // Not saved to database, calculated on the fly in reports
        // In a future update these will get real values stored 
        // in DB.

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public decimal TempEstimatedItemPortion { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public decimal TempEstimatedItemDiscount { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public decimal TempEstimatedShippingPortion { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public decimal TempEstimatedShippingDiscount { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public decimal TempEstimatedHandlingPortion { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public decimal TempEstimatedTaxPortion { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public string TempCustomerName { get; set; }

        /// <summary>
        ///     Do not use for any API development. This property will be removed later.
        /// </summary>
        public string TempCustomerEmail { get; set; }

        #endregion

		#region Constructors

		public OrderTransaction()
		{
			Id = Guid.NewGuid();
			StoreId = 0;
			OrderId = string.Empty;
			OrderNumber = string.Empty;
			TimeStampUtc = DateTime.UtcNow;
			Action = ActionType.Unknown;
			Amount = 0m;
			GiftCard = new GiftCardData();
			CreditCard = new CardData();
			Success = false;
			Voided = false;
			RefNum1 = string.Empty;
			RefNum2 = string.Empty;
			LinkedToTransaction = string.Empty;
			Messages = string.Empty;
			PurchaseOrderNumber = string.Empty;
			CheckNumber = string.Empty;
			CompanyAccountNumber = string.Empty;
			AdditionalSettings = new Dictionary<string, string>();
		}

		public OrderTransaction(Transaction t)
		{
			Id = Guid.NewGuid();
			StoreId = 0;
			OrderId = string.Empty;
			LinkedToTransaction = string.Empty;
			AdditionalSettings = new Dictionary<string, string>();
			PopulateFromPaymentTransaction(t);
		} 

		#endregion

		#region Public Methods

		/// <summary>
        ///     Hydrates the current transaction object from another.
		/// </summary>
		/// <param name="t">A fully populated transaction object.</param>
        public void PopulateFromPaymentTransaction(Transaction t)
		{
			if (t == null)
				return;

			TimeStampUtc = DateTime.UtcNow;
			Action = t.Action;
			Amount = t.Amount;
			if (t.IsRefundTransaction)
			{
                Amount = t.Amount*-1;
			}
			GiftCard = t.GiftCard;
			CreditCard = t.Card;
			Success = t.Result.Succeeded;
			Voided = false;
			RefNum1 = t.Result.ReferenceNumber;
			RefNum2 = t.Result.ReferenceNumber2;
			Messages = string.Empty;
			if (t.Result.Messages.Count > 0)
			{
                foreach (var m in t.Result.Messages)
				{
					Messages += ":: " + m.Code + " - " + m.Description + " ";
				}
			}
			CheckNumber = t.CheckNumber;
			PurchaseOrderNumber = t.PurchaseOrderNumber;

			CompanyAccountNumber = t.CompanyAccountNumber;
			AdditionalSettings = new Dictionary<string, string>();
			if (t.AdditionalSettings != null)
			{
				foreach (var s in t.AdditionalSettings)
				{
					AdditionalSettings.Add(s.Key, s.Value);
				}
			}
		}

		/// <summary>
        ///     Voids are only allowed for 16 hours dur to the restrictions set by payment gateways. Refunds should be used after
        ///     16 hours.
		/// </summary>
		/// <remarks>
        ///     Only allow voids for about 16 hours since most credit card companies don't allow voids after the batch is processed
        ///     for the day. Merchants should use a Refund instead of void at that point.
		/// </remarks>
		public bool IsVoidable
		{
			get
			{
                var isWithinTimeWindow = false;

                var cutOffTicks = DateTime.UtcNow.AddHours(-16).Ticks;
                var timestampTicks = TimeStampUtc.Ticks;
				if (timestampTicks >= cutOffTicks)
				{
					isWithinTimeWindow = true;
				}

				if (isWithinTimeWindow)
				{
					if (Action == ActionType.CreditCardCapture ||
						Action == ActionType.CreditCardCharge ||
						Action == ActionType.CreditCardHold ||
						Action == ActionType.CreditCardRefund ||
						Action == ActionType.PayPalCapture ||
						Action == ActionType.PayPalCharge ||
						Action == ActionType.PayPalHold ||
						Action == ActionType.PayPalRefund ||
						Action == ActionType.ThirdPartyPayMethodCapture ||
						Action == ActionType.ThirdPartyPayMethodCharge ||
						Action == ActionType.ThirdPartyPayMethodHold ||
						Action == ActionType.ThirdPartyPayMethodRefund)
						return true;
				}
				return false;
			}
		}

		/// <summary>
        ///     Reflects the amount of currency applied to the order in this transaction.
		/// </summary>
		public decimal AmountAppliedToOrder
		{
			get
			{
				decimal result = 0;

				if (Success)
				{
					if (!Voided)
					{
						if (Action == ActionType.CreditCardCapture ||
							Action == ActionType.CreditCardCharge ||
							Action == ActionType.CreditCardRefund ||
							Action == ActionType.CashReceived ||
							Action == ActionType.CashReturned ||
							Action == ActionType.CheckReceived ||
							Action == ActionType.CheckReturned ||
							Action == ActionType.GiftCardCapture ||
							Action == ActionType.GiftCardDecrease ||
							Action == ActionType.GiftCardIncrease ||
							Action == ActionType.PayPalCapture ||
							Action == ActionType.PayPalCharge ||
							Action == ActionType.PayPalRefund ||
							Action == ActionType.PurchaseOrderAccepted ||
							Action == ActionType.CompanyAccountAccepted ||
							Action == ActionType.RewardPointsDecrease ||
							Action == ActionType.RewardPointsCapture ||
							Action == ActionType.RewardPointsIncrease ||
							Action == ActionType.ThirdPartyPayMethodCapture ||
							Action == ActionType.ThirdPartyPayMethodCharge ||
							Action == ActionType.ThirdPartyPayMethodRefund ||
							Action == ActionType.RecurringPayment)
						{
							result += Amount;
						}
					}
				}

				return result;
			}
		}

		/// <summary>
        ///     Reflects the amount of currency held for the order in this transaction.
		/// </summary>
		public decimal AmountHeldForOrder
		{
			get
			{
				decimal result = 0;

				if (Success)
				{
					if (!Voided)
					{
						if (Action == ActionType.CreditCardHold ||
							Action == ActionType.GiftCardHold ||
							Action == ActionType.PayPalHold ||
							Action == ActionType.RewardPointsHold ||
							Action == ActionType.RewardPointsUnHold ||
							Action == ActionType.ThirdPartyPayMethodHold)
						{
							result += Amount;
						}
					}
				}

				return result;
			}
		}

		/// <summary>
        ///     Returns the order that this transaction was made for.
		/// </summary>
		/// <param name="hccApp">An instance of the Hotcakes Application context.</param>
		/// <returns>Order</returns>
        public Order FindOrderForThis(HotcakesApplication hccApp)
		{
            if (!string.IsNullOrEmpty(OrderId))
			{
				return hccApp.OrderServices.Orders.FindForCurrentStore(OrderId);
			}
			return null;
		}

		/// <summary>
        ///     Evaluates whether any transactions are linked to the current one that have succeeded.
		/// </summary>
		/// <param name="action">Transaction type to evaluate.</param>
		/// <param name="transactions">A collection of transactions to parse for a match.</param>
		/// <returns>If true, a successful transaction was found that links to this one.</returns>
        public bool HasSuccessfulLinkedAction(ActionType action, List<OrderTransaction> transactions)
		{
            return
                transactions.Any(
                    t => t.Success && !t.Voided && t.Action == action && t.LinkedToTransaction == IdAsString);
		} 

		#endregion

        #region DTO

        /// <summary>
        ///     Allows you to convert the current transaction object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of OrderTransactionDTO</returns>
        public OrderTransactionDTO ToDto()
        {
            var dto = new OrderTransactionDTO();

            dto.Action = (OrderTransactionActionDTO) (int) Action;
            dto.Amount = Amount;
            dto.CheckNumber = CheckNumber;
            dto.CompanyAccountNumber = CompanyAccountNumber;
            dto.CreditCard = new OrderTransactionCardDataDTO
            {
                CardHolderName = CreditCard.CardHolderName,
                CardIsEncrypted = false,
                CardNumber = CreditCard.CardNumber,
                ExpirationMonth = CreditCard.ExpirationMonth,
                ExpirationYear = CreditCard.ExpirationYear
            };
            dto.GiftCard.LineItemId = GiftCard.LineItemId;
            dto.GiftCard.CardNumber = GiftCard.CardNumber;
            dto.GiftCard.Amount = GiftCard.Amount;
            dto.GiftCard.RecipientName = GiftCard.RecipientName;
            dto.GiftCard.RecipientEmail = GiftCard.RecipientEmail;
            dto.GiftCard.ExpirationDate = GiftCard.ExpirationDate;
            dto.Id = Id;
            dto.LinkedToTransaction = LinkedToTransaction;
            dto.Messages = Messages;
            dto.OrderId = OrderId;
            dto.OrderNumber = OrderNumber;
            dto.PurchaseOrderNumber = PurchaseOrderNumber;
            dto.RefNum1 = RefNum1;
            dto.RefNum2 = RefNum2;
            dto.StoreId = StoreId;
            dto.Success = Success;
            dto.TimeStampUtc = TimeStampUtc;
            dto.Voided = Voided;

            return dto;
        }

        /// <summary>
        ///     Allows you to populate the current transaction object using a OrderTransactionDTO instance
        /// </summary>
        /// <param name="dto">An instance of the transaction from the REST API</param>
        public void FromDto(OrderTransactionDTO dto)
        {
            if (dto == null) return;

            Action = (ActionType) (int) dto.Action;
            Amount = dto.Amount;
            CheckNumber = dto.CheckNumber ?? string.Empty;
            CompanyAccountNumber = dto.CompanyAccountNumber ?? string.Empty;
            if (dto.CreditCard != null)
            {
                if (dto.CreditCard.CardIsEncrypted == false)
                {
                    CreditCard.CardNumber = dto.CreditCard.CardNumber ?? string.Empty;
                }
                CreditCard.CardHolderName = dto.CreditCard.CardHolderName ?? string.Empty;
                CreditCard.ExpirationMonth = dto.CreditCard.ExpirationMonth;
                CreditCard.ExpirationYear = dto.CreditCard.ExpirationYear;
            }
            if (dto.GiftCard != null)
            {
                GiftCard.LineItemId = dto.GiftCard.LineItemId;
                GiftCard.CardNumber = dto.GiftCard.CardNumber;
                GiftCard.Amount = dto.GiftCard.Amount;
                GiftCard.RecipientName = dto.GiftCard.RecipientName;
                GiftCard.RecipientEmail = dto.GiftCard.RecipientEmail;
                GiftCard.ExpirationDate = dto.GiftCard.ExpirationDate;
            }
            Id = dto.Id;
            LinkedToTransaction = dto.LinkedToTransaction ?? string.Empty;
            Messages = dto.Messages ?? string.Empty;
            OrderId = dto.OrderId ?? string.Empty;
            OrderNumber = dto.OrderNumber ?? string.Empty;
            PurchaseOrderNumber = dto.PurchaseOrderNumber ?? string.Empty;
            RefNum1 = dto.RefNum1 ?? string.Empty;
            RefNum2 = dto.RefNum2 ?? string.Empty;
            StoreId = dto.StoreId;
            Success = dto.Success;
            TimeStampUtc = dto.TimeStampUtc;
            Voided = dto.Voided;
        }

        #endregion

        #region Additional Settings

        /// <summary>
        ///     Adds or updates an additional setting using the given values.
        /// </summary>
        /// <param name="key">The unique name that is used to store and retrieve the additional setting.</param>
        /// <param name="value">This is the information that you are saving to later retrieve.</param>
        public void SetAdditionalSetting(string key, string value)
        {
            if (AdditionalSettings.ContainsKey(key))
            {
                AdditionalSettings[key] = value;
            }
            else
            {
                AdditionalSettings.Add(key, value);
            }
        }

        /// <summary>
        ///     Queries for and returns the requested additional setting value.
        /// </summary>
        /// <param name="key">The unique name that is used to store and retrieve the additional setting.</param>
        /// <returns></returns>
        public string GetAdditionalSettingAsString(string key)
        {
            var result = string.Empty;
            if (AdditionalSettings == null) return result;
            if (AdditionalSettings.ContainsKey(key))
            {
                result = AdditionalSettings[key];
            }
            return result;
        }
        
        /// <summary>
        ///     Queries for and returns the requested additional setting value.
        /// </summary>
        /// <param name="key">The unique name that is used to store and retrieve the additional setting.</param>
        /// <returns></returns>
        public decimal GetAdditionalSettingAsDecimal(string key)
        {
            decimal result = -1;

            var s = GetAdditionalSettingAsString(key);
            decimal.TryParse(s, out result);

            return result;
        }
        
        /// <summary>
        ///     Queries for and returns the requested additional setting value.
        /// </summary>
        /// <param name="key">The unique name that is used to store and retrieve the additional setting.</param>
        /// <returns></returns>
        public int GetAdditionalSettingAsInteger(string key)
        {
            var result = -1;

            var s = GetAdditionalSettingAsString(key);
            int.TryParse(s, out result);

            return result;
        }

        #endregion
    }
}