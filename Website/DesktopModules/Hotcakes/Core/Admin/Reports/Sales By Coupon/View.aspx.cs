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
using System.Linq;
using System.Web.UI.WebControls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Reports.Sales_By_Coupon
{
    public partial class View : BaseReportPage
    {
        #region Fields

        protected decimal OrderTotal;
        protected int OrdersCount;

        #endregion

        #region Event Handlers

        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);

            PageTitle = Localization.GetString("SalesByCoupon");
            PageMessageBox = ucMessageBox;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (!IsPostBack)
            {
                LoadCouponCodes();
            }
        }

        protected void dgList_OnItemDataBound(object sender, DataGridItemEventArgs e)
        {
            if (e.Item.ItemType == ListItemType.Header)
            {
                e.Item.Cells[0].Text = Localization.GetString("OrderNumber");
                e.Item.Cells[1].Text = Localization.GetString("CustomerEmail");
                e.Item.Cells[2].Text = Localization.GetString("Date");
                e.Item.Cells[3].Text = Localization.GetString("OrderTotal");
            }
        }

        #endregion

        #region Implementation

        private void LoadCouponCodes()
        {
            var codes = HccApp.MarketingServices.FindAllCouponCodes();

            if (codes.Count > 0)
            {
                lstCouponCode.DataSource = codes.OrderBy(y => y);
                lstCouponCode.DataBind();
            }
            else
            {
                ShowMessage(Localization.GetString("NoCouponCodes"), ErrorTypes.Info);
                lstCouponCode.Items.Add(new ListItem(Localization.GetString("NoCouponCodes")));
            }
        }

        protected override void BindReport()
        {
            if (lstCouponCode.Items.Count > 0)
            {
                var code = lstCouponCode.SelectedValue;

                var utcStart = ucDateRangePicker.GetStartDateUtc(HccApp);
                var utcEnd = ucDateRangePicker.GetEndDateUtc(HccApp);

                var possibleDateRange = HccApp.MarketingServices.FindDateRangeForCouponCode(code);
                if (possibleDateRange != null)
                {
                    if (possibleDateRange.StartDateUtc < utcStart)
                        possibleDateRange.StartDateUtc = utcStart;

                    if (possibleDateRange.EndDateUtc < utcEnd)
                        possibleDateRange.EndDateUtc = utcEnd;

                    var orders = HccApp.OrderServices.Orders.FindByCouponCode(code, possibleDateRange.StartDateUtc,
                        possibleDateRange.EndDateUtc);
                    if (orders == null) return;

                    orders.ForEach(
                        o => { o.TimeOfOrderUtc = DateHelper.ConvertUtcToStoreTime(HccApp, o.TimeOfOrderUtc); });

                    dgList.DataSource = orders;
                    dgList.DataBind();

                    OrderTotal = orders.Sum(y => y.TotalGrand);
                    OrdersCount = orders.Count;
                }
                else
                {
                    lblNoRecordsMessage.Text = Localization.GetString("NoRecords");
                }
            }
            else
            {
                lblNoRecordsMessage.Text = Localization.GetString("NoCouponCodes");
            }

            ShowNoRecordsMessage(OrdersCount == 0);
        }

        private void ShowNoRecordsMessage(bool show)
        {
            pnlReportData.Visible = !show;
            lblNoRecordsMessage.Visible = show;
        }

        #endregion
    }
}