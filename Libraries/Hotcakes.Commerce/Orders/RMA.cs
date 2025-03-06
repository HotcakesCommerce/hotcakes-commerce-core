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
using System.Collections.Generic;
using System.Linq;

namespace Hotcakes.Commerce.Orders
{
    /// <summary>
    ///     This is the primary object that is used to manage all aspects of RMA.
    /// </summary>
    [Serializable]
    public class RMA
    {
        private RMAStatus _status = RMAStatus.Unsubmitted;

        public RMA()
        {
            StoreId = 0;
            Bvin = string.Empty;
            LastUpdatedUtc = DateTime.UtcNow;
            OrderBvin = string.Empty;
            OrderNumber = string.Empty;
            Name = string.Empty;
            EmailAddress = string.Empty;
            PhoneNumber = string.Empty;
            Comments = string.Empty;
            Number = -1;
            DateOfReturnUtc = DateTime.UtcNow;
            Items = new List<RMAItem>();
        }

        /// <summary>
        ///     This is the ID of the Hotcakes store. Typically, this is 1, except in multi-tenant environments.
        /// </summary>
        public long StoreId { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the RMA
        /// </summary>
        public string Bvin { get; set; }

        /// <summary>
        ///     The last updated date is used for auditing purposes to know when the RMA was last updated.
        /// </summary>
        public DateTime LastUpdatedUtc { get; set; }

        /// <summary>
        ///     The unique ID or bvin of the order that this RMA is related to.
        /// </summary>
        public string OrderBvin { get; set; }

        /// <summary>
        ///     The unique order number that merchants see in the store admin.
        /// </summary>
        /// <remarks>
        ///     Order number is not automatically loaded, set it manually
        /// </remarks>
        public string OrderNumber { get; set; }

        /// <summary>
        ///     The name of the customer submitting the RMA.
        /// </summary>
        /// <remarks>
        ///     This value is not really used in the application at this time.
        /// </remarks>
        public string Name { get; set; }

        /// <summary>
        ///     The email address of the customer submitting the RMA>
        /// </summary>
        /// <remarks>
        ///     This value is not really used in the application at this time.
        /// </remarks>
        public string EmailAddress { get; set; }

        /// <summary>
        ///     The phone number of the customer submitting the RMA.
        /// </summary>
        /// <remarks>
        ///     This value is not really used in the application at this time.
        /// </remarks>
        public string PhoneNumber { get; set; }

        /// <summary>
        ///     Useful comments about the RMA.
        /// </summary>
        /// <remarks>
        ///     This value is not really used in the application at this time.
        /// </remarks>
        public string Comments { get; set; }

        /// <summary>
        ///     Unique identifier of the RMA for merchants and customers to use.
        /// </summary>
        public int Number { get; set; }

        /// <summary>
        ///     The date/time stamp of the return of the line item(s).
        /// </summary>
        public DateTime DateOfReturnUtc { get; set; }

        /// <summary>
        ///     A collection of the line items that are part of this RMA.
        /// </summary>
        public List<RMAItem> Items { get; set; }

        /// <summary>
        ///     The status of the current RMA.
        /// </summary>
        public RMAStatus Status
        {
            get { return _status; }
            set
            {
                _status = value;
            }
        }

        /// <summary>
        ///     A human-readable version of the RMA status for display to merchants.
        /// </summary>
        public string StatusText
        {
            get
            {
                switch (Status)
                {
                    case RMAStatus.None:
                        return "None";
                    case RMAStatus.Closed:
                        return "Closed";
                    case RMAStatus.Open:
                        return "Open";
                    case RMAStatus.Pending:
                        return "Pending";
                    case RMAStatus.Unsubmitted:
                        return "Unsubmitted";
                    case RMAStatus.Rejected:
                        return "Rejected";
                    case RMAStatus.NeedRefund:
                        return "Issue Refund";
                    default:
                        return "None";
                }
            }
        }

        #region Refund Ammounts

        /// <summary>
        ///     Grand total amount to be refunded to the customer, including line item, shipping, tax, and gift wrap.
        /// </summary>
        public decimal TotalGrandRefundAmount
        {
            get
            {
                return TotalRefundItemAmount +
                       TotalRefundShippingAmount +
                       TotalRefundTaxAmount +
                       TotalRefundGiftWrapAmount;
            }
        }

        /// <summary>
        ///     Refund amount of the line item(s).
        /// </summary>
        public decimal TotalRefundItemAmount
        {
            get { return Items.Sum(y => y.RefundAmount); }
        }

        /// <summary>
        ///     Refund amount of the shipping for the line item(s).
        /// </summary>
        public decimal TotalRefundShippingAmount
        {
            get { return Items.Sum(y => y.RefundShippingAmount); }
        }

        /// <summary>
        ///     Refund amount of the taxes for the line item(s).
        /// </summary>
        public decimal TotalRefundTaxAmount
        {
            get { return Items.Sum(y => y.RefundTaxAmount); }
        }

        /// <summary>
        ///     Refund amount of the gift wrapping for the line item(s).
        /// </summary>
        public decimal TotalRefundGiftWrapAmount
        {
            get { return Items.Sum(y => y.RefundGiftWrapAmount); }
        }

        #endregion
    }
}