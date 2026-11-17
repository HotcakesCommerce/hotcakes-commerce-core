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
    /// Scores fraud-check data against the fraud rules for the current store.
    /// </summary>
    /// <remarks>
    /// Each matching rule contributes its fixed weight, and the total is capped at
    /// 10. Matching rules also append their compatibility-sensitive default messages
    /// to <see cref="FraudCheckData.Messages"/>. This class performs exact comparisons
    /// against the values supplied by <see cref="FraudCheckData"/>; it does not alter
    /// rule values or provide a separate fraud decision.
    /// </remarks>
    public class FraudScorer
    {
        private readonly HccRequestContext _context;
        private readonly FraudRuleRepository _repository;

        /// <summary>
        /// Initializes a scorer with an explicit rule repository.
        /// </summary>
        /// <param name="context">The request context used to identify the current store.</param>
        /// <param name="repository">The repository from which store rules are loaded.</param>
        public FraudScorer(HccRequestContext context, FraudRuleRepository repository)
        {
            _repository = repository;
            _context = context;
        }

        /// <summary>
        /// Initializes a scorer with a repository created for the supplied context.
        /// </summary>
        /// <param name="context">The request context used to identify the current store.</param>
        public FraudScorer(HccRequestContext context)
        {
            _repository = new FraudRuleRepository(context);
            _context = context;
        }

        /// <summary>
        /// Scores the supplied fraud-check data against all rules for the current store.
        /// </summary>
        /// <param name="data">The normalized fraud-check data to compare.</param>
        /// <returns>
        /// The accumulated fraud score, capped at 10. A score of zero is returned when
        /// no rules are available.
        /// </returns>
        /// <remarks>
        /// Matching weights are credit card 7, email address 5, domain name 3, phone
        /// number 3, and IP address 1. The total is capped only after all rules are
        /// evaluated. Matching rules append their exact default messages to
        /// <see cref="FraudCheckData.Messages"/>: <c>Credit Card Matched Fraud Rules</c>,
        /// <c>Email Address Matched Fraud Rules</c>, <c>Domain Matched Fraud Rules</c>,
        /// <c>Phone Number Matched Fraud Rules</c>, and <c>IP Address Fraud Rules</c>,
        /// respectively. Comparisons are exact after the normalization performed while
        /// assigning values to <see cref="FraudCheckData"/>.
        /// </remarks>
        public decimal ScoreData(FraudCheckData data)
        {
            decimal result = 0;

            var storeRules = _repository.FindForStore(_context.CurrentStore.Id);
            if (storeRules == null) return result;

            foreach (var rule in storeRules)
            {
                result += ScoreSingleRule(data, rule);
            }

            if (result > 10) result = 10;

            return result;
        }

        private decimal ScoreSingleRule(FraudCheckData data, FraudRule rule)
        {
            decimal result = 0;

            switch (rule.RuleType)
            {
                case FraudRuleType.CreditCardNumber:
                    if (rule.RuleValue == data.CreditCard)
                    {
                        result += 7;
                        data.Messages.Add("Credit Card Matched Fraud Rules");
                    }
                    break;
                case FraudRuleType.DomainName:
                    if (rule.RuleValue == data.DomainName)
                    {
                        result += 3;
                        data.Messages.Add("Domain Matched Fraud Rules");
                    }
                    break;
                case FraudRuleType.EmailAddress:
                    if (rule.RuleValue == data.EmailAddress)
                    {
                        result += 5;
                        data.Messages.Add("Email Address Matched Fraud Rules");
                    }
                    break;
                case FraudRuleType.IPAddress:
                    if (rule.RuleValue == data.IpAddress)
                    {
                        result += 1;
                        data.Messages.Add("IP Address Fraud Rules");
                    }
                    break;
                case FraudRuleType.PhoneNumber:
                    if (rule.RuleValue == data.PhoneNumber)
                    {
                        result += 3;
                        data.Messages.Add("Phone Number Matched Fraud Rules");
                    }
                    break;
            }
            return result;
        }
    }
}
