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

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Repository class to perform different database operation on
    ///     hcc_CatalogRoles table.
    /// </summary>
    public class CatalogRolesRepository : HccSimpleRepoBase<hcc_CatalogRoles, CatalogRole>
    {
        #region Constructors

        public CatalogRolesRepository(HccRequestContext context)
            : base(context)
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        ///     Find the CatalogRole by role id.
        /// </summary>
        /// <param name="roleId">Unique identifer of the role</param>
        /// <returns>Returns the CatalogRole instance for the matching role id</returns>
        public CatalogRole Find(long roleId)
        {
            return FindFirstPoco(y => y.CatalogRoleId == roleId);
        }

        /// <summary>
        ///     Get list of all role names
        /// </summary>
        /// <returns>Return list of distinct role names</returns>
        public List<string> FindAllRoleNames()
        {
            var storeId = Context.CurrentStore.Id;
            using (var s = CreateStrategy())
            {
                return s.GetQuery().Where(r => r.StoreId == storeId)
                    .Select(r => r.RoleName)
                    .Distinct()
                    .ToList();
            }
        }

        public List<CatalogRole> FindByProductId(Guid productId)
        {
            return FindListPoco(q => { return q.Where(r => r.ProductId == productId); });
        }

        public List<CatalogRole> FindByCategoryId(Guid categoryId)
        {
            return FindListPoco(q => { return q.Where(r => r.CategoryId == categoryId); });
        }

        public List<CatalogRole> FindByCategoryIds(List<Guid> categoryIds)
        {
            return
                FindListPoco(
                    q => { return q.Where(r => r.CategoryId.HasValue && categoryIds.Contains(r.CategoryId.Value)); });
        }

        public List<CatalogRole> FindByProductTypeId(Guid productTypeId)
        {
            return FindListPoco(q => { return q.Where(r => r.ProductTypeId == productTypeId); });
        }

        public bool Delete(long roleId)
        {
            return Delete(y => y.CatalogRoleId == roleId);
        }

        public void CloneForProduct(string productId, string newProductId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var newProductGuid = DataTypeHelper.BvinToGuid(newProductId);
            using (var s = CreateStrategy())
            {
                var items = s.GetQuery()
                    .Where(bp => bp.ProductId == productGuid).ToList();

                foreach (var item in items)
                {
                    s.Detach(item);
                    item.ProductId = newProductGuid;
                    s.Add(item);
                }

                s.SubmitChanges();
            }
        }

        #endregion

        #region Implementation

        protected override void CopyDataToModel(hcc_CatalogRoles data, CatalogRole model)
        {
            model.CatalogRoleId = data.CatalogRoleId;
            model.RoleName = data.RoleName;

            if (data.ProductId.HasValue)
            {
                model.ReferenceId = data.ProductId.Value;
                model.RoleType = CatalogRoleType.ProductRole;
            }
            if (data.CategoryId.HasValue)
            {
                model.ReferenceId = data.CategoryId.Value;
                model.RoleType = CatalogRoleType.CategoryRole;
            }
            if (data.ProductTypeId.HasValue)
            {
                model.ReferenceId = data.ProductTypeId.Value;
                model.RoleType = CatalogRoleType.ProductTypeRole;
            }
        }

        protected override void CopyModelToData(hcc_CatalogRoles data, CatalogRole model)
        {
            data.CatalogRoleId = model.CatalogRoleId;
            data.RoleName = model.RoleName;
            data.StoreId = Context.CurrentStore.Id;

            data.ProductId = null;
            data.CategoryId = null;
            data.ProductTypeId = null;

            switch (model.RoleType)
            {
                case CatalogRoleType.ProductRole:
                    data.ProductId = model.ReferenceId;
                    break;
                case CatalogRoleType.CategoryRole:
                    data.CategoryId = model.ReferenceId;
                    break;
                case CatalogRoleType.ProductTypeRole:
                    data.ProductTypeId = model.ReferenceId;
                    break;
                default:
                    break;
            }
        }

        #endregion
    }
}