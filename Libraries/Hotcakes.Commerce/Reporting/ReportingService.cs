#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.SqlServer;
using System.Globalization;
using System.Linq;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Marketing;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;
using Hotcakes.Web.Data;
using StackExchange.Profiling;

namespace Hotcakes.Commerce.Reporting
{
    public class ReportingService : HccServiceBase
    {
        #region Constructor

        public ReportingService(HccRequestContext context)
            : base(context)
        {
        }

        #endregion

        #region Internal declaration

        public class AffiliateItem
        {
            public decimal? Amount;
            public int Count;
            public long Id;
            public string Name;
            public int UserId;
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Gets the orders summary.
        /// </summary>
        /// <returns></returns>
        public OrdersSummaryData GetOrdersSummary()
        {
            var data = new OrdersSummaryData();

            using (var db = CreateReadOrderStrategy())
            {
                var storeId = Context.CurrentStore.Id;

                var summList =
                    db.GetQuery(
                        o => o.StoreId == storeId && o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .AsNoTracking()
                        .GroupBy(o => new {o.StatusCode})
                        .Select(g => new {g.Key.StatusCode, OrderCount = g.Count()})
                        .OrderBy(o => o.StatusCode)
                        .ToList();

                var resReceived = summList.FirstOrDefault(o => o.StatusCode == OrderStatusCode.Received);
                var resHold = summList.FirstOrDefault(o => o.StatusCode == OrderStatusCode.OnHold);
                var resReadyPayment = summList.FirstOrDefault(o => o.StatusCode == OrderStatusCode.ReadyForPayment);
                var resReadyShipping = summList.FirstOrDefault(o => o.StatusCode == OrderStatusCode.ReadyForShipping);

                data.NewCount = resReceived != null ? resReceived.OrderCount : 0;
                data.OnHoldCount = resHold != null ? resHold.OrderCount : 0;
                data.ReadyForPaymentCount = resReadyPayment != null ? resReadyPayment.OrderCount : 0;
                data.ReadyForShippingCount = resReadyShipping != null ? resReadyShipping.OrderCount : 0;
            }

            return data;
        }

        /// <summary>
        ///     Gets the sales information.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        public SalesInfo GetSalesInfo(SalesPeriod period)
        {
            using (MiniProfiler.Current.Step("GetSalesInfo"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var payedStatuses = new[] {(int) OrderPaymentStatus.Paid, (int) OrderPaymentStatus.Overpaid};
                    var info = new SalesInfo();

                    var range = DateHelper.GetDateRange(period);
                    var prevRange = DateHelper.GetDateRange(period, true);
                    var qOrders =
                        db.GetQuery(
                            o =>
                                o.StoreId == storeId && o.TimeOfOrder >= range.StartDate &&
                                o.TimeOfOrder <= range.EndDate).AsNoTracking();
                    var qOrdersPrev =
                        db.GetQuery(
                            o =>
                                o.StoreId == storeId && o.TimeOfOrder >= prevRange.StartDate &&
                                o.TimeOfOrder <= prevRange.EndDate).AsNoTracking();
                    var qPlacedOrders = qOrders.Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .Where(o => payedStatuses.Contains(o.PaymentStatus));
                    var qTrans = GetFilteredTransactions(db.GetQuery<hcc_OrderTransactions>(), range);

                    LoadChartDataByPayment(info, qTrans, period, range);

                    info.OrdersCount = qOrders.Count(i => i.hcc_LineItem.Any());
                    info.OrdersCompleted = qPlacedOrders.Count();

                    info.OrdersTotalSum = qTrans.Sum(t => (decimal?) t.Amount) ?? 0;
                    ;

                    info.OrdersSuccessfulTransactions = qTrans.Count();

                    // Sales by Device
                    var countByDeviceList = qPlacedOrders
                        .GroupBy(o => o.UserDeviceType)
                        .Select(g => new {UserDeviceType = g.Key, Count = g.Count()}).ToList();

                    var resDCount = countByDeviceList.FirstOrDefault(o => o.UserDeviceType == (int) DeviceType.Desktop);
                    var resTCount = countByDeviceList.FirstOrDefault(o => o.UserDeviceType == (int) DeviceType.Tablet);
                    var resPCount = countByDeviceList.FirstOrDefault(o => o.UserDeviceType == (int) DeviceType.Phone);

                    info.SalesByDesktopCount = resDCount != null ? resDCount.Count : 0;
                    info.SalesByTabletCount = resTCount != null ? resTCount.Count : 0;
                    info.SalesByPhoneCount = resPCount != null ? resPCount.Count : 0;

                    var prevSalesByDesktopCount = qOrdersPrev.Count(o => o.UserDeviceType == (int) DeviceType.Desktop);
                    var prevSalesByTabletCount = qOrdersPrev.Count(o => o.UserDeviceType == (int) DeviceType.Tablet);
                    var prevSalesByPhoneCount = qOrdersPrev.Count(o => o.UserDeviceType == (int) DeviceType.Phone);
                    decimal deviceTotal = info.SalesByDesktopCount + info.SalesByTabletCount + info.SalesByPhoneCount;
                    decimal prevDeviceTotal = prevSalesByDesktopCount + prevSalesByTabletCount + prevSalesByPhoneCount;

                    if (deviceTotal > 0)
                    {
                        info.SalesByDesktopPercent = 100*info.SalesByDesktopCount/deviceTotal;
                        info.SalesByTabletPercent = 100*info.SalesByTabletCount/deviceTotal;
                        info.SalesByPhonePercent = 100*info.SalesByPhoneCount/deviceTotal;
                    }
                    if (prevDeviceTotal > 0)
                    {
                        info.SalesByDesktopPercentPrev = 100*prevSalesByDesktopCount/prevDeviceTotal;
                        info.SalesByTabletPercentPrev = 100*prevSalesByTabletCount/prevDeviceTotal;
                        info.SalesByPhonePercentPrev = 100*prevSalesByPhoneCount/prevDeviceTotal;
                    }

                    return info;
                }
            }
        }

        /// <summary>
        ///     Gets the analytic information.
        /// </summary>
        /// <param name="period">The period.</param>
        /// <returns></returns>
        public AnalyticInfo GetAnalyticInfo(SalesPeriod period)
        {
            using (MiniProfiler.Current.Step("GetAnalyticInfo"))
            {
                var info = new AnalyticInfo();
                var storeId = Context.CurrentStore.Id;
                var payedStatuses = new[] {(int) OrderPaymentStatus.Paid, (int) OrderPaymentStatus.Overpaid};
                var range = DateHelper.GetDateRange(period);
                var prevRange = DateHelper.GetDateRange(period, true);

                using (var db = CreateReadOrderStrategy())
                {
                    var qOrders = db.GetQuery(o => o.StoreId == storeId
                                                   && payedStatuses.Contains(o.PaymentStatus)
                                                   && o.TimeOfOrder >= range.StartDate && o.TimeOfOrder <= range.EndDate).AsNoTracking();
                    var qOrdersPrev = db.GetQuery(o => o.StoreId == storeId
                                                       && payedStatuses.Contains(o.PaymentStatus)
                                                       && o.TimeOfOrder >= prevRange.StartDate &&
                                                       o.TimeOfOrder <= prevRange.EndDate).AsNoTracking();
                    var qPlacedOrders = qOrders.Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled);

                    // Sales by Device
                    var countByDeviceList = qPlacedOrders
                        .GroupBy(o => o.UserDeviceType)
                        .Select(g => new {UserDeviceType = g.Key, Count = g.Count()}).ToList();

                    var resDCount = countByDeviceList.FirstOrDefault(o => o.UserDeviceType == (int) DeviceType.Desktop);
                    var resTCount = countByDeviceList.FirstOrDefault(o => o.UserDeviceType == (int) DeviceType.Tablet);
                    var resPCount = countByDeviceList.FirstOrDefault(o => o.UserDeviceType == (int) DeviceType.Phone);

                    info.SalesByDesktopCount = resDCount != null ? resDCount.Count : 0;
                    info.SalesByTabletCount = resTCount != null ? resTCount.Count : 0;
                    info.SalesByPhoneCount = resPCount != null ? resPCount.Count : 0;

                    var prevSalesByDesktopCount = qOrdersPrev.Count(o => o.UserDeviceType == (int) DeviceType.Desktop);
                    var prevSalesByTabletCount = qOrdersPrev.Count(o => o.UserDeviceType == (int) DeviceType.Tablet);
                    var prevSalesByPhoneCount = qOrdersPrev.Count(o => o.UserDeviceType == (int) DeviceType.Phone);
                    decimal deviceTotal = info.SalesByDesktopCount + info.SalesByTabletCount + info.SalesByPhoneCount;
                    decimal prevDeviceTotal = prevSalesByDesktopCount + prevSalesByTabletCount + prevSalesByPhoneCount;

                    if (deviceTotal > 0)
                    {
                        info.SalesByDesktopPercent = 100*info.SalesByDesktopCount/deviceTotal;
                        info.SalesByTabletPercent = 100*info.SalesByTabletCount/deviceTotal;
                        info.SalesByPhonePercent = 100*info.SalesByPhoneCount/deviceTotal;
                    }
                    if (prevDeviceTotal > 0)
                    {
                        info.SalesByDesktopPercentPrev = 100*prevSalesByDesktopCount/prevDeviceTotal;
                        info.SalesByTabletPercentPrev = 100*prevSalesByTabletCount/prevDeviceTotal;
                        info.SalesByPhonePercentPrev = 100*prevSalesByPhoneCount/prevDeviceTotal;
                    }

                    // Revenue vs Units Sold
                    var qTrans = GetFilteredTransactions(db.GetQuery<hcc_OrderTransactions>().AsNoTracking(), range);
                    var totalSum = qTrans.Sum(t => (decimal?) t.Amount) ?? 0;
                    var totalCost = qPlacedOrders
                        .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, li => li.OrderBvin, (o, li) => li)
                        .Join(db.GetQuery<hcc_Product>(), li => li.ProductId, p => p.bvin, (li, p) => new {li, p})
                        .Sum(lp => (decimal?) (lp.p.SiteCost*lp.li.Quantity)) ?? 0;
                    info.Revenue = totalSum - totalCost;

                    info.UnitsSold = qPlacedOrders
                        .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, li => li.OrderBvin, (o, li) => li)
                        .Sum(li => (int?) li.Quantity) ?? 0;
                    info.AverageDealSize = qPlacedOrders.Average(o => (decimal?) o.GrandTotal) ?? 0;

                    // Promotions
                    var marketingServices = Factory.CreateService<MarketingService>(Context);
                    var resPromo = RepoStrategyFunctions.GetPromotionsActivityList(db, Context.CurrentStore.Id,
                        range.StartDate, range.EndDate);
                    var promoList =
                        marketingServices.Promotions.FindByIds(resPromo.Select(r => (long) r.PromotionId).ToList());

                    info.ActivePromotions = promoList
                        .Select(
                            p =>
                                new AnalyticInfo.PromoInfo
                                {
                                    Name = p.Name,
                                    Count = (int) resPromo.First(r => r.PromotionId == p.Id).OrderCount,
                                    Url = p.Id.ToString()
                                })
                        .OrderByDescending(p => p.Count)
                        .ToList();

                    // Gist Cards
                    var qGiftCards =
                        db.GetQuery<hcc_GiftCard>().AsNoTracking()
                            .Where(
                                g =>
                                    g.StoreId == storeId && g.IssueDateUtc >= range.StartDate &&
                                    g.IssueDateUtc <= range.EndDate);

                    info.GiftCardSold = qGiftCards.Count();
                    info.GiftCardSoldAmount = qGiftCards.Sum(g => (decimal?) g.Amount) ?? 0;
                    info.GiftCardUsed = qGiftCards.Count(g => g.UsedAmount > 0);
                    info.GiftCardUsedAmount = qGiftCards.Sum(g => (decimal?) g.UsedAmount) ?? 0;

                    // Rewards Points
                    var qPoints =
                        db.GetQuery<hcc_RewardsPoints>().AsNoTracking()
                            .Where(
                                p =>
                                    p.StoreId == storeId && p.TransactionTime >= range.StartDate &&
                                    p.TransactionTime <= range.EndDate);
                    info.RewardPointsAccrued = qPoints.Where(p => p.Points > 0).Sum(p => (int?) p.Points) ?? 0;
                    info.RewardPointsUsed = -qPoints.Where(p => p.Points < 0).Sum(p => (int?) p.Points) ?? 0;
                }

