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
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Utilities;

namespace Hotcakes.Commerce.Content
{
    /// <summary>Persists and queries store-scoped <see cref="CustomUrl"/> records.</summary>
    public class CustomUrlRepository : HccSimpleRepoBase<hcc_CustomUrl, CustomUrl>
    {
        /// <summary>Initializes the repository for the specified commerce context.</summary>
        public CustomUrlRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>Copies persisted custom URL fields into the model, including normalized URL properties.</summary>
        protected override void CopyDataToModel(hcc_CustomUrl data, CustomUrl model)
        {
            model.Bvin = data.bvin;
            model.StoreId = data.StoreId;
            model.IsPermanentRedirect = data.IsPermanentRedirect;
            model.LastUpdated = data.LastUpdated;
            model.RedirectToUrl = data.RedirectToUrl;
            model.RequestedUrl = data.RequestedUrl;
            model.SystemData = data.SystemData;
            model.SystemDataType = (CustomUrlType) data.SystemDataType;
        }

        /// <summary>Copies custom URL model fields into the persistence record.</summary>
        protected override void CopyModelToData(hcc_CustomUrl data, CustomUrl model)
        {
            data.bvin = model.Bvin;
            data.StoreId = model.StoreId;
            data.IsPermanentRedirect = model.IsPermanentRedirect;
            data.LastUpdated = model.LastUpdated;
            data.RedirectToUrl = model.RedirectToUrl;
            data.RequestedUrl = model.RequestedUrl;
            data.SystemData = model.SystemData;
            data.SystemDataType = (int) model.SystemDataType;
        }

        /// <summary>Finds a URL by identifier only when it belongs to the current store.</summary>
        public CustomUrl Find(string bvin)
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

        /// <summary>Finds a URL by identifier without applying current-store filtering.</summary>
        public CustomUrl FindForAllStores(string bvin)
        {
            return FindFirstPoco(cu => cu.bvin == bvin);
        }

        /// <summary>Creates a URL, assigning an identifier when needed and stamping the current store and UTC update time.</summary>
        public override bool Create(CustomUrl item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            item.LastUpdated = DateTime.UtcNow;
            return base.Create(item);
        }

        /// <summary>Updates a URL belonging to the current store and refreshes its UTC update time.</summary>
        /// <returns><see langword="false"/> when the URL belongs to another store or persistence fails.</returns>
        public bool Update(CustomUrl customUrl)
        {
            if (customUrl.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            customUrl.LastUpdated = DateTime.UtcNow;
            return Update(customUrl, cu => cu.bvin == customUrl.Bvin);
        }

        /// <summary>Deletes the current-store URL with the specified identifier.</summary>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            return Delete(cu => cu.bvin == bvin && cu.StoreId == storeId);
        }

        /// <summary>Finds current-store URLs associated with the specified system data.</summary>
        public List<CustomUrl> FindBySystemData(string systemData)
        {
            var storeId = Context.CurrentStore.Id;
            return FindBySystemData(systemData, storeId);
        }

        /// <summary>Deletes all current-store URLs associated with the specified system data.</summary>
        public bool DeleteBySystemData(string systemData)
        {
            var storeId = Context.CurrentStore.Id;
            return Delete(cu => cu.StoreId == storeId && cu.SystemData == systemData);
        }

        /// <summary>Finds URLs associated with system data in the specified store.</summary>
        public List<CustomUrl> FindBySystemData(string systemData, long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(cu => cu.StoreId == storeId)
                    .Where(cu => cu.SystemData == systemData);
            });
        }

        /// <summary>Finds a current-store URL by its lowercase-invariant requested path.</summary>
        public CustomUrl FindByRequestedUrl(string requestedUrl)
        {
            var match = requestedUrl.ToLowerInvariant();
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(cu => cu.StoreId == storeId && cu.RequestedUrl == match);
        }

        /// <summary>Finds a current-store URL by requested path and URL type.</summary>
        public CustomUrl FindByRequestedUrl(string requestedUrl, CustomUrlType type)
        {
            var match = requestedUrl.ToLowerInvariant();
            var storeId = Context.CurrentStore.Id;
            return
                FindFirstPoco(cu => cu.StoreId == storeId && cu.RequestedUrl == match && cu.SystemDataType == (int) type);
        }

        /// <summary>Finds a current-store URL by its lowercase-invariant redirect destination.</summary>
        public CustomUrl FindByRedirectToUrl(string redirectToUrl)
        {
            var match = redirectToUrl.ToLowerInvariant();
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(cu => cu.StoreId == storeId && cu.RedirectToUrl == match);
        }

        /// <summary>Gets all URLs for the current store, ordered by requested URL.</summary>
        public List<CustomUrl> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

        /// <summary>Gets one page of current-store URLs ordered by requested URL.</summary>
        /// <param name="pageNumber">The one-based page number.</param>
        /// <param name="pageSize">The maximum number of records in the page.</param>
        /// <param name="totalCount">Receives the total number of matching records.</param>
        public List<CustomUrl> FindAllPaged(int pageNumber, int pageSize, ref int totalCount)
        {
            var storeId = Context.CurrentStore.Id;

            using (var strategy = CreateReadStrategy())
            {
                var query = strategy
                    .GetQuery(cu => cu.StoreId == storeId)
                    .AsNoTracking()
                    .OrderBy(cu => cu.RequestedUrl);

                totalCount = query.Count();

                var items = GetPagedItems(query, pageNumber, pageSize).ToList();
                return ListPoco(items);
            }
        }

        /// <summary>Registers a permanent redirect unless the requested URL is already in use, then updates redirects for the object.</summary>
        public void Register301(string requestedUrl, string redirectUrl, string objectId, CustomUrlType customUrlType,
            HccRequestContext context, HotcakesApplication app)
        {
            var AlreadyInUse = UrlRewriter.IsUrlInUse(requestedUrl, string.Empty, context, app);
            if (AlreadyInUse) return;
            var customUrl = new CustomUrl
            {
                IsPermanentRedirect = true,
                RedirectToUrl = redirectUrl,
                RequestedUrl = requestedUrl,
                StoreId = app.CurrentRequestContext.CurrentStore.Id,
                SystemData = objectId,
                SystemDataType = customUrlType
            };
            Create(customUrl);
            UpdateAllUrlsForObject(objectId, redirectUrl);
        }

        /// <summary>Updates the redirect destination of all current-store URLs associated with an object.</summary>
        public void UpdateAllUrlsForObject(string objectId, string redirectToUrl)
        {
            var all = FindBySystemData(objectId);
            if (all == null) return;
            foreach (var cu in all)
            {
                cu.RedirectToUrl = redirectToUrl;
                Update(cu);
            }
        }

        internal void DestoryAllForStore(long storeId)
        {
            Delete(cu => cu.StoreId == storeId);
        }
    }
}
