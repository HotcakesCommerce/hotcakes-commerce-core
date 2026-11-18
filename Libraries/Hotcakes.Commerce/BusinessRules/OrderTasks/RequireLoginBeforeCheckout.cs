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

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// Redirects an anonymous checkout request to the login page.
    /// </summary>
    /// <remarks>
    /// The task identifier is <c>141375d5-6dcd-4ab1-8f2f-515b2d5c5778</c>. The redirect target is
    /// <c>~/login.aspx?ReturnTo=Checkout</c>. Redirect exceptions are logged and do not fail the workflow;
    /// rollback has no effect.
    /// </remarks>
    public class RequireLoginBeforeCheckout : OrderTask
    {
        /// <summary>
        /// Creates a new login-requirement task.
        /// </summary>
        /// <returns>A new <see cref="RequireLoginBeforeCheckout"/> instance.</returns>
        public override Task Clone()
        {
            return new RequireLoginBeforeCheckout();
        }

        /// <summary>
        /// Redirects to checkout login when the workflow user identifier is empty.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> containing the current request.</param>
        /// <returns>Always <see langword="true"/>; redirect exceptions are logged.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            try
            {
                if (context.UserId.Trim() == string.Empty)
                {
                    if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
                    {
                        context.HccApp.CurrentRequestContext.RoutingContext.HttpContext.Response.Redirect(
                            "~/login.aspx?ReturnTo=Checkout");
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
            return true;
        }

        /// <summary>
        /// Does not undo a redirect or otherwise change the request.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the stable task identifier used to register this task in workflows.
        /// </summary>
        /// <returns><c>141375d5-6dcd-4ab1-8f2f-515b2d5c5778</c>.</returns>
        public override string TaskId()
        {
            return "141375d5-6dcd-4ab1-8f2f-515b2d5c5778";
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Require Login Before Checkout</c>.</returns>
        public override string TaskName()
        {
            return "Require Login Before Checkout";
        }
    }
}
