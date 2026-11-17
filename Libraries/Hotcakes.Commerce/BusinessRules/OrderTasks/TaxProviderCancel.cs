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

using Hotcakes.Commerce.Taxes.Providers;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    /// <summary>Cancels the tax document for the current order through the configured tax provider.</summary>
    /// <remarks>This is an <see cref="OrderTask"/> executed with an <see cref="OrderTaskContext"/>.</remarks>
    /// <remarks>No provider call is made when the store has no enabled tax provider.</remarks>
    public class TaxProviderCancel : OrderTask
    {
        /// <summary>Executes tax-document cancellation for the current order.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>Always <see langword="true"/> after optionally invoking the provider.</returns>
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

        /// <summary>Reports that tax cancellation has no local rollback implementation.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>Always <see langword="true"/>; an external provider cancellation is not reversed here.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>Gets the default display name for this task.</summary>
        /// <returns><c>Tax provider Cancel Taxes</c>.</returns>
        public override string TaskName()
        {
            return "Tax provider Cancel Taxes";
        }

        /// <summary>Gets a provider-specific display name when a context and provider are available.</summary>
        /// <param name="context">The order workflow context, or <see langword="null"/> for the default name.</param>
        /// <returns>The provider name followed by <c> Cancel Taxes</c>, or the default task name.</returns>
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

        /// <summary>Gets the stable workflow identifier for this task.</summary>
        /// <returns>The task ID <c>CE7E044D-47D8-410C-8F5C-7156C6C6ED51</c>.</returns>
        public override string TaskId()
        {
            return "CE7E044D-47D8-410C-8F5C-7156C6C6ED51";
        }

        /// <summary>Gets the step label using the context-sensitive task name.</summary>
        /// <param name="context">The order workflow context.</param>
        /// <returns>The value returned by <see cref="TaskName(OrderTaskContext)"/>.</returns>
        public override string StepName(OrderTaskContext context)
        {
            return TaskName(context);
        }

        /// <summary>Creates an independent tax-cancellation task instance.</summary>
        /// <returns>A new <see cref="TaxProviderCancel"/> instance.</returns>
        public override Task Clone()
        {
            return new TaxProviderCancel();
        }
    }
}
