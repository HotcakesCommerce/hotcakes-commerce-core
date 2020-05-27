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
using Hotcakes.Commerce.Data;

namespace Hotcakes.Commerce.Catalog
{
    public interface ICategoryRepository
    {
        bool Create(Category item);
        bool Delete(string bvin);
        Category Find(string bvin);
        List<CategorySnapshot> FindAll();
        List<Category> FindAllPaged(int pageNumber, int pageSize);
        List<CategorySnapshot> FindAllForAllStores();
        List<CategorySnapshot> FindAllSnapshotsPaged(int pageNumber, int pageSize);
        List<CategorySnapshot> FindAllSnapshotsPagedForAllStores(int pageNumber, int pageSize);
        Category FindBySlug(string urlSlug);
        Category FindBySlugForStore(string urlSlug, long storeId);
        List<CategorySnapshot> FindChildren(string parentId);
        List<CategorySnapshot> FindChildren(string parentId, int pageNumber, int pageSize, ref int totalRowCount);
        Category FindForAllStores(string bvin);
        List<Category> FindMany(List<string> bvins);
        List<Category> FindMany(string name);
        List<CategorySnapshot> FindManySnapshots(List<string> bvins);
        List<CategorySnapshot> FindVisibleChildren(string parentId);
        List<CategorySnapshot> FindVisibleChildren(string parentId, int pageNumber, int pageSize, ref int totalRowCount);
        bool Update(Category c);
        DalSingleOperationResult<Category> UpdateAdv(Category c);
        void DestroyAllForStore(long storeId);
        int FindMaxSort(string parentId);
    }
}