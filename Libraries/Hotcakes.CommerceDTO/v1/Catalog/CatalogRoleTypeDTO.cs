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

namespace Hotcakes.CommerceDTO.v1.Catalog
{
    /// <summary>
    ///     This enumeration describes the type or way a security role should be used.
    /// </summary>
    public enum CatalogRoleTypeDTO
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