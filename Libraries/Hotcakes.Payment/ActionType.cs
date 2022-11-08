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

namespace Hotcakes.Payment
{
	/// <summary>
    ///     Various actions that a transaction can be.
	/// </summary>
	public enum ActionType
	{
		/// <summary>
        ///     Unknown - should not be used.
		/// </summary>
		Unknown = 0,

		/// <summary>
        ///     A transaction that does nothing but store CC information on the server
		/// </summary>
		CreditCardInfo = 1,

		/// <summary>
        ///     Holds funds as an authorization against future capture
		/// </summary>
		CreditCardHold = 100,

		/// <summary>
        ///     Captured funds previously held/authorized
		/// </summary>
		CreditCardCapture = 101,

		/// <summary>
        ///     A hold + capture or charge in a single step
		/// </summary>
		CreditCardCharge = 102,

		/// <summary>
        ///     Refunds money to the CC
		/// </summary>
		CreditCardRefund = 103,

		/// <summary>
        ///     Voids a previous transaction (usually only works before batch is settled)
		/// </summary>
		CreditCardVoid = 104,

		/// <summary>
		/// A Transaction that is ignored by Payment gateway even if everything is correct
		/// </summary>
		CreditCardIgnored = 105,

		/// <summary>
        ///     Receive Payment as a Check
		/// </summary>
		CheckReceived = 201,

		/// <summary>
        ///     Send Payment as a Check
		/// </summary>
		CheckReturned = 202,

		/// <summary>
        ///     Receive a Payment as Cash
		/// </summary>
		CashReceived = 301,

		/// <summary>
        ///     Return Cash
		/// </summary>
		CashReturned = 302,

		/// <summary>
        ///     Purchase Order Number Info Stored
		/// </summary>
		PurchaseOrderInfo = 401,

		/// <summary>
        ///     Consider PO as Valid Payment
		/// </summary>
		PurchaseOrderAccepted = 402,

		/// <summary>
        ///     Company Account Number Saved
		/// </summary>
		CompanyAccountInfo = 450,

		/// <summary>
        ///     Company Account Number accepted as payment
		/// </summary>
		CompanyAccountAccepted = 451,

		/// <summary>
        ///     Record Gift Card Information
		/// </summary>
		GiftCardInfo = 501,

		/// <summary>
        ///     Hold a Specific Amount on a Gift Card
		/// </summary>
		GiftCardHold = 502,

		/// <summary>
        ///     Release previous hold on gift card amount
		/// </summary>
		GiftCardUnHold = 508,

		/// <summary>
        ///     Capture a Held Amount on a Gift Card
		/// </summary>
		GiftCardCapture = 503,

		/// <summary>
        ///     Reduce Value of Gift Card
		/// </summary>
		GiftCardDecrease = 504,

		/// <summary>
        ///     Increase Value of Gift Card
		/// </summary>
		GiftCardIncrease = 505,

		/// <summary>
        ///     Create a new Gift Card with an initial amount
		/// </summary>
		GiftCardCreateNew = 509,

		/// <summary>
        ///     Activate a Gift Card Number
		/// </summary>
		GiftCardActivate = 506,

		/// <summary>
        ///     Find the current balance of a gift card
		/// </summary>
		GiftCardBalanceInquiry = 507,

		/// <summary>
        ///     Deactivate a Gift Card Number
		/// </summary>
		GiftCardDeactivate = 510,

		/// <summary>
        ///     Record Reward Points Information
		/// </summary>
		RewardPointsInfo = 551,

		/// <summary>
        ///     Hold a Specific Amount of Reward Points
		/// </summary>
		RewardPointsHold = 552,

		/// <summary>
        ///     Capture a Held Amount of Reward Points
		/// </summary>
		RewardPointsCapture = 553,

		/// <summary>
        ///     Reduce Points Available
		/// </summary>
		RewardPointsDecrease = 554,

		/// <summary>
        ///     Increase Points Available
		/// </summary>
		RewardPointsIncrease = 555,

		/// <summary>
        ///     Un-hold a Specific Amount of Reward Points
		/// </summary>
		RewardPointsUnHold = 558,

		/// <summary>
        ///     Funds Held at PayPal
		/// </summary>
		PayPalHold = 601,

		/// <summary>
        ///     Capture previously held Funds
		/// </summary>
		PayPalCapture = 602,

		/// <summary>
        ///     Charge a PayPal Account
		/// </summary>
		PayPalCharge = 603,

		/// <summary>
        ///     Send Money to a PayPal Account
		/// </summary>
		PayPalRefund = 604,

		/// <summary>
        ///     Void a Pending Transaction
		/// </summary>
		PayPalVoid = 605,

		/// <summary>
        ///     Customer Requests PayPal Express Checkout
		/// </summary>
		PayPalExpressCheckoutInfo = 606,

		/// <summary>
        ///     Customer Requests Checkout using Third Party Pay Method
		/// </summary>
		ThirdPartyPayMethodInfo = 701,

		/// <summary>
        ///     Funds Held by Third Party Payment Method
		/// </summary>
		ThirdPartyPayMethodHold = 702,

		/// <summary>
        ///     Capture previously held Funds
		/// </summary>
		ThirdPartyPayMethodCapture = 703,

		/// <summary>
        ///     Charge via Third Party Pay Method
		/// </summary>
		ThirdPartyPayMethodCharge = 704,

		/// <summary>
        ///     Refund Money via Third Party Pay Method
		/// </summary>
		ThirdPartyPayMethodRefund = 705,

		/// <summary>
        ///     Void a Pending Transaction
		/// </summary>
		ThirdPartyPayMethodVoid = 706,

		/// <summary>
        ///     Create a Recurring Subscription
		/// </summary>
		RecurringSubscriptionInfo = 801,

		RecurringSubscriptionCreate = 802,

		RecurringSubscriptionUpdate = 803,

		RecurringSubscriptionCancel = 804,

		RecurringPayment = 805,

		/// <summary>
        ///     Records customer request to pay offline
		/// </summary>
		OfflinePaymentRequest = 9999,
    }
}
