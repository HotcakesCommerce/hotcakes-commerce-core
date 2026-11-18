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

using System;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Dnn.Workflow
{
    /// <summary>
    /// Assigns membership roles for membership products purchased in an order.
    /// </summary>
    [Serializable]
    public class MembershipTask : OrderTask
    {
        private TextLogger _logger;

        protected TextLogger ExceptionLogger
        {
            get
            {
                if (_logger == null)
                {
                    _logger = new TextLogger();
                }

                return _logger;
            }
        }

        /// <summary>
        /// Processes the order's products and assigns their membership roles to the customer.
        /// </summary>
        /// <param name="context">The order task context containing the order and commerce services.</param>
        /// <returns><see langword="true"/> when all products are processed; otherwise, <see langword="false"/> when a product cannot be found.</returns>
        public override bool Execute(OrderTaskContext context)
        {
            var result = true;

            foreach (var orderItem in context.Order.Items)
            {
                var product = context.HccApp.CatalogServices.Products.FindWithCache(orderItem.ProductId);

                if (product != null)
                {
                    if (product.IsRecurring)
                    {
                        ProcessRecurringProduct(context, orderItem.Quantity, product);
                    }
                    else if (!product.IsBundle)
                    {
                        ProcessSingleProduct(context, orderItem.Quantity, product);
                    }
                    else
                    {
                        foreach (var bundledProduct in product.BundledProducts)
                        {
                            if (bundledProduct.BundledProduct == null)
                            {
                                // reload the bundled product objects if any are missing
                                EnsureBundledProducts(context, ref product);
                                break;
                            }
                        }

                        foreach (var bundledProduct in product.BundledProducts)
                        {
                            ProcessSingleProduct(context, bundledProduct.Quantity, bundledProduct.BundledProduct);
                        }
                    }
                }
                else
                {
                    result = false;
                }
            }

            return result;
        }

        private void ProcessSingleProduct(OrderTaskContext context, int quantity, Product product)
        {
            var membershipType = context.HccApp.CatalogServices.MembershipTypes.Find(product.ProductTypeId);

            if (membershipType != null)
            {
                var now = DateTime.Now;

                context.HccApp.MembershipServices.AssignMembershipRole(
                    context.Order.UserID,
                    membershipType,
                    membershipType.GetTimeSpan(now, quantity)
                    );
            }
        }

        private void ProcessRecurringProduct(OrderTaskContext context, int quantity, Product product)
        {
            var membershipType = context.HccApp.CatalogServices.MembershipTypes.Find(product.ProductTypeId);

            if (membershipType != null)
            {
                var now = DateTime.Now;

                context.HccApp.MembershipServices.AssignMembershipRole(context.Order.UserID,
                    membershipType,
                    product.GetRecurringSpan(now, quantity, true)
                    );
            }
        }

        /// <summary>
        /// Rolls back this task.
        /// </summary>
        /// <param name="context">The order task context for the operation.</param>
        /// <returns><see langword="true"/> because membership assignments are not reversed by this task.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the display name for this workflow task.
        /// </summary>
        /// <returns>The task name.</returns>
        public override string TaskName()
        {
            return "Associate user with membership role";
        }

        /// <summary>
        /// Gets the stable identifier for this workflow task.
        /// </summary>
        /// <returns>The task identifier.</returns>
        public override string TaskId()
        {
            return "293524A7-F084-48DB-9BCD-D34306FC4265";
        }

        /// <summary>
        /// Creates a new instance of this workflow task.
        /// </summary>
        /// <returns>A new <see cref="MembershipTask"/> instance.</returns>
        public override Task Clone()
        {
            return new MembershipTask();
        }

        /// <summary>
        /// This method is probably only a workaround. It resolves Issue #175, but the root cause and fix probably should be in the DAL.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="product"></param>
        private void EnsureBundledProducts(OrderTaskContext context, ref Product product)
        {
            // repo references
            var productRepo = new ProductRepository(context.HccApp.CurrentRequestContext);

            // iterate through each bundled product in the list
            foreach (var bundledProduct in product.BundledProducts)
            {
                try
                {
                    if (!string.IsNullOrEmpty(bundledProduct.BundledProductId))
                    {
                        bundledProduct.BundledProduct = productRepo.Find(bundledProduct.BundledProductId);
                    }
                }
                catch (Exception ex)
                {
                    ExceptionLogger.LogException(ex);
                }
            }
        }
    }
}
