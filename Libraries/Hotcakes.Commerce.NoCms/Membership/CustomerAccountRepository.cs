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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Contacts;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Provides store-scoped persistence operations for <see cref="CustomerAccount"/> records.
    /// </summary>
    public class CustomerAccountRepository : HccSimpleRepoBase<hcc_User, CustomerAccount>, ICustomerAccountRepository
    {
        /// <summary>
        /// Initializes a repository using the specified commerce request context.
        /// </summary>
        /// <param name="context">The request context used to resolve the current store.</param>
        public CustomerAccountRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        /// Finds a customer account by identifier in the current store.
        /// </summary>
        /// <param name="bvin">The account identifier to find.</param>
        /// <param name="useCache">A compatibility parameter retained for callers that specify cache behavior.</param>
        /// <returns>The matching account in the current store, or <see langword="null"/> when no account is found there.</returns>
        /// <seealso cref="FindForAllStores(string)"/>
        public CustomerAccount Find(string bvin, bool useCache = true)
        {
            var result = FindForAllStores(bvin);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        /// <summary>
        /// Finds a customer account by identifier without restricting the initial lookup to the current store.
        /// </summary>
        /// <param name="bvin">The account identifier to find.</param>
        /// <returns>The matching account, or <see langword="null"/> when no account is found.</returns>
        public CustomerAccount FindForAllStores(string bvin)
        {
            return FindFirstPoco(ca => ca.bvin == bvin);
        }

        /// <summary>
        /// Creates a customer account in the current store.
        /// </summary>
        /// <param name="item">The account to persist.</param>
        /// <returns><see langword="true"/> when the account is created; otherwise, <see langword="false"/>.</returns>
        public override bool Create(CustomerAccount item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdatedUtc = DateTime.UtcNow;

            var result = base.Create(item);
            if (result) Context.IntegrationEvents.CustomerAccountCreated(item);
            return result;
        }

        /// <summary>
        /// Updates a customer account when it belongs to the current store.
        /// </summary>
        /// <param name="customerAccount">The account containing the values to persist.</param>
        /// <returns><see langword="true"/> when the account is updated; otherwise, <see langword="false"/>.</returns>
        public bool Update(CustomerAccount customerAccount)
        {
            if (customerAccount.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            customerAccount.LastUpdatedUtc = DateTime.UtcNow;
            var result = Update(customerAccount, c => c.bvin == customerAccount.Bvin);
            if (result)
                Context.IntegrationEvents.CustomerAccountUpdated(customerAccount);
            return result;
        }

        /// <summary>
        /// Deletes a customer account from the current store.
        /// </summary>
        /// <param name="bvin">The account identifier to delete.</param>
        /// <returns><see langword="true"/> when the account is deleted; otherwise, <see langword="false"/>.</returns>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        /// <summary>
        /// Provides the portal-level account deletion hook.
        /// </summary>
        public void DeleteAllForPortal()
        {
        }

        /// <summary>
        /// Finds all customer accounts in the current store.
        /// </summary>
        /// <returns>All customer accounts in the current store.</returns>
        public List<CustomerAccount> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        /// <summary>
        /// Finds a page of customer accounts in the current store.
        /// </summary>
        /// <param name="pageNumber">The one-based page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of accounts to return.</param>
        /// <returns>The requested page of customer accounts.</returns>
        public override List<CustomerAccount> FindAllPaged(int pageNumber, int pageSize)
        {
            var allCount = 0;
            return FindAllPaged(pageNumber, pageSize, ref allCount);
        }

        /// <summary>
        /// Finds a page of customer accounts in the current store and reports the total count.
        /// </summary>
        /// <param name="pageNumber">The one-based page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of accounts to return.</param>
        /// <param name="totalCount">When this method returns, the total number of matching accounts.</param>
        /// <returns>The requested page of customer accounts.</returns>
        public List<CustomerAccount> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;

            using (var strategy = CreateReadStrategy())
            {
                var query = strategy
                    .GetQuery(ca => ca.StoreId == storeId)
                    .AsNoTracking()
                    .OrderBy(ca => ca.Email);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        /// <summary>
        /// Finds customer accounts in the current store by email address.
        /// </summary>
        /// <param name="email">The email address to search for.</param>
        /// <returns>The matching accounts, or <see langword="null"/> when the email is empty.</returns>
        public List<CustomerAccount> FindByEmail(string email)
        {
            if (string.IsNullOrEmpty(email))
            {
                return null;
            }

            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => q.Where(y => y.StoreId == storeId && y.Email == email));
        }

        /// <summary>
        /// Finds a customer account in the current store by username.
        /// </summary>
        /// <param name="username">The username to search for.</param>
        /// <returns>The matching account, or <see langword="null"/> when no account is found.</returns>
        public CustomerAccount FindByUsername(string username)
        {
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(ca => ca.StoreId == storeId && ca.Email == username);
        }

        /// <summary>
        /// Finds multiple customer accounts in the current store by their identifiers.
        /// </summary>
        /// <param name="ids">The account identifiers to search for.</param>
        /// <returns>The matching accounts ordered by email address.</returns>
        public List<CustomerAccount> FindMany(List<string> ids)
        {
            var result = new List<CustomerAccount>();
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(ca => ca.StoreId == storeId)
                    .Where(ca => ids.Contains(ca.bvin))
                    .OrderBy(ca => ca.Email);
            });
        }

        /// <summary>
        /// Finds a page of customer accounts in the current store using a text filter.
        /// </summary>
        /// <param name="filter">The text to search for in account contact and address fields.</param>
        /// <param name="pageNumber">The one-based page number to retrieve.</param>
        /// <param name="pageSize">The maximum number of accounts to return.</param>
        /// <param name="totalCount">When this method returns, the total number of matching accounts.</param>
        /// <returns>The requested page of matching customer accounts.</returns>
        public List<CustomerAccount> FindByFilter(string filter, int pageNumber, int pageSize, ref int totalCount)
        {
            var result = new List<CustomerAccount>();
            var storeId = Context.CurrentStore.Id;


            using (var strategy = CreateReadStrategy())
            {
                var query = strategy
                    .GetQuery(ca => ca.StoreId == storeId)
                    .AsNoTracking()
                    .Where(ca => ca.Email.Contains(filter) ||
                                 ca.FirstName.Contains(filter) ||
                                 ca.LastName.Contains(filter) ||
                                 ca.Phones.Contains(filter) ||
                                 ca.AddressBook.Contains(filter))
                    .OrderBy(ca => ca.Email);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        /// <summary>
        /// Deletes all customer accounts belonging to a store.
        /// </summary>
        /// <param name="storeId">The identifier of the store whose accounts should be deleted.</param>
        public void DestroyAllForStore(long storeId)
        {
            Delete(ca => ca.StoreId == storeId);
        }

        protected override void CopyDataToModel(hcc_User data, CustomerAccount model)
        {
            model.Bvin = data.bvin;
            model.StoreId = data.StoreId;
            model.LastUpdatedUtc = data.LastUpdated;
            model.CreationDateUtc = data.CreationDate;
            model.Email = data.Email;
            model.FailedLoginCount = data.FailedLoginCount;
            model.FirstName = data.FirstName;
            model.LastLoginDateUtc = data.LastLoginDate;
            model.LastName = data.LastName;
            model.Locked = data.Locked == 1;
            model.LockedUntilUtc = data.LockedUntil;
            model.Notes = data.Comment;
            model.Password = data.Password;
            model.PricingGroupId = data.PricingGroup;
            model.TaxExempt = data.TaxExempt == 1;

            Address shipAddr = null;
            Address billAddr = null;
            try
            {
                shipAddr = Json.ObjectFromJson<Address>(data.ShippingAddress);
                billAddr = Json.ObjectFromJson<Address>(data.BillingAddress);
                model.Addresses = AddressList.FromJson(data.AddressBook);
                model.Phones = PhoneNumberList.FromJson(data.Phones);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
            model.ShippingAddress = shipAddr ?? new Address();

            model.BillingAddress = billAddr ?? new Address();
        }

        protected override void CopyModelToData(hcc_User data, CustomerAccount model)
        {
            data.bvin = model.Bvin;
            data.StoreId = model.StoreId;
            data.LastUpdated = model.LastUpdatedUtc;
            data.AddressBook = model.Addresses.ToJson();
            data.CreationDate = model.CreationDateUtc;
            data.Email = model.Email;
            data.FailedLoginCount = model.FailedLoginCount;
            data.FirstName = model.FirstName;
            data.LastLoginDate = model.LastLoginDateUtc;
            data.LastName = model.LastName;
            data.Locked = model.Locked ? 1 : 0;
            data.LockedUntil = model.LockedUntilUtc;
            data.Comment = model.Notes;
            data.Password = model.Password;
            data.Phones = model.Phones.ToJson();
            data.PricingGroup = model.PricingGroupId;
            data.TaxExempt = model.TaxExempt ? 1 : 0;
            data.CustomQuestionAnswers = string.Empty; // To Be Remove Soon
            try
            {
                data.ShippingAddress = Json.ObjectToJson(model.ShippingAddress);
                data.BillingAddress = Json.ObjectToJson(model.BillingAddress);
            }
            catch (Exception ex)
            {
                EventLog.LogEvent(ex);
            }
        }


        internal bool DeleteForStore(string bvin, long storeId)
        {
            var item = FindForAllStores(bvin);
            if (item == null) return false;
            var result = Delete(c => c.bvin == bvin);
            if (result)
                Context.IntegrationEvents.CustomerAccountDeleted(item);
            return result;
        }
    }
}
