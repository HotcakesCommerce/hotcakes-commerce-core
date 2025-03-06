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
using System.Linq;
using Hotcakes.Commerce;
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for managing product variants
    /// </summary>
    [Serializable]
    public class ProductVariantsHandler : BaseRestHandler
    {
        public ProductVariantsHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     This method is not implemented and should not be called.
        /// </summary>
        /// <param name="parameters">Not implemented</param>
        /// <param name="querystring">Not implemented</param>
        /// <returns>Not implemented</returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var prodid = querystring["productid"];

            var variants = HccApp.CatalogServices.ProductVariants.FindByProductId(prodid);
            var data = variants.Select(v => v.ToDto());

            return JsonApiResponse(data);
        }

        /// <summary>
        ///     Allows the REST API to update the SKU for a specific variant in a product
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. The first parameter found in the URL is the
        ///     product ID (bvin).
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductVariantSkuUpdateDTO object</param>
        /// <returns>Boolean - Serialized (JSON) version of the boolean ApiResponse</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var data = string.Empty;
            var bvin = FirstParameter(parameters);
            var response = new ApiResponse<bool>();

            ProductVariantSkuUpdateDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<ProductVariantSkuUpdateDTO>(postdata);
            }
            catch (Exception ex)
            {
                response.Errors.Add(new ApiError("EXCEPTION", ex.Message));
                return Json.ObjectToJson(response);
            }

            // locate the existing product
            var existing = HccApp.CatalogServices.Products.Find(bvin);

            // only proceed if the product is found in the catalog
            if (existing != null && existing.Bvin != string.Empty)
            {
                // iterate through each variant in the existing product
                foreach (var variant in existing.Variants)
                {
                    // in order to proceed...
                    // the selections in the existing variant must match the options posted in the API call
                    if (SelectionDataMatches(variant.Selections, postedItem.MatchingOptions))
                    {
                        // update the variant with the new SKU
                        variant.Sku = postedItem.Sku;

                        // update the existing product in the catalog
                        HccApp.CatalogServices.Products.Update(existing);

                        // the update was successful
                        response.Content = true;
                        break;
                    }
                }
            }

            data = Json.ObjectToJson(response);
            return data;
        }

        /// <summary>
        ///     This method is not implemented and should not be called.
        /// </summary>
        /// <param name="parameters">Not implemented</param>
        /// <param name="querystring">Not implemented</param>
        /// <param name="postdata">The postdata.</param>
        /// <returns>
        ///     Not implemented
        /// </returns>
        public override string DeleteAction(string parameters, NameValueCollection querystring, string postdata)
        {
            return MethodNotImplementedResponse();
        }

        private bool SelectionDataMatches(OptionSelectionList variantSelections, List<VariantOptionDataDTO> options)
        {
            // the number of existing variant selections & posted options must match
            if (variantSelections.Count != options.Count)
            {
                return false;
            }

            var expectedMatched = options.Count;
            var actualMatches = 0;

            // iterate through each posted option
            foreach (var opt in options)
            {
                // this is a match if the existing variant selection Bvin matches the choice Bvin
                //   AND
                // the existing variant selection data matches the selected posted option
                var match =
                    variantSelections.Where(
                        y =>
                            y.OptionBvin.Replace("-", string.Empty).ToLowerInvariant() ==
                            opt.ChoiceId.Replace("-", string.Empty).ToLowerInvariant()
                            &&
                            y.SelectionData.Replace("-", string.Empty).ToLowerInvariant() ==
                            opt.ChoiceItemId.Replace("-", string.Empty).ToLowerInvariant()).FirstOrDefault();

                // increment the number of matches, when appropriate
                if (match != null)
                {
                    actualMatches++;
                }
            }

            // returns true only if the posted options match the expected available variant selection data
            return actualMatches == expectedMatched;
        }

        private string MethodNotImplementedResponse()
        {
            var response = new ApiResponse<bool>();

            response.Content = false;
            response.Errors.Add(new ApiError("EXCEPTION", "Method not implemented."));

            return Json.ObjectToJson(response);
        }
    }
}