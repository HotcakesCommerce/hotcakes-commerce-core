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

using System.Collections.Specialized;
using Hotcakes.Payment.Gateways;
using AuthorizeNetSDK = AuthorizeNet;

namespace Hotcakes.Payment.RecurringGateways
{
    public class AuthorizeNet : RecurringPaymentGateway
    {
        #region Constructor

        public AuthorizeNet()
        {
            Settings = new AuthorizeNetSettings();
        }

        #endregion

        public override Range GetRangeForIntervalType(RecurringIntervalType intervalType)
        {
            switch (intervalType)
            {
                case RecurringIntervalType.Days:
                    return new Range {Minimum = 7, Maximum = 365};
                case RecurringIntervalType.Months:
                    return new Range {Minimum = 1, Maximum = 12};
                default:
                    return base.GetRangeForIntervalType(intervalType);
            }
        }

        protected override void CreateSubscription(Transaction t)
        {
            var gateway = CreateSubscriptionGateway();

            var subs = AuthorizeNetSDK.SubscriptionRequest.CreateNew();
            subs.SubscriptionName = t.RecurringBilling.SubscriptionName;
            if (subs.SubscriptionName.Length > 50)
                subs.SubscriptionName = subs.SubscriptionName.Substring(0, 50);

            subs.Invoice = t.MerchantInvoiceNumber;
            subs.Description = t.MerchantDescription;

            subs.BillingInterval = (short) t.RecurringBilling.Interval;
            subs.BillingIntervalUnits = (AuthorizeNetSDK.BillingIntervalUnits) t.RecurringBilling.IntervalType;
            subs.Amount = t.Amount;

            subs.CardNumber = t.Card.CardNumber;
            subs.CardCode = t.Card.SecurityCode;
            subs.CardExpirationMonth = t.Card.ExpirationMonth;
            subs.CardExpirationYear = t.Card.ExpirationYear;

            subs.BillingAddress = CreateBillingAddress(t);
            subs.CustomerEmail = t.Customer.Email;

            var result = gateway.CreateSubscription(subs);
            t.Result.ReferenceNumber = result.SubscriptionID;
            t.Result.Succeeded = !string.IsNullOrEmpty(result.SubscriptionID);
        }

        protected override void UpdateSubscription(Transaction t)
        {
            var gateway = CreateSubscriptionGateway();

            var subs = AuthorizeNetSDK.SubscriptionRequest.CreateNew();

            subs.BillingInterval = (short) t.RecurringBilling.Interval;
            subs.BillingIntervalUnits = (AuthorizeNetSDK.BillingIntervalUnits) t.RecurringBilling.IntervalType;
            subs.Amount = t.Amount;

            // if billing address is going to be updated only
            if (t.Card != null)
            {
                subs.CardNumber = t.Card.CardNumber;
                subs.CardCode = t.Card.SecurityCode;
                subs.CardExpirationMonth = t.Card.ExpirationMonth;
                subs.CardExpirationYear = t.Card.ExpirationYear;
            }

            subs.BillingAddress = CreateBillingAddress(t);
            subs.CustomerEmail = t.Customer.Email;
            subs.SubscriptionID = t.RecurringBilling.SubscriptionId;

            t.Result.Succeeded = gateway.UpdateSubscription(subs);
            t.Result.ReferenceNumber = subs.SubscriptionID;
        }

        protected override void CancelSubscription(Transaction t)
        {
            var gateway = CreateSubscriptionGateway();

            t.Result.Succeeded = gateway.CancelSubscription(t.RecurringBilling.SubscriptionId);
            t.Result.ReferenceNumber = t.RecurringBilling.SubscriptionId;
        }

        private AuthorizeNetSDK.Address CreateBillingAddress(Transaction t)
        {
            var address = new AuthorizeNetSDK.Address
            {
                First = t.Customer.FirstName,
                Last = t.Customer.LastName,
                Company = t.Customer.Company,
                Street = t.Customer.Street,
                City = t.Customer.City
            };

            var country = t.Customer.CountryData;
            address.Country = country != null ? country.IsoNumeric : t.Customer.CountryName;

            // TODO: Add code to make sure we've got the correct state format
            if (!string.IsNullOrWhiteSpace(t.Customer.RegionName))
            {
                address.State = t.Customer.RegionName;
            }

            address.Zip = t.Customer.PostalCode;
            address.Phone = t.Customer.Phone;
            return address;
        }

        private AuthorizeNetSDK.SubscriptionGateway CreateSubscriptionGateway()
        {
            var mode = Settings.DeveloperMode ? AuthorizeNetSDK.ServiceMode.Test : AuthorizeNetSDK.ServiceMode.Live;
            return new AuthorizeNetSDK.SubscriptionGateway(Settings.MerchantLoginId, Settings.TransactionKey, mode);
        }

