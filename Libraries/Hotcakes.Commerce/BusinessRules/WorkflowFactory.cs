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

using Hotcakes.Commerce.BusinessRules.OrderTasks;

namespace Hotcakes.Commerce.BusinessRules
{
    public class WorkflowFactory
    {
        public virtual Workflow CreateWorkflow(WorkflowNames name)
        {
            Task[] tasks = null;

            switch (name)
            {
                case WorkflowNames.VerifyOrderSize:
                    tasks = LoadVerifyOrderSizeTasks();
                    break;
                case WorkflowNames.DropShip:
                    tasks = LoadDropShipTasks();
                    break;
                case WorkflowNames.OrderEdited:
                    tasks = LoadOrderEditedTasks();
                    break;
                case WorkflowNames.OrderStatusChanged:
                    tasks = LoadOrderStatusChangedTasks();
                    break;
                case WorkflowNames.PackageShipped:
                    tasks = LoadPackageShippedTasks();
                    break;
                case WorkflowNames.PaymentChanged:
                    tasks = LoadPaymentChangedTasks();
                    break;
                case WorkflowNames.PaymentComplete:
                    tasks = LoadPaymentCompleteTasks();
                    break;
                case WorkflowNames.ProcessNewOrder:
                    tasks = LoadProcessNewOrderTasks();
                    break;
                case WorkflowNames.ProcessNewOrderPayments:
                    tasks = LoadProcessNewOrderPaymentsTasks();
                    break;
                case WorkflowNames.ProcessNewOrderAfterPayments:
                    // After Payments, notify customer, etc.
                    tasks = LoadProcessNewOrderAfterPaymentsTasks();
                    break;
                case WorkflowNames.ShippingChanged:
                    tasks = LoadShippingChangedTasks();
                    break;
                case WorkflowNames.ShippingComplete:
                    tasks = LoadShippingCompleteTasks();
                    break;
                case WorkflowNames.ThirdPartyCheckoutSelected:
                    tasks = LoadThirdPartyCheckoutSelectedTasks();
                    break;
                case WorkflowNames.ProcessNewReturn:
                    // TODO: Fill in return tasks here
                    break;
            }

            return new Workflow(tasks);
        }

        /// <summary>
        ///     This is the workflow that runs on an order whenever your customer checks out.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadVerifyOrderSizeTasks()
        {
            return new Task[]
            {
                new ApplyMinimumOrderAmount(),
                new CheckForOrderMaximums()
            };
        }

        /// <summary>
        ///     This is the workflow that runs whenever an order is marked as shipped.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadDropShipTasks()
        {
            return new Task[]
            {
                new RunAllDropShipWorkflows()
            };
        }

        /// <summary>
        ///     This is the workflow that runs after a package is marked shipped.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadPackageShippedTasks()
        {
            return new Task[]
            {
                new EmailShippingInfo()
            };
        }

        /// <summary>
        ///     Whenever an order is edited, this workflow is run. Add a new array of Task[] in order to add your custom code to
        ///     the workflow that is run here.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadOrderEditedTasks()
        {
            return new Task[]
            {
                new TaxProviderCancel(),
                new RunPaymentChangedWorkflow(),
                new UpdateOrder()
            };
        }

        /// <summary>
        ///     This workflow will run when the order status is changed. Currently, it checks to synchronize gift card values.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadOrderStatusChangedTasks()
        {
            return new Task[]
            {
                new GiftCertificatesStatusUpdate(GiftCertificatesStatusUpdate.Mode.OrderStatusChanged)
            };
        }

        /// <summary>
        ///     This workflow is run whenever the payment method for an order is changed.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadPaymentChangedTasks()
        {
            return new Task[]
            {
                new RunWorkFlowIfPaid(),
                new MarkCompletedWhenShippedAndPaid(),
                new ChangeOrderStatusWhenPaymentRemoved(),
                new TaxProviderCancelWhenPaymentRemoved(),
                new GiftCertificatesStatusUpdate(GiftCertificatesStatusUpdate.Mode.PaymentChanged),
                new UpdateOrder()
            };
        }

        /// <summary>
        ///     This workflow is run after payment has been applied. This is a great place to run workflow tasks that should not
        ///     run until after a valid payment has been applied to the order.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadPaymentCompleteTasks()
        {
            return new Task[]
            {
                new UpdateOrder(),
                new TaxProviderCommitTaxes(),
                new UpdateOrder(),
                new EmailVATInvoice(),
                new IssueGiftCertificates(),
                new IssueRewardsPoints(),
                new RunAllDropShipWorkflows()
            };
        }

        /// <summary>
        ///     This is the workflow that runs whenever an order is moved from the "New" state to the "TODO" state.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadProcessNewOrderTasks()
        {
            return new Task[]
            {
                new WorkflowNote("Starting Process Order Workflow"),
                new UpdateOrder(),
                new CheckForZeroDollarOrders(),
                new CreateUserAccountForNewCustomer(),
                new AssignOrderToUser(),
                new AssignOrderNumber(),
                new MakeOrderAddressUsersCurrentAddress(),
                new AddUserAddressesToAddressBook(),
                new UpdateLineItemsForSave(),
                new UpdateOrder(),
                new MakePlacedOrder(),
                new WorkflowNote("Finished Process Order Workflow"),
                new UpdateOrder()
            };
        }

        /// <summary>
        ///     This workflow is run after a new order has been accepted to process payments. This is where PayPal Express is
        ///     applied as well as credit cards and reward points.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadProcessNewOrderPaymentsTasks()
        {
            // Receive Payments and throw error if needed
            return new Task[]
            {
                //new DebitGiftCertificates(),
                new WorkflowNote("Starting Process Payment Workflow"),
                new UpdateOrder(),
                new ReceiveRewardsPoints(),
                new AuthorizeGiftCards(),
                new ReceiveCreditCards(),
                new ReceivePaypalExpressPayments(),
                new CreateRecurringSubscriptions(),
                new WorkflowNote("Finished Process Payment Workflow"),
                new UpdateOrder()
            };
        }

        /// <summary>
        ///     This workflow is run after the payments have been applied.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadProcessNewOrderAfterPaymentsTasks()
        {
            return new Task[]
            {
                new WorkflowNote("Starting Order After Payment Workflow"),
                new UpdateOrder(),
                new LocalFraudCheck(),
                new MarkCompletedWhenShippedAndPaid(),
                new EmailOrder("Customer"),
                new EmailOrder("Admin"),
                new WorkflowNote("Finished Order After Payment Workflow"),
                new UpdateOrder()
            };
        }

        /// <summary>
        ///     This workflow is run after the status of shipping changes.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadShippingChangedTasks()
        {
            return new Task[]
            {
                new MarkCompletedWhenShippedAndPaid(),
                new ChangeOrderStatusWhenShipmentRemoved(),
                new UpdateOrder()
                //new RunShippingCompleteWorkFlow()
            };
        }

        /// <summary>
        ///     This workflow is run after shipping is marked as complete. This is a great place to add tasks that should happen
        ///     after an order is shipped like those related to external
        ///     ERP inventory integration.
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadShippingCompleteTasks()
        {
            return new Task[]
            {
                new EmailShippingInfo()
            };
        }

        /// <summary>
        ///     This workflow is run to execute external checkout providers, meaning those not run directly against an API.
        ///     To-date, this is only PayPal Express?
        /// </summary>
        /// <returns></returns>
        protected virtual Task[] LoadThirdPartyCheckoutSelectedTasks()
        {
            return new Task[]
            {
                new StartPaypalExpressCheckout(),
                new StartMonerisCheckout(),
                new StartOgoneCheckout()
            };
        }
    }
}