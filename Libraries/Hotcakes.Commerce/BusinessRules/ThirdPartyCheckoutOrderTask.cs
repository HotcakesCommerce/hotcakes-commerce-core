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
    public abstract class ThirdPartyCheckoutOrderTask : OrderTask
    {
        public abstract string PaymentMethodId { get; }

        public abstract bool ProcessCheckout(OrderTaskContext context);

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
