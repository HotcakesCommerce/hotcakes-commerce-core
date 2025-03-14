﻿#region License

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
    public interface IXmlProductCategoryRepository : IDisposable
    {

        #region Product Category Repository Service

        /// <summary>
        /// Gets the total product category.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductCategory();

        /// <summary>
        /// Gets the total category.
        /// </summary>
        /// <returns></returns>
        int GetTotalCategory();

        /// <summary>
        /// Gets the add product category.
        /// </summary>
        /// <returns></returns>
        List<string> GetAddProductCategory();

        /// <summary>
        /// Gets the delete product category.
        /// </summary>
        /// <returns></returns>
        List<string> GetDeleteProductCategory();

        /// <summary>
        /// Gets the find all count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllCount();

        /// <summary>
        /// Gets the find all for all store count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllForAllStoreCount();

        /// <summary>
        /// Gets the find all paged count.
        /// </summary>
        /// <returns></returns>
        int GetFindAllPagedCount();


        #endregion

    }
}
