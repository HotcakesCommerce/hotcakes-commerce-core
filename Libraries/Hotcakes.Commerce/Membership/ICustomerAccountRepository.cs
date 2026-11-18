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

namespace Hotcakes.Commerce.Membership
{
    /// <summary>
    /// Defines persistence and lookup operations for <see cref="CustomerAccount"/> records.
    /// </summary>
    /// <remarks>Implementations provide the CMS-specific storage boundary. Store and portal scoping, caching, ordering, paging, and the meaning of an empty or missing result are implementation details that callers must preserve.</remarks>
    public interface ICustomerAccountRepository
    {
        /// <summary>Finds an account in the current store by its identifier.</summary>
        /// <param name="bvin">The account identifier.</param>
        /// <param name="useCache">Whether the provider may use its cache.</param>
        /// <returns>The matching account in the current store, or <see langword="null"/> when none is available.</returns>
        CustomerAccount Find(string bvin, bool useCache = true);

        /// <summary>Finds an account by identifier without restricting the lookup to the current store when the provider supports that distinction.</summary>
        /// <param name="bvin">The account identifier.</param>
        /// <returns>The matching account, or <see langword="null"/> when none is available.</returns>
        /// <remarks>The DNN implementation resolves this through its portal user store, while the no-CMS implementation performs an unscoped lookup.</remarks>
        CustomerAccount FindForAllStores(string bvin);

        /// <summary>Creates an account.</summary>
        /// <param name="item">The account to persist.</param>
        /// <returns><see langword="true"/> when the provider creates the account; otherwise, <see langword="false"/>.</returns>
        bool Create(CustomerAccount item);

        /// <summary>Updates an existing account.</summary>
        /// <param name="c">The account containing the values to persist.</param>
        /// <returns><see langword="true"/> when the provider updates the account; otherwise, <see langword="false"/>.</returns>
        bool Update(CustomerAccount c);

        /// <summary>Deletes an account by identifier.</summary>
        /// <param name="bvin">The account identifier.</param>
        /// <returns><see langword="true"/> when an account was deleted; otherwise, <see langword="false"/>.</returns>
        bool Delete(string bvin);

        /// <summary>Deletes all accounts belonging to the current portal where the provider supports portal-scoped deletion.</summary>
        void DeleteAllForPortal();

        /// <summary>Gets all accounts visible to the current provider scope.</summary>
        /// <returns>The accounts, or an empty list when none exist.</returns>
        List<CustomerAccount> FindAll();

        /// <summary>Gets one page of accounts and returns the total matching count.</summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of accounts in the page.</param>
        /// <param name="totalCount">Receives the total count before paging.</param>
        /// <returns>The requested page of accounts.</returns>
        List<CustomerAccount> FindAllPaged(int pageNumber, int pageSize, ref int totalCount);

        /// <summary>Gets one page of accounts without returning the total count.</summary>
        /// <param name="pageNumber">The provider's page number.</param>
        /// <param name="pageSize">The maximum number of accounts in the page.</param>
        /// <returns>The requested page of accounts.</returns>
        List<CustomerAccount> FindAllPaged(int pageNumber, int pageSize);

        /// <summary>Finds accounts by exact email address.</summary>
        /// <param name="email">The email address to look up.</param>
        /// <returns>Matching accounts, or a provider-defined null/empty result for an empty input or no matches.</returns>
        List<CustomerAccount> FindByEmail(string email);

        /// <summary>Finds an account by username.</summary>
        /// <param name="username">The username to look up.</param>
        /// <returns>The matching account, or <see langword="null"/> when none is available.</returns>
        /// <remarks>The no-CMS implementation currently matches this lookup against email, whereas the DNN implementation uses the CMS username.</remarks>
        CustomerAccount FindByUsername(string username);

        /// <summary>Finds multiple accounts by their identifiers.</summary>
        /// <param name="ids">The account identifiers to match.</param>
        /// <returns>The matching accounts in provider-defined order.</returns>
        List<CustomerAccount> FindMany(List<string> ids);

        /// <summary>Finds a page of accounts matching a provider-defined filter.</summary>
        /// <param name="filter">The text or criteria to search.</param>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of accounts in the page.</param>
        /// <param name="totalCount">Receives the total number of matching accounts before paging.</param>
        /// <returns>The requested filtered page.</returns>
        List<CustomerAccount> FindByFilter(string filter, int pageNumber, int pageSize, ref int totalCount);

        /// <summary>Counts all accounts visible to the current provider scope.</summary>
        /// <returns>The account count.</returns>
        int CountOfAll();

        /// <summary>Destroys all accounts belonging to the specified store when the provider supports store-scoped destruction.</summary>
        /// <param name="storeId">The store identifier.</param>
        void DestroyAllForStore(long storeId);
    }
}
