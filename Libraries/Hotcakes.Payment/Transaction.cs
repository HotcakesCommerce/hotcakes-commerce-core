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

namespace Hotcakes.Payment
{
    /// <summary>
    /// Represents a payment operation and its inputs, related data, and result.
    /// </summary>
    [Serializable]
    public class Transaction
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="Transaction"/> class with default payment data objects and collections.
        /// </summary>
        public Transaction()
        {
            Id = new Guid();
            Amount = 0m;
            RewardPoints = 0;
            Action = ActionType.Unknown;
            RecurringBilling = new RecurringData();
            GiftCard = new GiftCardData();
            Card = new CardData();
            Customer = new CustomerData();
            PreviousTransactionNumber = string.Empty;
            PreviousTransactionAuthCode = string.Empty;
            MerchantDescription = string.Empty;
            MerchantInvoiceNumber = string.Empty;
            Result = new ResultData();
            CheckNumber = string.Empty;
            PurchaseOrderNumber = string.Empty;
            CompanyAccountNumber = string.Empty;
            Items = new List<TransactionItem>();
            AdditionalSettings = new Dictionary<string, string>();
        }

        /// <summary>
        ///     The unique identifier of current transaction
        /// </summary>
        /// <value>
        ///     The identifier.
        /// </value>
        public Guid Id { get; set; }

        /// <summary>
        ///     The currency value of this transaction operation.
        /// </summary>
        /// <value>
        ///     The amount.
        /// </value>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the reward points associated with this transaction.</summary>
        public int RewardPoints { get; set; }

        /// <summary>
        ///     Identifies transaction type that have to be performed.
        /// </summary>
        /// <value>
        ///     The action.
        /// </value>
        public ActionType Action { get; set; }

        /// <summary>
        ///     The customer from whose name transaction is initiated.
        /// </summary>
        /// <value>
        ///     The customer.
        /// </value>
        public CustomerData Customer { get; set; }

        /// <summary>
        ///     The additional transaction settings.
        /// </summary>
        /// <value>
        ///     The additional settings.
        /// </value>
        public Dictionary<string, string> AdditionalSettings { get; set; }

        /// <summary>Gets or sets the number of the transaction being referenced by this operation.</summary>
        public string PreviousTransactionNumber { get; set; }
        /// <summary>Gets or sets the authorization code of the transaction being referenced.</summary>
        public string PreviousTransactionAuthCode { get; set; }
        /// <summary>Gets or sets the description supplied to the merchant or payment provider.</summary>
        public string MerchantDescription { get; set; }
        /// <summary>Gets or sets the merchant invoice number associated with this transaction.</summary>
        public string MerchantInvoiceNumber { get; set; }

        /// <summary>Gets or sets the recurring billing details for this transaction.</summary>
        /// <seealso cref="RecurringData"/>
        public RecurringData RecurringBilling { get; set; }

        /// <summary>
        ///     The gift card data in case gift card transaction is executed.
        /// </summary>
        /// <value>
        ///     The card.
        /// </value>
        public GiftCardData GiftCard { get; set; }

        /// <summary>
        ///     The card data in case payment is done by credit card.
        /// </summary>
        /// <value>
        ///     The card.
        /// </value>
        public CardData Card { get; set; }

        /// <summary>
        ///     The check number when payment by check is selected.
        /// </summary>
        /// <value>
        ///     The check number.
        /// </value>
        public string CheckNumber { get; set; }

        /// <summary>
        ///     The purchase order number that is used when purchse order payment method is used.
        /// </summary>
        /// <value>
        ///     The purchase order number.
        /// </value>
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        ///     The company account number that can be used when purchse is done on company account.
        /// </summary>
        /// <value>
        ///     The company account number.
        /// </value>
        public string CompanyAccountNumber { get; set; }

        /// <summary>
        ///     The line items data related to this transaction.
        /// </summary>
        /// <value>
        ///     The items.
        /// </value>
        public List<TransactionItem> Items { get; set; }

        /// <summary>
        ///     Execution result of specified transaction.
        /// </summary>
        /// <value>
        ///     The result.
        /// </value>
        public ResultData Result { get; set; }

        /// <summary>
        ///     Identifies whether this instance is refund transaction.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this transaction refunds money; otherwise, <c>false</c>.
        /// </value>
        public bool IsRefundTransaction
        {
            get
            {
                if (Action == ActionType.CreditCardRefund ||
                    Action == ActionType.CashReturned ||
                    Action == ActionType.CheckReturned ||
                    Action == ActionType.GiftCardIncrease ||
                    Action == ActionType.RewardPointsIncrease ||
                    Action == ActionType.PayPalRefund ||
                    Action == ActionType.ThirdPartyPayMethodRefund)
                {
                    return true;
                }

                return false;
            }
        }

        #region Public methods

        /// <summary>Sets an additional transaction setting, replacing any existing value for the key.</summary>
        /// <param name="key">The setting key.</param>
        /// <param name="value">The setting value.</param>
        /// <seealso cref="GetAdditionalSettingAsString(string)"/>
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

        /// <summary>Gets an additional transaction setting as a string.</summary>
        /// <param name="key">The setting key.</param>
        /// <returns>The setting value, or an empty string when the key is not present.</returns>
        /// <seealso cref="SetAdditionalSetting(string, string)"/>
        public string GetAdditionalSettingAsString(string key)
        {
            var result = string.Empty;
            if (AdditionalSettings.ContainsKey(key))
            {
                result = AdditionalSettings[key];
            }
            return result;
        }

        /// <summary>Gets an additional transaction setting parsed as a decimal.</summary>
        /// <param name="key">The setting key.</param>
        /// <returns>The parsed decimal value, or <c>-1</c> when the value is missing or cannot be parsed.</returns>
        public decimal GetAdditionalSettingAsDecimal(string key)
        {
            decimal result = -1;

            var s = GetAdditionalSettingAsString(key);
            decimal.TryParse(s, out result);

            return result;
        }

        /// <summary>Gets an additional transaction setting parsed as an integer.</summary>
        /// <param name="key">The setting key.</param>
        /// <returns>The parsed integer value, or <c>-1</c> when the value is missing or cannot be parsed.</returns>
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
