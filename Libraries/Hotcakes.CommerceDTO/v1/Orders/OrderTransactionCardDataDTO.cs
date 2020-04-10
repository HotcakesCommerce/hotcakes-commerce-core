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
using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage credit card data in the REST API.
    /// </summary>
    /// <remarks>The main application equivalent is CardData.</remarks>
    [DataContract]
    [Serializable]
    public class OrderTransactionCardDataDTO
    {
        public OrderTransactionCardDataDTO()
        {
            CardHolderName = string.Empty;
            CardIsEncrypted = false;
            CardNumber = string.Empty;
            ExpirationMonth = 1;
            ExpirationYear = 1900;
        }

        /// <summary>
        ///     The number of the credit card.
        /// </summary>
        [DataMember]
        public string CardNumber { get; set; }

        /// <summary>
        ///     Used to tell the API if the card number is encrypted or not.
        /// </summary>
        [DataMember]
        public bool CardIsEncrypted { get; set; }

        /// <summary>
        ///     The month that the credit card will expire.
        /// </summary>
        [DataMember]
        public int ExpirationMonth { get; set; }

        /// <summary>
        ///     The year that the credit card will expire.
        /// </summary>
        [DataMember]
        public int ExpirationYear { get; set; }

        /// <summary>
        ///     The name of the credit card holder as it appears on the card.
        /// </summary>
        [DataMember]
        public string CardHolderName { get; set; }
    }
}