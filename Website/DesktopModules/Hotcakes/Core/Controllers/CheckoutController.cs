#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2013-2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2021 Upendo Ventures, LLC
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
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using System.Web.Security;
using System.Text.RegularExpressions;
using DotNetNuke.Security;
using DotNetNuke.Security.Membership;
using DotNetNuke.Services.Exceptions;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Analytics;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Common;
using Hotcakes.Commerce.Dnn;
using Hotcakes.Commerce.Extensions;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Metrics;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Payment;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Urls;
using Hotcakes.Common.Dnn;
using Hotcakes.Modules.Core.Controllers.Shared;
using Hotcakes.Modules.Core.Filters;
using Hotcakes.Modules.Core.Integration;
using Hotcakes.Modules.Core.Models;
using Hotcakes.Modules.Core.Models.Json;
using Hotcakes.Payment;
using Hotcakes.Payment.Gateways;
using Hotcakes.Web.Logging;
using Hotcakes.Web.Validation;
using Address = Hotcakes.Commerce.Contacts.Address;
using static Hotcakes.Payment.Gateways.StripeProcessor;

namespace Hotcakes.Modules.Core.Controllers
{
    [HccCustomHeaders]
    [Serializable]
    public partial class CheckoutController : BaseStoreController
    {
        #region Constants

        private const string LOGIN_MODE_NEWACC = "#hcTabNewAccount";
        private const string LOGIN_MODE_GUEST = "#hcTabGuest";

        private const string HCC_KEY = "hcc";

        private PortalSecurity security = new PortalSecurity();

        #endregion

        #region Properties

        protected bool ShowConfirmation
        {
            get { return false; }
        }

        protected bool IsOrderConfirmed
        {
            get { return Request.Params["hcOrderConfirmed"] == "true"; }
        }

        #endregion

        #region Public methods

        // GET: /checkout
        [NonCacheableResponseFilter]
        public ActionResult Index()
        {
            if (CurrentCart == null || CurrentCart.Items == null || CurrentCart.Items.Count == 0)
            {
                return Redirect(Url.RouteHccUrl(HccRoute.Cart));
            }
            
            var model = LoadCheckoutModel();
            HccApp.AnalyticsService.RegisterEvent(HccApp.CurrentCustomerId, ActionTypes.GoToChekout, null);
            VerifySessionError(model);
            RenderErrorSummary(model);
            CheckFreeItems(model);
            if (HccApp.CurrentStore.Settings.PaymentCreditCardGateway == PaymentGatewayType.Stripe)
            {
                var stripeProcessor = new StripeProcessor();
                var sett = HccApp.CurrentStore.Settings;
                var mSett = sett.PaymentSettingsGet(sett.PaymentCreditCardGateway);
                stripeProcessor.BaseSettings.Merge(mSett);
                var paymentIntentRequest = new PaymentIntentRequestItem() { TotalAmount = model.CurrentOrder.TotalGrand };
                var paymentIntent = stripeProcessor.CreatePaymentIntent(paymentIntentRequest);
                model.PaymentIntentClientSecret = paymentIntent.ClientSecret;
                model.PaymentIntentId = paymentIntent.Id;
                model.StripePublicKey = stripeProcessor.Settings.StripePublicKey;
            }
            return View(model);
        }

        // POST: /checkout
        [NonCacheableResponseFilter]
        [ActionName("Index")]
        [HccHttpPost]
        public ActionResult IndexPost()
        {
           
            var model = LoadCheckoutModel();
            LoadValuesFromForm(model);
            
            var intResult = CheckoutIntegration.Create(HccApp).BeforeCheckoutCompleted(HccApp, model);

            if (!intResult.IsAborted)
            {
                if (ValidateOrder(model))
                {
                    AddPaymentInfoTransaction(model);
                    HccApp.OrderServices.Orders.Update(model.CurrentOrder);

                    var redirectUrl = ProcessOrder(model);
                    if (redirectUrl != null)
                    {
                        return Redirect(redirectUrl.RouteValues.Values.First().ToString());
                    }
                }
            }
            else
            {
                FlashWarning(intResult.AbortMessage);
            }

            RenderErrorSummary(model);

            return View(model);
        }

