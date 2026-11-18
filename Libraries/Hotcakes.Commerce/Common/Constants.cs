#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020 UpendoVentures, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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

namespace Hotcakes.Commerce.Common
{
    /// <summary>
    /// Provides the compatibility-sensitive keys and literal values used by Hotcakes Commerce.
    /// </summary>
    public class Constants
    {
        #region Common
        /// <summary>
        /// The comma delimiter used by common formatting and serialization code.
        /// </summary>
        public const string COMMA = ",";
        /// <summary>
        /// The short key used to identify Hotcakes Commerce data.
        /// </summary>
        public const string HCC_KEY = "hcc";
        #endregion

        #region Store Settings
        /// <summary>
        /// Compatibility-sensitive store setting key for applyvatrules.
        /// </summary>
        public const string STORESETTING_APPLYVATRULES = "ApplyVATRules";
        /// <summary>
        /// Compatibility-sensitive store setting key for preservecartsession.
        /// </summary>
        public const string STORESETTING_PRESERVECARTSESSION = "PreserveCartInSession";
        /// <summary>
        /// Compatibility-sensitive store setting key for sendabandonedemails.
        /// </summary>
        public const string STORESETTING_SENDABANDONEDEMAILS = "SendAbandonedCartEmails";
        /// <summary>
        /// Compatibility-sensitive store setting key for sendabandonedinterval.
        /// </summary>
        public const string STORESETTING_SENDABANDONEDINTERVAL = "SendAbandonedEmailIn";
        /// <summary>
        /// Compatibility-sensitive store setting key for allowapiclearperiod.
        /// </summary>
        public const string STORESETTING_ALLOWAPICLEARPERIOD = "AllowApiToClearUntil";
        /// <summary>
        /// Compatibility-sensitive store setting key for logoimage.
        /// </summary>
        public const string STORESETTING_LOGOIMAGE = "LogoImage";
        /// <summary>
        /// Compatibility-sensitive store setting key for uselogoimage.
        /// </summary>
        public const string STORESETTING_USELOGOIMAGE = "UseLogoImage";
        /// <summary>
        /// Compatibility-sensitive store setting key for logotext.
        /// </summary>
        public const string STORESETTING_LOGOTEXT = "LogoText";
        /// <summary>
        /// Compatibility-sensitive store setting key for minimumorderamount.
        /// </summary>
        public const string STORESETTING_MINIMUMORDERAMOUNT = "MinimumOrderAmount";
        /// <summary>
        /// Compatibility-sensitive store setting key for friendlyname.
        /// </summary>
        public const string STORESETTING_FRIENDLYNAME = "FriendlyName";
        /// <summary>
        /// Compatibility-sensitive store setting key for logorevision.
        /// </summary>
        public const string STORESETTING_LOGOREVISION = "LogoRevision";
        /// <summary>
        /// Compatibility-sensitive store setting key for swatchedenabled.
        /// </summary>
        public const string STORESETTING_SWATCHEDENABLED = "ProductEnableSwatches";
        /// <summary>
        /// Compatibility-sensitive store setting key for metricsrecordsearches.
        /// </summary>
        public const string STORESETTING_METRICSRECORDSEARCHES = "MetricsRecordSearches";
        /// <summary>
        /// Compatibility-sensitive store setting key for requirephonenumber.
        /// </summary>
        public const string STORESETTING_REQUIREPHONENUMBER = "RequirePhoneNumber";
        /// <summary>
        /// Compatibility-sensitive store setting key for countryiso3codes.
        /// </summary>
        public const string STORESETTING_COUNTRYISO3CODES = "DisabledCountryIso3Codes";

        /// <summary>
        /// Compatibility-sensitive store setting key for quickbooksorderaccount.
        /// </summary>
        public const string STORESETTING_QUICKBOOKSORDERACCOUNT = "QuickbooksOrderAccount";

        /// <summary>
        /// Compatibility-sensitive store setting key for quickbooksshippingaccount.
        /// </summary>
        public const string STORESETTING_QUICKBOOKSSHIPPINGACCOUNT = "QuickbooksShippingAccount";
        /// <summary>
        /// Compatibility-sensitive store setting key for forceadminssl.
        /// </summary>
        public const string STORESETTING_FORCEADMINSSL = "ForceAdminSSL";