                var customerPointsManager = Factory.CreateService<CustomerPointsManager>(Context);
                info.RewardPointsAccruedAmount = customerPointsManager.DollarCreditForPoints(info.RewardPointsAccrued);
                info.RewardPointsUsedAmount = customerPointsManager.DollarCreditForPoints(info.RewardPointsUsed);

                return info;
            }
        }

        /// <summary>
        ///     Gets the top5 products.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public List<Top5Item> GetTop5Products(Top5ProductMode mode)
        {
            using (MiniProfiler.Current.Step("GetTop5Products"))
            {
                List<Top5Item> items = null;

                using (var db = CreateReadOrderStrategy())
                {
                    IQueryable<Top5Item> q = null;

                    switch (mode)
                    {
                        case Top5ProductMode.Amount:
                        case Top5ProductMode.Quantity:
                            q = db.GetQuery().AsNoTracking()
                                .Where(
                                    o =>
                                        o.StoreId == Context.CurrentStore.Id && o.IsPlaced == 1 &&
                                        o.StatusCode != OrderStatusCode.Cancelled)
                                .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, l => l.OrderBvin, (o, l) => new {o, l})
                                .GroupBy(ol => new {ol.l.ProductId, ol.l.ProductName})
                                .Select(g => new Top5Item
                                {
                                    Guid = g.Key.ProductId,
                                    Name = g.Key.ProductName,
                                    Amount = g.Sum(ol => ol.l.LineTotal),
                                    Count = g.Sum(ol => ol.l.Quantity)
                                });

                            break;
                        case Top5ProductMode.Rating:
                        case Top5ProductMode.Reviews:
                            var qReview = db.GetQuery<hcc_ProductReview>().AsNoTracking().Where(r => r.Approved == 1);

                            q = db.GetQuery().AsNoTracking()
                                .Where(
                                    o =>
                                        o.StoreId == Context.CurrentStore.Id && o.IsPlaced == 1 &&
                                        o.StatusCode != OrderStatusCode.Cancelled)
                                .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, l => l.OrderBvin,
                                    (o, l) => new {l.ProductId, l.ProductName})
                                .Distinct()
                                .GroupJoin(qReview, l => l.ProductId, r => r.ProductId, (l, reviews) => new {l, reviews})
                                .Select(lr => new Top5Item
                                {
                                    Guid = lr.l.ProductId,
                                    Name = lr.l.ProductName,
                                    Amount = lr.reviews.Average(r => (decimal) r.Rating),
                                    Count = lr.reviews.Count()
                                });

                            break;
                        default:
                            break;
                    }

                    if (mode == Top5ProductMode.Amount || mode == Top5ProductMode.Rating)
                    {
                        q = q.OrderByDescending(i => i.Amount);
                    }
                    else
                    {
                        q = q.OrderByDescending(i => i.Count);
                    }

                    items = q.Take(5).ToList();

                    if (mode == Top5ProductMode.Rating || mode == Top5ProductMode.Reviews)
                    {
                        items.ForEach(i => i.AmountFormat = "F");
                    }
                }

