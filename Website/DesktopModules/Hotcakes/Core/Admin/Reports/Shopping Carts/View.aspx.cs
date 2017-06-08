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