        /// <summary>
        /// Compatibility-sensitive store setting key for timezone.
        /// </summary>
        public const string STORESETTING_TIMEZONE = "TimeZone";
        /// <summary>
        /// Default literal value associated with the <see cref="STORESETTING_TIMEZONE"/> store setting.
        /// </summary>
        public const string STORESETTING_TIMEZONEDEFAULT = "Eastern Standard Time";
        /// <summary>
        /// Compatibility-sensitive store setting key for culturecode.
        /// </summary>
        public const string STORESETTING_CULTURECODE = "CultureCode";
        /// <summary>
        /// Default literal value associated with the <see cref="STORESETTING_CULTURECODE"/> store setting.
        /// </summary>
        public const string STORESETTING_CULTUREDEFAULT = "en-US";

        /// <summary>
        /// Compatibility-sensitive store setting key for issuepointsforuserprice.
        /// </summary>
        public const string STORESETTING_ISSUEPOINTSFORUSERPRICE = "IssuePointsForUserPrice";
        /// <summary>
        /// Compatibility-sensitive store setting key for userewardpointsforuserprice.
        /// </summary>
        public const string STORESETTING_USEREWARDPOINTSFORUSERPRICE = "UseRewardsPointsForUserPrice";
        /// <summary>
        /// Compatibility-sensitive store setting key for rewardpointsonpurchase.
        /// </summary>
        public const string STORESETTING_REWARDPOINTSONPURCHASE = "RewardsPointsOnPurchasesActive";
        /// <summary>
        /// Compatibility-sensitive store setting key for rewardpointsonproducts.
        /// </summary>
        public const string STORESETTING_REWARDPOINTSONPRODUCTS = "RewardsPointsOnProductsActive";
        /// <summary>
        /// Compatibility-sensitive store setting key for rewardpointsperdollarspent.
        /// </summary>
        public const string STORESETTING_REWARDPOINTSPERDOLLARSPENT = "RewardsPointsIssuedPerDollarSpent";
        /// <summary>
        /// Compatibility-sensitive store setting key for rewardpointsperdollarcredit.
        /// </summary>
        public const string STORESETTING_REWARDPOINTSPERDOLLARCREDIT = "RewardsPointsNeededPerDollarCredit";
        /// <summary>
        /// Compatibility-sensitive store setting key for rewardpointsname.
        /// </summary>
        public const string STORESETTING_REWARDPOINTSNAME = "RewardsPointsName";
        /// <summary>
        /// Default literal value associated with the <see cref="STORESETTING_REWARDPOINTSNAME"/> store setting.
        /// </summary>
        public const string STORESETTING_REWARDPOINTSNAMEDEFAULT = "Reward Points";

        /// <summary>
        /// Compatibility-sensitive store setting key for forcetermsagreement.
        /// </summary>
        public const string STORESETTING_FORCETERMSAGREEMENT = "ForceTermsAgreement";
        /// <summary>
        /// Compatibility-sensitive store setting key for maxitemsperorder.
        /// </summary>
        public const string STORESETTING_MAXITEMSPERORDER = "MaxItemsPerOrder";
        /// <summary>
        /// Compatibility-sensitive store setting key for maxweightperorder.
        /// </summary>
        public const string STORESETTING_MAXWEIGHTPERORDER = "MaxWeightPerOrder";
        /// <summary>
        /// Compatibility-sensitive store setting key for allowzerodollarorders.
        /// </summary>
        public const string STORESETTING_ALLOWZERODOLLARORDERS = "AllowZeroDollarOrders";
        /// <summary>
        /// Compatibility-sensitive store setting key for autoissuermanumbers.
        /// </summary>
        public const string STORESETTING_AUTOISSUERMANUMBERS = "AutomaticallyIssueRMANumbers";
        /// <summary>
        /// Compatibility-sensitive store setting key for childchoicesonbundles.
        /// </summary>
        public const string STORESETTING_CHILDCHOICESONBUNDLES = "UseChildChoicesAdjustmentsForBundles";
        /// <summary>
        /// Compatibility-sensitive store setting key for handlingamount.
        /// </summary>
        public const string STORESETTING_HANDLINGAMOUNT = "HandlingAmount";
        /// <summary>
        /// Compatibility-sensitive store setting key for handlingnonshipping.
        /// </summary>
        public const string STORESETTING_HANDLINGNONSHIPPING = "HandlingNonShipping";
        /// <summary>
        /// Compatibility-sensitive store setting key for handlingtype.
        /// </summary>
        public const string STORESETTING_HANDLINGTYPE = "HandlingType";

        /// <summary>
        /// Compatibility-sensitive store setting key for taxprovider.
        /// </summary>
        public const string STORESETTING_TAXPROVIDER = "TaxProvidersEnabled";
        /// <summary>
        /// Compatibility-sensitive store setting key for taxprovidersetting.
        /// </summary>
        public const string STORESETTING_TAXPROVIDERSETTING = "taxprovsetting";

