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
using System.Linq;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Social;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Dnn
{
    /// <summary>
    /// Provides category persistence with DNN catalog-role security and journal integration.
    /// </summary>
    [Serializable]
    public class DnnCategoryRepository : CategoryRepository
    {
        private readonly ISocialService _socialService;

        /// <summary>
        /// Initializes the repository for the specified commerce request context.
        /// </summary>
        /// <param name="context">The context used to resolve the current store and social service.</param>
        public DnnCategoryRepository(HccRequestContext context)
            : base(context)
        {
            _socialService = Factory.CreateSocialService(context);
        }

        #region Public methods

        /// <summary>
        /// Creates a category and updates its DNN journal record.
        /// </summary>
        /// <param name="item">The category to create.</param>
        /// <returns><c>true</c> when the category is created; otherwise, <c>false</c>.</returns>
        public override bool Create(Category item)
        {
            var res = base.Create(item);
            OnCreated(item);
            return res;
        }

        /// <summary>
        /// Updates a category and updates its DNN journal record.
        /// </summary>
        /// <param name="c">The category to update.</param>
        /// <returns><c>true</c> when the category is updated; otherwise, <c>false</c>.</returns>
        public override bool Update(Category c)
        {
            var res = base.Update(c);
            OnUpdated(c);
            return res;
        }

        /// <summary>
        /// Deletes a category by its business key and updates its DNN journal state.
        /// </summary>
        /// <param name="key">The category business key.</param>
        /// <returns><c>true</c> when the category is deleted; otherwise, <c>false</c>.</returns>
        public override bool Delete(string key)
        {
            var res = base.Delete(key);
            OnDeleted(key);
            return res;
        }

        #endregion

        #region Implementation

        protected override IQueryable<JoinedItem<hcc_Category, hcc_CategoryTranslation>> GetSecureQuery(
            IRepoStrategy<hcc_Category> strategy)
        {
            var query = GetJoinedQuery(strategy);

            var user = DnnUserController.Instance.GetCurrentUserInfo();

            if (user == null)
            {
                return query = query.Where(i => !i.Item.hcc_CatalogRoles.Any());
            }

            if (DnnUserController.Instance.IsPortalAdmin(user))
            {
                return query;
            }

            return query.Where(i =>
                !i.Item.hcc_CatalogRoles.Any() ||
                i.Item.hcc_CatalogRoles.Any(r => user.Roles.Contains(r.RoleName)));
        }

        private void OnCreated(Category cat)
        {
            _socialService.UpdateJournalRecord(cat);
        }

        private void OnUpdated(Category cat)
        {
            _socialService.UpdateJournalRecord(cat);
        }

        private void OnDeleted(string bvin)
        {
        }

        #endregion
    }
}
