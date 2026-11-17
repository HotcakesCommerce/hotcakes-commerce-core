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

using System.Xml.Serialization;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Represents the XML response returned by Moneris when a Hosted Pay Page
    /// data-preload request is submitted.
    /// </summary>
    /// <remarks>
    /// The root element is <c>response</c>. The element names are part of the
    /// Moneris contract and are preserved by the <see cref="XmlElementAttribute"/>
    /// declarations on each property.
    /// </remarks>
    [XmlRoot("response")]
    public class MonerisHPPDPResponse
    {
        /// <summary>
        /// Gets or sets the Moneris Hosted Pay Page identifier returned in
        /// <c>hpp_id</c>.
        /// </summary>
        [XmlElement("hpp_id")]
        public string HostedPayPageId { get; set; }

        /// <summary>
        /// Gets or sets the preload ticket returned in <c>ticket</c>.
        /// </summary>
        [XmlElement("ticket")]
        public string Ticket { get; set; }

        /// <summary>
        /// Gets or sets the order identifier returned in <c>order_id</c>.
        /// </summary>
        [XmlElement("order_id")]
        public string OrderId { get; set; }

        /// <summary>
        /// Gets or sets the provider response code returned in
        /// <c>response_code</c>.
        /// </summary>
        [XmlElement("response_code")]
        public string ResponseCode { get; set; }
    }
}
