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
    /// <summary>
    /// Provides membership operations for the NoCms MVC host using <see cref="CustomerAccountRepository"/>.
    /// </summary>
    /// <remarks>
    /// The service adapts the shared <see cref="MembershipServices"/> contract to customer-account persistence;
    /// host-specific authentication state is represented by the MVC request and customer authentication cookie.
    /// </remarks>
    public class MvcMembershipService : MembershipServices
    {
        /// <summary>
        /// Initializes the service for the specified commerce request context.
        /// </summary>
        /// <param name="context">The request context used to scope the customer repository and current store.</param>
        public MvcMembershipService(HccRequestContext context)
            : base(context)
        {
            Customers = Factory.CreateRepo<CustomerAccountRepository>(Context);
        }

        /// <summary>
        /// Gets the first customer account matching an email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The matching <see cref="CustomerAccount"/>, or <see langword="null"/> when no account matches.</returns>
        /// <seealso cref="CustomerAccountRepository.FindByEmail(string)"/>
        public CustomerAccount GetCustomer(string email)
        {
            var customer = Customers.FindByEmail(email);

            if (customer != null)
            {
                return customer.FirstOrDefault();
            }

            return null;
        }

        /// <summary>
        /// Determines whether the current user is logged in.
        /// </summary>
        /// <returns>The NoCms implementation does not provide this operation.</returns>
        public override bool IsUserLoggedIn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current user is logged in as a super user.
        /// </summary>
        /// <returns>The NoCms implementation does not provide this operation.</returns>
        public override bool IsSuperUserLoggedIn()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Determines whether the current user has a named application permission.
        /// </summary>
        /// <param name="perm">The permission name to evaluate.</param>
        /// <param name="app">The application whose permission is being evaluated.</param>
        /// <returns>The NoCms implementation does not provide this operation.</returns>
        public override bool HasCurrentUserPermission(string perm, HotcakesApplication app)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets the identifier of the current user.
        /// </summary>
        /// <returns>The NoCms implementation does not provide this operation.</returns>
        public override string GetCurrentUserId()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Creates a customer account after checking for a duplicate email address and encrypting its password.
        /// </summary>
        /// <param name="u">The customer account to create.</param>
        /// <param name="status">When this method returns, the normalized result of the creation attempt.</param>
        /// <param name="clearPassword">The clear-text password to encrypt for the new account.</param>
        /// <returns><see langword="true"/> when the account is created; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="EncryptPassword(string, CustomerAccount)"/>
        /// <seealso cref="CustomerAccountRepository.Create(CustomerAccount)"/>
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

        /// <summary>
        /// Changes a customer's password after verifying the existing password.
        /// </summary>
        /// <param name="email">The email address of the customer account.</param>
        /// <param name="oldPassword">The current clear-text password.</param>
        /// <param name="newPassword">The new clear-text password.</param>
        /// <returns><see langword="true"/> when the password is changed; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="ValidateUser(string, string)"/>
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

        /// <summary>
        /// Resets a customer's password without requiring the current password.
        /// </summary>
        /// <param name="email">The email address of the customer account.</param>
        /// <param name="newPassword">The new clear-text password.</param>
        /// <returns><see langword="true"/> when the password is reset; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="EncryptPassword(string, CustomerAccount)"/>
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

        /// <summary>
        /// Validates a customer's credentials and applies account lockout checks.
        /// </summary>
        /// <param name="email">The email address to authenticate.</param>
        /// <param name="password">The clear-text password to validate.</param>
        /// <returns>An operation result describing whether authentication succeeded and, when it fails, the user-facing reason.</returns>
        /// <seealso cref="CustomerCheckLock(CustomerAccount)"/>
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

        /// <summary>
        /// Validates credentials and writes the authenticated customer's identifier to the MVC authentication cookie.
        /// </summary>
        /// <param name="email">The email address to authenticate.</param>
        /// <param name="password">The clear-text password to validate.</param>
        /// <param name="errorMessage">When authentication fails, the user-facing error message; otherwise, an empty string.</param>
        /// <param name="userId">When authentication succeeds, the customer's identifier; otherwise, <see langword="null"/>.</param>
        /// <returns><see langword="true"/> when the customer is authenticated; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="ValidateUser(string, string)"/>
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

        /// <summary>
        /// Clears the current customer's authentication cookie.
        /// </summary>
        /// <param name="context">The HTTP context whose response receives the cleared cookie.</param>
        /// <param name="app">The application used to resolve the current store.</param>
        /// <returns><see langword="true"/> after the cookie-clearing operation is issued.</returns>
        /// <seealso cref="LoginUser(string, string, out string, out string)"/>
        public bool LogoutCustomer(HttpContextBase context, HotcakesApplication app)
        {
            var result = true;
            Cookies.SetCookieString(WebAppSettings.CookieNameAuthenticationTokenCustomer(app.CurrentStore.Id),
                string.Empty,
                context, false, Factory.CreateEventLogger());
            return result;
        }

        /// <summary>
        /// Encrypts a customer's clear-text password for storage.
        /// </summary>
        /// <param name="password">The clear-text password.</param>
        /// <param name="cust">The customer account whose password is being encrypted.</param>
        /// <returns>The encrypted password value.</returns>
        /// <remarks>The NoCms implementation returns an empty value; derived providers can supply a password algorithm.</remarks>
        public virtual string EncryptPassword(string password, CustomerAccount cust)
        {
            var result = string.Empty;
            return result;
        }

        /// <summary>
        /// Deletes a customer account from the customer repository.
        /// </summary>
        /// <param name="customer">The customer account to delete.</param>
        /// <returns><see langword="true"/> when the repository deletes the account; otherwise, <see langword="false"/>.</returns>
        /// <seealso cref="CustomerAccountRepository.Delete(string)"/>
        public bool DeleteCustomer(CustomerAccount customer)
        {
            return Customers.Delete(customer.Bvin);
        }

        /// <summary>
        /// Unlocks a customer account and clears its failed-login count.
        /// </summary>
        /// <param name="c">The customer account to unlock.</param>
        /// <seealso cref="LockCustomer(CustomerAccount)"/>
        public void UnlockCustomer(CustomerAccount c)
        {
            c.Locked = false;
            c.FailedLoginCount = 0;
            c.LockedUntilUtc = DateTime.UtcNow.AddMilliseconds(-1);
            UpdateCustomer(c);
        }

        /// <summary>
        /// Locks a customer account until the configured lockout period expires.
        /// </summary>
        /// <param name="c">The customer account to lock.</param>
        /// <seealso cref="UnlockCustomer(CustomerAccount)"/>
        public void LockCustomer(CustomerAccount c)
        {
            c.Locked = true;
            c.LockedUntilUtc = DateTime.UtcNow.AddMinutes(WebAppSettings.UserLockoutMinutes);
            UpdateCustomer(c);
        }

        /// <summary>
        /// Updates a customer's lock state based on its current lockout expiration and failed-login count.
        /// </summary>
        /// <param name="c">The customer account whose lock state is evaluated.</param>
        /// <seealso cref="LockCustomer(CustomerAccount)"/>
        /// <seealso cref="UnlockCustomer(CustomerAccount)"/>
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

        /// <summary>
        /// Gets settings associated with a user.
        /// </summary>
        /// <param name="userId">The user identifier whose settings are requested.</param>
        /// <param name="cached"><see langword="true"/> to permit cached settings; otherwise, <see langword="false"/>.</param>
        /// <returns>The NoCms implementation does not provide this operation.</returns>
        public override Dictionary<string, string> GetUserSettings(string userId, bool cached = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Updates settings associated with a user.
        /// </summary>
        /// <param name="userId">The user identifier whose settings are updated.</param>
        /// <param name="settings">The settings to store.</param>
        /// <param name="cached"><see langword="true"/> to permit cached settings; otherwise, <see langword="false"/>.</param>
        public override void UpdateUserSettings(string userId, Dictionary<string, string> settings, bool cached = true)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Assigns a membership role to a user for a specified duration.
        /// </summary>
        /// <param name="userId">The user identifier receiving the membership role.</param>
        /// <param name="membershipType">The membership product type to assign.</param>
        /// <param name="timeSpan">The duration of the membership assignment.</param>
        public override void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan)
        {
            throw new NotImplementedException();
        }
    }
}
