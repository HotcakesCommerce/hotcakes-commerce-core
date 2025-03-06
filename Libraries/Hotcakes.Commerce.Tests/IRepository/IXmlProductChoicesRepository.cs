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
    public interface IXmlProductChoicesRepository : IDisposable
    {
        #region Product Choice Repository Service

        /// <summary>
        /// Gets the total product shared choice count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductSharedChoiceCount();

        /// <summary>
        /// Gets the total product choice count.
        /// </summary>
        /// <returns></returns>
        int GetTotalProductChoiceCount();

        /// <summary>
        /// Gets the name of the delete product choice.
        /// </summary>
        /// <returns></returns>
        int GetDeleteProductChoiceName();

        /// <summary>
        /// Gets the name of the add product shared choice.
        /// </summary>
        /// <returns></returns>
        string GetAddProductSharedChoiceName();

        /// <summary>
        /// Gets the type of the add product choice.
        /// </summary>
        /// <returns></returns>
        int[] GetAddProductChoiceTypeCode();

        /// <summary>
        /// Gets the name of the edit product choice.
        /// </summary>
        /// <returns></returns>
        int[] GetEditProductChoiceName();

        /// <summary>
        /// Gets the edit product choice.
        /// </summary>
        /// <returns></returns>
        Option GetEditProductChoice();

        /// <summary>
        /// Sets the product choice HTML.
        /// </summary>
        /// <param name="choice">The option.</param>
        void SetProductChoiceInfo(ref Option choice);

        /// <summary>
        /// Sets the product choice item.
        /// </summary>
        /// <param name="choice">The option.</param>
        void AddProductChoiceItem(ref Option choice);

        /// <summary>
        /// Deletes the product choice item.
        /// </summary>
        /// <param name="choice">The choice.</param>
        void DeleteProductChoiceItem(ref Option choice);

        /// <summary>
        /// Edits the product choice item.
        /// </summary>
        /// <param name="choice">The choice.</param>
        void EditProductChoiceItem(ref Option choice);



        #region ProductXOption
        /// <summary>
        /// Finds all count.
        /// </summary>
        /// <returns></returns>
        int FindAll_PXO_Count();
        /// <summary>
        /// Finds all for all stores count.
        /// </summary>
        /// <returns></returns>
        int FindAll_PXO_ForAllStoresCount();
        /// <summary>
        /// Finds all paged count.
        /// </summary>
        /// <returns></returns>
        int FindAll_PXO_PagedCount();
        /// <summary>
        /// Finds for product count.
        /// </summary>
        /// <returns></returns>
        int Find_PXO_ForProductCount();
        /// <summary>
        /// Finds for option count.
        /// </summary>
        /// <returns></returns>
        int Find_PXO_ForOptionCount();

        /// <summary>
        /// Gets the name of the option.
        /// </summary>
        /// <returns></returns>
        string GetOptionName();


        #endregion

        #region ProductOption

        /// <summary>
        /// Finds the all product option count.
        /// </summary>
        /// <returns></returns>
        int FindAll_PO_Count();

        /// <summary>
        /// Find product option by product identifier count.
        /// </summary>
        /// <returns></returns>
        int Find_PO_ByProductIdCount();

        /// <summary>
        /// Finds the many product option count.
        /// </summary>
        /// <returns></returns>
        int FindMany_PO_Count();

        /// <summary>
        /// Gets the product option list.
        /// </summary>
        /// <returns></returns>
        List<string> GetProductOptionList();


        #endregion


        #region ProductOptionItems

        #endregion

        #endregion
    }
}
