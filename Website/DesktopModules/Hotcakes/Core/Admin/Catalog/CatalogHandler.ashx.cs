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

using System.Linq;
using System.Web;
using Hotcakes.Commerce;
using Hotcakes.Modules.Core.Admin.AppCode;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Admin.Catalog
{
    /// <summary>
    ///     Summary description for CatalogHandler
    /// </summary>
    public class CatalogHandler : BaseHandler, IHttpHandler
    {
        protected override object HandleAction(HttpRequest request, HotcakesApplication hccApp)
        {
            if (request.RequestContext.HttpContext.User.Identity.IsAuthenticated == false)
            {
                // not found
                request.RequestContext.HttpContext.Response.StatusCode = 404;
                request.RequestContext.HttpContext.Response.End();
                return null;
            }

            var method = request.Params["method"];

            switch (method)
            {
                case "Slugify":
                    return Slugify(request, hccApp);
                case "ResortAdditionalImages":
                    return ResortAdditionalImages(request, hccApp);
                case "ResortRelatedProducts":
                    return ResortRelatedProducts(request, hccApp);
                case "ResortBundledProducts":
                    return ResortBundledProducts(request, hccApp);
                case "ResortProductPropertyChoices":
                    return ResortProductPropertyChoices(request, hccApp);
                case "ResortProductProperties":
                    return ResortProductProperties(request, hccApp);
                case "ResortChoiceItems":
                    return ResortChoiceItems(request, hccApp);
                default:
                    break;
            }
            return true;
        }

        private object ResortChoiceItems(HttpRequest request, HotcakesApplication hccApp)
        {
            var optionId = request.Params["optionId"];
            var itemIds = request.Params["itemIds"];
            var sortedItemIds = itemIds.Split(',').ToList();

            return hccApp.CatalogServices.ProductOptions.ResortOptionItems(optionId, sortedItemIds);
        }

        private object Slugify(HttpRequest request, HotcakesApplication hccApp)
        {
            var name = request.Params["name"];
            return Text.Slugify(name);
        }

        private object ResortAdditionalImages(HttpRequest request, HotcakesApplication hccApp)
        {
            var productId = request.Params["productId"];
            var itemIds = request.Params["itemIds"];
            var sortedItemIds = itemIds.Split(',').ToList();

            return hccApp.CatalogServices.ProductImages.Resort(productId, sortedItemIds);
        }

        private object ResortRelatedProducts(HttpRequest request, HotcakesApplication hccApp)
        {
            var productId = request.Params["productId"];
            var itemIds = request.Params["itemIds"];
            var sortedItemIds = itemIds.Split(',').ToList();

            return hccApp.CatalogServices.ProductRelationships.ResortRelationships(productId, sortedItemIds);
        }

        private object ResortBundledProducts(HttpRequest request, HotcakesApplication hccApp)
        {
            var bundledProductIds = request.Params["bundledProductIds"];

            var sortedItemIds = bundledProductIds.
                Split(',').
                Select(id => long.Parse(id)).
                ToList();

            return hccApp.CatalogServices.BundledProducts.ResortBundledProducts(sortedItemIds);
        }

        private object ResortProductPropertyChoices(HttpRequest request, HotcakesApplication hccApp)
        {
            var productPropertyChoiceIds = request.Params["productPropertyChoiceIds"];

            var sortedItemIds = productPropertyChoiceIds.
                Split(',').
                Select(id => long.Parse(id)).
                ToList();

            return hccApp.CatalogServices.ProductProperties.ResortProductPropertyChoices(sortedItemIds);
        }

        private object ResortProductProperties(HttpRequest request, HotcakesApplication hccApp)
        {
            var propertyIds = request.Params["propertyIds"];
            var typeId = request.Params["typeId"];

            var sortedItemIds = propertyIds.
                Split(',').
                Select(id => long.Parse(id)).
                ToList();

            return hccApp.CatalogServices.ProductTypesXProperties.ResortProperties(typeId, sortedItemIds);
        }
    }
}