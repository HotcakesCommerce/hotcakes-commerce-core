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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;

namespace Hotcakes.Commerce.Membership
{
    public abstract class MembershipServices : HccServiceBase, IMembershipServices
    {
        public MembershipServices(HccRequestContext context)
            : base(context)
        {
            UserQuestions = Factory.CreateRepo<UserQuestionRepository>(Context);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateService instead")]
        public MembershipServices(HccRequestContext c, bool isForMemoryOnly)
            : this(c)
        {
        }

        #endregion

        public UserQuestionRepository UserQuestions { get; protected set; }
        public ICustomerAccountRepository Customers { get; protected set; }

        // User accounts methods
        public abstract bool LoginUser(string username, string password, out string errorMessage, out string userId);
        public abstract bool IsUserLoggedIn();
        public abstract bool IsSuperUserLoggedIn();
        public abstract string GetCurrentUserId();
        public abstract bool HasCurrentUserPermission(string perm, HotcakesApplication app);

        public virtual string GetLoginPagePath()
        {
            return "~/adminaccount/login";
        }

        public virtual List<string> GetAllRoles()
        {
            return new List<string>();
        }

        public abstract Dictionary<string, string> GetUserSettings(string userId, bool cached = true);
        public abstract void UpdateUserSettings(string userId, Dictionary<string, string> settings, bool cached = true);

        // Customers methods
        public bool CreateCustomer(CustomerAccount u, string clearPassword)
        {
            var status = CreateUserStatus.None;
            return CreateCustomer(u, out status, clearPassword);
        }

        public abstract bool CreateCustomer(CustomerAccount u, out CreateUserStatus status, string clearPassword);

        public bool UpdateCustomer(CustomerAccount u)
        {
            CreateUserStatus status;
            return UpdateCustomer(u, out status);
        }

        public bool UpdateCustomer(CustomerAccount u, out CreateUserStatus status)
        {
            var result = false;
            status = CreateUserStatus.None;

            if (u != null)
            {
                var testUser = Customers.FindByUsername(u.Username);
                if (testUser != null && testUser.Bvin != u.Bvin)
                {
                    status = CreateUserStatus.DuplicateUsername;
                    return false;
                }

                if (Customers.Update(u))
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

        public bool UpdateCustomerEmail(CustomerAccount user, string newEmail)
        {
            var oldEmail = user.Email;
            user.Email = newEmail;
            var result = UpdateCustomer(user);
            if (result)
            {
                Context.IntegrationEvents.CustomerAccountEmailChanged(oldEmail, newEmail);
            }
            return result;
        }

        // Use this VERY carefully
        public bool DestroyAllCustomers(HotcakesApplication app)
        {
            var current = DateTime.UtcNow;
            var availableUntil = app.CurrentStore.Settings.AllowApiToClearUntil;
            var compareResult = DateTime.Compare(current, availableUntil);
            if (compareResult >= 0)
            {
                return false;
            }
            var all = Customers.FindAll();
            foreach (var a in all)
            {
                Customers.Delete(a.Bvin);
            }
            return true;
        }

        public bool DestroyCurrentPortalCustomers()
        {
            Customers.DeleteAllForPortal();
            return true;
        }

        public bool CheckIfNewAddressAndAddWithUpdate(CustomerAccount a, Address address)
        {
            var addressWasAdded = a.CheckIfNewAddressAndAddNoUpdate(address);
            if (addressWasAdded) UpdateCustomer(a);
            return addressWasAdded;
        }

        protected IRepoStrategy<CustomerAccount> CreateCustomerStrategy()
        {
            return Factory.Instance.CreateStrategy<CustomerAccount>();
        }

        public abstract void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan);

        public abstract void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan, bool notifyUser);

    }
}