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
    /// Provides the base implementation for gateways that process credit-card transactions.
    /// </summary>
    /// <seealso cref="GatewayBase" />
    [Serializable]
    public abstract class PaymentGateway : GatewayBase
    {
        /// <summary>
        /// Dispatches a transaction to the operation associated with its action.
        /// </summary>
        /// <param name="t">The transaction to process.</param>
        /// <remarks>
        /// Derived gateways override the protected operation methods to provide provider-specific
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
                    case ActionType.CreditCardHold:
                        Authorize(t);
                        break;
                    case ActionType.CreditCardCharge:
                        Charge(t);
                        break;
                    case ActionType.CreditCardCapture:
                        Capture(t);
                        break;
                    case ActionType.CreditCardRefund:
                        Refund(t);
                        break;
                    case ActionType.CreditCardVoid:
                        Void(t);
                        break;
                }
            }
            catch (Exception ex)
            {
                t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP", MessageType.Error));
                t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }

        /// <summary>
        /// Authorizes the transaction amount without completing the charge.
        /// </summary>
        /// <param name="t">The transaction to authorize.</param>
        /// <remarks>Derived gateways override this method when authorization is supported.</remarks>
        protected virtual void Authorize(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Charges the transaction amount.
        /// </summary>
        /// <param name="t">The transaction to charge.</param>
        /// <remarks>Derived gateways override this method when charging is supported.</remarks>
        protected virtual void Charge(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Captures a previously authorized transaction.
        /// </summary>
        /// <param name="t">The transaction to capture.</param>
        /// <remarks>Derived gateways override this method when capture is supported.</remarks>
        protected virtual void Capture(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Refunds a previously charged transaction.
        /// </summary>
        /// <param name="t">The transaction to refund.</param>
        /// <remarks>Derived gateways override this method when refunds are supported.</remarks>
        protected virtual void Refund(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Voids an authorization or otherwise reverses an unsettled transaction.
        /// </summary>
        /// <param name="t">The transaction to void.</param>
        /// <remarks>Derived gateways override this method when voids are supported.</remarks>
        protected virtual void Void(Transaction t)
        {
            NotSupported(t);
        }

        private void NotSupported(Transaction t)
        {
            t.Result.Succeeded = false;
            t.Result.Messages.Add(new Message("This operation is not supported", "UNSUPPORTED", MessageType.Warning));
        }
    }
}
