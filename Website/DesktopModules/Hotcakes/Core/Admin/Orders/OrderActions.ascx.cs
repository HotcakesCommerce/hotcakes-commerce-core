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
using Hotcakes.Commerce;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Orders
{
    /// <summary>
    /// Provides the order action controls used by the administrative order view.
    /// </summary>
    public partial class OrderActions : HccUserControl
    {
        /// <summary>Gets or sets the page used to display the current order.</summary>
        public string CurrentOrderPage { get; set; }

        /// <summary>Gets or sets whether order actions are hidden.</summary>
        public bool HideActions { get; set; }

        /// <summary>Gets whether multiple order templates are available.</summary>
        public bool AreMultipleTemplates
        {
            get
            {
                var id = Request.QueryString["id"];
                return !string.IsNullOrEmpty(id) && id.Contains(",");
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!Page.IsPostBack)
            {
                var id = Request.QueryString["id"];
                lnkDetails.NavigateUrl = "ViewOrder.aspx?id=" + id;
                lnkEditOrder.NavigateUrl = "EditOrder.aspx?id=" + id;
                lnkPayment.NavigateUrl = "OrderPayments.aspx?id=" + id;
                lnkPrint.NavigateUrl = "PrintOrder.aspx?id=" + id;
                lnkShipping.NavigateUrl = "ShipOrder.aspx?id=" + id;

                var lastManagerPage = SessionManager.GetCookieString("AdminLastManager");
                if (!string.IsNullOrEmpty(lastManagerPage))
                {
                    lnkManager.NavigateUrl = lastManagerPage;
                }
                else
                {
                    var lastPage = SessionManager.AdminOrderSearchLastPage;
                    if (lastPage < 1)
                    {
                        lastPage = 1;
                    }
                    lnkManager.NavigateUrl = "Default.aspx?p=" + lastPage;
                }
            }
        }

        protected string GetCurrentCssClass(string page)
        {
            return page == CurrentOrderPage ? "hcCurrent" : string.Empty;
        }
    }
}
