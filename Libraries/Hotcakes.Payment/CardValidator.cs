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

namespace Hotcakes.Payment
{
    /// <summary>
    ///     This is the class used to help validate the credit card numbers that are input into the system.
    /// </summary>
    [Serializable]
    public static class CardValidator
    {
        /// <summary>
        ///     This method will return the type of the credit card, based upon the credit card number provided.
        /// </summary>
        /// <param name="number">A credit card number of at least 6 characters in length.</param>
        /// <returns>The type of the credit card is returned.</returns>
        public static CardType GetCardTypeFromNumber(string number)
        {
            var cleanNumber = CleanCardNumber(number);
            
			if (string.IsNullOrEmpty(cleanNumber) == true)
			{
				return CardType.Unknown;
			}

            // Switch cards, starting with "4"
            if (cleanNumber.StartsWith("4903")) return CardType.Switch;
            if (cleanNumber.StartsWith("4905")) return CardType.Switch;
            if (cleanNumber.StartsWith("4911")) return CardType.Switch;
            if (cleanNumber.StartsWith("4936")) return CardType.Switch;
            if (cleanNumber.StartsWith("564182")) return CardType.Switch;

            // Visa
            if (cleanNumber.StartsWith("4")) return CardType.Visa;
            
            // Maestro Cards starting with "5"
            if (cleanNumber.StartsWith("5018")) return CardType.Maestro;
            if (cleanNumber.StartsWith("5020")) return CardType.Maestro;
            if (cleanNumber.StartsWith("5038")) return CardType.Maestro;

            // MasterCard
            if (cleanNumber.StartsWith("5")) return CardType.MasterCard;

            // Amex
            if (cleanNumber.StartsWith("34")) return CardType.Amex;
            if (cleanNumber.StartsWith("37")) return CardType.Amex;


            // Discover Card
            if (cleanNumber.StartsWith("6011")) return CardType.Discover;
            if (cleanNumber.StartsWith("644")) return CardType.Discover;
            if (cleanNumber.StartsWith("645")) return CardType.Discover;
            if (cleanNumber.StartsWith("646")) return CardType.Discover;
            if (cleanNumber.StartsWith("647")) return CardType.Discover;
            if (cleanNumber.StartsWith("648")) return CardType.Discover;
            if (cleanNumber.StartsWith("649")) return CardType.Discover;
            if (cleanNumber.StartsWith("65")) return CardType.Discover;

            var firstSix = 0;
            var intConversion = int.TryParse(cleanNumber.Substring(0, 6), out firstSix);

            if (intConversion)
            {
                if (firstSix >= 622126 && firstSix <= 622925)
                {
                    return CardType.Discover;
                }
            }

            // Could be Discover or China Union
            if (cleanNumber.StartsWith("6221")) return CardType.Unknown;

            // Solo
            if (cleanNumber.StartsWith("6334")) return CardType.Solo;
            if (cleanNumber.StartsWith("6767")) return CardType.Solo;

            // Switch cards starting with "6"
            if (cleanNumber.StartsWith("633110")) return CardType.Switch;
            if (cleanNumber.StartsWith("6333")) return CardType.Switch;
            
            // could be switch or maestro
            if (cleanNumber.StartsWith("6759")) return CardType.Unknown;

            // Maestro Cards Starting with "6"
            if (cleanNumber.StartsWith("6304")) return CardType.Maestro;
            if (cleanNumber.StartsWith("6761")) return CardType.Maestro;

            // JCB
            if (cleanNumber.StartsWith("35")) return CardType.JCB;

            // Diner's Club
            if (cleanNumber.StartsWith("300")) return CardType.DinersClub;
            if (cleanNumber.StartsWith("305")) return CardType.DinersClub;
            if (cleanNumber.StartsWith("36")) return CardType.DinersClub;
            if (cleanNumber.StartsWith("3852")) return CardType.DinersClub;

            return CardType.Unknown;
        }

