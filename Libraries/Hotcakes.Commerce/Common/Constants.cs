#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2020 UpendoVentures, LLC
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
    public class Constants
    {
        #region Common
        public const string COMMA = ",";
        #endregion

        #region Store Settings
        public const string STORESETTING_APPLYVATRULES = "ApplyVATRules";
        public const string STORESETTING_PRESERVECARTSESSION = "PreserveCartInSession";
        public const string STORESETTING_SENDABANDONEDEMAILS = "SendAbandonedCartEmails";
        public const string STORESETTING_SENDABANDONEDINTERVAL = "SendAbandonedEmailIn";
        public const string STORESETTING_ALLOWAPICLEARPERIOD = "AllowApiToClearUntil";
        public const string STORESETTING_LOGOIMAGE = "LogoImage";
        public const string STORESETTING_USELOGOIMAGE = "UseLogoImage";
        public const string STORESETTING_LOGOTEXT = "LogoText";
        public const string STORESETTING_MINIMUMORDERAMOUNT = "MinimumOrderAmount";
        public const string STORESETTING_FRIENDLYNAME = "FriendlyName";
        public const string STORESETTING_LOGOREVISION = "LogoRevision";
        public const string STORESETTING_SWATCHEDENABLED = "ProductEnableSwatches";
        public const string STORESETTING_METRICSRECORDSEARCHES = "MetricsRecordSearches";
        public const string STORESETTING_REQUIREPHONENUMBER = "RequirePhoneNumber";
        public const string STORESETTING_COUNTRYISO3CODES = "DisabledCountryIso3Codes";

        public const string STORESETTING_QUICKBOOKSORDERACCOUNT = "QuickbooksOrderAccount";

        public const string STORESETTING_QUICKBOOKSSHIPPINGACCOUNT = "QuickbooksShippingAccount";
        public const string STORESETTING_FORCEADMINSSL = "ForceAdminSSL";

        public const string STORESETTING_TIMEZONE = "TimeZone";
        public const string STORESETTING_TIMEZONEDEFAULT = "Eastern Standard Time";
        public const string STORESETTING_CULTURECODE = "CultureCode";
        public const string STORESETTING_CULTUREDEFAULT = "en-US";

        public const string STORESETTING_ISSUEPOINTSFORUSERPRICE = "IssuePointsForUserPrice";
        public const string STORESETTING_USEREWARDPOINTSFORUSERPRICE = "UseRewardsPointsForUserPrice";
        public const string STORESETTING_REWARDPOINTSONPURCHASE = "RewardsPointsOnPurchasesActive";
        public const string STORESETTING_REWARDPOINTSONPRODUCTS = "RewardsPointsOnProductsActive";
        public const string STORESETTING_REWARDPOINTSPERDOLLARSPENT = "RewardsPointsIssuedPerDollarSpent";
        public const string STORESETTING_REWARDPOINTSPERDOLLARCREDIT = "RewardsPointsNeededPerDollarCredit";
        public const string STORESETTING_REWARDPOINTSNAME = "RewardsPointsName";
        public const string STORESETTING_REWARDPOINTSNAMEDEFAULT = "Reward Points";

        public const string STORESETTING_FORCETERMSAGREEMENT = "ForceTermsAgreement";
        public const string STORESETTING_MAXITEMSPERORDER = "MaxItemsPerOrder";
        public const string STORESETTING_MAXWEIGHTPERORDER = "MaxWeightPerOrder";
        public const string STORESETTING_ALLOWZERODOLLARORDERS = "AllowZeroDollarOrders";
        public const string STORESETTING_AUTOISSUERMANUMBERS = "AutomaticallyIssueRMANumbers";
        public const string STORESETTING_CHILDCHOICESONBUNDLES = "UseChildChoicesAdjustmentsForBundles";
        public const string STORESETTING_HANDLINGAMOUNT = "HandlingAmount";
        public const string STORESETTING_HANDLINGNONSHIPPING = "HandlingNonShipping";
        public const string STORESETTING_HANDLINGTYPE = "HandlingType";

        public const string STORESETTING_TAXPROVIDER = "TaxProvidersEnabled";
        public const string STORESETTING_TAXPROVIDERSETTING = "taxprovsetting";

        public const string STORESETTING_PAYMENTMETHODS = "PaymentMethodsEnabled";
        public const string STORESETTING_CREDITAUTHORIZEONLY = "PaymentCreditCardAuthorizeOnly";
        public const string STORESETTING_CREDITREQUIRECVV = "PaymentCreditCardRequireCVV";
        public const string STORESETTING_CREDITGATEWAY = "PaymentCreditCardGateway";
        public const string STORESETTING_RECURRINGGATEWAY = "PaymentReccuringGateway";
        public const string STORESETTING_ACCEPTEDCARDS = "PaymentAcceptedCards";
        public const string STORESETTING_GIFTCARDGATEWAY = "GiftCardGateway";
        public const string STORESETTING_GIFTCARDAUTHORIZEONLY = "PaymentGiftCardAuthorizeOnly";
        public const string STORESETTING_DISPLAYFULLCARDNUMBER = "PaymentDisplayFullCreditCardNumbers";
        public const string STORESETTING_PAYMENTMETHODSETTINGS = "methodsettings";
        public const string STORESETTING_PAYMENTGATEWAYSETTINGS = "paysettings";
        public const string STORESETTING_PAYMENTGIFTCARDSETTINGS = "gcpaysettings";

        public const string STORESETTING_REVIEWCOUNT = "ProductReviewCount";
        public const string STORESETTING_REVIEWMODERATE = "ProductReviewModerate";
        public const string STORESETTING_ALLOWREVIEWS = "AllowProductReviews";

        public const string STORESETTING_FEDEXKEY = "ShippingFedExKey";
        public const string STORESETTING_FEDEXPASSWORD = "ShippingFedExPassword";
        public const string STORESETTING_FEDEXACCOUNT = "ShippingFedExAccountNumber";
        public const string STORESETTING_FEDEXDIAGNOSTICS = "ShippingFedExDiagnostics";
        public const string STORESETTING_FEDEXDEFAULTPACKAGING = "ShippingFedExDefaultPackaging";
        public const string STORESETTING_FEDEXDROPOFFTYPE = "ShippingFedExDropOffType";
        public const string STORESETTING_FEDEXFORCERESIDENTIAL = "ShippingFedExForceResidentialRates";
        public const string STORESETTING_FEDEXMETERNUMBER = "ShippingFedExMeterNumber";
        public const string STORESETTING_FEDEXDEVURL = "ShippingFedExUseDevelopmentServiceUrl";

        public const string STORESETTING_UPSACCOUNT = "ShippingUpsAccountNumber";
        public const string STORESETTING_UPSDEFAULTPACKAGING = "ShippingUpsDefaultPackaging";
        public const string STORESETTING_UPSDEFAULTPAYMENT = "ShippingUpsDefaultPayment";
        public const string STORESETTING_UPSDEFAULTSERVICE = "ShippingUpsDefaultService";
        public const string STORESETTING_UPSDIAGNOSITCS = "ShippingUPSDiagnostics";
        public const string STORESETTING_UPSFORCERESIDENTIAL = "ShippingUpsForceResidential";
        public const string STORESETTING_UPSLICENSE = "Shipping_UPS_License";
        public const string STORESETTING_UPSPASSWORD = "Shipping_UPS_Password";
        public const string STORESETTING_UPSPICKUPTYPE = "Shipping_UPS_Pickup_Type";
        public const string STORESETTING_UPSSHIPDIMENSIONS = "ShippingUpsSkipDimensions";
        public const string STORESETTING_UPSUSERNAME = "Shipping_UPS_Username";
        public const string STORESETTING_UPSWRITEXML = "ShippingUPSWriteXML";

        public const string STORESETTING_UPSFDEFAULTPACKAGING = "ShippingUpsFreightDefaultPackaging";
        public const string STORESETTING_UPSFDEFAULTPAYMENT = "ShippingUpsFreightDefaultPayment";
        public const string STORESETTING_UPSFDIAGNOSTICS = "ShippingUPSFreightDiagnostics";
        public const string STORESETTING_UPSFFORCERESIDENTIAL = "ShippingUpsFreightForceResidential";
        public const string STORESETTING_UPSFSKIPDIMENSIONS = "ShippingUpsFreightSkipDimensions";
        public const string STORESETTING_UPSFBILLINGOPTION = "ShippingUpsFreightBillingOption";
        public const string STORESETTING_UPSFHANDLEONEUNITTYPE = "ShippingUpsFreightHandleOneUnitType";
        public const string STORESETTING_UPSFFREIGHTCLASS = "ShippingUpsFreightFreightClass";

        public const string STORESETTING_USPSUSERID = "ShippingUSPostalUserId";
        public const string STORESETTING_USPSDIAGNOSITCS = "ShippingUSPostalDiagnostics";

        public const string STORESETTING_AFFILIATECOMMISSIONAMOUNT = "AffiliateCommissionAmount";
        public const string STORESETTING_AFFILIATECOMMISSIONTYPE = "AffiliateCommissionType";
        public const string STORESETTING_AFFILIATECONFLICTMODE = "AffiliateConflictMode";
        public const string STORESETTING_AFFILIATEREFERRALDAYS = "AffiliateReferralDays";
        public const string STORESETTING_AFFILIATEREQUIREAPPROVAL = "AffiliateRequireApproval";
        public const string STORESETTING_AFFILIATEDISPLAYCHILDREN = "AffiliateDisplayChildren";
        public const string STORESETTING_AFFILIATESHOWIDCHECKOUT = "AffiliateShowIDOnCheckout";
        public const string STORESETTING_AFFILIATEAGREEMENTTEXT = "AffiliatAgreementText";
        public const string STORESETTING_AFFILIATEREVIEW = "AffiliateReview";

        public const string STORESETTING_STORECLOSED = "StoreClosed";
        public const string STORESETTING_STORECLOSEDDESC = "StoreClosedDescription";
        #endregion

        #region Views
        public const string TAG_CANONICAL = "<link rel=\"canonical\" href=\"{0}\" />";
        public const string TAG_OGTITLE = "<meta property=\"og:title\" content=\"{0}\"/>";
        public const string TAG_OGTYPE = "<meta property=\"og:type\" content=\"product\"/>";
        public const string TAG_OGURL = "<meta property=\"og:url\" content=\"{0}\"/>";
        public const string TAG_OGIMAGE = "<meta property=\"og:image\" content=\"{0}\"/>";
        public const string TAG_OGSITENAME = "<meta property=\"og:site_name\" content=\"{0}\" />";
        public const string TAG_OGFBADMIN = "<meta property=\"fb:admins\" content=\"{0}\" />";
        public const string TAG_OGFBAPPID = "<meta property=\"fb:app_id\" content=\"{0}\" />";
        public const string TAG_IOGPRICEAMOUNT = "<meta property=\"og:price:amount\" content=\"{0}\" />";
        public const string TAG_IOGPRICECURRENCY = "<meta property=\"og:price:currency\" content=\"{0}\" />";
        #endregion
    }
}
