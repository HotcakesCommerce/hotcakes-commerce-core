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
    /// Provides the base implementation for gateways that process gift-card transactions.
    /// </summary>
    [Serializable]
    public abstract class GiftCardGateway
    {
        /// <summary>
        /// Gets the stable identifier used to distinguish the gateway.
        /// </summary>
        public abstract string Id { get; }

        /// <summary>
        /// Gets the display name of the gateway.
        /// </summary>
        public abstract string Name { get; }

        /// <summary>
        /// Gets the gateway's base configuration settings.
        /// </summary>
        /// <seealso cref="MethodSettings" />
        public abstract MethodSettings BaseSettings { get; }

        /// <summary>
        /// Gets a value indicating whether the gateway supports gift-card authorization.
        /// </summary>
        public abstract bool CanAuthorize { get; }

        /// <summary>
        /// Dispatches a gift-card transaction to the operation associated with its action.
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
                    case ActionType.GiftCardHold:
                        Authorize(t);
                        break;
                    case ActionType.GiftCardDecrease:
                        Charge(t);
                        break;
                    case ActionType.GiftCardCapture:
                        Capture(t);
                        break;
                    case ActionType.GiftCardIncrease:
                        Refund(t);
                        break;
                    case ActionType.GiftCardUnHold:
                        Void(t);
                        break;
                    case ActionType.GiftCardCreateNew:
                        CreateNew(t);
                        break;
                    case ActionType.GiftCardActivate:
                        Activate(t);
                        break;
                    case ActionType.GiftCardDeactivate:
                        Deactivate(t);
                        break;
                    case ActionType.GiftCardBalanceInquiry:
                        BalanceInquiry(t);
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
        /// Places a hold on the gift-card balance.
        /// </summary>
        /// <param name="t">The transaction to authorize.</param>
        protected virtual void Authorize(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Decreases the gift-card balance.
        /// </summary>
        /// <param name="t">The transaction that decreases the balance.</param>
        protected virtual void Charge(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Captures a previously authorized gift-card hold.
        /// </summary>
        /// <param name="t">The transaction to capture.</param>
        protected virtual void Capture(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Increases the gift-card balance.
        /// </summary>
        /// <param name="t">The transaction that increases the balance.</param>
        protected virtual void Refund(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Releases a gift-card hold.
        /// </summary>
        /// <param name="t">The transaction whose hold should be released.</param>
        protected virtual void Void(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Creates a new gift card.
        /// </summary>
        /// <param name="t">The transaction containing the gift-card request.</param>
        protected virtual void CreateNew(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Activates a gift card.
        /// </summary>
        /// <param name="t">The transaction identifying the gift card.</param>
        protected virtual void Activate(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Deactivates a gift card.
        /// </summary>
        /// <param name="t">The transaction identifying the gift card.</param>
        protected virtual void Deactivate(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Retrieves the current gift-card balance.
        /// </summary>
        /// <param name="t">The transaction containing the gift-card request.</param>
        protected virtual void BalanceInquiry(Transaction t)
        {
            NotSupported(t);
        }

        /// <summary>
        /// Records that the requested operation is not supported by this gateway.
        /// </summary>
        /// <param name="t">The transaction receiving the unsupported-operation result.</param>
        protected void NotSupported(Transaction t)
        {
            t.Result.Succeeded = false;
            t.Result.Messages.Add(new Message("This operation is not supported", "UNSUPPORTED", MessageType.Warning));
        }
    }
}
