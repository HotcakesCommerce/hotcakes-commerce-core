using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Shipping;
using Hotcakes.Commerce.Taxes;
using Hotcakes.Commerce.Tests.IRepository.Admin;
using Hotcakes.Commerce.Tests.TestData;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Payment;
using Hotcakes.Shipping;
using Hotcakes.Shipping.FedEx;
using Hotcakes.Shipping.Services;
using Hotcakes.Shipping.Ups;
using Hotcakes.Shipping.USPostal;
using Address = Hotcakes.Commerce.Contacts.Address;
using Hotcakes.Licensing.Common.DataContracts;

namespace Hotcakes.Commerce.Tests.XmlRepository.Admin
{
    /// <summary>
    /// Repository
    /// </summary>
    public class XmlSetupWizardRepository : IXmlSetupWizardRepository
    {
        /// <summary>
        /// The _xmldoc
        /// </summary>
        private XElement _xmldoc = null;

        /// <summary>
        /// Initializes a new instance
        /// </summary>
        public XmlSetupWizardRepository()
        {
            _xmldoc = new XmlDataContext(Convert.ToString(Models.Enum.DataKeyEnum.SetupWizard)).GetXml();
        }


        /// <summary>
        /// Get Store Data
        /// </summary>
        /// <returns></returns>
        public Store GetStore()
        {
            try
            {
                var store = (_xmldoc.Elements("Store").Select(el => new Store
                    {
                        StoreName = Convert.ToString(el.Element("Name").Value),
                        Id = (el.Element("Id") == null) ? 0 : Convert.ToInt32(el.Element("Id").Value),
                        CustomUrl = Convert.ToString(el.Element("Name").Value),
                        DateCreated = DateTime.Now,
                        StoreGuid = Guid.NewGuid(),
                    })).FirstOrDefault();

                #region Settings

                if (store == null) return new Store();


                #region Other Settings

                var sst = _xmldoc.Elements("Store").Elements("Settings").Select(element => new StoreSettings(store)
                    {
                        MaxItemsPerOrder = element.Element("MaxItemsPerOrder") == null
                                               ? 0
                                               : Convert.ToInt32(element.Element("MaxItemsPerOrder").Value),
                        MaxWeightPerOrder = element.Element("MaxWeightPerOrder") == null
                                                ? 0
                                                : Convert.ToInt32(element.Element("MaxWeightPerOrder").Value),
                        AllowProductReviews = element.Element("AllowProductReviews") == null ||
                                                  Convert.ToBoolean(element.Element("AllowProductReviews").Value),
                        ProductReviewModerate = element.Element("ProductReviewModerate") == null ||
                                                Convert.ToBoolean(element.Element("ProductReviewModerate").Value),
                        ProductReviewCount = element.Element("ProductReviewCount") == null
                                                 ? 0
                                                 : Convert.ToInt32(element.Element("ProductReviewCount").Value),
                        MinumumOrderAmount = element.Element("MinumumOrderAmount") == null
                                                 ? 0
                                                 : Convert.ToDecimal(element.Element("MinumumOrderAmount").Value),
                        LogoText = element.Element("LogoText") == null
                                       ? string.Empty
                                       : Convert.ToString(element.Element("LogoText").Value),
                        UseLogoImage = element.Element("UseLogoImage") != null &&
                                       Convert.ToBoolean(element.Element("UseLogoImage").Value),
                        LogoRevision = element.Element("LogoRevision") == null
                                           ? 0
                                           : Convert.ToInt32(element.Element("LogoRevision").Value),
                        FriendlyName = element.Element("FriendlyName") == null
                                           ? string.Empty
                                           : Convert.ToString(element.Element("FriendlyName").Value),
                        ForceAdminSSL = element.Element("ForceAdminSSL") != null &&
                                        Convert.ToBoolean(element.Element("ForceAdminSSL").Value),
                        LogoImage = element.Element("LogoImage") == null
                                        ? string.Empty
                                        : Convert.ToString(element.Element("LogoImage").Value),
                        PaymentMethodsEnabled = (from f in element.Elements("PaymentMethodsEnabled")
                                                 select Convert.ToString(f.Element("Id"))).ToList(),
                        PaymentCreditCardGateway = element.Element("PaymentCreditCardGateway") == null
                                                       ? string.Empty
                                                       : Convert.ToString(element.Element("PaymentCreditCardGateway").Value),
                        DisplayFullCreditCardNumbers = element.Element("DisplayFullCreditCardNumbers") != null &&
                                                       Convert.ToBoolean(element.Element("DisplayFullCreditCardNumbers").Value),
                        PaymentCreditCardRequireCVV = element.Element("PaymentCreditCardRequireCVV") == null ||
                                                      Convert.ToBoolean(element.Element("PaymentCreditCardRequireCVV").Value),
                        PaymentCreditCardAuthorizeOnly = element.Element("PaymentCreditCardAuthorizeOnly") == null ||
                                                         Convert.ToBoolean(element.Element("PaymentCreditCardAuthorizeOnly").Value),
                        PaymentAcceptedCards = element.Element("PaymentAcceptedCards") == null
                                                   ? new List<CardType>()
                                                   : Convert.ToString(element.Element("PaymentAcceptedCards").Value)
                                                            .Split(',')
                                                            .Select(x => (CardType)Convert.ToInt32(x))
                                                            .ToList(),
                        HandlingAmount = element.Element("HandlingAmount") == null
                                                       ? 0
                                                       : Convert.ToDecimal(element.Element("HandlingAmount").Value),
                        HandlingType = element.Element("HandlingType") == null
                                                       ? 0
                                                       : Convert.ToInt32(element.Element("HandlingType").Value),
                        HandlingNonShipping = element.Element("HandlingNonShipping") != null && Convert.ToBoolean(element.Element("HandlingNonShipping").Value),
                    }).FirstOrDefault();
                if (sst == null)
                    return store;
                store.Settings = sst;

                #endregion

                #region MailServer Settings
                var mailserver = _xmldoc.Elements("Store").Elements("Settings").Elements("MailServer").Select(elmailserver => new StoreSettingsMailServer(sst)
                    {
                        FromEmail = elmailserver.Element("FromEmail") == null
                                                              ? string.Empty
                                                              : Convert.ToString(elmailserver.Element("FromEmail").Value),
                        EmailForGeneral = elmailserver.Element("EmailForGeneral") == null
                                              ? string.Empty
                                              : Convert.ToString(elmailserver.Element("EmailForGeneral").Value),
                        EmailForNewOrder = elmailserver.Element("EmailForNewOrder") == null
                                               ? string.Empty
                                               : Convert.ToString(elmailserver.Element("EmailForNewOrder").Value),
                        UseCustomMailServer = elmailserver.Element("UseCustomMailServer") != null && Convert.ToBoolean(elmailserver.Element("UseCustomMailServer").Value),
                    }).FirstOrDefault();

                if (mailserver != null)
                {
                    store.Settings.MailServer.EmailForGeneral = mailserver.EmailForGeneral;
                    store.Settings.MailServer.FromEmail = mailserver.FromEmail;
                    store.Settings.MailServer.EmailForNewOrder = mailserver.EmailForNewOrder;
                    store.Settings.MailServer.UseCustomMailServer = mailserver.UseCustomMailServer;
                }

                #endregion

                #region Paypal Settings

                var paypal = _xmldoc.Elements("Store").Elements("Settings").Elements("PayPal").Select(elpaypal => new StoreSettingsPayPal(sst)
                {
                    FastSignupEmail = elpaypal.Element("FastSignupEmail") == null
                                                               ? string.Empty
                                                               : Convert.ToString(
                                                                   elpaypal.Element("FastSignupEmail").Value),
                    Currency = elpaypal.Element("Currency") == null
                                   ? string.Empty
                                   : Convert.ToString(elpaypal.Element("Currency").Value),
                    AllowUnconfirmedAddresses = elpaypal.Element("AllowUnconfirmedAddresses") == null || Convert.ToBoolean(elpaypal.Element("AllowUnconfirmedAddresses").Value),
                    ExpressAuthorizeOnly = elpaypal.Element("ExpressAuthorizeOnly") != null && Convert.ToBoolean(elpaypal.Element("ExpressAuthorizeOnly").Value),
                    Signature = elpaypal.Element("Signature") == null
                                    ? string.Empty
                                    : Convert.ToString(elpaypal.Element("Signature").Value),
                    Password = elpaypal.Element("Password") == null
                                   ? string.Empty
                                   : Convert.ToString(elpaypal.Element("Password").Value),
                    Mode = elpaypal.Element("Mode") == null
                               ? string.Empty
                               : Convert.ToString(elpaypal.Element("Mode").Value),
                    UserName = elpaypal.Element("UserName") == null
                                                          ? string.Empty
                                                          : Convert.ToString(elpaypal.Element("UserName").Value),

                }).FirstOrDefault();

                if (paypal != null)
                {
                    store.Settings.PayPal.UserName = paypal.UserName;
                    store.Settings.PayPal.Password = paypal.Password;
                    store.Settings.PayPal.Mode = paypal.Mode;
                    store.Settings.PayPal.Signature = paypal.Signature;
                    store.Settings.PayPal.FastSignupEmail = paypal.FastSignupEmail;
                    store.Settings.PayPal.ExpressAuthorizeOnly = paypal.ExpressAuthorizeOnly;
                    store.Settings.PayPal.AllowUnconfirmedAddresses = paypal.AllowUnconfirmedAddresses;
                    store.Settings.PayPal.Currency = paypal.Currency;
                }


                #endregion

                #region Urls

                var urls = _xmldoc.Elements("Store").Elements("Settings").Elements("Urls").Select(elurls => new StoreSettingsUrls(sst)
                {
                    CategoryUrl = elurls.Element("CategoryUrl") == null
                                      ? string.Empty
                                      : Convert.ToString(elurls.Element("CategoryUrl").Value),
                    ProductUrl = elurls.Element("ProductUrl") == null
                                     ? string.Empty
                                     : Convert.ToString(elurls.Element("ProductUrl").Value),
                    CheckoutUrl = elurls.Element("CheckoutUrl") == null
                                      ? string.Empty
                                      : Convert.ToString(elurls.Element("CheckoutUrl").Value),
                    AddressBookUrl = elurls.Element("AddressBookUrl") == null
                                         ? string.Empty
                                         : Convert.ToString(elurls.Element("AddressBookUrl").Value),
                    CartUrl = elurls.Element("CartUrl") == null
                                  ? string.Empty
                                  : Convert.ToString(elurls.Element("CartUrl").Value),
                    OrderHistoryUrl = elurls.Element("OrderHistoryUrl") == null
                                          ? string.Empty
                                          : Convert.ToString(elurls.Element("OrderHistoryUrl").Value),
                    ProductReviewUrl = elurls.Element("ProductReviewUrl") == null
                                           ? string.Empty
                                           : Convert.ToString(
                                               elurls.Element("ProductReviewUrl").Value),
                    SearchUrl = elurls.Element("SearchUrl") == null
                                    ? string.Empty
                                    : Convert.ToString(elurls.Element("SearchUrl").Value),
                    WishListUrl = elurls.Element("WishListUrl") == null
                                      ? string.Empty
                                      : Convert.ToString(elurls.Element("WishListUrl").Value),
                    HideSetupWizardWelcome = elurls.Element("HideSetupWizardWelcome") != null && Convert.ToBoolean(elurls.Element("HideSetupWizardWelcome").Value),
                }).FirstOrDefault();

                if (urls != null)
                {
                    store.Settings.Urls.CategoryUrl = urls.CategoryUrl;
                    store.Settings.Urls.ProductUrl = urls.ProductUrl;
                    store.Settings.Urls.WishListUrl = urls.WishListUrl;
                    store.Settings.Urls.HideSetupWizardWelcome = urls.HideSetupWizardWelcome;
                    store.Settings.Urls.SearchUrl = urls.SearchUrl;
                    store.Settings.Urls.OrderHistoryUrl = urls.OrderHistoryUrl;
                    store.Settings.Urls.AddressBookUrl = urls.AddressBookUrl;
                    store.Settings.Urls.CheckoutUrl = urls.CheckoutUrl;
                    store.Settings.Urls.ProductReviewUrl = urls.ProductReviewUrl;
                }



                #endregion

                #endregion

                return store;

            }
            catch (Exception)
            {
                return null;
            }
        }

