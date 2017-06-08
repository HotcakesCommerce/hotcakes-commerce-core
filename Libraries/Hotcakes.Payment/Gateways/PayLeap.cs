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
using System.Text;
using System.Web;
using System.Xml.Linq;

namespace Hotcakes.Payment.Gateways
{
    [Serializable]
    public class PayLeap : PaymentGateway
    {
        private const string LiveUrl = "https://secure.payleap.com/SmartPayments/transact.asmx/ProcessCreditCard";
        private const string DeveloperUrl = "http://test.payleap.com/SmartPayments/transact.asmx/ProcessCreditCard";

        public PayLeap()
        {
            Settings = new PayLeapSettings();
        }

        public override string Name
        {
            get { return "PayLeap"; }
        }

        public override string Id
        {
            get { return "6FC76AD8-66BF-47b0-8982-1C4118F01645"; }
        }

        public PayLeapSettings Settings { get; set; }

        public override MethodSettings BaseSettings
        {
            get { return Settings; }
        }

        public override void ProcessTransaction(Transaction t)
        {
            var result = false;

            try
            {
                var url = LiveUrl;
                if (Settings.DeveloperMode)
                {
                    url = DeveloperUrl;
                }

                // Build Data String
                // Card Number and Expiration
                var expDate = t.Card.ExpirationMonthPadded;
                expDate += t.Card.ExpirationYearTwoDigits;


                // Set Parameters
                var sb = new StringBuilder();
                var postData = string.Empty;

                sb.Append("UserName=");
                sb.Append(SafeWriteString(Settings.Username.Trim()));
                sb.Append("&Password=");
                sb.Append(SafeWriteString(Settings.Password.Trim()));
                sb.Append("&Amount=");
                sb.Append(SafeWriteString(t.Amount.ToString()));
                sb.Append("&InvNum=");
                sb.Append(SafeWriteString(t.MerchantInvoiceNumber));
                sb.Append("&Street=");
                sb.Append(SafeWriteString(t.Customer.Street));
                sb.Append("&Zip=");
                sb.Append(SafeWriteString(t.Customer.PostalCode));
                sb.Append("&NameOnCard=");
                sb.Append(SafeWriteString(t.Card.CardHolderName));

                sb.Append("&MagData=");

                // Extra Tags
                var sbextra = new StringBuilder();
                sbextra.Append("<CustomerId>" + TextHelper.XmlEncode(t.Customer.Email) + "</CustomerId>");
                sbextra.Append("<City>" + TextHelper.XmlEncode(t.Customer.City) + "</City>");
                if (t.Customer.RegionName != string.Empty)
                {
                    sbextra.Append("<BillToState>" + TextHelper.XmlEncode(t.Customer.RegionName) + "</BillToState>");
                }
                if (Settings.TrainingMode)
                {
                    sbextra.Append("<TrainingMode>T</TrainingMode>");
                }
                sbextra.Append("<EntryMode>MANUAL</EntryMode>");

                switch (t.Action)
                {
                    case ActionType.CreditCardCharge:
                        // Charge
                        sb.Append("&TransType=");
                        sb.Append(SafeWriteString("Sale"));
                        sb.Append("&PNRef=");
                        break;
                    case ActionType.CreditCardHold:
                        // Authorize
                        sb.Append("&TransType=");
                        sb.Append(SafeWriteString("Auth"));
                        sb.Append("&PNRef=");
                        break;
                    case ActionType.CreditCardCapture:
                        // Capture, Post Authorize
                        sb.Append("&TransType=");
                        sb.Append(SafeWriteString("Force"));
                        sb.Append("&PNRef=");
                        sb.Append(SafeWriteString(t.PreviousTransactionNumber));
                        sbextra.Append("<AuthCode>" + t.PreviousTransactionAuthCode + "</AuthCode>");
                        break;
                    case ActionType.CreditCardVoid:
                        // Void
                        sb.Append("&TransType=");
                        sb.Append(SafeWriteString("Void"));
                        sb.Append("&PNRef=");
                        sb.Append(SafeWriteString(t.PreviousTransactionNumber));
                        break;
                    case ActionType.CreditCardRefund:
                        // Refund, Credit
                        sb.Append("&TransType=");
                        sb.Append(SafeWriteString("Return"));
                        sb.Append("&PNRef=");
                        sb.Append(SafeWriteString(t.PreviousTransactionNumber));
                        break;
                }

                // Add Card Number, CVV Code and Expiration Date
                sb.Append("&CardNum=");
                sb.Append(SafeWriteString(t.Card.CardNumber));

                sb.Append("&CVNum=");
                if (!string.IsNullOrEmpty(t.Card.SecurityCode))
                {
                    sb.Append(SafeWriteString(t.Card.SecurityCode));
                }

                sb.Append("&ExpDate=");
                if (t.Action != ActionType.CreditCardVoid)
                {
                    sb.Append(SafeWriteString(expDate));
                }

                // Write Extra Tags
                sb.Append("&ExtData=");
                sb.Append(SafeWriteString(sbextra.ToString()));
                
                // Dump string builder to string to send to Authorize.Net
                postData = sb.ToString();

                var xmlresponse = string.Empty;
                try
                {
                    xmlresponse = NetworkUtilities.SendRequestByPost(url, postData);
                }
                catch (Exception ex)
                {
                    throw new ArgumentException("Error: URL|" + url + "  POST|" + postData + " RESPONSE|" + xmlresponse +
                                                " :: " + ex.Message);
                }

                if (Settings.EnableDebugTracing)
                {
                    t.Result.Messages.Add(new Message(postData, "TRACE-POST:", MessageType.Error));
                    t.Result.Messages.Add(new Message(xmlresponse, "TRACE-REPLY:", MessageType.Error));
                }

                var response = XDocument.Parse(xmlresponse);
                XNamespace ns = response.Root.Attribute("xmlns").Value ?? "";
                var r = new PayLeapResponse();
                r.Parse(response);

                if (r != null)
                {
                    t.Result.CvvCode = CvnResponseType.Unavailable;
                    t.Result.ResponseCode = r.AuthCode;
                    t.Result.ResponseCodeDescription = r.Message;
                    t.Result.ReferenceNumber = r.PNRef;
                    t.Result.ReferenceNumber2 = r.AuthCode;
                    t.Result.AvsCode = ParseAvsCode(r.GetAVSResult);

                    if (r.Result == "0")
                    {
                        result = true;
                    }
                    else
                    {
                        result = false;
                        t.Result.Messages.Add(new Message(r.RespMSG, r.AuthCode, MessageType.Warning));
                    }
                }
            }
            catch (Exception ex)
            {
                result = false;
                t.Result.Messages.Add(new Message("Unknown Payment Error: " + ex.Message, "HCP_PL_1001",
                    MessageType.Error));
                t.Result.Messages.Add(new Message("Stack Trace " + ex.StackTrace, "STACKTRACE", MessageType.Error));
            }

            t.Result.Succeeded = result;
        }

