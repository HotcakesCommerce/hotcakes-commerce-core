#region License

// Distributed under the MIT License
// ============================================================
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
using System.Collections.Generic;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.AppCode;

namespace Hotcakes.Modules.Core.Admin.Reports.Shopping_Carts
{

    partial class View : BaseAdminPage
    {

        private decimal TotalSub = 0;
        private int TotalCount = 0;

        protected override void OnPreInit(EventArgs e)
        {
            base.OnPreInit(e);
            this.PageTitle = "Reports - Shopping Carts";
            this.CurrentTab = AdminTabType.Reports;
            ValidateCurrentUserHasPermission(SystemPermissions.ReportsView);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
            if (!Page.IsPostBack)
            {
                LoadData();
            }
        }

        void LoadData()
        {

            try
            {

                TotalSub = 0;
                TotalCount = 0;

                OrderSearchCriteria c = new OrderSearchCriteria();
                c.IsPlaced = false;
				c.SortDescending = true;

                List<OrderSnapshot> found = new List<OrderSnapshot>();
                int totalCarts = 0;
                found = HccApp.OrderServices.Orders.FindByCriteriaPaged(c, 1, 1000, ref totalCarts);

                TotalCount = found.Count;

                foreach (OrderSnapshot o in found)
                {
                    TotalSub += o.TotalOrderBeforeDiscounts;
                }

                dgList.DataSource = found;
                dgList.DataBind();
            }

            catch (Exception Ex)
            {
                msg.ShowException(Ex);
                EventLog.LogEvent(Ex);
            }

        }

        protected void dgList_ItemDataBound(object sender, System.Web.UI.WebControls.DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Item || e.Item.ItemType == ListItemType.AlternatingItem)
            {
                OrderSnapshot order = (OrderSnapshot)e.Item.DataItem;
            }

            if (e.Item.ItemType == ListItemType.Footer)
            {
                e.Item.Cells[0].Text = "Totals:";
                e.Item.Cells[2].Text = string.Format("{0:C}", TotalSub);
            }

        }

        protected void dgList_Edit(object sender, DataGridCommandEventArgs e)
        {
            string bvin = Convert.ToString(dgList.DataKeys[e.Item.ItemIndex]);
            Response.Redirect("~/HCC/Admin/Orders/ViewOrder.aspx?id=" + bvin);
        }

    }
}