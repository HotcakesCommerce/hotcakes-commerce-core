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

using Hotcakes.Commerce.Taxes.Providers;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class TaxProviderCancel : OrderTask
    {
        public override bool Execute(OrderTaskContext context)
        {
            var taxProviderId = context.HccApp.CurrentStore.Settings.TaxProviderEnabled;

            if (!string.IsNullOrEmpty(taxProviderId))
            {
                var provider = TaxProviders.CurrentTaxProvider(context.HccApp.CurrentStore);
                provider.CancelTaxDocument(context.Order, context.HccApp.CurrentRequestContext);
            }
            return true;
        }

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskName()
        {
            return "Tax provider Cancel Taxes";
        }

        public override string TaskName(OrderTaskContext context)
        {
            if (context != null)
            {
                var taxProviderId = context.HccApp.CurrentStore.Settings.TaxProviderEnabled;

                if (!string.IsNullOrEmpty(taxProviderId))
                {
                    var provider = TaxProviders.CurrentTaxProvider(context.HccApp.CurrentStore);

                    return provider.ProviderName + " Cancel Taxes";
                }
                return "Tax provider Cancel Taxes";
            }
            return "Tax provider Cancel Taxes";
        }

        public override string TaskId()
        {
            return "CE7E044D-47D8-410C-8F5C-7156C6C6ED51";
        }

        public override string StepName(OrderTaskContext context)
        {
            return TaskName(context);
        }

        public override Task Clone()
        {
            return new TaxProviderCancel();
        }
    }
}