                return items;
            }
        }

        /// <summary>
        ///     Gets the top5 abandoned products.
        /// </summary>
        /// <returns></returns>
        public List<Top5Item> GetTop5AbandonedProducts()
        {
            using (MiniProfiler.Current.Step("GetTop5AbandonedProducts"))
            {
                List<Top5Item> items = null;

                using (var db = CreateReadOrderStrategy())
                {
                    var q = db.GetQuery().AsNoTracking().Where(o => o.StoreId == Context.CurrentStore.Id && o.IsPlaced != 1)
                        .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, li => li.OrderBvin, (o, li) => li)
                        .GroupBy(li => new {li.ProductId, li.ProductName})
                        .Select(g => new Top5Item
                        {
                            Guid = g.Key.ProductId,
                            Name = g.Key.ProductName,
                            Count = g.Count()
                        })
                        .OrderByDescending(i => i.Count);


                    items = q.Take(5).ToList();
                }

                return items;
            }
        }

        /// <summary>
        ///     Gets the top5 customers.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public List<Top5Item> GetTop5Customers(Top5CustomerMode mode)
        {
            using (MiniProfiler.Current.Step("GetTop5Customers"))
            {
                var storeId = Context.CurrentStore.Id;
                List<Top5Item> items = null;

                using (var db = CreateReadOrderStrategy())
                {
                    IQueryable<Top5Item> q = null;

                    switch (mode)
                    {
                        case Top5CustomerMode.Amount:
                        case Top5CustomerMode.Frequency:
                            q = db.GetQuery(
                                o =>
                                    o.StoreId == storeId && o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                                .AsNoTracking()
                                .GroupBy(o => new {o.UserId, o.UserEmail})
                                .Select(g => new Top5Item
                                {
                                    Id = g.Key.UserId,
                                    Name = g.Key.UserEmail,
                                    Amount = g.Sum(o => o.GrandTotal),
                                    Count = g.Count()
                                });
                            break;
                        case Top5CustomerMode.Activity:
                            q = db.GetQuery<hcc_AnalyticsEvent>(a => a.StoreId == storeId && a.UserId != "")
                                .AsNoTracking()
                                .GroupBy(o => o.UserId)
                                .Select(g => new Top5Item
                                {
                                    Id = g.Key,
                                    Count = g.Count()
                                });
                            break;
                        case Top5CustomerMode.Abandoned:
                            q = db.GetQuery()
                                .AsNoTracking()
                                .Where(
                                    o => o.StoreId == Context.CurrentStore.Id && o.IsPlaced == 0 && o.hcc_LineItem.Any())
                                .GroupBy(o => new {o.UserId, o.UserEmail})
                                .Select(g => new Top5Item
                                {
                                    Id = g.Key.UserId,
                                    Name = g.Key.UserEmail,
                                    Amount = g.Sum(o => o.GrandTotal),
                                    Count = g.Count()
                                });
                            break;
                        default:
                            break;
                    }

                    if (mode == Top5CustomerMode.Amount || mode == Top5CustomerMode.Abandoned)
                    {
                        q = q.OrderByDescending(i => i.Amount);
                    }
                    else
                    {
                        q = q.OrderByDescending(i => i.Count);
                    }

                    items = q.Take(5).ToList();
                }

                var membershipServices = Factory.CreateService<MembershipServices>(Context);
                var users = membershipServices.Customers.FindMany(items.Select(i => i.Id.Trim()).ToList());
                foreach (var item in items)
                {
                    var user = users.FirstOrDefault(u => u.Bvin == item.Id);
                    if (user != null)
                    {
                        item.Name = user.Username;
                    }
                    else if (string.IsNullOrEmpty(item.Name))
                    {
                        item.Name = string.IsNullOrEmpty(item.Id) ? "[Anonymous]" : "[USER DELETED]";
                    }
                }

                return items;
            }
        }

        /// <summary>
        ///     Gets the top5 search terms.
        /// </summary>
        /// <returns></returns>
        public List<Top5Item> GetTop5SearchTerms()
        {
            using (MiniProfiler.Current.Step("GetTop5SearchTerms"))
            {
                List<Top5Item> items = null;

                using (var db = CreateReadOrderStrategy())
                {
                    var q = db.GetQuery<hcc_SearchQuery>().Where(sq => sq.StoreId == Context.CurrentStore.Id)
                        .AsNoTracking()
                        .GroupBy(sq => sq.QueryPhrase)
                        .Select(g => new Top5Item
                        {
                            Name = g.Key,
                            Count = g.Count()
                        })
                        .OrderByDescending(i => i.Count);


                    items = q.Take(5).ToList();
                }

                return items;
            }
        }

        /// <summary>
        ///     Gets the top5 vendors manufacturers.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        [Obsolete("Deprecated in Hotcakes Commerce 03.03.00. Please use the GetTop5VendorsManufacturers method instead. Removing in version 03.04.00 or later.")]
        public List<Top5Item> GetTop5VendorsManufactures(Top5VendorType type)
        {
            return GetTop5VendorsManufacturers(type);
        }

        /// <summary>
        ///     Gets the top5 vendors manufacturers.
        /// </summary>
        /// <param name="type">The type.</param>
        /// <returns></returns>
        public List<Top5Item> GetTop5VendorsManufacturers(Top5VendorType type)
        {
            using (MiniProfiler.Current.Step("GetTop5VendorsManufacturers"))
            {
                List<Top5Item> items = null;

                using (var db = CreateReadOrderStrategy())
                {
                    IQueryable<Top5Item> q = null;

                    switch (type)
                    {
                        case Top5VendorType.Vendors:
                            q = db.GetQuery().AsNoTracking().Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                                .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, l => l.OrderBvin, (o, l) => l)
                                .Join(db.GetQuery<hcc_Product>(), li => li.ProductId, p => p.bvin,
                                    (li, p) => new {li, p})
                                .Join(db.GetQuery<hcc_Vendor>(), lp => lp.p.VendorID, v => v.bvin,
                                    (lp, v) => new {lp.li, lp.p, v})
                                .GroupBy(lpv => new {lpv.v.bvin, lpv.v.DisplayName})
                                .Select(g => new Top5Item
                                {
                                    Guid = g.Key.bvin,
                                    Name = g.Key.DisplayName,
                                    Amount = g.Sum(ol => ol.li.LineTotal),
                                    Count = g.Count()
                                });

                            break;
                        case Top5VendorType.Manufacturers:
                            q = db.GetQuery().AsNoTracking().Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                                .Join(db.GetQuery<hcc_LineItem>(), o => o.bvin, l => l.OrderBvin, (o, l) => l)
                                .Join(db.GetQuery<hcc_Product>(), li => li.ProductId, p => p.bvin,
                                    (li, p) => new {li, p})
                                .Join(db.GetQuery<hcc_Manufacturer>(), lp => lp.p.ManufacturerID, m => m.bvin,
                                    (lp, m) => new {lp.li, lp.p, m})
                                .GroupBy(lpv => new {lpv.m.bvin, lpv.m.DisplayName})
                                .Select(g => new Top5Item
                                {
                                    Guid = g.Key.bvin,
                                    Name = g.Key.DisplayName,
                                    Amount = g.Sum(ol => ol.li.LineTotal),
                                    Count = g.Count()
                                });
                            break;
                        default:
                            break;
                    }

                    q = q.OrderByDescending(i => i.Amount);
                    //q = q.OrderByDescending(i => i.Count);
                    items = q.Take(5).ToList();
                }

                return items;
            }
        }

        /// <summary>
        ///     Gets the top5 affiliates.
        /// </summary>
        /// <param name="mode">The mode.</param>
        /// <returns></returns>
        public List<Top5Item> GetTop5Affiliates(Top5AffiliateMode mode)
        {
            using (MiniProfiler.Current.Step("GetTop5Affiliates"))
            {
                var storeId = Context.CurrentStore.Id;
                List<AffiliateItem> items = null;

                using (var db = CreateReadOrderStrategy())
                {
                    IQueryable<AffiliateItem> q = null;

                    switch (mode)
                    {
                        case Top5AffiliateMode.Referral:
                            q = db.GetQuery<hcc_Affiliate>(a => a.StoreId == storeId)
                                .Join(db.GetQuery<hcc_Affiliate>(), a => a.ReferralID, ar => ar.AffiliateID,
                                    (a, ar) => new {a, ar})
                                .GroupBy(ar => new {ar.ar.Id, ar.ar.UserId})
                                .Select(g => new AffiliateItem
                                {
                                    Id = g.Key.Id,
                                    UserId = g.Key.UserId,
                                    Count = g.Count()
                                }).OrderByDescending(i => i.Count);

                            break;
                        case Top5AffiliateMode.Revenue:
                            q = db.GetQuery(o => o.StoreId == storeId)
                                .Join(db.GetQuery<hcc_Affiliate>(), o => o.AffiliateId, a => a.Id, (o, a) => new {o, a})
                                .GroupBy(oa => new {oa.a.Id, oa.a.UserId})
                                .Select(g => new AffiliateItem
                                {
                                    Id = g.Key.Id,
                                    UserId = g.Key.UserId,
                                    Amount = g.Sum(oa => oa.o.GrandTotal),
                                    Count = g.Count()
                                }).OrderByDescending(i => i.Amount);
                            break;
                        default:
                            break;
                    }

                    items = q.AsNoTracking().Take(5).ToList();

                    var membershipServices = Factory.CreateService<MembershipServices>(Context);
                    var users = membershipServices.Customers.FindMany(items.Select(i => i.UserId.ToString()).ToList());

                    foreach (var item in items)
                    {
                        var user = users.FirstOrDefault(u => u.Bvin == item.UserId.ToString());
                        if (user != null)
                        {
                            item.Name = user.Username;
                        }
                        else
                        {
                            item.Name = "[USER DELETED]";
                        }
                    }
                }

                return items.Select(i => new Top5Item
                {
                    Id = i.Id.ToString(),
                    Name = i.Name,
                    Amount = i.Amount,
                    Count = i.Count
                }).ToList();
            }
        }

        /// <summary>
        ///     Create agregated abandoned products report
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        public List<AbandonedProduct> GetAbandonedProducts(DateTime startDate, DateTime endDate, int pageNumber,
            int pageSize, out int totalCount)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;

            using (var db = CreateReadOrderStrategy())
            {
                var items = db.GetQuery<hcc_Order>()
                    .AsNoTracking()
                    .Where(o => o.StoreId == Context.CurrentStore.Id)
                    .Where(o => o.IsPlaced == 0)
                    .Where(o => o.hcc_LineItem.Count() > 0)
                    .Where(o => o.TimeOfOrder > startDate)
                    .Where(o => o.TimeOfOrder < endDate)
                    .SelectMany(o => o.hcc_LineItem)
                    .GroupBy(l => new {l.ProductId, l.ProductName})
                    .Select(g => new AbandonedProduct
                    {
                        ProductGuid = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        Quantity = g.Sum(l => l.Quantity),
                        CartsCount = g.Select(l => l.hcc_Order.Id).Distinct().Count(),
                        ContactsCount =
                            g.Select(l => l.hcc_Order.UserId).Distinct().Count(u => u != null && u != string.Empty)
                    });

                totalCount = items.Count();
                return items.
                    OrderByDescending(l => l.CartsCount).
                    Skip(skip).
                    Take(take).
                    ToList();
            }
        }

        /// <summary>
        ///     Create payment failure products report
        /// </summary>
        /// <param name="startDate">The start date.</param>
        /// <param name="endDate">The end date.</param>
        /// <param name="pageNumber">The page number.</param>
        /// <param name="pageSize">Size of the page.</param>
        /// <param name="totalCount">The total count.</param>
        /// <returns></returns>
        public List<AbandonedProduct> GetPaymentFailure(DateTime startDate, DateTime endDate, int pageNumber,
            int pageSize, out int totalCount)
        {
            if (pageNumber < 1) pageNumber = 1;
            var take = pageSize;
            var skip = (pageNumber - 1)*pageSize;

            using (var db = CreateReadOrderStrategy())
            {
                var items = db.GetQuery<hcc_Order>()
                    .AsNoTracking()
                    .Where(o => o.StoreId == Context.CurrentStore.Id)
                    .Where(o => o.hcc_LineItem.Count() > 0)
                    .Where(o => o.TimeOfOrder > startDate)
                    .Where(o => o.TimeOfOrder < endDate)
                    .Where(o => o.hcc_OrderTransactions.Any(t => !t.Success))
                    .Where(o => o.PaymentStatus == (int)OrderPaymentStatus.Unknown || o.PaymentStatus == (int)OrderPaymentStatus.Unpaid)
                    .SelectMany(o => o.hcc_LineItem)
                    .GroupBy(l => new {l.ProductId, l.ProductName})
                    .Select(g => new AbandonedProduct
                    {
                        ProductGuid = g.Key.ProductId,
                        ProductName = g.Key.ProductName,
                        Quantity = g.Sum(l => l.Quantity),
                        CartsCount = g.Select(l => l.hcc_Order.Id).Distinct().Count(),
                        ContactsCount =
                            g.Select(l => l.hcc_Order.UserId).Distinct().Count(u => u != null && u != string.Empty)
                    });

                totalCount = items.Count();
                return items.
                    OrderByDescending(l => l.CartsCount).
                    Skip(skip).
                    Take(take).
                    ToList();
            }
        }

        public PerformanceInfo GetProductPerformance(string productId, SalesPeriod period)
        {
            using (MiniProfiler.Current.Step("GetProductPerformance"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var productGuid = DataTypeHelper.BvinToGuid(productId);
                    var payedStatuses = new[] {(int) OrderPaymentStatus.Paid, (int) OrderPaymentStatus.Overpaid};
                    var info = new PerformanceInfo();

                    var productViewed = ActionTypes.ProductViewed.ToString();

                    var range = DateHelper.GetDateRange(period);
                    var qOrders = db.GetQuery(o => o.StoreId == storeId
                                                   && o.TimeOfOrder >= range.StartDate && o.TimeOfOrder <= range.EndDate).AsNoTracking();
                    var qPuchasedItems = qOrders
                        .Where(o => payedStatuses.Contains(o.PaymentStatus))
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem)
                        .Where(li => li.ProductId == productGuid);
                    var qAddeToCartItems = qOrders
                        .SelectMany(o => o.hcc_LineItem)
                        .Where(li => li.ProductId == productGuid);
                    var qAbandonedItems = qOrders
                        .Where(o => o.IsPlaced == 0 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem)
                        .Where(li => li.ProductId == productGuid);
                    var qAnalyticsEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= range.StartDate && ae.DateTime <= range.EndDate)
                        .Where(ae => ae.ObjectId == productGuid)
                        .Where(ae => ae.Action == ActionTypes.ProductImagesChanged.ToString()
                                     || ae.Action == ActionTypes.ProductCopyChanged.ToString()
                                     || ae.Action == ActionTypes.ProductPriceChanged.ToString());
                    var qViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= range.StartDate && ae.DateTime <= range.EndDate)
                        .Where(ae => ae.ObjectId == productGuid)
                        .Where(ae => ae.Action == productViewed);

                    info.Purchases = qPuchasedItems.Count();
                    info.AddsToCart = qAddeToCartItems.Count();
                    info.Views = qViewEvents.Count();

                    var previousRange = DateHelper.GetDateRange(period, true);
                    var qPrevOrders = db.GetQuery(o => o.StoreId == storeId
                                                       && o.TimeOfOrder >= previousRange.StartDate &&
                                                       o.TimeOfOrder <= previousRange.EndDate).AsNoTracking();
                    var qPrevPuchasedItems = qPrevOrders
                        .Where(o => payedStatuses.Contains(o.PaymentStatus))
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem)
                        .Where(li => li.ProductId == productGuid);
                    var qPrevAddeToCartItems = qPrevOrders
                        .SelectMany(o => o.hcc_LineItem)
                        .Where(li => li.ProductId == productGuid);
                    var qPrevViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= previousRange.StartDate &&
                                ae.DateTime <= previousRange.EndDate)
                        .Where(ae => ae.Action == productViewed)
                        .Where(ae => ae.ObjectId == productGuid);

                    info.PurchasesPrev = qPrevPuchasedItems.Count();
                    info.AddsToCartPrev = qPrevAddeToCartItems.Count();
                    info.ViewsPrev = qPrevViewEvents.Count();

                    PopulatePerformanceChartData(db, info, period, range, qPuchasedItems, qAbandonedItems,
                        qAnalyticsEvents, productGuid, null);

                    return info;
                }
            }
        }

        public PerformanceInfo GetCategoryPerformance(string categoryId, SalesPeriod period)
        {
            using (MiniProfiler.Current.Step("GetCategoryPerformance"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var categoryGuid = DataTypeHelper.BvinToGuid(categoryId);
                    var payedStatuses = new[] {(int) OrderPaymentStatus.Paid, (int) OrderPaymentStatus.Overpaid};
                    var info = new PerformanceInfo();

                    var productViewed = ActionTypes.ProductViewed.ToString();

                    var category = db.GetQuery<hcc_Category>().AsNoTracking().FirstOrDefault(c => c.bvin == categoryGuid);
                    var categoryProducts = db.GetQuery<hcc_ProductXCategory>().AsNoTracking()
                        .Where(pc => pc.CategoryId == categoryGuid);

                    var range = DateHelper.GetDateRange(period);
                    var qOrders = db.GetQuery(o => o.StoreId == storeId
                                                   && o.TimeOfOrder >= category.CreationDate
                                                   && o.TimeOfOrder >= range.StartDate && o.TimeOfOrder <= range.EndDate).AsNoTracking();
                    var qPuchasedItems = qOrders
                        .Where(o => payedStatuses.Contains(o.PaymentStatus))
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem)
                        .Join(categoryProducts, li => li.ProductId, cp => cp.ProductId, (li, cp) => li);
                    var qAbandonedItems = qOrders
                        .Where(o => o.IsPlaced == 0 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem)
                        .Join(categoryProducts, li => li.ProductId, cp => cp.ProductId, (li, cp) => li);
                    var qAddeToCartItems = qOrders
                        .SelectMany(o => o.hcc_LineItem)
                        .Join(categoryProducts, li => li.ProductId, cp => cp.ProductId, (li, cp) => li);
                    var qAnalyticsEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= range.StartDate && ae.DateTime <= range.EndDate)
                        .Where(ae => ae.ObjectId == categoryGuid)
                        .Where(ae => ae.Action == ActionTypes.CategoryImagesChanged.ToString()
                                     || ae.Action == ActionTypes.CategoryCopyChanged.ToString()
                                     || ae.Action == ActionTypes.CategoryProductsUpdated.ToString());
                    var qViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= range.StartDate && ae.DateTime <= range.EndDate &&
                                ae.DateTime >= category.CreationDate)
                        .Join(categoryProducts, ae => ae.ObjectId, cp => cp.ProductId, (ae, cp) => ae)
                        .Where(ae => ae.Action == productViewed);

                    info.Purchases = qPuchasedItems.Count();
                    info.AddsToCart = qAddeToCartItems.Count();
                    info.Views = qViewEvents.Count();

                    var previousRange = DateHelper.GetDateRange(period, true);
                    var qPrevOrders = db.GetQuery(o => o.StoreId == storeId
                                                       && o.TimeOfOrder >= category.CreationDate
                                                       && o.TimeOfOrder >= previousRange.StartDate &&
                                                       o.TimeOfOrder <= previousRange.EndDate);
                    var qPrevPuchasedItems = qPrevOrders
                        .Where(o => payedStatuses.Contains(o.PaymentStatus))
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem)
                        .Join(categoryProducts, li => li.ProductId, cp => cp.ProductId, (li, cp) => li);
                    var qPrevAddeToCartItems = qPrevOrders
                        .SelectMany(o => o.hcc_LineItem)
                        .Join(categoryProducts, li => li.ProductId, cp => cp.ProductId, (li, cp) => li);
                    var qPrevViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= previousRange.StartDate &&
                                ae.DateTime <= previousRange.EndDate && ae.DateTime >= category.CreationDate)
                        .Where(ae => ae.Action == productViewed)
                        .Join(categoryProducts, ae => ae.ObjectId, cp => cp.ProductId, (ae, cp) => ae);

                    info.PurchasesPrev = qPrevPuchasedItems.Count();
                    info.AddsToCartPrev = qPrevAddeToCartItems.Count();
                    info.ViewsPrev = qPrevViewEvents.Count();

                    PopulatePerformanceChartData(db, info, period, range, qPuchasedItems, qAbandonedItems,
                        qAnalyticsEvents, null, categoryGuid);

                    return info;
                }
            }
        }

        public PerformanceInfo GetProductPerformance(SalesPeriod period)
        {
            using (MiniProfiler.Current.Step("GetProductPerformance"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var payedStatuses = new[] {(int) OrderPaymentStatus.Paid, (int) OrderPaymentStatus.Overpaid};
                    var info = new PerformanceInfo();

                    var productViewed = ActionTypes.ProductViewed.ToString();

                    var range = DateHelper.GetDateRange(period);
                    var qOrders = db.GetQuery(o => o.StoreId == storeId
                                                   && o.TimeOfOrder >= range.StartDate && o.TimeOfOrder <= range.EndDate).AsNoTracking();
                    var qPuchasedItems = qOrders
                        .Where(o => payedStatuses.Contains(o.PaymentStatus))
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem);
                    var qAbandonedItems = qOrders
                        .Where(o => o.IsPlaced == 0 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem);
                    var qAddeToCartItems = qOrders
                        .SelectMany(o => o.hcc_LineItem);
                    var qViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= range.StartDate && ae.DateTime <= range.EndDate)
                        .Where(ae => ae.Action == productViewed);

                    info.Purchases = qPuchasedItems.Count();
                    info.AddsToCart = qAddeToCartItems.Count();
                    info.Views = qViewEvents.Count();

                    var previousRange = DateHelper.GetDateRange(period, true);
                    var qPrevOrders = db.GetQuery(o => o.StoreId == storeId
                                                       && o.TimeOfOrder >= previousRange.StartDate &&
                                                       o.TimeOfOrder <= previousRange.EndDate).AsNoTracking();
                    var qPrevPuchasedItems = qPrevOrders
                        .Where(o => payedStatuses.Contains(o.PaymentStatus))
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem);
                    var qPrevAddeToCartItems = qPrevOrders
                        .SelectMany(o => o.hcc_LineItem);
                    var qPrevViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(
                            ae =>
                                ae.StoreId == storeId && ae.DateTime >= previousRange.StartDate &&
                                ae.DateTime <= previousRange.EndDate)
                        .Where(ae => ae.Action == productViewed);

                    info.PurchasesPrev = qPrevPuchasedItems.Count();
                    info.AddsToCartPrev = qPrevAddeToCartItems.Count();
                    info.ViewsPrev = qPrevViewEvents.Count();

                    var qAnalyticsEvents = Enumerable.Empty<hcc_AnalyticsEvent>().AsQueryable();
                    PopulatePerformanceChartData(db, info, period, range, qPuchasedItems, qAbandonedItems,
                        qAnalyticsEvents);

                    return info;
                }
            }
        }

        public TopChangeInfo GetTopChangeByBounces(SalesPeriod period, SortDirection sortDirection, int pageNumber,
            int pageSize)
        {
            using (MiniProfiler.Current.Step("GetTopChangeByBounces"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var range = DateHelper.GetDateRange(period);
                    var previousRange = DateHelper.GetDateRange(period, true);

                    var productViewed = ActionTypes.ProductViewed.ToString();

                    var qAddedToCartEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(ae => ae.StoreId == storeId)
                        .Where(ae => ae.Action == ActionTypes.ProductAddedToCart.ToString());
                    var qViewEvents = db.GetQuery<hcc_AnalyticsEvent>()
                        .AsNoTracking()
                        .Where(ae => ae.StoreId == storeId)
                        .Where(ae => ae.Action == productViewed)
                        .Where(ae => qAddedToCartEvents.
                            Count(
                                iae => iae.ObjectId == ae.ObjectId && iae.ShoppingSessionGuid == ae.ShoppingSessionGuid) ==
                                     0);

                    var qProductTranslation = db.GetQuery<hcc_ProductTranslation>()
                        .AsNoTracking()
                        .Where(
                            it =>
                                it.Culture == Context.MainContentCulture || it.Culture == Context.FallbackContentCulture);

                    var qProducts = db.GetQuery<hcc_Product>()
                        .AsNoTracking()
                        .Where(p => p.StoreId == storeId)
                        .GroupJoin(qProductTranslation, p => p.bvin, pt => pt.ProductId, (p, pt)
                            =>
                            new
                            {
                                p,
                                pt =
                                    pt.OrderBy(ptr => ptr.Culture == Context.MainContentCulture ? 1 : 2)
                                        .FirstOrDefault()
                            })
                        .GroupJoin(qViewEvents, jp => jp.p.bvin, ae => ae.ObjectId,
                            (jp, ae) =>
                                new {ProductId = jp.p.bvin, jp.pt.ProductName, AnalyticsEvents = ae.DefaultIfEmpty()})
                        .SelectMany(
                            g => g.AnalyticsEvents.Select(jp => new {g.ProductId, g.ProductName, AnalyticsEvent = jp}))
                        .GroupBy(li => new {li.ProductId, li.ProductName})
                        .Select(g => new
                        {
                            g.Key.ProductId,
                            g.Key.ProductName,
                            Bounces = g.Where(j => j.AnalyticsEvent.DateTime >= range.StartDate
                                                   && j.AnalyticsEvent.DateTime <= range.EndDate)
                                .Select(j => new {j.AnalyticsEvent.ShoppingSessionGuid, j.AnalyticsEvent.ObjectId})
                                .Distinct()
                                .Count(),
                            PrevBounces = g.Where(j => j.AnalyticsEvent.DateTime >= previousRange.StartDate
                                                       && j.AnalyticsEvent.DateTime <= previousRange.EndDate)
                                .Select(j => new {j.AnalyticsEvent.ShoppingSessionGuid, j.AnalyticsEvent.ObjectId})
                                .Distinct()
                                .Count()
                        })
                        .Select(d => new TopChangeItemInfo
                        {
                            ProductId = d.ProductId,
                            ProductName = d.ProductName,
                            Change =
                                d.PrevBounces != 0
                                    ? (d.Bounces - d.PrevBounces)/(decimal) d.PrevBounces
                                    : (d.Bounces != 0 ? 1 : 0)
                        });

                    if (sortDirection == SortDirection.Ascending)
                        qProducts = qProducts.OrderBy(d => Math.Abs(d.Change));
                    else
                        qProducts = qProducts.OrderByDescending(d => Math.Abs(d.Change));

                    var info = new TopChangeInfo();

                    info.TotalCount = qProducts.Count();
                    info.Items = qProducts
                        .Skip((pageNumber - 1)*pageSize)
                        .Take(pageSize)
                        .ToList();

                    return info;
                }
            }
        }

        public TopChangeInfo GetTopChangeByAbandoments(SalesPeriod period, SortDirection sortDirection, int pageNumber,
            int pageSize)
        {
            using (MiniProfiler.Current.Step("TopChangeByAbandoments"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var range = DateHelper.GetDateRange(period);
                    var previousRange = DateHelper.GetDateRange(period, true);

                    var qLineItems = db.GetQuery<hcc_LineItem>().AsNoTracking();

                    var qProductTranslation = db.GetQuery<hcc_ProductTranslation>()
                        .AsNoTracking()
                        .Where(
                            it =>
                                it.Culture == Context.MainContentCulture || it.Culture == Context.FallbackContentCulture);

                    var qProducts = db.GetQuery<hcc_Product>()
                        .AsNoTracking()
                        .Where(p => p.StoreId == storeId)
                        .GroupJoin(qProductTranslation, p => p.bvin, pt => pt.ProductId, (p, pt)
                            =>
                            new
                            {
                                p,
                                pt =
                                    pt.OrderBy(ptr => ptr.Culture == Context.MainContentCulture ? 1 : 2)
                                        .FirstOrDefault()
                            })
                        .GroupJoin(qLineItems, jp => jp.p.bvin, li => li.ProductId,
                            (jp, li) => new {ProductId = jp.p.bvin, jp.pt.ProductName, LineItems = li.DefaultIfEmpty()})
                        .SelectMany(g => g.LineItems.Select(jp => new {g.ProductId, g.ProductName, LineItem = jp}))
                        .GroupBy(li => new {li.ProductId, li.ProductName})
                        .Select(g => new
                        {
                            g.Key.ProductId,
                            g.Key.ProductName,
                            Abandoments = g.Where(j => j.LineItem.hcc_Order.IsPlaced == 0
                                                       && j.LineItem.hcc_Order.StatusCode != OrderStatusCode.Cancelled
                                                       && j.LineItem.hcc_Order.TimeOfOrder >= range.StartDate
                                                       && j.LineItem.hcc_Order.TimeOfOrder <= range.EndDate)
                                .Sum(j => (int?) j.LineItem.Quantity) ?? 0,
                            PrevAbandoments = g.Where(j => j.LineItem.hcc_Order.IsPlaced == 0
                                                           &&
                                                           j.LineItem.hcc_Order.StatusCode != OrderStatusCode.Cancelled
                                                           &&
                                                           j.LineItem.hcc_Order.TimeOfOrder >= previousRange.StartDate
                                                           && j.LineItem.hcc_Order.TimeOfOrder <= previousRange.EndDate)
                                .Sum(j => (int?) j.LineItem.Quantity) ?? 0
                        })
                        .Select(d => new TopChangeItemInfo
                        {
                            ProductId = d.ProductId,
                            ProductName = d.ProductName,
                            Change =
                                d.PrevAbandoments != 0
                                    ? (d.Abandoments - d.PrevAbandoments)/(decimal) d.PrevAbandoments
                                    : (d.Abandoments != 0 ? 1 : 0)
                        });

                    if (sortDirection == SortDirection.Ascending)
                        qProducts = qProducts.OrderBy(d => Math.Abs(d.Change));
                    else
                        qProducts = qProducts.OrderByDescending(d => Math.Abs(d.Change));

                    var info = new TopChangeInfo();

                    info.TotalCount = qProducts.Count();
                    info.Items = qProducts
                        .Skip((pageNumber - 1)*pageSize)
                        .Take(pageSize)
                        .ToList();

                    return info;
                }
            }
        }

        public TopChangeInfo GetTopChangeByPurchases(SalesPeriod period, SortDirection sortDirection, int pageNumber,
            int pageSize)
        {
            using (MiniProfiler.Current.Step("GetTopChangeByPurchases"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var storeId = Context.CurrentStore.Id;
                    var payedStatuses = new[] {(int) OrderPaymentStatus.Paid, (int) OrderPaymentStatus.Overpaid};
                    var range = DateHelper.GetDateRange(period);
                    var previousRange = DateHelper.GetDateRange(period, true);

                    var qLineItems = db.GetQuery<hcc_LineItem>().AsNoTracking();

                    var qProductTranslation = db.GetQuery<hcc_ProductTranslation>()
                        .AsNoTracking()
                        .Where(
                            it =>
                                it.Culture == Context.MainContentCulture || it.Culture == Context.FallbackContentCulture);

                    var qProducts = db.GetQuery<hcc_Product>()
                        .AsNoTracking()
                        .Where(p => p.StoreId == storeId)
                        .GroupJoin(qProductTranslation, p => p.bvin, pt => pt.ProductId, (p, pt)
                            =>
                            new
                            {
                                p,
                                pt =
                                    pt.OrderBy(ptr => ptr.Culture == Context.MainContentCulture ? 1 : 2)
                                        .FirstOrDefault()
                            })
                        .GroupJoin(qLineItems, jp => jp.p.bvin, li => li.ProductId,
                            (jp, li) => new {ProductId = jp.p.bvin, jp.pt.ProductName, LineItems = li.DefaultIfEmpty()})
                        .SelectMany(g => g.LineItems.Select(jp => new {g.ProductId, g.ProductName, LineItem = jp}))
                        .GroupBy(li => new {li.ProductId, li.ProductName})
                        .Select(g => new
                        {
                            g.Key.ProductId,
                            g.Key.ProductName,
                            Purchases = g.Where(j => j.LineItem.hcc_Order.IsPlaced == 1
                                                     && j.LineItem.hcc_Order.StatusCode != OrderStatusCode.Cancelled
                                                     && j.LineItem.hcc_Order.TimeOfOrder >= range.StartDate
                                                     && j.LineItem.hcc_Order.TimeOfOrder <= range.EndDate
                                                     && payedStatuses.Contains(j.LineItem.hcc_Order.PaymentStatus))
                                .Sum(j => (int?) j.LineItem.Quantity) ?? 0,
                            PrevPurchases = g.Where(j => j.LineItem.hcc_Order.IsPlaced == 1
                                                         && j.LineItem.hcc_Order.StatusCode != OrderStatusCode.Cancelled
                                                         && j.LineItem.hcc_Order.TimeOfOrder >= previousRange.StartDate
                                                         && j.LineItem.hcc_Order.TimeOfOrder <= previousRange.EndDate
                                                         && payedStatuses.Contains(j.LineItem.hcc_Order.PaymentStatus))
                                .Sum(j => (int?) j.LineItem.Quantity) ?? 0
                        })
                        .Select(d => new TopChangeItemInfo
                        {
                            ProductId = d.ProductId,
                            ProductName = d.ProductName,
                            Change =
                                d.PrevPurchases != 0
                                    ? (d.Purchases - d.PrevPurchases)/(decimal) d.PrevPurchases
                                    : (d.Purchases != 0 ? 1 : 0)
                        });

                    if (sortDirection == SortDirection.Ascending)
                        qProducts = qProducts.OrderBy(d => Math.Abs(d.Change));
                    else
                        qProducts = qProducts.OrderByDescending(d => Math.Abs(d.Change));

                    var info = new TopChangeInfo();

                    info.TotalCount = qProducts.Count();
                    info.Items = qProducts
                        .Skip((pageNumber - 1)*pageSize)
                        .Take(pageSize)
                        .ToList();

                    return info;
                }
            }
        }

        public TopChangeInfo GetTopAffectedProducts(SalesPeriod period, TopAffectedSort sortBy,
            SortDirection sortDirection, int pageNumber, int pageSize)
        {
            using (MiniProfiler.Current.Step("GetTopAffectedProducts"))
            {
                using (var db = Factory.CreateReadOnlyHccDbContext())
                {
                    var storeId = Context.CurrentStore.Id;
                    var range = DateHelper.GetDateRange(period);
                    var previousRange = DateHelper.GetDateRange(period, true);

                    var opTotalCount = new ObjectParameter("TotalCount", typeof (int));

                    var result = db.GetTopAffectedProducts(storeId,
                        Context.MainContentCulture,
                        Context.FallbackContentCulture,
                        range.StartDate,
                        range.EndDate,
                        previousRange.StartDate,
                        previousRange.EndDate,
                        (int) sortBy,
                        (int) sortDirection,
                        pageNumber,
                        pageSize,
                        opTotalCount);

                    var info = new TopChangeInfo();
                    foreach (var item in result)
                    {
                        info.Items.Add(new TopChangeItemInfo
                        {
                            ProductId = item.ProductId,
                            ProductName = item.ProductName,
                            BouncesChange = item.BouncesChange.Value,
                            AbandomentsChange = item.AbandomentsChange.Value,
                            PurchasesChange = item.PurchasesChange.Value,
                            Change = item.Change.Value
                        });
                    }
                    info.TotalCount = (int) opTotalCount.Value;

                    return info;
                }
            }
        }

        public PurchasedWithProductPerformanceInfo GetPurchasedWithProductPerformance(string productId,
            SalesPeriod period)
        {
            using (MiniProfiler.Current.Step("GetProductPerformance"))
            {
                using (var db = CreateReadOrderStrategy())
                {
                    var productGuid = DataTypeHelper.BvinToGuid(productId);
                    var storeId = Context.CurrentStore.Id;

                    var info = new PurchasedWithProductPerformanceInfo();

                    var range = DateHelper.GetDateRange(period);
                    var qOrders = db.GetQuery(o => o.StoreId == storeId
                                                   && o.TimeOfOrder >= range.StartDate && o.TimeOfOrder <= range.EndDate).AsNoTracking();
                    var qPuchasedItems = qOrders
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem).Where(li => li.ProductId == productGuid);

                    info.QuantitySold = qPuchasedItems.Sum(li => (int?) li.Quantity) ?? 0;
                    info.Revenue = qPuchasedItems.Sum(li => (decimal?) li.LineTotal) ?? 0;

                    var previousRange = DateHelper.GetDateRange(period, true);
                    var qPrevOrders = db.GetQuery(o => o.StoreId == storeId
                                                       && o.TimeOfOrder >= previousRange.StartDate &&
                                                       o.TimeOfOrder <= previousRange.EndDate).AsNoTracking();
                    var qPrevPuchasedItems = qPrevOrders
                        .Where(o => o.IsPlaced == 1 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem).Where(li => li.ProductId == productGuid);
                    var qPrevAbandonedItems = qOrders
                        .Where(o => o.IsPlaced == 0 && o.StatusCode != OrderStatusCode.Cancelled)
                        .SelectMany(o => o.hcc_LineItem).Where(li => li.ProductId == productGuid);

                    info.QuantitySoldPrev = qPrevPuchasedItems.Sum(li => (int?) li.Quantity) ?? 0;
                    info.RevenuePrev = qPrevAbandonedItems.Sum(li => (decimal?) li.LineTotal) ?? 0;

                    return info;
                }
            }
        }

        #endregion

        #region Implementation

        private void LoadChartDataByPayment(SalesInfo info, IQueryable<hcc_OrderTransactions> qTrans, SalesPeriod period,
            DateRange range)
        {
            var ci = CultureInfo.CurrentUICulture;

            info.ChartLabels = new List<string>();
            info.ChartData = new List<decimal>();

            switch (period)
            {
                case SalesPeriod.Year:
                    var mDataList = qTrans.OrderBy(o => o.Timestamp)
                        .Select(o => new {Tran = o, o.Timestamp.Month})
                        .GroupBy(o => o.Month)
                        .Select(g => new {Month = g.Key, Amount = g.Sum(o => o.Tran.Amount)})
                        .ToList();

                    for (var i = 1; i <= 12; i++)
                    {
                        var m = i + range.StartDate.Month;
                        if (m > 12) m = m - 12;

                        info.ChartLabels.Add(ci.DateTimeFormat.GetAbbreviatedMonthName(m).ToUpper());
                        var mData = mDataList.FirstOrDefault(k => k.Month == m);
                        info.ChartData.Add(mData != null ? Math.Round(mData.Amount) : 0);
                    }

                    break;
                case SalesPeriod.Quarter:
                    var qDataList = qTrans.OrderBy(o => o.Timestamp)
                        .Select(o => new {Tran = o, o.Timestamp.Month})
                        .GroupBy(o => o.Month)
                        .Select(g => new {Month = g.Key, Amount = g.Sum(o => o.Tran.Amount)})
                        .ToList();

                    for (var i = 1; i <= 3; i++)
                    {
                        var m = i + range.StartDate.Month;
                        if (m > 12) m = m - 12;
                        info.ChartLabels.Add(ci.DateTimeFormat.GetAbbreviatedMonthName(m).ToUpper());
                        var qData = qDataList.FirstOrDefault(q => q.Month == m);
                        info.ChartData.Add(qData != null ? Math.Round(qData.Amount) : 0);
                    }

                    break;
                case SalesPeriod.Month:
                    var date = range.StartDate;
                    var dDataList = qTrans.OrderBy(o => o.Timestamp)
                        .Select(o => new {Tran = o, o.Timestamp.Day})
                        .GroupBy(o => o.Day)
                        .Select(g => new {Day = g.Key, Amount = g.Sum(o => o.Tran.Amount)})
                        .ToList();

                    for (var i = 1; i <= 30; i++)
                    {
                        date = date.AddDays(1);

                        info.ChartLabels.Add(date.Day.ToString());
                        var dData = dDataList.FirstOrDefault(d => d.Day == date.Day);
                        info.ChartData.Add(dData != null ? Math.Round(dData.Amount) : 0);
                    }
                    break;
                case SalesPeriod.Week:
                    var wDataList = qTrans.OrderBy(o => o.Timestamp)
                        .Select(o => new {Tran = o, Day = SqlFunctions.DatePart("dw", o.Timestamp) - 1})
                        .GroupBy(o => o.Day)
                        .Select(g => new {Day = g.Key, Amount = g.Sum(o => o.Tran.Amount)})
                        .ToList();

                    for (var i = 1; i <= 7; i++)
                    {
                        var d = i + (int) range.StartDate.DayOfWeek;
                        if (d > 6) d = d - 7;
                        info.ChartLabels.Add(ci.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek) d).ToUpper());
                        var wData = wDataList.FirstOrDefault(w => w.Day == d);
                        info.ChartData.Add(wData != null ? Math.Round(wData.Amount) : 0);
                    }

                    break;
                default:
                    break;
            }
        }

        private void LoadChartDataByOrder(SalesInfo info, IQueryable<hcc_Order> qOrders, SalesPeriod period,
            DateRange range)
        {
            var ci = CultureInfo.CurrentUICulture;

            info.ChartLabels = new List<string>();
            info.ChartData = new List<decimal>();

            switch (period)
            {
                case SalesPeriod.Year:
                    var mDataList = qOrders.OrderBy(o => o.TimeOfOrder)
                        .Select(o => new {Order = o, o.TimeOfOrder.Month})
                        .GroupBy(o => o.Month)
                        .Select(g => new {Month = g.Key, Amount = g.Sum(o => o.Order.GrandTotal)})
                        .ToList();

                    for (var i = 1; i <= 12; i++)
                    {
                        var m = i + range.StartDate.Month;
                        if (m > 12) m = m - 12;

                        info.ChartLabels.Add(ci.DateTimeFormat.GetAbbreviatedMonthName(m).ToUpper());
                        var mData = mDataList.FirstOrDefault(k => k.Month == m);
                        info.ChartData.Add(mData != null ? Math.Round(mData.Amount) : 0);
                    }

                    break;
                case SalesPeriod.Quarter:
                    var qDataList = qOrders.OrderBy(o => o.TimeOfOrder)
                        .Select(o => new {Order = o, o.TimeOfOrder.Month})
                        .GroupBy(o => o.Month)
                        .Select(g => new {Month = g.Key, Amount = g.Sum(o => o.Order.GrandTotal)})
                        .ToList();

                    for (var i = 1; i <= 3; i++)
                    {
                        var m = i + range.StartDate.Month;
                        if (m > 12) m = m - 12;
                        info.ChartLabels.Add(ci.DateTimeFormat.GetAbbreviatedMonthName(m).ToUpper());
                        var qData = qDataList.FirstOrDefault(q => q.Month == m);
                        info.ChartData.Add(qData != null ? Math.Round(qData.Amount) : 0);
                    }

                    break;
                case SalesPeriod.Month:
                    var date = range.StartDate;
                    var dDataList = qOrders.OrderBy(o => o.TimeOfOrder)
                        .Select(o => new {Order = o, o.TimeOfOrder.Day})
                        .GroupBy(o => o.Day)
                        .Select(g => new {Day = g.Key, Amount = g.Sum(o => o.Order.GrandTotal)})
                        .ToList();

                    for (var i = 1; i <= 30; i++)
                    {
                        info.ChartLabels.Add(date.Day.ToString());
                        var dData = dDataList.FirstOrDefault(d => d.Day == date.Day);
                        info.ChartData.Add(dData != null ? Math.Round(dData.Amount) : 0);

                        date = date.AddDays(1);
                    }
                    break;
                case SalesPeriod.Week:
                    var wDataList = qOrders.OrderBy(o => o.TimeOfOrder)
                        .Select(o => new {Order = o, Day = SqlFunctions.DatePart("dw", o.TimeOfOrder)})
                        .GroupBy(o => o.Day)
                        .Select(g => new {Day = g.Key, Amount = g.Sum(o => o.Order.GrandTotal)})
                        .ToList();

                    for (var i = 1; i <= 7; i++)
                    {
                        var d = i + (int) range.StartDate.DayOfWeek;
                        if (d > 6) d = d - 7;
                        info.ChartLabels.Add(ci.DateTimeFormat.GetAbbreviatedDayName((DayOfWeek) d).ToUpper());
                        var wData = wDataList.FirstOrDefault(w => w.Day == d);
                        info.ChartData.Add(wData != null ? Math.Round(wData.Amount) : 0);
                    }

                    break;
                default:
                    break;
            }
        }

        private IQueryable<hcc_OrderTransactions> GetFilteredTransactions(IQueryable<hcc_OrderTransactions> query,
            DateRange range)
        {
            var rewardCodes = new[]
            {ActionType.RewardPointsCapture, ActionType.RewardPointsIncrease, ActionType.RewardPointsDecrease};
            var giftcardCodes = new[]
            {ActionType.GiftCardCapture, ActionType.GiftCardDecrease, ActionType.GiftCardIncrease};
            var actionCodes = ActionTypeUtils.BalanceChangingActions
                .Where(a => !rewardCodes.Contains(a))
                .Where(a => !giftcardCodes.Contains(a))
                .Select(a => (int) a).ToList();

            return query.Where(y => y.StoreId == Context.CurrentStore.Id)
                .Where(y => y.Timestamp >= range.StartDate && y.Timestamp <= range.EndDate)
                .Where(y => y.Success && !y.Voided && actionCodes.Contains(y.Action))
                .Where(y => y.hcc_Order.StatusCode != OrderStatusCode.Cancelled);
        }

        protected IRepoStrategy<hcc_Order> CreateReadOrderStrategy()
        {
            return Factory.Instance.CreateReadStrategy<hcc_Order>();
        }

        private PerformanceInfo PopulatePerformanceChartData(IRepoStrategy<hcc_Order> db, PerformanceInfo info,
            SalesPeriod period, DateRange range, IQueryable<hcc_LineItem> qPuchasedItems,
            IQueryable<hcc_LineItem> qAbandonedItems, IQueryable<hcc_AnalyticsEvent> qAnalyticsEvents,
            Guid? productGuid = null, Guid? categoryGuid = null)
        {
            var storeId = Context.CurrentStore.Id;
            var ci = CultureInfo.CurrentUICulture;

            switch (period)
            {
                case SalesPeriod.Year:
                case SalesPeriod.Month:
                case SalesPeriod.Quarter:
                {
                    var purchasedList = qPuchasedItems
                        .Select(
                            li =>
                                new
                                {
                                    LineItem = li,
                                    li.hcc_Order.TimeOfOrder.Month,
                                    li.hcc_Order.TimeOfOrder.Day,
                                    li.hcc_Order.TimeOfOrder.Year
                                })
                        .GroupBy(o => new {o.Month, o.Day, o.Year})
                        .Select(
                            g => new {g.Key.Month, g.Key.Day, Amount = g.Sum(gi => gi.LineItem.Quantity), g.Key.Year})
                        .ToList();
                    var abandonedList = qAbandonedItems
                        .Select(
                            li =>
                                new
                                {
                                    LineItem = li,
                                    li.hcc_Order.TimeOfOrder.Month,
                                    li.hcc_Order.TimeOfOrder.Day,
                                    li.hcc_Order.TimeOfOrder.Year
                                })
                        .GroupBy(o => new {o.Month, o.Day, o.Year})
                        .Select(
                            g => new {g.Key.Month, g.Key.Day, Amount = g.Sum(gi => gi.LineItem.Quantity), g.Key.Year})
                        .ToList();
                    var bouncedList = RepoStrategyFunctions.GetBouncedListByDay(db,
                        storeId,
                        range.StartDate,
                        range.EndDate,
                        productGuid,
                        categoryGuid);

                    var eventsList = qAnalyticsEvents
                        .Select(ae => new {Event = ae, ae.DateTime.Month, ae.DateTime.Day, ae.DateTime.Year})
                        .GroupBy(o => new {o.Month, o.Day, o.Year})
                        .Select(g => new {g.Key.Month, g.Key.Year, g.Key.Day, Events = g.Select(e => e.Event).ToList()})
                        .ToList();

                    var i = 0;
                    var date = range.StartDate;
                    while (date <= range.EndDate)
                    {
                        var purchasedData =
                            purchasedList.FirstOrDefault(
                                k => k.Month == date.Month && k.Day == date.Day && k.Year == date.Year);
                        var abandonedData =
                            abandonedList.FirstOrDefault(
                                k => k.Month == date.Month && k.Day == date.Day && k.Year == date.Year);
                        var bouncedData =
                            bouncedList.FirstOrDefault(
                                k => k.Month == date.Month && k.Day == date.Day && k.Year == date.Year);

                        var eventData =
                            eventsList.FirstOrDefault(
                                k => k.Month == date.Month && k.Day == date.Day && k.Year == date.Year);
                        var events = eventData != null ? eventData.Events : null;

                        info.PurchasedData.Add(purchasedData != null ? purchasedData.Amount : 0);
                        info.AbandonedData.Add(abandonedData != null ? abandonedData.Amount : 0);
                        info.BouncedData.Add(bouncedData != null ? bouncedData.Amount : 0);

                        switch (period)
                        {
                            case SalesPeriod.Year:
                            case SalesPeriod.Quarter:
                                var monthLabel = string.Empty;
                                if (date.Day == 15)
                                    monthLabel = date.ToString("MMMM");
                                info.ChartLabels.Add(monthLabel);
                                break;
                            case SalesPeriod.Month:
                                info.ChartLabels.Add(date.Day.ToString());
                                break;
                        }

                        info.Events.Add(null);
                        if (events != null)
                        {
                            var uniqueEvents = events.Select(e => e.Action).Distinct().ToArray();
                            info.Events.Insert(i, uniqueEvents);
                        }
                        date = date.AddDays(1);
                        i++;
                    }
                }
                    break;
                case SalesPeriod.Week:
                {
                    var purchasedList = qPuchasedItems
                        .Select(li => new {LineItem = li, li.hcc_Order.TimeOfOrder.Day, li.hcc_Order.TimeOfOrder.Hour})
                        .GroupBy(o => new {o.Day, o.Hour})
                        .Select(g => new {g.Key.Day, g.Key.Hour, Amount = g.Sum(gi => gi.LineItem.Quantity)})
                        .ToList();
                    var abandonedList = qAbandonedItems
                        .Select(li => new {LineItem = li, li.hcc_Order.TimeOfOrder.Day, li.hcc_Order.TimeOfOrder.Hour})
                        .GroupBy(o => new {o.Day, o.Hour})
                        .Select(g => new {g.Key.Day, g.Key.Hour, Amount = g.Sum(gi => gi.LineItem.Quantity)})
                        .ToList();
                    var bouncedList = RepoStrategyFunctions.GetBouncedListByHour(db,
                        storeId,
                        range.StartDate,
                        range.EndDate,
                        productGuid,
                        categoryGuid);
                    var eventsList = qAnalyticsEvents
                        .Select(ae => new {Event = ae, ae.DateTime.Day, ae.DateTime.Hour})
                        .GroupBy(o => new {o.Day, o.Hour})
                        .Select(g => new {g.Key.Day, g.Key.Hour, Events = g.Select(e => e.Event).ToList()})
                        .ToList();

                    var i = 0;
                    var date = range.StartDate;
                    while (date <= range.EndDate)
                    {
                        var purchasedData = purchasedList.FirstOrDefault(k => k.Day == date.Day && k.Hour == date.Hour);
                        var abandonedData = abandonedList.FirstOrDefault(k => k.Day == date.Day && k.Hour == date.Hour);
                        var bouncedData = bouncedList.FirstOrDefault(k => k.Day == date.Day && k.Hour == date.Hour);

                        var eventData = eventsList.FirstOrDefault(k => k.Day == date.Day && k.Hour == date.Hour);
                        var events = eventData != null ? eventData.Events : null;

                        info.PurchasedData.Add(purchasedData != null ? purchasedData.Amount : 0);
                        info.AbandonedData.Add(abandonedData != null ? abandonedData.Amount : 0);
                        info.BouncedData.Add(bouncedData != null ? bouncedData.Amount : 0);

                        var dayLabel = string.Empty;
                        if (date.Hour == 12)
                            dayLabel = ci.DateTimeFormat.GetDayName(date.DayOfWeek);
                        info.ChartLabels.Add(dayLabel);

                        info.Events.Add(null);
                        if (events != null)
                        {
                            var uniqueEvents = events.Select(e => e.Action).Distinct().ToArray();
                            info.Events.Insert(i, uniqueEvents);
                        }

                        date = date.AddHours(1);
                        i++;
                    }
                }
                    break;
                default:
                    break;
            }

            return info;
        }

        #endregion
    }
}