        #region Step0
        /// <summary>
        /// Gets the store URL.
        /// </summary>
        /// <returns></returns>
        public StoreSettingsUrls GetStoreUrl()
        {
            return GetStoreUrlInfo(_xmldoc.Elements("Step0").Elements("StoreUrls"));
        }

        /// <summary>
        /// Shippings the zones.
        /// </summary>
        /// <returns></returns>
        public List<Zone> ShippingZones()
        {
            try
            {
                var lstzone = new List<Zone>();
                foreach (XElement v1 in _xmldoc.Elements("Step0").Elements("ShippingZone").Elements("Name"))
                {
                    lstzone.Add(new Zone
                        {
                            Name = Convert.ToString(v1.Value)
                        });
                }

                return lstzone ?? new List<Zone>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the store URL information.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private StoreSettingsUrls GetStoreUrlInfo(IEnumerable<XElement> element)
        {
            try
            {
                if (element == null) return new StoreSettingsUrls(new Store().Settings);
                var urls = element.Select(el => new StoreSettingsUrls(new Store().Settings)
                {
                    CategoryUrl = Convert.ToString(el.Element("CategoryUrl").Value),
                    ProductUrl = Convert.ToString(el.Element("ProductUrl").Value),
                    CheckoutUrl = Convert.ToString(el.Element("CheckoutUrl").Value),
                    WishListUrl = Convert.ToString(el.Element("WishListUrl").Value),
                    SearchUrl = Convert.ToString(el.Element("SearchUrl").Value),
                    ProductReviewUrl = Convert.ToString(el.Element("ProductReviewUrl").Value),
                    OrderHistoryUrl = Convert.ToString(el.Element("OrderHistoryUrl").Value),
                    CartUrl = Convert.ToString(el.Element("CartUrl").Value),
                    AddressBookUrl = Convert.ToString(el.Element("AddressBookUrl").Value),
                }).FirstOrDefault();
                return urls ?? new StoreSettingsUrls(new Store().Settings);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Step1
        /// <summary>
        /// Gets the store information.
        /// </summary>
        /// <returns></returns>
        public StoreSettings GetStoreInfo()
        {
            try
            {
                var dictionary = (from el in _xmldoc.Elements("Step1").Elements("StoreInfo")
                                  select new StoreSettings(new Store())
                                      {
                                          FriendlyName = Convert.ToString(el.Element("FriendlyName").Value),
                                          ForceAdminSSL = Convert.ToBoolean(el.Element("ForceAdminSSL").Value),
                                          LogoImage = Convert.ToString(el.Element("ImageUrl").Value),

                                      }).FirstOrDefault();
                return dictionary ?? new StoreSettings(new Store());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the pages.
        /// </summary>
        /// <returns></returns>
        public StoreSettingsUrls GetPages()
        {
            return GetStoreUrlInfo(_xmldoc.Elements("Step1").Elements("StoreUrls").Elements("Pages"));
        }

        /// <summary>
        /// Gets the page tab count.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, int> GetPageTabCount()
        {
            try
            {
                var dictionary = _xmldoc.Elements("Step1").Elements("StoreInfo").Select(el => new Dictionary<string, int>
                                            {
                                                {
                                                    "CategoryTabCount",
                                                    (el.Element("CategoryTabCount") == null
                                                         ? 0
                                                         : Convert.ToInt32(el.Element("CategoryTabCount").Value))
                                                },
                                                {
                                                    "ProductTabCount",
                                                    (el.Element("ProductTabCount") == null
                                                         ? 0
                                                         : Convert.ToInt32(el.Element("ProductTabCount").Value))
                                                },
                                                {
                                                    "CheckoutTabCount",
                                                    (el.Element("CheckoutTabCount") == null
                                                         ? 0
                                                         : Convert.ToInt32(el.Element("CheckoutTabCount").Value))
                                                },
                                            }).FirstOrDefault();
                return dictionary ?? new Dictionary<string, int>();
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the country count.
        /// </summary>
        /// <returns></returns>
        public int GetCountryCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step1").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("CountryCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("CountryCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the region count.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetRegionCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step1").Elements("RegionCount").FirstOrDefault();
                if (element == null) return new Dictionary<string, string>();
                return new Dictionary<string, string>
                    {
                        {"CId",element.Element("CId") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("CId").Value)},
                        {"Count",element.Element("Count") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("Count").Value)},
                    };

            }
            catch (Exception)
            {

                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Gets the culture count.
        /// </summary>
        /// <returns></returns>
        public int GetCultureCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step1").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("CultureCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("CultureCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the contact address.
        /// </summary>
        /// <returns></returns>
        public Address GetContactAddress()
        {
            try
            {
                var element = _xmldoc.Elements("Step1").Elements("ContactAddress").FirstOrDefault();
                if (element == null) return null;
                return new Address
                    {
                        FirstName = Convert.ToString(element.Element("FirstName").Value),
                        LastName = Convert.ToString(element.Element("LastName").Value),
                        Company = Convert.ToString(element.Element("Company").Value),
                        Line1 = Convert.ToString(element.Element("Line1").Value),
                        Line2 = Convert.ToString(element.Element("Line2").Value),
                        City = Convert.ToString(element.Element("City").Value),
                        PostalCode = Convert.ToString(element.Element("PostalCode").Value),
                        Phone = Convert.ToString(element.Element("Phone").Value),
                        Bvin = Convert.ToString(element.Element("Bvin").Value),

                    };
            }
            catch (Exception)
            {

                return null;
            }
        }

        /// <summary>
        /// Gets the currency culture code.
        /// </summary>
        /// <returns></returns>
        public string GetCurrencyCultureCode()
        {
            try
            {
                var element = _xmldoc.Elements("Step1").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("CurrencyCultureId") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("CurrencyCultureId").Value);
            }
            catch (Exception)
            {

                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the time zone identifier.
        /// </summary>
        /// <returns></returns>
        public string GetTimeZoneId()
        {
            try
            {
                var element = _xmldoc.Elements("Step1").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("TimeZoneId") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("TimeZoneId").Value);

            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        #endregion

        #region Step2
        /// <summary>
        /// Gets the payment method count.
        /// </summary>
        /// <returns></returns>
        public int GetPaymentMethodCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step2").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("PaymentMethodCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("PaymentMethodCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// Gets the total payment method count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalPaymentMethodCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step2").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("TotalPaymentMethodCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("TotalPaymentMethodCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }

        }

        /// <summary>
        /// Gets the payment methods.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetPaymentMethods()
        {
            var element = _xmldoc.Elements("Step2").Elements("PaymentMethods").Elements("Card");
            if (element == null) return new Dictionary<string, string>();
            var lst = new Dictionary<string, string>();
            foreach (var xElement in element)
            {
                var id = xElement.Element("Id") == null ? string.Empty : Convert.ToString(xElement.Element("Id").Value);
                var name = xElement.Element("Name") == null
                               ? string.Empty
                               : Convert.ToString(xElement.Element("Name").Value);

                if (!string.IsNullOrEmpty(id))
                    lst.Add(id, name);
            }

            return lst;
        }

        /// <summary>
        /// Gets the credit card information.
        /// </summary>
        /// <returns></returns>
        public StoreSettings GetCreditCardInfo()
        {
            try
            {
                var cardinfo = _xmldoc.Elements("Step2").Elements("CreditCardOptions").Select(el => new StoreSettings(new Store())
                                          {
                                              PaymentCreditCardAuthorizeOnly = el.Element("PaymentCreditCardAuthorizeOnly") == null ||
                                                  Convert.ToBoolean(el.Element("PaymentCreditCardAuthorizeOnly").Value),
                                              PaymentCreditCardRequireCVV = el.Element("PaymentCreditCardRequireCVV") == null ||
                                                  Convert.ToBoolean(el.Element("PaymentCreditCardRequireCVV").Value),
                                              DisplayFullCreditCardNumbers = el.Element("DisplayFullCreditCardNumbers") == null ||
                                                  Convert.ToBoolean(el.Element("DisplayFullCreditCardNumbers").Value),
                                              PaymentCreditCardGateway = el.Element("PaymentCreditCardGateway") == null
                                                      ? string.Empty
                                                      : Convert.ToString(el.Element("PaymentCreditCardGateway").Value),
                                              PaymentAcceptedCards = el.Element("PaymentAcceptedCards") == null
                                                      ? new List<CardType>()
                                                      : Convert.ToString(el.Element("PaymentAcceptedCards").Value)
                                                               .Split(',')
                                                               .Select(x => (CardType)Convert.ToInt32(x))
                                                               .ToList(),
                                          }).FirstOrDefault();
                return cardinfo ?? new StoreSettings(new Store());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the total gateway count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalGatewayCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step2").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("TotalGatewayCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("TotalGatewayCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the pay pal information.
        /// </summary>
        /// <returns></returns>
        public StoreSettingsPayPal GetPayPalInfo()
        {
            try
            {
                var element = _xmldoc.Elements("Step2").Elements("PayPalOptions");
                if (element == null) return null;

                var paypalinfo = (from el in _xmldoc.Elements("Step2").Elements("PayPalOptions")
                                  select new StoreSettingsPayPal(new Store().Settings)
                                    {
                                        Currency = el.Element("Currency") == null ? string.Empty : Convert.ToString(el.Element("Currency").Value),
                                        AllowUnconfirmedAddresses = el.Element("AllowUnconfirmedAddresses") == null || Convert.ToBoolean(el.Element("AllowUnconfirmedAddresses").Value),
                                        ExpressAuthorizeOnly = el.Element("ExpressAuthorizeOnly") == null || Convert.ToBoolean(el.Element("ExpressAuthorizeOnly").Value),
                                        FastSignupEmail = el.Element("FastSignupEmail") == null ? string.Empty : Convert.ToString(el.Element("FastSignupEmail").Value),
                                        Signature = el.Element("Signature") == null ? string.Empty : Convert.ToString(el.Element("Signature").Value),
                                        Password = el.Element("Password") == null ? string.Empty : Convert.ToString(el.Element("Password").Value),
                                        Mode = el.Element("Mode") == null ? string.Empty : Convert.ToString(el.Element("Mode").Value),
                                        UserName = el.Element("UserName") == null ? string.Empty : Convert.ToString(el.Element("UserName").Value),
                                    }).FirstOrDefault();
                return paypalinfo ?? new StoreSettingsPayPal(new Store().Settings);
            }
            catch (Exception)
            {
                return null;
            }
        }
        #endregion

        #region Step3
        /// <summary>
        /// Gets the shipping method count.
        /// </summary>
        /// <returns></returns>
        public int GetShippingMethodCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("ShippingMethodCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("ShippingMethodCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        /// <summary>
        /// Gets the total shipping method count.
        /// </summary>
        /// <returns></returns>
        public int GetTotalShippingMethodCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("TotalShippingMethodCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("TotalShippingMethodCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the handling setting.
        /// </summary>
        /// <returns></returns>
        public StoreSettings GetHandlingSetting()
        {
            try
            {
                var cardinfo = _xmldoc.Elements("Step3").Elements("Handling").Select(el => new StoreSettings(new Store())
                {
                    HandlingAmount = el.Element("HandlingAmount") == null
                                                      ? 0
                                                      : Convert.ToDecimal(el.Element("HandlingAmount").Value),
                    HandlingType = el.Element("HandlingType") == null
                                                   ? 0
                                                   : Convert.ToInt32(el.Element("HandlingType").Value),
                    HandlingNonShipping = el.Element("HandlingNonShipping") != null && Convert.ToBoolean(el.Element("HandlingNonShipping").Value),

                }).FirstOrDefault();
                return cardinfo ?? new StoreSettings(new Store());
            }
            catch (Exception)
            {
                return null;
            }
        }

        /// <summary>
        /// Gets the shipping zone count.
        /// </summary>
        /// <returns></returns>
        public int GetShippingZoneCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("ShippingZoneCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("ShippingZoneCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the name of the add shipping zone.
        /// </summary>
        /// <returns></returns>
        public string GetAddShippingZoneName()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippingZone").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("AddZoneName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("AddZoneName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the edit shipping zone.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, string> GetEditShippingZone()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippingZone").Elements("EditZoneName").FirstOrDefault();
                if (element == null) return new Dictionary<string, string>();
                return new Dictionary<string, string>
                    {
                        {"OldName",element.Element("OldName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("OldName").Value)},
                        {"NewName",element.Element("NewName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("NewName").Value)},
                    };
            }
            catch (Exception)
            {
                return new Dictionary<string, string>();
            }
        }

        /// <summary>
        /// Gets the delete shipping zone.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteShippingZone()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippingZone").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("DeleteZoneName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("DeleteZoneName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the add zone area.
        /// </summary>
        /// <returns></returns>
        public Zone GetAddZoneArea()
        {
            return GetZone(_xmldoc.Elements("Step3").Elements("ShippingZone").Elements("ZoneArea").Elements("AddArea").FirstOrDefault());
        }

        /// <summary>
        /// Gets the delete zone area.
        /// </summary>
        /// <returns></returns>
        public Zone GetDeleteZoneArea()
        {
            return GetZone(_xmldoc.Elements("Step3").Elements("ShippingZone").Elements("ZoneArea").Elements("DeleteArea").FirstOrDefault());
        }

        /// <summary>
        /// Gets the zone.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Zone GetZone(XElement element)
        {
            try
            {
                if (element == null) return new Zone();
                return new Zone
                    {
                        Name = element.Element("ZoneName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("ZoneName").Value),
                        Areas = new List<ZoneArea>
                             {
                                 new ZoneArea
                                     {
                                         CountryIsoAlpha3 = element.Element("ContryCode") == null
                                                               ? string.Empty
                                                               : Convert.ToString(element.Element("ContryCode").Value),
                                         RegionAbbreviation = element.Element("RegionCode") == null
                                                               ? string.Empty
                                                               : Convert.ToString(element.Element("RegionCode").Value),
                                         
                                     },
                             }

                    };
            }
            catch (Exception)
            {
                return new Zone();
            }
        }

        /// <summary>
        /// Gets the delete shipping method.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteShippingMethod()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("DeleteMethodName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("DeleteMethodName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        #region Shipping Methods Add/Edit Functions
        /// <summary>
        /// Gets the add flat rate per item.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public FlatRatePerItemSettings GetAddSMInfo_FlatRatePerItem(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("FlatRatePerItem").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new FlatRatePerItemSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);
                var settings = new FlatRatePerItemSettings();
                settings.Merge(spMethod.Settings);
                decimal amount = 0;
                amount = element.Element("Amount") == null
                                     ? 0
                                     : Convert.ToDecimal(element.Element("Amount").Value);
                settings.Amount = Money.RoundCurrency(amount);

                return settings;
            }
            catch (Exception)
            {
                return new FlatRatePerItemSettings();
            }
        }
        /// <summary>
        /// Gets the edit flat rate per item.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public FlatRatePerItemSettings GetEditSMInfo_FlatRatePerItem(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("FlatRatePerItem").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new FlatRatePerItemSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);
                var settings = new FlatRatePerItemSettings();
                settings.Merge(spMethod.Settings);
                decimal amount = 0;
                amount = element.Element("Amount") == null
                                     ? 0
                                     : Convert.ToDecimal(element.Element("Amount").Value);
                settings.Amount = Money.RoundCurrency(amount);

                return settings;
            }
            catch (Exception)
            {
                return new FlatRatePerItemSettings();
            }
        }
        /// <summary>
        /// Gets the add flat rate per order.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public FlatRatePerOrderSettings GetAddSMInfo_FlatRatePerOrder(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("FlatRatePerOrder").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new FlatRatePerOrderSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new FlatRatePerOrderSettings();
                settings.Merge(spMethod.Settings);
                decimal amount = 0;
                amount = element.Element("Amount") == null
                                     ? 0
                                     : Convert.ToDecimal(element.Element("Amount").Value);
                settings.Amount = Money.RoundCurrency(amount);

                return settings;
            }
            catch (Exception)
            {
                return new FlatRatePerOrderSettings();
            }
        }
        /// <summary>
        /// Gets the edit flat rate per order.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public FlatRatePerOrderSettings GetEditSMInfo_FlatRatePerOrder(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("FlatRatePerOrder").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new FlatRatePerOrderSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new FlatRatePerOrderSettings();
                settings.Merge(spMethod.Settings);
                decimal amount = 0;
                amount = element.Element("Amount") == null
                                     ? 0
                                     : Convert.ToDecimal(element.Element("Amount").Value);
                settings.Amount = Money.RoundCurrency(amount);

                return settings;
            }
            catch (Exception)
            {
                return new FlatRatePerOrderSettings();
            }
        }

        /// <summary>
        /// Gets the add rate per weight formula.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public RatePerWeightFormulaSettings GetAddSMInfo_RatePerWeightFormula(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("RatePerWeightFormula").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new RatePerWeightFormulaSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new RatePerWeightFormulaSettings();
                settings.Merge(spMethod.Settings);
                settings.BaseAmount = element.Element("BaseAmount") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("BaseAmount").Value);
                settings.BaseWeight = element.Element("BaseWeight") == null
                                      ? 0
                                      : Convert.ToDecimal(element.Element("BaseWeight").Value);
                settings.MinWeight = element.Element("MinWeight") == null
                                      ? 0
                                      : Convert.ToDecimal(element.Element("MinWeight").Value);
                settings.MaxWeight = element.Element("MaxWeight") == null
                                      ? 0
                                      : Convert.ToDecimal(element.Element("MaxWeight").Value);

                settings.AdditionalWeightCharge = element.Element("AdditionalWeightCharge") == null
                                     ? 0
                                     : Convert.ToDecimal(element.Element("AdditionalWeightCharge").Value);


                return settings;
            }
            catch (Exception)
            {
                return new RatePerWeightFormulaSettings();
            }
        }
        /// <summary>
        /// Gets the edit rate per weight formula.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public RatePerWeightFormulaSettings GetEditSMInfo_RatePerWeightFormula(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("RatePerWeightFormula").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new RatePerWeightFormulaSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new RatePerWeightFormulaSettings();
                settings.Merge(spMethod.Settings);
                settings.BaseAmount = element.Element("BaseAmount") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("BaseAmount").Value);
                settings.BaseWeight = element.Element("BaseWeight") == null
                                      ? 0
                                      : Convert.ToDecimal(element.Element("BaseWeight").Value);
                settings.MinWeight = element.Element("MinWeight") == null
                                      ? 0
                                      : Convert.ToDecimal(element.Element("MinWeight").Value);
                settings.MaxWeight = element.Element("MaxWeight") == null
                                      ? 0
                                      : Convert.ToDecimal(element.Element("MaxWeight").Value);

                settings.AdditionalWeightCharge = element.Element("AdditionalWeightCharge") == null
                                     ? 0
                                     : Convert.ToDecimal(element.Element("AdditionalWeightCharge").Value);



                return settings;
            }
            catch (Exception)
            {
                return new RatePerWeightFormulaSettings();
            }
        }

        /// <summary>
        /// Gets the shipping method information.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <param name="element">The element.</param>
        private void GetShippingMethodInfo(ref ShippingMethod spMethod, XElement element)
        {
            try
            {
                if (element == null) return;
                spMethod.Name = element.Element("Name") == null
                                    ? string.Empty
                                    : Convert.ToString(element.Element("Name").Value);

                spMethod.ZoneId = element.Element("ZoneId") == null
                                      ? 0
                                      : Convert.ToInt32(element.Element("ZoneId").Value);
                spMethod.ShippingProviderId = element.Element("ShippingProviderId") == null
                                    ? string.Empty
                                    : Convert.ToString(element.Element("ShippingProviderId").Value);
            }
            catch (Exception)
            {
                return;
            }
        }

        /// <summary>
        /// Gets the add rate table.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public RateTableSettings GetAddSMInfo_RateTable(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("RateTable").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new RateTableSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new RateTableSettings();
                settings.Merge(spMethod.Settings);

                var level = element.Elements("RateLevel").Select(x => new RateTableLevel
                    {
                        Level = element.Element("Level") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("Level").Value),
                        Percent = element.Element("Percent") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("Percent").Value),
                        Rate = element.Element("Rate") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("Rate").Value),

                    }).ToList();

