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

using Hotcakes.Commerce.Orders;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class GiftCertificatesStatusUpdate : OrderTask
    {
        #region Fields

        private HotcakesApplication _app;

        #endregion

        #region Constructor

        public GiftCertificatesStatusUpdate(Mode mode)
        {
            TaskMode = mode;
        }

        #endregion

        #region Properties

        private Mode TaskMode { get; set; }

        #endregion

        #region Public Methods

        public override bool Execute(OrderTaskContext context)
        {
            _app = context.HccApp;

            switch (TaskMode)
            {
                case Mode.OrderStatusChanged:
                    HandleOrderStatusChanged(context);
                    break;
                case Mode.PaymentChanged:
                    HandlePaymentChanged(context);
                    break;
                default:
                    break;
            }

            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskName()
        {
            return "Disable Gift Cards When Payment Removed";
        }

        public override string TaskId()
        {
            return "BBB982F8-E966-4D5A-A370-615247EC1ADB";
        }

        public override Task Clone()
        {
            return new GiftCertificatesStatusUpdate(TaskMode);
        }

        #endregion

        #region Implementation

        private void HandleOrderStatusChanged(OrderTaskContext context)
        {
            if (context.PreviousOrderStatusCode != OrderStatusCode.Cancelled
                && context.Order.StatusCode == OrderStatusCode.Cancelled)
            {
                _app.OrderServices.UpdateGiftCardsStatus(context.Order, false, _app);
            }
            else if (context.PreviousOrderStatusCode == OrderStatusCode.Cancelled
                     && context.Order.StatusCode != OrderStatusCode.Cancelled
                     && context.Order.PaymentStatus >= OrderPaymentStatus.Paid)
            {
                _app.OrderServices.UpdateGiftCardsStatus(context.Order, true, _app);
            }
        }

        private void HandlePaymentChanged(OrderTaskContext context)
        {
            if (context.PreviousPaymentStatus >= OrderPaymentStatus.Paid
                && context.Order.PaymentStatus < OrderPaymentStatus.Paid)
            {
                _app.OrderServices.UpdateGiftCardsStatus(context.Order, false, _app);
            }
            else if (context.PreviousPaymentStatus < OrderPaymentStatus.Paid
                     && context.Order.PaymentStatus >= OrderPaymentStatus.Paid)
            {
                _app.OrderServices.UpdateGiftCardsStatus(context.Order, true, _app);
            }
        }

        public enum Mode
        {
            OrderStatusChanged,
            PaymentChanged
        }

        #endregion
    }
}