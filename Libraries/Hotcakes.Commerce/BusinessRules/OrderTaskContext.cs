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
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Provides order workflow state together with the active commerce request context.</summary>
    /// <remarks>Previous status values are stored in the inherited <see cref="TaskContext.Inputs"/> collection under the <c>hcc</c> namespace.</remarks>
    public class OrderTaskContext : TaskContext
    {
        /// <summary>Initializes the context from <see cref="HccRequestContext.Current"/>.</summary>
        public OrderTaskContext()
            : this(HccRequestContext.Current)
        {
        }

        /// <summary>Initializes the context with a specific commerce request context.</summary>
        /// <param name="requestContext">The request context that defines store and request scope.</param>
        public OrderTaskContext(HccRequestContext requestContext)
        {
            RequestContext = requestContext;
        }

        /// <summary>Gets or sets the order payment status from before the current workflow operation.</summary>
        /// <value>The previous payment status stored in <see cref="TaskContext.Inputs"/>.</value>
        public OrderPaymentStatus PreviousPaymentStatus
        {
            get { return (OrderPaymentStatus) Inputs.GetPropertyAsInt("hcc", "PreviousPaymentStatus"); }
            set { Inputs.SetProperty("hcc", "PreviousPaymentStatus", (int) value); }
        }

        /// <summary>Gets or sets the order status code from before the current workflow operation.</summary>
        /// <value>The previous status code stored in <see cref="TaskContext.Inputs"/>.</value>
        public string PreviousOrderStatusCode
        {
            get { return Inputs.GetProperty("hcc", "PreviousOrderStatusCode"); }
            set { Inputs.SetProperty("hcc", "PreviousOrderStatusCode", value); }
        }

        /// <summary>Gets or sets the order being processed.</summary>
        /// <value>The order in the current store and request scope.</value>
        public Order Order { get; set; }

        /// <summary>Gets or sets the commerce request context used by this workflow.</summary>
        /// <value>The request context used to resolve the current store and application services.</value>
        public HccRequestContext RequestContext { get; set; }

        /// <summary>Gets a commerce application facade bound to <see cref="RequestContext"/>.</summary>
        /// <value>A newly constructed <see cref="HotcakesApplication"/> for the current request context.</value>
        public HotcakesApplication HccApp
        {
            get { return new HotcakesApplication(RequestContext); }
        }
    }
}