        /// <summary>
        ///     This method will remove any non-numeric values from the card number passed into it.
        /// </summary>
        /// <param name="number">The value to remove unexpected characters from.</param>
        /// <returns>The original value, with only numeris characters</returns>
        public static string CleanCardNumber(string number)
        {
            var result = string.Empty;

            if (string.IsNullOrWhiteSpace(number))
            {
                return result;
            }

            var numberChars = number.ToCharArray();
            foreach (var numberChar in numberChars)
            {
				if ((int)numberChar >= 48 && (int)numberChar <= 57)
                {
                    result += numberChar;
                }
            }
            return result;
        }

        /// <summary>
        ///     This method will inspect the credit card number, and determine whether it is valid or not.
        /// </summary>
        /// <param name="number">The full credit card number</param>
        /// <returns>
        ///     If the credit card doesn't match the expected format and/or length of the prescribed card number, "false" will
        ///     be returned.
        /// </returns>
        public static bool IsCardNumberValid(string number)
        {
            var type = GetCardTypeFromNumber(number);
            return IsCardNumberValid(number, type);
        }

        /// <summary>
        ///     This method will inspect the credit card number, and determine whether it is valid or not.
        /// </summary>
        /// <param name="number">The full credit card number</param>
        /// <param name="type">The type of credit card the number belongs to</param>
        /// <returns>
        ///     If the credit card doesn't match the expected format and/or length of the prescribed card number, "false" will
        ///     be returned.
        /// </returns>
        public static bool IsCardNumberValid(string number, CardType type)
        {
            var cleanCardNumber = CleanCardNumber(number);

			//If card number is empty for some reason, it's not valid
			if (cleanCardNumber.Length == 0 || string.IsNullOrEmpty(cleanCardNumber))
			{
				return false;
			}

            // If the card type doesn't match the number, it's not valid for the type
            if (GetCardTypeFromNumber(cleanCardNumber) != type)
            {
                return false;
            }

            if (!IsMod10(cleanCardNumber))
            {
                return false;
            }
            
            switch (type)
            {
                case CardType.Amex:
                    return LengthCheck(cleanCardNumber, 15);
                case CardType.DinersClub:
                    return LengthCheck(cleanCardNumber, 14);
                case CardType.Discover:
                    return LengthCheck(cleanCardNumber, 16);
                case CardType.JCB:
                    return LengthCheck(cleanCardNumber, 16);
                case CardType.Maestro:
                    return LengthCheck(cleanCardNumber, 12, 19);
                case CardType.MasterCard:
                    return LengthCheck(cleanCardNumber, 16);
                case CardType.Solo:
                    return LengthCheck(cleanCardNumber, 16, 19);
                case CardType.Switch:
                    return LengthCheck(cleanCardNumber, 16, 19);
                case CardType.Visa:
                    return LengthCheck(cleanCardNumber, 13, 16);
            }
            return false;
        }

        private static bool IsMod10(string cleanCardNumber)
        {
            // Make sure we have at least a number that looks kind of like a card
            if (cleanCardNumber.Length >= 12)
            {
                // If we have an uneven card number length, mod 2 should be 1 for double digits
                var doubleModResult = cleanCardNumber.Length%2;

                var sum = 0;

                // Walk Number Backwards
                for (var i = cleanCardNumber.Length - 1; i > -1; i--)
                {
                    if (i%2 == doubleModResult)
                    {
                        // Double the digit
                        var n = CharToInt(cleanCardNumber[i])*2;
                        if (n > 9)
                        {
                            // if the result as more than 1 digit, add the digits
                            n += 1;
                        }
                        sum += n;
                    }
                    else
                    {
                        sum += CharToInt(cleanCardNumber[i]);
                    }
                }

                if (sum%10 == 0)
                {
                    return true;
                }
            }
            return false;
        }   

        private static int CharToInt(char input)
        {
            var output = 0;
            if (int.TryParse(input.ToString(), out output))
            {
                return output;
            }
            return 0;
        }

        private static bool LengthCheck(string cleanCardNumber, int exactLength)
        {
            return LengthCheck(cleanCardNumber, exactLength, exactLength);
        }

        private static bool LengthCheck(string cleanCardNumber, int min, int max)
        {
            if (cleanCardNumber.Length >= min && cleanCardNumber.Length <= max)
            {
                return true;
            }
            return false;
        }
    }
}