        /// <summary>
        /// Compatibility-sensitive store setting key for paymentmethods.
        /// </summary>
        public const string STORESETTING_PAYMENTMETHODS = "PaymentMethodsEnabled";
        /// <summary>
        /// Compatibility-sensitive store setting key for creditauthorizeonly.
        /// </summary>
        public const string STORESETTING_CREDITAUTHORIZEONLY = "PaymentCreditCardAuthorizeOnly";
        /// <summary>
        /// Compatibility-sensitive store setting key for creditrequirecvv.
        /// </summary>
        public const string STORESETTING_CREDITREQUIRECVV = "PaymentCreditCardRequireCVV";
        /// <summary>
        /// Compatibility-sensitive store setting key for creditgateway.
        /// </summary>
        public const string STORESETTING_CREDITGATEWAY = "PaymentCreditCardGateway";
        /// <summary>
        /// Compatibility-sensitive store setting key for recurringgateway.
        /// </summary>
        public const string STORESETTING_RECURRINGGATEWAY = "PaymentReccuringGateway";
        /// <summary>
        /// Compatibility-sensitive store setting key for acceptedcards.
        /// </summary>
        public const string STORESETTING_ACCEPTEDCARDS = "PaymentAcceptedCards";
        /// <summary>
        /// Compatibility-sensitive store setting key for giftcardgateway.
        /// </summary>
        public const string STORESETTING_GIFTCARDGATEWAY = "GiftCardGateway";
        /// <summary>
        /// Compatibility-sensitive store setting key for giftcardauthorizeonly.
        /// </summary>
        public const string STORESETTING_GIFTCARDAUTHORIZEONLY = "PaymentGiftCardAuthorizeOnly";
        /// <summary>
        /// Compatibility-sensitive store setting key for displayfullcardnumber.
        /// </summary>
        public const string STORESETTING_DISPLAYFULLCARDNUMBER = "PaymentDisplayFullCreditCardNumbers";
        /// <summary>
        /// Compatibility-sensitive store setting key for paymentmethodsettings.
        /// </summary>
        public const string STORESETTING_PAYMENTMETHODSETTINGS = "methodsettings";
        /// <summary>
        /// Compatibility-sensitive store setting key for paymentgatewaysettings.
        /// </summary>
        public const string STORESETTING_PAYMENTGATEWAYSETTINGS = "paysettings";
        /// <summary>
        /// Compatibility-sensitive store setting key for paymentgiftcardsettings.
        /// </summary>
        public const string STORESETTING_PAYMENTGIFTCARDSETTINGS = "gcpaysettings";

        /// <summary>
        /// Compatibility-sensitive store setting key for reviewcount.
        /// </summary>
        public const string STORESETTING_REVIEWCOUNT = "ProductReviewCount";
        /// <summary>
        /// Compatibility-sensitive store setting key for reviewmoderate.
        /// </summary>
        public const string STORESETTING_REVIEWMODERATE = "ProductReviewModerate";
        /// <summary>
        /// Compatibility-sensitive store setting key for allowreviews.
        /// </summary>
        public const string STORESETTING_ALLOWREVIEWS = "AllowProductReviews";

        /// <summary>
        /// Compatibility-sensitive store setting key for fedexkey.
        /// </summary>
        public const string STORESETTING_FEDEXKEY = "ShippingFedExKey";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexpassword.
        /// </summary>
        public const string STORESETTING_FEDEXPASSWORD = "ShippingFedExPassword";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexaccount.
        /// </summary>
        public const string STORESETTING_FEDEXACCOUNT = "ShippingFedExAccountNumber";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexdiagnostics.
        /// </summary>
        public const string STORESETTING_FEDEXDIAGNOSTICS = "ShippingFedExDiagnostics";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexdefaultpackaging.
        /// </summary>
        public const string STORESETTING_FEDEXDEFAULTPACKAGING = "ShippingFedExDefaultPackaging";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexdropofftype.
        /// </summary>
        public const string STORESETTING_FEDEXDROPOFFTYPE = "ShippingFedExDropOffType";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexforceresidential.
        /// </summary>
        public const string STORESETTING_FEDEXFORCERESIDENTIAL = "ShippingFedExForceResidentialRates";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexmeternumber.
        /// </summary>
        public const string STORESETTING_FEDEXMETERNUMBER = "ShippingFedExMeterNumber";
        /// <summary>
        /// Compatibility-sensitive store setting key for fedexdevurl.
        /// </summary>
        public const string STORESETTING_FEDEXDEVURL = "ShippingFedExUseDevelopmentServiceUrl";

