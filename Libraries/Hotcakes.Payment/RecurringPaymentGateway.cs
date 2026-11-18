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

namespace Hotcakes.Payment
{
    /// <summary>
    /// Provides the base implementation for gateways that manage recurring-payment subscriptions.
    /// </summary>
    /// <seealso cref="GatewayBase" />
    [Serializable]
    public abstract class RecurringPaymentGateway : GatewayBase
    {
        /// <summary>
        /// Dispatches a recurring-payment transaction to the operation associated with its action.
        /// </summary>
        /// <param name="t">The recurring-payment transaction to process.</param>
        /// <remarks>
        /// Derived gateways override the protected subscription methods to provide provider-specific
        /// behavior. Unsupported operations and processing failures are recorded on
        /// <see cref="Transaction.Result" />.
        /// </remarks>
        /// <seealso cref="ActionType" />
        /// <seealso cref="Transaction" />
        public virtual void ProcessTransaction(Transaction t)
        {
            try
            {
                switch (t.Action)
                {
                    case ActionType.RecurringSubscriptionCreate:
                        CreateSubscription(t);
                        break;
                    case ActionType.RecurringSubscriptionUpdate:
                        UpdateSubscription(t);
                        break;
                    case ActionType.RecurringSubscriptionCancel:
                        CancelSubscription(t);
                        break;
                }
            }
            catch (Exception ex)
            {
                t.Result.Messages.Add(new Message("Unknown Recurring Payment Error: " + ex.Message, "HCRP",
                    MessageType.Error));
                t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }

        /// <summary>
        /// Gets the supported interval range for a recurring-payment interval type.
        /// </summary>
        /// <param name="intervalType">The interval type for which to retrieve the range.</param>
        /// <returns>The inclusive minimum and maximum values supported for the interval.</returns>
        /// <seealso cref="RecurringIntervalType" />
        /// <seealso cref="Range" />
        public virtual Range GetRangeForIntervalType(RecurringIntervalType intervalType)
        {
            return new Range {Minimum = 1, Maximum = int.MaxValue};
        }

        /// <summary>
        /// Creates a recurring-payment subscription.
        /// </summary>
        /// <param name="t">The transaction containing the subscription request.</param>
        protected virtual void CreateSubscription(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Updates an existing recurring-payment subscription.
        /// </summary>
        /// <param name="t">The transaction containing the subscription changes.</param>
        protected virtual void UpdateSubscription(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Cancels an existing recurring-payment subscription.
        /// </summary>
        /// <param name="t">The transaction identifying the subscription to cancel.</param>
        protected virtual void CancelSubscription(Transaction t)
        {
            NotSupported(t);
        }

        private void NotSupported(Transaction t)
        {
            t.Result.Succeeded = false;
            t.Result.Messages.Add(new Message("This operation is not supported", "UNSUPPORTED", MessageType.Warning));
        }

        /// <summary>
        /// Represents the inclusive minimum and maximum values supported for a recurring interval.
        /// </summary>
        public class Range
        {
            /// <summary>
            /// Gets or sets the minimum supported interval value.
            /// </summary>
            public int Minimum { get; set; }

            /// <summary>
            /// Gets or sets the maximum supported interval value.
            /// </summary>
            public int Maximum { get; set; }
        }
    }
}
