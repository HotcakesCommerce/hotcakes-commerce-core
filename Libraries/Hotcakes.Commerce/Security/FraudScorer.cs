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

namespace Hotcakes.Commerce.Security
{
    public class FraudScorer
    {
        private readonly HccRequestContext _context;
        private readonly FraudRuleRepository _repository;

        public FraudScorer(HccRequestContext context, FraudRuleRepository repository)
        {
            _repository = repository;
            _context = context;
        }

        public FraudScorer(HccRequestContext context)
        {
            _repository = new FraudRuleRepository(context);
            _context = context;
        }

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