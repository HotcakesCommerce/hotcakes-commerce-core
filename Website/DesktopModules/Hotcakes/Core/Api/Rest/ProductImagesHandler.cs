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
using System.Collections.Specialized;
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class ProductImagesHandler : BaseRestHandler
    {
        public ProductImagesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        // List or Find Single
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var byproduct = querystring["byproduct"] ?? string.Empty;

            if (string.IsNullOrEmpty(bvin))
            {
                var response = new ApiResponse<List<ProductImageDTO>>();

                var dto = new List<ProductImageDTO>();
                var items = new List<ProductImage>();

                if (string.IsNullOrEmpty(byproduct))
                {
                    // Find all product images
                    items = HccApp.CatalogServices.ProductImages.FindAllPaged(1, int.MaxValue);

                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            dto.Add(item.ToDto());
                        }
                    }
                }
                else
                {
                    // Find all product images for the product
                    items = HccApp.CatalogServices.ProductImages.FindByProductIdPaged(byproduct, 1, int.MaxValue);

                    if (items.Any())
                    {
                        foreach (var item in items)
                        {
                            dto.Add(item.ToDto());
                        }
                    }
                }

                response.Content = dto;

                data = Json.ObjectToJson(response);
            }
            else
            {
                // Find one specific product image
                var item = HccApp.CatalogServices.ProductImages.Find(bvin);

                var response = new ApiResponse<ProductImageDTO> {Content = item.ToDto()};

                data = Json.ObjectToJson(response);
            }

            return data;
        }

        // Create or Update
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<ProductImageDTO>();

            ProductImageDTO postedItem = null;

            try
            {
                postedItem = Json.ObjectFromJson<ProductImageDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new ProductImage();

            item.FromDto(postedItem);

            if (string.IsNullOrEmpty(bvin))
            {
                if (HccApp.CatalogServices.ProductImageCreate(item))
                {
                    bvin = item.Bvin;
                }
            }
            else
            {
                HccApp.CatalogServices.ProductImageUpdate(item);
            }

            var resultItem = HccApp.CatalogServices.ProductImages.Find(bvin);

            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);

            return data;
        }

        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.ProductImageDelete(bvin)};

            data = Json.ObjectToJson(response);

            return data;
        }
    }
}