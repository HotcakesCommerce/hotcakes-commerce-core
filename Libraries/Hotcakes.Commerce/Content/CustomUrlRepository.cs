#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
    public class CustomUrlRepository : HccSimpleRepoBase<hcc_CustomUrl, CustomUrl>
    {
        public CustomUrlRepository(HccRequestContext context)
            : base(context)
        {
        }

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

        public CustomUrl FindForAllStores(string bvin)
        {
            return FindFirstPoco(cu => cu.bvin == bvin);
        }

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

        public bool Update(CustomUrl customUrl)
        {
            if (customUrl.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            customUrl.LastUpdated = DateTime.UtcNow;
            return Update(customUrl, cu => cu.bvin == customUrl.Bvin);
        }

        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            return Delete(cu => cu.bvin == bvin && cu.StoreId == storeId);
        }

        public List<CustomUrl> FindBySystemData(string systemData)
        {
            var storeId = Context.CurrentStore.Id;
            return FindBySystemData(systemData, storeId);
        }

        public bool DeleteBySystemData(string systemData)
        {
            var storeId = Context.CurrentStore.Id;
            return Delete(cu => cu.StoreId == storeId && cu.SystemData == systemData);
        }

        public List<CustomUrl> FindBySystemData(string systemData, long storeId)
        {
            return FindListPoco(q =>
            {
                return q.Where(cu => cu.StoreId == storeId)
                    .Where(cu => cu.SystemData == systemData);
            });
        }

        public CustomUrl FindByRequestedUrl(string requestedUrl)
        {
            var match = requestedUrl.ToLowerInvariant();
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(cu => cu.StoreId == storeId && cu.RequestedUrl == match);
        }

        public CustomUrl FindByRequestedUrl(string requestedUrl, CustomUrlType type)
        {
            var match = requestedUrl.ToLowerInvariant();
            var storeId = Context.CurrentStore.Id;
            return
                FindFirstPoco(cu => cu.StoreId == storeId && cu.RequestedUrl == match && cu.SystemDataType == (int) type);
        }

        public CustomUrl FindByRedirectToUrl(string redirectToUrl)
        {
            var match = redirectToUrl.ToLowerInvariant();
            var storeId = Context.CurrentStore.Id;
            return FindFirstPoco(cu => cu.StoreId == storeId && cu.RedirectToUrl == match);
        }

        public List<CustomUrl> FindAll()
        {
            var totalCount = 0;
            return FindAllPaged(1, int.MaxValue, ref totalCount);
        }

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