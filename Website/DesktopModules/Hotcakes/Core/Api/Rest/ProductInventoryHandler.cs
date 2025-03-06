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
using Hotcakes.Commerce.Catalog;
using Hotcakes.CommerceDTO.v1;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.Web;

namespace Hotcakes.Modules.Core.Api.Rest
{
    /// <summary>
    ///     This class handles all of the REST API calls for product inventory management
    /// </summary>
    [Serializable]
    public class ProductInventoryHandler : BaseRestHandler
    {
        public ProductInventoryHandler(HotcakesApplication app)
            : base(app)
        {
        }

        /// <summary>
        ///     The GET method allows the REST API to return the product inventory level to the application that called it
        /// </summary>
        /// <param name="parameters">Parameters passed in the URL of the REST API call</param>
        /// <param name="querystring">
        ///     Name/value pairs from the REST API call querystring. This method does not expect any
        ///     querystring values.
        /// </param>
        /// <returns>
        ///     String - a JSON representation of the Errors/Content to return. Content will contain the serialized version of
        ///     ProductInventoryDTO
        /// </returns>
        public override string GetAction(string parameters, NameValueCollection querystring)
        {
            var data = string.Empty;

            if (string.IsNullOrEmpty(parameters))
            {
                // List
                var byproduct = querystring["byproduct"] ?? string.Empty;
                var response = new ApiResponse<List<ProductInventoryDTO>>();

                var results = new List<ProductInventory>();

                if (byproduct.Trim().Length > 0)
                {
                    results = HccApp.CatalogServices.ProductInventories.FindByProductId(byproduct);
                }
                else
                {
                    results = HccApp.CatalogServices.ProductInventories.FindAllForCurrentStore();
                }

                var dto = new List<ProductInventoryDTO>();

                foreach (var prdInv in results)
                {
                    dto.Add(prdInv.ToDto());
                }
                response.Content = dto;
                data = response.ObjectToJson();

                return data;
            }
            var bvin = FirstParameter(parameters);

            var item = HccApp.CatalogServices.ProductInventories.Find(bvin);

            if (item == null)
            {
                return JsonApiResponseError("NULL", "Could not locate that item. Check bvin and try again.");
            }

            return JsonApiResponse(item.ToDto());
        }

        /// <summary>
        ///     Allows the REST API to create or update inventory for a product
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. If there is a first parameter found in the
        ///     URL, the method will assume it is the product inventory ID (bvin) and that this is an update, otherwise it assumes
        ///     to create product inventory.
        /// </param>
        /// <param name="querystring">Name/value pairs from the REST API call querystring. This is not used in this method.</param>
        /// <param name="postdata">Serialized (JSON) version of the ProductInventoryDTO object</param>
        /// <returns>ProductInventoryDTO - Serialized (JSON) version of the product inventory</returns>
        public override string PostAction(string parameters, NameValueCollection querystring, string postdata)
        {
            var bvin = FirstParameter(parameters);

            ProductInventoryDTO postedItem = null;
            try
            {
                postedItem = Json.ObjectFromJson<ProductInventoryDTO>(postdata);
            }
            catch (Exception ex)
            {
                return JsonApiResponseException(ex);
            }

            var item = new ProductInventory();
            // convert the dto to a cbo
            item.FromDto(postedItem);

            // update the store id if it isn't populated (prevents an obscured issue with updates)
            if (item.StoreId == 0)
            {
                item.StoreId = HccApp.CurrentStore.Id;
            }

            if (string.IsNullOrEmpty(bvin))
            {
                // see if there is already an inventory object for product
                var pi = HccApp.CatalogServices.ProductInventories.FindByProductIdAndVariantId(item.ProductBvin,
                    item.VariantId);
                if (pi != null)
                {
                    // update the existing object with the posted information
                    pi.OutOfStockPoint = item.OutOfStockPoint;
                    pi.LowStockPoint = item.LowStockPoint;
                    pi.QuantityOnHand = item.QuantityOnHand;
                    pi.QuantityReserved = item.QuantityReserved;

                    // update the inventory
                    HccApp.CatalogServices.ProductInventories.Update(pi);

                    // update the local variable to get updated data later
                    bvin = pi.Bvin;
                }
                else
                {
                    // if inventory object doesn't exist yet, create one.
                    if (HccApp.CatalogServices.ProductInventories.Create(item))
                    {
                        // update the local variable to get updated data later
                        bvin = item.Bvin;
                    }
                }
            }
            else
            {
                // update the inventory
                HccApp.CatalogServices.ProductInventories.Update(item);
            }

            // update the visibility/availability status of the product
            if (!string.IsNullOrEmpty(item.ProductBvin))
            {
                var product = HccApp.CatalogServices.Products.Find(item.ProductBvin);
                if (product != null)
                {
                    HccApp.CatalogServices.UpdateProductVisibleStatusAndSave(product);
                }
            }

            // get an updated instance of the inventory
            var resultItem = HccApp.CatalogServices.ProductInventories.Find(bvin);

            // return the result
            return JsonApiResponse(resultItem.ToDto());
        }

        /// <summary>
        ///     Allows for the REST API to delete a product inventory record from the store
        /// </summary>
        /// <param name="parameters">
        ///     Parameters passed in the URL of the REST API call. A single parameter (product inventory
        ///     ID/bvin) is expected.
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
            var response = new ApiResponse<bool> {Content = HccApp.CatalogServices.ProductInventories.Delete(bvin)};

            data = response.ObjectToJson();
            return data;
        }
    }
}