        public static Transaction ParseSilentPost(NameValueCollection postData)
        {
            var authTrans = new SilentPostHelper().Parse(postData);

            if (authTrans.IsRecurring)
            {
                var t = new Transaction
                {
                    Amount = authTrans.RequestedAmount,
                    Action = ActionType.RecurringPayment,
                    Customer = new CustomerData
                    {
                        FirstName = authTrans.FirstName,
                        LastName = authTrans.LastName,
                        Email = authTrans.CustomerEmail,
                        Company = authTrans.BillingAddress.Company,
                        Street = authTrans.BillingAddress.Street,
                        City = authTrans.BillingAddress.City,
                        RegionName = authTrans.BillingAddress.State,
                        PostalCode = authTrans.BillingAddress.Zip,
                        CountryName = authTrans.BillingAddress.Country,
                        Phone = authTrans.BillingAddress.Phone
                    },
                    Result = new ResultData
                    {
                        //AvsCode = authTrans.AVSCode	
                        AvsCodeDescription = authTrans.AVSResponse,
                        ResponseCode = authTrans.ResponseCode.ToString(),
                        ResponseCodeDescription = authTrans.ResponseReason,
                        Succeeded = authTrans.ResponseCode == 1,
                        ReferenceNumber = authTrans.Subscription.ID.ToString(),
                        ReferenceNumber2 = authTrans.Subscription.PayNum.ToString()
                    },
                    MerchantInvoiceNumber = authTrans.InvoiceNumber
                };

                return t;
            }
            return null;
        }

        #region Internal declaration

        /// <summary>
        /// </summary>
        internal class SilentPostHelper
        {
            internal AuthorizeNetSDK.Transaction Parse(NameValueCollection postData)
            {
                var t = new AuthorizeNetSDK.Transaction
                {
                    ResponseCode = GetIntValue(postData["x_response_code"], 0),
                    //x_response_subcode
                    //x_response_reason_code
                    ResponseReason = postData["x_response_reason_text"],
                    AuthorizationCode = postData["x_auth_code"],
                    AVSCode = postData["x_avs_code"],
                    TransactionID = postData["x_trans_id"],
                    InvoiceNumber = postData["x_invoice_num"],
                    Description = postData["x_description"],
                    RequestedAmount = GetDecimalValue(postData["x_amount"], 0),
                    //postData["x_method"],
                    TransactionType = postData["x_type"],
                    CustomerID = postData["x_cust_id"],
                    FirstName = postData["x_first_name"],
                    LastName = postData["x_last_name"],
                    BillingAddress = new AuthorizeNetSDK.Address
                    {
                        Company = postData["x_company"],
                        Street = postData["x_address"],
                        City = postData["x_city"],
                        State = postData["x_state"],
                        Zip = postData["x_zip"],
                        Country = postData["x_country"],
                        Phone = postData["x_phone"],
                        Fax = postData["x_fax"]
                    },
                    CustomerEmail = postData["x_e-mail"],
                    ShippingAddress = new AuthorizeNetSDK.Address
                    {
                        First = postData["x_ship_to_first_name"],
                        Last = postData["x_ship_to_last_name"],
                        Company = postData["x_ship_to_company"],
                        Street = postData["x_ship_to_address"],
                        City = postData["x_ship_to_city"],
                        State = postData["x_ship_to_state"],
                        Zip = postData["x_ship_to_zip"],
                        Country = postData["x_ship_to_country"]
                    },
                    Tax = GetDecimalValue(postData["x_tax"], 0),
                    Duty = GetDecimalValue(postData["x_duty"], 0),
                    //FraudFilters = postData["x_freight"],
                    TaxExempt = (postData["x_tax_exempt"] ?? string.Empty).ToUpper() == "TRUE",
                    PONumber = postData["x_po_num"],
                    //postData["x_MD5_Hash"],
                    CardResponseCode = postData["x_cavv_response"],
                    //postData["x_test_request"],
                    Subscription = new AuthorizeNetSDK.SubscriptionPayment
                    {
                        ID = GetIntValue(postData["x_subscription_id"], 0),
                        PayNum = GetIntValue(postData["x_subscription_paynum"], 0)
                    }
                };

                t.IsRecurring = t.Subscription.ID > 0;

                return t;
            }

            private static int GetIntValue(string value, int defValue)
            {
                int i;
                if (int.TryParse(value, out i))
                {
                    return i;
                }

                return defValue;
            }

            private static decimal GetDecimalValue(string value, decimal defValue)
            {
                decimal i;
                if (decimal.TryParse(value, out i))
                {
                    return i;
                }

                return defValue;
            }
        }

        #endregion

        #region Properties

        public override string Name
        {
            get { return "Authorize.Net"; }
        }

        public override string Id
        {
            get { return "1ED5E3E8-BF78-462F-AB16-906EFFC28D2A"; }
        }

        public AuthorizeNetSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        #endregion
    }
}