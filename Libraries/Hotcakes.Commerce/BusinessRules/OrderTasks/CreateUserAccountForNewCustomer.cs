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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Content;
using Hotcakes.Commerce.Membership;
using Hotcakes.Commerce.Utilities;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.BusinessRules.OrderTasks
{
    public class CreateUserAccountForNewCustomer : OrderTask
    {
        protected bool _isCreateGuestAccount;

        protected virtual bool RequiresUniqueEmail
        {
            get { return true; }
        }

        public override bool Execute(OrderTaskContext context)
        {
            try
            {
                var existAccount =
                    context.HccApp.MembershipServices.Customers.Find(context.UserId);

                if (existAccount == null)
                {
                    var username = context.Inputs.GetProperty("hcc", "RegUsername");
                    var password = context.Inputs.GetProperty("hcc", "RegPassword");
                    _isCreateGuestAccount = string.IsNullOrEmpty(username);

                    if (_isCreateGuestAccount && RequiresUniqueEmail)
                    {
                        existAccount =
                            context.HccApp.MembershipServices.Customers.FindByEmail(context.Order.UserEmail)
                                .FirstOrDefault();
                    }

                    if (existAccount == null)
                    {
                        var account = new CustomerAccount();

                        if (_isCreateGuestAccount)
                        {
                            var length = WebAppSettings.PasswordMinimumLength;
                            account.Password = PasswordGenerator.GeneratePassword(length);
                        }
                        else
                        {
                            account.Username = username;
                            account.Password = password;
                        }

                        account.Email = context.Order.UserEmail;

                        if (context.Order.HasShippingItems)
                        {
                            account.FirstName = context.Order.ShippingAddress.FirstName;
                            account.LastName = context.Order.ShippingAddress.LastName;
                        }
                        else
                        {
                            account.FirstName = context.Order.BillingAddress.FirstName;
                            account.LastName = context.Order.BillingAddress.LastName;
                        }

                        CreateAccount(context, account);

                        context.UserId = account.Bvin;
                    }
                }

                if (existAccount != null)
                {
                    context.UserId = existAccount.Bvin;
                }
                return true;
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
                return true;
            }
        }

        protected virtual void CreateAccount(OrderTaskContext context, CustomerAccount n)
        {
            try
            {
                if (context.HccApp.MembershipServices.CreateCustomer(n, n.Password))
                {
                    // Update Addresses for Customer
                    context.Order.BillingAddress.CopyTo(n.BillingAddress);
                    context.Order.ShippingAddress.CopyTo(n.ShippingAddress);
                    context.HccApp.MembershipServices.UpdateCustomer(n);

                    if (_isCreateGuestAccount)
                    {
                        context.Order.CustomProperties.Add("hcc", "allowpasswordreset", "1");
                    }

                    // Email Password to Customer
                    EmailPasswordToCustomer(context, n);
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }

        protected void EmailPasswordToCustomer(OrderTaskContext context, CustomerAccount n)
        {
            try
            {
                var hccRequestContext = HccRequestContextUtils.GetContextWithCulture(context.RequestContext,
                    context.Order.UsedCulture);
                var contentService = Factory.CreateService<ContentService>(hccRequestContext);
                var t = context.HccApp.ContentServices.GetHtmlTemplateOrDefault(HtmlTemplateType.ForgotPassword);

                if (t != null)
                {
                    var replacers = new List<IReplaceable>();
                    replacers.Add(n);
                    replacers.Add(new Replaceable("[[NewPassword]]", n.Password));
                    t = t.ReplaceTagsInTemplate(hccRequestContext, replacers);

                    var m = t.ConvertToMailMessage(n.Email);

                    if (!MailServices.SendMail(m, hccRequestContext.CurrentStore))
                    {
                        EventLog.LogEvent("Create Account During Checkout",
                            "Failed to send email to new customer " + n.Email, EventLogSeverity.Warning);
                    }
                }
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }


        public override bool Rollback(OrderTaskContext context)
        {
            return true;
        }

        public override string TaskId()
        {
            return "1755C649-4C16-41A6-B5AE-5259067FFF0E";
        }

        public override string TaskName()
        {
            return "Create User Account for New Customer";
        }

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

        public override Task Clone()
        {
            return new CreateUserAccountForNewCustomer();
        }
    }
}