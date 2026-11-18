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

using System;

namespace Hotcakes.Commerce.Payment.Methods
{
    /// <summary>
    /// Represents the parsed fields returned by an Ogone payment response.
    /// </summary>
    /// <remarks>
    /// The properties retain the provider's response concepts and are used
    /// together with <see cref="OgoneUtils"/> when validating the returned
    /// SHA signature. No normalization of the provider values is implied by
    /// this contract.
    /// </remarks>
    [Serializable]
    public class OgoneResponse
    {
        /// <summary>Gets or sets the provider order identifier.</summary>
        public string OrderId { get; set; }

        /// <summary>Gets or sets the transaction amount.</summary>
        public decimal Amount { get; set; }

        /// <summary>Gets or sets the transaction currency.</summary>
        public string Currency { get; set; }

        /// <summary>Gets or sets the payment method returned by Ogone.</summary>
        public string PaymentMethod { get; set; }

        /// <summary>Gets or sets the provider acceptance value.</summary>
        public string Acceptance { get; set; }

        /// <summary>Gets or sets the numeric provider transaction status.</summary>
        public int Status { get; set; }

        /// <summary>Gets or sets the card number value returned by the provider.</summary>
        public string CardNumber { get; set; }

        /// <summary>Gets or sets the provider payment identifier.</summary>
        public string PayId { get; set; }

        /// <summary>Gets or sets the provider error value.</summary>
        public string NcError { get; set; }

        /// <summary>Gets or sets the payment brand returned by the provider.</summary>
        public string Brand { get; set; }

        /// <summary>Gets or sets the SHA signature returned by the provider.</summary>
        public string ShaSignature { get; set; }
    }
}
