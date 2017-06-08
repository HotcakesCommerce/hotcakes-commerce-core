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
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This is the primary class used for product/category/product type roles in the REST API.
    /// </summary>
    [DataContract]
    [Serializable]
    [XmlInclude(typeof (ProductStatusDTO))]
    public class CatalogRoleDTO
    {
        /// <summary>
        ///     The unique ID or primary key of the catalog role.
        /// </summary>
        public long CatalogRoleId { get; set; }

        /// <summary>
        ///     The name of the role or security role as it appears in the CMS.
        /// </summary>
        public string RoleName { get; set; }

        /// <summary>
        ///     The unique ID or primary key of one of following entities: product, category, product type.
        /// </summary>
        public Guid ReferenceId { get; set; }

        /// <summary>
        ///     This property specifies what object will be linked with catalog role.
        /// </summary>
        public CatalogRoleTypeDTO RoleType { get; set; }
    }
}