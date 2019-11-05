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

namespace Hotcakes.Payment
{
    [Serializable]
    public abstract class GiftCardGateway
    {
        public abstract string Id { get; }

        public abstract string Name { get; }

        public abstract MethodSettings BaseSettings { get; }

        public abstract bool CanAuthorize { get; }

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

        protected virtual void Authorize(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void Charge(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void Capture(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void Refund(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void Void(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void CreateNew(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void Activate(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void Deactivate(Transaction t)
        {
            NotSupported(t);
        }

        protected virtual void BalanceInquiry(Transaction t)
        {
            NotSupported(t);
        }

        protected void NotSupported(Transaction t)
        {
            t.Result.Succeeded = false;
            t.Result.Messages.Add(new Message("This operation is not supported", "UNSUPPORTED", MessageType.Warning));
        }
    }
}