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

using System;
using DotNetNuke.Services.Exceptions;
using DotNetNuke.Services.Scheduling;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Accounts;
using Hotcakes.Commerce.Orders;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Modules.Core
{
    public class UpdateRecurringOrdersJob : SchedulerClient
    {
        public UpdateRecurringOrdersJob(ScheduleHistoryItem item)
        {
            ScheduleHistoryItem = item;
        }

        public override void DoWork()
        {
            try
            {
                Progressing();

                var context = new HccRequestContext();
                var accountServices = Factory.CreateService<AccountService>(context);
                var stores = accountServices.Stores.FindAllPaged(1, int.MaxValue);

                foreach (var store in stores)
                {
                    context.CurrentStore = store;
                    HccRequestContextUtils.UpdateUserContentCulture(context);

                    ProcessRecurringOrders(context);
                }

                //Show success
                ScheduleHistoryItem.Succeeded = true;
            }
            catch (Exception ex)
            {
                ScheduleHistoryItem.Succeeded = false;
                Errored(ref ex);
                Exceptions.LogException(ex);
            }
        }

        private void ProcessRecurringOrders(HccRequestContext context)
        {
            var criteria = new OrderSearchCriteria();
            criteria.IsRecurring = true;

            var orderRepo = Factory.CreateRepo<OrderRepository>(context);
            var orderSnapshots = orderRepo.FindByCriteria(criteria);

            foreach (var orderSnapshot in orderSnapshots)
            {
                // TODO: optimize to prevent getting and saving of subitems
                var order = orderRepo.FindForCurrentStore(orderSnapshot.bvin);
                order.EvaluateCurrentShippingStatus();

                orderRepo.Update(order, false);
            }
        }
    }
}