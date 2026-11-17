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

using System.Collections.Generic;
using Hotcakes.Commerce.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    /// Represents the catalog interface <see cref="ICategoryRepository" /> and its associated behavior.
    /// </summary>
    public interface ICategoryRepository
    {
        /// <summary>
        /// Performs the catalog operation represented by <c>Create</c>.
        /// </summary>
        /// <param name="item">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        bool Create(Category item);
        /// <summary>
        /// Performs the catalog operation represented by <c>Delete</c>.
        /// </summary>
        /// <param name="bvin">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        bool Delete(string bvin);
        /// <summary>
        /// Performs the catalog operation represented by <c>Find</c>.
        /// </summary>
        /// <param name="bvin">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        Category Find(string bvin);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindAll</c>.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindAll();
        /// <summary>
        /// Performs the catalog operation represented by <c>FindAllPaged</c>.
        /// </summary>
        /// <param name="pageNumber">Input used by the operation.</param>
        /// <param name="pageSize">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<Category> FindAllPaged(int pageNumber, int pageSize);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindAllForAllStores</c>.
        /// </summary>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindAllForAllStores();
        /// <summary>
        /// Performs the catalog operation represented by <c>FindAllSnapshotsPaged</c>.
        /// </summary>
        /// <param name="pageNumber">Input used by the operation.</param>
        /// <param name="pageSize">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindAllSnapshotsPaged(int pageNumber, int pageSize);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindAllSnapshotsPagedForAllStores</c>.
        /// </summary>
        /// <param name="pageNumber">Input used by the operation.</param>
        /// <param name="pageSize">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindAllSnapshotsPagedForAllStores(int pageNumber, int pageSize);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindBySlug</c>.
        /// </summary>
        /// <param name="urlSlug">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        Category FindBySlug(string urlSlug);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindBySlugForStore</c>.
        /// </summary>
        /// <param name="urlSlug">Input used by the operation.</param>
        /// <param name="storeId">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        Category FindBySlugForStore(string urlSlug, long storeId);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindChildren</c>.
        /// </summary>
        /// <param name="parentId">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindChildren(string parentId);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindChildren</c>.
        /// </summary>
        /// <param name="parentId">Input used by the operation.</param>
        /// <param name="pageNumber">Input used by the operation.</param>
        /// <param name="pageSize">Input used by the operation.</param>
        /// <param name="totalRowCount">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindChildren(string parentId, int pageNumber, int pageSize, ref int totalRowCount);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindForAllStores</c>.
        /// </summary>
        /// <param name="bvin">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        Category FindForAllStores(string bvin);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindMany</c>.
        /// </summary>
        /// <param name="bvins">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<Category> FindMany(List<string> bvins);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindMany</c>.
        /// </summary>
        /// <param name="name">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<Category> FindMany(string name);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindManySnapshots</c>.
        /// </summary>
        /// <param name="bvins">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindManySnapshots(List<string> bvins);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindVisibleChildren</c>.
        /// </summary>
        /// <param name="parentId">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindVisibleChildren(string parentId);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindVisibleChildren</c>.
        /// </summary>
        /// <param name="parentId">Input used by the operation.</param>
        /// <param name="pageNumber">Input used by the operation.</param>
        /// <param name="pageSize">Input used by the operation.</param>
        /// <param name="totalRowCount">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        List<CategorySnapshot> FindVisibleChildren(string parentId, int pageNumber, int pageSize, ref int totalRowCount);
        /// <summary>
        /// Performs the catalog operation represented by <c>Update</c>.
        /// </summary>
        /// <param name="c">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        bool Update(Category c);
        /// <summary>
        /// Performs the catalog operation represented by <c>UpdateAdv</c>.
        /// </summary>
        /// <param name="c">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        DalSingleOperationResult<Category> UpdateAdv(Category c);
        /// <summary>
        /// Performs the catalog operation represented by <c>DestroyAllForStore</c>.
        /// </summary>
        /// <param name="storeId">Input used by the operation.</param>
        void DestroyAllForStore(long storeId);
        /// <summary>
        /// Performs the catalog operation represented by <c>FindMaxSort</c>.
        /// </summary>
        /// <param name="parentId">Input used by the operation.</param>
        /// <returns>The result of the operation.</returns>
        int FindMaxSort(string parentId);
    }
}
