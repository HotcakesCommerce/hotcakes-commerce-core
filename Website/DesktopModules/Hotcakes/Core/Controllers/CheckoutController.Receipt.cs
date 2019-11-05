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

using System.Collections.Generic;
using System.Web.Mvc;
using DotNetNuke.Security.Membership;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Orders;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Models;

namespace Hotcakes.Modules.Core.Controllers
{
    public partial class CheckoutController
    {
        #region Public methods

        //GET: /checkout/receipt
        [NonCacheableResponseFilter]
        public ActionResult Receipt()
        {
            var model = LoadReceiptOrderModel();

            return View(model);
        }

        #endregion

        #region Implementation

        private OrderViewModel LoadReceiptOrderModel()
        {
            if (Request.Params["id"] != null)
            {
                var o = HccApp.OrderServices.Orders.FindForCurrentStore(Request.Params["id"]);
                if (o == null)
                    StoreExceptionHelper.ShowError(Localization.GetString("OrderNotFound"));

                if (o.CustomProperties.GetProperty("hcc", "allowpasswordreset") == "1" &&
                    MembershipProviderConfig.PasswordRetrievalEnabled)
                {
                    ViewBag.AllowPasswordReset = true;
                    ViewBag.Email = o.UserEmail;
                    ViewBag.OrderBvin = o.bvin;
                }
                else
                {
                    ViewBag.AllowPasswordReset = false;

                    if (HccApp.CurrentCustomerId != o.UserID)
                        StoreExceptionHelper.ShowInfo(Localization.GetString("PleaseLogin"));
                }

                var model = new OrderViewModel(o);

                var paySummary = HccApp.OrderServices.PaymentSummary(o);
                ViewBag.OrderPaymentSummary = paySummary;

                if (o.IsRecurring)
                {
                    foreach (var item in o.Items)
                    {
                        item.RecurringBilling.LoadPaymentInfo(HccApp);
                    }
                }

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

                if (!string.IsNullOrEmpty(SessionManager.AnalyticsOrderId))
                {
                    HccApp.AnalyticsService.RegisterEvent(HccApp.CurrentCustomerId, ActionTypes.ChekoutCompleted, null);
                    RenderAnalytics(o);
                    SessionManager.AnalyticsOrderId = string.Empty;
                }

                return model;
            }
            StoreExceptionHelper.ShowError(Localization.GetString("OrderNumberMissing"));

            return null;
        }

        #endregion
    }
}