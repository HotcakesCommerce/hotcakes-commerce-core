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
using Hotcakes.Commerce.Payment;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Models
{
    /// <summary>
    ///     The CheckoutPaymentViewModel is used for the checkout view that customers use
    ///     to complete their purchase.
    /// </summary>
    [Serializable]
    public class CheckoutPaymentViewModel
    {
        /// <summary>
        ///     Set default values.
        /// </summary>
        public CheckoutPaymentViewModel()
        {
            PaymentMethods = new List<PaymentMethod>();

            NoPaymentNeeded = false;

            DataCompanyAccountNumber = string.Empty;
            DataCreditCard = new CardData();
            DataPurchaseOrderNumber = string.Empty;
            AcceptedCardTypes = new List<CardType>();
        }

        /// <summary>
        ///     If order total is zero and order is not recurring then this is set as true.
        /// </summary>
        public bool NoPaymentNeeded { get; set; }

        /// <summary>
        ///     Payment method that was selected by the customer. Customers may only choose and use one.
        /// </summary>
        public string SelectedMethodId { get; set; }

        /// <summary>
        ///     List of enabled payment methods availalbe to use during checkout.
        /// </summary>
        public List<PaymentMethod> PaymentMethods { get; set; }

        /// <summary>
        ///     If the checkout uses the "Purchase Order" payment method, this should contain
        ///     a purchase order (PO) number.
        /// </summary>
        public string DataPurchaseOrderNumber { get; set; }

        /// <summary>
        ///     If an order uses "Company Account" as payment method, this will have a value.
        /// </summary>
        public string DataCompanyAccountNumber { get; set; }

        /// <summary>
        ///     Stores PA-DSS compliant information about the credit card whichever entered by
        ///     customer during checkout.
        /// </summary>
        public CardData DataCreditCard { get; set; }

        /// <summary>
        ///     Contains the acceptable cart types for payment on checkout, as defined in the
        ///     administration area.
        /// </summary>
        public List<CardType> AcceptedCardTypes { get; set; }
    }
}