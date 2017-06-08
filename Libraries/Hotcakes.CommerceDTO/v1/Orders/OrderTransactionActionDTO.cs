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

using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     Various actions that a transaction can be.
    /// </summary>
    [DataContract]
    public enum OrderTransactionActionDTO
    {
        /// <summary>
        ///     Unknown - should not be used.
        /// </summary>
        [EnumMember] Uknown = 0,

        /// <summary>
        ///     A transaction that does nothing but store CC information on the server
        /// </summary>
        [EnumMember] CreditCardInfo = 1,

        /// <summary>
        ///     Holds funds as an authorization against future capture
        /// </summary>
        [EnumMember] CreditCardHold = 100,

        /// <summary>
        ///     Captured funds previously held/authorized
        /// </summary>
        [EnumMember] CreditCardCapture = 101,

        /// <summary>
        ///     A hold + capture or charge in a single step
        /// </summary>
        [EnumMember] CreditCardCharge = 102,

        /// <summary>
        ///     Refunds money to the CC
        /// </summary>
        [EnumMember] CreditCardRefund = 103,

        /// <summary>
        ///     Voids a previous transaction (usually only works before batch is settled)
        /// </summary>
        [EnumMember] CreditCardVoid = 104,

        /// <summary>
        ///     Receive Payment as a Check
        /// </summary>
        [EnumMember] CheckReceived = 201,

        /// <summary>
        ///     Send Payment as a Check
        /// </summary>
        [EnumMember] CheckReturned = 202,

        /// <summary>
        ///     Receive a Payment as Cash
        /// </summary>
        [EnumMember] CashReceived = 301,

        /// <summary>
        ///     Return Cash
        /// </summary>
        [EnumMember] CashReturned = 302,

        /// <summary>
        ///     Purchase Order Number Info Stored
        /// </summary>
        [EnumMember] PurchaseOrderInfo = 401,

        /// <summary>
        ///     Consider PO as Valid Payment
        /// </summary>
        [EnumMember] PurchaseOrderAccepted = 402,

        /// <summary>
        ///     Company Account Number Saved
        /// </summary>
        [EnumMember] CompanyAccountInfo = 450,

        /// <summary>
        ///     Company Account Number accepted as payment
        /// </summary>
        [EnumMember] CompanyAccountAccepted = 451,

        /// <summary>
        ///     Record Gift Card Information
        /// </summary>
        [EnumMember] GiftCardInfo = 501,

        /// <summary>
        ///     Hold a Specific Amount on a Gift Card
        /// </summary>
        [EnumMember] GiftCardHold = 502,

        /// <summary>
        ///     Release previous hold on gift card amount
        /// </summary>
        GiftCardUnHold = 508,

        /// <summary>
        ///     Capture a Held Amount on a Gift Card
        /// </summary>
        [EnumMember] GiftCardCapture = 503,

        /// <summary>
        ///     Reduce Value of Gift Card
        /// </summary>
        [EnumMember] GiftCardDecrease = 504,

        /// <summary>
        ///     Increase Value of Gift Card
        /// </summary>
        [EnumMember] GiftCardIncrease = 505,

        /// <summary>
        ///     Create a new Gift Card with an initial amount
        /// </summary>
        GiftCardCreateNew = 509,

        /// <summary>
        ///     Activate a Gift Card Number
        /// </summary>
        [EnumMember] GiftCardActivate = 506,

        /// <summary>
        ///     Find the current balance of a gift card
        /// </summary>
        [EnumMember] GiftCardBalanceInquiry = 507,

        /// <summary>
        ///     Record Reward Points Information
        /// </summary>
        [EnumMember] RewardPointsInfo = 551,

        /// <summary>
        ///     Hold a Specific Amount of Reward Points
        /// </summary>
        [EnumMember] RewardPointsHold = 552,

        /// <summary>
        ///     Capture a Held Amount of Reward Points
        /// </summary>
        [EnumMember] RewardPointsCapture = 553,

        /// <summary>
        ///     Reduce Points Available
        /// </summary>
        [EnumMember] RewardPointsDecrease = 554,

        /// <summary>
        ///     Increase Points Available
        /// </summary>
        [EnumMember] RewardPointsIncrease = 555,

        /// <summary>
        ///     Find the current balance of points available
        /// </summary>
        [EnumMember] RewardPointsBalanceInquiry = 557,

        /// <summary>
        ///     Un-hold a Specific Amount of Reward Points
        /// </summary>
        [EnumMember] RewardPointsUnHold = 558,

        /// <summary>
        ///     Funds Held at PayPal
        /// </summary>
        [EnumMember] PayPalHold = 601,

        /// <summary>
        ///     Capture previously held Funds
        /// </summary>
        [EnumMember] PayPalCapture = 602,

        /// <summary>
        ///     Charge a PayPal Account
        /// </summary>
        [EnumMember] PayPalCharge = 603,

        /// <summary>
        ///     Send Money to a PayPal Account
        /// </summary>
        [EnumMember] PayPalRefund = 604,

        /// <summary>
        ///     Void a Pending Transaction
        /// </summary>
        [EnumMember] PayPalVoid = 605,

        /// <summary>
        ///     Customer Requests PayPal Express Checkout
        /// </summary>
        [EnumMember] PayPalExpressCheckoutInfo = 606,

        /// <summary>
        ///     Records customer request to pay offline
        /// </summary>
        [EnumMember] OfflinePaymentRequest = 9999
    }
}