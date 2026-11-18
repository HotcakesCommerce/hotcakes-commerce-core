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

namespace Hotcakes.Commerce.Payment
{
    /// <summary>
    /// Describes a payment method displayed to customers during checkout.
    /// Implementations are discovered by <see cref="PaymentMethods"/> through loaded assemblies.
    /// </summary>
    [Serializable]
    public abstract class PaymentMethod
    {
        /// <summary>
        /// Gets the stable identifier used in store payment-method settings.
        /// </summary>
        public abstract string MethodId { get; }

        /// <summary>
        /// Gets the compatibility-sensitive display name for the method.
        /// </summary>
        public abstract string MethodName { get; }

        /// <summary>
        /// Gets the ordering value used when available methods are sorted.
        /// </summary>
        public abstract int SortIndex { get; }

        /// <summary>
        /// Gets a value indicating whether payment is expected before order placement.
        /// </summary>
        public virtual bool PayBeforePlacement
        {
            get { return false; }
        }

        /// <summary>
        /// Gets a value indicating whether the method supports recurring billing.
        /// </summary>
        public virtual bool SupportRecurringBilling
        {
            get { return false; }
        }
    }
}
