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
    ///     This class used to perform database operation for hcc_ApiKey table.
    /// </summary>
    [Serializable]
    public class ApiKeyRepository : HccSimpleRepoBase<hcc_ApiKey, ApiKey>
    {
        public ApiKeyRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Copy data from database instance to model instance.
        /// </summary>
        /// <param name="data">Database table instance</param>
        /// <param name="model">Model instance</param>
        protected override void CopyDataToModel(hcc_ApiKey data, ApiKey model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.Key = data.ApiKey1;
        }

        /// <summary>
        ///     Copy data from Model instance to Database table instance.
        /// </summary>
        /// <param name="data">Database table instance</param>
        /// <param name="model">Model instance</param>
        protected override void CopyModelToData(hcc_ApiKey data, ApiKey model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.ApiKey1 = model.Key;
        }

        /// <summary>
        ///     Delete API key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public bool Delete(long id)
        {
            return Delete(a => a.Id == id);
        }

        /// <summary>
        ///     Get <see cref="ApiKey" /> instance by Key.
        /// </summary>
        /// <param name="key">API Key</param>
        /// <returns>APIKey instance</returns>
        public ApiKey FindByKey(string key)
        {
            var parts = key.Split('-');
            long storeId = -1;
            if (parts.Count() > 0)
            {
                var tempId = parts[0];
                long.TryParse(tempId, out storeId);
            }
            if (storeId < 0) return null;

            return FindFirstPoco(a => a.StoreId == storeId && a.ApiKey1 == key);
        }

        /// <summary>
        ///     Get list of Keys by Store Id.
        /// </summary>
        /// <param name="storeId">Store unique identifier</param>
        /// <returns>List of ApiKey instances</returns>
        public List<ApiKey> FindByStoreId(long storeId)
        {
            return FindListPoco(q => { return q.Where(a => a.StoreId == storeId); });
        }

        /// <summary>
        ///     Delete all Keys for given store.
        /// </summary>
        /// <param name="storeId">Store unique identifer.</param>
        internal void DestoryAllForStore(long storeId)
        {
            Delete(a => a.StoreId == storeId);
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ApiKeyRepository InstantiateForMemory(HccRequestContext c)
        {
            return new ApiKeyRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public static ApiKeyRepository InstantiateForDatabase(HccRequestContext c)
        {
            return new ApiKeyRepository(c);
        }

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ApiKeyRepository(HccRequestContext c, IRepositoryStrategy<hcc_ApiKey> r, ILogger log)
            : this(c)
        {
        }

        #endregion
    }
}