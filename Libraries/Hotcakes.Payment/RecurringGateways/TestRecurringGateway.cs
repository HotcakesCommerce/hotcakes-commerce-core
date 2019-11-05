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

namespace Hotcakes.Payment.RecurringGateways
{
    public class TestRecurringGateway : RecurringPaymentGateway
    {
        public TestRecurringGateway()
        {
            Settings = new TestRecurringGatewaySettings();
        }

        public override string Name
        {
            get { return "Test Recurring Gateway"; }
        }

        public override string Id
        {
            get { return "E411EBBA-3D54-4A2D-B869-E13D6E1E0D0D"; }
        }

        public TestRecurringGatewaySettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        public override void ProcessTransaction(Transaction t)
        {
            var result = false;

            switch (t.Action)
            {
                case ActionType.RecurringSubscriptionCreate:
                    result = Settings.ResponseForCreate;
                    break;
                case ActionType.RecurringSubscriptionUpdate:
                    result = Settings.ResponseForUpdate;
                    break;
                case ActionType.RecurringSubscriptionCancel:
                    result = Settings.ResponseForCancel;
                    break;
            }

            t.Result.ReferenceNumber = Guid.NewGuid().ToString();
            t.Result.ReferenceNumber2 = Guid.NewGuid().ToString();

            t.Result.Succeeded = result;
        }

        public override Range GetRangeForIntervalType(RecurringIntervalType intervalType)
        {
            // Since we have only 1 real recurring payment gateway implementted
            // implemented same validation as for AuthorizeNet gateway
            // to prevent confusion for users when switching between them
            switch (intervalType)
            {
                case RecurringIntervalType.Days:
                    return new Range {Minimum = 7, Maximum = 365};
                case RecurringIntervalType.Months:
                    return new Range {Minimum = 1, Maximum = 12};
                default:
                    return base.GetRangeForIntervalType(intervalType);
            }
        }
    }
}