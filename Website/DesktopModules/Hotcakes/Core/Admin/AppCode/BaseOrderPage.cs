#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Modules.Core.Admin.AppCode
{
    [Serializable]
    public abstract class BaseOrderPage : BaseAdminPage
    {
        #region Properties

        public string OrderId
        {
            get
            {
                var id = Request.QueryString["id"];
                if (!string.IsNullOrEmpty(id))
                    return id;
                return (string) ViewState["OrderId"] ?? string.Empty;
            }
            set { ViewState["OrderId"] = value; }
        }

        public string ReturnUrl
        {
            get { return Request.QueryString["returnUrl"]; }
        }

        public string RmaId
        {
            get
            {
                var rmaid = Request.QueryString["rmaid"];
                if (!string.IsNullOrEmpty(rmaid))
                    return rmaid;
                return (string) ViewState["RmaId"] ?? string.Empty;
            }
            set { ViewState["RmaId"] = value; }
        }

        private Order _currentOrder;

        public Order CurrentOrder
        {
            get
            {
                if (_currentOrder == null)
                {
                    _currentOrder = HccApp.OrderServices.Orders.FindForCurrentStore(OrderId);

                    if (_currentOrder != null && _currentOrder.IsRecurring)
                    {
                        foreach (var li in _currentOrder.Items)
                        {
                            li.RecurringBilling.LoadPaymentInfo(HccApp);
                        }
                    }
                }
                return _currentOrder;
            }
        }

        #endregion
    }
}