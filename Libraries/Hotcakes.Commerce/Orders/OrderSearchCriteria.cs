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

namespace Hotcakes.Commerce.Orders
{
	/// <summary>
	/// Represents the OrderSearchCriteria order-domain type and preserves the established public compatibility surface.
	/// </summary>
	public class OrderSearchCriteria
	{								
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public bool IsPlaced {get;set;}
		public bool? IsRecurring { get; set; }
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public string Keyword {get;set;}
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public string StatusCode {get;set;}
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public OrderPaymentStatus PaymentStatus {get;set;}
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public OrderShippingStatus ShippingStatus {get;set;}
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public DateTime EndDateUtc {get;set;}
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public DateTime StartDateUtc {get;set;}
		public long? AffiliateId {get;set;}
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public string OrderNumber {get;set;}
        /// <summary>
        /// The search criterion used when filtering orders.
        /// </summary>
        public bool SortDescending { get; set; }
		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public bool IsIncludeCanceledOrder { get; set; }

		/// <summary>
		/// The search criterion used when filtering orders.
		/// </summary>
		public bool IncludeUnplaced { get; set; }

		/// <summary>
		/// Provides the member operation while preserving the established order-domain behavior.
		/// </summary>
		public OrderSearchCriteria()
		{
            IsPlaced = true;
            Keyword = string.Empty;
            StatusCode = string.Empty;
            PaymentStatus = OrderPaymentStatus.Unknown;
            ShippingStatus = OrderShippingStatus.Unknown;
            StartDateUtc = new DateTime(1900, 1, 1);
		    EndDateUtc = new DateTime(3000, 1, 1);
            AffiliateId = null;
            OrderNumber = string.Empty;
            SortDescending = false;
			IsIncludeCanceledOrder = false;
			IncludeUnplaced = false;
		}

	}
}

