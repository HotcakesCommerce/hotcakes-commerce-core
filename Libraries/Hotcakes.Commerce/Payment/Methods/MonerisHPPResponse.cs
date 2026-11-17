#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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

using System.Globalization;
using System.Xml.Serialization;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Represents the XML response returned by a Moneris Hosted Pay Page.
    /// </summary>
    /// <remarks>
    /// The root element is <c>response</c>. The provider uses the exact XML
    /// element names declared below, including the case-sensitive names
    /// <c>Card</c>, <c>transactionKey</c>, <c>Ticket</c>, and <c>Eci</c>.
    /// </remarks>
    [XmlRoot("response")]
    public class MonerisHPPResponse
    {
        /// <summary>Gets or sets the order identifier in <c>response_order_id</c>.</summary>
        [XmlElement("response_order_id")]
        public string ResponseOrderId { get; set; }

        /// <summary>Gets or sets the provider response code in <c>response_code</c>.</summary>
        [XmlElement("response_code")]
        public string ResponseCode { get; set; }

        /// <summary>Gets or sets the provider date in <c>date_stamp</c>.</summary>
        [XmlElement("date_stamp")]
        public string DateStamp { get; set; }

        /// <summary>Gets or sets the provider time in <c>time_stamp</c>.</summary>
        [XmlElement("time_stamp")]
        public string TimeStamp { get; set; }

        /// <summary>Gets or sets the bank approval code in <c>bank_approval_code</c>.</summary>
        [XmlElement("bank_approval_code")]
        public string BankApprovalCode { get; set; }

        /// <summary>Gets or sets the numeric transaction result in <c>result</c>.</summary>
        [XmlElement("result")]
        public int Result { get; set; }

        /// <summary>Gets or sets the transaction name in <c>trans_name</c>.</summary>
        [XmlElement("trans_name")]
        public string TransactionName { get; set; }

        /// <summary>Gets or sets the cardholder value in <c>cardholder</c>.</summary>
        [XmlElement("cardholder")]
        public string Cardholder { get; set; }

        /// <summary>
        /// Gets or sets the original charge amount text from <c>charge_total</c>.
        /// Use <see cref="ChargeTotal"/> for its parsed invariant-culture value.
        /// </summary>
        [XmlElement("charge_total")]
        public string ChargeTotalOriginal { get; set; }

        /// <summary>Gets or sets the card value from the case-sensitive <c>Card</c> element.</summary>
        [XmlElement("Card")]
        public string Card { get; set; }

        /// <summary>Gets or sets the provider message in <c>message</c>.</summary>
        [XmlElement("message")]
        public string Message { get; set; }

        /// <summary>Gets or sets the ISO response code in <c>iso_code</c>.</summary>
        [XmlElement("iso_code")]
        public string IsoCode { get; set; }

        /// <summary>Gets or sets the bank transaction identifier in <c>bank_transaction_id</c>.</summary>
        [XmlElement("bank_transaction_id")]
        public string BankTransactionId { get; set; }

        /// <summary>Gets or sets the transaction key from the case-sensitive <c>transactionKey</c> element.</summary>
        [XmlElement("transactionKey")]
        public string TransactionKey { get; set; }

        /// <summary>Gets or sets the ticket from the case-sensitive <c>Ticket</c> element.</summary>
        [XmlElement("Ticket")]
        public string Ticket { get; set; }

        /// <summary>Gets or sets the 3-D Secure ECI value from the case-sensitive <c>Eci</c> element.</summary>
        [XmlElement("Eci")]
        public int Eci { get; set; }

        /// <summary>Gets or sets the transaction number in <c>txn_num</c>.</summary>
        [XmlElement("txn_num")]
        public string TxnNum { get; set; }

        /// <summary>Gets or sets the address-verification response code in <c>avs_response_code</c>.</summary>
        [XmlElement("avs_response_code")]
        public string AvsResponseCode { get; set; }

        /// <summary>Gets or sets the card-validation response code in <c>cvd_response_code</c>.</summary>
        [XmlElement("cvd_response_code")]
        public string CvdResponseCode { get; set; }

        /// <summary>Gets or sets the CAVV result code in <c>cavv_result_code</c>.</summary>
        [XmlElement("cavv_result_code")]
        public string CavvResultCode { get; set; }

        /// <summary>Gets or sets whether the transaction is identified as Visa Debit in <c>is_visa_debit</c>.</summary>
        [XmlElement("is_visa_debit")]
        public bool IsVisaDebit { get; set; }

        /// <summary>
        /// Gets the value of <see cref="ChargeTotalOriginal"/> parsed as a decimal.
        /// </summary>
        /// <remarks>
        /// Parsing uses <see cref="NumberStyles.Any"/> and
        /// <see cref="CultureInfo.InvariantCulture"/>. If parsing fails, the
        /// existing <c>decimal.TryParse</c> behavior returns <c>0</c>.
        /// </remarks>
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
