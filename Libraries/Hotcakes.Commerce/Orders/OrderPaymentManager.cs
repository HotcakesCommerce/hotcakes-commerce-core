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
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Payment.Methods;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    public class OrderPaymentManager
    {
        #region Constructor

        public OrderPaymentManager(Order ord, HotcakesApplication app)
        {
            o = ord;
            _app = app;
            svc = _app.OrderServices;

            _pointsManager = new CustomerPointsManager(app.CurrentRequestContext);
        }

        #endregion

        #region Public methods / Offline Payment

        public bool OfflinePaymentAddInfo(decimal amount, string description)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.Action = ActionType.OfflinePaymentRequest;
            var ot = new OrderTransaction(t);
            ot.Messages = description;
            ot.Success = true;
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        #endregion

        #region Fields

        private readonly HotcakesApplication _app;
        private readonly OrderService svc;
        private readonly Order o;
        private readonly CustomerPointsManager _pointsManager;

        #endregion

        #region Public methods

        public OrderTransaction FindTransactionById(string id)
        {
            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.IdAsString.ToLower() == id.ToLower())
                {
                    return t;
                }
            }
            return null;
        }

        public bool ClearAllTransactions()
        {
            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                svc.Transactions.Delete(t.Id);
            }
            //reload local if we were using local store
            return true;
        }

        public bool ClearAllNonStoreCreditTransactions()
        {
            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.RewardPointsInfo ||
                    t.Action == ActionType.GiftCardInfo)
                {
                    continue;
                }
                svc.Transactions.Delete(t.Id);
            }
            //reload local if we were using local store
            return true;
        }

        /// <summary>
        ///     Creates a new transaction with the billing &amp; shipping addresses and line items for the order.
        /// </summary>
        /// <returns>Transaction</returns>
        public Transaction CreateEmptyTransaction(bool addLineItems = true)
        {
            var t = new Transaction
            {
                Customer =
                {
                    IpAddress = _app.CurrentRequestContext.RoutingContext.HttpContext.Request.UserHostAddress,
                    Email = o.UserEmail,
                    City = o.BillingAddress.City,
                    Company = o.BillingAddress.Company,
                    CountryName = o.BillingAddress.CountrySystemName,
                    CountryBvin = o.BillingAddress.CountryBvin,
                    CountryData = o.BillingAddress.CountryData,
                    FirstName = o.BillingAddress.FirstName,
                    LastName = o.BillingAddress.LastName,
                    Phone = o.BillingAddress.Phone,
                    PostalCode = o.BillingAddress.PostalCode,
                    RegionName = o.BillingAddress.RegionSystemName,
                    RegionBvin = o.BillingAddress.RegionBvin,
                    Street = o.BillingAddress.Line1,
                    ShipCity = o.ShippingAddress.City,
                    ShipCompany = o.ShippingAddress.Company,
                    ShipCountryName = o.ShippingAddress.CountrySystemName,
                    ShipCountryBvin = o.ShippingAddress.CountryBvin,
                    ShipCountryData = o.ShippingAddress.CountryData,
                    ShipFirstName = o.ShippingAddress.FirstName,
                    ShipLastName = o.ShippingAddress.LastName,
                    ShipPhone = o.ShippingAddress.Phone,
                    ShipPostalCode = o.ShippingAddress.PostalCode,
                    ShipRegionName = o.ShippingAddress.RegionSystemName,
                    ShipRegionBvin = o.ShippingAddress.RegionBvin,
                    ShipStreet = o.ShippingAddress.Line1
                },
                MerchantDescription = "Order " + o.OrderNumber,
                MerchantInvoiceNumber = o.OrderNumber
            };

            foreach (var li in o.Items)
            {
                t.Items.Add(new TransactionItem
                {
                    Description = li.ProductName,
                    LineTotal = li.LineTotal,
                    Sku = li.ProductSku,
                    IsNonShipping = li.IsNonShipping
                });
            }

            return t;
        }

        #endregion

        #region Public methods / Cash

        public bool CashReceive(decimal amount)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.Action = ActionType.CashReceived;
            var ot = new OrderTransaction(t);
            ot.Success = true;
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CashRefund(decimal amount, string rmaBvin = "")
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.Action = ActionType.CashReturned;
            var ot = new OrderTransaction(t);
            ot.Success = true;
            ot.RMABvin = rmaBvin;
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        #endregion

        #region Public methods / Checks

        public bool CheckReceive(decimal amount, string checkNumber)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.CheckNumber = checkNumber;
            t.Action = ActionType.CheckReceived;
            var ot = new OrderTransaction(t);
            ot.Success = true;
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CheckReturn(decimal amount, string checkNumber, string rmaBvin = "")
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.CheckNumber = checkNumber;
            t.Action = ActionType.CheckReturned;
            var ot = new OrderTransaction(t);
            ot.Success = true;
            ot.RMABvin = rmaBvin;
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        #endregion

        #region Public methods / Purchase Order

        public bool PurchaseOrderAccept(string poNumber)
        {
            var t = CreateEmptyTransaction();
            t.PurchaseOrderNumber = poNumber;
            t.Action = ActionType.PurchaseOrderAccepted;
            var ot = new OrderTransaction(t);

            var existing = LocateExistingPurchaseOrder(poNumber);
            if (existing == null)
            {
                ot.Success = false;
                ot.Messages = "Could not located the requested PO.";
            }
            else
            {
                if (existing.HasSuccessfulLinkedAction(ActionType.PurchaseOrderAccepted,
                    svc.Transactions.FindForOrder(o.bvin)))
                {
                    // Fail, already accepted
                    ot.Success = false;
                    ot.Messages = "The requested PO has already been accepted.";
                }

                // Succes, receive it, link it to the info transaction
                ot.Amount = EnsurePositiveAmount(existing.Amount);
                ot.LinkedToTransaction = existing.IdAsString;
                ot.Success = true;
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        private OrderTransaction LocateExistingPurchaseOrder(string poNumber)
        {
            foreach (var t in PurchaseOrderInfoListAll())
            {
                if (t.PurchaseOrderNumber.ToLower() == poNumber.Trim().ToLower())
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> PurchaseOrderInfoListAll()
        {
            return FindAllTransactionsOfType(ActionType.PurchaseOrderInfo);
        }

        public List<OrderTransaction> PurchaseOrderInfoListAllNonAccepted()
        {
            var result = new List<OrderTransaction>();
            var transactions = svc.Transactions.FindForOrder(o.bvin);
            foreach (var t in PurchaseOrderInfoListAll())
            {
                if (!t.HasSuccessfulLinkedAction(ActionType.PurchaseOrderAccepted, transactions))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public bool PurchaseOrderAddInfo(string poNumber, decimal amount)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.PurchaseOrderNumber = poNumber;
            t.Action = ActionType.PurchaseOrderInfo;
            var ot = new OrderTransaction(t) {Success = true};
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        #endregion

        #region Public methods / Company Account

        public bool CompanyAccountAccept(string accountNumber)
        {
            var t = CreateEmptyTransaction();
            t.CompanyAccountNumber = accountNumber;
            t.Action = ActionType.CompanyAccountAccepted;
            var ot = new OrderTransaction(t);

            var existing = LocateExistingCompanyAccount(accountNumber);
            if (existing == null)
            {
                ot.Success = false;
                ot.Messages = "Could not located the requested Company Account.";
            }
            else
            {
                if (existing.HasSuccessfulLinkedAction(ActionType.CompanyAccountAccepted,
                    svc.Transactions.FindForOrder(o.bvin)))
                {
                    // Fail, already accepted
                    ot.Success = false;
                    ot.Messages = "The requested Company Account has already been accepted.";
                }

                // Succes, receive it, link it to the info transaction
                ot.Amount = EnsurePositiveAmount(existing.Amount);
                ot.LinkedToTransaction = existing.IdAsString;
                ot.Success = true;
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        private OrderTransaction LocateExistingCompanyAccount(string accountNumber)
        {
            foreach (var t in CompanyAccountInfoListAll())
            {
                if (t.CompanyAccountNumber.ToLower() == accountNumber.Trim().ToLower())
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> CompanyAccountInfoListAll()
        {
            return FindAllTransactionsOfType(ActionType.CompanyAccountInfo);
        }

        public List<OrderTransaction> CompanyAccountInfoListAllNonAccepted()
        {
            var result = new List<OrderTransaction>();
            var transactions = svc.Transactions.FindForOrder(o.bvin);
            foreach (var t in CompanyAccountInfoListAll())
            {
                if (!t.HasSuccessfulLinkedAction(ActionType.CompanyAccountAccepted, transactions))
                {
                    result.Add(t);
                }
            }

            return result;
        }

        public bool CompanyAccountAddInfo(string accountNumber, decimal amount)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.CompanyAccountNumber = accountNumber;
            t.Action = ActionType.CompanyAccountInfo;
            var ot = new OrderTransaction(t) {Success = true};
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        #endregion

        #region Public methods / Credit Cards

        public bool CreditCardAddInfo(CardData card, decimal amount)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.Card = card;
            t.Action = ActionType.CreditCardInfo;
            var ot = new OrderTransaction(t) {Success = true};
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CreditCardHold(string infoTransactionId, decimal amount, string securityCode = null)
        {
            var t = CreditCardInfoFind(infoTransactionId);
            return CreditCardHold(t, amount, securityCode);
        }

        public bool CreditCardHold(OrderTransaction infoTransaction, decimal amount, string securityCode = null)
        {
            if (infoTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Card = infoTransaction.CreditCard;
            t.Card.SecurityCode = securityCode;
            t.Action = ActionType.CreditCardHold;
            t.Amount = EnsurePositiveAmount(amount);
            var ot = new OrderTransaction(t);

            if (infoTransaction.Action != ActionType.CreditCardInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be CC info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = PaymentGateways.CurrentPaymentProcessor(context.CurrentStore);
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = infoTransaction.IdAsString};
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public List<OrderTransaction> CreditCardInfoListAll()
        {
            return FindAllTransactionsOfType(ActionType.CreditCardInfo);
        }

        public OrderTransaction CreditCardInfoFind(string id)
        {
            return FindSingleTransactionByTypeAndId(ActionType.CreditCardInfo, id);
        }

        public List<OrderTransaction> CreditCardHoldListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.CreditCardHold)
                {
                    if (!t.Voided && t.Success)
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public List<OrderTransaction> CreditCardChargeOrCaptureListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.CreditCardCapture || t.Action == ActionType.CreditCardCharge)
                {
                    if (!t.Voided && t.Success)
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public OrderTransaction CreditCardHoldFind(string id)
        {
            foreach (var t in CreditCardHoldListAll())
            {
                if (t.IdAsString == id)
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> CreditCardChargeListAllRefundable()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.CreditCardCapture ||
                    t.Action == ActionType.CreditCardCharge)
                {
                    if (!t.Voided)
                    {
                        if (t.Success)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            return result;
        }

        public bool CreditCardCapture(string holdTransactionId, decimal amount, string securityCode = null)
        {
            var t = CreditCardHoldFind(holdTransactionId);
            return CreditCardCapture(t, amount, securityCode);
        }

        public bool CreditCardCapture(OrderTransaction holdTransaction, decimal amount, string securityCode = null)
        {
            if (holdTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Card = holdTransaction.CreditCard;
            t.Card.SecurityCode = securityCode;
            t.Action = ActionType.CreditCardCapture;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = holdTransaction.RefNum1;
            t.PreviousTransactionAuthCode = holdTransaction.RefNum2;
            t.Items = GetLineItemsForTransaction(holdTransaction.OrderNumber);
            var ot = new OrderTransaction(t);

            if (holdTransaction.Action != ActionType.CreditCardHold)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be CC hold type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = PaymentGateways.CurrentPaymentProcessor(context.CurrentStore);
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t);
                ot.LinkedToTransaction = holdTransaction.IdAsString;
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CreditCardCharge(string infoTransactionId, decimal amount, string securityCode = null)
        {
            var t = CreditCardInfoFind(infoTransactionId);
            return CreditCardCharge(t, amount, securityCode);
        }

        public bool CreditCardCharge(OrderTransaction infoTransaction, decimal amount, string securityCode = null)
        {
            if (infoTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Card = infoTransaction.CreditCard;
            t.Card.SecurityCode = securityCode;
            t.Action = ActionType.CreditCardCharge;
            t.Amount = EnsurePositiveAmount(amount);
            t.Items = GetLineItemsForTransaction(infoTransaction.OrderNumber);
            var ot = new OrderTransaction(t);

            if (infoTransaction.Action != ActionType.CreditCardInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be CC info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = PaymentGateways.CurrentPaymentProcessor(context.CurrentStore);
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = infoTransaction.IdAsString};
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CreditCardRefund(string previousTransaction, decimal amount, string securityCode = null,
            string rmaBvin = "")
        {
            var t = FindTransactionById(previousTransaction);
            return CreditCardRefund(t, amount, securityCode, rmaBvin);
        }

        public bool CreditCardRefund(OrderTransaction previousTransaction, decimal amount, string securityCode = null,
            string rmaBvin = "")
        {
            if (previousTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Card = previousTransaction.CreditCard;
            t.Card.SecurityCode = securityCode;
            t.Action = ActionType.CreditCardRefund;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = previousTransaction.RefNum1;
            t.PreviousTransactionAuthCode = previousTransaction.RefNum2;
            t.Items = GetLineItemsForTransaction(previousTransaction.OrderNumber);
            var ot = new OrderTransaction(t) {RMABvin = rmaBvin};

            if (previousTransaction.Action != ActionType.CreditCardCapture
                && previousTransaction.Action != ActionType.CreditCardCharge
                && previousTransaction.Action != ActionType.CreditCardInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be CC capture or charge type to refund.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = PaymentGateways.CurrentPaymentProcessor(context.CurrentStore);
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t)
                {
                    LinkedToTransaction = previousTransaction.IdAsString,
                    RMABvin = rmaBvin
                };
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CreditCardVoid(string previousTransaction, decimal amount, string securityCode = null)
        {
            var t = FindTransactionById(previousTransaction);
            return CreditCardVoid(t, amount, securityCode);
        }

        public bool CreditCardVoid(OrderTransaction previousTransaction, decimal amount, string securityCode = null)
        {
            if (previousTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Card = previousTransaction.CreditCard;
            t.Card.SecurityCode = securityCode;
            t.Action = ActionType.CreditCardVoid;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = previousTransaction.RefNum1;
            t.PreviousTransactionAuthCode = previousTransaction.RefNum2;
            t.Items = GetLineItemsForTransaction(previousTransaction.OrderNumber);
            var ot = new OrderTransaction(t);

            if (!previousTransaction.IsVoidable)
            {
                ot.Success = false;
                ot.Messages = "Transaction can not be voided.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = PaymentGateways.CurrentPaymentProcessor(context.CurrentStore);
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = previousTransaction.IdAsString};

                // if the void went through, make sure we mark the previous transaction as voided
                if (t.Result.Succeeded)
                {
                    previousTransaction.Voided = true;
                    svc.Transactions.Update(previousTransaction);
                }
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool CreditCardCompleteAllCreditCards()
        {
            var result = true;

            var currentContext = _app.CurrentRequestContext;

            var transactions = svc.Transactions.FindForOrder(o.bvin);

            foreach (var p in transactions)
            {
                if (p.Action == ActionType.CreditCardInfo ||
                    p.Action == ActionType.CreditCardHold)
                {
                    // if we already have an auth or charge on the card, skip
                    if (p.HasSuccessfulLinkedAction(ActionType.CreditCardCharge, transactions) ||
                        p.HasSuccessfulLinkedAction(ActionType.CreditCardHold, transactions) ||
                        p.HasSuccessfulLinkedAction(ActionType.CreditCardCapture, transactions)
                        )
                    {
                        continue;
                    }

                    try
                    {
                        var t = CreateEmptyTransaction();
                        t.Card = p.CreditCard;
                        t.Amount = p.Amount;

                        if (p.Action == ActionType.CreditCardHold)
                        {
                            t.Action = ActionType.CreditCardCapture;
                        }
                        else
                        {
                            t.Action = ActionType.CreditCardCharge;
                        }
                        t.Items = GetLineItemsForTransaction(p.OrderNumber);

                        var proc = PaymentGateways.CurrentPaymentProcessor(currentContext.CurrentStore);
                        proc.ProcessTransaction(t);

                        var ot = new OrderTransaction(t);
                        ot.LinkedToTransaction = p.IdAsString;
                        svc.AddPaymentTransactionToOrder(o, ot);

                        if (t.Result.Succeeded == false) result = false;
                    }
                    catch (Exception ex)
                    {
                        EventLog.LogEvent(ex);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Public methods / Recurring Billing

        public List<OrderTransaction> RecurringSubscriptionInfoListAll()
        {
            return FindAllTransactionsOfType(ActionType.RecurringSubscriptionInfo);
        }

        public bool RecurringSubscriptionAddCardInfo(CardData card)
        {
            var t = CreateEmptyTransaction();
            t.Card = card;
            t.Action = ActionType.RecurringSubscriptionInfo;
            var ot = new OrderTransaction(t) {Success = true};
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public ResultData RecurringSubscriptionCreate(OrderTransaction infoTransaction, LineItem li)
        {
            if (infoTransaction == null)
                throw new ArgumentNullException("infoTransaction");
            if (infoTransaction.Action != ActionType.RecurringSubscriptionInfo)
                throw new ArgumentException("infoTransaction must be of a RecurringSubscriptionInfo type.");

            var t = CreateEmptyTransaction(false);
            t.Items.Add(new TransactionItem
            {
                Description = li.ProductName,
                LineTotal = li.LineTotal,
                Sku = li.ProductSku,
                IsNonShipping = li.IsNonShipping
            });
            t.Action = ActionType.RecurringSubscriptionCreate;
            t.Card = infoTransaction.CreditCard;
            t.Amount = li.LineTotalWithTaxPortion(_app.CurrentStore.Settings.ApplyVATRules);
            t.RecurringBilling.SubscriptionName = string.Format("Hcc{0}-{1}-{2}", o.OrderNumber, li.Id, li.ProductName);
            t.RecurringBilling.Interval = li.RecurringBilling.Interval;
            t.RecurringBilling.IntervalType = li.RecurringBilling.IntervalType;
            t.MerchantInvoiceNumber = string.Format("{0}-{1}", o.OrderNumber, li.Id);

            var processor = PaymentGateways.CurrentRecurringBillingProcessor(_app.CurrentStore);
            if (processor != null)
            {
                processor.ProcessTransaction(t);
            }

            var ot = new OrderTransaction(t)
            {
                LinkedToTransaction = infoTransaction.IdAsString,
                LineItemId = li.Id
            };

            svc.AddPaymentTransactionToOrder(o, ot);

            return t.Result;
        }

        public ResultData RecurringSubscriptionCancel(long lineItemId)
        {
            var li = o.GetLineItem(lineItemId);

            if (li == null)
                throw new ArgumentException("lineItemId");

            var tCreateSub =
                FindAllTransactionsOfType(ActionType.RecurringSubscriptionCreate)
                    .FirstOrDefault(s => s.LineItemId == lineItemId);

            var t = CreateEmptyTransaction(false);
            t.Items.Add(new TransactionItem
            {
                Description = li.ProductName,
                LineTotal = li.LineTotal,
                Sku = li.ProductSku,
                IsNonShipping = li.IsNonShipping
            });
            t.Action = ActionType.RecurringSubscriptionCancel;

            if (tCreateSub != null)
            {
                t.RecurringBilling.SubscriptionId = tCreateSub.RefNum1;

                var processor = PaymentGateways.CurrentRecurringBillingProcessor(_app.CurrentStore);
                if (processor != null)
                {
                    processor.ProcessTransaction(t);
                }
            }
            else
            {
                t.Result.Messages.Add(new Message("Subscription was not created or create transaction is missing",
                    "ERROR", MessageType.Error));
            }

            var ot = new OrderTransaction(t);
            if (tCreateSub != null)
                ot.LinkedToTransaction = tCreateSub.IdAsString;
            ot.LineItemId = li.Id;

            li.RecurringBilling.IsCancelled = t.Result.Succeeded;

            svc.AddPaymentTransactionToOrder(o, ot);

            return t.Result;
        }

        public ResultData RecurringSubscriptionUpdate(long lineItemId, CardData cardData)
        {
            var li = o.GetLineItem(lineItemId);

            if (li == null)
                throw new ArgumentException("lineItemId");

            var tCreateSub =
                FindAllTransactionsOfType(ActionType.RecurringSubscriptionCreate)
                    .FirstOrDefault(s => s.LineItemId == lineItemId);

            var t = CreateEmptyTransaction(false);
            t.Items.Add(new TransactionItem
            {
                Description = li.ProductName,
                LineTotal = li.LineTotal,
                Sku = li.ProductSku,
                IsNonShipping = li.IsNonShipping
            });
            t.Action = ActionType.RecurringSubscriptionUpdate;
            if (tCreateSub != null)
            {
                t.Card = cardData;
                t.Amount = tCreateSub.Amount;
                t.RecurringBilling.SubscriptionId = tCreateSub.RefNum1;

                t.RecurringBilling.Interval = li.RecurringBilling.Interval;
                t.RecurringBilling.IntervalType = li.RecurringBilling.IntervalType;

                var processor = PaymentGateways.CurrentRecurringBillingProcessor(_app.CurrentStore);
                if (processor != null)
                {
                    processor.ProcessTransaction(t);
                }
            }
            else
            {
                t.Result.Messages.Add(new Message("Subscription was not created or create transaction is missing",
                    "ERROR", MessageType.Error));
            }

            var ot = new OrderTransaction(t);
            if (tCreateSub != null)
                ot.LinkedToTransaction = tCreateSub.IdAsString;
            ot.LineItemId = li.Id;

            svc.AddPaymentTransactionToOrder(o, ot);

            return t.Result;
        }

        public bool RecurringPaymentReceive(Transaction paymentTransaction)
        {
            if (paymentTransaction == null)
                throw new ArgumentNullException("paymentTransaction");
            if (paymentTransaction.Action != ActionType.RecurringPayment)
                throw new ArgumentException("paymentTransaction must be of a RecurringPayment type.");

            var subsId = paymentTransaction.Result.ReferenceNumber;
            var tCreateSub =
                FindAllTransactionsOfType(ActionType.RecurringSubscriptionCreate)
                    .FirstOrDefault(s => s.RefNum1 == subsId);

            var ot = new OrderTransaction(paymentTransaction);
            if (tCreateSub != null)
            {
                ot.LinkedToTransaction = tCreateSub.IdAsString;
                ot.LineItemId = tCreateSub.LineItemId;
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public List<OrderTransaction> RecurringPaymentsGetByLineItem(long lineItemId)
        {
            return FindAllTransactionsOfType(ActionType.RecurringPayment)
                .Where(t => t.LineItemId == lineItemId)
                .ToList();
        }

        public OrderTransaction GetLastCreditCardTransaction()
        {
            var allTransactions = svc.Transactions.FindAllTransactionsForOrder(o.bvin);
            var lastUpdate = allTransactions.LastOrDefault(t => t.Action == ActionType.RecurringSubscriptionUpdate);
            if (lastUpdate != null)
                return lastUpdate;
            return allTransactions.LastOrDefault(t => t.Action == ActionType.RecurringSubscriptionCreate);
        }

        public long? GetLineItemBySubsription(string subscriptionId)
        {
            var tCreateSub =
                FindAllTransactionsOfType(ActionType.RecurringSubscriptionCreate)
                    .FirstOrDefault(s => s.RefNum1 == subscriptionId);
            return tCreateSub != null ? tCreateSub.LineItemId : null;
        }

        public string GetSubscriptionByLineItem(long lineItemId)
        {
            var tCreateSub =
                FindAllTransactionsOfType(ActionType.RecurringSubscriptionCreate)
                    .FirstOrDefault(s => s.LineItemId == lineItemId);
            return tCreateSub != null ? tCreateSub.RefNum1 : null;
        }

        #endregion

        #region Public methods / PayPal

        public bool PayPalExpressAddInfo(decimal amount, string token, string payerId)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.Action = ActionType.PayPalExpressCheckoutInfo;
            var ot = new OrderTransaction(t)
            {
                MethodId = PaymentMethods.PaypalExpressId,
                Success = true,
                RefNum1 = token,
                RefNum2 = payerId
            };
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool PayPalExpressHasInfo()
        {
            var result = PayPalExpressInfoListAll();
            if (result.Count > 0) return true;
            return false;
        }

        public List<OrderTransaction> PayPalExpressInfoListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.PayPalExpressCheckoutInfo)
                {
                    result.Add(t);
                }
            }
            return result;
        }

        public OrderTransaction PayPalExpressInfoFind(string id)
        {
            foreach (var t in PayPalExpressInfoListAll())
            {
                if (t.IdAsString == id)
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> PayPalExpressHoldListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.PayPalHold)
                {
                    if (!t.Voided && t.Success)
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public OrderTransaction PayPalExpressHoldFind(string id)
        {
            foreach (var t in PayPalExpressHoldListAll())
            {
                if (t.IdAsString == id)
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> PayPalExpressListAllRefundable()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.PayPalCapture ||
                    t.Action == ActionType.PayPalCharge)
                {
                    if (!t.Voided)
                    {
                        if (t.Success)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            return result;
        }

        public bool PayPalExpressHold(string infoTransactionId, decimal amount)
        {
            var t = PayPalExpressInfoFind(infoTransactionId);
            return PayPalExpressHold(t, amount);
        }

        public bool PayPalExpressHold(OrderTransaction infoTransaction, decimal amount)
        {
            var txnSuccess = false;
            if (infoTransaction == null) return txnSuccess;

            var t = CreateEmptyTransaction();
            t.Card = infoTransaction.CreditCard;
            t.Action = ActionType.PayPalHold;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = infoTransaction.RefNum1;
            t.PreviousTransactionAuthCode = infoTransaction.RefNum2;
            var ot = new OrderTransaction(t) {MethodId = PaymentMethods.PaypalExpressId};
            if (infoTransaction.Action != ActionType.PayPalExpressCheckoutInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be PayPal info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var processor = new PaypalExpress();
            if (processor != null)
            {
                var paymentAuthorized = processor.Authorize(t, _app);
                ot = new OrderTransaction(t);
                ot.Success = ot.Success && paymentAuthorized;
                ot.LinkedToTransaction = infoTransaction.IdAsString;
            }

            txnSuccess = ot.Success && svc.AddPaymentTransactionToOrder(o, ot);
            return txnSuccess;
        }

        public bool PayPalExpressCapture(string holdTransactionId, decimal amount)
        {
            var t = PayPalExpressHoldFind(holdTransactionId);
            return PayPalExpressCapture(t, amount);
        }

        public bool PayPalExpressCapture(OrderTransaction holdTransaction, decimal amount)
        {
            if (holdTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Action = ActionType.PayPalCapture;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = holdTransaction.RefNum1;
            t.PreviousTransactionAuthCode = holdTransaction.RefNum2;
            var ot = new OrderTransaction(t) {MethodId = PaymentMethods.PaypalExpressId};
            if (holdTransaction.Action != ActionType.PayPalHold)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be PayPal hold type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var processor = new PaypalExpress();
            if (processor != null)
            {
                processor.Capture(t, _app);
                ot = new OrderTransaction(t) {LinkedToTransaction = holdTransaction.IdAsString};
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool PayPalExpressCharge(string infoTransactionId, decimal amount)
        {
            var t = PayPalExpressInfoFind(infoTransactionId);
            return PayPalExpressCharge(t, amount);
        }

        public bool PayPalExpressCharge(OrderTransaction infoTransaction, decimal amount)
        {
            if (infoTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Action = ActionType.PayPalCharge;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = infoTransaction.RefNum1;
            t.PreviousTransactionAuthCode = infoTransaction.RefNum2;
            var ot = new OrderTransaction(t) {MethodId = PaymentMethods.PaypalExpressId};
            if (infoTransaction.Action != ActionType.PayPalExpressCheckoutInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be PayPal info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var processor = new PaypalExpress();
            if (processor != null)
            {
                processor.Charge(t, _app);
                ot = new OrderTransaction(t) {LinkedToTransaction = infoTransaction.IdAsString};
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool PayPalExpressRefund(string previousTransaction, decimal amount, string rmaBvin = "")
        {
            var t = FindTransactionById(previousTransaction);
            return PayPalExpressRefund(t, amount, rmaBvin);
        }

        public bool PayPalExpressRefund(OrderTransaction previousTransaction, decimal amount, string rmaBvin = "")
        {
            if (previousTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Action = ActionType.PayPalRefund;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = previousTransaction.RefNum1;
            t.PreviousTransactionAuthCode = previousTransaction.RefNum2;
            var ot = new OrderTransaction(t)
            {
                RMABvin = rmaBvin,
                MethodId = PaymentMethods.PaypalExpressId
            };
            if (previousTransaction.Action != ActionType.PayPalCapture
                && previousTransaction.Action != ActionType.PayPalCharge
                && previousTransaction.Action != ActionType.PayPalExpressCheckoutInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be PayPal capture or charge type to refund.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var processor = new PaypalExpress();
            if (processor != null)
            {
                processor.Refund(t, _app);
                ot = new OrderTransaction(t)
                {
                    RMABvin = rmaBvin,
                    LinkedToTransaction = previousTransaction.IdAsString
                };
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool PayPalExpressVoid(string previousTransaction, decimal amount)
        {
            var t = FindTransactionById(previousTransaction);
            return PayPalExpressVoid(t, amount);
        }

        public bool PayPalExpressVoid(OrderTransaction previousTransaction, decimal amount)
        {
            if (previousTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Action = ActionType.PayPalVoid;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = previousTransaction.RefNum1;
            t.PreviousTransactionAuthCode = previousTransaction.RefNum2;
            var ot = new OrderTransaction(t) {MethodId = PaymentMethods.PaypalExpressId};
            if (!previousTransaction.IsVoidable)
            {
                ot.Success = false;
                ot.Messages = "Transaction can not be voided.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var processor = new PaypalExpress();
            if (processor != null)
            {
                processor.Void(t, _app);
                ot = new OrderTransaction(t) {LinkedToTransaction = previousTransaction.IdAsString};

                // if the void went through, make sure we mark the previous transaction as voided
                if (t.Result.Succeeded)
                {
                    previousTransaction.Voided = true;
                    svc.Transactions.Update(previousTransaction);
                }
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool PayPalExpressCompleteAllPayments()
        {
            var result = true;

            var transactions = svc.Transactions.FindForOrder(o.bvin);

            foreach (var p in transactions)
            {
                if (p.Action == ActionType.PayPalExpressCheckoutInfo ||
                    p.Action == ActionType.PayPalHold)
                {
                    // if we already have an auth or charge on the card, skip
                    if (
                        p.HasSuccessfulLinkedAction(ActionType.PayPalCharge, transactions) ||
                        p.HasSuccessfulLinkedAction(ActionType.PayPalCapture, transactions) ||
                        p.HasSuccessfulLinkedAction(ActionType.PayPalHold, transactions)
                        )
                    {
                        continue;
                    }

                    try
                    {
                        var t = CreateEmptyTransaction();
                        t.Card = p.CreditCard;
                        t.Amount = p.Amount;

                        var processor = new PaypalExpress();

                        if (p.Action == ActionType.PayPalHold)
                        {
                            t.Action = ActionType.PayPalCapture;
                            processor.Capture(t, _app);
                        }
                        else
                        {
                            t.Action = ActionType.PayPalCharge;
                            processor.Charge(t, _app);
                        }

                        var ot = new OrderTransaction(t) {LinkedToTransaction = p.IdAsString};
                        svc.AddPaymentTransactionToOrder(o, ot);

                        if (t.Result.Succeeded == false) result = false;
                    }
                    catch (Exception ex)
                    {
                        EventLog.LogEvent(ex);
                    }
                }
            }

            return result;
        }

        #endregion

        #region Public methods / Rewards Points

        public string RewardsPointsAvailableDescription()
        {
            var result = "0 - " + 0.ToString("c");

            if (o.UserID != string.Empty)
            {
                var points = _pointsManager.FindAvailablePoints(o.UserID);
                result = points + " - " + _pointsManager.DollarCreditForPoints(points).ToString("c");
            }

            return result;
        }

        public bool RewardsPointsAddInfo(int points)
        {
            var t = CreateEmptyTransaction();
            t.Amount = _pointsManager.DollarCreditForPoints(EnsurePositiveAmount(points));
            t.Customer.UserId = o.UserID;
            t.RewardPoints = points;
            t.Action = ActionType.RewardPointsInfo;
            var ot = new OrderTransaction(t) {Success = true};
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        private OrderTransaction FindOrCreateRewardsInfo(string infoTransactionId)
        {
            OrderTransaction t = null;

            if (infoTransactionId == string.Empty)
            {
                if (RewardsPointsInfoListAll().Count < 1)
                {
                    RewardsPointsAddInfo(0);
                }
                var infos = RewardsPointsInfoListAll();
                t = infos[0];
            }
            else
            {
                t = RewardsPointsInfoFind(infoTransactionId);
            }

            return t;
        }

        public bool RewardsPointsHold(string infoTransactionId, int points)
        {
            var t = FindOrCreateRewardsInfo(infoTransactionId);
            return RewardsPointsHold(t, points);
        }

        public bool RewardsPointsHold(OrderTransaction infoTransaction, int points)
        {
            if (infoTransaction == null) return false;

            var rewardPoints = EnsurePositiveAmount(points);
            var t = CreateEmptyTransaction();
            t.Action = ActionType.RewardPointsHold;
            t.Amount = _pointsManager.DollarCreditForPoints(rewardPoints);
            t.RewardPoints = rewardPoints;
            var ot = new OrderTransaction(t);

            // ensure that only transactions are processed when the available points are in the customer's account
            var availablePoints = _pointsManager.FindAvailablePoints(o.UserID);
            if (availablePoints < rewardPoints)
            {
                ot.Success = false;
                ot.Messages = string.Format("Customer only has {0} points available and cannot hold {1} points.",
                    availablePoints, rewardPoints);
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (rewardPoints == 0)
            {
                ot.Success = false;
                ot.Messages = "Cannot hold 0 points.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (infoTransaction.Action != ActionType.RewardPointsInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Rewards Points info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (_pointsManager.HoldPoints(o.UserID, rewardPoints))
            {
                ot.Success = true;
                ot.Messages = t.RewardPoints + " Points Held";
            }
            else
            {
                ot.Success = false;
                ot.Messages = "Not enough points available to hold";
            }
            ot.LinkedToTransaction = infoTransaction.IdAsString;

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool RewardsPointsUnHold(string holdTransactionId, int points)
        {
            var t = RewardsPointsHoldFind(holdTransactionId);
            return RewardsPointsUnHold(t, points);
        }

        public bool RewardsPointsUnHold(OrderTransaction holdTransaction, int points)
        {
            if (holdTransaction == null) return false;

            var rewardPoints = EnsurePositiveAmount(points);
            var t = CreateEmptyTransaction();
            t.Action = ActionType.RewardPointsUnHold;
            t.Amount = _pointsManager.DollarCreditForPoints(rewardPoints);
            t.RewardPoints = rewardPoints;
            var ot = new OrderTransaction(t);

            if (holdTransaction.Action != ActionType.RewardPointsHold)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Rewards Points Hold type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (_pointsManager.UnHoldPoints(o.UserID, rewardPoints))
            {
                ot.Success = true;
                ot.Messages = t.RewardPoints + " Points Held";

                holdTransaction.Voided = true;
                svc.Transactions.Update(holdTransaction);
            }
            else
            {
                ot.Success = false;
                ot.Messages = "Not enought points held to unhold";
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public List<OrderTransaction> RewardsPointsInfoListAll()
        {
            return FindAllTransactionsOfType(ActionType.RewardPointsInfo);
        }

        public OrderTransaction RewardsPointsInfoFind(string id)
        {
            return FindSingleTransactionByTypeAndId(ActionType.RewardPointsInfo, id);
        }

        public List<OrderTransaction> RewardsPointsHoldListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.RewardPointsHold)
                {
                    if (!t.Voided && t.Success)
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public OrderTransaction RewardsPointsHoldFind(string id)
        {
            foreach (var t in RewardsPointsHoldListAll())
            {
                if (t.IdAsString == id)
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> RewardsPointsListAllRefundable()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.RewardPointsCapture ||
                    t.Action == ActionType.RewardPointsDecrease)
                {
                    if (!t.Voided)
                    {
                        if (t.Success)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            return result;
        }

        public bool RewardsPointsCapture(string holdTransactionId, int points)
        {
            var t = RewardsPointsHoldFind(holdTransactionId);
            return RewardsPointsCapture(t, points);
        }

        public bool RewardsPointsCapture(OrderTransaction holdTransaction, int points)
        {
            if (holdTransaction == null) return false;

            var rewardPoints = EnsurePositiveAmount(points);
            var t = CreateEmptyTransaction();
            t.Action = ActionType.RewardPointsCapture;
            t.Amount = _pointsManager.DollarCreditForPoints(rewardPoints);
            t.RewardPoints = rewardPoints;
            var ot = new OrderTransaction(t);

            if (holdTransaction.Action != ActionType.RewardPointsHold)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Rewards Points hold type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (_pointsManager.CapturePoints(o.UserID, rewardPoints))
            {
                ot.Success = true;
                ot.Messages = t.RewardPoints + " Points Captured";
            }
            else
            {
                ot.Success = false;
                ot.Messages = "Not enough points available to capture";
            }
            ot.LinkedToTransaction = holdTransaction.IdAsString;

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool RewardsPointsCharge(string infoTransactionId, int points)
        {
            var t = FindOrCreateRewardsInfo(infoTransactionId);
            return RewardsPointsCharge(t, points);
        }

        public bool RewardsPointsCharge(OrderTransaction infoTransaction, int points)
        {
            if (infoTransaction == null) return false;

            var rewardPoints = EnsurePositiveAmount(points);
            var t = CreateEmptyTransaction();
            t.Action = ActionType.RewardPointsDecrease;
            t.Amount = _pointsManager.DollarCreditForPoints(rewardPoints);
            var ot = new OrderTransaction(t);

            // ensure that only transactions are processed when the available points are in the customer's account
            var availablePoints = _pointsManager.FindAvailablePoints(o.UserID);
            if (availablePoints < rewardPoints)
            {
                ot.Success = false;
                ot.Messages = string.Format("Customer only has {0} points available and can't spend {1} points.",
                    availablePoints, rewardPoints);
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (rewardPoints == 0)
            {
                ot.Success = false;
                ot.Messages = "Cannot spend 0 points.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (infoTransaction.Action != ActionType.RewardPointsInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Rewards Points info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (_pointsManager.DecreasePoints(o.UserID, rewardPoints))
            {
                ot.Success = true;
                ot.Messages = rewardPoints + " Points Used";
            }
            else
            {
                ot.Success = false;
                ot.Messages = "Unable to Use Points";
            }
            ot.LinkedToTransaction = infoTransaction.IdAsString;

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool RewardsPointsRefund(string previousTransaction, int points, string rmaBvin = "")
        {
            var t = FindTransactionById(previousTransaction);
            return RewardsPointsRefund(t, points, rmaBvin);
        }

        public bool RewardsPointsRefund(OrderTransaction previousTransaction, int points, string rmaBvin = "")
        {
            if (previousTransaction == null) return false;

            var rewardPoints = EnsurePositiveAmount(points);
            var t = CreateEmptyTransaction();
            t.Action = ActionType.RewardPointsIncrease;
            t.Amount = _pointsManager.DollarCreditForPoints(rewardPoints);
            var ot = new OrderTransaction(t) {RMABvin = rmaBvin};

            if (previousTransaction.Action != ActionType.RewardPointsCapture
                && previousTransaction.Action != ActionType.RewardPointsDecrease
                && previousTransaction.Action != ActionType.RewardPointsInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Rewards Points capture or charge type to refund.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            if (_pointsManager.IssuePoints(o.UserID, rewardPoints))
            {
                ot.Success = true;
                ot.Messages = rewardPoints + " Points Issued";
            }
            else
            {
                ot.Success = false;
                ot.Messages = "Unable to Issue Points";
            }
            ot.LinkedToTransaction = previousTransaction.IdAsString;

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        #endregion

        #region Public methods / Gift Cards

        /// <summary>
        ///     Adds gift card "Info Transaction".
        /// </summary>
        /// <param name="giftCard">The gift card.</param>
        /// <param name="amount">The gift card amount.</param>
        /// <returns></returns>
        public bool GiftCardAddInfo(GiftCardData giftCard, decimal amount)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.GiftCard = giftCard;
            t.Action = ActionType.GiftCardInfo;
            var ot = new OrderTransaction(t) {Success = true};
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        /// <summary>
        ///     Updates gift card "Info Transaction".
        /// </summary>
        /// <param name="infoTransaction">The information transaction.</param>
        /// <param name="amount">The gift card amount.</param>
        /// <returns></returns>
        public bool GiftCardUpdateInfo(OrderTransaction infoTransaction, decimal amount)
        {
            infoTransaction.Amount = amount;
            return svc.Transactions.Update(infoTransaction);
        }

        /// <summary>
        ///     Removes gift card "Info Transaction".
        /// </summary>
        /// <param name="giftCardNumber">The gift card number.</param>
        /// <returns></returns>
        public bool GiftCardRemoveInfo(string giftCardNumber)
        {
            var ot = GiftCardInfoFindByNumber(giftCardNumber);

            if (ot != null)
            {
                return svc.Transactions.Delete(ot.Id);
            }

            return false;
        }

        /// <summary>
        ///     Holds specified gift card amount.
        /// </summary>
        /// <param name="infoTransactionId">The information transaction identifier.</param>
        /// <param name="amount">The amount to hold.</param>
        /// <returns></returns>
        public bool GiftCardHold(string infoTransactionId, decimal amount)
        {
            var t = GiftCardInfoFind(infoTransactionId);
            return GiftCardHold(t, amount);
        }

        public bool GiftCardHold(OrderTransaction infoTransaction, decimal amount)
        {
            if (infoTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.GiftCard = infoTransaction.GiftCard;
            t.Action = ActionType.GiftCardHold;
            t.Amount = EnsurePositiveAmount(amount);
            var ot = new OrderTransaction(t);

            if (infoTransaction.Action != ActionType.GiftCardInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Gift Card info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = context.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = infoTransaction.IdAsString};
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public List<OrderTransaction> GiftCardInfoListAll()
        {
            return FindAllTransactionsOfType(ActionType.GiftCardInfo);
        }

        public OrderTransaction GiftCardInfoFind(string id)
        {
            return FindSingleTransactionByTypeAndId(ActionType.GiftCardInfo, id);
        }

        public OrderTransaction GiftCardInfoFindByNumber(string giftCardNumber)
        {
            if (string.IsNullOrEmpty(giftCardNumber))
                return null;

            giftCardNumber = giftCardNumber.Trim().ToLower();

            return FindAllTransactionsOfType(ActionType.GiftCardInfo)
                .FirstOrDefault(ot => ot.GiftCard.CardNumber.ToLower() == giftCardNumber);
        }

        public List<OrderTransaction> GiftCardHoldListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.GiftCardHold)
                {
                    if (!t.Voided && t.Success)
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public List<OrderTransaction> GiftCardChargeOrCaptureListAll()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.GiftCardCapture || t.Action == ActionType.GiftCardDecrease)
                {
                    if (!t.Voided && t.Success)
                    {
                        result.Add(t);
                    }
                }
            }
            return result;
        }

        public OrderTransaction GiftCardHoldFind(string id)
        {
            foreach (var t in GiftCardHoldListAll())
            {
                if (t.IdAsString == id)
                {
                    return t;
                }
            }
            return null;
        }

        public List<OrderTransaction> GiftCardChargeListAllRefundable()
        {
            var result = new List<OrderTransaction>();

            foreach (var t in svc.Transactions.FindForOrder(o.bvin))
            {
                if (t.Action == ActionType.GiftCardCapture ||
                    t.Action == ActionType.GiftCardDecrease)
                {
                    if (!t.Voided)
                    {
                        if (t.Success)
                        {
                            result.Add(t);
                        }
                    }
                }
            }
            return result;
        }

        public bool GiftCardCapture(string holdTransactionId, decimal amount)
        {
            var t = GiftCardHoldFind(holdTransactionId);
            return GiftCardCapture(t, amount);
        }

        public bool GiftCardCapture(OrderTransaction holdTransaction, decimal amount)
        {
            if (holdTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.GiftCard = holdTransaction.GiftCard;
            t.Action = ActionType.GiftCardCapture;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = holdTransaction.RefNum1;
            t.PreviousTransactionAuthCode = holdTransaction.RefNum2;
            var ot = new OrderTransaction(t);

            if (holdTransaction.Action != ActionType.GiftCardHold)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Gift Card hold type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = context.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = holdTransaction.IdAsString};
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool GiftCardUnHold(string previousTransaction, decimal amount)
        {
            var t = FindTransactionById(previousTransaction);
            return GiftCardUnHold(t, amount);
        }

        private bool GiftCardUnHold(OrderTransaction previousTransaction, decimal amount)
        {
            if (previousTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.Action = ActionType.GiftCardUnHold;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = previousTransaction.RefNum1;
            t.PreviousTransactionAuthCode = previousTransaction.RefNum2;
            var ot = new OrderTransaction(t);

            if (!previousTransaction.IsVoidable)
            {
                ot.Success = false;
                ot.Messages = "Transaction can not be voided.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = context.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = previousTransaction.IdAsString};

                // if the void went through, make sure we mark the previous transaction as voided
                if (t.Result.Succeeded)
                {
                    previousTransaction.Voided = true;
                    svc.Transactions.Update(previousTransaction);
                }
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool GiftCardDecreaseWithCard(string newCardNumber, decimal amount)
        {
            var t = CreateEmptyTransaction();
            t.Amount = EnsurePositiveAmount(amount);
            t.GiftCard.CardNumber = newCardNumber;
            t.Action = ActionType.GiftCardInfo;
            var ot = new OrderTransaction(t) {Success = true};
            if (svc.AddPaymentTransactionToOrder(o, ot))
            {
                return GiftCardDecrease(ot, amount);
            }
            return false;
        }

        public bool GiftCardDecrease(string infoTransactionId, decimal amount)
        {
            var t = GiftCardInfoFind(infoTransactionId);
            return GiftCardDecrease(t, amount);
        }

        public bool GiftCardDecrease(OrderTransaction infoTransaction, decimal amount)
        {
            if (infoTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.GiftCard = infoTransaction.GiftCard;
            t.Action = ActionType.GiftCardDecrease;
            t.Amount = EnsurePositiveAmount(amount);
            var ot = new OrderTransaction(t);

            if (infoTransaction.Action != ActionType.GiftCardInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Gift Card info type to process.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = context.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t) {LinkedToTransaction = infoTransaction.IdAsString};
            }

            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool GiftCardIncrease(string previousTransaction, decimal amount, string rmaBvin = "")
        {
            var t = FindTransactionById(previousTransaction);
            return GiftCardIncrease(t, amount, rmaBvin);
        }

        public bool GiftCardIncrease(OrderTransaction previousTransaction, decimal amount, string rmaBvin = "")
        {
            if (previousTransaction == null) return false;

            var t = CreateEmptyTransaction();
            t.GiftCard = previousTransaction.GiftCard;
            t.Action = ActionType.GiftCardIncrease;
            t.Amount = EnsurePositiveAmount(amount);
            t.PreviousTransactionNumber = previousTransaction.RefNum1;
            t.PreviousTransactionAuthCode = previousTransaction.RefNum2;
            var ot = new OrderTransaction(t) {RMABvin = rmaBvin};

            if (previousTransaction.Action != ActionType.GiftCardCapture
                && previousTransaction.Action != ActionType.GiftCardDecrease
                && previousTransaction.Action != ActionType.GiftCardInfo)
            {
                ot.Success = false;
                ot.Messages = "Transaction must be Gift Card capture, increase or info type to increase.";
                return svc.AddPaymentTransactionToOrder(o, ot);
            }

            var context = _app.CurrentRequestContext;
            var processor = context.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
            if (processor != null)
            {
                processor.ProcessTransaction(t);
                ot = new OrderTransaction(t)
                {
                    RMABvin = rmaBvin,
                    LinkedToTransaction = previousTransaction.IdAsString
                };
            }
            return svc.AddPaymentTransactionToOrder(o, ot);
        }

        public bool GiftCardCompleteAllGiftCards()
        {
            var result = true;

            var currentContext = _app.CurrentRequestContext;

            var transactions = svc.Transactions.FindForOrder(o.bvin);

            foreach (var p in transactions)
            {
                if (p.Action == ActionType.GiftCardHold)
                {
                    // if we already have an auth or charge on the card, skip
                    if (p.HasSuccessfulLinkedAction(ActionType.GiftCardDecrease, transactions) ||
                        p.HasSuccessfulLinkedAction(ActionType.GiftCardCapture, transactions)
                        )
                    {
                        continue;
                    }

                    try
                    {
                        var t = CreateEmptyTransaction();
                        t.Card = p.CreditCard;
                        t.GiftCard = p.GiftCard;
                        t.Amount = p.Amount;

                        t.Action = ActionType.GiftCardCapture;

                        var proc = currentContext.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();
                        proc.ProcessTransaction(t);

                        var ot = new OrderTransaction(t);
                        ot.LinkedToTransaction = p.IdAsString;
                        svc.AddPaymentTransactionToOrder(o, ot);

                        if (t.Result.Succeeded == false) result = false;
                    }
                    catch (Exception ex)
                    {
                        EventLog.LogEvent(ex);
                    }
                }
            }

            return result;
        }

        public string GiftCardCreate(GiftCardData giftCard)
        {
            var t = CreateEmptyTransaction();
            t.Action = ActionType.GiftCardCreateNew;
            t.GiftCard = giftCard;
            var ot = new OrderTransaction(t);

            var context = _app.CurrentRequestContext;
            var processor = context.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();

            if (processor != null)
            {
                processor.ProcessTransaction(t);
            }

            var number = string.Empty;
            if (t.Result.Succeeded)
            {
                number = ot.GiftCard.CardNumber;
            }
            return number;
        }

        public bool GiftCardActivate(string giftCardNumber)
        {
            return GiftCardCallGatewayAction(giftCardNumber, ActionType.GiftCardActivate);
        }

        public bool GiftCardDeactivate(string giftCardNumber)
        {
            return GiftCardCallGatewayAction(giftCardNumber, ActionType.GiftCardDeactivate);
        }

        private bool GiftCardCallGatewayAction(string giftCardNumber, ActionType aType)
        {
            var t = CreateEmptyTransaction();
            t.Action = aType;
            t.GiftCard.CardNumber = giftCardNumber;

            var processor = _app.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();

            if (processor != null)
            {
                processor.ProcessTransaction(t);
            }

            return t.Result.Succeeded;
        }

        public GiftCardBalanceResponse GiftCardBalanceInquiry(string giftCardNumber)
        {
            var response = new GiftCardBalanceResponse();

            var t = CreateEmptyTransaction();
            t.Action = ActionType.GiftCardBalanceInquiry;
            t.GiftCard.CardNumber = giftCardNumber;

            var processor = _app.CurrentStore.Settings.PaymentCurrentGiftCardProcessor();

            processor.ProcessTransaction(t);
            foreach (var m in t.Result.Messages)
            {
                if (m.Severity == MessageType.Warning ||
                    m.Severity == MessageType.Error)
                {
                    response.Messages.Add(m);
                }
            }
            response.Success = t.Result.Succeeded;
            response.CurrentValue = t.Result.BalanceAvailable;

            return response;
        }

        private List<TransactionItem> GetLineItemsForTransaction(string orderNumber)
        {
            var repo = new OrderRepository(_app.CurrentRequestContext);
            var order = repo.FindByOrderNumber(orderNumber);

            if (order != null && order.OrderNumber == orderNumber)
            {
                var items = new List<TransactionItem>();

                foreach(var item in order.Items)
                {
                    items.Add(new TransactionItem
                    {
                        Sku = item.ProductSku,
                        LineTotal = item.LineTotal,
                        Description = item.ProductShortDescription,
                        IsNonShipping = item.IsNonShipping
                    });
                }

                return items;
            }

            return null;
        }

        #endregion

        #region Implementation

        private List<OrderTransaction> FindAllTransactionsOfType(ActionType type)
        {
            return svc.Transactions.FindForOrder(o.bvin, type);
        }

        private OrderTransaction FindSingleTransactionByTypeAndId(ActionType type, string Id)
        {
            foreach (var t in svc.Transactions.FindForOrder(o.bvin, type))
            {
                if (t.IdAsString == Id) return t;
            }
            return null;
        }

        private decimal EnsurePositiveAmount(decimal input)
        {
            if (input < 0)
            {
                return input*-1;
            }
            return input;
        }

        private int EnsurePositiveAmount(int input)
        {
            if (input < 0)
            {
                return input*-1;
            }
            return input;
        }

        #endregion
    }
}