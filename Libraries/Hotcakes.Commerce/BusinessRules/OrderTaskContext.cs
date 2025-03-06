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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules
{
    public class OrderTaskContext : TaskContext
    {
        public OrderTaskContext()
            : this(HccRequestContext.Current)
        {
        }

        public OrderTaskContext(HccRequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        public OrderPaymentStatus PreviousPaymentStatus
        {
            get { return (OrderPaymentStatus) Inputs.GetPropertyAsInt("hcc", "PreviousPaymentStatus"); }
            set { Inputs.SetProperty("hcc", "PreviousPaymentStatus", (int) value); }
        }

        public string PreviousOrderStatusCode
        {
            get { return Inputs.GetProperty("hcc", "PreviousOrderStatusCode"); }
            set { Inputs.SetProperty("hcc", "PreviousOrderStatusCode", value); }
        }

        public Order Order { get; set; }

        public HccRequestContext RequestContext { get; set; }

        public HotcakesApplication HccApp
        {
            get { return new HotcakesApplication(RequestContext); }
        }
    }
}