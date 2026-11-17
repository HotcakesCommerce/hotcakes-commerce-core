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
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Provides the common membership and customer-account lifecycle operations shared by CMS providers.
    /// </summary>
    /// <remarks>Authentication and provider-specific account creation remain abstract so DNN and no-CMS implementations can preserve their own security semantics.</remarks>
    public abstract class MembershipServices : HccServiceBase, IMembershipServices
    {
        /// <summary>Initializes the service for a commerce request context.</summary>
        /// <param name="context">The request context used to create repositories and raise integration events.</param>
        public MembershipServices(HccRequestContext context)
            : base(context)
        {
            UserQuestions = Factory.CreateRepo<UserQuestionRepository>(Context);
        }

        /// <summary>Gets the repository for membership-related user questions.</summary>
        public UserQuestionRepository UserQuestions { get; protected set; }

        /// <summary>Gets or sets the customer-account repository used by this service.</summary>
        public ICustomerAccountRepository Customers { get; protected set; }

        /// <summary>Authenticates a user and returns the provider's user identifier when successful.</summary>
        /// <param name="username">The provider-specific username or login value.</param>
        /// <param name="password">The clear password supplied for authentication.</param>
        /// <param name="errorMessage">Receives a provider-facing failure message.</param>
        /// <param name="userId">Receives the authenticated user identifier, or the provider's failure value.</param>
        /// <returns><see langword="true"/> when authentication succeeds; otherwise, <see langword="false"/>.</returns>
        public abstract bool LoginUser(string username, string password, out string errorMessage, out string userId);

        /// <summary>Determines whether the current request has an authenticated user.</summary>
        /// <returns><see langword="true"/> when a user is logged in; otherwise, <see langword="false"/>.</returns>
        public abstract bool IsUserLoggedIn();

        /// <summary>Determines whether the current request is authenticated as a super user.</summary>
        /// <returns><see langword="true"/> when a super user is logged in; otherwise, <see langword="false"/>.</returns>
        public abstract bool IsSuperUserLoggedIn();

        /// <summary>Gets the current authenticated user's identifier.</summary>
        /// <returns>The provider-specific identifier, or an empty value when no user is logged in.</returns>
        public abstract string GetCurrentUserId();

        /// <summary>Checks the current user's provider-defined permission.</summary>
        /// <param name="perm">The permission name.</param>
        /// <param name="app">The application whose permission matrix is consulted.</param>
        /// <returns><see langword="true"/> when the current user has permission; otherwise, <see langword="false"/>.</returns>
        public abstract bool HasCurrentUserPermission(string perm, HotcakesApplication app);

        /// <summary>Gets the login page path for the current provider.</summary>
        /// <returns>The relative or absolute login page path.</returns>
        public virtual string GetLoginPagePath()
        {
            return "~/adminaccount/login";
        }

        /// <summary>Gets all roles known to the current provider.</summary>
        /// <returns>A list of role names, or an empty list when the provider has no role implementation.</returns>
        public virtual List<string> GetAllRoles()
        {
            return new List<string>();
        }

        /// <summary>Gets the provider-specific settings for a user.</summary>
        /// <param name="userId">The provider-specific user identifier.</param>
        /// <param name="cached">Whether the provider may read cached settings.</param>
        /// <returns>A name/value dictionary of settings.</returns>
        public abstract Dictionary<string, string> GetUserSettings(string userId, bool cached = true);

        /// <summary>Updates provider-specific settings for a user.</summary>
        /// <param name="userId">The provider-specific user identifier.</param>
        /// <param name="settings">The settings to write.</param>
        /// <param name="cached">Whether the provider may use cached user data.</param>
        public abstract void UpdateUserSettings(string userId, Dictionary<string, string> settings, bool cached = true);

        /// <summary>Creates a customer using a clear password and discards the detailed status.</summary>
        /// <param name="u">The customer account to create.</param>
        /// <param name="clearPassword">The clear password passed to the provider.</param>
        /// <returns><see langword="true"/> when creation succeeds; otherwise, <see langword="false"/>.</returns>
        public bool CreateCustomer(CustomerAccount u, string clearPassword)
        {
            var status = CreateUserStatus.None;
            return CreateCustomer(u, out status, clearPassword);
        }

        /// <summary>Creates a customer and reports the provider-normalized outcome.</summary>
        /// <param name="u">The customer account to create.</param>
        /// <param name="status">Receives a <see cref="CreateUserStatus"/> describing the outcome.</param>
        /// <param name="clearPassword">The clear password supplied to the provider; providers may transform it before persistence.</param>
        /// <returns><see langword="true"/> only when <paramref name="status"/> is <see cref="CreateUserStatus.Success"/>.</returns>
        /// <remarks>The DNN implementation maps duplicate email, duplicate username, invalid password, and rejected-user provider results to the corresponding statuses and maps other failures to <see cref="CreateUserStatus.UpdateFailed"/>. The no-CMS implementation reports an existing email as <see cref="CreateUserStatus.DuplicateUsername"/>; these mappings are compatibility behavior.</remarks>
        public abstract bool CreateCustomer(CustomerAccount u, out CreateUserStatus status, string clearPassword);

        /// <summary>Updates a customer and discards the detailed status.</summary>
        /// <param name="u">The customer account to update.</param>
        /// <returns><see langword="true"/> when the update succeeds; otherwise, <see langword="false"/>.</returns>
        public bool UpdateCustomer(CustomerAccount u)
        {
            CreateUserStatus status;
            return UpdateCustomer(u, out status);
        }

        /// <summary>Updates a customer after checking for a conflicting username.</summary>
        /// <param name="u">The customer account to update; a <see langword="null"/> value leaves the status as <see cref="CreateUserStatus.None"/>.</param>
        /// <param name="status">Receives <see cref="CreateUserStatus.DuplicateUsername"/>, <see cref="CreateUserStatus.Success"/>, or <see cref="CreateUserStatus.UpdateFailed"/> as applicable.</param>
        /// <returns><see langword="true"/> when the repository update succeeds; otherwise, <see langword="false"/>.</returns>
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

        /// <summary>Updates a customer's email address and raises the email-changed integration event after a successful update.</summary>
        /// <param name="user">The customer account to mutate and update.</param>
        /// <param name="newEmail">The replacement email address.</param>
        /// <returns><see langword="true"/> when the account update succeeds; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The account object is changed before the repository update. If the update fails, the in-memory email remains changed.</remarks>
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

        /// <summary>Deletes every customer visible to the repository when the API clear window is still open.</summary>
        /// <param name="app">The application whose current store supplies <c>AllowApiToClearUntil</c>.</param>
        /// <returns><see langword="true"/> when the deletion pass is allowed and completed; otherwise, <see langword="false"/>.</returns>
        /// <remarks>This operation is intentionally time-gated and destructive. It deletes accounts individually and should be used with extreme care.</remarks>
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

        /// <summary>Deletes all customers in the current portal through the repository.</summary>
        /// <returns>Always <see langword="true"/> after the repository call returns.</returns>
        public bool DestroyCurrentPortalCustomers()
        {
            Customers.DeleteAllForPortal();
            return true;
        }

        /// <summary>Adds a new address to an account and persists the account when an address was added.</summary>
        /// <param name="a">The customer account whose address book is checked.</param>
        /// <param name="address">The address to compare and possibly add.</param>
        /// <returns><see langword="true"/> when the account's in-memory address book accepted a new address; otherwise, <see langword="false"/>.</returns>
        /// <remarks>The return value reflects the address-book mutation, not whether the subsequent repository update succeeds.</remarks>
        public bool CheckIfNewAddressAndAddWithUpdate(CustomerAccount a, Address address)
        {
            var addressWasAdded = a.CheckIfNewAddressAndAddNoUpdate(address);
            if (addressWasAdded) UpdateCustomer(a);
            return addressWasAdded;
        }

        /// <summary>Creates the repository strategy used for customer-account persistence operations.</summary>
        /// <returns>A customer-account repository strategy from the configured factory.</returns>
        protected IRepoStrategy<CustomerAccount> CreateCustomerStrategy()
        {
            return Factory.Instance.CreateStrategy<CustomerAccount>();
        }

        /// <summary>Assigns or renews the provider role associated with a membership product.</summary>
        /// <param name="userId">The provider-specific user identifier.</param>
        /// <param name="membershipType">The membership product whose role is assigned.</param>
        /// <param name="timeSpan">The membership duration to apply.</param>
        public abstract void AssignMembershipRole(string userId, MembershipProductType membershipType, TimeSpan timeSpan);

    }
}
