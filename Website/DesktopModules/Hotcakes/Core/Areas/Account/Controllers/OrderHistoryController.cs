#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Mvc;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Storage;
using Hotcakes.Commerce.Urls;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Modules.Core.Areas.Account.Models;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Areas.Account.Controllers
{
    [CustomerSignedInFilter]
    [Serializable]
    public class OrderHistoryController : BaseStoreController
    {
        #region Public methods

        public ActionResult Index()
        {
            var model = new OrderHistoryViewModel();
            LoadOrders(model);
            model.RewardPoints = HccApp.CustomerPointsManager.FindAvailablePoints(HccApp.CurrentCustomerId);

            return View(model);
        }

        public ActionResult Details(string id)
        {
            var model = LoadOrder(id);

            return View(model);
        }

        public ActionResult DownloadFile(string id)
        {
            var orderBvin = Request.QueryString["oid"] ?? string.Empty;
            var fileId = id;
            var userId = HccApp.CurrentCustomerId;

            var o = HccApp.OrderServices.Orders.FindForCurrentStore(orderBvin);
            var file = HccApp.CatalogServices.ProductFiles.Find(fileId);

            // Validation checks
            if (file == null)
            {
                FlashWarning("The file could not be located for download.");
                return View("DownloadFileError");
            }
            if (o == null)
            {
                FlashWarning("The order number could not be located for downloads.");
                return View("DownloadFileError");
            }
            if (o.UserID != userId)
            {
                FlashWarning("This order does not belong to the current user. Please try again.");
                return View("DownloadFileError");
            }

            if (file.MaxDownloads <= 0) file.MaxDownloads = 32000;
            var currentCount = o.GetFileDownloadCount(file.Bvin);
            if (currentCount >= file.MaxDownloads)
            {
                FlashWarning(
                    "This file has already been downloaded the maximum number of allowed times. Please contact store administrator for assistance.");
                return View("DownloadFileError");
            }

            if (file.AvailableMinutes != 0)
            {
                if (DateTime.UtcNow.AddMinutes(file.AvailableMinutes*-1) > o.TimeOfOrderUtc)
                {
                    FlashWarning("File can no longer be downloaded. Its available time period has elapsed.");
                    return View("DownloadFileError");
                }
            }

            // Load File from Disk
            var extension = Path.GetExtension(file.FileName);
            var name = Path.GetFileName(file.FileName);

            var storeId = HccApp.CurrentStore.Id;
            var diskFileName = file.Bvin + "_" + file.FileName + ".config";
            if (!DiskStorage.FileVaultFileExists(storeId, diskFileName))
            {
                FlashWarning("The file source code not be located.");
                return View("DownloadFileError");
            }
            var bytes = DiskStorage.FileVaultGetBytes(storeId, diskFileName);
            var type = MimeTypes.FindTypeForExtension(extension);

            // Record download
            o.IncreaseFileDownloadCount(file.Bvin);
            HccApp.OrderServices.Orders.Update(o);

            // Send File
            var r = new FileContentResult(bytes, type) {FileDownloadName = file.FileName};
            return r;
        }

        public JsonResult CancelSubscription(string orderId, long lineItemId)
        {
            var o = HccApp.OrderServices.Orders.FindForCurrentStore(orderId);
            var payManager = new OrderPaymentManager(o, HccApp);
            var res = payManager.RecurringSubscriptionCancel(lineItemId);

            return GetStatusMessage(res);
        }

        #endregion

        #region Implementation

        private JsonResult GetStatusMessage(ResultData res)
        {
            var sb = new StringBuilder();
            foreach (var error in res.Errors)
            {
                sb.AppendFormat("{0} <br/>", error.Description);
            }
            return GetStatusMessage(sb.ToString(), res.Succeeded);
        }

        private void LoadOrders(OrderHistoryViewModel model)
        {
            var pageSize = 20;
            var totalCount = 0;
            var pageNumber = GetPageNumber();

            // pull all Orders
            model.Orders = HccApp.OrderServices.Orders.FindByUserId(HccApp.CurrentCustomerId, pageNumber, pageSize,
                ref totalCount);

            model.PagerData = new PagerViewModel
            {
                PageSize = pageSize,
                TotalItems = totalCount,
                CurrentPage = pageNumber,
                PagerUrlFormat = HccUrlBuilder.RouteHccUrl(HccRoute.OrderHistory, new {page = "{0}"}),
                PagerUrlFormatFirst = HccUrlBuilder.RouteHccUrl(HccRoute.OrderHistory)
            };
        }

        private OrderViewModel LoadOrder(string bvin)
        {
            var o = HccApp.OrderServices.Orders.FindForCurrentStore(bvin);

            if (o.IsRecurring)
            {
                foreach (var li in o.Items)
                {
                    li.RecurringBilling.LoadPaymentInfo(HccApp);
                }

                if (o.Items.Any(i => !i.RecurringBilling.IsCancelled))
                {
                    ViewData["AllowEditBilling"] = true;
                }
            }

            var model = new OrderViewModel(o);

            var paySummary = HccApp.OrderServices.PaymentSummary(o);
            ViewBag.OrderPaymentSummary = paySummary;

            // File Downloads
            var fileDownloads = new List<ProductFile>();
            if (o.PaymentStatus == OrderPaymentStatus.Paid && o.StatusCode != OrderStatusCode.OnHold)
            {
                foreach (var item in o.Items)
                {
                    if (item.ProductId != string.Empty)
                    {
                        var productFiles = HccApp.CatalogServices.ProductFiles.FindByProductId(item.ProductId);
                        foreach (var file in productFiles)
                        {
                            fileDownloads.Add(file);
                        }
                    }
                }
            }
            ViewBag.FileDownloads = fileDownloads;

            return model;
        }

        private int GetPageNumber()
        {
            var result = 1;
            if (Request.QueryString["page"] != null)
            {
                int.TryParse(Request.QueryString["page"], out result);
            }
            if (result < 1) result = 1;
            return result;
        }

        #endregion
    }
}