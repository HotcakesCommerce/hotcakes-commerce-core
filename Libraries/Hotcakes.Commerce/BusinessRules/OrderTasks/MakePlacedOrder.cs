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
using System.Collections.Generic;
using System.Web;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>
    /// Marks an order as placed, reserves inventory, records request metadata, and assigns the received status.
    /// </summary>
    /// <remarks>
    /// The task identifier is <c>8F2BB6B4-2FEF-406d-A62D-075CD74D2551</c>. An already placed order is unchanged;
    /// an order with no items reports the customer-visible <c>OrderAlreadyPlaced</c> error, while inventory failures
    /// report <c>Stock Too Low</c>. Rollback intentionally never unplaces an order.
    /// </remarks>
    public class MakePlacedOrder : OrderTask
    {
        /// <summary>
        /// Places the order, reserves inventory, records request information, and sets its status to received.
        /// </summary>
        /// <param name="context">The <see cref="OrderTaskContext"/> containing the order and request context.</param>
        /// <returns><see langword="false"/> when the order has no items or inventory cannot be reserved; otherwise <see langword="true"/>.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.IsPlaced)
                return true;
            if (context.Order.Items.Count == 0)
            {
                context.Errors.Add(new WorkflowMessage("Order already placed.",
                    GlobalLocalization.GetString("OrderAlreadyPlaced"), true));
                return false;
            }

            context.Order.IsPlaced = true;
            context.Order.TimeOfOrderUtc = DateTime.UtcNow;

            var errors = new List<string>();
            if (!context.HccApp.OrdersReserveInventoryForAllItems(context.Order, errors))
            {
                foreach (var item in errors)
                {
                    context.Errors.Add(new WorkflowMessage("Stock Too Low", item, true));
                }
                return false;
            }

            if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
            {
                var request = context.HccApp.CurrentRequestContext.RoutingContext.HttpContext.Request;
                var note = new OrderNote();
                note.IsPublic = false;
                note.Note = "Customer IP: " + request.UserHostAddress;
                note.Note += "<br> Customer Host: " + request.UserHostName;
                note.Note += "<br> Browser: " + request.UserAgent;
                context.Order.Notes.Add(note);

                context.Order.UserDeviceType = DetermineDeviceType(request);
            }

            var c = OrderStatusCode.FindByBvin(OrderStatusCode.Received);
            if (c != null)
            {
                var affiliateId = context.HccApp.ContactServices.GetCurrentAffiliateId();

                context.Order.StatusName = c.StatusName;
                context.Order.StatusCode = c.Bvin;
                context.Order.AffiliateID = affiliateId;

                if (affiliateId.HasValue)
                {
                    context.HccApp.ContactServices.UpdateProfileAffiliateId(affiliateId.Value);
                }
            }
            return true;
        }

        /// <summary>
        /// Determines the device classification recorded on the order for the current request.
        /// </summary>
        /// <param name="request">The current HTTP request.</param>
        /// <returns>The device classification; the base implementation returns <see cref="DeviceType.Desktop"/>.</returns>
        protected virtual DeviceType DetermineDeviceType(HttpRequestBase request)
        {
            return DeviceType.Desktop;
        }

        /// <summary>
        /// Intentionally does not unplace the order or undo inventory reservation.
        /// </summary>
        /// <param name="context">The workflow context.</param>
        /// <returns>Always <see langword="true"/>.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            // No Rollback for this. Never unplace an order
            return true;
        }

        /// <summary>
        /// Gets the stable task identifier used to register this task in workflows.
        /// </summary>
        /// <returns><c>8F2BB6B4-2FEF-406d-A62D-075CD74D2551</c>.</returns>
        public override string TaskId()
        {
            return "8F2BB6B4-2FEF-406d-A62D-075CD74D2551";
        }

        /// <summary>
        /// Gets the display name of this task.
        /// </summary>
        /// <returns><c>Make Placed Order</c>.</returns>
        public override string TaskName()
        {
            return "Make Placed Order";
        }

        /// <summary>
        /// Gets the workflow step name, which is the task name.
        /// </summary>
        /// <returns><c>Make Placed Order</c>.</returns>
        public override string StepName()
        {
            var result = string.Empty;
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        /// <summary>
        /// Creates a new order-placement task.
        /// </summary>
        /// <returns>A new <see cref="MakePlacedOrder"/> instance.</returns>
        public override Task Clone()
        {
            return new MakePlacedOrder();
        }
    }
}
