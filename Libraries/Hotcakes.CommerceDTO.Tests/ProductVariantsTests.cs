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

using System.Collections.Generic;
using System.Linq;
using Hotcakes.CommerceDTO.v1.Catalog;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Hotcakes.CommerceDTO.Tests
{
    /// <summary>
    ///     This class is used to perform test against the Product Variants end points in REST API.
    /// </summary>
    [TestClass]
    public class ProductVariantsTests : ApiTestBase
    {
        /// <summary>
        ///     This method is used test get variants by Product.
        /// </summary>
        [TestMethod]
        public void ProductVariants_FindByVariant()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Get Product by Slug
            var res1 = proxy.ProductsBySlug(TestConstants.TestProduct1Slug);
            CheckErrors(res1);

            //Get Product variants by Product
            var res2 = proxy.ProductVariantsFindByProduct(res1.Content.Bvin);
            CheckErrors(res2);
        }

        /// <summary>
        ///     This method is used to test Update Product Variant SKU.
        /// </summary>
        [TestMethod]
        public void ProductVariantUpdateSKU()
        {
            //Create API Proxy.
            var proxy = CreateApiProxy();

            //Get Product Variants by Product.
            var res2 = proxy.ProductVariantsFindByProduct(TestConstants.TestProductBvin);
            var listMatchingOptions = new List<VariantOptionDataDTO>();
            foreach (var item in res2.Content.First().Selections)
            {
                var variantOption = new VariantOptionDataDTO
                {
                    ChoiceId = item.OptionBvin,
                    ChoiceItemId = item.SelectionData
                };

                listMatchingOptions.Add(variantOption);
            }

            //Update SKU for the Product Variant.
            var productvariant = new ProductVariantSkuUpdateDTO
            {
                ProductBvin = TestConstants.TestProductBvin,
                Sku = TestConstants.TestProduct1Sku,
                MatchingOptions = listMatchingOptions
            };
            var updateResponse = proxy.ProductVariantUpdateSku(productvariant);
            CheckErrors(updateResponse);
            Assert.IsTrue(updateResponse.Content);
        }

        /// <summary>
        ///     This method is used to test specific test case scenario related to PBI 12570
        /// </summary>
        [TestMethod]
        public void ProductInventory_Bug12570()
        {
            //Create API Proxy
            var proxy = CreateApiProxy();

            //Find Product by Slug
            var resP = proxy.ProductsBySlug(TestConstants.TestProduct1Slug);
            CheckErrors(resP);

            //Find Product Variants for given product
            var resV = proxy.ProductVariantsFindByProduct(resP.Content.Bvin);
            CheckErrors(resV);

            //Create Product Inventory
            var prodInvDto = new ProductInventoryDTO
            {
                ProductBvin = resP.Content.Bvin,
                VariantId = resV.Content.First().Bvin,
                QuantityOnHand = 11
            };

            //Update Product Inventory
            // 1-st pass
            var resI = proxy.ProductInventoryUpdate(prodInvDto);
            CheckErrors(resI);
            Assert.AreEqual(resI.Content.QuantityOnHand, prodInvDto.QuantityOnHand);

            //Update Product Inventory
            // 2-nd pass
            prodInvDto.QuantityOnHand = 10;
            resI = proxy.ProductInventoryUpdate(prodInvDto);

            CheckErrors(resI);
            Assert.AreEqual(resI.Content.QuantityOnHand, prodInvDto.QuantityOnHand);
        }
    }
}