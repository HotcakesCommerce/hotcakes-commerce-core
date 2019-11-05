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
using System.Web.Script.Serialization;

namespace Hotcakes.Payment
{
    /// <summary>
    ///     This is the primary object that is used to manage credit card data in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is OrderTransactionCardDataDTO.</remarks>
    [Serializable]
    public class CardData
    {
        private string _CardNumber = string.Empty;

        public CardData()
        {
            SecurityCode = string.Empty;
            CardHolderName = string.Empty;
        }

        /// <summary>
        ///     The number of the credit card.
        /// </summary>
        public string CardNumber
        {
            get { return _CardNumber; }
            set { _CardNumber = CardValidator.CleanCardNumber(value); }
        }

        /// <summary>
        ///     The name of the credit card holder as it appears on the card.
        /// </summary>
        public string CardHolderName { get; set; }

        /// <summary>
        ///     The year that the credit card will expire.
        /// </summary>
		public int ExpirationYear { get; set; }

        /// <summary>
        ///     The month that the credit card will expire.
        /// </summary>
		public int ExpirationMonth { get; set; }

        /// <summary>
        ///     The CVV or code that is unique to identify the card.
        /// </summary>
        /// <remarks>
        ///     This value is not saved anywhere in the application per PA-DSS compliance standards.
        /// </remarks>
        [ScriptIgnore]
        public string SecurityCode { get; set; }

        /// <summary>
        ///     The correct two-character representation of the expiration month.
        /// </summary>
        [ScriptIgnore]
        public string ExpirationMonthPadded
        {
            get
            {
                var result = ExpirationMonth.ToString();
                if (ExpirationMonth < 10) result = "0" + result;
                return result;
            }
        }

        /// <summary>
        ///     The correct two-character representation of the expiration year.
        /// </summary>
        [ScriptIgnore]
        public string ExpirationYearTwoDigits
        {
            get
            {
                var result = ExpirationYear.ToString();
                if (result.Length > 2)
                {
                    result = result.Substring(result.Length - 2, 2);
                }
                return result;
            }
        }

        /// <summary>
        ///     Returns the last 4-digits of the credit card number for display in the application.
        /// </summary>
        [ScriptIgnore]
        public string CardNumberLast4Digits
        {
            get
            {
                var result = string.Empty;
                if (CardNumber.Trim().Length >= 4)
                {
                    result = CardNumber.Substring(CardNumber.Length - 4, 4);
                }
                return result;
            }
        }
        
        /// <summary>
        ///     Gets the type of card that this is based upon the card number.
        /// </summary>
        [ScriptIgnore]
        public CardType CardType
        {
            get { return CardValidator.GetCardTypeFromNumber(CardNumber); }
        }
        
        /// <summary>
        ///     Returns the human-readable version of the card type, based upon the card number.
        /// </summary>
        [ScriptIgnore]
        public string CardTypeName
        {
            get
            {
                var t = CardType;
                switch (t)
                {
                    case CardType.Amex:
                        return "AMEX";
                    case CardType.DinersClub:
                        return "Diner's Club";
                    case CardType.Discover:
                        return "Discover";
                    case CardType.JCB:
                        return "JCB";
                    case CardType.Maestro:
                        return "Maestro";
                    case CardType.MasterCard:
                        return "MasterCard";
                    case CardType.Solo:
                        return "Solo";
                    case CardType.Switch:
                        return "Switch";
                    case CardType.Visa:
                        return "Visa";
                    default:
                        return "Unknown";
                }
            }
        }

        /// <summary>
        ///     Validates and reports the findings of the card number and expiration date validation.
        /// </summary>
        /// <param name="localTime">The current date/time stamp of the transaction.</param>
        /// <returns>Boolean - if true, the card number and date are valid.</returns>
        public bool IsCardValid(DateTime localTime)
        {
            if (!CardValidator.IsCardNumberValid(CardNumber))
            {
                return false;
            }

            if (CardHasExpired(localTime))
            {
                return false;
            }

            return true;
        }

		public bool IsCardNumberValid()
		{
			if (CardValidator.IsCardNumberValid(this.CardNumber) == false)
			{
				return false;
			}
			return true;
		}

        /// <summary>
        ///     Validates the expiration date of the card.
        /// </summary>
        /// <param name="localTime">The current date/time stamp of the transaction.</param>
        /// <returns>Boolean - if true, the expiration date is still in the future.</returns>
        public bool CardHasExpired(DateTime localTime)
        {
            if (ExpirationYear < localTime.Year)
            {
                return true;
            }

            if (ExpirationYear == localTime.Year)
            {
                if (ExpirationMonth < localTime.Month)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
