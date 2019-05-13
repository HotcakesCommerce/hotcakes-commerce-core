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
using System.Web;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Membership;
using Hotcakes.Web;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.NoCms.Membership
{
    public class MvcMembershipService : MembershipServices
    {
        public MvcMembershipService(HccRequestContext context)
            : base(context)
        {
            Customers = Factory.CreateRepo<CustomerAccountRepository>(Context);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public MvcMembershipService(HccRequestContext c, bool isForMemoryOnly)
            : this(c)
        {
        }

        #endregion

        public CustomerAccount GetCustomer(string email)
        {
            var customer = Customers.FindByEmail(email);

            if (customer != null)
            {
                return customer.FirstOrDefault();
            }

            return null;
        }

        public override bool IsUserLoggedIn()
        {
            throw new NotImplementedException();
        }

        public override bool IsSuperUserLoggedIn()
        {
            throw new NotImplementedException();
        }

        public override bool HasCurrentUserPermission(string perm, HotcakesApplication app)
        {
            throw new NotImplementedException();
        }

        public override string GetCurrentUserId()
        {
            throw new NotImplementedException();
        }

        public override bool CreateCustomer(CustomerAccount u, out CreateUserStatus status, string clearPassword)
        {
            var result = false;
            status = CreateUserStatus.None;

            if (u != null)
            {
                var testUser = new CustomerAccount();
                testUser = Customers.FindByEmail(u.Email).FirstOrDefault();
                if (testUser != null)
                {
                    if (testUser.Bvin != string.Empty)
                    {
                        status = CreateUserStatus.DuplicateUsername;
                        return false;
                    }
                }

                u.Password = EncryptPassword(clearPassword, u);
                
                if (Customers.Create(u))
                {
                    result = true;
                    status = CreateUserStatus.Success;
                }
                else
                {
                    status = CreateUserStatus.UpdateFailed;
                }
            }

            return result;
        }

        public bool ChangePasswordForCustomer(string email, string oldPassword, string newPassword)
        {
            var result = false;

            var u = Customers.FindByEmail(email).FirstOrDefault();
            if (u != null)
            {
                if (DoPasswordsMatchForCustomer(oldPassword, u))
                {
                    u.Password = EncryptPassword(newPassword, u);
                    CreateUserStatus s;
                    result = UpdateCustomer(u, out s);
                }
            }

            return result;
        }

        public virtual bool ResetPasswordForCustomer(string email, string newPassword)
        {
            var result = false;
            var u = Customers.FindByEmail(email).FirstOrDefault();
            if (u != null)
            {
                u.Password = EncryptPassword(newPassword, u);
                var s = CreateUserStatus.None;
                result = UpdateCustomer(u, out s);
            }
            return result;
        }

        public SystemOperationResult ValidateUser(string email, string password)
        {
            var result = new SystemOperationResult();

            var u = Customers.FindByEmail(email).FirstOrDefault();
            if (u != null)
            {
                if (DoPasswordsMatchForCustomer(password, u))
                {
                    CustomerCheckLock(u);
                    if (u.Locked == false)
                    {
                        // Reset Failed Login Count
                        if (u.FailedLoginCount > 0)
                        {
                            u.FailedLoginCount = 0;
                            UpdateCustomer(u);
                        }
                        result.Success = true;
                    }
                    else
                    {
                        result.Success = false;
                        result.Message = GlobalLocalization.GetString("AccountLocked");
                    }
                }
                else
                {
                    result.Message = GlobalLocalization.GetString("LoginIncorrect");
                    u.FailedLoginCount += 1;
                    UpdateCustomer(u);
                    CustomerCheckLock(u);
                }
            }
            else
            {
                result.Message = GlobalLocalization.GetString("LoginIncorrect");
            }

            if (result.Success == false)
            {
                EventLog.LogEvent("Membership", "Login Failed for User: " + email, EventLogSeverity.Information);
            }

            return result;
        }

        private bool DoPasswordsMatchForCustomer(string trialpassword, CustomerAccount u)
        {
            return u.Password.Equals(EncryptPassword(trialpassword, u), StringComparison.InvariantCulture);
        }

        public override bool LoginUser(string email, string password, out string errorMessage, out string userId)
        {
            var result = false;
            errorMessage = string.Empty;
            userId = null;

            try
            {
                var op = ValidateUser(email, password);
                if (op.Success == false)
                {
                    errorMessage = op.Message;
                    return false;
                }

                var u = Customers.FindByEmail(email).FirstOrDefault();
                if (u == null)
                {
                    errorMessage = "Please check your email address and password and try again.";
                    return false;
                }

                userId = u.Bvin;

                Cookies.SetCookieString(WebAppSettings.CookieNameAuthenticationTokenCustomer(Context.CurrentStore.Id),
                    u.Bvin,
                    Context.RoutingContext.HttpContext, false, Factory.CreateEventLogger());
                result = true;
            }
            catch (Exception ex)
            {
                result = false;
                EventLog.LogEvent(ex);
                errorMessage = "Unknown login error. Contact administrator for assistance.";
            }

            return result;
        }

        public bool LogoutCustomer(HttpContextBase context, HotcakesApplication app)
        {
            var result = true;
            Cookies.SetCookieString(WebAppSettings.CookieNameAuthenticationTokenCustomer(app.CurrentStore.Id),
                string.Empty,
                context, false, Factory.CreateEventLogger());
            return result;
        }

        public virtual string EncryptPassword(string password, CustomerAccount cust)
        {
            var result = string.Empty;
            return result;
        }

        public bool DeleteCustomer(CustomerAccount customer)
        {
            return Customers.Delete(customer.Bvin);
        }

        public void UnlockCustomer(CustomerAccount c)
        {
            c.Locked = false;
            c.FailedLoginCount = 0;
            c.LockedUntilUtc = DateTime.UtcNow.AddMilliseconds(-1);
            UpdateCustomer(c);
        }

        public void LockCustomer(CustomerAccount c)
        {
            c.Locked = true;
            c.LockedUntilUtc = DateTime.UtcNow.AddMinutes(WebAppSettings.UserLockoutMinutes);
            UpdateCustomer(c);
        }

        public void CustomerCheckLock(CustomerAccount c)
        {
            if (c.Locked)
            {
                if (DateTime.Compare(DateTime.UtcNow, c.LockedUntilUtc) > 0)
                {
                    UnlockCustomer(c);
                }
            }
            else
            {
                if (c.FailedLoginCount >= WebAppSettings.UserLockoutAttempts)
                {
                    LockCustomer(c);
                    EventLog.LogEvent("Membership", "User Account " + c.Email + " was locked.", EventLogSeverity.Warning);
                }
            }
        }

        public override Dictionary<string, string> GetUserSettings(string userId, bool cached = true)
        {
            throw new NotImplementedException();
        }

        public override void UpdateUserSettings(string userId, Dictionary<string, string> settings, bool cached = true)
        {
            throw new NotImplementedException();
        }

        public override void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }

        public override void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan, bool notifyUser)
        {
            throw new NotImplementedException();
        }
    }
}