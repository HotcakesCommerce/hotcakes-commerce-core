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

using System.Runtime.Serialization;

namespace Hotcakes.CommerceDTO.v1.Orders
{
    /// <summary>
    ///     Various statuses to reflect the current state of payments in an order.
    /// </summary>
    [DataContract]
    public enum OrderPaymentStatusDTO
    {
        /// <summary>
        ///     Unknown status - should not be used.
        /// </summary>
        [EnumMember] Unknown = 0,

        /// <summary>
        ///     The order has not yet received any payments.
        /// </summary>
        [EnumMember] Unpaid = 1,

        /// <summary>
        ///     The order has received part but not all of the payment required.
        /// </summary>
        [EnumMember] PartiallyPaid = 2,

        /// <summary>
        ///     The order has been paid in full.
        /// </summary>
        [EnumMember] Paid = 3,

        /// <summary>
        ///     The order has received more funds than necessary for the order.
        /// </summary>
        [EnumMember] Overpaid = 4
    }
}