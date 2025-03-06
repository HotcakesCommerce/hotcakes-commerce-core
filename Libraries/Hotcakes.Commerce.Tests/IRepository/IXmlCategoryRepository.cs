#region License

// Distributed under the MIT License
// ============================================================
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Hotcakes.Commerce.Catalog;

namespace Hotcakes.Commerce.Tests.IRepository
{
    public interface IXmlCategoryRepository : IDisposable
    {

        #region Category Count
        /// <summary>
        /// Gets the total category count.
        /// </summary>
        /// <returns></returns>
        int GetTotalCategoryCount();

        /// <summary>
        /// Gets the total category child count.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, int> GetTotalCategoryChildCount();

        /// <summary>
        /// Gets the category display template count.
        /// </summary>
        /// <returns></returns>
        int GetTotalDisplayTemplateCount();

        /// <summary>
        /// Gets the total populate columns count.
        /// </summary>
        /// <returns></returns>
        int GetTotalPopulateColumnsCount();
        #endregion

       
        #region Category Add/Edit/Delete

        /// <summary>
        /// Gets the add category.
        /// </summary>
        /// <returns></returns>
        Category GetAddCategory();

        /// <summary>
        /// Gets the add child category.
        /// </summary>
        /// <returns></returns>
        Category GetAddChildCategory();

        /// <summary>
        /// Gets the category taxonomy.
        /// </summary>
        /// <returns></returns>
        string GetCategoryTaxonomy();

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <returns></returns>
        string GetEditCategoryName();

        /// <summary>
        /// Gets the category.
        /// </summary>
        /// <returns></returns>
        string GetCategoryName();

        /// <summary>
        /// Gets the edit category.
        /// </summary>
        /// <returns></returns> 
        Category GetEditCategory();

        /// <summary>
        /// Gets the delete category.
        /// </summary>
        /// <returns></returns>
        string GetDeleteCategory();
        #endregion


        #region Link Add/Edit/Delete
        /// <summary>
        /// Gets the add custom link.
        /// </summary>
        /// <returns></returns>
        Category GetAddCustomLink();

        /// <summary>
        /// Gets the add child custom link.
        /// </summary>
        /// <returns></returns>
        Category GetAddChildCustomLink();

        /// <summary>
        /// Gets the edit custom link.
        /// </summary>
        /// <returns></returns>
        Category GetEditCustomLink();

        /// <summary>
        /// Gets the name of the custom link.
        /// </summary>
        /// <returns></returns>
        string GetCustomLinkName();

        /// <summary>
        /// Gets the delete custom link.
        /// </summary>
        /// <returns></returns> 
        string GetDeleteCustomLink();
        #endregion


        #region Category Product
        /// <summary>
        /// Gets the total vendor count.
        /// </summary>
        /// <returns></returns>
        int GetTotalVendorCount();

        /// <summary>
        /// Gets the total manufacturer count.
        /// </summary>
        /// <returns></returns>
        int GetTotalManufacturerCount();

        /// <summary>
        /// Gets the category product count.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, int> GetCategoryProductCount();

        /// <summary>
        /// Gets the total product count.
        /// </summary>
        /// <returns></returns>
        Dictionary<int, ProductSearchCriteria> GetTotalProductCount();

        /// <summary>
        /// Gets the add product to category.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, List<string>> GetAddProductToCategory();

        /// <summary>
        /// Gets the delete product from category.
        /// </summary>
        /// <returns></returns>
        Dictionary<string, List<string>> GetDeleteProductFromCategory();

        #endregion


        #region Category Role
        /// <summary>
        /// Gets the role refname.
        /// </summary>
        /// <returns></returns>
        string GetRoleName();

        /// <summary>
        /// Gets the total role count.
        /// </summary>
        /// <returns></returns>
        int GetTotalRoleCount();

        /// <summary>
        /// Gets the total category role count.
        /// </summary>
        /// <returns></returns>
        int GetTotalCategoryRoleCount();

        /// <summary>
        /// Gets the delete role.
        /// </summary>
        /// <returns></returns>
        string GetDeleteRole();

        /// <summary>
        /// Gets the add role.
        /// </summary>
        /// <returns></returns>
        CatalogRole GetAddRole(string refid);
        #endregion


        #region Category Find
        /// <summary>
        /// Gets the category slug.
        /// </summary>
        /// <returns></returns>
        string GetCategorySlug();

        /// <summary>
        /// Gets the cate ids.
        /// </summary>
        /// <returns></returns>
        List<string> GetCateIds();

        /// <summary>
        /// Gets the store cat snap count.
        /// </summary>
        /// <returns></returns>
        int GetStoreCatSnapCount();

        /// <summary>
        /// Gets the total cat snap count.
        /// </summary>
        /// <returns></returns>
        int GetTotalCatSnapCount();

        /// <summary>
        /// Gets the total cat pages.
        /// </summary>
        /// <returns></returns>
        int GetTotalCatPages();

        /// <summary>
        /// Gets the total visible child cat count.
        /// </summary>
        /// <returns></returns>
        int GetTotalVisibleChildCatCount();

        /// <summary>
        /// Gets the total child cat count.
        /// </summary>
        /// <returns></returns>
        int GetTotalChildCatCount();
        #endregion

    }
}
