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

namespace Hotcakes.Commerce.Security
{
    /// <summary>
    /// Identifies the fraud-check input represented by a <see cref="FraudRule"/>.
    /// </summary>
    /// <remarks>
    /// The numeric values are persisted with fraud rules and must remain compatible
    /// with existing records.
    /// </remarks>
    public enum FraudRuleType
    {
        /// <summary>
        /// Indicates that no fraud rule type has been selected.
        /// </summary>
        None = 0,

        /// <summary>
        /// Matches an IP address.
        /// </summary>
        IPAddress = 1,

        /// <summary>
        /// Matches a domain name.
        /// </summary>
        DomainName = 2,

        /// <summary>
        /// Matches an email address.
        /// </summary>
        EmailAddress = 3,

        /// <summary>
        /// Matches a phone number after the check-data normalization applied by
        /// <see cref="FraudCheckData.PhoneNumber"/>.
        /// </summary>
        PhoneNumber = 4,

        /// <summary>
        /// Matches a credit-card number after the check-data normalization applied by
        /// <see cref="FraudCheckData.CreditCard"/>.
        /// </summary>
        CreditCardNumber = 5
    }
}