                if (level.Count > 1)
                {
                    settings.AddLevel(level[0]);
                    settings.AddLevel(level[1]);
                }
                else
                    settings.AddLevel(level[0]);

                return settings;
            }
            catch (Exception)
            {
                return new RateTableSettings();
            }
        }

        /// <summary>
        /// Gets the edit rate table.
        /// </summary>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public RateTableSettings GetEditSMInfo_RateTable(ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("RateTable").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new RateTableSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new RateTableSettings();
                settings.Merge(spMethod.Settings);

                var level = element.Elements("RateLevel").Select(x => new RateTableLevel
                    {
                        Level = element.Element("Level") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("Level").Value),
                        Percent = element.Element("Percent") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("Percent").Value),
                        Rate = element.Element("Rate") == null
                                       ? 0
                                       : Convert.ToDecimal(element.Element("Rate").Value),

                    }).ToList();

                if (level.Count > 1)
                {
                    settings.AddLevel(level[0]);
                    settings.AddLevel(level[1]);
                }
                else
                    settings.AddLevel(level[0]);

                return settings;
            }
            catch (Exception)
            {
                return new RateTableSettings();
            }
        }

        /// <summary>
        /// Gets the add ups.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public UPSServiceSettings GetAddSMInfo_UPS(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("UPS").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new UPSServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                // Global Settings
                store.Settings.ShippingUpsAccountNumber = Convert.ToString(element.Element("ShippingUpsAccountNumber").Value);
                store.Settings.ShippingUpsForceResidential = (element.Element("ShippingUpsForceResidential") != null && Convert.ToBoolean(element.Element("ShippingUpsForceResidential").Value));
                store.Settings.ShippingUpsPickupType = (element.Element("ShippingUpsPickupType") != null ? 0 : Convert.ToInt32(element.Element("ShippingUpsPickupType").Value));
                store.Settings.ShippingUpsDefaultService = (element.Element("ShippingUpsDefaultService") != null ? 0 : Convert.ToInt32(element.Element("ShippingUpsDefaultService").Value));
                store.Settings.ShippingUpsDefaultPackaging = (element.Element("ShippingUpsPickupType") != null ? 0 : Convert.ToInt32(element.Element("ShippingUpsPickupType").Value));
                store.Settings.ShippingUpsSkipDimensions = (element.Element("ShippingUpsSkipDimensions") != null && Convert.ToBoolean(element.Element("ShippingUpsSkipDimensions").Value));
                store.Settings.ShippingUPSDiagnostics = (element.Element("ShippingUPSDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingUPSDiagnostics").Value));


                var settings = new UPSServiceSettings();
                settings.Merge(spMethod.Settings);
                settings.GetAllRates = false;

                // var filter = new List<Hotcakes.Shipping.IServiceCode>();

                var filter = element.Elements("ServiceCodeFilter").SelectMany(x => new List<Hotcakes.Shipping.IServiceCode>
                {
                   new ServiceCode
                       {
                           Code =  Convert.ToString(x.Element("Name").Value),
                           DisplayName =  Convert.ToString(x.Element("Name").Value),
                       },
                  
                }).ToList();

                settings.ServiceCodeFilter = filter;

                return settings;
            }
            catch (Exception)
            {
                return new UPSServiceSettings();
            }
        }

        /// <summary>
        /// Gets the edit ups.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public UPSServiceSettings GetEditSMInfo_UPS(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("UPS").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new UPSServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                // Global Settings
                store.Settings.ShippingUpsAccountNumber = Convert.ToString(element.Element("ShippingUpsAccountNumber").Value);
                store.Settings.ShippingUpsForceResidential = (element.Element("ShippingUpsForceResidential") != null && Convert.ToBoolean(element.Element("ShippingUpsForceResidential").Value));
                store.Settings.ShippingUpsPickupType = (element.Element("ShippingUpsPickupType") != null ? 0 : Convert.ToInt32(element.Element("ShippingUpsPickupType").Value));
                store.Settings.ShippingUpsDefaultService = (element.Element("ShippingUpsDefaultService") != null ? 0 : Convert.ToInt32(element.Element("ShippingUpsDefaultService").Value));
                store.Settings.ShippingUpsDefaultPackaging = (element.Element("ShippingUpsPickupType") != null ? 0 : Convert.ToInt32(element.Element("ShippingUpsPickupType").Value));
                store.Settings.ShippingUpsSkipDimensions = (element.Element("ShippingUpsSkipDimensions") != null && Convert.ToBoolean(element.Element("ShippingUpsSkipDimensions").Value));
                store.Settings.ShippingUPSDiagnostics = (element.Element("ShippingUPSDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingUPSDiagnostics").Value));


                var settings = new UPSServiceSettings();
                settings.Merge(spMethod.Settings);
                settings.GetAllRates = false;

                // var filter = new List<Hotcakes.Shipping.IServiceCode>();

                var filter = element.Elements("ServiceCodeFilter").SelectMany(x => new List<Hotcakes.Shipping.IServiceCode>
                {
                   new ServiceCode
                       {
                           Code =  Convert.ToString(x.Element("Name").Value),
                           DisplayName =  Convert.ToString(x.Element("Name").Value),
                       },
                  
                }).ToList();

                settings.ServiceCodeFilter = filter;

                return settings;
            }
            catch (Exception)
            {
                return new UPSServiceSettings();
            }
        }

        /// <summary>
        /// Gets the add ups internation.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public USPostalServiceSettings GetAddSMInfo_UPS_Internation(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("USPostalService-International").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new USPostalServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                // Global Settings
                store.Settings.ShippingUSPostalDiagnostics = (element.Element("ShippingUSPostalDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingUSPostalDiagnostics").Value));

                var settings = new USPostalServiceSettings();
                settings.Merge(spMethod.Settings);

                var filter = element.Elements("ServiceCodeFilter").SelectMany(x => new List<Hotcakes.Shipping.IServiceCode>
                {
                   new ServiceCode
                       {
                           Code =  Convert.ToString(x.Element("Name").Value),
                           DisplayName =  Convert.ToString(x.Element("Name").Value),
                       },
                  
                }).ToList();

                settings.ServiceCodeFilter = filter;

                return settings;
            }
            catch (Exception)
            {
                return new USPostalServiceSettings();
            }
        }

        /// <summary>
        /// Gets the edit ups internation.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public USPostalServiceSettings GetEditSMInfo_UPS_Internation(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("USPostalService-International").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new USPostalServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                // Global Settings
                store.Settings.ShippingUSPostalDiagnostics = (element.Element("ShippingUSPostalDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingUSPostalDiagnostics").Value));

                var settings = new USPostalServiceSettings();
                settings.Merge(spMethod.Settings);

                var filter = element.Elements("ServiceCodeFilter").SelectMany(x => new List<Hotcakes.Shipping.IServiceCode>
                {
                   new ServiceCode
                       {
                           Code =  Convert.ToString(x.Element("Name").Value),
                           DisplayName =  Convert.ToString(x.Element("Name").Value),
                       },
                  
                }).ToList();

                settings.ServiceCodeFilter = filter;

                return settings;
            }
            catch (Exception)
            {
                return new USPostalServiceSettings();
            }
        }

        /// <summary>
        /// Gets the add ups domestic.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public USPostalServiceSettings GetAddSMInfo_UPS_Domestic(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("USPostalService-Domestic").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new USPostalServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                // Global Settings
                store.Settings.ShippingUSPostalDiagnostics = (element.Element("ShippingUSPostalDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingUSPostalDiagnostics").Value));

                var settings = new USPostalServiceSettings();
                settings.Merge(spMethod.Settings);

                var filter = element.Elements("ServiceCodeFilter").SelectMany(x => new List<Hotcakes.Shipping.IServiceCode>
                {
                   new ServiceCode
                       {
                           Code =  Convert.ToString(x.Element("Name").Value),
                           DisplayName =  Convert.ToString(x.Element("Name").Value),
                       },
                  
                }).ToList();

                settings.ServiceCodeFilter = filter;

                var packageCode = Convert.ToString(element.Element("PackageType").Value);
                var packageCodeInt = -1;
                if (int.TryParse(packageCode, out packageCodeInt))
                {
                    settings.PackageType = (Hotcakes.Shipping.USPostal.v4.DomesticPackageType)packageCodeInt;
                }

                return settings;
            }
            catch (Exception)
            {
                return new USPostalServiceSettings();
            }
        }

        /// <summary>
        /// Gets the edit ups domestic.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public USPostalServiceSettings GetEditSMInfo_UPS_Domestic(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("USPostalService-Domestic").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new USPostalServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                // Global Settings
                store.Settings.ShippingUSPostalDiagnostics = (element.Element("ShippingUSPostalDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingUSPostalDiagnostics").Value));

                var settings = new USPostalServiceSettings();
                settings.Merge(spMethod.Settings);

                var filter = element.Elements("ServiceCodeFilter").SelectMany(x => new List<Hotcakes.Shipping.IServiceCode>
                {
                   new ServiceCode
                       {
                           Code =  Convert.ToString(x.Element("Name").Value),
                           DisplayName =  Convert.ToString(x.Element("Name").Value),
                       },
                  
                }).ToList();

                settings.ServiceCodeFilter = filter;

                var packageCode = Convert.ToString(element.Element("PackageType").Value);
                var packageCodeInt = -1;
                if (int.TryParse(packageCode, out packageCodeInt))
                {
                    settings.PackageType = (Hotcakes.Shipping.USPostal.v4.DomesticPackageType)packageCodeInt;
                }

                return settings;
            }
            catch (Exception)
            {
                return new USPostalServiceSettings();
            }
        }

        /// <summary>
        /// Gets the add  fedex.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public FedExServiceSettings GetAddSMInfo_FedEx(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Add").Elements("FedEx").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new FedExServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new FedExServiceSettings();
                settings.Merge(spMethod.Settings);


                settings.ServiceCode = element.Element("ServiceCode") == null ? 0 : Convert.ToInt32(element.Element("ServiceCode").Value);
                settings.Packaging = element.Element("Packaging") == null ? 0 : Convert.ToInt32(element.Element("Packaging").Value);

                store.Settings.ShippingFedExKey = Convert.ToString(element.Element("ShippingFedExKey").Value);
                store.Settings.ShippingFedExPassword = Convert.ToString(element.Element("ShippingFedExPassword").Value);
                store.Settings.ShippingFedExAccountNumber = Convert.ToString(element.Element("ShippingFedExAccountNumber").Value);
                store.Settings.ShippingFedExMeterNumber = Convert.ToString(element.Element("ShippingFedExMeterNumber").Value);
                store.Settings.ShippingFedExDefaultPackaging = element.Element("ShippingFedExDefaultPackaging") == null ? 0 : Convert.ToInt32(element.Element("ShippingFedExDefaultPackaging").Value);
                store.Settings.ShippingFedExDropOffType = element.Element("ShippingFedExDropOffType") == null ? 0 : Convert.ToInt32(element.Element("ShippingFedExDropOffType").Value);
                store.Settings.ShippingFedExForceResidentialRates = element.Element("ShippingFedExForceResidentialRates") != null && Convert.ToBoolean(element.Element("ShippingFedExForceResidentialRates").Value);
                store.Settings.ShippingFedExUseListRates = element.Element("ShippingFedExUseListRates") != null && Convert.ToBoolean(element.Element("ShippingFedExUseListRates").Value);
                store.Settings.ShippingFedExDiagnostics = element.Element("ShippingFedExDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingFedExDiagnostics").Value);
                store.Settings.ShippingFedExUseDevelopmentServiceUrl = element.Element("ShippingFedExUseDevelopmentServiceUrl") != null && Convert.ToBoolean(element.Element("ShippingFedExUseDevelopmentServiceUrl").Value);


                return settings;
            }
            catch (Exception)
            {
                return new FedExServiceSettings();
            }
        }

        /// <summary>
        /// Gets the edit fedex.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <param name="spMethod">The sp method.</param>
        /// <returns></returns>
        public FedExServiceSettings GetEditSMInfo_FedEx(ref Store store, ref ShippingMethod spMethod)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("Edit").Elements("FedEx").FirstOrDefault();
                if (element == null)
                {
                    spMethod = new ShippingMethod();
                    return new FedExServiceSettings();
                }
                GetShippingMethodInfo(ref spMethod, element);

                var settings = new FedExServiceSettings();
                settings.Merge(spMethod.Settings);


                settings.ServiceCode = element.Element("ServiceCode") == null ? 0 : Convert.ToInt32(element.Element("ServiceCode").Value);
                settings.Packaging = element.Element("Packaging") == null ? 0 : Convert.ToInt32(element.Element("Packaging").Value);

                store.Settings.ShippingFedExKey = Convert.ToString(element.Element("ShippingFedExKey").Value);
                store.Settings.ShippingFedExPassword = Convert.ToString(element.Element("ShippingFedExPassword").Value);
                store.Settings.ShippingFedExAccountNumber = Convert.ToString(element.Element("ShippingFedExAccountNumber").Value);
                store.Settings.ShippingFedExMeterNumber = Convert.ToString(element.Element("ShippingFedExMeterNumber").Value);
                store.Settings.ShippingFedExDefaultPackaging = element.Element("ShippingFedExDefaultPackaging") == null ? 0 : Convert.ToInt32(element.Element("ShippingFedExDefaultPackaging").Value);
                store.Settings.ShippingFedExDropOffType = element.Element("ShippingFedExDropOffType") == null ? 0 : Convert.ToInt32(element.Element("ShippingFedExDropOffType").Value);
                store.Settings.ShippingFedExForceResidentialRates = element.Element("ShippingFedExForceResidentialRates") != null && Convert.ToBoolean(element.Element("ShippingFedExForceResidentialRates").Value);
                store.Settings.ShippingFedExUseListRates = element.Element("ShippingFedExUseListRates") != null && Convert.ToBoolean(element.Element("ShippingFedExUseListRates").Value);
                store.Settings.ShippingFedExDiagnostics = element.Element("ShippingFedExDiagnostics") != null && Convert.ToBoolean(element.Element("ShippingFedExDiagnostics").Value);
                store.Settings.ShippingFedExUseDevelopmentServiceUrl = element.Element("ShippingFedExUseDevelopmentServiceUrl") != null && Convert.ToBoolean(element.Element("ShippingFedExUseDevelopmentServiceUrl").Value);


                return settings;
            }
            catch (Exception)
            {
                return new FedExServiceSettings();
            }
        }

        #endregion

        #region Shipping Methods TestRates Functions

        /// <summary>
        /// Gets the test rate fedex shipment.
        /// </summary>
        /// <param name="store">The store.</param>
        /// <returns></returns>
        public Shipment GetTestRate_FedExShipment(Store store)
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("FedExTestRates").FirstOrDefault();
                if (element == null)
                    return new Shipment();
                var element1 = element.Elements("Source").FirstOrDefault();
                var element2 = element.Elements("Destination").FirstOrDefault();
                var shipment = new Shipment
                    {
                        SourceAddress = GetFedExShippingAddress(element1, store),
                        DestinationAddress = GetFedExShippingAddress(element2, store)
                    };

                return shipment;
            }
            catch (Exception)
            {
                return new Shipment();
            }
        }

        /// <summary>
        /// Gets the fedex shipping address.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <param name="store">The store.</param>
        /// <returns></returns>
        private Address GetFedExShippingAddress(XElement element, Store store)
        {
            if (element == null) return new Address();
            var address = new Address
                {
                    CountryBvin = Convert.ToString(element.Element("Country").Value),
                    RegionBvin = Convert.ToString(element.Element("State").Value),
                    FirstName = Convert.ToString(element.Element("FirstName").Value),
                    LastName = Convert.ToString(element.Element("LastName").Value),
                    Company = Convert.ToString(element.Element("Company").Value),
                    Line1 = Convert.ToString(element.Element("Address1").Value),
                    Line2 = Convert.ToString(element.Element("Address2").Value),
                    City = Convert.ToString(element.Element("City").Value),
                    PostalCode = Convert.ToString(element.Element("Zip").Value),
                    Phone = Convert.ToString(element.Element("Phone").Value)
                };

            const int type = 0;
            address.AddressType = (AddressTypes)type;
            address.StoreId = store.Id;

            return address;
        }

        /// <summary>
        /// Gets the test rate fedex shippable information.
        /// </summary>
        /// <returns></returns>
        public Shippable GetTestRate_FedExShippableInfo()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("FedExTestRates").Elements("Package").FirstOrDefault();
                if (element == null)
                    return new Shippable();
                var shippableinfo = new Shippable
                {
                    BoxHeight = element.Element("Height") == null ? 0 : Convert.ToDecimal(element.Element("Height").Value),
                    BoxLength = element.Element("Length") == null ? 0 : Convert.ToDecimal(element.Element("Length").Value),
                    BoxWidth = element.Element("Width") == null ? 0 : Convert.ToDecimal(element.Element("Width").Value),
                    BoxLengthType = LengthType.Inches,
                    BoxWeight = element.Element("Weight") == null ? 0 : Convert.ToDecimal(element.Element("Weight").Value),
                    BoxWeightType = Hotcakes.Shipping.WeightType.Pounds,
                };
                return shippableinfo;
            }
            catch (Exception)
            {
                return new Shippable();
            }
        }

        /// <summary>
        /// Gets the test rate fedex settings.
        /// </summary>
        /// <returns></returns>
        public FedExGlobalServiceSettings GetTestRate_FedExSettings()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("FedExTestRates").Elements("Package").FirstOrDefault();
                if (element == null)
                    return new FedExGlobalServiceSettings();
                var testSettings = new FedExGlobalServiceSettings
                {
                    AccountNumber = Convert.ToString(element.Element("ShippingFedExAccountNumber").Value),
                    DefaultDropOffType = (DropOffType)(element.Element("ShippingFedExDropOffType") == null ? 0 : Convert.ToInt32(element.Element("ShippingFedExDropOffType").Value)),
                    DefaultPackaging = (PackageType)(element.Element("ShippingFedExDefaultPackaging") == null ? 0 : Convert.ToInt32(element.Element("ShippingFedExDefaultPackaging").Value)),
                    DiagnosticsMode = true,
                    ForceResidentialRates = element.Element("ShippingFedExForceResidentialRates") != null && Convert.ToBoolean(element.Element("ShippingFedExForceResidentialRates").Value),
                    MeterNumber = Convert.ToString(element.Element("ShippingFedExMeterNumber").Value),
                    UseListRates = element.Element("ShippingFedExUseListRates") != null && Convert.ToBoolean(element.Element("ShippingFedExUseListRates").Value),
                    UserKey = Convert.ToString(element.Element("ShippingFedExKey").Value),
                    UserPassword = Convert.ToString(element.Element("ShippingFedExPassword").Value),
                    UseDevelopmentServiceUrl = element.Element("ShippingFedExUseDevelopmentServiceUrl") != null && Convert.ToBoolean(element.Element("ShippingFedExUseDevelopmentServiceUrl").Value),
                };
                return testSettings;
            }
            catch (Exception)
            {
                return new FedExGlobalServiceSettings();
            }
        }

        /// <summary>
        /// Gets the test rate fedex service code.
        /// </summary>
        /// <returns></returns>
        public int GetTestRate_FedExServiceCode()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("FedExTestRates").Elements("Package").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("ServiceCode") == null
                           ? 0
                           : Convert.ToInt32(element.Element("ServiceCode").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        /// <summary>
        /// Gets the test rate ups domestic shipment.
        /// </summary>
        /// <returns></returns>
        public Shipment GetTestRate_UPS_DomesticShipment()
        {
            var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("UPS-DomesticTestRates").FirstOrDefault();
            if (element == null) return new Shipment();
            var shipment = new Hotcakes.Shipping.Shipment
                {
                    DestinationAddress = { PostalCode = Convert.ToString(element.Element("ZipTo").Value) },
                    SourceAddress = { PostalCode = Convert.ToString(element.Element("ZipFrom").Value) }
                };
            return shipment;
        }
        /// <summary>
        /// Gets the test rate ups domestic shippable information.
        /// </summary>
        /// <returns></returns>
        public Shippable GetTestRate_UPS_DomesticShippableInfo()
        {
            var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("UPS-DomesticTestRates").FirstOrDefault();
            if (element == null) return new Shippable();

            var item = new Hotcakes.Shipping.Shippable
                {
                    BoxLength = element.Element("Length") == null ? 0 : Convert.ToDecimal(element.Element("Length").Value),
                    BoxHeight = element.Element("Height") == null ? 0 : Convert.ToDecimal(element.Element("Height").Value),
                    BoxWidth = element.Element("Width") == null ? 0 : Convert.ToDecimal(element.Element("Width").Value),
                    BoxLengthType = Hotcakes.Shipping.LengthType.Inches,
                    BoxWeight = element.Element("Weight") == null ? 0 : Convert.ToDecimal(element.Element("Weight").Value),
                    BoxWeightType = Hotcakes.Shipping.WeightType.Pounds,
                    QuantityOfItemsInBox = 1
                };

            return item;
        }
        /// <summary>
        /// Gets the test rate ups domestic service setting.
        /// </summary>
        /// <returns></returns>
        public USPostalServiceSettings GetTestRate_UPS_DomesticServiceSetting()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("UPS-DomesticTestRates").FirstOrDefault();
                if (element == null) return new USPostalServiceSettings();

                var settings = new USPostalServiceSettings();
                var code = new Hotcakes.Shipping.ServiceCode
                    {
                        Code = Convert.ToString(element.Element("ServiceCodeFilter").Value),
                        DisplayName = Convert.ToString(element.Element("ServiceCodeFilter").Value)
                    };
                var codes = new List<Hotcakes.Shipping.IServiceCode> { code };
                settings.ServiceCodeFilter = codes;
                var temp = -1;
                int.TryParse(Convert.ToString(element.Element("PackageType").Value), out temp);
                settings.PackageType = (Hotcakes.Shipping.USPostal.v4.DomesticPackageType)temp;
                return settings;
            }
            catch (Exception)
            {
                return new USPostalServiceSettings();
            }
        }

        /// <summary>
        /// Gets the test rate ups internation shipment.
        /// </summary>
        /// <returns></returns>
        public Shipment GetTestRate_UPS_InternationShipment()
        {
            var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("UPS-InternationalTestRates").FirstOrDefault();
            if (element == null) return new Shipment();
            var shipment = new Hotcakes.Shipping.Shipment
            {
                DestinationAddress = { CountryBvin = Convert.ToString(element.Element("Country").Value) },
                SourceAddress = { PostalCode = Convert.ToString(element.Element("Zip").Value) }
            };
            return shipment;
        }
        /// <summary>
        /// Gets the test rate ups internation shippable information.
        /// </summary>
        /// <returns></returns>
        public Shippable GetTestRate_UPS_InternationShippableInfo()
        {
            var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("UPS-InternationalTestRates").FirstOrDefault();
            if (element == null) return new Shippable();

            var item = new Hotcakes.Shipping.Shippable
            {
                BoxLength = element.Element("Length") == null ? 0 : Convert.ToDecimal(element.Element("Length").Value),
                BoxHeight = element.Element("Height") == null ? 0 : Convert.ToDecimal(element.Element("Height").Value),
                BoxWidth = element.Element("Width") == null ? 0 : Convert.ToDecimal(element.Element("Width").Value),
                BoxLengthType = Hotcakes.Shipping.LengthType.Inches,
                BoxWeight = element.Element("Weight") == null ? 0 : Convert.ToDecimal(element.Element("Weight").Value),
                BoxWeightType = Hotcakes.Shipping.WeightType.Pounds,
                QuantityOfItemsInBox = 1
            };

            return item;
        }
        /// <summary>
        /// Gets the test rate ups internation service setting.
        /// </summary>
        /// <returns></returns>
        public USPostalServiceSettings GetTestRate_UPS_InternationServiceSetting()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").Elements("UPS-InternationalTestRates").FirstOrDefault();
                if (element == null) return new USPostalServiceSettings();

                var settings = new USPostalServiceSettings();
                var code = new Hotcakes.Shipping.ServiceCode
                {
                    Code = Convert.ToString(element.Element("ServiceCodeFilter").Value),
                    DisplayName = Convert.ToString(element.Element("ServiceCodeFilter").Value)
                };
                var codes = new List<Hotcakes.Shipping.IServiceCode> { code };
                settings.ServiceCodeFilter = codes;

                return settings;
            }
            catch (Exception)
            {
                return new USPostalServiceSettings();
            }
        }


        #endregion


        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public int GetUpsServiceCodeCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step3").Elements("ShippinMethods").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("ServiceCodeCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("ServiceCodeCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        #endregion

        #region Step4

        /// <summary>
        /// Gets the store tax schedule count.
        /// </summary>
        /// <returns></returns>
        public int GetStoreTaxScheduleCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step4").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("StoreTaxScheduleCount") == null
                           ? 0
                           : Convert.ToInt32(element.Element("StoreTaxScheduleCount").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the delete tax schedule.
        /// </summary>
        /// <returns></returns>
        public string GetDeleteTaxSchedule()
        {
            try
            {
                var element = _xmldoc.Elements("Step4").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("DeleteTaxScheduleName") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("DeleteTaxScheduleName").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Ges the tax schedule apply vat rules.
        /// </summary>
        /// <returns></returns>
        public bool GeTaxScheduleApplyVatRules()
        {
            try
            {
                var element = _xmldoc.Elements("Step4").FirstOrDefault();
                if (element == null) return true;
                return element.Element("ApplyVATRules") == null || Convert.ToBoolean(element.Element("ApplyVATRules").Value);
            }
            catch (Exception)
            {
                return true;
            }
        }

        /// <summary>
        /// Gets the edit tax schedule.
        /// </summary>
        /// <returns></returns>
        public TaxSchedule GetEditTaxSchedule()
        {
            try
            {
                var taxschedule = _xmldoc.Elements("Step4").Elements("TaxSchedule").Elements("Edit").Select(x => new TaxSchedule
                       {
                           Name = x.Element("Name") == null ? string.Empty : Convert.ToString(x.Element("Name").Value),
                           DefaultRate = x.Element("DefaultRate") == null ? 0 : Convert.ToDecimal(x.Element("DefaultRate").Value),
                           DefaultShippingRate = x.Element("DefaultShippingRate") == null ? 0 : Convert.ToDecimal(x.Element("DefaultShippingRate").Value),

                       }).FirstOrDefault();

                return taxschedule ?? new TaxSchedule();
            }
            catch (Exception)
            {
                return new TaxSchedule();
            }

        }

        /// <summary>
        /// Gets the add tax schedule.
        /// </summary>
        /// <returns></returns>
        public TaxSchedule GetAddTaxSchedule()
        {
            try
            {
                var taxschedule = _xmldoc.Elements("Step4").Elements("TaxSchedule").Elements("Add").Select(x => new TaxSchedule
                {
                    Name = x.Element("Name") == null ? string.Empty : Convert.ToString(x.Element("Name").Value),
                    DefaultRate = x.Element("DefaultRate") == null ? 0 : Convert.ToDecimal(x.Element("DefaultRate").Value),
                    DefaultShippingRate = x.Element("DefaultShippingRate") == null ? 0 : Convert.ToDecimal(x.Element("DefaultShippingRate").Value),

                }).FirstOrDefault();

                return taxschedule ?? new TaxSchedule();
            }
            catch (Exception)
            {
                return new TaxSchedule();
            }

        }

        /// <summary>
        /// Gets the add tax list.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Tax> GetAddTaxList()
        {
            try
            {
                var element = _xmldoc.Elements("Step4").Elements("AddTax").FirstOrDefault();
                if (element == null) return new Dictionary<string, Tax>();
                if (string.IsNullOrEmpty(Convert.ToString(element.Element("TaxScheduleName").Value))) return new Dictionary<string, Tax>();

                var tax = new Dictionary<string, Tax>
                {
                    {Convert.ToString(element.Element("TaxScheduleName").Value),new Tax
                        {
                            RegionAbbreviation=element.Element("RegionAbbreviation") == null ? string.Empty : Convert.ToString(element.Element("RegionAbbreviation").Value),
                            ShippingRate=element.Element("ShippingRate") == null ? 0 : Convert.ToDecimal(element.Element("ShippingRate").Value),
                            Rate=element.Element("Rate") == null ? 0 : Convert.ToDecimal(element.Element("Rate").Value),
                            PostalCode=element.Element("PostalCode") == null ? string.Empty : Convert.ToString(element.Element("PostalCode").Value),
                            ApplyToShipping=element.Element("ApplyToShipping") == null || Convert.ToBoolean( element.Element("ApplyToShipping").Value),
                            CountryIsoAlpha3=element.Element("CountryIsoAlpha3") == null ? string.Empty : Convert.ToString(element.Element("CountryIsoAlpha3").Value),
                        }},
                
                };

                return tax ?? new Dictionary<string, Tax>();
            }
            catch (Exception)
            {
                return new Dictionary<string, Tax>();
            }
        }

        /// <summary>
        /// Gets the add tax.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Tax> GetAddTax()
        {
            return GetTax(_xmldoc.Elements("Step4").Elements("AddTax").FirstOrDefault());
        }

        /// <summary>
        /// Gets the delete tax.
        /// </summary>
        /// <returns></returns>
        public Dictionary<string, Tax> GetDeleteTax()
        {
            return GetTax(_xmldoc.Elements("Step4").Elements("DeleteTax").FirstOrDefault());
        }

        /// <summary>
        /// Gets the tax.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private Dictionary<string, Tax> GetTax(XElement element)
        {
            try
            {
                if (element == null) return new Dictionary<string, Tax>();
                if (string.IsNullOrEmpty(Convert.ToString(element.Element("TaxScheduleName").Value))) return new Dictionary<string, Tax>();

                var tax = new Dictionary<string, Tax>
                {
                    {Convert.ToString(element.Element("TaxScheduleName").Value),new Tax
                        {
                            RegionAbbreviation=element.Element("RegionAbbreviation") == null ? string.Empty : Convert.ToString(element.Element("RegionAbbreviation").Value),
                            ShippingRate=element.Element("ShippingRate") == null ? 0 : Convert.ToDecimal(element.Element("ShippingRate").Value),
                            Rate=element.Element("Rate") == null ? 0 : Convert.ToDecimal(element.Element("Rate").Value),
                            PostalCode=element.Element("PostalCode") == null ? string.Empty : Convert.ToString(element.Element("PostalCode").Value),
                            ApplyToShipping=element.Element("ApplyToShipping") == null || Convert.ToBoolean( element.Element("ApplyToShipping").Value),
                            CountryIsoAlpha3=element.Element("CountryIsoAlpha3") == null ? string.Empty : Convert.ToString(element.Element("CountryIsoAlpha3").Value),
                        }},
                
                };

                return tax ?? new Dictionary<string, Tax>();
            }
            catch (Exception)
            {
                return new Dictionary<string, Tax>();
            }
        }
        #endregion

        #region Step5
        /// <summary>
        /// Gets the login information.
        /// </summary>
        /// <returns></returns>
        public CredentialsDTO GetLoginInfo()
        {
            return GetLoginInfo(_xmldoc.Elements("Step5").Elements("Login").FirstOrDefault());
        }

        /// <summary>
        /// Gets the register information.
        /// </summary>
        /// <returns></returns>
        public UserDTO GetRegisterInfo()
        {
            try
            {
                var logininfo = _xmldoc.Elements("Step5").Elements("Register").Select(x => new UserDTO
                {
                    FirstName = x.Element("FirstName") == null ? string.Empty : Convert.ToString(x.Element("FirstName")),
                    LastName = x.Element("LastName") == null ? string.Empty : Convert.ToString(x.Element("LastName")),
                    Username = x.Element("Username") == null ? string.Empty : Convert.ToString(x.Element("Username")),
                    Password = x.Element("Password") == null ? string.Empty : Convert.ToString(x.Element("Password")),
                    Email = x.Element("Email") == null ? string.Empty : Convert.ToString(x.Element("Email")),

                }).FirstOrDefault();

                return logininfo ?? new UserDTO();
            }
            catch (Exception)
            {
                return new UserDTO();
            }
        }

        /// <summary>
        /// Gets the license order count.
        /// </summary>
        /// <returns></returns>
        public int GetLicenseOrderCount()
        {
            try
            {
                var element = _xmldoc.Elements("Step5").Elements("LicenseOrder").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("Count") == null
                           ? 0
                           : Convert.ToInt32(element.Element("Count").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }


        /// <summary>
        /// Gets the license login information.
        /// </summary>
        /// <returns></returns>
        public CredentialsDTO GetLicenseLoginInfo()
        {
            return GetLoginInfo(_xmldoc.Elements("Step5").Elements("LicenseOrder").Elements("Login").FirstOrDefault());
        }

        /// <summary>
        /// Gets the login information.
        /// </summary>
        /// <param name="element">The element.</param>
        /// <returns></returns>
        private CredentialsDTO GetLoginInfo(XElement element)
        {
            try
            {
                var logininfo = new CredentialsDTO
                    {
                        Username =
                            element.Element("Username") == null ? string.Empty : Convert.ToString(element.Element("Username")),
                        Password =
                            element.Element("Password") == null ? string.Empty : Convert.ToString(element.Element("Password")),
                    };

                return logininfo ?? new CredentialsDTO();
            }
            catch (Exception)
            {
                return new CredentialsDTO();
            }
        }

        /// <summary>
        /// Gets the license order identifier.
        /// </summary>
        /// <returns></returns>
        public int GetLicenseOrderId()
        {
            try
            {
                var element = _xmldoc.Elements("Step5").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("LicenseOrderId") == null
                           ? 0
                           : Convert.ToInt32(element.Element("LicenseOrderId").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }

        /// <summary>
        /// Gets the order lookup query.
        /// </summary>
        /// <returns></returns>
        public OrderQueryDTO GetOrderLookupQuery()
        {
            try
            {
                var oq = _xmldoc.Element("Step5").Elements("LicenseLookup").Select(el => new OrderQueryDTO
                {
                    LicenseKey = Convert.ToString(el.Element("LicenseKey").Value),
                    InvoiceId = Convert.ToString(el.Element("InvoiceId").Value),
                    Email = Convert.ToString(el.Element("Email").Value),

                }).FirstOrDefault();
                return oq ?? new OrderQueryDTO();
            }
            catch (Exception)
            {
                return new OrderQueryDTO();
            }
        }


        /// <summary>
        /// Gets the order lookup MSG.
        /// </summary>
        /// <returns></returns>
        public string GetOrderLookupMsg()
        {
            try
            {
                var element = _xmldoc.Elements("Step5").Elements("LicenseLookup").FirstOrDefault();
                if (element == null) return string.Empty;
                return element.Element("Msg") == null
                           ? string.Empty
                           : Convert.ToString(element.Element("Msg").Value);
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Gets the order store.
        /// </summary>
        /// <returns></returns>
        public int GetOrderStore()
        {
            try
            {
                var element = _xmldoc.Elements("Step5").Elements("LicenseLookup").FirstOrDefault();
                if (element == null) return 0;
                return element.Element("OrderStore") == null
                           ? 0
                           : Convert.ToInt32(element.Element("OrderStore").Value);
            }
            catch (Exception)
            {
                return 0;
            }
        }
        #endregion


        #region Dispose Object
        /// <summary>
        /// The disposed
        /// </summary>
        private bool _disposed = false;

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (!this._disposed)
            {
                if (disposing)
                {
                    _xmldoc = null;
                }
            }
            this._disposed = true;
        }

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion

    }
}