        /// <summary>
        ///     Gets called after a shipping method was selected, delayed, to update the order summary
        /// </summary>
        /// <returns></returns>
        [HccHttpPost]
        public ActionResult ApplyShippingMethod()
        {
            var result = new OrderSummaryJsonModel();

            var rateKey = Request.Form["MethodId"] ?? string.Empty;
            var orderid = Request.Form["OrderId"] ?? string.Empty;

            rateKey = security.InputFilter(rateKey.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            orderid = security.InputFilter(orderid.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderid);
            HccApp.OrderServices.OrdersRequestShippingMethodByUniqueKey(rateKey, order);
            HccApp.CalculateOrderAndSave(order);

            if (HccApp.CurrentStore.Settings.GiftCard.GiftCardsEnabled && !order.IsRecurring)
            {
                UpdateGiftCardsBalance(order);
                result.GiftCards = LoadGiftCardModel(order);
            }

            result.totalsastable = order.TotalsAsTable();
            result.PaymentViewModel = LoadPaymentsModel(order);

            result.orderitems = order.Items.Select(i => new
            {
                LineTotal = i.LineTotal.ToString("C"),
                LineTotalWithoutDiscounts = i.LineTotalWithoutDiscounts.ToString("C"),
                i.HasAnyDiscounts
            }).ToList();

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        [HccHttpPost]
        public ActionResult ApplyAddressChange(FormCollection form)
        {
            var result = new OrderSummaryJsonModel();

            var orderid = Request.Form["OrderId"] ?? string.Empty;
            orderid = security.InputFilter(orderid.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderid);

            if (order != null)
            {
                var billshipsame = false;
                var billshipsameString = Request.Form["billshipsame"];
                if (!string.IsNullOrEmpty(billshipsameString))
                {
                    billshipsameString = security.InputFilter(billshipsameString.Trim(), PortalSecurity.FilterFlag.NoMarkup);
                    billshipsame = bool.Parse(billshipsameString);
                }

                PopulateAddress(order.ShippingAddress, form, "shipping");
                if (billshipsame)
                {
                    order.ShippingAddress.CopyTo(order.BillingAddress);
                }
                else
                {
                    PopulateAddress(order.BillingAddress, form, "billing");
                }

                HccApp.CalculateOrderAndSave(order);

                if (HccApp.CurrentStore.Settings.GiftCard.GiftCardsEnabled && !order.IsRecurring)
                {
                    UpdateGiftCardsBalance(order);
                    result.GiftCards = LoadGiftCardModel(order);
                }

                result.totalsastable = order.TotalsAsTable();
                result.PaymentViewModel = LoadPaymentsModel(order);

                if (HccApp.CurrentCustomer != null)
                {
                    int userPoints;
                    var amountToUse = RewardsPotentialCredit(order, HccApp.CurrentCustomer, out userPoints);
                    var dollarValue = HccApp.CustomerPointsManager.DollarCreditForPoints(amountToUse);
                    result.LabelRewardsUse = "Use " + amountToUse + " [" + dollarValue.ToString("C") + "] " +
                                             HccApp.CurrentStore.Settings.RewardsPointsName;
                }

                result.orderitems = order.Items.Select(i => new
                {
                    LineTotal = i.LineTotal.ToString("C"),
                    LineTotalWithoutDiscounts = i.LineTotalWithoutDiscounts.ToString("C"),
                    i.HasAnyDiscounts,
                    LineTaxPortion = i.TaxPortion
                }).ToList();

                var service = new AddressService(HccApp.CurrentStore);
                result.ShippingValidationResult = VerifyAddress(service, order.ShippingAddress);

                if (!billshipsame)
                {
                    result.BillingValidationResult = VerifyAddress(service, order.BillingAddress);
                }
            }
            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        [HccHttpPost]
        public ActionResult ApplyRewardPointsChange(FormCollection form)
        {
            var result = new OrderSummaryJsonModel();

            var orderid = Request.Form["OrderId"] ?? string.Empty;
            orderid = security.InputFilter(orderid.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderid);

            var useRewardPoints = Request.Form["userewardspoints"] == "1";
            ApplyRewardsPoints(order, HccApp.CurrentCustomer, useRewardPoints);

            HccApp.CalculateOrderAndSave(order);

            if (HccApp.CurrentStore.Settings.GiftCard.GiftCardsEnabled && !order.IsRecurring)
            {
                UpdateGiftCardsBalance(order);
                result.GiftCards = LoadGiftCardModel(order);
            }

            result.totalsastable = order.TotalsAsTable();
            result.PaymentViewModel = LoadPaymentsModel(order);

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }
        
        /// <summary>
        /// Attach Payment method to Stripe Payment Intent
        /// </summary>
        /// <returns></returns>
        [HccHttpPost]
        public ActionResult AttachPaymentMethod()
        {
            var stripeProcessor = new StripeProcessor();
            var sett = HccApp.CurrentStore.Settings;
            var mSett = sett.PaymentSettingsGet(sett.PaymentCreditCardGateway);
            stripeProcessor.BaseSettings.Merge(mSett);

            string paymentIntentId = Request.Form["PaymentIntentId"];
            string paymentMethodId = Request.Form["PaymentMethodId"];
            var result = stripeProcessor.AttachPaymentMethod(paymentMethodId, paymentIntentId);
            return new PreJsonResult(Web.Json.ObjectToJson(new {id = result.Id}));
        }

        [HccHttpPost]
        public ActionResult CleanCreditCard()
        {
            var result = new CleanCreditCardJsonModel();
            var notclean = Request.Form["CardNumber"];
            notclean = security.InputFilter(notclean.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            var context = new HccRequestContext();
            var settingsRepo = Factory.CreateRepo<StoreSettingsRepository>(context);

            var keybytes = Encoding.UTF8.GetBytes(settingsRepo.FindSingleSetting(1, Constants.STORESETTING_AESINITVECTOR).SettingValue);
            var iv = Encoding.UTF8.GetBytes(settingsRepo.FindSingleSetting(1, Constants.STORESETTING_AESKEY).SettingValue);

            var encrypted = Convert.FromBase64String(notclean);
            var decriptedFromJavascript = DecryptStringFromBytes(encrypted, keybytes, iv);

            if (!string.IsNullOrEmpty(decriptedFromJavascript))
            {
                result.CardNumber = CardValidator.CleanCardNumber(decriptedFromJavascript);
                return new PreJsonResult(Web.Json.ObjectToJson(result));
            }

            return new PreJsonResult(string.Empty);
        }

        private static string DecryptStringFromBytes(byte[] cipherText, byte[] key, byte[] iv)
        {
            // Check arguments.  
            if (cipherText == null || cipherText.Length <= 0)
            {
                return null;
            }
            if (key == null || key.Length <= 0)
            {
                return null;
            }
            if (iv == null || iv.Length <= 0)
            {
                return null;
            }

            string plaintext = null;
            using (var rijAlg = new RijndaelManaged())
            {
                //Settings  
                rijAlg.Mode = CipherMode.CBC;
                rijAlg.Padding = PaddingMode.PKCS7;
                rijAlg.FeedbackSize = 128;

                rijAlg.Key = key;
                rijAlg.IV = iv;

                // Create a decrytor to perform the stream transform.  
                var decryptor = rijAlg.CreateDecryptor(rijAlg.Key, rijAlg.IV);

                try
                {
                    // Create the streams used for decryption.  
                    using (var msDecrypt = new MemoryStream(cipherText))
                    {
                        using (var csDecrypt = new CryptoStream(msDecrypt, decryptor, CryptoStreamMode.Read))
                        {

                            using (var srDecrypt = new StreamReader(csDecrypt))
                            {
                                // Read the decrypted bytes from the decrypting stream  
                                // and place them in a string.  
                                plaintext = srDecrypt.ReadToEnd();

                            }

                        }
                    }
                }
                catch (Exception ex)
                {
                    Exceptions.LogException(ex);
                }
            }

            return plaintext;
        }

        [HccHttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult IsEmailKnown()
        {
            var result = new IsEmailKnownJsonModel();
            var email = Request.Form["email"];
            email = security.InputFilter(email.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            email = email.Trim().ToLower();

            var users = HccApp.MembershipServices.Customers.FindByEmail(email);

            if (users != null && users.Count > 0)
            {
                var customer = users.FirstOrDefault();

                if (!string.IsNullOrEmpty(customer.Bvin))
                {
                    result.success = "1";
                    result.username = customer.Username;
                }
            }

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        [HccHttpPost]
        public ActionResult UserAddress(string addressBvin)
        {
            var user = HccApp.CurrentCustomer;
            Address address = null;
            if (user != null)
            {
                address = user.Addresses.
                    OfType<Address>().
                    FirstOrDefault(a => a.Bvin == addressBvin);
            }
            return new PreJsonResult(Web.Json.ObjectToJson(address));
        }

        [HccHttpPost]
        public ActionResult AddNewGiftCard(FormCollection form)
        {
            var cardNumber = form["card"];
            var order = CurrentCart;
            var payManager = new OrderPaymentManager(order, HccApp);
            var result = new AddNewGiftCardJsonModel();

            UpdateGiftCardsBalance(order);

            var warning = string.Empty;

            if (!order.Items.Any(i => i.IsGiftCard))
            {
                var totalDue = order.TotalGrandAfterStoreCredits(HccApp.OrderServices);
                var duplicate = payManager.GiftCardInfoFindByNumber(cardNumber) != null;

                if (totalDue > 0 && !string.IsNullOrWhiteSpace(cardNumber) && !duplicate)
                {
                    var gcBalance = payManager.GiftCardBalanceInquiry(cardNumber);

                    if (gcBalance.CurrentValue > 0 && gcBalance.Success)
                    {
                        var toApply = Math.Min(gcBalance.CurrentValue, totalDue);
                        payManager.GiftCardAddInfo(new GiftCardData { CardNumber = cardNumber }, toApply);
                    }
                    else
                    {
                        // Build warning message
                        warning += Localization.GetString("GiftCardInvalidWarning") + "<br />";

                        foreach (var m in gcBalance.Messages)
                        {
                            warning += m.Description + "<br />";
                        }
                    }
                }
            }
            HccApp.CalculateOrderAndSave(order);

            result.GiftCards = LoadGiftCardModel(order, warning);
            result.TotalsAsTable = order.TotalsAsTable();
            result.PaymentViewModel = LoadPaymentsModel(order);

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        [HccHttpPost]
        public ActionResult RemoveGiftCard(FormCollection form)
        {
            var cardNumber = form["card"];
            var result = new AddNewGiftCardJsonModel();

            var payManager = new OrderPaymentManager(CurrentCart, HccApp);

            if (payManager.GiftCardRemoveInfo(cardNumber))
            {
                HccApp.CalculateOrderAndSave(CurrentCart);
            }

            UpdateGiftCardsBalance(CurrentCart);
            result.GiftCards = LoadGiftCardModel(CurrentCart);
            result.TotalsAsTable = CurrentCart.TotalsAsTable();
            result.PaymentViewModel = LoadPaymentsModel(CurrentCart);

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        [HccHttpPost]
        public ActionResult GetCustomerData()
        {
            var model = LoadCheckoutModel();
            var result = new CustomerDataJsonModel();

            var rewardPoints = result.RewardPoints;
            rewardPoints.ShowRewards = model.ShowRewards;
            rewardPoints.LabelRewardsUse = model.LabelRewardsUse;
            rewardPoints.LabelRewardPoints = model.LabelRewardPoints;
            rewardPoints.RewardPointsAvailable = model.RewardPointsAvailable;

            result.Addresses = model.CurrentCustomer.Addresses;
            result.BillingAddress = model.CurrentOrder.BillingAddress;
            result.ShippingAddress = model.CurrentOrder.ShippingAddress;

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        /// <summary>
        ///     Gets called after the VAT number was changed.
        /// </summary>
        /// <returns></returns>
        [HccHttpPost]
        public ActionResult ApplyEUVatRules()
        {
            // https://www.oreilly.com/library/view/regular-expressions-cookbook/9781449327453/ch04s21.html - 23.06.2020
            Regex EUVatRegex = new Regex(@"^(
            (AT)?U[0-9]{8} |                              # Austria
            (BE)?0[0-9]{9} |                              # Belgium
            (BG)?[0-9]{9,10} |                            # Bulgaria
            (CY)?[0-9]{8}L |                              # Cyprus
            (CZ)?[0-9]{8,10} |                            # Czech Republic
            (DE)?[0-9]{9} |                               # Germany
            (DK)?[0-9]{8} |                               # Denmark
            (EE)?[0-9]{9} |                               # Estonia
            (EL|GR)?[0-9]{9} |                            # Greece
            (ES)?[0-9A-Z][0-9]{7}[0-9A-Z] |               # Spain
            (FI)?[0-9]{8} |                               # Finland
            (FR)?[0-9A-Z]{2}[0-9]{9} |                    # France
            (HU)?[0-9]{8} |                               # Hungary
            (IE)?[0-9]S[0-9]{5}L |                        # Ireland
            (IT)?[0-9]{11} |                              # Italy
            (LT)?([0-9]{9}|[0-9]{12}) |                   # Lithuania
            (LU)?[0-9]{8} |                               # Luxembourg
            (LV)?[0-9]{11} |                              # Latvia
            (MT)?[0-9]{8} |                               # Malta
            (NL)?[0-9]{9}B[0-9]{2} |                      # Netherlands
            (PL)?[0-9]{10} |                              # Poland
            (PT)?[0-9]{9} |                               # Portugal
            (RO)?[0-9]{2,10} |                            # Romania
            (SE)?[0-9]{12} |                              # Sweden
            (SI)?[0-9]{8} |                               # Slovenia
            (SK)?[0-9]{10}                                # Slovakia
            )$", RegexOptions.IgnoreCase | RegexOptions.IgnorePatternWhitespace);


            var countryRepo = Factory.CreateRepo<CountryRepository>();
            var storeIsoCode = countryRepo.Find(HccApp.ContactServices.Addresses.FindStoreContactAddress().CountryBvin).IsoCode;

            string result = null;
            string userVatNumber = security.InputFilter(Regex.Replace(Request.Form["UserVatNumber"], "[-. ]", ""), PortalSecurity.FilterFlag.NoMarkup);
            bool foundMatch = EUVatRegex.IsMatch(userVatNumber);

            var orderid = Request.Form["OrderId"] ?? string.Empty;
            orderid = security.InputFilter(orderid.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            var order = HccApp.OrderServices.Orders.FindForCurrentStore(orderid);

            // EU VAT: remove tax if VAT number is valid and customer is not in the same country as seller
            if (foundMatch)
            {
                if (storeIsoCode != userVatNumber.Substring(0, 2).ToUpper())
                {
                    foreach (var i in order.Items)
                    {
                        i.IsTaxExempt = true;
                    }
                    HccApp.CalculateOrderAndSave(order);
                    result = "VatFree";
                }
                else result = "SameCountry";
            }
            else
            {
                foreach (var i in order.Items)
                {
                    i.IsTaxExempt = HccApp.CatalogServices.Products.FindBySku(i.ProductSku).TaxExempt;
                }
                HccApp.CalculateOrderAndSave(order);
                result = Localization.GetString("VatNumberValMsg");
            }

            return new PreJsonResult(Web.Json.ObjectToJson(result));
        }

        #endregion

        #region Implementation / Load model

        private CheckoutViewModel LoadCheckoutModel()
        {
            var model = new CheckoutViewModel { CurrentOrder = CurrentCart };

            LoadCurrentCustomer(model);
            LoadGiftCards(model);
            LoadPayments(model);

            // No shipping order - hide shipping address but show billing address section
            if (!model.CurrentOrder.HasShippingItems) model.BillShipSame = false;

            LoadRewardsPoints(model);
            LoadTermsAgreement(model);

            model.RequirePhoneNumber = HccApp.CurrentStore.Settings.RequirePhoneNumber;

            // Populate Countries
            model.Countries = HccApp.GlobalizationServices.Countries.FindActiveCountries();
            model.ShowAffiliateId = HccApp.CurrentStore.Settings.AffiliateShowIDOnCheckout;

            var context = new HccRequestContext();
            var settingsRepo = Factory.CreateRepo<StoreSettingsRepository>(context);

            model.AESEncryptInitVector = settingsRepo.FindSingleSetting(1, Constants.STORESETTING_AESINITVECTOR).SettingValue;
            model.AESEncryptKey = settingsRepo.FindSingleSetting(1, Constants.STORESETTING_AESKEY).SettingValue;

            VerifyOrderSize(model);

            return model;
        }

        private void VerifySessionError(CheckoutViewModel model)
        {
            var sessionError = SessionManager.GetSessionObject("SessionError_" + model.CurrentOrder.bvin);
            if (sessionError != null && sessionError.ToString() == bool.TrueString)
            {
                model.Violations.Add(new RuleViolation("", "", "Whoops! Your previous session expired. We're sorry, but your previous order can't be placed, and you'll need to checkout again."));
                SessionManager.SetSessionObject("SessionError_" + model.CurrentOrder.bvin, "");
            }
        }

        private void LoadCurrentCustomer(CheckoutViewModel model)
        {
            // Email
            model.IsLoggedIn = SessionManager.IsUserAuthenticated(HccApp);
            if (model.IsLoggedIn)
            {
                model.CurrentCustomer = HccApp.CurrentCustomer;
                model.CurrentOrder.UserEmail = HccApp.CurrentCustomer.Email;

                // Copy customer addresses to order
                model.CurrentCustomer.ShippingAddress.CopyTo(model.CurrentOrder.ShippingAddress);
                if (!model.BillShipSame)
                {
                    var billAddr = model.CurrentCustomer.BillingAddress;
                    billAddr.CopyTo(model.CurrentOrder.BillingAddress);
                }
            }
        }

        private void LoadPayments(CheckoutViewModel model)
        {
            model.PaymentViewModel = LoadPaymentsModel(model.CurrentOrder);
        }

        private CheckoutPaymentViewModel LoadPaymentsModel(Order order)
        {
            var payModel = new CheckoutPaymentViewModel
            {
                PaymentMethods = PaymentMethods.EnabledMethods(HccApp.CurrentStore, order.IsRecurring),
                AcceptedCardTypes = HccApp.CurrentStore.Settings.PaymentAcceptedCards,
                NoPaymentNeeded = (order.TotalGrandAfterStoreCredits(HccApp.OrderServices) <= 0) &&
                                  !order.IsRecurring
            };

            if (!payModel.NoPaymentNeeded && payModel.PaymentMethods.Count > 0)
            {
                payModel.SelectedMethodId = payModel.PaymentMethods.First().MethodId;
            }

            return payModel;
        }

        private void LoadGiftCards(CheckoutViewModel model)
        {
            if (HccApp.CurrentStore.Settings.GiftCard.GiftCardsEnabled && !model.CurrentOrder.IsRecurring)
            {
                UpdateGiftCardsBalance(model.CurrentOrder);
                model.GiftCards = LoadGiftCardModel(model.CurrentOrder);
            }
        }

        private void UpdateGiftCardsBalance(Order order)
        {
            var payManager = new OrderPaymentManager(order, HccApp);
            var totalDue = order.TotalGrandAfterStoreCredits(HccApp.OrderServices);
            var clearAll = order.Items.Any(i => i.IsGiftCard);

            foreach (var gcInfo in payManager.GiftCardInfoListAll())
            {
                var gcBalance = payManager.GiftCardBalanceInquiry(gcInfo.GiftCard.CardNumber);

                if (gcBalance.CurrentValue > 0 && gcBalance.Success && !clearAll)
                {
                    var toApply = Math.Min(gcBalance.CurrentValue, totalDue + gcInfo.Amount);

                    if (toApply > 0)
                    {
                        payManager.GiftCardUpdateInfo(gcInfo, toApply);
                        totalDue -= toApply - gcInfo.Amount;
                    }
                    else
                    {
                        HccApp.OrderServices.Transactions.Delete(gcInfo.Id);
                    }
                }
                else
                {
                    HccApp.OrderServices.Transactions.Delete(gcInfo.Id);
                }
            }
        }

        private GiftCardViewModel LoadGiftCardModel(Order o, string additionalMessage = "")
        {
            var model = new GiftCardViewModel
            {
                ShowGiftCards = HccApp.CurrentStore.Settings.GiftCard.GiftCardsEnabled && !o.IsRecurring
            };

            if (o.Items.Any(i => i.IsGiftCard))
            {
                model.ShowGiftCards = false;
                model.Summary += Localization.GetString("GiftCardCannotUseToBuyGiftCard");
            }

            if (model.ShowGiftCards)
            {
                var payManager = new OrderPaymentManager(o, HccApp);

                foreach (var gcinfo in payManager.GiftCardInfoListAll())
                {
                    var giftCardNumber = gcinfo.GiftCard.CardNumber;
                    var balanceResp = payManager.GiftCardBalanceInquiry(giftCardNumber);

                    model.Cards.Add(new GiftCardItem
                    {
                        CardNumber = giftCardNumber,
                        Balance = balanceResp.CurrentValue.ToString("c"),
                        Charge = gcinfo.Amount.ToString("c")
                    });
                }


                if (model.Cards.Count > 0)
                {
                    var due = o.TotalGrandAfterStoreCredits(HccApp.OrderServices);

                    if (due > 0)
                    {
                        model.Summary = Localization.GetString("GiftCardPartialPayment") + due.ToString("c");
                    }
                    else
                    {
                        model.Summary = IsRewardsPointsApplied(o)
                            ? Localization.GetString("GiftCardAndRewardsPointsWholePayment")
                            : Localization.GetString("GiftCardWholePayment");
                    }
                }

                model.Summary += additionalMessage;
            }

            return model;
        }

        private bool IsRewardsPointsApplied(Order o)
        {
            var trans = HccApp.OrderServices.Transactions.FindForOrder(o.bvin);
            var val = HccApp.OrderServices.Transactions.TransactionsPotentialValue(trans, ActionType.RewardPointsInfo);
            return val > 0;
        }

        private void LoadRewardsPoints(CheckoutViewModel model)
        {
            // Labels
            model.LabelRewardPoints = HccApp.CurrentStore.Settings.RewardsPointsName;
            model.ShowRewards = false;

            if (model.CurrentCustomer != null)
            {
                var uid = model.CurrentCustomer.Bvin;
                if (HccApp.CurrentStore.Settings.RewardsPointsEnabled && !model.CurrentOrder.IsRecurring)
                {
                    int points;
                    var amountToUse = RewardsPotentialCredit(model.CurrentOrder, model.CurrentCustomer, out points);
                    if (amountToUse > 0)
                    {
                        model.ShowRewards = true;
                        var orderTotal = HccApp.CurrentStore.Settings.UseRewardsPointsForUserPrice
                            ? model.CurrentOrder.TotalOrderAfterDiscounts
                            : model.CurrentOrder.TotalOrderWithoutUserPricedProducts;

                        model.RewardPointsAvailable = "You have " + points + " " + model.LabelRewardPoints +
                                                      " available.";
                        var dollarValue = HccApp.CustomerPointsManager.DollarCreditForPoints(amountToUse);
                        model.LabelRewardsUse = "Use " + amountToUse + " [" + dollarValue.ToString("C") + "] " +
                                                model.LabelRewardPoints;
                    }
                }
            }

            model.UseRewardsPoints = false;
            ApplyRewardsPoints(model.CurrentOrder, model.CurrentCustomer, model.UseRewardsPoints);
        }

        private void ApplyRewardsPoints(Order order, CustomerAccount user, bool useRewardPoints)
        {
            // Remove any current points info transactions
            foreach (var t in HccApp.OrderServices.Transactions.FindForOrder(order.bvin))
            {
                if (t.Action == ActionType.RewardPointsInfo)
                {
                    HccApp.OrderServices.Transactions.Delete(t.Id);
                }
            }

            if (!useRewardPoints)
                return;

            int userPoints;
            var amountToUse = RewardsPotentialCredit(order, user, out userPoints);

            var payManager = new OrderPaymentManager(order, HccApp);
            payManager.RewardsPointsAddInfo(amountToUse);
        }

        private void LoadTermsAgreement(CheckoutViewModel model)
        {
            // Agree Checkbox
            if (HccApp.CurrentStore.Settings.ForceTermsAgreement)
            {
                model.ShowAgreeToTerms = true;
                model.AgreedToTerms = false;
                model.AgreedToTermsDescription = Localization.GetString("TermsAndConditionsAgreement");
                model.LabelTerms = Localization.GetString("TermsAndConditions");
            }
            else
            {
                model.ShowAgreeToTerms = false;
                model.AgreedToTerms = true;
            }
        }

        private void VerifyOrderSize(CheckoutViewModel model)
        {
            var c = new OrderTaskContext
            {
                UserId = HccApp.CurrentCustomerId,
                Order = model.CurrentOrder
            };

            if (!Workflow.RunByName(c, WorkflowNames.VerifyOrderSize))
            {
                var customerMessageFound = false;
                foreach (var msg in c.GetCustomerVisibleErrors())
                {
                    customerMessageFound = true;
                    model.Violations.Add(new RuleViolation("VerifyOrderSize Workflow", msg.Name, msg.Description));
                }
                if (!customerMessageFound)
                {
                    model.Violations.Add(new RuleViolation("VerifyOrderSize Workflow", "",
                        "Checkout failed but no errors were recorded."));
                }
            }
        }

        #endregion

        #region Implementation

        private void LoadValuesFromForm(CheckoutViewModel model)
        {
            LoadUserDataFromForm(model);

            // Addresses
            model.BillShipSame = Request.Form["chkbillsame"] != null;
            LoadAddressFromForm("shipping", model.CurrentOrder.ShippingAddress);
            if (model.BillShipSame)
            {
                model.CurrentOrder.ShippingAddress.CopyTo(model.CurrentOrder.BillingAddress);
            }
            else
            {
                LoadAddressFromForm("billing", model.CurrentOrder.BillingAddress);
            }
            // Save addresses to customer account
            if (model.IsLoggedIn)
            {
                model.CurrentOrder.ShippingAddress.CopyTo(model.CurrentCustomer.ShippingAddress);
                if (!model.BillShipSame)
                {
                    model.CurrentOrder.BillingAddress.CopyTo(model.CurrentCustomer.BillingAddress);
                }
                HccApp.MembershipServices.Customers.Update(model.CurrentCustomer);
            }

            //Shipping
            var shippingRateKey = Request.Form["shippingrate"];
            if (!string.IsNullOrEmpty(shippingRateKey))
            {
                shippingRateKey = security.InputFilter(shippingRateKey.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            }

            HccApp.OrderServices.OrdersRequestShippingMethodByUniqueKey(shippingRateKey, model.CurrentOrder);

            // Save Values so far in case of later errors
            HccApp.CalculateOrder(model.CurrentOrder);

            // Reward Points 
            model.UseRewardsPoints = Request.Form["userewardspoints"] == "1";
            ApplyRewardsPoints(model.CurrentOrder, model.CurrentCustomer, model.UseRewardsPoints);

            // Payment Methods
            LoadPaymentFromForm(model);

            // Instructions
            model.CurrentOrder.Instructions = Request.Form["specialinstructions"];
            if (!string.IsNullOrEmpty(model.CurrentOrder.Instructions))
            {
                model.CurrentOrder.Instructions = security.InputFilter(model.CurrentOrder.Instructions.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            }

            // Agree to Terms
            var agreedValue = Request.Form["agreed"];
            if (!string.IsNullOrEmpty(agreedValue))
            {
                model.AgreedToTerms = true;
            }

            // Stripe Data
            var paymentIntentId = Request.Form["PaymentIntentId"];
            if (!string.IsNullOrEmpty(paymentIntentId))
            {
                model.CurrentOrder.ThirdPartyOrderId = paymentIntentId;
                model.PaymentIntentId = paymentIntentId;
            }

            var paymentMethodId = Request.Form["PaymentMethodId"];
            if (!string.IsNullOrEmpty(paymentMethodId))
            {
                model.PaymentMethodId = paymentMethodId;
            }

            var clientSecret = Request.Form["PaymentIntentClientSecret"];
            if (!string.IsNullOrEmpty(clientSecret))
            {
                model.PaymentIntentClientSecret = clientSecret;
            }
            var publicKey = Request.Form["StripePublicKey"];
            if (!string.IsNullOrEmpty(publicKey))
            {
                model.StripePublicKey = publicKey;
            }

            model.AffiliateId = Request.Form["affiliateid"];
            if (!string.IsNullOrEmpty(model.AffiliateId))
            {
                model.AffiliateId = security.InputFilter(model.AffiliateId.Trim(), PortalSecurity.FilterFlag.NoMarkup);

                var referrerUrl = string.Empty;
                if (Request.UrlReferrer != null)
                {
                    referrerUrl = Request.UrlReferrer.AbsoluteUri ?? string.Empty;
                }

                HccApp.ContactServices.SetAffiliateReferral(model.AffiliateId, referrerUrl);
            }

            // Save all the changes to the order
            HccApp.OrderServices.Orders.Update(model.CurrentOrder);
        }

        private void LoadUserDataFromForm(CheckoutViewModel model)
        {
            if (!model.IsLoggedIn)
            {
                /*
                 * This line is kept for backwards compatibility. Before HCC 3.3.0, viewsets all had
                 * 3 radio buttons of the same value, making this line always return the expected
                 * value.
                 */
                model.LoginTabID = Request.Form["loginChoose"];

                if (model.LoginTabID == null)
                {
                    // this is a request from HCC 3.3.0 or newer
                    model.LoginTabID = Request.Form["loginChoose0"] ?? Request.Form["loginChoose2"];
                }

                model.LoginTabID = security.InputFilter(model.LoginTabID.Trim(), PortalSecurity.FilterFlag.NoMarkup);

                // Email
                if (model.LoginTabID == LOGIN_MODE_NEWACC)
                {
                    model.CurrentOrder.UserEmail = Request.Form["regemail"].Trim().ToLower();
                    model.RegUsername = Request.Form["regusername"].Trim().ToLower();

                    model.CurrentOrder.UserEmail = security.InputFilter(model.CurrentOrder.UserEmail, PortalSecurity.FilterFlag.NoMarkup);
                    model.RegUsername = security.InputFilter(model.RegUsername, PortalSecurity.FilterFlag.NoMarkup);

                    if (IsOrderConfirmed)
                    {
                        model.RegPassword = SessionManager.UserRegistrationPassword;
                        SessionManager.UserRegistrationPassword = null;
                    }
                    else
                    {
                        model.RegPassword = Request.Form["regpassword"];
                        if (model.RegPassword == Request.Form["regconfirmpassword"])
                        {
                            model.RegPassword = security.InputFilter(model.RegPassword, PortalSecurity.FilterFlag.NoMarkup);
                            if (ShowConfirmation)
                                SessionManager.UserRegistrationPassword = model.RegPassword;
                        }
                        else
                        {
                            model.Violations.Add(new RuleViolation(Localization.GetString("ConfirmPassword"), string.Empty,
                                Localization.GetString("ConfirmPasswordMismatch")));
                        }
                    }
                }
                else if (model.LoginTabID == LOGIN_MODE_GUEST)
                {
                    model.CurrentOrder.UserEmail = Request.Form["customeremail"].Trim().ToLower();
                    model.CurrentOrder.UserEmail = security.InputFilter(model.CurrentOrder.UserEmail, PortalSecurity.FilterFlag.NoMarkup);
                }
            }
        }

        private void LoadPaymentFromForm(CheckoutViewModel model)
        {
            var order = model.CurrentOrder;
            var payModel = model.PaymentViewModel;
            if (HccApp.CurrentStore.Settings.PaymentCreditCardGateway == PaymentGatewayType.Stripe)
            {
                payModel.DataCreditCard.StripeCardType = Request.Form["StripeCardType"] ?? string.Empty;
            {
            payModel.DataCreditCard.CardNumber = Request.Form["cccardnumber"] ?? string.Empty;
            payModel.SelectedMethodId = Request.Form["paymethod"] ?? string.Empty;
            payModel.DataPurchaseOrderNumber = Request.Form["ponumber"] ?? string.Empty;
            payModel.DataCompanyAccountNumber = Request.Form["accountnumber"] ?? string.Empty;
            payModel.DataCreditCard.CardHolderName = Request.Form["cccardholder"] ?? string.Empty;
            
               
            var expMonth = 0;
            int.TryParse(Request.Form["ccexpmonth"] ?? string.Empty, out expMonth);
            payModel.DataCreditCard.ExpirationMonth = expMonth;
            var expYear = 0;
            int.TryParse(Request.Form["ccexpyear"] ?? string.Empty, out expYear);
            payModel.DataCreditCard.ExpirationYear = expYear;

            var cardSecurityCode = Request.Form["ccsecuritycode"] ?? string.Empty;

            if (!string.IsNullOrEmpty(payModel.SelectedMethodId)) payModel.SelectedMethodId = security.InputFilter(payModel.SelectedMethodId, PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(payModel.DataPurchaseOrderNumber)) payModel.DataPurchaseOrderNumber = security.InputFilter(payModel.DataPurchaseOrderNumber, PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(payModel.DataCompanyAccountNumber)) payModel.DataCompanyAccountNumber = security.InputFilter(payModel.DataCompanyAccountNumber, PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(payModel.DataCreditCard.CardHolderName)) payModel.DataCreditCard.CardHolderName = security.InputFilter(payModel.DataCreditCard.CardHolderName, PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(payModel.DataCreditCard.CardNumber)) payModel.DataCreditCard.CardNumber = security.InputFilter(payModel.DataCreditCard.CardNumber, PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(cardSecurityCode)) cardSecurityCode = security.InputFilter(cardSecurityCode, PortalSecurity.FilterFlag.NoMarkup);

            if (IsOrderConfirmed)
            {
                cardSecurityCode = SessionManager.CardSecurityCode;
                SessionManager.CardSecurityCode = null;
            }
            else
            {
                if (ShowConfirmation)
                {
                    SessionManager.CardSecurityCode = cardSecurityCode;
                }
            }
            payModel.DataCreditCard.SecurityCode = cardSecurityCode;

            payModel.NoPaymentNeeded = (order.TotalGrandAfterStoreCredits(HccApp.OrderServices) <= 0) &&
                                       !order.IsRecurring;

            //Add Payment info only if form is valid and order needs to be processed
            //AddPaymentInfoTransaction(model);
        }

        private void AddPaymentInfoTransaction(CheckoutViewModel model)
        {
            var payManager = new OrderPaymentManager(model.CurrentOrder, HccApp);

            var total = model.CurrentOrder.TotalGrandAfterStoreCredits(HccApp.OrderServices);

            // Don't add payment info if gift cards or points cover the entire order.
            if (total > 0 || model.CurrentOrder.IsRecurring)
            {
                var payModel = model.PaymentViewModel;

                if (!model.CurrentOrder.IsRecurring)
                {
                    switch (payModel.SelectedMethodId)
                    {
                        case PaymentMethods.CreditCardId:
                            payManager.CreditCardAddInfo(payModel.DataCreditCard, total);
                            break;
                        case PaymentMethods.PurchaseOrderId:
                            payManager.PurchaseOrderAddInfo(payModel.DataPurchaseOrderNumber.Trim(), total);
                            break;
                        case PaymentMethods.CompanyAccountId:
                            payManager.CompanyAccountAddInfo(payModel.DataCompanyAccountNumber.Trim(), total);
                            break;
                        case PaymentMethods.CheckId:
                            payManager.OfflinePaymentAddInfo(total, "Customer will pay by check.");
                            break;
                        case PaymentMethods.TelephoneId:
                            payManager.OfflinePaymentAddInfo(total, "Customer will call with payment info.");
                            break;
                        case PaymentMethods.CashOnDeliveryId:
                            payManager.OfflinePaymentAddInfo(total, "Customer will pay cash on delivery.");
                            break;
                        case PaymentMethods.PaypalExpressId:
                            // Need token and id before we can add this to the order
                            // Handled on the checkout page.
                            //payManager.PayPalExpressAddInfo(o.TotalGrand);
                            break;
                        default:
                            break;
                    }
                }
                else
                {
                    switch (payModel.SelectedMethodId)
                    {
                        case PaymentMethods.CreditCardId:
                            payManager.RecurringSubscriptionAddCardInfo(payModel.DataCreditCard);
                            break;
                        default:
                            break;
                    }
                }
            }
        }

        private int RewardsPotentialCredit(Order order, CustomerAccount user, out int userPoints)
        {
            var orderTotal = HccApp.CurrentStore.Settings.UseRewardsPointsForUserPrice
                ? order.TotalOrderAfterDiscounts
                : order.TotalOrderWithoutUserPricedProducts;

            userPoints = HccApp.CustomerPointsManager.FindAvailablePoints(user.Bvin);
            var orderPoints = HccApp.CustomerPointsManager.PointsNeededForPurchaseAmount(orderTotal);
            return Math.Min(userPoints, orderPoints);
        }

        private void LoadAddressFromForm(string prefix, Address address)
        {
            address.CountryBvin = Request.Form[prefix + "country"] ?? address.CountryBvin;
            address.FirstName = Request.Form[prefix + "firstname"] ?? address.FirstName;
            address.LastName = Request.Form[prefix + "lastname"] ?? address.LastName;
            address.Company = Request.Form[prefix + "company"] ?? address.Company;
            address.Line1 = Request.Form[prefix + "address"] ?? address.Line1;
            address.Line2 = Request.Form[prefix + "address2"] ?? address.Line2;
            address.City = Request.Form[prefix + "city"] ?? address.City;
            address.RegionBvin = Request.Form[prefix + "state"] ?? address.RegionBvin;
            if (address.RegionBvin == "_") address.RegionBvin = string.Empty;
            address.PostalCode = Request.Form[prefix + "zip"] ?? address.PostalCode;
            address.Phone = Request.Form[prefix + "phone"] ?? address.Phone;

            if (!string.IsNullOrEmpty(address.CountryBvin)) address.CountryBvin = security.InputFilter(address.CountryBvin.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.FirstName)) address.FirstName = security.InputFilter(address.FirstName.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.LastName)) address.LastName = security.InputFilter(address.LastName.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.Company)) address.Company = security.InputFilter(address.Company.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.Line1)) address.Line1 = security.InputFilter(address.Line1.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.Line2)) address.Line2 = security.InputFilter(address.Line2.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.City)) address.City = security.InputFilter(address.City.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.RegionBvin)) address.RegionBvin = security.InputFilter(address.RegionBvin.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.PostalCode)) address.PostalCode = security.InputFilter(address.PostalCode.Trim(), PortalSecurity.FilterFlag.NoMarkup);
            if (!string.IsNullOrEmpty(address.Phone)) address.Phone = security.InputFilter(address.Phone.Trim(), PortalSecurity.FilterFlag.NoMarkup);

            address.Bvin = string.Empty;
            var user = HccApp.CurrentCustomer;
            if (user != null)
            {
                var addressBookAddressBvin = Request.Form[prefix + "addressbvin"];

                if (!string.IsNullOrEmpty(addressBookAddressBvin)) addressBookAddressBvin = security.InputFilter(addressBookAddressBvin.Trim(), PortalSecurity.FilterFlag.NoMarkup);

                var addressBookAddress = user.Addresses.FirstOrDefault(a => a.Bvin == addressBookAddressBvin);
                if (address.IsEqualTo(addressBookAddress))
                    address.Bvin = addressBookAddressBvin;
            }
        }

        private bool ValidateOrder(CheckoutViewModel model)
        {
            var result = true;

            if (!model.AgreedToTerms && model.ShowAgreeToTerms)
            {
                model.Violations.Add(new RuleViolation("Terms", "Terms",
                    Localization.GetString("SiteTermsAgreementError")));
                result = false;
            }

            // Validate Email
            ValidationHelper.ValidEmail(Localization.GetString("EmailAddressValMsg"), model.CurrentOrder.UserEmail,
                model.Violations, "customeremail");

            if (!model.IsLoggedIn && model.LoginTabID == LOGIN_MODE_NEWACC)
            {
                ValidateNewAccount(model);
            }

            // Validate Shipping Address
            if (model.CurrentOrder.HasShippingItems)
            {
                model.Violations.AddRange(ValidateAddress(model.CurrentOrder.ShippingAddress, "Shipping", model.RequirePhoneNumber));
            }

            // Validate Billing Address
            if (!model.BillShipSame)
            {
                model.Violations.AddRange(ValidateAddress(model.CurrentOrder.BillingAddress, "Billing", model.RequirePhoneNumber));
            }

            // Make sure a shipping method is selected
            // Cart validation checks for shipping method unique key
            if (model.CurrentOrder.HasShippingItems)
            {
                ValidationHelper.Required(Localization.GetString("ShippingMethodValMsg"),
                    model.CurrentOrder.ShippingMethodUniqueKey, model.Violations, "shippingrate");
            }

            // If Gift Card Covers the whole order, skip any payment checks beyond gift cards
            if (CurrentCart.TotalGrandAfterStoreCredits(HccApp.OrderServices) > 0)
            {
                // Payment Validation
                model.Violations.AddRange(ValidatePayment(model));
            }
            if (model.Violations.Count > 0)
            {
                result = false;
            }

            if (result && !IsOrderConfirmed && ShowConfirmation)
            {
                model.ShowConfirmation = true;
                result = false;
            }

            return result;
        }

        private void ValidateNewAccount(CheckoutViewModel model)
        {
            ValidationHelper.Required(Localization.GetString("UsernameValMsg"), Localization.GetString("Username"),
                model.RegUsername, model.Violations, "regusername");
            ValidationHelper.Required(Localization.GetString("PasswordValMsg"), Localization.GetString("Password"),
                model.RegPassword, model.Violations, "regpassword");

            if (MembershipProviderConfig.RequiresUniqueEmail)
            {
                if (HccApp.MembershipServices.Customers.FindByEmail(model.CurrentOrder.UserEmail) != null)
                {
                    model.Violations.Add(new RuleViolation(Localization.GetString("Email"), model.CurrentOrder.UserEmail,
                        Localization.GetString("DuplicateEmail")));
                }
            }

            if (DnnUserController.Instance.IsDnnUsernameExists(model.RegUsername.Trim()))
            {
                model.Violations.Add(new RuleViolation(Localization.GetString("Username"), model.RegUsername,
                    Localization.GetString("UsernameExists"), "regusername"));
            }

            if (!MembershipUtils.CheckPasswordComplexity(Membership.Provider, model.RegPassword))
            {
                var msgInvalidPassword = Localization.GetString("InvalidPassword")
                    .Replace("{MinLength}", Membership.Provider.MinRequiredPasswordLength.ToString());

                model.Violations.Add(new RuleViolation(Localization.GetString("Password"), string.Empty, msgInvalidPassword,
                    "regusername"));
            }
        }

        private List<RuleViolation> ValidateAddress(Address a, string prefix, bool phoneRequired)
        {
            var result = new List<RuleViolation>();

            var pre = prefix.Trim().ToLowerInvariant();

            ValidationHelper.Required(Localization.GetString(prefix + "CountryNameValMsg"), a.CountryBvin, result,
                pre + "countryname");
            ValidationHelper.Required(Localization.GetString(prefix + "FirstNameValMsg"), a.FirstName, result,
                pre + "firstname");
            ValidationHelper.Required(Localization.GetString(prefix + "LastNameValMsg"), a.LastName, result,
                pre + "lastname");
            ValidationHelper.Required(Localization.GetString(prefix + "StreetValMsg"), a.Line1, result, pre + "address");
            ValidationHelper.Required(Localization.GetString(prefix + "CityValMsg"), a.City, result, pre + "city");
            ValidationHelper.Required(Localization.GetString(prefix + "PostalCodeValMsg"), a.PostalCode, result,
                pre + "zip");

            if (phoneRequired)
            {
                ValidationHelper.Required(Localization.GetString(prefix + "PhoneNumberValMsg"), a.Phone, result, pre + "phone");
            }

            var country = HccApp.GlobalizationServices.Countries.Find(a.CountryBvin);
            if (country != null && country.Regions.Count > 0)
                ValidationHelper.Required(Localization.GetString(prefix + "RegionStateValMsg"), a.RegionBvin, result,
                    pre + "state");
            return result;
        }

        private List<RuleViolation> ValidatePayment(CheckoutViewModel model)
        {
            var violations = new List<RuleViolation>();

            // Nothing to validate if no payment is needed
            if (model.PaymentViewModel.NoPaymentNeeded)
            {
                return violations;
            }

            if (!string.IsNullOrEmpty(model.PaymentViewModel.SelectedMethodId))
            {
                switch (model.PaymentViewModel.SelectedMethodId)
                {
                    case PaymentMethods.CreditCardId:
                        return ValidateCreditCard(model);
                    case PaymentMethods.PurchaseOrderId:
                        ValidationHelper.Required(Localization.GetString("PurchaseOrderNumberValMsg"),
                            model.PaymentViewModel.DataPurchaseOrderNumber.Trim(), violations, "purchaseorder");
                        return violations;
                    case PaymentMethods.CompanyAccountId:
                        ValidationHelper.Required(Localization.GetString("CompanyAccountNumberValMsg"),
                            model.PaymentViewModel.DataCompanyAccountNumber.Trim(), violations, "companyaccount");
                        return violations;
                    default:
                        return violations;
                }
            }
            // We haven't return anything so nothing is selected.
            // Try CC as default payment method        
            if (model.PaymentViewModel.DataCreditCard.CardNumber.Length > 12)
            {
                model.PaymentViewModel.SelectedMethodId = PaymentMethods.CreditCardId;
                return ValidateCreditCard(model);
            }

            // nothing selected, trial of cc failed
            violations.Add(new RuleViolation("Payment Method", string.Empty, Localization.GetString("SelectPaymentMethod"), string.Empty));

            return violations;
        }

        private List<RuleViolation> ValidateCreditCard(CheckoutViewModel model)
        {
            var violations = new List<RuleViolation>();

            if (HccApp.CurrentStore.Settings.PaymentCreditCardGateway == PaymentGatewayType.Stripe)
            {
                if (string.IsNullOrEmpty(model.PaymentMethodId))
                {
                    violations.Add(new RuleViolation("Payment Method", string.Empty, Localization.GetString("StripePaymentError"), string.Empty));
                }
                return violations;
            }

            var cardData = model.PaymentViewModel.DataCreditCard;
            if (!CardValidator.IsCardNumberValid(cardData.CardNumber))
            {
                violations.Add(new RuleViolation("Credit Card Number", string.Empty, Localization.GetString("EnterValidCard"),
                    "cccardnumber"));
            }
            var cardTypeCheck = CardValidator.GetCardTypeFromNumber(cardData.CardNumber);
            var acceptedCards = HccApp.CurrentStore.Settings.PaymentAcceptedCards;
            if (!acceptedCards.Contains(cardTypeCheck))
            {
                violations.Add(new RuleViolation("Card Type Not Accepted", string.Empty,
                    Localization.GetString("CardTypeNotAcceptedError"), "cccardnumber"));
            }

            ValidationHelper.RequiredMinimum(1, Localization.GetString("CardExpirationYearValMsg"),
                cardData.ExpirationYear, violations, "ccexpyear");
            ValidationHelper.RequiredMinimum(1, Localization.GetString("CardExpirationMonthValMsg"),
                cardData.ExpirationMonth, violations, "ccexpmonth");
            if (cardData.ExpirationYear > 0 && cardData.ExpirationMonth > 0)
            {
                if (cardData.CardHasExpired(DateTime.UtcNow))
                {
                    var expiryDate = new DateTime(cardData.ExpirationYear, cardData.ExpirationMonth, 1);
                    violations.Add(new RuleViolation(string.Empty, expiryDate.ToString(),
                        Localization.GetString("CreditCardExpirationValMsg")));
                }
            }

            ValidationHelper.Required(Localization.GetString("NameOnCardValMsg"), cardData.CardHolderName, violations,
                "cccardholder");

            if (HccApp.CurrentStore.Settings.PaymentCreditCardRequireCVV)
            {
                ValidationHelper.RequiredMinimum(3, Localization.GetString("CardSecurityCodeValMsg"),
                    cardData.SecurityCode.Length, violations, "ccsecuritycode");
            }
            return violations;
        }

        private RedirectToRouteResult ProcessOrder(CheckoutViewModel model)
        {
            // Save as Order
            var c = new OrderTaskContext
            {
                UserId = HccApp.CurrentCustomerId,
                Order = model.CurrentOrder
            };
            c.Inputs.SetProperty(HCC_KEY, "RegUsername", model.RegUsername);
            c.Inputs.SetProperty(HCC_KEY, "RegPassword", model.RegPassword);
            c.Inputs.SetProperty(HCC_KEY, "CardSecurityCode", model.PaymentViewModel.DataCreditCard.SecurityCode);

            var paymentMethod = PaymentMethods.Find(model.PaymentViewModel.SelectedMethodId);
            if (paymentMethod != null && paymentMethod.PayBeforePlacement)
            {
                c.Inputs.Add(HCC_KEY, "MethodId", paymentMethod.MethodId);
                c.Inputs.Add(HCC_KEY, "ViaCheckout", "1");

                if (!Workflow.RunByName(c, WorkflowNames.ThirdPartyCheckoutSelected))
                {
                    EventLog.LogEvent("Third Party Payment Method Checkout Failed", "Specific Errors to follow",
                        EventLogSeverity.Error);
                    foreach (var item in c.GetCustomerVisibleErrors())
                    {
                        model.Violations.Add(new RuleViolation("Workflow", item.Name, item.Description));
                    }
                }
            }
            else
            {
                if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrder))
                {
                    // Clear Cart ID because we're now an order
                    SessionManager.SetCurrentCartId(HccApp.CurrentStore, string.Empty);

                    // Process Payment
                    if (Workflow.RunByName(c, WorkflowNames.ProcessNewOrderPayments))
                    {
                        Workflow.RunByName(c, WorkflowNames.ProcessNewOrderAfterPayments);
                        var tempOrder = HccApp.OrderServices.Orders.FindForCurrentStore(model.CurrentOrder.bvin);
                        HccApp.CurrentRequestContext.IntegrationEvents.OrderReceived(tempOrder, HccApp);
                        SessionManager.AnalyticsOrderId = model.CurrentOrder.bvin;
                        return RedirectToAction(Url.RouteHccUrl(HccRoute.Checkout,
                            new { action = "receipt", id = model.CurrentOrder.bvin }));
                    }
                    else
                    {
                        // Redirect to Payment Error
                        SessionManager.SetCurrentPaymentPendingCartId(HccApp.CurrentStore, model.CurrentOrder.bvin);
                        return RedirectToAction(Url.RouteHccUrl(HccRoute.Checkout, new { action = "paymenterror" }));
                    }
                }
                else
                {
                    var workflowErrAdded = false;
                    // Show Errors
                    foreach (var item in c.GetCustomerVisibleErrors())
                    {
                        model.Violations.Add(new RuleViolation("Workflow", item.Name, item.Description));
                        workflowErrAdded = true;
                    }

                    if (!workflowErrAdded)
                    {
                        model.Violations.Add(new RuleViolation("Workflow", "Internal Error",
                            Localization.GetString("InternalErrorOccured")));
                    }
                }
            }
            return null;
        }

        private void RenderAnalytics(Order o)
        {
            // Reset Analytics for receipt page
            var analyticsScripts = string.Empty;

            // Add Tracker and Maybe Ecommerce Tracker to Top
            var googleSettings = HccApp.AccountServices.GetGoogleAnalyticsSettings();
            if (googleSettings.UseTracker)
            {
                if (HccApp.CurrentStore.Settings.Analytics.UseGoogleEcommerce)
                {
                    // Ecommerce + Page Tracker
                    analyticsScripts += GoogleAnalytics.RenderLatestTrackerAndTransaction(
                        googleSettings.TrackerId,
                        o,
                        HccApp.CurrentStore.Settings.Analytics.GoogleEcommerceStoreName,
                        HccApp.CurrentStore.Settings.Analytics.GoogleEcommerceCategory);
                }
            }

            // Adwords Tracker at bottom if needed
            if (HccApp.CurrentStore.Settings.Analytics.UseGoogleAdWords)
            {
                analyticsScripts += GoogleAnalytics.RenderGoogleAdwordTracker(
                                                        o.TotalGrand,
                                                        HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsId,
                                                        HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsFormat,
                                                        HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsLabel,
                                                        HccApp.CurrentStore.Settings.Analytics.GoogleAdWordsBgColor,
                                                        Request.IsSecureConnection);
            }

            // Add Yahoo Tracker to Bottom if Needed
            if (HccApp.CurrentStore.Settings.Analytics.UseYahooTracker)
            {
                analyticsScripts += YahooAnalytics.RenderYahooTracker(o,
                    HccApp.CurrentStore.Settings.Analytics.YahooAccountId);
            }

            if (HccApp.CurrentStore.Settings.Analytics.UseShopZillaSurvey)
            {
                analyticsScripts += ShopZilla.RenderReceiptSurvey(o, HccApp);
            }

            RenderToBody("hccECommerceAnalytics", analyticsScripts);
        }

        private AddressValidationJsonModel VerifyAddress(AddressService service, Address address)
        {
            var model = new AddressValidationJsonModel();
            string message;
            Address nmAddr = null;
            model.IsValid = service.Validate(address, out message, out nmAddr);
            var isNormalized = nmAddr != null;

            model.Message = message;
            model.NormalizedAddress = nmAddr;
            model.NormalizedAddressHtml = isNormalized ? nmAddr.GetLinesHtml(false, false) : null;
            model.OriginalAddressHtml = address.GetLinesHtml(false, false);

            return model;
        }

        private void PopulateAddress(Address address, FormCollection form, string prefix)
        {
            var country = form[prefix + "country"] ?? string.Empty;
            var firstname = form[prefix + "firstname"] ?? string.Empty;
            var lastname = form[prefix + "lastname"] ?? string.Empty;
            var addressline = form[prefix + "address"] ?? string.Empty;
            var addressline2 = form[prefix + "address2"] ?? string.Empty;
            var city = form[prefix + "city"] ?? string.Empty;
            var state = form[prefix + "state"] ?? string.Empty;
            var zip = form[prefix + "zip"] ?? string.Empty;

            address.FirstName = firstname;
            address.LastName = lastname;
            address.Line1 = addressline;
            address.Line2 = addressline2;
            address.City = city;
            address.PostalCode = zip;
            var c = HccApp.GlobalizationServices.Countries.Find(country);
            if (c != null)
            {
                address.CountryBvin = country;
                var region = c.Regions.
                    FirstOrDefault(r => r.Abbreviation == state);
                if (region != null)
                    address.RegionBvin = region.Abbreviation;
            }
        }

        private void RenderErrorSummary(CheckoutViewModel model)
        {
            foreach (var v in model.Violations)
            {
                FlashFailure(v.ErrorMessage);
            }
        }

        private void CheckFreeItems(CheckoutViewModel model)
        {
            if (model.CurrentOrder != null)
            {
                var freeItem = model.CurrentOrder
                    .CustomProperties
                    .FirstOrDefault(s => s.DeveloperId == HCC_KEY && s.Key == "outfreeitems");

                if (freeItem != null)
                {
                    var ids = freeItem.Value.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries);

                    if (ids.Any())
                    {
                        var prods = HccApp.CatalogServices.Products.FindManyWithCache(ids);
                        var value = string.Join(", ", prods.Select(p => p.ProductName + " (" + p.Sku + ")"));
                        var text = Localization.GetString("FreeProductIsOut");
                        var message = string.Format(text, value);
                        FlashWarning(message);
                    }
                }
            }
        }

        #endregion
    }
}