        /// <summary>
        /// Compatibility-sensitive store setting key for upsaccount.
        /// </summary>
        public const string STORESETTING_UPSACCOUNT = "ShippingUpsAccountNumber";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsdefaultpackaging.
        /// </summary>
        public const string STORESETTING_UPSDEFAULTPACKAGING = "ShippingUpsDefaultPackaging";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsdefaultpayment.
        /// </summary>
        public const string STORESETTING_UPSDEFAULTPAYMENT = "ShippingUpsDefaultPayment";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsdefaultservice.
        /// </summary>
        public const string STORESETTING_UPSDEFAULTSERVICE = "ShippingUpsDefaultService";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsdiagnositcs.
        /// </summary>
        public const string STORESETTING_UPSDIAGNOSITCS = "ShippingUPSDiagnostics";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsforceresidential.
        /// </summary>
        public const string STORESETTING_UPSFORCERESIDENTIAL = "ShippingUpsForceResidential";
        /// <summary>
        /// Compatibility-sensitive store setting key for upspickuptype.
        /// </summary>
        public const string STORESETTING_UPSPICKUPTYPE = "Shipping_UPS_Pickup_Type";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsshipdimensions.
        /// </summary>
        public const string STORESETTING_UPSSHIPDIMENSIONS = "ShippingUpsSkipDimensions";
        /// <summary>
        /// Compatibility-sensitive store setting key for upswritexml.
        /// </summary>
        public const string STORESETTING_UPSWRITEXML = "ShippingUPSWriteXML";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsclientid.
        /// </summary>
        public const string STORESETTING_UPSCLIENTID = "Shipping_UPS_ClientId";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsclientsecret.
        /// </summary>
        public const string STORESETTING_UPSCLIENTSECRET = "Shipping_UPS_ClientSecret";
        /// <summary>
        /// Compatibility-sensitive store setting key for upstestingmode.
        /// </summary>
        public const string STORESETTING_UPSTESTINGMODE = "Shipping_UPS_Testing";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsusername.
        /// </summary>
        public const string STORESETTING_UPSUSERNAME = "Shipping_UPS_Username";
        /// <summary>
        /// Compatibility-sensitive store setting key for upslicense.
        /// </summary>
        public const string STORESETTING_UPSLICENSE = "Shipping_UPS_License";
        /// <summary>
        /// Compatibility-sensitive store setting key for upspassword.
        /// </summary>
        public const string STORESETTING_UPSPASSWORD = "Shipping_UPS_Password";

        /// <summary>
        /// Compatibility-sensitive store setting key for upsfdefaultpackaging.
        /// </summary>
        public const string STORESETTING_UPSFDEFAULTPACKAGING = "ShippingUpsFreightDefaultPackaging";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsfdefaultpayment.
        /// </summary>
        public const string STORESETTING_UPSFDEFAULTPAYMENT = "ShippingUpsFreightDefaultPayment";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsfdiagnostics.
        /// </summary>
        public const string STORESETTING_UPSFDIAGNOSTICS = "ShippingUPSFreightDiagnostics";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsfforceresidential.
        /// </summary>
        public const string STORESETTING_UPSFFORCERESIDENTIAL = "ShippingUpsFreightForceResidential";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsfskipdimensions.
        /// </summary>
        public const string STORESETTING_UPSFSKIPDIMENSIONS = "ShippingUpsFreightSkipDimensions";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsfbillingoption.
        /// </summary>
        public const string STORESETTING_UPSFBILLINGOPTION = "ShippingUpsFreightBillingOption";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsfhandleoneunittype.
        /// </summary>
        public const string STORESETTING_UPSFHANDLEONEUNITTYPE = "ShippingUpsFreightHandleOneUnitType";
        /// <summary>
        /// Compatibility-sensitive store setting key for upsffreightclass.
        /// </summary>
        public const string STORESETTING_UPSFFREIGHTCLASS = "ShippingUpsFreightFreightClass";

        /// <summary>
        /// Compatibility-sensitive store setting key for uspsuserid.
        /// </summary>
        public const string STORESETTING_USPSUSERID = "ShippingUSPostalUserId";
        /// <summary>
        /// Compatibility-sensitive store setting key for uspsdiagnositcs.
        /// </summary>
        public const string STORESETTING_USPSDIAGNOSITCS = "ShippingUSPostalDiagnostics";

