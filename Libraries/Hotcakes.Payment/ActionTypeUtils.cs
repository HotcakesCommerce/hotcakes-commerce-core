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

using System.Collections.Generic;

namespace Hotcakes.Payment
{
	public static class ActionTypeUtils
	{
        /// <summary>
        ///     Tells you whether the transaction is a credit card transaction or not.
		/// </summary>
		/// <param name="action">Transaction type to evaluate.</param>
		/// <returns>If true, the transaction type (action) is a credit card transaction.</returns>
		public static bool IsCreditCardTransaction(ActionType action)
		{
            return action == ActionType.CreditCardCapture ||
				action == ActionType.CreditCardCharge ||
				action == ActionType.CreditCardHold ||
				action == ActionType.CreditCardInfo ||
				action == ActionType.CreditCardRefund ||
                   action == ActionType.CreditCardVoid;
		}

		/// <summary>
        ///     Tells you whether the transaction is a gift card transaction or not.
		/// </summary>
		/// <param name="action">Transaction type to evaluate.</param>
		/// <returns>If true, the transaction type (action) is a gift card transaction.</returns>
		public static bool IsGiftCardTransaction(ActionType action)
		{
            return action == ActionType.GiftCardCapture ||
				action == ActionType.GiftCardDecrease ||
				action == ActionType.GiftCardHold ||
				action == ActionType.GiftCardIncrease ||
				action == ActionType.GiftCardInfo ||
                   action == ActionType.GiftCardUnHold;
		}

		/// <summary>
        ///     Tells you whether the transaction is a PayPal transaction or not.
		/// </summary>
		/// <param name="action">Transaction type to evaluate.</param>
		/// <returns>If true, the transaction type (action) is a PayPal transaction.</returns>
		public static bool IsPayPalTransaction(ActionType action)
		{
            return action == ActionType.PayPalCapture ||
				action == ActionType.PayPalCharge ||
				action == ActionType.PayPalExpressCheckoutInfo ||
				action == ActionType.PayPalHold ||
				action == ActionType.PayPalRefund ||
                   action == ActionType.PayPalVoid;
		}

		/// <summary>
        ///     Tells you whether the transaction is a reward point transaction or not.
		/// </summary>
		/// <param name="action">Transaction type to evaluate.</param>
		/// <returns>If true, the transaction type (action) is a reward point transaction.</returns>
		public static bool IsRewardPointTransaction(ActionType action)
		{
            return action == ActionType.RewardPointsCapture ||
					action == ActionType.RewardPointsDecrease ||
					action == ActionType.RewardPointsHold ||
					action == ActionType.RewardPointsIncrease ||
					action == ActionType.RewardPointsInfo ||
                   action == ActionType.RewardPointsUnHold;
		}

		/// <summary>
		/// The list of actions that influence accounts balance
		/// </summary>
		public static List<ActionType> BalanceChangingActions = new List<ActionType>(){
			ActionType.CashReceived,
			ActionType.CashReturned,
			ActionType.CheckReceived,
			ActionType.CheckReturned,
			ActionType.CompanyAccountAccepted,
			ActionType.PurchaseOrderAccepted,
			ActionType.CreditCardCapture,
			ActionType.CreditCardCharge,
			ActionType.CreditCardRefund,
			ActionType.GiftCardCapture,
			ActionType.GiftCardDecrease,
			ActionType.GiftCardIncrease,
			ActionType.PayPalCapture,
			ActionType.PayPalCharge,
			ActionType.PayPalRefund,
			ActionType.RewardPointsCapture,
			ActionType.RewardPointsIncrease,
			ActionType.RewardPointsDecrease,
			ActionType.ThirdPartyPayMethodCapture,
			ActionType.ThirdPartyPayMethodCharge,
			ActionType.ThirdPartyPayMethodRefund
		};

        /// <summary>
        /// The list of actions that influence accounts balance
        /// </summary>
        public static List<ActionType> BalanceChangingActionsForCreditCardReport = new List<ActionType>(){
			ActionType.CreditCardCapture,
			ActionType.CreditCardCharge,
			ActionType.CreditCardRefund
		};
	}
}
