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
using System.Globalization;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Dashboard
{
    public partial class Dashboard : HccUserControl
    {
        protected int ReviewsCount;

        protected OrdersSummaryData OrdersSummary { get; set; }

        protected string GetCurrencyHtmlLabel()
        {
            var ci = CultureInfo.CurrentUICulture;
            var position = (ci.NumberFormat.CurrencyPositivePattern & 1) == 1 ? " class='hcRight'" : string.Empty;
            return string.Format("<label{0}>{1}</label>", position, ci.NumberFormat.CurrencySymbol);
        }

        protected void lnkAddSamples_Click(object sender, EventArgs e)
        {
            SampleData.CreateSampleAnalyticsForStoreAsync();

            Response.Redirect("~/DesktopModules/Hotcakes/Core/Admin/catalog/default.aspx");
        }

        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            var productsCount = HccApp.CatalogServices.Products.FindAllCount();
            var ordersCount = HccApp.OrderServices.Orders.CountByCriteria(new OrderSearchCriteria {IsPlaced = true});

            if (ordersCount != 0)
            {
                var repService = Factory.CreateService<ReportingService>();
                OrdersSummary = repService.GetOrdersSummary();
            }
            else
            {
                OrdersSummary = GetSampleOrdersSummary();
            }
            ReviewsCount = HccApp.CatalogServices.ProductReviews.FindNotApproved(1, int.MaxValue).Count;

            divGetStarted.Visible = productsCount == 0;
            divSampleData.Visible = ordersCount == 0;
            divSampleDataOverlay.Visible = ordersCount == 0;

            if (!IsPostBack)
            {
                if (ordersCount != 0)
                {
                    var sett = new DashboardUserSelections(HccApp);

                    ddlRow1Period.SelectedValue = ((int) sett.Period1).ToString();
                    ddlRow2Period.SelectedValue = ((int) sett.Period2).ToString();
                }
                else
                {
                    ddlRow1Period.SelectedValue = ((int) SalesPeriod.Month).ToString();
                    ddlRow2Period.SelectedValue = ((int) SalesPeriod.Month).ToString();
                }
            }
        }

        #region Sample Data

        private OrdersSummaryData GetSampleOrdersSummary()
        {
            return new OrdersSummaryData
            {
                NewCount = 50,
                OnHoldCount = 10,
                ReadyForPaymentCount = 10,
                ReadyForShippingCount = 20
            };
        }

        #endregion
    }
}