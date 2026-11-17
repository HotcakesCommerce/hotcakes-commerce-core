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
using System.Collections.ObjectModel;

namespace Hotcakes.Commerce.Utilities
{
    /// <summary>
    /// Validates supported payment-card numbers by card type, length, prefix, and the Mod-10 checksum.
    /// </summary>
    /// <remarks>
    /// Validation removes non-digit characters before checking the number. The validator stores the card name and
    /// validation messages from the most recent call to <see cref="ValidateCard(string, string)"/>.
    /// </remarks>
    public class CreditCardValidator
    {
        private string _cardName = "unknown";

        private Collection<string> _errorMessages = new Collection<string>();

        /// <summary>
        /// Gets or sets the validation messages produced by the most recent card validation.
        /// </summary>
        /// <value>A mutable collection of human-readable validation messages.</value>
        /// <remarks>
        /// <see cref="ValidateCard(string, string)"/> clears this collection before adding messages. Assigning a
        /// different collection changes the collection that subsequent validation calls clear and populate.
        /// </remarks>
        public Collection<string> ErrorMessages
        {
            get { return _errorMessages; }
            set { _errorMessages = value; }
        }

        /// <summary>
        /// Gets or sets the English name associated with the most recently selected card type.
        /// </summary>
        /// <value>The card name, such as <c>Visa</c>; an unknown card type leaves the value unchanged by validation.</value>
        public string CardName
        {
            get { return _cardName; }
            set { _cardName = value; }
        }

        /// <summary>
        /// Removes all non-digit characters from a card-number string.
        /// </summary>
        /// <param name="rawCardNumber">The input string containing the card number and any formatting characters.</param>
        /// <returns>A string containing only the digit characters from <paramref name="rawCardNumber"/>.</returns>
        /// <remarks>
        /// The method does not validate the resulting number and expects a non-<see langword="null"/> input.
        /// </remarks>
        public static string CleanCardNumber(string rawCardNumber)
        {
            var result = string.Empty;
            var counter = 0;
            char[] s;

            for (counter = 0; counter <= rawCardNumber.Length - 1; counter++)
            {
                s = rawCardNumber.ToCharArray(counter, 1);

                if (Convert.ToInt32(s[0]) >= 48 & Convert.ToInt32(s[0]) <= 57)
                {
                    result += s[0];
                }
            }
            return result;
        }

        /// <summary>
        /// Validates a card number against the supported format and Mod-10 checksum for a card type.
        /// </summary>
        /// <param name="rawCardNumber">The card number to check; non-digit formatting characters are ignored.</param>
        /// <param name="cardType">
        /// The card type code: <c>A</c> for American Express, <c>V</c> for Visa, <c>D</c> for Discover,
        /// <c>M</c> for MasterCard, <c>J</c> for JCB, or <c>C</c> for Diner's Club. Several additional card names
        /// (<c>SOLO</c>, <c>SWITCH</c>, <c>STAR</c>, <c>MAESTRO</c>, <c>VISADELTA</c>, and <c>VISAELECTRON</c>)
        /// are accepted without number validation.
        /// </param>
        /// <returns><see langword="true"/> when the card type is recognized and its checks pass, or when the type is
        /// intentionally treated as unvalidated; otherwise, <see langword="false"/>.</returns>
        /// <remarks>
        /// Before validation, <see cref="ErrorMessages"/> is cleared and <see cref="CardName"/> is updated. Failed
        /// checks append descriptive messages to <see cref="ErrorMessages"/>. An unrecognized card type is accepted
        /// without validation.
        /// </remarks>
        public bool ValidateCard(string rawCardNumber, string cardType)
        {
            var result = true;

            _errorMessages.Clear();
            _cardName = string.Empty;

            var workingCardNumber = CleanCardNumber(rawCardNumber);
            var length = string.Empty;
            var prefix = string.Empty;

            switch (cardType.ToUpper())
            {
                case "A":
                    _cardName = "American Express";
                    length = "15";
                    prefix = "34;37";
                    break;
                case "V":
                    _cardName = "VISA";
                    length = "13;16";
                    prefix = "4";
                    break;
                case "M":
                    _cardName = "MasterCard";
                    length = "16";
                    prefix = "51;52;53;54;55";
                    break;
                case "D":
                    _cardName = "Discover";
                    length = "16";
                    prefix = "6011";
                    break;
                case "C":
                    _cardName = "Diner's Club";
                    length = "14";
                    prefix = "300;301;302;303;304;305;36;38";
                    break;
                case "J":
                    _cardName = "JCB";
                    length = "15;16";
                    prefix = "3;2131;1800";
                    break;
                case "SOLO":
                    _cardName = "Solo";
                    return true;
                case "SWITCH":
                    _cardName = "Switch";
                    return true;
                case "STAR":
                    _cardName = "STAR Card";
                    return true;
                case "MAESTRO":
                    _cardName = "Maestro / Switch";
                    return true;
                case "VISADELTA":
                    _cardName = "Visa Delta";
                    return true;
                case "VISAELECTRON":
                    _cardName = "Visa Electron";
                    return true;
                default:
                    // Unknown Card Type so skip validation
                    return true;
            }


            var lengths = length.Split(';');
            var prefixes = prefix.Split(';');

            var prefixOK = false;
            var lengthOK = false;
            var modOK = false;

            // Check card length against acceptable lengths
            int i;
            for (i = 0; i <= lengths.Length - 1; i++)
            {
                if (workingCardNumber.Length == Convert.ToInt32(lengths[i]))
                {
                    lengthOK = true;
                }
            }

            // Check to makes sure prefix is valid
            int j;
            for (j = 0; j <= prefixes.Length - 1; j++)
            {
                if (prefixes[j].Length <= workingCardNumber.Length)
                {
                    if (workingCardNumber.Substring(0, prefixes[j].Length) == prefixes[j])
                    {
                        prefixOK = true;
                    }
                }
            }

            modOK = Mod10CheckCard(workingCardNumber);

            if (lengthOK == false)
            {
                _errorMessages.Add("Invalid length for " + _cardName);
                result = false;
            }
            if (prefixOK == false)
            {
                _errorMessages.Add("Invalid number for " + _cardName);
                result = false;
            }
            if (modOK == false)
            {
                _errorMessages.Add("This is not a valid credit card number.");
                result = false;
            }

            return result;
        }

        private static bool Mod10CheckCard(string cardNumber)
        {
            try
            {
                // Array to contain individual numbers
                int lastNumber;

                // So, get length of card
                var CardLength = cardNumber.Length;

                // Double the value of alternate digits, starting with the second digit
                // from the right, i.e. back to front.
                // Loop through starting at the end
                int i;
                var odd = false;
                var sum = 0;

                var Chars = cardNumber.ToCharArray();

                for (i = CardLength - 1; i >= 0; i += -1)
                {
                    if (odd)
                    {
                        lastNumber = int.Parse(Chars[i].ToString())*2;
                    }
                    else
                    {
                        lastNumber = int.Parse(Chars[i].ToString());
                    }

                    if (lastNumber > 9)
                    {
                        lastNumber -= 9;
                    }
                    sum += lastNumber;

                    odd = !odd;
                }

                return sum%10 == 0;
            }
            catch
            {
                return false;
            }
        }
    }
}
