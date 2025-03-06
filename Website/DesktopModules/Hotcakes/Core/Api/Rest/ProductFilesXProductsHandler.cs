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

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class ProductFilesXProductsHandler : BaseRestHandler
    {
        public ProductFilesXProductsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        // List or Find Single
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;
            var productBvin = FirstParameter(parameters);

            if (!string.IsNullOrEmpty(parameters))
            {
                // List
                var response = new ApiResponse<List<ProductFileDTO>>();

                response.Content = new List<ProductFileDTO>();

                var items = HccApp.CatalogServices.ProductFiles.FindByProductId(productBvin);

                if (items != null && items.Count > 0)
                {
                    foreach (var item in items)
                    {
                        response.Content.Add(item.ToDto());
                    }
                }
                else
                {
                    response.Errors.Add(new ApiError("NULL", "No files were found for this product."));
                }

                data = Json.ObjectToJson(response);
            }

            return data;
        }

        // Create or Update
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var productBvin = GetParameterByIndex(1, parameters);

            var minutes = querystring["minutes"];
            var availableMinutes = 0;
            int.TryParse(minutes, out availableMinutes);

            var downloads = querystring["downloads"];
            var maxDownloads = 0;
            int.TryParse(downloads, out maxDownloads);

            var response = new ApiResponse<bool>
            {
                Content = HccApp.CatalogServices.ProductFiles.AddAssociatedProduct(bvin, productBvin,
                    availableMinutes, maxDownloads)
            };

            data = Json.ObjectToJson(response);
            return data;
        }

        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var productBvin = GetParameterByIndex(1, parameters);
            var response = new ApiResponse<bool>
            {
                Content = HccApp.CatalogServices.ProductFiles.RemoveAssociatedProduct(bvin, productBvin)
            };

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}