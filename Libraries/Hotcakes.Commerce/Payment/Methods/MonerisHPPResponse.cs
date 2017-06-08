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

using System.Globalization;
using System.Xml.Serialization;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    ///     Moneris Hosted Pay Page Response
    /// </summary>
    [XmlRoot("response")]
    public class MonerisHPPResponse
    {
        [XmlElement("response_order_id")]
        public string ResponseOrderId { get; set; }

        [XmlElement("response_code")]
        public string ResponseCode { get; set; }

        [XmlElement("date_stamp")]
        public string DateStamp { get; set; }

        [XmlElement("time_stamp")]
        public string TimeStamp { get; set; }

        [XmlElement("bank_approval_code")]
        public string BankApprovalCode { get; set; }

        [XmlElement("result")]
        public int Result { get; set; }

        [XmlElement("trans_name")]
        public string TransactionName { get; set; }

        [XmlElement("cardholder")]
        public string Cardholder { get; set; }

        [XmlElement("charge_total")]
        public string ChargeTotalOriginal { get; set; }

        [XmlElement("Card")]
        public string Card { get; set; }

        [XmlElement("message")]
        public string Message { get; set; }

        [XmlElement("iso_code")]
        public string IsoCode { get; set; }

        [XmlElement("bank_transaction_id")]
        public string BankTransactionId { get; set; }

        [XmlElement("transactionKey")]
        public string TransactionKey { get; set; }

        [XmlElement("Ticket")]
        public string Ticket { get; set; }

        [XmlElement("Eci")]
        public int Eci { get; set; }

        [XmlElement("txn_num")]
        public string TxnNum { get; set; }

        [XmlElement("avs_response_code")]
        public string AvsResponseCode { get; set; }

        [XmlElement("cvd_response_code")]
        public string CvdResponseCode { get; set; }

        [XmlElement("cavv_result_code")]
        public string CavvResultCode { get; set; }

        [XmlElement("is_visa_debit")]
        public bool IsVisaDebit { get; set; }

        [XmlIgnore]
        public decimal ChargeTotal
        {
            get
            {
                decimal price;
                decimal.TryParse(ChargeTotalOriginal, NumberStyles.Any, CultureInfo.InvariantCulture, out price);
                return price;
            }
        }
    }
}