        /// <summary>
        /// Compatibility-sensitive store setting key for affiliatecommissionamount.
        /// </summary>
        public const string STORESETTING_AFFILIATECOMMISSIONAMOUNT = "AffiliateCommissionAmount";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliatecommissiontype.
        /// </summary>
        public const string STORESETTING_AFFILIATECOMMISSIONTYPE = "AffiliateCommissionType";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliateconflictmode.
        /// </summary>
        public const string STORESETTING_AFFILIATECONFLICTMODE = "AffiliateConflictMode";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliatereferraldays.
        /// </summary>
        public const string STORESETTING_AFFILIATEREFERRALDAYS = "AffiliateReferralDays";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliaterequireapproval.
        /// </summary>
        public const string STORESETTING_AFFILIATEREQUIREAPPROVAL = "AffiliateRequireApproval";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliatedisplaychildren.
        /// </summary>
        public const string STORESETTING_AFFILIATEDISPLAYCHILDREN = "AffiliateDisplayChildren";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliateshowidcheckout.
        /// </summary>
        public const string STORESETTING_AFFILIATESHOWIDCHECKOUT = "AffiliateShowIDOnCheckout";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliateagreementtext.
        /// </summary>
        public const string STORESETTING_AFFILIATEAGREEMENTTEXT = "AffiliatAgreementText";
        /// <summary>
        /// Compatibility-sensitive store setting key for affiliatereview.
        /// </summary>
        public const string STORESETTING_AFFILIATEREVIEW = "AffiliateReview";

        /// <summary>
        /// Compatibility-sensitive store setting key for storeclosed.
        /// </summary>
        public const string STORESETTING_STORECLOSED = "StoreClosed";
        /// <summary>
        /// Compatibility-sensitive store setting key for storecloseddesc.
        /// </summary>
        public const string STORESETTING_STORECLOSEDDESC = "StoreClosedDescription";
        /// <summary>
        /// Compatibility-sensitive store setting key for aesinitvector.
        /// </summary>
        public const string STORESETTING_AESINITVECTOR = "AESInitVector";
        /// <summary>
        /// Compatibility-sensitive store setting key for aeskey.
        /// </summary>
        public const string STORESETTING_AESKEY = "AESKey";

        /// <summary>
        /// Store setting key or default value identified by <see cref="ENABLED_FAILED_PAYMENT_NOTIFICATION"/>.
        /// </summary>
        public const string ENABLED_FAILED_PAYMENT_NOTIFICATION = "EnableFailedPaymentNotification";
        #endregion

        #region Views
        /// <summary>
        /// HTML metadata tag format string for the canonical value.
        /// </summary>
        public const string TAG_CANONICAL = "<link rel=\"canonical\" href=\"{0}\" />";
        /// <summary>
        /// HTML metadata tag format string for the ogtitle value.
        /// </summary>
        public const string TAG_OGTITLE = "<meta property=\"og:title\" content=\"{0}\"/>";
        /// <summary>
        /// HTML metadata tag format string for the ogtype value.
        /// </summary>
        public const string TAG_OGTYPE = "<meta property=\"og:type\" content=\"product\"/>";
        /// <summary>
        /// HTML metadata tag format string for the ogurl value.
        /// </summary>
        public const string TAG_OGURL = "<meta property=\"og:url\" content=\"{0}\"/>";
        /// <summary>
        /// HTML metadata tag format string for the ogimage value.
        /// </summary>
        public const string TAG_OGIMAGE = "<meta property=\"og:image\" content=\"{0}\"/>";
        /// <summary>
        /// HTML metadata tag format string for the ogsitename value.
        /// </summary>
        public const string TAG_OGSITENAME = "<meta property=\"og:site_name\" content=\"{0}\" />";
        /// <summary>
        /// HTML metadata tag format string for the ogfbadmin value.
        /// </summary>
        public const string TAG_OGFBADMIN = "<meta property=\"fb:admins\" content=\"{0}\" />";
        /// <summary>
        /// HTML metadata tag format string for the ogfbappid value.
        /// </summary>
        public const string TAG_OGFBAPPID = "<meta property=\"fb:app_id\" content=\"{0}\" />";
        /// <summary>
        /// HTML metadata tag format string for the iogpriceamount value.
        /// </summary>
        public const string TAG_IOGPRICEAMOUNT = "<meta property=\"og:price:amount\" content=\"{0}\" />";
        /// <summary>
        /// HTML metadata tag format string for the iogpricecurrency value.
        /// </summary>
        public const string TAG_IOGPRICECURRENCY = "<meta property=\"og:price:currency\" content=\"{0}\" />";
        #endregion

        #region AllowUpcharge

        /// <summary>
        /// The fixed amount used when an upcharge is allowed.
        /// </summary>
        public const decimal UpchargeAmount = 3m;
        /// <summary>
        /// Indicates whether the fixed upcharge behavior is enabled.
        /// </summary>
        public const bool AllowUpcharge = false;

        #endregion
    }
}

