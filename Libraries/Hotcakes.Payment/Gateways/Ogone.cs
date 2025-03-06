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
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using System.Xml.Serialization;
using Hotcakes.Web;

namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class Ogone : PaymentGateway
    {
        private const string LiveOrderUrl = "https://secure.ogone.com/ncol/prod/orderdirect.asp";
        private const string DeveloperOrderUrl = "https://secure.ogone.com/ncol/test/orderdirect.asp";
        private const string LiveMaintenanceUrl = "https://secure.ogone.com/ncol/prod/maintenancedirect.asp";
        private const string DeveloperMaintenanceUrl = "https://secure.ogone.com/ncol/test/maintenancedirect.asp";

        public Ogone()
        {
            Settings = new OgoneSettings();
        }

        public override string Name
        {
            get { return "Ogone"; }
        }

        public override string Id
        {
            get { return "268F8127-760F-4D71-A4AA-39EA95DF35D5"; }
        }

        public OgoneSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        protected override void Authorize(Transaction t)
        {
            ProcessOrderTransaction(t, "RES");
        }

        protected override void Charge(Transaction t)
        {
            ProcessOrderTransaction(t, "SAL");
        }

        protected override void Capture(Transaction t)
        {
            ProcessMaintenanceTransaction(t, "SAL");
        }

        protected override void Refund(Transaction t)
        {
            ProcessMaintenanceTransaction(t, "RFD");
        }

        protected override void Void(Transaction t)
        {
            ProcessMaintenanceTransaction(t, "DEL");
        }

        private void ProcessOrderTransaction(Transaction t, string type)
        {
            var url = LiveOrderUrl;
            if (Settings.DeveloperMode)
            {
                url = DeveloperOrderUrl;
            }
            var parameters = new NameValueCollection();
            parameters.Add("PSPID", Settings.PaymentServiceProviderId);
            parameters.Add("USERID", Settings.UserId);
            parameters.Add("PSWD", Settings.Password);

            parameters.Add("ORDERID", t.MerchantInvoiceNumber + "_" + DateTime.UtcNow.Ticks);
            var amount = (int) (t.Amount*100);
            parameters.Add("AMOUNT", amount.ToString());

            parameters.Add("CURRENCY", Settings.Currency);
            parameters.Add("CARDNO", t.Card.CardNumber);
            parameters.Add("ED", t.Card.ExpirationMonthPadded + t.Card.ExpirationYearTwoDigits);
            parameters.Add("CVC", t.Card.SecurityCode);

            parameters.Add("CN", Text.TrimToLength(t.Customer.FirstName + " " + t.Customer.LastName, 35));
            parameters.Add("EMAIL", Text.TrimToLength(t.Customer.Email, 50));

            parameters.Add("ECOM_PAYMENT_CARD_VERIFICATION", t.Card.SecurityCode);
            parameters.Add("OWNERADDRESS", Text.TrimToLength(t.Customer.Street, 35));
            parameters.Add("OWNERZIP", Text.TrimToLength(t.Customer.PostalCode, 10));
            parameters.Add("OWNERTOWN", Text.TrimToLength(t.Customer.City, 40));
            if (t.Customer.CountryData != null)
                parameters.Add("OWNERCTY", Text.TrimToLength(t.Customer.CountryData.IsoCode, 2));
            parameters.Add("OWNERTELNO", Text.TrimToLength(t.Customer.Phone, 20));

            parameters.Add("OPERATION", type);

            parameters.Add("REMOTE_ADDR", t.Customer.IpAddress);

            SendRequest(t, url, parameters);
        }

        private void ProcessMaintenanceTransaction(Transaction t, string type)
        {
            var url = LiveMaintenanceUrl;
            if (Settings.DeveloperMode)
            {
                url = DeveloperMaintenanceUrl;
            }
            var parameters = new NameValueCollection
            {
                {"PSPID", Settings.PaymentServiceProviderId},
                {"USERID", Settings.UserId},
                {"PSWD", Settings.Password},
                {"PAYID", t.PreviousTransactionNumber}
            };

            var amount = (int) (t.Amount*100);
            parameters.Add("AMOUNT", amount.ToString());

            parameters.Add("OPERATION", type);

            SendRequest(t, url, parameters);
        }

        private void SendRequest(Transaction t, string url, NameValueCollection parameters)
        {
            using (var client = new WebClient())
            {
                if (Settings.DebugMode)
                    t.Result.Messages.Add(new Message(Url.BuldQueryString(parameters), "DEBUG", MessageType.Information));

                var responseBytes = client.UploadValues(url, "POST", parameters);
                var responseBody = Encoding.UTF8.GetString(responseBytes);

                if (Settings.DebugMode)
                    t.Result.Messages.Add(new Message(HttpUtility.HtmlEncode(responseBody), "DEBUG",
                        MessageType.Information));

                ProcessResponse(t, responseBody);
            }
        }

        private void ProcessResponse(Transaction t, string responseBody)
        {
            OgoneResponse response;

            using (TextReader reader = new StringReader(responseBody))
            {
                var serializer = new XmlSerializer(typeof (OgoneResponse));
                response = (OgoneResponse) serializer.Deserialize(reader);
            }

            // X1 statuses evidently correspon to offline processing
            // But they also are returned by Maintenance transactions
            // So we will treat them as Successful here
            if (response.Status == 5 || response.Status == 51 || // Authorisation
                response.Status == 6 || response.Status == 61 || // Void
                response.Status == 8 || response.Status == 81 || // Refund
                response.Status == 9 || response.Status == 91) // Payment
            {
                if (!string.IsNullOrEmpty(response.PayId))
                    t.Result.ReferenceNumber = response.PayId;
                t.Result.Succeeded = true;
            }
            else if (response.Status == 52 ||
                     response.Status == 62 ||
                     response.Status == 82 ||
                     response.Status == 92
                )
            {
                if (!string.IsNullOrEmpty(response.PayId))
                    t.Result.ReferenceNumber = response.PayId;
                t.Result.Succeeded = false;
                t.Result.Messages.Add(new Message("Ogone Transaction Uncertain", "UNCERTAIN", MessageType.Warning));
                t.Result.Messages.Add(new Message(response.Status.ToString(), "STATUS", MessageType.Error));
                t.Result.Messages.Add(new Message(response.NcStatus.ToString(), "NCSTATUS", MessageType.Error));
                t.Result.Messages.Add(new Message(response.NcError, "NCERROR", MessageType.Error));
                t.Result.Messages.Add(new Message(response.NcErrorPlus, "NCERRORPLUS", MessageType.Error));
            }
            else
            {
                t.Result.Messages.Add(new Message("Ogone Transaction Failed", "FAILED", MessageType.Error));
                t.Result.Messages.Add(new Message(response.Status.ToString(), "STATUS", MessageType.Error));
                t.Result.Messages.Add(new Message(response.NcStatus.ToString(), "NCSTATUS", MessageType.Error));
                t.Result.Messages.Add(new Message(response.NcError, "NCERROR", MessageType.Error));
                t.Result.Messages.Add(new Message(response.NcErrorPlus, "NCERRORPLUS", MessageType.Error));
                t.Result.Succeeded = false;
            }
        }
    }

    [XmlRoot("ncresponse")]
    public class OgoneResponse
    {
        [XmlAttribute("orderID")]
        public string OrderId { get; set; }

        [XmlAttribute("PAYID")]
        public string PayId { get; set; }

        [XmlAttribute("PAYIDSUB")]
        public string PaySubId { get; set; }

        [XmlAttribute("NCSTATUS")]
        public int NcStatus { get; set; }

        [XmlAttribute("NCERROR")]
        public string NcError { get; set; }

        [XmlAttribute("NCERRORPLUS")]
        public string NcErrorPlus { get; set; }

        [XmlAttribute("ACCEPTANCE")]
        public string Acceptance { get; set; }

        [XmlAttribute("STATUS")]
        public int Status { get; set; }

        [XmlAttribute("ECI")]
        public string ECI { get; set; }

        [XmlAttribute("amount")]
        public string Amount { get; set; }

        [XmlAttribute("currency")]
        public string Currency { get; set; }

        [XmlAttribute("PM")]
        public string PaymentMethod { get; set; }

        [XmlAttribute("BRAND")]
        public string Brand { get; set; }
    }
}