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

namespace Hotcakes.Commerce.Contacts
{
    /// <summary>Represents a payment recorded against an affiliate's earned commission.</summary>
    /// <remarks>Payments are associated with an affiliate and are used by affiliate payment reports.</remarks>
    public class AffiliatePayment
    {
        /// <summary>Gets or sets the unique payment identifier.</summary>
        public long Id { get; set; }

        /// <summary>Gets or sets the numeric identifier of the affiliate receiving the payment.</summary>
        /// <seealso cref="Affiliate"/>
        public long AffiliateId { get; set; }

        /// <summary>Gets or sets the amount paid to the affiliate.</summary>
        public decimal PaymentAmount { get; set; }

        /// <summary>Gets or sets the UTC date on which the payment was recorded.</summary>
        public DateTime PaymentDateUtc { get; set; }

        /// <summary>Gets or sets notes associated with the payment.</summary>
        public string Notes { get; set; }

        /// <summary>Gets or sets the name of the file associated with the payment record.</summary>
        public string FileName { get; set; }
    }
}