        #region Helper Methods

        private string SafeWriteBool(bool input)
        {
            if (input)
            {
                return HttpUtility.UrlEncode("TRUE");
            }
            return HttpUtility.UrlEncode("FALSE");
        }

        private string SafeWriteString(string input)
        {
            return HttpUtility.UrlEncode(input);
        }

        private AvsResponseType ParseAvsCode(string code)
        {
            var result = AvsResponseType.Unavailable;

            switch (code.ToUpper())
            {
                case "A":
                    result = AvsResponseType.PartialMatchAddress;
                    break;
                case "B":
                    result = AvsResponseType.Unavailable;
                    break;
                case "E":
                    result = AvsResponseType.Error;
                    break;
                case "G":
                    result = AvsResponseType.Unavailable;
                    break;
                case "N":
                    result = AvsResponseType.NoMatch;
                    break;
                case "P":
                    result = AvsResponseType.Unavailable;
                    break;
                case "R":
                    result = AvsResponseType.Unavailable;
                    break;
                case "S":
                    result = AvsResponseType.Unavailable;
                    break;
                case "U":
                    result = AvsResponseType.Unavailable;
                    break;
                case "W":
                    result = AvsResponseType.PartialMatchPostalCode;
                    break;
                case "X":
                    result = AvsResponseType.FullMatch;
                    break;
                case "Y":
                    result = AvsResponseType.FullMatch;
                    break;
                case "Z":
                    result = AvsResponseType.PartialMatchPostalCode;
                    break;
            }

            return result;
        }

        private CvnResponseType ParseSecurityCode(string code)
        {
            var result = CvnResponseType.Unavailable;

            switch (code.ToUpper())
            {
                case "M":
                    result = CvnResponseType.Match;
                    break;
                case "N":
                    result = CvnResponseType.NoMatch;
                    break;
                case "P":
                    result = CvnResponseType.Unavailable;
                    break;
                case "S":
                    result = CvnResponseType.Error;
                    break;
                case "U":
                    result = CvnResponseType.Unavailable;
                    break;
                case "X":
                    result = CvnResponseType.Unavailable;
                    break;
            }
            return result;
        }

        #endregion
    }

    public class PayLeapResponse
    {
        public PayLeapResponse()
        {
            Result = string.Empty;
            RespMSG = string.Empty;
            Message = string.Empty;
            AuthCode = string.Empty;
            PNRef = string.Empty;
            GetAVSResult = string.Empty;
            GetCVResult = string.Empty;
        }

        public string Result { get; set; }
        public string Message { get; set; }
        public string RespMSG { get; set; }
        public string AuthCode { get; set; }
        public string PNRef { get; set; }
        public string GetAVSResult { get; set; }
        public string GetCVResult { get; set; }

        public void Parse(XDocument response)
        {
            if (response != null)
            {
                foreach (var e in response.Root.Elements())
                {
                    switch (e.Name.LocalName)
                    {
                        case "Result":
                            Result = e.Value;
                            break;
                        case "RespMSG":
                            RespMSG = e.Value;
                            break;
                        case "Message":
                            Message = e.Value;
                            break;
                        case "AuthCode":
                            AuthCode = e.Value;
                            break;
                        case "GetAVSResult":
                            GetAVSResult = e.Value;
                            break;
                        case "GetCVResult":
                            GetCVResult = e.Value;
                            break;
                        case "PNRef":
                            PNRef = e.Value;
                            break;
                    }
                }
            }
        }
    }
}