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
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Dnn.Workflow
{
    [Serializable]
    public class MembershipTask : OrderTask
    {
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

        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskName()
        {
            return "Associate user with membership role";
        }

        public override string TaskId()
        {
            return "293524A7-F084-48DB-9BCD-D34306FC4265";
        }

        public override Task Clone()
        {
            return new MembershipTask();
        }
    }
}