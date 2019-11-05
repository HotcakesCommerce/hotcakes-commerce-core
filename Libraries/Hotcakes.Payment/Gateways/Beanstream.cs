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

using System;
using System.Collections.Specialized;
using System.Globalization;
using System.Net;
using System.Text;
using System.Web;
using Hotcakes.Web;

namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class Beanstream : PaymentGateway
    {
        public const string ApiUrl = "https://www.beanstream.com/scripts/process_transaction.asp";

        public Beanstream()
        {
            Settings = new BeanstreamSettings();
        }

        public override string Name
        {
            get { return "Beanstream"; }
        }

        public override string Id
        {
            get { return "D14D5F35-F5CE-4C7E-9495-E9164EBDFF4E"; }
        }

        public BeanstreamSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        protected override void Authorize(Transaction t)
        {
            ProcessPurcahseTransaction(t, "PA");
        }

        protected override void Charge(Transaction t)
        {
            ProcessPurcahseTransaction(t, "P");
        }

        protected override void Capture(Transaction t)
        {
            ProcessAdjustmentTransaction(t, "PAC");
        }

        protected override void Refund(Transaction t)
        {
            ProcessAdjustmentTransaction(t, "R");
        }

        protected override void Void(Transaction t)
        {
            ProcessAdjustmentTransaction(t, "PAC");
        }

        private void ProcessPurcahseTransaction(Transaction t, string type)
        {
            var parameters = new NameValueCollection();
            parameters.Add("requestType", "BACKEND");
            parameters.Add("merchant_id", Settings.MerchantId);

            parameters.Add("username", Settings.UserName);
            parameters.Add("password", Settings.Password);

            parameters.Add("trnOrderNumber", t.MerchantInvoiceNumber + "_" + DateTime.UtcNow.Ticks);
            parameters.Add("trnAmount", t.Amount.ToString("N", CultureInfo.InvariantCulture));
            parameters.Add("trnType", type);

            parameters.Add("trnCardOwner", Text.TrimToLength(t.Card.CardHolderName, 64));
            parameters.Add("trnCardNumber", t.Card.CardNumber);
            parameters.Add("trnExpMonth", t.Card.ExpirationMonthPadded);
            parameters.Add("trnExpYear", t.Card.ExpirationYearTwoDigits);
            parameters.Add("trnCardCvd", t.Card.SecurityCode);

            parameters.Add("ordName", Text.TrimToLength(t.Customer.FirstName + " " + t.Customer.LastName, 64));
            parameters.Add("ordEmailAddress", Text.TrimToLength(t.Customer.Email, 64));
            parameters.Add("ordPhoneNumber", Text.TrimToLength(t.Customer.Phone, 32));
            parameters.Add("ordAddress1", Text.TrimToLength(t.Customer.Street, 64));
            parameters.Add("ordCity", Text.TrimToLength(t.Customer.City, 32));
            parameters.Add("ordProvince", Text.TrimToLength(t.Customer.RegionBvin, 2));
            parameters.Add("ordPostalCode", Text.TrimToLength(t.Customer.PostalCode, 16));
            var country = t.Customer.CountryData;
            if (country != null)
                parameters.Add("ordCountry", Text.TrimToLength(country.IsoCode, 2));

            SendRequest(t, parameters);
        }

        private void ProcessAdjustmentTransaction(Transaction t, string type)
        {
            var parameters = new NameValueCollection();
            parameters.Add("requestType", "BACKEND");
            parameters.Add("merchant_id", Settings.MerchantId);

            parameters.Add("username", Settings.UserName);
            parameters.Add("password", Settings.Password);

            parameters.Add("trnOrderNumber", t.MerchantInvoiceNumber + "_" + Guid.NewGuid());
            parameters.Add("trnAmount", t.Amount.ToString("N", CultureInfo.InvariantCulture));

            parameters.Add("trnType", type);
            parameters.Add("adjId", t.PreviousTransactionNumber);

            SendRequest(t, parameters);
        }

        private void SendRequest(Transaction t, NameValueCollection parameters)
        {
            using (var client = new WebClient())
            {
                if (Settings.DebugMode)
                    t.Result.Messages.Add(new Message(Url.BuldQueryString(parameters), "DEBUG", MessageType.Information));

                var responseBytes = client.UploadValues(ApiUrl, "POST", parameters);
                var responseBody = Encoding.UTF8.GetString(responseBytes);

                if (Settings.DebugMode)
                    t.Result.Messages.Add(new Message(HttpUtility.UrlDecode(responseBody), "DEBUG",
                        MessageType.Information));

                var returnValues = HttpUtility.ParseQueryString(responseBody);
                ProcessResponse(t, returnValues);
            }
        }

        private void ProcessResponse(Transaction t, NameValueCollection returnValues)
        {
            if (returnValues["trnApproved"] == "1")
            {
                var trnId = returnValues["trnId"];
                if (!string.IsNullOrEmpty(trnId))
                    t.Result.ReferenceNumber = trnId;
                t.Result.Succeeded = true;
            }
            else
            {
                t.Result.Messages.Add(new Message("Beanstream Transaction Failed", "FAILED", MessageType.Error));
                t.Result.Messages.Add(new Message(returnValues["messageId"], "messageId", MessageType.Error));
                t.Result.Messages.Add(new Message(returnValues["messageText"], "messageText", MessageType.Error));
                t.Result.Messages.Add(new Message(returnValues["errorType"], "errorType", MessageType.Error));
                t.Result.Messages.Add(new Message(returnValues["errorFields"], "errorFields", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }
    }
}