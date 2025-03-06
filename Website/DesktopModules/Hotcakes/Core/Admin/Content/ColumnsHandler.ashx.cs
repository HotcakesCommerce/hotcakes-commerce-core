﻿#region License

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

using System.Linq;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Modules.Core.Admin.AppCode;

namespace Hotcakes.Modules.Core.Admin.Content
{
    /// <summary>
    ///     Summary description for ColumnsHandler
    /// </summary>
    public class ColumnsHandler : BaseHandler, IHttpHandler
    {
        protected override object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            var method = request.Params["method"];

            switch (method)
            {
                case "ResortColumnItems":
                    return ResortColumnItems(request, hccApp);
                case "ResortCategoryRotator":
                    return ResortCategoryRotator(request, hccApp);
                case "ResortProductRotator":
                    return ResortProductRotator(request, hccApp);
                case "ResortImageRotator":
                    return ResortImageRotator(request, hccApp);
                default:
                    break;
            }
            return true;
        }


        private object ResortImageRotator(HttpRequest request, HotcakesApplication hccApp)
        {
            ResortBlockItems(request, hccApp, "Images");
            return true;
        }

        private object ResortProductRotator(HttpRequest request, HotcakesApplication hccApp)
        {
            ResortBlockItems(request, hccApp, "Products");
            return true;
        }

        private object ResortCategoryRotator(HttpRequest request, HotcakesApplication hccApp)
        {
            ResortBlockItems(request, hccApp, "Categories");
            return true;
        }

        private static void ResortBlockItems(HttpRequest request, HotcakesApplication hccApp, string listName)
        {
            var blockId = request.Params["blockId"];
            var itemIds = request.Params["itemIds"];
            var sortedItemIds = itemIds.Split(',').ToList();

            var block = hccApp.ContentServices.Columns.FindBlock(blockId);
            block.Lists.UpdateSortOrder(sortedItemIds, listName);
            hccApp.ContentServices.Columns.UpdateBlock(block);
        }

        private object ResortColumnItems(HttpRequest request, HotcakesApplication hccApp)
        {
            var columnId = request.Params["columnId"];
            var itemIds = request.Params["itemIds"];

            var sortedItemIds = itemIds.Split(',').ToList();
            hccApp.ContentServices.Columns.ResortBlocksItems(columnId, sortedItemIds);

            return true;
        }
    }
}