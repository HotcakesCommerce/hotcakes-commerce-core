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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Globalization;

namespace Hotcakes.Commerce.Orders
{
	public class OrderStatusCode
	{
        public const string Cancelled = "A7FFDB90-C566-4cf2-93F4-D42367F359D5";
        public const string OnHold = "88B5B4BE-CA7B-41a9-9242-D96ED3CA3135";
        public const string Received = "F37EC405-1EC6-4a91-9AC4-6836215FBBBC";
        public const string ReadyForPayment = "e42f8c28-9078-47d6-89f8-032c9a6e1cce";
        public const string ReadyForShipping = "0c6d4b57-3e46-4c20-9361-6b0e5827db5a";
        public const string Completed = "09D7305D-BD95-48d2-A025-16ADC827582A";
		//public const string SessionTimeout = "C3965268-7AEE-4627-8C27-3CAE2965EA29";
        public string Bvin { get; set; }
        public DateTime LastUpdated { get; set; }
		public string StatusName {get;set;}
		public bool SystemCode {get;set;}
		public int SortOrder {get;set;}

		public string StatusDisplayName
		{
			get
			{
				return LocalizationUtils.GetOrderStatus(StatusName, HccRequestContext.Current.MainContentCulture);
			}
		}

		public OrderStatusCode()
		{
            Bvin = string.Empty;
            LastUpdated = DateTime.UtcNow;
            StatusName = string.Empty;
            SystemCode = false;
            SortOrder = 0;
		}
	
        public static List<OrderStatusCode> FindAll()
        {
            var result = new List<OrderStatusCode>();

            result.Add(new OrderStatusCode
            {
                Bvin = "A7FFDB90-C566-4cf2-93F4-D42367F359D5",
                SystemCode = true,
                StatusName = "Cancelled",
                SortOrder = 0
            });
            result.Add(new OrderStatusCode
            {
                Bvin = "88B5B4BE-CA7B-41a9-9242-D96ED3CA3135",
                SystemCode = true,
                StatusName = "On Hold",
                SortOrder = 1
            });
            result.Add(new OrderStatusCode
            {
                Bvin = "F37EC405-1EC6-4a91-9AC4-6836215FBBBC",
                SystemCode = true,
                StatusName = "Received",
                SortOrder = 2
            });
            result.Add(new OrderStatusCode
            {
                Bvin = "e42f8c28-9078-47d6-89f8-032c9a6e1cce",
                SystemCode = true,
                StatusName = "Ready for Payment",
                SortOrder = 3
            });
            result.Add(new OrderStatusCode
            {
                Bvin = "0c6d4b57-3e46-4c20-9361-6b0e5827db5a",
                SystemCode = true,
                StatusName = "Ready for Shipping",
                SortOrder = 5
            });
            result.Add(new OrderStatusCode
            {
                Bvin = "09D7305D-BD95-48d2-A025-16ADC827582A",
                SystemCode = true,
                StatusName = "Complete",
                SortOrder = 6
            });

            return result;
        }

        public static OrderStatusCode FindByBvin(string bvin)
        {
            return FindAll().FirstOrDefault(os => os.Bvin == bvin);
        }
	}
}
