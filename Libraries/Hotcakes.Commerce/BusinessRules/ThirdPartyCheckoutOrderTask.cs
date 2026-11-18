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

namespace Hotcakes.Commerce.BusinessRules
{
    /// <summary>Base class for order tasks that hand checkout to an external payment provider.</summary>
    /// <remarks>Only the task whose <see cref="PaymentMethodId"/> matches the <c>MethodId</c> input processes checkout; other tasks return success without changing the order.</remarks>
    public abstract class ThirdPartyCheckoutOrderTask : OrderTask
    {
        /// <summary>Gets the payment method identifier handled by this task.</summary>
        /// <value>The exact identifier compared with the workflow input named <c>MethodId</c>.</value>
        public abstract string PaymentMethodId { get; }

        /// <summary>Starts checkout with the selected third-party provider.</summary>
        /// <param name="context">The order context containing the order and provider input.</param>
        /// <returns><c>true</c> when provider processing succeeds; otherwise, <c>false</c>.</returns>
        public abstract bool ProcessCheckout(OrderTaskContext context);

        /// <summary>Matches the selected provider, assigns an order number when needed, and starts checkout.</summary>
        /// <param name="context">The order context whose <c>MethodId</c> input selects this provider.</param>
        /// <returns>The result of <see cref="ProcessCheckout(OrderTaskContext)"/> for a matching provider; otherwise, <c>true</c>.</returns>
        /// <remarks>A missing order number is persisted before provider processing so external payment systems can display the invoice number.</remarks>
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Inputs["MethodId"] != null && context.Inputs["MethodId"].Value == PaymentMethodId)
            {
                context.Order.CustomProperties.Add("hcc", "MethodId", PaymentMethodId);

                // Assign the order number for PayPal or any third party payment to show under the Invoice ID. If this is not done then PayPal wont show this orders number in the business UI.
                if (string.IsNullOrEmpty(context.Order.OrderNumber))
                {
                    context.Order.OrderNumber = context.HccApp.OrderServices.GenerateNewOrderNumber(context.HccApp.CurrentRequestContext.CurrentStore.Id).ToString();
                    Hotcakes.Commerce.Orders.OrderNote orderNote = new Hotcakes.Commerce.Orders.OrderNote();
                    orderNote.IsPublic = false;
                    orderNote.Note = "This order was assigned number " + context.Order.OrderNumber;
                    context.Order.Notes.Add(orderNote);
                }

                context.HccApp.OrderServices.Orders.Update(context.Order);

                return ProcessCheckout(context);
            }
            return true;
        }
    }
}
