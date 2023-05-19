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
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;
using Hotcakes.CommerceDTO.v1.Contacts;
using Hotcakes.CommerceDTO.v1.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.IO;
using Hotcakes.CommerceDTO.v1.Membership;
using Hotcakes.CommerceDTO.v1.Taxes;
using System.Collections.Generic;
using Hotcakes.CommerceDTO.v1.Orders;
using System.Threading;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used perform create necesarie test data 
    ///     REST API.
    /// </summary>
    public class SampleData : ApiTestBase
    {
        /// <summary>
        ///     This method create Test Affiliate.
        /// </summary>
        public static AffiliateDTO AffiliatesTestCreate(Api proxy)
        {           
            //Create Test Affiliate
            var affiliate = new AffiliateDTO
            {
                StoreId = 1,
                DisplayName = "TestUserName2" + DateTime.Now.ToString("ss"),
                Enabled = true,
                CommissionAmount = 50,
                ReferralId = "TestReferall2" + DateTime.Now.ToString("ss")
            };

            var createAffiliateResponse = proxy.AffiliatesCreate(affiliate);

           return createAffiliateResponse.Content;
        }

        /// <summary>
        ///     This method remove Test Affiliate.
        /// </summary>
        public static void AffiliatesTestDelete(Api proxy, AffiliateDTO affiliateDTO)
        {
            proxy.AffiliatesDelete(affiliateDTO.Id);
        }

        /// <summary>
        ///     This method create Test Product.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static ProductDTO CreateTestProduct(Api proxy)
        {
            var product = new ProductDTO
            {
                ProductName = "Unit tests product" + DateTime.Now.ToString("ss"),
                AllowReviews = true,
                ListPrice = 687,
                LongDescription = "This is test product",
                Sku = "TST200" + DateTime.Now.ToString("ss"),
                StoreId = 1,
                TaxExempt = true,                
                IsAvailableForSale = true,
                InventoryMode = ProductInventoryModeDTO.AlwayInStock,
                UrlSlug = "test-category-from-unit-tests",
            };

            var response = proxy.ProductsCreate(product, null);
            return response.Content;
        }


        /// <summary>
        ///     This method remove Test Product.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="bvin">Product unique identifier.</param>
        public static void RemoveTestProduct(Api proxy, string bvin)
        {
            var response = proxy.ProductsDelete(bvin);
        }

        /// <summary>
        ///     This method create Test Category.
        /// </summary>
        /// <returns></returns>
        public static CategoryDTO CreateTestCategory(Api proxy)
        {
            //Create Test Category 1
            var dto1 = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category 1",
                RewriteUrl = "hcc-test",
                ParentId = null
            };
            var cat1Respose = proxy.CategoriesCreate(dto1);

            //Create Test Category 2 associated with Category 1
            var dto2 = new CategoryDTO
            {
                StoreId = 1,
                Name = "Test Category 2",
                RewriteUrl = "test-category-from-unit-tests",
                ParentId = cat1Respose.Content.Bvin
            };
            var cat2Respose = proxy.CategoriesCreate(dto2);

            return cat2Respose.Content;
        }

        /// <summary>
        ///     This moethod remove Test Category.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="bvin">Category unique identifier.</param>
        public static void RemoveTestCategory(Api proxy, string bvin)
        {
            //Removes the category to which it is associated if it has any
            if (proxy.CategoriesFind(bvin).Content.ParentId != null)
            {
                var parentID = proxy.CategoriesFind(bvin).Content.ParentId;
                var removeParentResponse = proxy.CategoriesDelete(parentID);
            }
            //Remove Test Category.
            var removeCategoyResponse = proxy.CategoriesDelete(bvin);
        }



        /// <summary>
        ///     This method create Test ProductType.
        /// </summary>
        /// <returns></returns>
        public static ProductTypeDTO CreateTestProductType(Api proxy)
        {
            //Create Test ProductType.
            var productType = new ProductTypeDTO
            {
                ProductTypeName = "UnitTest Type",
                TemplateName = "TestTemplate",
                StoreId = 1,
                IsPermanent = true
            };

            var createResponse = proxy.ProductTypesCreate(productType);

            return createResponse.Content;
        }

        /// <summary>
        ///     This method remove Test ProductType.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="bvin">ProductType unique identifier.</param>
        public static void RemoveTestProductType(Api proxy, string bvin)
        {
            var response = proxy.ProductTypesDelete(bvin);
        }

        /// <summary>
        ///     This method create Test GiftCard.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static GiftCardDTO CreateTestGiftCard(Api proxy)
        {
            //Create Test GiftCard
            var card = new GiftCardDTO
            {
                CardNumber = "GIFT0000-1111-22222",
                IssueDateUtc = DateTime.UtcNow,
                ExpirationDateUtc = DateTime.UtcNow.AddMonths(1),
                Amount = 100,
            };
            //Create
            var response = proxy.GiftCardCreate(card);
            return response.Content;
        }

        /// <summary>
        ///     This method remove Test GiftCard.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="GiftCardId">GiftCard unique identifier.</param>
        public static void RemoveTestGiftCard(Api proxy, long GiftCardId)
        {
            var response = proxy.GiftCardDelete(GiftCardId);
        }

        /// <summary>
        ///     This method create Test Manufacturer.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static VendorManufacturerDTO CreateTestManufacturer(Api proxy)
        {
            //Create Test Manufacturer
            var vendorManufacture = new VendorManufacturerDTO
            {
                Address = new AddressDTO(),
                ContactType = VendorManufacturerTypeDTO.Manufacturer,
                DisplayName = "New Manufacturer",
                EmailAddress = "testmanufacturer@test.com",
                StoreId = 1
            };
            var response = proxy.ManufacturerCreate(vendorManufacture);
            return response.Content;
        }

        /// <summary>
        ///     This method remove Test Manufacturer.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">Manufacturer unique identifier.</param>
        public static void RemoveTestManufacturer(Api proxy, string Bvin)
        {
            var response = proxy.ManufacturerDelete(Bvin);
        }

        /// <summary>
        ///     This method create Test PriceGroup.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static PriceGroupDTO CreateTestPriceGroup(Api proxy)
        {
            //Create Test PriceGroup.
            var priceGroup = new PriceGroupDTO
            {
                StoreId = 1,
                Name = "Test PriceGroup",
                AdjustmentAmount = 5,
                PricingType = PricingTypesDTO.AmountAboveCost
            };
            var response = proxy.PriceGroupsCreate(priceGroup);
            return response.Content;
        }

        /// <summary>
        ///     This method remove Test PriceGroup.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">PriceGroup unique identifier.</param>
        public static void RemoveTestPriceGroup(Api proxy, string Bvin)
        {
            var response = proxy.PriceGroupsDelete(Bvin);
        }

        /// <summary>
        ///     This method create Test Relationship.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static ProductRelationshipDTO CreateTestRelationship(Api proxy)
        {
            //Create Test Product 1 as prerequisites          
            var product1Respose = CreateTestProduct(proxy);

            Thread.Sleep(1000);

            //Create Test Product 2 as prerequisites          
            var product2Respose = CreateTestProduct(proxy);            

            //Create Product Relationship
            var productRelationShip = new ProductRelationshipDTO
            {
                MarketingDescription = "Test Marketing Desc",
                ProductId = product1Respose.Bvin,
                RelatedProductId = product2Respose.Bvin,
                StoreId = 1
            };
            var createResponse = proxy.ProductRelationshipsCreate(productRelationShip);
            return createResponse.Content;
        }

        /// <summary>
        ///     This method remove Test Relationship.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="id">Product Relationship unique identifier.</param>
        public static void RemoveTestRelationship(Api proxy, long id)
        {
            //Find ProductRelationships
            var relation = proxy.ProductRelationshipsFind(id);
            //Unrelate ProductRelationships
            proxy.ProductRelationshipsUnrelate(relation.Content.ProductId,
                relation.Content.RelatedProductId);
            //Remove Test Product 1
            proxy.ProductsDelete(relation.Content.RelatedProductId);
            //Remove Test Product 2
            proxy.ProductsDelete(relation.Content.ProductId);
        }

        /// <summary>
        ///     This method create test ProductReview.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static ProductReviewDTO CreateTestProductReview(Api proxy)
        {
            //Create Test Product as prerequisites          
            var productRespose = CreateTestProduct(proxy);

            //Create Product Review
            var productReview = new ProductReviewDTO
            {
                Approved = true,
                ProductBvin = productRespose.Bvin,
                Rating = ProductReviewRatingDTO.FiveStars,
                UserID = "1",
                ReviewDateUtc = DateTime.UtcNow
            };
            var createResponse = proxy.ProductReviewsCreate(productReview);
            return createResponse.Content;
        }

        /// <summary>
        ///     This method remove Test ProductReview.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">ProductReview unique identifier.</param>
        public static void RemoveTestProductReview(Api proxy, string Bvin)
        {
            string product;
            //Find ProductReviews
            var review = proxy.ProductReviewsFind(Bvin);
            //Save Bvin in variable
            product = review.Content.ProductBvin;
            //Remove Test ProductReview.
            proxy.ProductReviewsDelete(Bvin);
            //Remove Test Product.
            proxy.ProductsDelete(product);
        }

        /// <summary>
        ///     This method create Test TaxSchedules.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static TaxScheduleDTO CreateTestTaxSchedules(Api proxy)
        {
            //Create TaxSchedule
            var taxSchedule = new TaxScheduleDTO
            {
                DefaultRate = 5,
                DefaultShippingRate = 5,
                Name = "Test TaxSchedule",
                StoreId = 1
            };
            var createResponse = proxy.TaxSchedulesCreate(taxSchedule);
            return createResponse.Content;
        }

        /// <summary>
        ///     This method remove Test TaxSchedules.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="id">TaxSchedules unique identifier.</param>
        public static void RemoveTestTaxSchedules(Api proxy, long id)
        {
            var response = proxy.TaxSchedulesDelete(id);
        }

        /// <summary>
        ///     This method create Test Tax.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="id">Tax unique identifier.</param>
        /// <returns></returns>
        public static TaxDTO CreateTestTax(Api proxy, long id)
        {
            //Create Tax
            var tax = new TaxDTO
            {
                Rate = 5,
                ShippingRate = 5,
                ApplyToShipping = true,
                StoreId = 1,
                PostalCode = "33401",
                CountryIsoAlpha3 = "US",
                TaxScheduleId = id
            };
            var createTaxResponse = proxy.TaxesCreate(tax);
            return createTaxResponse.Content;
        }

        /// <summary>
        ///     This method remove Test Tax.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="id">Tax unique identifier.</param>
        public static void RemoveTestTax(Api proxy, long id)
        {
            var response = proxy.TaxesDelete(id);
        }

        /// <summary>
        ///     This method create Test Vendor.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static VendorManufacturerDTO CreateTestVendor(Api proxy)
        {
            //Create Vendor
            var vendorManufacture = new VendorManufacturerDTO
            {
                Address = new AddressDTO(),
                ContactType = VendorManufacturerTypeDTO.Vendor,
                DisplayName = "New Vendor",
                EmailAddress = "testvendor@test.com",
                StoreId = 1
            };
            var response = proxy.VendorCreate(vendorManufacture);
            return response.Content;
        }

        /// <summary>
        ///     This method remove Test Vendor.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">Vendor unique identifier.</param>
        public static void RemoveTestVendor(Api proxy, string Bvin)
        {
            var response = proxy.VendorDelete(Bvin);
        }

        /// <summary>
        ///     This method create Test CustomerAccount.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static CustomerAccountDTO CreateTestCustomerAccount(Api proxy)
        {
            //Create Customer Account
            //User with tTest already exists
            var account = new CustomerAccountDTO
            {
                FirstName = "TestCustomerAccountFirstName" + DateTime.Now.ToString("ss"),
                LastName = "TestCustomerAccountLastName" + DateTime.Now.ToString("ss"),
                Email = "testCustomerAccount@gmail.com",
                Password = "password1",
                BillingAddress = new AddressDTO
                {
                    City = "New York",
                    CountryName = "United States",
                    RegionName = "New York"
                }
            };
            var response = proxy.CustomerAccountsCreate(account);
            return response.Content;
        }

        /// <summary>
        ///     This method remove Test CustomerAccount.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">CustomerAccount unique identifier.</param>
        public static void RemoveTestCustomerAccount(Api proxy, string Bvin)
        {
            var response = proxy.CustomerAccountsDelete(Bvin);
        }

        /// <summary>
        ///     This method create Test Variant.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="productRespose">ProductDTO.</param>
        /// <returns></returns>
        public static void CreateTestVariant(Api proxy, ProductDTO productRespose)
        {

            //Create Test Option as prerequisites 
            var option = new OptionDTO
            {
                StoreId = 1,
                Name = "TEST Option",
                OptionType = OptionTypesDTO.DropDownList,
                IsVariant = true,
                Items = new List<OptionItemDTO>(),
            };
            option.Items.Add(
               new OptionItemDTO
               {
                   Name = "Test",
                   PriceAdjustment = 1.0M,
                   WeightAdjustment = 2.0M,
                   StoreId = 1,
                   OptionBvin = option.Bvin,
                   IsDefault = true,
                   SortOrder = 2,
               }
            );

            var createResponse = proxy.ProductOptionsCreate(option);

            //Assign product option to test product
            var assingtoProductResponse = proxy.ProductOptionsAssignToProduct(createResponse.Content.Bvin,
                productRespose.Bvin, true);

            //Generate product variants for options
            var generateVariantsResponse = proxy.ProductOptionsGenerateAllVariants(productRespose.Bvin);

        }

        /// <summary>
        ///     This method create Test Order.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static OrderDTO CreateTestOrder(Api proxy)
        {

            //Create Test Product 1 as prerequisites          
            var product1Respose = CreateTestProduct(proxy);

            Thread.Sleep(1000);

            //Create Test Product 2 as prerequisites          
            var product2Respose = CreateTestProduct(proxy);

            //Create Order
            var order = new OrderDTO
            {
                StoreId = 1,
                Items = new List<LineItemDTO>
                {
                    new LineItemDTO
                    {
                        StoreId = 1,
                        ProductId = product1Respose.Bvin
                    },
                    new LineItemDTO
                    {
                        StoreId = 1,
                        ProductId = product2Respose.Bvin
                    }
                }
            };
            var response = proxy.OrdersCreate(order);
            return response.Content;
        }

        /// <summary>
        ///     This method remove Test Order.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">Test Order unique identifier.</param>
        public static void RemoveTestOrder(Api proxy, string Bvin)
        {
            //Find Order
            var order = proxy.OrdersFind(Bvin);

            //Remove all Test Products in Order
            foreach (var item in order.Content.Items)
            {
                //Remove Test Products
                RemoveTestProduct(proxy, item.ProductId);
            }
            var response = proxy.OrdersDelete(Bvin);
        }

        /// <summary>
        ///     This method create test OrderTransaction.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <returns></returns>
        public static OrderTransactionDTO CreateTestOrderTransaction(Api proxy)
        {

            //Create Test Product 1 as prerequisites          
            var orderRespose = CreateTestOrder(proxy);

            //Create Test OrderTransactionCard as prerequisites   
            var card = new OrderTransactionCardDataDTO
            {
                CardNumber = "GIFT0000-1111-22222",
                CardHolderName = "TestOrderTransactionCard",
                CardIsEncrypted = false,
                ExpirationMonth = DateTime.Now.Month + 1,
                ExpirationYear = DateTime.Now.Year + 1,
            };

            //Create OrderTransaction
            var orderTransaction = new OrderTransactionDTO
            {
                StoreId = 1,
                OrderId = orderRespose.Bvin,
                OrderNumber = orderRespose.OrderNumber,
                Action = OrderTransactionActionDTO.CashReceived,
                Amount = orderRespose.TotalGrand,
                Success = true,
                Voided = false,
                TimeStampUtc = DateTime.UtcNow,
                CreditCard = card,
            };
            var response = proxy.OrderTransactionsCreate(orderTransaction);
            return response.Content;
        }

        /// <summary>
        ///     This method remove test OrderTransactions.
        /// </summary>
        /// <param name="proxy">REST API Proxy instance.</param>
        /// <param name="Bvin">OrderTransactions unique identifier.</param>
        public static void RemoveTestOrderTransactions(Api proxy, Guid Bvin)
        {
            //Find OrderTransactions
            var trasaction = proxy.OrderTransactionsFind(Bvin);
            //Remove Test Order in OrderTransactions
            RemoveTestOrder(proxy, trasaction.Content.OrderId.ToString());
            //Remove Test OrderTransactions
            var response = proxy.OrderTransactionsDelete(Bvin);
        }
    }
}
