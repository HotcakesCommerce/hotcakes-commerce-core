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

namespace Hotcakes.Payment.RecurringGateways
{
    /// <summary>
    /// Provides a deterministic recurring-payment gateway for testing subscription workflows.
    /// </summary>
    /// <remarks>
    /// The result of each recurring subscription action is controlled by the corresponding
    /// property on <see cref="TestRecurringGatewaySettings" />.
    /// </remarks>
    /// <seealso cref="RecurringPaymentGateway" />
    /// <seealso cref="TestRecurringGatewaySettings" />
    public class TestRecurringGateway : RecurringPaymentGateway
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="TestRecurringGateway" /> class
        /// with default simulated responses.
        /// </summary>
        public TestRecurringGateway()
        {
            Settings = new TestRecurringGatewaySettings();
        }

        /// <summary>
        /// Gets the display name of this recurring-payment gateway.
        /// </summary>
        public override string Name
        {
            get { return "Test Recurring Gateway"; }
        }

        /// <summary>
        /// Gets the stable identifier of this recurring-payment gateway.
        /// </summary>
        public override string Id
        {
            get { return "E411EBBA-3D54-4A2D-B869-E13D6E1E0D0D"; }
        }

        /// <summary>
        /// Gets or sets the simulated response settings used by this gateway.
        /// </summary>
        public TestRecurringGatewaySettings Settings { get; set; }

        /// <summary>
        /// Gets the settings exposed through the base recurring-payment gateway contract.
        /// </summary>
        /// <seealso cref="Settings" />
        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        /// <summary>
        /// Processes a recurring transaction using the configured simulated response.
        /// </summary>
        /// <param name="t">The recurring transaction to process.</param>
        /// <remarks>
        /// Create, update, and cancel actions use <see cref="TestRecurringGatewaySettings.ResponseForCreate" />,
        /// <see cref="TestRecurringGatewaySettings.ResponseForUpdate" />, and
        /// <see cref="TestRecurringGatewaySettings.ResponseForCancel" />, respectively.
        /// Each invocation assigns generated reference numbers to the transaction result.
        /// </remarks>
        /// <seealso cref="ActionType" />
        /// <seealso cref="Transaction.Result" />
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

        /// <summary>
        /// Gets the interval range used by the test gateway for recurring subscriptions.
        /// </summary>
        /// <param name="intervalType">The unit used to measure the recurring interval.</param>
        /// <returns>
        /// A <see cref="RecurringPaymentGateway.Range" /> whose values constrain daily intervals
        /// to 7 through 365 and monthly intervals to 1 through 12; other units use the base range.
        /// </returns>
        /// <seealso cref="RecurringIntervalType" />
        /// <seealso cref="RecurringPaymentGateway.GetRangeForIntervalType(RecurringIntervalType)" />
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
