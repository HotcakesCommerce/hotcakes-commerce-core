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
using System.Web.Mvc;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Payment;

namespace Hotcakes.Modules.Core.Controllers
{
    [Serializable]
    public class EditBillingController : BaseAppController
    {
        public ActionResult Index()
        {
            var order = RouteData.Values["order"] as Order;
            var model = new EditBillingViewModel {OrderId = order.bvin};

            LoadPaymentModel(model, order);
            LoadAddress(model, order);

            return View(model);
        }

        [HccHttpPost]
        public JsonResult Save(FormCollection form)
        {
            var model = new EditBillingViewModel();

            if (TryUpdateModel(model))
            {
                var orderId = model.OrderId;
                var o = HccApp.OrderServices.Orders.FindForCurrentStore(orderId);
                var payManager = new OrderPaymentManager(o, HccApp);

                model.AddressModel.CopyTo(o.BillingAddress);
                HccApp.OrderServices.Orders.Update(o);

                var cardForm = model.PaymentModel.CreditCardForm;

                var cardData = new CardData
                {
                    CardNumber = cardForm.CardNumber,
                    ExpirationMonth = cardForm.ExpirationMonth,
                    ExpirationYear = cardForm.ExpirationYear,
                    SecurityCode = cardForm.SecurityCode,
                    CardHolderName = cardForm.CardHolderName
                };

                foreach (var li in o.Items)
                {
                    li.RecurringBilling.LoadPaymentInfo(HccApp);

                    if (!li.RecurringBilling.IsCancelled)
                    {
                        var result = payManager.RecurringSubscriptionUpdate(li.Id, cardData);

                        if (!result.Succeeded)
                        {
                            ModelState.AddModelError("rbUpdate",
                                string.Format("Subscription '{0}' update failed.", li.ProductName));
                        }
                    }
                }

                if (ModelState.IsValid)
                {
                    return Json(new { Status = "OK", Message = string.Empty });
                }
            }

            return Json(new {Status = "Invalid", Message = GetValidationSummaryMessage()});
        }

        [HccHttpPost]
        public JsonResult CleanCreditCard(string number)
        {
            var cardNumber = CardValidator.CleanCardNumber(number);
            return Json(cardNumber);
        }

        private void LoadAddress(EditBillingViewModel model, Order order)
        {
            var aModel = new AddressViewModel(order.BillingAddress)
            {
                Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries()
            };

            var country = HccApp.GlobalizationServices.Countries.Find(aModel.CountryBvin);
            aModel.Regions = new List<Region>(country.Regions);

            var notSelectedRegion = new Region
            {
                DisplayName = Localization.GetString("NotSelectedItem"),
                Abbreviation = aModel.Regions.Count == 0 ? "_" : string.Empty
            };
            aModel.Regions.Insert(0, notSelectedRegion);

            model.AddressModel = aModel;
        }

        private void LoadPaymentModel(EditBillingViewModel model, Order order)
        {
            var payManager = new OrderPaymentManager(order, HccApp);
            var pModel = new PaymentViewModel
            {
                PaymentMethods = PaymentMethods.EnabledMethods(HccApp.CurrentStore, true),
                CreditCardForm = {AcceptedCardTypes = HccApp.CurrentStore.Settings.PaymentAcceptedCards}
            };

            var trInfo = payManager.GetLastCreditCardTransaction();

            pModel.CreditCardForm.ExpirationMonth = trInfo.CreditCard.ExpirationMonth;
            pModel.CreditCardForm.ExpirationYear = trInfo.CreditCard.ExpirationYear;
            pModel.CreditCardForm.CardHolderName = trInfo.CreditCard.CardHolderName;
            pModel.CreditCardForm.CardNumber = "XXXXXXXXXXXX" + trInfo.CreditCard.CardNumberLast4Digits;

            model.PaymentModel = pModel;
        }
    }
}