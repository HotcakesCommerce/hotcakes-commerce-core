#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2016 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Taxes.Providers;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    [Obsolete("Obsolete in 1.10.0. Use TaxProviderCancelWhenPaymentRemoved instead")]
    public class AvalaraCancelWhenPaymentRemoved : OrderTask
    {
        public override bool Execute(OrderTaskContext context)
        {
            if (context.Order.PaymentStatus == OrderPaymentStatus.Unpaid)
            {
                var provider = TaxProviders.CurrentTaxProvider(context.HccApp.CurrentStore);

                if (provider != null && provider.ProviderId == TaxProviders.AvataxServiceId)
                {
                    provider.CancelTaxDocument(context.Order, context.HccApp.CurrentRequestContext);
                }
            }
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskName()
        {
            return "Avalara Cancel Taxes When Payment Removed";
        }

        public override string TaskId()
        {
            return "d10d9ffa-dda1-48ed-bfed-5a088cf685d7";
        }

        public override Task Clone()
        {
            return new AvalaraCancelWhenPaymentRemoved();
        }
    }
}