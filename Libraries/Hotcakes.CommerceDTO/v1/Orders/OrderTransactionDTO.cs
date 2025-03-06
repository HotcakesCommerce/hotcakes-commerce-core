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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of transactions in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is OrderTransaction.</remarks>
    [DataContract]
    [Serializable]
    public class OrderTransactionDTO
    {
        public OrderTransactionDTO()
        {
            Action = OrderTransactionActionDTO.Uknown;
            Amount = 0;
            CheckNumber = string.Empty;
            CompanyAccountNumber = string.Empty;
            GiftCard = new OrderTransactionGiftCardDataDTO();
            CreditCard = new OrderTransactionCardDataDTO();
            Id = new Guid();
            LinkedToTransaction = string.Empty;
            Messages = string.Empty;
            OrderId = string.Empty;
            OrderNumber = string.Empty;
            PurchaseOrderNumber = string.Empty;
            RefNum1 = string.Empty;
            RefNum2 = string.Empty;
            StoreId = 0;
            Success = false;
            TimeStampUtc = DateTime.UtcNow;
            Voided = false;
        }

        /// <summary>
        ///     The unique ID of the transaction.
        /// </summary>
        [DataMember]
        public Guid Id { get; set; }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        [DataMember]
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the order that this transaction belongs to.
        /// </summary>
        [DataMember]
        public string OrderId { get; set; }

        /// <summary>
        ///     The display number of the order that this transaction belongs to.
        /// </summary>
        [DataMember]
        public string OrderNumber { get; set; }

        /// <summary>
        ///     A date/time stamp used for auditing purposes to know when the transaction was created.
        /// </summary>
        [DataMember]
        public DateTime TimeStampUtc { get; set; }

        /// <summary>
        ///     Defines what kind of transaction this is.
        /// </summary>
        [DataMember]
        public OrderTransactionActionDTO Action { get; set; }

        /// <summary>
        ///     The total amount of the transaction.
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        ///     Information to define the associated gift card, when necessary.
        /// </summary>
        [DataMember]
        public OrderTransactionGiftCardDataDTO GiftCard { get; set; }

        /// <summary>
        ///     Information to define the associated credit card, when necessary.
        /// </summary>
        [DataMember]
        public OrderTransactionCardDataDTO CreditCard { get; set; }

        /// <summary>
        ///     Defines whether the transaction was successful or not.
        /// </summary>
        [DataMember]
        public bool Success { get; set; }

        /// <summary>
        ///     Notes that this transaction is a void for another transaction.
        /// </summary>
        [DataMember]
        public bool Voided { get; set; }

        /// <summary>
        ///     The primary reference number or token returned by the payment processor.
        /// </summary>
        [DataMember]
        public string RefNum1 { get; set; }

        /// <summary>
        ///     The secondary reference number or token returned by the payment processor.
        /// </summary>
        [DataMember]
        public string RefNum2 { get; set; }

        /// <summary>
        ///     When necessary, the unique ID of another transaction that this one is linked to, such as a credit or gift card
        ///     payment.
        /// </summary>
        [DataMember]
        public string LinkedToTransaction { get; set; }

        /// <summary>
        ///     Useful information about the transaction, such as errors or warnings.
        /// </summary>
        [DataMember]
        public string Messages { get; set; }

        /// <summary>
        ///     When the transaction is a check payment, the check number should be here.
        /// </summary>
        [DataMember]
        public string CheckNumber { get; set; }

        /// <summary>
        ///     When the transaction is a purchase order payment, the PO number should be here.
        /// </summary>
        [DataMember]
        public string PurchaseOrderNumber { get; set; }

        /// <summary>
        ///     When the transaction uses company accounts for payment, the company's account number should be here.
        /// </summary>
        [DataMember]
        public string CompanyAccountNumber { get; set; }
    }
}