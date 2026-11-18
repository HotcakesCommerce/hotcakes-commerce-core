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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.BusinessRules;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.NoCms
{
    /// <summary>
    /// Creates a <see cref="CustomerAccount"/> for an order when the customer does not already have an account.
    /// </summary>
    public class CreateUserAccountForNewCustomer : OrderTask
    {
        /// <summary>
        /// Finds an existing customer by the order email address or creates a new customer account from the order details.
        /// </summary>
        /// <param name="context">The workflow context containing the order, commerce application, and request information.</param>
        /// <returns><see langword="true"/> after the existing or newly created customer identifier has been assigned to <see cref="OrderTaskContext.UserId"/>.</returns>
        /// <remarks>
        /// New accounts receive a generated password whose length is based on <see cref="WebAppSettings.PasswordMinimumLength"/>,
        /// with a minimum of eight characters. When the forgot-password template is available, the generated password is sent
        /// to the new customer through the configured mail service.
        /// </remarks>
        public override bool Execute(OrderTaskContext context)
        {
            var u = context.HccApp.MembershipServices.Customers.FindByEmail(context.Order.UserEmail).FirstOrDefault();
            if (u != null)
            {
                if (u.Bvin != string.Empty)
                {
                    context.UserId = u.Bvin;
                    return true;
                }
            }

            var n = new CustomerAccount();
            n.Email = context.Order.UserEmail;
            var length = WebAppSettings.PasswordMinimumLength;
            if (length < 8) length = 8;
            var newPassword = PasswordGenerator.GeneratePassword(length);
            n.Password = newPassword;
            n.FirstName = context.Order.ShippingAddress.FirstName;
            n.LastName = context.Order.ShippingAddress.LastName;

            if (context.HccApp.MembershipServices.CreateCustomer(n, n.Password))
            {
                // Update Addresses for Customer
                context.Order.BillingAddress.CopyTo(n.BillingAddress);
                context.Order.ShippingAddress.CopyTo(n.ShippingAddress);
                context.HccApp.MembershipServices.UpdateCustomer(n);
                context.Order.CustomProperties.Add("hcc", "allowpasswordreset", "1");

                // Email Password to Customer
                var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context.RequestContext,
                    context.Order.UsedCulture);
                var contentService = Factory.CreateService<ContentService>(hccRequestContext);
                var t = contentService.GetHtmlTemplateOrDefault(HtmlTemplateType.ForgotPassword);
                if (t != null)
                {
                    var replacers = new List<IReplaceable>();
                    replacers.Add(n);
                    replacers.Add(new Replaceable("[[NewPassword]]", newPassword));
                    t = t.ReplaceTagsInTemplate(hccRequestContext, replacers);

                    var m = t.ConvertToMailMessage(n.Email);
                    if (!MailServices.SendMail(m, hccRequestContext.CurrentStore))
                    {
                        EventLog.LogEvent("Create Account During Checkout",
                            "Failed to send email to new customer " + n.Email, EventLogSeverity.Warning);
                    }
                }
            }
            context.UserId = n.Bvin;

            return true;
        }

        /// <summary>
        /// Performs the workflow rollback operation for this task.
        /// </summary>
        /// <param name="context">The workflow context for the task being rolled back.</param>
        /// <returns><see langword="true"/> because this task does not undo the customer account operation.</returns>
        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        /// <summary>
        /// Gets the stable identifier used to register this workflow task.
        /// </summary>
        /// <returns>The task identifier <c>1755C649-4C16-41A6-B5AE-5259067FFF0E</c>.</returns>
        public override string TaskId()
        {
            return "1755C649-4C16-41A6-B5AE-5259067FFF0E";
        }

        /// <summary>
        /// Gets the display name of this workflow task.
        /// </summary>
        /// <returns><c>Create User Account for New Customer</c>.</returns>
        public override string TaskName()
        {
            return "Create User Account for New Customer";
        }

        /// <summary>
        /// Gets the display name of the workflow step.
        /// </summary>
        /// <returns><c>Create User Account for New Customer</c>, falling back to <see cref="TaskName"/> if the configured name is empty.</returns>
        public override string StepName()
        {
            var result = string.Empty;
            result = "Create User Account for New Customer";
            if (result == string.Empty)
            {
                result = TaskName();
            }
            return result;
        }

        /// <summary>
        /// Creates an independent instance of this workflow task.
        /// </summary>
        /// <returns>A new <see cref="CreateUserAccountForNewCustomer"/> instance.</returns>
        public override Task Clone()
        {
            return new CreateUserAccountForNewCustomer();
        }
    }
}
