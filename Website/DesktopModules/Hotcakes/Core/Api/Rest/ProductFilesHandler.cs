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
using System.Collections.Specialized;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class ProductFilesHandler : BaseRestHandler
    {
        public ProductFilesHandler(HotcakesApplication app)
            : base(app)
        {
        }

        // List or Find Single
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            // Find One Specific Category
            var response = new ApiResponse<ProductFileDTO>();
            var bvin = FirstParameter(parameters);
            var item = HccApp.CatalogServices.ProductFiles.Find(bvin);
            if (item == null)
            {
                response.Errors.Add(new ApiError("NULL", "Could not locate that item. Check bvin and try again."));
            }
            else
            {
                response.Content = item.ToDto();
            }

            data = Json.ObjectToJson(response);

            return data;
        }

        // Create or Update
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<ProductFileDTO>();

            ProductFileDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<ProductFileDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            var item = new ProductFile();
            item.FromDto(postedItem);

            if (bvin == string.Empty)
            {
                if (HccApp.CatalogServices.ProductFiles.Create(item))
                {
                    bvin = item.Bvin;
                }
            }
            else
            {
                HccApp.CatalogServices.ProductFiles.Update(item);
            }
            var resultItem = HccApp.CatalogServices.ProductFiles.Find(bvin);
            if (resultItem != null) response.Content = resultItem.ToDto();

            data = Json.ObjectToJson(response);
            return data;
        }

        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.ProductFiles.Delete(bvin)};

            data = Json.ObjectToJson(response);
            return data;
        }
    }
}