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

namespace Hotcakes.Commerce.Security
{
    /// <summary>
    /// Describes one persisted fraud rule and the store to which it belongs.
    /// </summary>
    /// <remarks>
    /// <see cref="FraudRuleRepository"/> maps this model to the <c>hcc_Fraud</c>
    /// persistence record. The value is interpreted according to <see cref="RuleType"/>
    /// and is compared with the corresponding normalized value on
    /// <see cref="FraudCheckData"/>.
    /// </remarks>
    public class FraudRule
    {
        /// <summary>
        /// Initializes an empty rule with the current UTC timestamp, no rule type,
        /// an empty value, and no store identifier.
        /// </summary>
        /// <remarks>
        /// Repository creation replaces the timestamp, assigns a BVIN when necessary,
        /// and scopes the rule to the current store.
        /// </remarks>
        public FraudRule()
        {
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            RuleType = FraudRuleType.None;
            RuleValue = string.Empty;
            StoreId = 0;
        }

        /// <summary>
        /// Gets or sets the unique business value identifier used to locate the rule.
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        /// Gets or sets the UTC time at which the rule was last updated.
        /// </summary>
        public DateTime LastUpdated { get; set; }

        /// <summary>
        /// Gets or sets the kind of fraud input against which <see cref="RuleValue"/>
        /// is compared.
        /// </summary>
        public FraudRuleType RuleType { get; set; }

        /// <summary>
        /// Gets or sets the persisted comparison value for this rule.
        /// </summary>
        /// <remarks>
        /// The value is not normalized by this model. Callers should store it in the
        /// same form produced for the corresponding <see cref="FraudCheckData"/>
        /// property so that <see cref="FraudScorer"/> can compare it correctly.
        /// </remarks>
        public string RuleValue { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the store owning this rule.
        /// </summary>
        public long StoreId { get; set; }
    }
}
