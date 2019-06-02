#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;
using Hotcakes.Web.Logging;

namespace Hotcakes.Commerce.Accounts
{
    /// <summary>
    ///     This class used to perform database operations against hcc_AuthTokens table.
    /// </summary>
    [Serializable]
    public class AuthTokenRepository : HccSimpleRepoBase<hcc_AuthTokens, AuthToken>
    {
        public AuthTokenRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Copy database table instance to model instance.
        /// </summary>
        /// <param name="data">Database table instance</param>
        /// <param name="model">Model instance</param>
        protected override void CopyDataToModel(hcc_AuthTokens data, AuthToken model)
        {
            model.Id = data.Id;
            model.Expires = data.Expires;
            model.TokenId = data.TokenId;
            model.UserId = data.UserId;
        }

        /// <summary>
        ///     Copy model instance to database table instance.
        /// </summary>
        /// <param name="data">Database table instance</param>
        /// <param name="model">Model instance</param>
        protected override void CopyModelToData(hcc_AuthTokens data, AuthToken model)
        {
            data.Id = model.Id;
            data.Expires = model.Expires;
            data.TokenId = model.TokenId;
            data.UserId = model.UserId;
        }

        /// <summary>
        ///     Update AuthToken.
        /// </summary>
        /// <param name="t">Instance of the AuthToken</param>
        /// <returns>Returns true if updated successfully otherwise false.</returns>
        public bool Update(AuthToken t)
        {
            return Update(t, y => y.Id == t.Id);
        }

        /// <summary>
        ///     Delete AuthToken by unique identifier.
        /// </summary>
        /// <param name="id">AuthToken unique identifier.</param>
        /// <returns>True if deleted successfully otherwise false.</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>
        ///     Find AuthToken info by token id.
        /// </summary>
        /// <param name="tokenId">Token Id</param>
        /// <returns>AuthToken instance</returns>
        public AuthToken FindByTokenId(Guid tokenId)
        {
            return FindFirstPoco(y => y.TokenId == tokenId);
        }

        /// <summary>
        ///     Get list of AuthTokens for given user.
        /// </summary>
        /// <param name="userId">User unique identifier</param>
        /// <returns>List of AuthToken instances.</returns>
        public List<AuthToken> FindByUserId(long userId)
        {
            return FindListPoco(q => { return q.Where(y => y.UserId == userId); });
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static AuthTokenRepository InstantiateForMemory(HccRequestContext c)
        {
            return new AuthTokenRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static AuthTokenRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new AuthTokenRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public AuthTokenRepository(HccRequestContext c, IRepositoryStrategy<hcc_AuthTokens> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}