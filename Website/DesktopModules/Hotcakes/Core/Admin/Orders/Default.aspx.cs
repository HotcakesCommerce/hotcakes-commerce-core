#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Text;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Orders
{
	partial class Default : BaseAdminPage
	{
		protected override void OnPreInit(EventArgs e)
		{
			base.OnPreInit(e);
			PageTitle = Localization.GetString("PageTitle");
			CurrentTab = AdminTabType.Orders;
			ValidateCurrentUserHasPermission(SystemPermissions.OrdersView);
		}

		protected override void OnInit(EventArgs e)
		{
			base.OnInit(e);
			lnkExportToExcel.Click += lnkExportToExcel_Click;
			lnkExportToQuickbooks.Click += lnkExportToQuickbooks_Click;
			gvOrders.RowDataBound += gvOrders_RowDataBound;
		}

		protected override void OnLoad(EventArgs e)
		{
			base.OnLoad(e);

            DateRangePicker1.RangeTypeChanged += DateRangePicker1_RangeTypeChanged;
			if (!Page.IsPostBack)
			{
				LoadSessionKeys();
				// Force Mode on Request
				if (Request.QueryString["mode"] != null)
				{
					switch (Request.QueryString["mode"].ToLower())
					{
						case "0":
							SetListToValue(lstStatus, string.Empty);
							break;
						case "1":
							SetListToValue(lstStatus, OrderStatusCode.Received);
							break;
						case "2":
							SetListToValue(lstStatus, OrderStatusCode.ReadyForPayment);
							break;
						case "3":
							SetListToValue(lstStatus, OrderStatusCode.ReadyForShipping);
							break;
						case "4":
							SetListToValue(lstStatus, OrderStatusCode.Completed);
							break;
						case "5":
							SetListToValue(lstStatus, OrderStatusCode.OnHold);
							break;
						case "6":
							SetListToValue(lstStatus, OrderStatusCode.Cancelled);
							break;
					}
				}

				LoadTemplates();
			}

            var pageNumber = 1;
			if (Request.QueryString["p"] != null)
			{
                var tempPage = Request.QueryString["p"];
                var temp = 0;
				if (int.TryParse(tempPage, out temp))
				{
					if (temp > 0)
					{
						pageNumber = temp;
					}
				}
			}

            SessionManager.SetCookieString("AdminLastManager", "Default.aspx?p=" + pageNumber);

            LocalizeView();

			FindOrders(pageNumber);
		}

        private void gvOrders_RowDataBound(object sender, GridViewRowEventArgs e)
		{
			if (e.Row.RowType == DataControlRowType.DataRow)
			{
				var orderSnapshot = e.Row.DataItem as OrderSnapshot;

				// Highlights
                var highlight = HccApp.OrderServices.FindHighlightForOrder(orderSnapshot);
				if (!string.IsNullOrEmpty(highlight))
				{
					e.Row.CssClass = "hc" + highlight;
				}
				
				var strongAmount = e.Row.FindControl("strongAmount") as HtmlGenericControl;
				var spanRecurringInfo = e.Row.FindControl("spanRecurringInfo") as HtmlGenericControl;
				var spanRecurringPopup = e.Row.FindControl("spanRecurringPopup") as HtmlGenericControl;
				var btnDetails = e.Row.FindControl("btnDetails");
				var btnPayment = e.Row.FindControl("btnPayment");
				var btnShipping = e.Row.FindControl("btnShipping");

				strongAmount.InnerText = orderSnapshot.TotalGrand.ToString("C");

                btnPayment.Visible = orderSnapshot.StatusCode == OrderStatusCode.ReadyForPayment;
                btnShipping.Visible = orderSnapshot.StatusCode == OrderStatusCode.ReadyForShipping;
				btnDetails.Visible = !btnPayment.Visible && !btnShipping.Visible;

				if (orderSnapshot.IsRecurring)
				{
					strongAmount.Visible = false;
					spanRecurringInfo.Visible = true;

					var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderSnapshot.bvin);
					var sb = new StringBuilder();
					order.Items.ForEach(i =>
						{
                        var lineInfo = Localization.GetFormattedString("Every", i.LineTotal.ToString("C"),
                            i.RecurringBilling.Interval,
                            LocalizationUtils.GetRecurringIntervalLower(i.RecurringBilling.IntervalType));
							sb.AppendFormat("{0} <br />", lineInfo);
						});
					spanRecurringPopup.InnerHtml = sb.ToString();
				}
			}
		}

		private void LoadTemplates()
		{
			lstPrintTemplate.DataSource = HccApp.ContentServices.GetAllOrderTemplates();
			lstPrintTemplate.DataTextField = "DisplayName";
			lstPrintTemplate.DataValueField = "Id";
			lstPrintTemplate.DataBind();
		}

        private void LocalizeView()
        {
            LocalizationUtils.LocalizeGridView(gvOrders, Localization);
        }

		// Searching
		private void FindOrders(int pageNumber)
		{
            var criteria = new OrderSearchCriteria();
			criteria.IsPlaced = true;
			criteria.StatusCode = lstStatus.SelectedValue;
			if (lstPaymentStatus.SelectedValue != string.Empty)
			{
                criteria.PaymentStatus = (OrderPaymentStatus) int.Parse(lstPaymentStatus.SelectedValue);
			}
			if (lstShippingStatus.SelectedValue != string.Empty)
			{
                criteria.ShippingStatus = (OrderShippingStatus) int.Parse(lstShippingStatus.SelectedValue);
			}
			criteria.StartDateUtc = DateRangePicker1.StartDate.ToUniversalTime();
			criteria.EndDateUtc = DateRangePicker1.EndDate.ToUniversalTime();
			criteria.Keyword = FilterField.Text.Trim();
			criteria.SortDescending = chkNewestFirst.Checked;
			criteria.IsIncludeCanceledOrder = true;
			criteria.IncludeUnplaced = false;

            var pageSize = 20;
            var totalCount = 0;

            var orders = HccApp.OrderServices.Orders.FindByCriteriaPaged(criteria, pageNumber, pageSize, ref totalCount);

			gvOrders.DataSource = orders;
			gvOrders.DataBind();

            litPager.Text =
                Paging.RenderPager(ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Orders/default.aspx?p={0}"),
                    pageNumber, totalCount, pageSize);
			litPager2.Text = litPager.Text;

			SaveSessionKeys(pageNumber);
			ShowCorrectBatchButtons(criteria.StatusCode);
		}

		private void SaveSessionKeys(int pageNumber)
		{
			SessionManager.AdminOrderSearchDateRange = DateRangePicker1.RangeType;
			SessionManager.AdminOrderSearchEndDate = DateRangePicker1.EndDate;
			SessionManager.AdminOrderSearchStartDate = DateRangePicker1.StartDate;
			SessionManager.AdminOrderSearchKeyword = FilterField.Text.Trim();
			SessionManager.AdminOrderSearchPaymentFilter = lstPaymentStatus.SelectedValue;
			SessionManager.AdminOrderSearchShippingFilter = lstShippingStatus.SelectedValue;
			SessionManager.AdminOrderSearchStatusFilter = lstStatus.SelectedValue;
			SessionManager.AdminOrderSearchLastPage = pageNumber;
			SessionManager.AdminOrderSearchNewestFirst = chkNewestFirst.Checked;
		}

		private void SetListToValue(DropDownList l, string value)
		{
			if (l == null) return;
			if (l.Items.Count < 1) return;
			if (l.Items.FindByValue(value) != null)
			{
				l.ClearSelection();
				l.Items.FindByValue(value).Selected = true;
			}
		}

		private void LoadSessionKeys()
		{
			FilterField.Text = SessionManager.AdminOrderSearchKeyword;

			SetListToValue(lstPaymentStatus, SessionManager.AdminOrderSearchPaymentFilter);
			SetListToValue(lstShippingStatus, SessionManager.AdminOrderSearchShippingFilter);
			SetListToValue(lstStatus, SessionManager.AdminOrderSearchStatusFilter);
			DateRangePicker1.RangeType = SessionManager.AdminOrderSearchDateRange;
			if (DateRangePicker1.RangeType == DateRangeType.Custom)
			{
				DateRangePicker1.StartDate = SessionManager.AdminOrderSearchStartDate;
				DateRangePicker1.EndDate = SessionManager.AdminOrderSearchEndDate;
			}
			chkNewestFirst.Checked = SessionManager.AdminOrderSearchNewestFirst;
		}

		// Rendering
		protected string GetTimeOfOrder(IDataItemContainer cont)
		{
			var o = cont.DataItem as OrderSnapshot;
			var timeOfOrder = DateHelper.ConvertUtcToStoreTime(HccApp, o.TimeOfOrderUtc);
			var currentTime = DateHelper.ConvertUtcToStoreTime(HccApp);
			return Dates.FriendlyShortDate(timeOfOrder, currentTime.Year);
		}

		protected string RenderCustomerMailToLink(IDataItemContainer cont)
		{
			var o = cont.DataItem as OrderSnapshot;
            return MailServices.MailToLink(o.UserEmail,
                string.Format(Localization.GetString("OrderSubject"), o.OrderNumber),
                string.Concat(o.BillingAddress.FirstName, ","),
                string.Concat(o.BillingAddress.FirstName, " ", o.BillingAddress.LastName));
        }

		protected string RenderStatusHtml(IDataItemContainer cont)
		{
			var o = cont.DataItem as OrderSnapshot;
			var sb = new StringBuilder();
            var url = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Orders/ViewOrder.aspx?id=" + o.bvin);

            var payText = LocalizationUtils.GetOrderPaymentStatus(o.PaymentStatus,
                HccRequestContext.Current.MainContentCulture);
            var payImage = "";
            var payLink = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Orders/OrderPayments.aspx?id=" + o.bvin);
			switch (o.PaymentStatus)
			{
				case OrderPaymentStatus.Overpaid:
					payImage = ResolveImgUrl("Lights/PaymentError.gif");
					break;
				case OrderPaymentStatus.PartiallyPaid:
					payImage = ResolveImgUrl("Lights/PaymentAuthorized.gif");
					break;
				case OrderPaymentStatus.Paid:
					payImage = ResolveImgUrl("Lights/PaymentComplete.gif");
					break;
				case OrderPaymentStatus.Unknown:
					payImage = ResolveImgUrl("Lights/PaymentNone.gif");
					break;
				case OrderPaymentStatus.Unpaid:
					payImage = ResolveImgUrl("Lights/PaymentNone.gif");
					break;
			}
            sb.Append("<a href=\"" + payLink + "\" title=\"" + payText + "\"><img src=\"" + payImage + "\" alt=\"" +
                      payText + "\" /></a>");


            var shipText = LocalizationUtils.GetOrderShippingStatus(o.ShippingStatus,
                HccRequestContext.Current.MainContentCulture);
            var shipImage = "";
            var shipLink = ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Orders/ShipOrder.aspx?id=" + o.bvin);
			switch (o.ShippingStatus)
			{
				case OrderShippingStatus.FullyShipped:
					shipImage = ResolveImgUrl("Lights/ShippingShipped.gif");
					break;
				case OrderShippingStatus.NonShipping:
					shipImage = ResolveImgUrl("Lights/ShippingNone.gif");
					break;
				case OrderShippingStatus.PartiallyShipped:
					shipImage = ResolveImgUrl("Lights/ShippingPartially.gif");
					break;
				case OrderShippingStatus.Unknown:
					shipImage = ResolveImgUrl("Lights/ShippingNone.gif");
					break;
				case OrderShippingStatus.Unshipped:
					shipImage = ResolveImgUrl("Lights/ShippingNone.gif");
					break;
			}
            sb.Append("<a href=\"" + shipLink + "\" title=\"" + shipText + "\"><img src=\"" + shipImage + "\" alt=\"" +
                      shipText + "\" /></a>");

            var statusText = LocalizationUtils.GetOrderStatus(o.StatusName, HccRequestContext.Current.MainContentCulture);
            var statImage = "";
			switch (o.StatusCode)
			{
				case OrderStatusCode.Completed:
					statImage = ResolveImgUrl("lights/OrderComplete.gif");
					break;
				case OrderStatusCode.Received:
					statImage = ResolveImgUrl("lights/OrderInProcess.gif");
					break;
				case OrderStatusCode.OnHold:
					statImage = ResolveImgUrl("lights/OrderOnHold.gif");
					break;
				case OrderStatusCode.ReadyForPayment:
					statImage = ResolveImgUrl("lights/OrderInProcess.gif");
					break;
				case OrderStatusCode.ReadyForShipping:
					statImage = ResolveImgUrl("lights/OrderInProcess.gif");
					break;
				default:
					statImage = ResolveImgUrl("lights/OrderInProcess.gif");
					break;
			}
            sb.Append("<a href=\"" + url + "\"><img src=\"" + statImage + "\" alt=\"" + statusText +
                      "\" style='margin-right:4px' /></a>");
			sb.Append("<div><a href=\"" + url + "\">" + statusText + "</a></div>");

			return sb.ToString();
		}

		private string ResolveImgUrl(string path)
		{
			return ResolveUrl("~/DesktopModules/Hotcakes/Core/Admin/Images/" + path);
		}

		#region Event handlers

		// Search & Auto Post Back Handlers
        private void lnkExportToExcel_Click(object sender, EventArgs e)
        {
            var criteria = new OrderSearchCriteria
		{
                IsPlaced = true,
                StatusCode = lstStatus.SelectedValue
            };
			if (lstPaymentStatus.SelectedValue != string.Empty)
			{
                criteria.PaymentStatus = (OrderPaymentStatus) int.Parse(lstPaymentStatus.SelectedValue);
			}
			if (lstShippingStatus.SelectedValue != string.Empty)
			{
                criteria.ShippingStatus = (OrderShippingStatus) int.Parse(lstShippingStatus.SelectedValue);
			}
			criteria.StartDateUtc = DateRangePicker1.StartDate.ToUniversalTime();
			criteria.EndDateUtc = DateRangePicker1.EndDate.ToUniversalTime();
			criteria.Keyword = FilterField.Text.Trim();
			criteria.SortDescending = chkNewestFirst.Checked;
			criteria.IsIncludeCanceledOrder = true;

            var pageSize = int.MaxValue;
            var totalCount = 0;

            var orders = HccApp.OrderServices.Orders.FindByCriteriaPaged(criteria, 1, pageSize, ref totalCount);
			var ordersExport = new OrdersExport(HccApp);
			ordersExport.ExportToExcel(Response, "Hotcakes_Orders.xlsx", orders);
		}

        private void lnkExportToQuickbooks_Click(object sender, EventArgs e)
        {
            var criteria = new OrderSearchCriteria
		{
                IsPlaced = true,
                StatusCode = lstStatus.SelectedValue
            };
			if (lstPaymentStatus.SelectedValue != string.Empty)
			{
                criteria.PaymentStatus = (OrderPaymentStatus) int.Parse(lstPaymentStatus.SelectedValue);
			}
			if (lstShippingStatus.SelectedValue != string.Empty)
			{
                criteria.ShippingStatus = (OrderShippingStatus) int.Parse(lstShippingStatus.SelectedValue);
			}
			criteria.StartDateUtc = DateRangePicker1.StartDate.ToUniversalTime();
			criteria.EndDateUtc = DateRangePicker1.EndDate.ToUniversalTime();
			criteria.Keyword = FilterField.Text.Trim();
			criteria.SortDescending = chkNewestFirst.Checked;
			criteria.IsIncludeCanceledOrder = true;

            var pageSize = int.MaxValue;
            var totalCount = 0;

            var orders = HccApp.OrderServices.Orders.FindByCriteriaPaged(criteria, 1, pageSize, ref totalCount);
			var ordersExport = new QuickbooksOrdersExport(HccApp);
			ordersExport.ExportToQuickbooks(Response, "Hotcakes_Orders.iif", orders);
		}

		protected void btnGo_Click(object sender, EventArgs e)
		{
			FindOrders(1);
		}

		protected void lstStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			FindOrders(1);
		}

		protected void lstPaymentStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			FindOrders(1);
		}

		protected void lstShippingStatus_SelectedIndexChanged(object sender, EventArgs e)
		{
			FindOrders(1);
		}

        private void DateRangePicker1_RangeTypeChanged(object sender, EventArgs e)
		{
			if (IsPostBack)
			{
				FindOrders(1);
			}
		}

		protected void chkNewestFirst_CheckedChanged(object sender, EventArgs e)
		{
			FindOrders(1);
		}

		// Batch Actions
		private void ShowCorrectBatchButtons(string statusCode)
		{
			OrderManagerActions.Visible = false;
			lnkAcceptAll.Visible = false;
			lnkPrintPacking.Visible = false;
			lnkShipAll.Visible = false;
			lnkChargeAll.Visible = false;

			switch (statusCode)
			{
				case OrderStatusCode.Received:
					lnkAcceptAll.Visible = true;
					OrderManagerActions.Visible = true;
					litH1.Text = Localization.GetString("lblNewOrders");
					break;
				case OrderStatusCode.Cancelled:
					litH1.Text = Localization.GetString("lblCanceledOrders");
                    break;
				case OrderStatusCode.Completed:
					litH1.Text = Localization.GetString("lblCompletedOrders");
					break;
				case OrderStatusCode.OnHold:
					litH1.Text = Localization.GetString("lblOrdersOnHold");
					break;
				case OrderStatusCode.ReadyForPayment:
					litH1.Text = Localization.GetString("lblOrdersReady");
					lnkChargeAll.Visible = true;
					OrderManagerActions.Visible = true;
					break;
				case OrderStatusCode.ReadyForShipping:
					litH1.Text = Localization.GetString("lblShippingReady");
					break;
				default:
					litH1.Text = Localization.GetString("PageTitle");
					break;
			}
        }

		protected void lnkAcceptAll_Click(object sender, EventArgs e)
		{
			OrderBatchProcessor.AcceptAllNewOrders(HccApp.OrderServices);
			FindOrders(1);
		}

		protected void lnkShipAll_Click(object sender, EventArgs e)
		{
			FindOrders(1);
		}

		protected void lnkPrintPacking_Click(object sender, EventArgs e)
		{
			FindOrders(1);
		}

		protected void lnkChargeAll_Click(object sender, EventArgs e)
		{
			OrderBatchProcessor.CollectPaymentAndShipPendingOrders(HccApp);
			FindOrders(1);
		}

		#endregion
	}
}