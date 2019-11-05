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

using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Security;
using Hotcakes.Payment;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class LocalFraudCheck : OrderTask
    {
        public override Task Clone()
        {
            return new LocalFraudCheck();
        }

        public override bool Execute(OrderTaskContext context)
        {
            var result = true;

            if (context.Order != null)
            {
                var d = new FraudCheckData();
                PopulateFraudData(d, context);

                var scorer = new FraudScorer(context.HccApp.CurrentRequestContext);

                context.Order.FraudScore = scorer.ScoreData(d);

                if (context.Order.FraudScore >= 5)
                {
                    var s = OrderStatusCode.FindByBvin(OrderStatusCode.OnHold);
                    context.Order.StatusCode = s.Bvin;
                    context.Order.StatusName = s.StatusName;
                    context.HccApp.OrderServices.Orders.Update(context.Order);
                }

                if (d.Messages.Count > 0)
                {
                    var n = new OrderNote();
                    n.IsPublic = false;
                    n.Note = "Fraud Check Failed";
                    foreach (var m in d.Messages)
                    {
                        n.Note += " | " + m;
                    }
                    context.Order.Notes.Add(n);
                }

                context.HccApp.OrderServices.Orders.Update(context.Order);
            }

            return result;
        }

        private void PopulateFraudData(FraudCheckData d, OrderTaskContext context)
        {
            if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext != null)
            {
                if (context.HccApp.CurrentRequestContext.RoutingContext.HttpContext.Request.UserHostAddress != null)
                {
                    d.IpAddress =
                        context.HccApp.CurrentRequestContext.RoutingContext.HttpContext.Request.UserHostAddress;
                }
            }

            if (context.Order.UserEmail != string.Empty)
            {
                d.EmailAddress = context.Order.UserEmail;
                var parts = d.EmailAddress.Split('@');
                if (parts.Length > 1)
                {
                    d.DomainName = parts[1];
                }
            }

            d.PhoneNumber = context.Order.BillingAddress.Phone;

            foreach (var p in context.HccApp.OrderServices.Transactions.FindForOrder(context.Order.bvin))
            {
                if (p.Action == ActionType.CreditCardInfo)
                {
                    d.CreditCard = p.CreditCard.CardNumber;
                    break;
                }
            }
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "14D38D1A-8B26-4a8b-B143-8485B4E7A584";
        }

        public override string TaskName()
        {
            return "Local Fraud Check";
        }
    }
}