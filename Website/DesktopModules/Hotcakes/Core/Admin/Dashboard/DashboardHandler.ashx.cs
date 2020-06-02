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
using System.Collections.Generic;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Reporting;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Dashboard
{
    /// <summary>
    ///     Summary description for DashboardHandler
    /// </summary>
    public class DashboardHandler : BaseHandler, IHttpHandler
    {
        protected override object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            if (request.RequestContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                // not found
                request.RequestContext.HttpContext.Response.StatusCode = 404;
                request.RequestContext.HttpContext.Response.End();
                return null;
            }

            var ordersCount = hccApp.OrderServices.Orders.CountByCriteria(new OrderSearchCriteria {IsPlaced = true});
            if (ordersCount == 0)
                return SampleHandleAction(request, hccApp);

            var method = request.Params["method"];
            switch (method)
            {
                case "GetSalesData":
                    return GetSalesData(request, hccApp);
                case "GetProductPerformanceData":
                    return GetProductPerformanceData(request, hccApp);
                case "GetTopChangeByBouncesData":
                    return GetTopChangeByBouncesData(request, hccApp);
                case "GetTopChangeByAbandomentsData":
                    return GetTopChangeByAbandomentsData(request, hccApp);
                case "GetTopChangeByPurchasesData":
                    return GetTopChangeByPurchasesData(request, hccApp);
                case "GetTopChangeJointData":
                    return GetTopChangeJointData(request, hccApp);
                default:
                    break;
            }
            return true;
        }

        private object GetSalesData(HttpRequest request, HotcakesApplication hccApp)
        {
            var period = (SalesPeriod) request.Params["period"].ConvertTo((int) SalesPeriod.Month);
            var service = Factory.CreateService<ReportingService>();
            var sett = new DashboardUserSelections(hccApp);
            sett.Period1 = period;
            return new SalesInfoJson(service.GetSalesInfo(period), Localization);
        }

        private object GetProductPerformanceData(HttpRequest request, HotcakesApplication hccApp)
        {
            var period = (SalesPeriod) request.Params["period"].ConvertTo((int) SalesPeriod.Month);
            var service = Factory.CreateService<ReportingService>();
            var sett = new DashboardUserSelections(hccApp);
            sett.Period2 = period;

            var performanceInfo = service.GetProductPerformance(period);
            return new PerformanceInfoJson(performanceInfo, period, Localization);
        }

        private object GetTopChangeByBouncesData(HttpRequest request, HotcakesApplication hccApp)
        {
            var period = (SalesPeriod) request.Params["period"].ConvertTo((int) SalesPeriod.Month);
            var sortDirection = (SortDirection) Enum.Parse(typeof (SortDirection), request.Params["sortDirection"]);
            var pageNumber = request.Params["pageNumber"].ConvertTo(1);
            var pageSize = request.Params["pageSize"].ConvertTo(5);

            var service = Factory.CreateService<ReportingService>();
            var sett = new DashboardUserSelections(hccApp);
            sett.Period2 = period;

            var info = service.GetTopChangeByBounces(period, sortDirection, pageNumber, pageSize);
            return new TopChangeInfoJson(info, Localization, pageSize);
        }

        private object GetTopChangeByAbandomentsData(HttpRequest request, HotcakesApplication hccApp)
        {
            var period = (SalesPeriod) request.Params["period"].ConvertTo((int) SalesPeriod.Month);
            var sortDirection = (SortDirection) Enum.Parse(typeof (SortDirection), request.Params["sortDirection"]);
            var pageNumber = request.Params["pageNumber"].ConvertTo(1);
            var pageSize = request.Params["pageSize"].ConvertTo(5);

            var service = Factory.CreateService<ReportingService>();
            var sett = new DashboardUserSelections(hccApp);
            sett.Period2 = period;

            var info = service.GetTopChangeByAbandoments(period, sortDirection, pageNumber, pageSize);
            return new TopChangeInfoJson(info, Localization, pageSize);
        }

        private object GetTopChangeByPurchasesData(HttpRequest request, HotcakesApplication hccApp)
        {
            var period = (SalesPeriod) request.Params["period"].ConvertTo((int) SalesPeriod.Month);
            var sortDirection = (SortDirection) Enum.Parse(typeof (SortDirection), request.Params["sortDirection"]);
            var pageNumber = request.Params["pageNumber"].ConvertTo(1);
            var pageSize = request.Params["pageSize"].ConvertTo(5);

            var service = Factory.CreateService<ReportingService>();
            var sett = new DashboardUserSelections(hccApp);
            sett.Period2 = period;

            var info = service.GetTopChangeByPurchases(period, sortDirection, pageNumber, pageSize);
            return new TopChangeInfoJson(info, Localization, pageSize);
        }

        private object GetTopChangeJointData(HttpRequest request, HotcakesApplication hccApp)
        {
            var period = (SalesPeriod) request.Params["period"].ConvertTo((int) SalesPeriod.Month);
            var sortBy = (TopAffectedSort) Enum.Parse(typeof (TopAffectedSort), request.Params["sortBy"]);
            var sortDirection = (SortDirection) Enum.Parse(typeof (SortDirection), request.Params["sortDirection"]);
            var pageNumber = request.Params["pageNumber"].ConvertTo(1);
            var pageSize = request.Params["pageSize"].ConvertTo(5);

            var service = Factory.CreateService<ReportingService>();
            var sett = new DashboardUserSelections(hccApp);
            sett.Period2 = period;

            var info = service.GetTopAffectedProducts(period, sortBy, sortDirection, pageNumber, pageSize);
            return new TopChangeInfoJson(info, Localization, pageSize);
        }

        #region Sample Data

        private object SampleHandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            var method = request.Params["method"];

            switch (method)
            {
                case "GetSalesData":
                    return GetSampleSalesData(request, hccApp);
                case "GetProductPerformanceData":
                    return GetSampleProductPerformanceData(request, hccApp);
                case "GetTopChangeByBouncesData":
                    return GetSampleTopChangeByBouncesData(request, hccApp);
                case "GetTopChangeByAbandomentsData":
                    return GetSampleTopChangeByAbandomentsData(request, hccApp);
                case "GetTopChangeByPurchasesData":
                    return GetSampleTopChangeByPurchasesData(request, hccApp);
                case "GetTopChangeJointData":
                    return GetSampleTopChangeJointData(request, hccApp);
                default:
                    break;
            }
            return true;
        }

        private object GetSampleSalesData(HttpRequest request, HotcakesApplication hccApp)
        {
            var info = new SalesInfo
            {
                ChartLabels =
                    new List<string>(new[]
                    {"Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"}),
                ChartData =
                    new List<decimal>(new decimal[]
                    {
                        1040000, 1700000, 700000, 1400000, 1400000, 1700000, 1200000, 1400000, 1700000, 1700000, 1360000,
                        1020000
                    }),
                OrdersCount = 30,
                OrdersCompleted = 10,
                OrdersTotalSum = 16300000,
                OrdersSuccessfulTransactions = 27000,
                SalesByDesktopCount = 14000,
                SalesByDesktopPercent = 70,
                SalesByDesktopPercentPrev = 80,
                SalesByTabletCount = 4000,
                SalesByTabletPercent = 20,
                SalesByTabletPercentPrev = 10,
                SalesByPhoneCount = 2000,
                SalesByPhonePercent = 10,
                SalesByPhonePercentPrev = 10
            };

            return new SalesInfoJson(info, Localization);
        }

        private object GetSampleProductPerformanceData(HttpRequest request, HotcakesApplication hccApp)
        {
            var info = new PerformanceInfo
            {
                BouncedData = new List<int>(new[] {450, 350, 300, 230, 240, 260, 240, 280, 280, 320, 340, 360}),
                AbandonedData = new List<int>(new[] {100, 140, 180, 220, 260, 300, 580, 600, 750, 825, 850, 850}),
                PurchasedData = new List<int>(new[] {250, 300, 400, 550, 580, 580, 400, 750, 780, 680, 800, 750}),
                ChartLabels =
                    new List<string>(new[]
                    {
                        "January", "February", "March", "April", "May", "June", "July", "August", "September", "October",
                        "November", "December"
                    }),
                Events = new List<string[]>(),
                Views = 100000,
                AddsToCart = 50000,
                Purchases = 30000,
                ViewsPrev = 83333,
                AddsToCartPrev = 60000,
                PurchasesPrev = 36666
            };
            return new PerformanceInfoJson(info, SalesPeriod.Year, Localization);
        }

        private object GetSampleTopChangeByBouncesData(HttpRequest request, HotcakesApplication hccApp)
        {
            var info = new TopChangeInfo
            {
                TotalCount = 50,
                Items = new List<TopChangeItemInfo>
                {
                    CreateTopChangeItem("Blue Bracelet", 4.11m),
                    CreateTopChangeItem("Brown Fedora", -0.85m),
                    CreateTopChangeItem("Butterfly Earrings", 0.16m),
                    CreateTopChangeItem("Cupcake", -0.08m),
                    CreateTopChangeItem("Laptop Computer", 0)
                }
            };
            return new TopChangeInfoJson(info, Localization, 5);
        }

        private object GetSampleTopChangeByAbandomentsData(HttpRequest request, HotcakesApplication hccApp)
        {
            var info = new TopChangeInfo
            {
                TotalCount = 50,
                Items = new List<TopChangeItemInfo>
                {
                    CreateTopChangeItem("Purple Top", 12m),
                    CreateTopChangeItem("Brown Fedora", -9.1m),
                    CreateTopChangeItem("White Shoes", 0.99m),
                    CreateTopChangeItem("Cupcake", -0.62m),
                    CreateTopChangeItem("Baseball Bat", 0.03m)
                }
            };
            return new TopChangeInfoJson(info, Localization, 5);
        }

        private object GetSampleTopChangeByPurchasesData(HttpRequest request, HotcakesApplication hccApp)
        {
            var info = new TopChangeInfo
            {
                TotalCount = 50,
                Items = new List<TopChangeItemInfo>
                {
                    CreateTopChangeItem("White Shoes", 2.2m),
                    CreateTopChangeItem("Butterfly Earrings", -.9m),
                    CreateTopChangeItem("Soccer Ball", 0.24m),
                    CreateTopChangeItem("Office Chair", -0.07m),
                    CreateTopChangeItem("Purple Top", 0.01m)
                }
            };
            return new TopChangeInfoJson(info, Localization, 5);
        }

        private object GetSampleTopChangeJointData(HttpRequest request, HotcakesApplication hccApp)
        {
            var info = new TopChangeInfo
            {
                TotalCount = 50,
                Items = new List<TopChangeItemInfo>
                {
                    CreateTopChangeItem("White Shoes", 4.61m, -3.4m, 0.99m, 2.2m),
                    CreateTopChangeItem("Purple Top", -2.99m, -9m, 12m, 0.01m),
                    CreateTopChangeItem("Soccer Ball", -0.8m, 0, 1.04m, 0.24m),
                    CreateTopChangeItem("Office Chair", 0.07m, -0.06m, -0.08m, -0.07m),
                    CreateTopChangeItem("Baseball Bat", -0.05m, 0.02m, 0.03m, 0)
                }
            };
            return new TopChangeInfoJson(info, Localization, 5);
        }

        private TopChangeItemInfo CreateTopChangeItem(string productName, decimal change,
            decimal bouncesChange = 0, decimal abandomentsChange = 0, decimal purchasesChange = 0)
        {
            return new TopChangeItemInfo
            {
                ProductId = Guid.NewGuid(),
                ProductName = productName,
                BouncesChange = bouncesChange,
                AbandomentsChange = abandomentsChange,
                PurchasesChange = purchasesChange,
                Change = change
            };
        }

        #endregion
    }
}