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

using System.Linq;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Search;

namespace Hotcakes.Commerce.Dnn
{
    public class DnnSearchProvider : SqlSearchProvider
    {
        public DnnSearchProvider(HccRequestContext hccRequestContext)
            : base(hccRequestContext)
        {
        }

        internal static IQueryable<hcc_Product> SecurityFilterImpl(IQueryable<hcc_Product> items)
        {
            // -------------------------
            // Keep synced logic in following functions:
            //	 - DnnProductSearcher.SecurityFilterImpl
            //	 - DnnProductRepository.GetSecureQuery
            // -------------------------

            var user = DnnUserController.Instance.GetCurrentUserInfo();

            if (user == null)
            {
                return items = items.Where(i =>
                    !i.hcc_CatalogRoles.Any() &&
                    !i.hcc_ProductXCategory.Any(pc => pc.hcc_Category.hcc_CatalogRoles.Any()) &&
                    !i.hcc_ProductType.hcc_CatalogRoles.Any());
            }

            if (DnnUserController.Instance.IsPortalAdmin(user))
            {
                return items;
            }

            return items.Where(i =>
                (!i.hcc_CatalogRoles.Any() &&
                 !i.hcc_ProductXCategory.Any(pc => pc.hcc_Category.hcc_CatalogRoles.Any()) &&
                 !i.hcc_ProductType.hcc_CatalogRoles.Any()
                    ) ||
                (!i.hcc_CatalogRoles.Any() &&
                 !i.hcc_ProductXCategory.Any(pc => pc.hcc_Category.hcc_CatalogRoles.Any()) &&
                 i.hcc_ProductType.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName))
                    ) ||
                (!i.hcc_CatalogRoles.Any() &&
                 i.hcc_ProductXCategory.Any(
                     pc => pc.hcc_Category.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName)))
                    ) ||
                i.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName)));
        }

        protected override IQueryable<hcc_Product> SecurityFilter(IQueryable<hcc_Product> items)
        {
            return SecurityFilterImpl(items);
        }

        protected override ProductSearchHelper CreateSearchHelper(HccRequestContext reqContext, HccDbContext context,
            long siteId)
        {
            return new DnnProductSearchHelper(reqContext, context, siteId);
        }

        protected class DnnProductSearchHelper : ProductSearchHelper
        {
            public DnnProductSearchHelper(HccRequestContext reqContext, HccDbContext context, long siteId)
                : base(reqContext, context, siteId)
            {
            }

            protected override IQueryable<hcc_Product> SecurityFilter(IQueryable<hcc_Product> items)
            {
                return SecurityFilterImpl(items);
            }
        }
    }
}