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
using System.Collections.Generic;
using System.Web.Mvc;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class TopWeeklySellersController : BaseAppController
    {
        public ActionResult Index()
        {
            var model = new SideMenuViewModel {Title = "Top Weekly Sellers"};

            var _StartDate = DateTime.Now;
            var _EndDate = DateTime.Now;
            var c = DateTime.Now;
            CalculateDates(c, ref _StartDate, ref _EndDate);
            model.Items = LoadProducts(_StartDate, _EndDate);

            return View(model);
        }

        public void CalculateDates(DateTime currentTime, ref DateTime start, ref DateTime end)
        {
            start = FindStartOfWeek(currentTime);
            end = start.AddDays(7);
            end = end.AddMilliseconds(-1);
        }

        private DateTime FindStartOfWeek(DateTime currentDate)
        {
            var result = currentDate;
            switch (currentDate.DayOfWeek)
            {
                case DayOfWeek.Sunday:
                    result = new DateTime(currentDate.Year, currentDate.Month, currentDate.Day, 0, 0, 0, 0);
                    break;
                case DayOfWeek.Monday:
                    result = currentDate.AddDays(-1);
                    break;
                case DayOfWeek.Tuesday:
                    result = currentDate.AddDays(-2);
                    break;
                case DayOfWeek.Wednesday:
                    result = currentDate.AddDays(-3);
                    break;
                case DayOfWeek.Thursday:
                    result = currentDate.AddDays(-4);
                    break;
                case DayOfWeek.Friday:
                    result = currentDate.AddDays(-5);
                    break;
                case DayOfWeek.Saturday:
                    result = currentDate.AddDays(-6);
                    break;
            }
            result = new DateTime(result.Year, result.Month, result.Day, 0, 0, 0, 0);
            return result;
        }

        private List<SideMenuItem> LoadProducts(DateTime start, DateTime end)
        {
            var s = start;
            var e = end;

            var t = HccApp.ReportingTopSellersByDate(s, e, 10);

            var result = new List<SideMenuItem>();
            foreach (var p in t)
            {
                var item = new SideMenuItem
                {
                    Title = p.ProductName,
                    Name = p.ProductName,
                    Url = UrlRewriter.BuildUrlForProduct(p)
                };
                result.Add(item);
            }
            return result;
        }
    }
}