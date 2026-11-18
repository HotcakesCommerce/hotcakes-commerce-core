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
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Security
{
    /// <summary>
    /// Holds the normalized values examined by the fraud-rule scorer and the messages
    /// produced when rules match those values.
    /// </summary>
    /// <remarks>
    /// This type is a data contract between fraud-data population and
    /// <see cref="FraudScorer"/>. Assignments are normalized by the property setters;
    /// the type does not perform validation or determine whether a value is fraudulent.
    /// </remarks>
    public class FraudCheckData
    {
        private string _CreditCard = string.Empty;
        private string _DomainName = string.Empty;
        private string _EmailAddress = string.Empty;

        private string _IpAddress = string.Empty;
        private string _PhoneNumber = string.Empty;

        /// <summary>
        /// Initializes empty fraud-check values and a message collection.
        /// </summary>
        public FraudCheckData()
        {
            Messages = new List<string>();
        }

        /// <summary>
        /// Gets or sets the IP address used for exact fraud-rule comparison.
        /// </summary>
        /// <remarks>
        /// The assigned value is trimmed. Unlike the other normalized fields, it is
        /// not lower-cased by this setter.
        /// </remarks>
        public string IpAddress
        {
            get { return _IpAddress; }
            set { _IpAddress = value.Trim(); }
        }

        /// <summary>
        /// Gets or sets the domain name used for fraud-rule comparison.
        /// </summary>
        /// <remarks>The assigned value is trimmed and lower-cased.</remarks>
        public string DomainName
        {
            get { return _DomainName; }
            set { _DomainName = value.Trim().ToLower(); }
        }

        /// <summary>
        /// Gets or sets the email address used for fraud-rule comparison.
        /// </summary>
        /// <remarks>The assigned value is trimmed and lower-cased.</remarks>
        public string EmailAddress
        {
            get { return _EmailAddress; }
            set { _EmailAddress = value.Trim().ToLower(); }
        }

        /// <summary>
        /// Gets or sets the phone number used for fraud-rule comparison.
        /// </summary>
        /// <remarks>
        /// The assigned value is trimmed, lower-cased, and passed through
        /// <see cref="CreditCardValidator.CleanCardNumber(string)"/>. The shared
        /// card-number cleaning routine is an observed compatibility behavior and is
        /// intentionally not changed here.
        /// </remarks>
        public string PhoneNumber
        {
            get { return _PhoneNumber; }
            set { _PhoneNumber = CreditCardValidator.CleanCardNumber(value.Trim().ToLower()); }
        }

        /// <summary>
        /// Gets or sets the credit-card number used for fraud-rule comparison.
        /// </summary>
        /// <remarks>
        /// The assigned value is trimmed, lower-cased, and passed through
        /// <see cref="CreditCardValidator.CleanCardNumber(string)"/>.
        /// </remarks>
        public string CreditCard
        {
            get { return _CreditCard; }
            set { _CreditCard = CreditCardValidator.CleanCardNumber(value.Trim().ToLower()); }
        }

        /// <summary>
        /// Gets or sets the messages appended by matching fraud rules.
        /// </summary>
        /// <remarks>
        /// The constructor creates an empty list. <see cref="FraudScorer"/> appends
        /// compatibility-sensitive default messages and does not clear existing entries.
        /// </remarks>
        public List<string> Messages { get; set; }
    }
}
