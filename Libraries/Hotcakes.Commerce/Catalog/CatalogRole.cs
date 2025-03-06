#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2025 Upendo Ventures, LLC
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
using Hotcakes.CommerceDTO.v1.Catalog;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     This is the primary class used for product/category/product type roles in the application.
    /// </summary>
    /// <remarks>The REST API equivalent is CatalogRoleDTO.</remarks>
    [Serializable]
    public class CatalogRole
    {
        /// <summary>
        ///     The unique ID or primary key of the catalog role.
        /// </summary>
        public long CatalogRoleId { get; set; }

        /// <summary>
        ///     The role name.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        ///     The unique ID or primary key of one of following entities: product, category, product type.
        /// </summary>
        public Guid ReferenceId { get; set; }

        /// <summary>
        ///     This property specify what object will be linked with catalog role.
        /// </summary>
        public CatalogRoleType RoleType { get; set; }

        #region DTO

        /// <summary>
        ///     Allows you to convert the current catalog role object to the DTO equivalent for use with the REST API
        /// </summary>
        /// <returns>A new instance of CatalogRoleDTO</returns>
        public CatalogRoleDTO ToDto()
        {
            return new CatalogRoleDTO
            {
                CatalogRoleId = CatalogRoleId,
                ReferenceId = ReferenceId,
                RoleName = RoleName,
                RoleType = (CatalogRoleTypeDTO) RoleType
            };
        }

        /// <summary>
        ///     Allows you to populate the current product review object using a CatalogRoleDTO instance
        /// </summary>
        /// <param name="dto">An instance of the catalog role from the REST API</param>
        public void FromDto(CatalogRoleDTO dto)
        {
            CatalogRoleId = dto.CatalogRoleId;
            ReferenceId = dto.ReferenceId;
            RoleName = dto.RoleName;
            RoleType = (CatalogRoleType) dto.RoleType;
        }

        #endregion
    }

    /// <summary>
    ///     This enumeration describes the type or way a security role should be used.
    /// </summary>
    public enum CatalogRoleType
    {
        /// <summary>
        ///     When used, this means that the role should be enforced at the product level.
        /// </summary>
        ProductRole = 0,

        /// <summary>
        ///     Categories will need to respect the security role, unless overridden at the product level.
        /// </summary>
        CategoryRole = 1,

        /// <summary>
        ///     Assigning security roles to the product type is the highest level that can be done, and overriddable at each lower
        ///     level.
        /// </summary>
        ProductTypeRole = 2
    }
}