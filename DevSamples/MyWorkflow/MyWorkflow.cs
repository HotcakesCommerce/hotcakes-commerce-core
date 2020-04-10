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

using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.BusinessRules.OrderTasks;
using Hotcakes.Commerce.Dnn.Workflow;
using MyCompany.MyWorkflow.Tasks;

namespace MyCompany.MyWorkflow
{
    /// <summary>
    ///     This is the MyWorkflow class. Developers should use this as a guide on how to integrate custom tasks or development
    ///     into
    ///     the normal flow of an order. Each of these workflows are executed by the payment workflow engine.
    ///     In order to inject your code at any point in this process, simply uncomment the override and update the steps to
    ///     include your code
    ///     at the appropriate time.
    /// </summary>
    public class MyWorkflow : DnnWorkflowFactory
    {
        /// <summary>
        ///     This is the workflow that runs whenever an order is moved from the "New" state to the "TODO" state.
        ///     Note that this is where we gave you an example - checkout the MySimpleTask and MyOrderTask tasks.
        /// </summary>
        /// <returns></returns>
        protected override Task[] LoadProcessNewOrderTasks()
        {
            return new Task[]
            {
                new WorkflowNote("Starting Process Order Workflow"),
                new UpdateOrder(),
                new CheckForZeroDollarOrders(),
                new DnnCreateUserAccountForNewCustomer(),
                new AssignOrderToUser(),
                new AssignOrderNumber(),
                new MakeOrderAddressUsersCurrentAddress(),
                new AddUserAddressesToAddressBook(),
                new UpdateLineItemsForSave(),
                new UpdateOrder(),
                new DnnMakePlacedOrder(),
                new WorkflowNote("Finished Process Order Workflow"),
                new UpdateOrder(),

                // NEW TASKS!!
                new MySimpleTask(),
                new MyOrderTask()
            };
        }


        ///// <summary>
        ///// This is the workflow that runs on an order whenever your customer checks out. 
        ///// Add your custom tasks anywhere in that process. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadVerifyOrderSizeTasks()
        //{
        //    return new Task[]{
        //                new ApplyMinimumOrderAmount(),
        //                new CheckForOrderMaximums()
        //            };
        //}

        ///// <summary>
        ///// This is the workflow that runs whenever an order is marked as shipped. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadDropShipTasks()
        //{
        //    return new Task[]{
        //                new RunAllDropShipWorkflows()
        //            };
        //}

        ///// <summary>
        ///// This is the workflow that runs after a package is marked shipped. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadPackageShippedTasks()
        //{
        //    return new Task[]{
        //        new EmailShippingInfo()
        //    };
        //}

        ///// <summary>
        ///// Whenever an order is edited, this workflow is run. Add a new array of Task[] in order to add your custom code to the workflow that is run here. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadOrderEditedTasks()
        //{
        //    return new Task[]{               
        //        new TaxProviderCancel(),
        //        new RunPaymentChangedWorkflow(),
        //        new UpdateOrder()
        //    };
        //}

        ///// <summary>
        ///// This workflow will run when the order status is changed. Currently, it checks to synchronize gift card values.
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadOrderStatusChangedTasks()
        //{
        //    return new Task[]{
        //        new GiftCertificatesStatusUpdate(GiftCertificatesStatusUpdate.Mode.OrderStatusChanged)
        //    };
        //}

        ///// <summary>
        ///// This workflow is run whenever the payment method for an order is changed. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadPaymentChangedTasks()
        //{
        //    return new Task[]{
        //        new RunWorkFlowIfPaid(),
        //        new MarkCompletedWhenShippedAndPaid(),
        //        new ChangeOrderStatusWhenPaymentRemoved(),        
        //        new TaxProviderCancelWhenPaymentRemoved(),
        //        new GiftCertificatesStatusUpdate(GiftCertificatesStatusUpdate.Mode.PaymentChanged),
        //        new UpdateOrder()
        //    };
        //}

        ///// <summary>
        ///// This workflow is run after payment has been applied. This is a great place to run workflow tasks that should not run until after a valid payment has been applied to the order. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadPaymentCompleteTasks()
        //{
        //    return new Task[]{
        //        new UpdateOrder(),
        //#pragma warning disable 0612, 0618
        //        new AvalaraCommitTaxes(),
        //#pragma warning restore 0612, 0618
        //        new TaxProviderCommitTaxes(),
        //        new UpdateOrder(),
        //        new EmailVATInvoice(),
        //        new IssueGiftCertificates(),
        //        new IssueRewardsPoints(),
        //        new RunAllDropShipWorkflows(),

        //        // Added to handle "membership" products
        //        new MembershipTask()
        //    };
        //}

        ///// <summary>
        ///// This workflow is run after a new order has been accepted to process payments. This is where PayPal Express is applied as well as credit cards and reward points. 
        ///// </summary>
        ///// <returns></returns>
        //protected override  Task[] LoadProcessNewOrderPaymentsTasks()
        //{
        //    return new Task[]{
        //        new WorkflowNote("Starting Process Order Workflow"),
        //        new UpdateOrder(),

        //        new CheckForZeroDollarOrders(),

        //        new CreateUserAccountForNewCustomer(),
        //        new AssignOrderToUser(),
        //        new AssignOrderNumber(),
        //        new MakeOrderAddressUsersCurrentAddress(),
        //        new AddUserAddressesToAddressBook(), 
        //        new UpdateLineItemsForSave(),
        //        new UpdateOrder(),
        //        new MakePlacedOrder(),
        //        new WorkflowNote("Finished Process Order Workflow"),
        //        new UpdateOrder()
        //    };
        //}

        ///// <summary>
        ///// This workflow is run after the payments have been applied.
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadProcessNewOrderAfterPaymentsTasks()
        //{
        //    return new Task[]{
        //                new WorkflowNote("Starting Order After Payment Workflow"),
        //                new UpdateOrder(),
        //                new LocalFraudCheck(),
        //                new MarkCompletedWhenShippedAndPaid(),
        //                new EmailOrder("Customer"),
        //                new EmailOrder("Admin"),
        //                new WorkflowNote("Finished Order After Payment Workflow"),
        //                new UpdateOrder()
        //            };
        //}

        ///// <summary>
        ///// This workflow is run after the status of shipping changes. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadShippingChangedTasks()
        //{
        //    return new Task[]{
        //                new MarkCompletedWhenShippedAndPaid(),
        //                new ChangeOrderStatusWhenShipmentRemoved(),
        //                new UpdateOrder(),
        //                new RunShippingCompleteWorkFlow()
        //            };
        //}

        ///// <summary>
        ///// This workflow is run after shipping is marked as complete. This is a great place to add tasks that should happen after an order is shipped like those related to external 
        ///// ERP inventory integration. 
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadShippingCompleteTasks()
        //{
        //    return new Task[]{
        //                new EmailShippingInfo()
        //            };
        //}

        ///// <summary>
        ///// This workflow is run to execute external checkout providers, meaning those not run directly against an API.  
        ///// To-date, this is only PayPal Express?
        ///// </summary>
        ///// <returns></returns>
        //protected override Task[] LoadThirdPartyCheckoutSelectedTasks()
        //{
        //    return new Task[]{
        //                new StartPaypalExpressCheckout(),
        //                new StartMonerisCheckout()
        //            };
        //}
    }
}