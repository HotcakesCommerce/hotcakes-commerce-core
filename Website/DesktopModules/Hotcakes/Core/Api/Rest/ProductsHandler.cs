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
using System.Collections.Generic;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Globalization;
using Hotcakes.Commerce.Utilities;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for product management
    /// </summary>
    [Serializable]
    public class ProductsHandler : BaseRestHandler
    {
        public ProductsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return one or many products to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. Possible keys include bycategory,
        ///     countonly, page, pagesize, bysku and byslug.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     ProductDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;
            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var categoryBvin = querystring["bycategory"];
                var countonly = querystring["countonly"];

                int pageInt;

                if (!int.TryParse(querystring["page"], out pageInt))
                {
                    pageInt = 1;
                }

                int pageSizeInt;

                if (!int.TryParse(querystring["pagesize"], out pageSizeInt))
                {
                    pageSizeInt = 1000;
                }

                if (!string.IsNullOrWhiteSpace(categoryBvin))
                {
                    // by category
                    var responsePage = new ApiResponse<PageOfProducts>();
                    responsePage.Content = new PageOfProducts();

                    var totalCount = 0;
                    var resultItems = HccApp.CatalogServices.FindProductForCategoryWithSort(categoryBvin,
                        CategorySortOrder.None, false, pageInt, pageSizeInt, ref totalCount);
                    responsePage.Content.TotalProductCount = totalCount;

                    foreach (var p in resultItems)
                    {
                        responsePage.Content.Products.Add(p.ToDto());
                    }

                    data = Json.ObjectToJson(responsePage);
                }
                else if (!string.IsNullOrWhiteSpace(countonly))
                {
                    // count only
                    var responseCount = new ApiResponse<long>();
                    responseCount.Content = 0;

                    var output = HccApp.CatalogServices.Products.FindAllCount();

                    responseCount.Content = output;

                    data = Json.ObjectToJson(responseCount);
                }
                else
                {
                    // all by page
                    var responsePage = new ApiResponse<PageOfProducts>();

                    responsePage.Content = new PageOfProducts();

                    var resultItems = new List<Product>();

                    resultItems = HccApp.CatalogServices.Products.FindAllPagedWithCache(pageInt, pageSizeInt);

                    foreach (var p in resultItems)
                    {
                        responsePage.Content.Products.Add(p.ToDto());
                    }

                    data = Json.ObjectToJson(responsePage);
                }
            }
            else
            {
                var bysku = querystring["bysku"] ?? string.Empty;
                var byslug = querystring["byslug"] ?? string.Empty;
                var bvin = FirstParameter(parameters);

                // Find One Specific Category
                var response = new ApiResponse<ProductDTO>();
                Product item = null;

                if (bysku.Trim().Length > 0)
                {
                    item = HccApp.CatalogServices.Products.FindBySku(bysku);
                }
                else if (byslug.Trim().Length > 0)
                {
                    item = HccApp.CatalogServices.Products.FindBySlug(byslug);
                }
                else
                {
                    item = HccApp.CatalogServices.Products.Find(bvin);
                }

                if (item == null)
                {
                    response.Errors.Add(new ApiError("NULL", "Could not locate that product. Check bvin and try again."));
                }
                else
                {
                    response.Content = item.ToDto();
                }

                data = Json.ObjectToJson(response);
            }

            return data;
        }

        /// <summary>
        ///     Allows the REST API to create or update a product
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product ID (bvin) and that this is an update, otherwise it assumes to create
        ///     a product.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductDTO object</param>
        /// <returns>ProductDTO - Serialized (JSON) version of the product</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<ProductDTO>();
            ProductDTO postedItem = null;

            try
            {
                postedItem = Json.ObjectFromJson<ProductDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new Product();
            item.FromDto(postedItem);

            var existing = HccApp.CatalogServices.Products.Find(item.Bvin);
            if (existing == null || existing.Bvin == string.Empty)
            {
                item.StoreId = HccApp.CurrentStore.Id;

                if (item.UrlSlug.Trim().Length < 1)
                {
                    item.UrlSlug = Text.Slugify(item.ProductName, false);
                }

                // Try 10000 times to append to URL if in use
                var rewriteUrlInUse = UrlRewriter.IsProductSlugInUse(item.UrlSlug, string.Empty, HccApp);
                var baseRewriteUrl = item.UrlSlug;

                for (var i = 2; i < 10000; i++)
                {
                    if (rewriteUrlInUse)
                    {
                        item.UrlSlug = string.Concat(baseRewriteUrl, "-", i.ToString());
                        rewriteUrlInUse = UrlRewriter.IsProductSlugInUse(item.UrlSlug, string.Empty, HccApp);

                        if (!rewriteUrlInUse)
                            break;
                    }
                }

                if (HccApp.CatalogServices.ProductsCreateWithInventory(item, false))
                {
                    bvin = item.Bvin;
                }
            }
            else
            {
                // adding missing lists that get overridden as a result 
                // of this endpoint updating the product without them
                item.BundledProducts = existing.BundledProducts;
                item.Images = existing.Images;
                item.Reviews = existing.Reviews;

                // these were already here
                item.Variants = existing.Variants;
                item.Options = existing.Options;

                HccApp.CatalogServices.ProductsUpdateWithSearchRebuild(item);
            }

            var resultItem = HccApp.CatalogServices.Products.Find(bvin);

            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);

            return data;
        }

        /// <summary>
        ///     Allows for the REST API to delete a product from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product ID/bvin) is
        ///     expected.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. Not used in this method.</param>
        /// <param name="postdata">This parameter is not used in this method</param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain either True or False,
        ///     depending on success of the deletion
        /// </returns>
        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);


            if (bvin == string.Empty)
            {
                var howManyString = querystring["howmany"];
                var howMany = 0;
                int.TryParse(howManyString, out howMany);

                // Clear All Products Requested
                var response = new ApiResponse<ClearProductsData> {Content = HccApp.ClearProducts(howMany)};
                data = Json.ObjectToJson(response);
            }
            else
            {
                // Single Item Delete
                var response = new ApiResponse<bool>
                {
                    Content = HccApp.CatalogServices.DestroyProduct(bvin, HccApp.CurrentStore.Id)
                };
                data = Json.ObjectToJson(response);
            }

            return data;
        }
    }
}