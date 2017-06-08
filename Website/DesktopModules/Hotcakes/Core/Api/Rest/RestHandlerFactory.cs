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
using Hotcakes.Commerce;

namespace Hotcakes.Modules.Core.Api.Rest
{
    [Serializable]
    public class RestHandlerFactory
    {
        public static IRestHandler Instantiate(string version, string modelname, HotcakesApplication app)
        {
            switch (modelname.Trim().ToLowerInvariant())
            {
                case "categories":
                    return new CategoriesHandler(app);
                case "pricegroups":
                    return new PriceGroupsHandler(app);
                case "customeraccounts":
                    return new CustomerAccountHandler(app);
                case "affiliates":
                    return new AffiliatesHandler(app);
                case "taxschedules":
                    return new TaxSchedulesHandler(app);
                case "taxes":
                    return new TaxesHandler(app);
                case "vendors":
                    return new VendorsHandler(app);
                case "manufacturers":
                    return new ManufacturersHandler(app);
                case "producttypes":
                    return new ProductTypesHandler(app);
                case "productproperties":
                    return new ProductPropertiesHandler(app);
                case "productoptions":
                    return new ProductOptionsHandler(app);
                case "products":
                    return new ProductsHandler(app);
                case "productmainimage":
                    return new ProductsMainImageHandler(app);
                case "categoriesimagesicon":
                    return new CategoriesImagesIconHandler(app);
                case "categoriesimagesbanner":
                    return new CategoriesImagesBannerHandler(app);
                case "productfiles":
                    return new ProductFilesHandler(app);
                case "productfilesdata":
                    return new ProductFilesDataHandler(app);
                case "productfilesxproducts":
                    return new ProductFilesXProductsHandler(app);
                case "productrelationships":
                    return new ProductRelationshipsHandler(app);
                case "productinventory":
                    return new ProductInventoryHandler(app);
                case "productimages":
                    return new ProductImagesHandler(app);
                case "productimagesupload":
                    return new ProductImagesUploadHandler(app);
                case "productvariantssku":
                case "productvariant":
                    return new ProductVariantsHandler(app);
                case "productvolumediscounts":
                    return new ProductVolumeDiscountsHandler(app);
                case "productreviews":
                    return new ProductReviewsHandler(app);
                case "categoryproductassociations":
                    return new CategoryProductAssociationsHandler(app);
                case "searchmanager":
                    return new SearchManagerHandler(app);
                case "orders":
                    return new OrdersHandler(app);
                case "ordertransactions":
                    return new OrderTransactionsHandler(app);
                case "utilities":
                    return new UtilitiesHandler(app);
                case "wishlistitems":
                    return new WishListItemsHandler(app);
                case "catalogroles":
                    return new CatalogRolesHandler(app);
                case "giftcards":
                    return new GiftCardsHandler(app);
            }

            return null;
        }
    }
}