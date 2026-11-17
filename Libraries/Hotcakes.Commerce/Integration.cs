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

using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce
{
    /// <summary>
    /// Synchronous integration event hub for customer-account and order notifications; raisers do not queue or swallow handler exceptions.
    /// </summary>
    public class Integration
    {
        /// <summary>
        /// Public API member: <c>public delegate void CustomerAccountEmailChangedHandler(object sender, string oldEmail, string newEmail);</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public delegate void CustomerAccountEmailChangedHandler(object sender, string oldEmail, string newEmail);

        /// <summary>
        /// Public API member: <c>public delegate void CustomerAccountEventHandler(object sender, CustomerAccount account);</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public delegate void CustomerAccountEventHandler(object sender, CustomerAccount account);

        /// <summary>
        /// Public API member: <c>public delegate void OrderEventHandler(object sender, Order order, HotcakesApplication app);</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public delegate void OrderEventHandler(object sender, Order order, HotcakesApplication app);

        /// <summary>
        /// Public API member: <c>public event CustomerAccountEventHandler OnCustomerAccountUpdated;</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public event CustomerAccountEventHandler OnCustomerAccountUpdated;
        /// <summary>
        /// Public API member: <c>public event CustomerAccountEventHandler OnCustomerAccountCreated;</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public event CustomerAccountEventHandler OnCustomerAccountCreated;
        /// <summary>
        /// Public API member: <c>public event CustomerAccountEventHandler OnCustomerAccountDeleted;</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public event CustomerAccountEventHandler OnCustomerAccountDeleted;
        /// <summary>
        /// Public API member: <c>public event CustomerAccountEmailChangedHandler OnCustomerAccountEmailChanged;</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public event CustomerAccountEmailChangedHandler OnCustomerAccountEmailChanged;
        /// <summary>
        /// Public API member: <c>public event OrderEventHandler OnOrderReceived;</c>. This member preserves the established Hotcakes Commerce contract and behavior.
        /// </summary>
        public event OrderEventHandler OnOrderReceived;

        /// <summary>
        /// Synchronously raises <see cref="OnCustomerAccountUpdated"/> when subscribed.
        /// </summary>
        public void CustomerAccountUpdated(CustomerAccount account)
        {
            if (OnCustomerAccountUpdated != null)
            {
                OnCustomerAccountUpdated(this, account);
            }
        }

        /// <summary>
        /// Synchronously raises <see cref="OnCustomerAccountCreated"/> when subscribed.
        /// </summary>
        public void CustomerAccountCreated(CustomerAccount account)
        {
            if (OnCustomerAccountCreated != null)
            {
                OnCustomerAccountCreated(this, account);
            }
        }

        /// <summary>
        /// Synchronously raises <see cref="OnCustomerAccountDeleted"/> when subscribed.
        /// </summary>
        public void CustomerAccountDeleted(CustomerAccount account)
        {
            if (OnCustomerAccountDeleted != null)
            {
                OnCustomerAccountDeleted(this, account);
            }
        }

        /// <summary>
        /// Synchronously raises <see cref="OnCustomerAccountEmailChanged"/> when subscribed.
        /// </summary>
        public void CustomerAccountEmailChanged(string oldEmail, string newEmail)
        {
            if (OnCustomerAccountEmailChanged != null)
            {
                OnCustomerAccountEmailChanged(this, oldEmail, newEmail);
            }
        }

        /// <summary>
        /// Synchronously raises <see cref="OnOrderReceived"/> when subscribed.
        /// </summary>
        public void OrderReceived(Order order, HotcakesApplication app)
        {
            if (OnOrderReceived != null)
            {
                OnOrderReceived(this, order, app);
            }
        }
    }
}
