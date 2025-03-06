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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage gift card transactions in the main application.
    /// </summary>
    /// <remarks>The REST API equivalent is GiftCardData.</remarks>
    [DataContract]
    [Serializable]
    public class OrderTransactionGiftCardDataDTO
    {
        /// <summary>
        ///     The unique ID of the line item in the order for this gift card.
        /// </summary>
        [DataMember]
        public long LineItemId { get; set; }

        /// <summary>
        ///     The assigned gift card number for this card.
        /// </summary>
        [DataMember]
        public string CardNumber { get; set; }

        /// <summary>
        ///     A code used to validate the gift card.
        /// </summary>
        public string SecurityCode { get; set; }

        /// <summary>
        ///     The amount that the gift card was purchased for.
        /// </summary>
        [DataMember]
        public decimal Amount { get; set; }

        /// <summary>
        ///     The full name of the gift card recipient.
        /// </summary>
        [DataMember]
        public string RecipientName { get; set; }

        /// <summary>
        ///     The email address of the gift card recipient.
        /// </summary>
        [DataMember]
        public string RecipientEmail { get; set; }

        /// <summary>
        ///     The date that the gift card will expire.
        /// </summary>
        [DataMember]
        public DateTime ExpirationDate { get; set; }
    }
}