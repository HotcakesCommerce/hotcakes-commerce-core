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

using System.Collections.Generic;

namespace Hotcakes.Payment
{
    /// <summary>
    /// Provides classifications and report-oriented groupings for <see cref="ActionType"/> values.
    /// </summary>
	public static class ActionTypeUtils
	{
        /// <summary>
        /// Determines whether an action represents a credit-card transaction.
        /// </summary>
        /// <param name="action">The transaction action to evaluate.</param>
        /// <returns><see langword="true"/> when <paramref name="action"/> is a credit-card transaction; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="ActionType.CreditCardCharge"/>
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
        /// Determines whether an action represents a gift-card transaction.
		/// </summary>
        /// <param name="action">The transaction action to evaluate.</param>
        /// <returns><see langword="true"/> when <paramref name="action"/> is a gift-card transaction; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="ActionType.GiftCardCapture"/>
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
        /// Determines whether an action represents a PayPal transaction.
		/// </summary>
        /// <param name="action">The transaction action to evaluate.</param>
        /// <returns><see langword="true"/> when <paramref name="action"/> is a PayPal transaction; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="ActionType.PayPalCharge"/>
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
        /// Determines whether an action represents a reward-point transaction.
		/// </summary>
        /// <param name="action">The transaction action to evaluate.</param>
        /// <returns><see langword="true"/> when <paramref name="action"/> is a reward-point transaction; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="ActionType.RewardPointsIncrease"/>
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
        /// Gets the mutable list of actions that affect an account balance.
        /// </summary>
        /// <remarks>
        /// The list contains the action values used when calculating general balance changes.
        /// </remarks>
        /// <seealso cref="ActionType.CreditCardCapture"/>
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
        /// Gets the mutable list of actions included in the credit-card balance report.
        /// </summary>
        /// <remarks>
        /// This list is limited to credit-card capture, charge, and refund operations.
        /// </remarks>
        /// <seealso cref="ActionType.CreditCardCapture"/>
        public static List<ActionType> BalanceChangingActionsForCreditCardReport = new List<ActionType>(){
			ActionType.CreditCardCapture,
			ActionType.CreditCardCharge,
			ActionType.CreditCardRefund
		};
	}
}

