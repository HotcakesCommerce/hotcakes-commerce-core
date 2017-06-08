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
using System.Linq;
using System.Linq.Expressions;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Social;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnProductRepository : ProductRepository
    {
        private readonly ISocialService _socialService;

        #region Constructor

        public DnnProductRepository(HccRequestContext c)
            : base(c)
        {
            _socialService = Factory.CreateSocialService(c);
            reviewRepository = new DnnProductReviewRepository(Context);
        }

        #endregion

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public DnnProductRepository(HccRequestContext c, bool isForMemoryOnly, ICatalogSharedRepositories shReps)
            : this(c)
        {
        }

        #endregion

        #region Public methods

        public override bool Create(Product item)
        {
            var res = base.Create(item);
            OnCreated(item);
            return res;
        }

        protected override DalSingleOperationResult<Product> UpdateAdv(Product m,
            Expression<Func<hcc_Product, bool>> predicate, bool useModelCulture, bool mergeSubItems = true)
        {
            var res = base.UpdateAdv(m, predicate, useModelCulture, mergeSubItems);
            OnUpdated(m);
            return res;
        }

        protected override IQueryable<JoinedItem<hcc_Product, hcc_ProductTranslation>> GetSecureQuery(
            IRepoStrategy<hcc_Product> strategy)
        {
            // -------------------------
            // Keep synced logic in following functions:
            //	 - DnnProductSearcher.SecurityFilterImpl
            //	 - DnnProductRepository.GetSecureQuery
            // -------------------------

            var items = base.GetSecureQuery(strategy);

            var pSett = DnnGlobal.Instance.GetCurrentPortalSettings();
            var user = DnnUserController.Instance.GetCurrentUserInfo();

            if (user == null)
            {
                return items = items.Where(i =>
                    !i.Item.hcc_CatalogRoles.Any() &&
                    !i.Item.hcc_ProductXCategory.Any(pc => pc.hcc_Category.hcc_CatalogRoles.Any()) &&
                    !i.Item.hcc_ProductType.hcc_CatalogRoles.Any());
            }

            if (user.IsSuperUser || user.IsInRole(pSett.AdministratorRoleName))
            {
                return items;
            }

            return items.Where(i =>
                (!i.Item.hcc_CatalogRoles.Any() &&
                 !i.Item.hcc_ProductXCategory.Any(pc => pc.hcc_Category.hcc_CatalogRoles.Any()) &&
                 !i.Item.hcc_ProductType.hcc_CatalogRoles.Any()
                    ) ||
                (!i.Item.hcc_CatalogRoles.Any() &&
                 !i.Item.hcc_ProductXCategory.Any(pc => pc.hcc_Category.hcc_CatalogRoles.Any()) &&
                 i.Item.hcc_ProductType.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName))
                    ) ||
                (!i.Item.hcc_CatalogRoles.Any() &&
                 i.Item.hcc_ProductXCategory.Any(
                     pc => pc.hcc_Category.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName)))
                    ) ||
                i.Item.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName)));
        }

        #endregion

        #region Implementation

        private void OnCreated(Product p)
        {
            _socialService.UpdateJournalRecord(p);
        }

        private void OnUpdated(Product p)
        {
            _socialService.UpdateJournalRecord(p);
        }

        #endregion
    }
}