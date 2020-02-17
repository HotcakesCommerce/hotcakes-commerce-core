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

using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce
{
    public class Integration
    {
        public delegate void CustomerAccountEmailChangedHandler(object sender, string oldEmail, string newEmail);

        public delegate void CustomerAccountEventHandler(object sender, CustomerAccount account);

        public delegate void OrderEventHandler(object sender, Order order, HotcakesApplication app);

        public event CustomerAccountEventHandler OnCustomerAccountUpdated;
        public event CustomerAccountEventHandler OnCustomerAccountCreated;
        public event CustomerAccountEventHandler OnCustomerAccountDeleted;
        public event CustomerAccountEmailChangedHandler OnCustomerAccountEmailChanged;
        public event OrderEventHandler OnOrderReceived;

        public void CustomerAccountUpdated(CustomerAccount account)
        {
            if (OnCustomerAccountUpdated != null)
            {
                OnCustomerAccountUpdated(this, account);
            }
        }

        public void CustomerAccountCreated(CustomerAccount account)
        {
            if (OnCustomerAccountCreated != null)
            {
                OnCustomerAccountCreated(this, account);
            }
        }

        public void CustomerAccountDeleted(CustomerAccount account)
        {
            if (OnCustomerAccountDeleted != null)
            {
                OnCustomerAccountDeleted(this, account);
            }
        }

        public void CustomerAccountEmailChanged(string oldEmail, string newEmail)
        {
            if (OnCustomerAccountEmailChanged != null)
            {
                OnCustomerAccountEmailChanged(this, oldEmail, newEmail);
            }
        }

        public void OrderReceived(Order order, HotcakesApplication app)
        {
            if (OnOrderReceived != null)
            {
                OnOrderReceived(this, order, app);
            }
        }
    }
}