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
using System.Linq;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.Orders
{
    public class RecurringBilling
    {
        #region Fields

        private readonly LineItem _li;

        #endregion

        #region Constructor

        public RecurringBilling(LineItem li)
        {
            _li = li;
            StartsOn = DateTime.Now; // TODO : we should get subscription creation date here
        }

        #endregion

        #region Public methods

        public void LoadPaymentInfo(HotcakesApplication app)
        {
            var o = app.OrderServices.Orders.FindForCurrentStore(_li.OrderBvin);
            var payMan = new OrderPaymentManager(o, app);

            var payments = payMan.RecurringPaymentsGetByLineItem(_li.Id);

            TotalPayed = payments.Sum(t => t.Amount);

            var paymentsCount = payments.Count;

            StartsOn = o.TimeOfOrderUtc;
            if (IntervalType == RecurringIntervalType.Days)
            {
                NextPaymentDate = StartsOn.AddDays(Interval*paymentsCount);
            }
            else if (IntervalType == RecurringIntervalType.Months)
            {
                NextPaymentDate = StartsOn.AddMonths(Interval*paymentsCount);
            }
        }

        #endregion

        #region Properties

        public DateTime StartsOn { get; set; }

        /// <summary>
        ///     Defines how often payments will be done
        /// </summary>
        public int Interval { get; set; }

        /// <summary>
        ///     Defines period type of recurring payments
        /// </summary>
        public RecurringIntervalType IntervalType { get; set; }

        /// <summary>
        ///     Gets or sets the next payment date.
        /// </summary>
        /// <value>
        ///     The next payment date.
        /// </value>
        public DateTime NextPaymentDate { get; set; }

        /// <summary>
        ///     Gets or sets the total payed.
        /// </summary>
        /// <value>
        ///     The total payed.
        /// </value>
        public decimal TotalPayed { get; set; }

        /// <summary>
        ///     Gets or sets a value indicating whether this instance is canceled.
        /// </summary>
        /// <value>
        ///     <c>true</c> if this instance is canceled; otherwise, <c>false</c>.
        /// </value>
        public bool IsCancelled { get; set; }

        #endregion

        #region Implementation

        #endregion
    }
}