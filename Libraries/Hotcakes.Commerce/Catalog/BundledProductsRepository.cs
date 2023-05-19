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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>
    ///     Repository class to perform different database operation on
    ///     hcc_BundledProducts table.
    /// </summary>
    public class BundledProductsRepository : HccSimpleRepoBase<hcc_BundledProducts, BundledProduct>
    {
        public BundledProductsRepository(HccRequestContext context)
            : base(context)
        {
        }

        /// <summary>
        ///     Find product by id from table
        /// </summary>
        /// <param name="id">product id</param>
        /// <returns>Return the bundled product information if find from the database</returns>
        public BundledProduct Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        /// <summary>
        ///     Create new bundled product record in table
        /// </summary>
        /// <param name="item">Bundled product instance. Detailed properties can be found at <see cref="BundledProduct" /></param>
        /// <returns>Return true if new entry done successfully</returns>
        public override bool Create(BundledProduct item)
        {
            item.SortOrder = FindMaxSort(item.ProductId) + 1;
            return base.Create(item);
        }

        /// <summary>
        ///     Update the bundled product in database
        /// </summary>
        /// <param name="c">Bundled product instance. Detailed properties can be found at <see cref="BundledProduct" /></param>
        /// <returns>Return true if the operation completed successfully</returns>
        public bool Update(BundledProduct c)
        {
            return Update(c, y => y.Id == c.Id);
        }

        /// <summary>
        ///     Find list of products by list of ids
        /// </summary>
        /// <param name="productIds">List of product GUIDs</param>
        /// <returns></returns>
        public List<BundledProduct> FindForProducts(List<string> productIds)
        {
            var productGuids = productIds.Select(id => DataTypeHelper.BvinToGuid(id)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => productGuids.Contains(y.ProductId))
                    .OrderBy(y => y.ProductId)
                    .ThenBy(y => y.SortOrder);
            });
        }

        /// <summary>
        ///     Find products by product GUID. It will return the list of the products added to bundle
        /// </summary>
        /// <param name="productId">Product GUID string</param>
        /// <returns>Returns the list of the products in bundle</returns>
        public List<BundledProduct> FindForProduct(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == productGuid)
                    .OrderBy(y => y.SortOrder);
            });
        }

        /// <summary>
        ///     Delete all the products added in the bundle for given product
        /// </summary>
        /// <param name="productId">Product GUID</param>
        /// <returns>Returns true if the products in bundle deleted successfully</returns>
        public bool DeleteAllForProduct(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductId == productGuid);
        }

        /// <summary>
        ///     Remove specific product from all the bundled product
        /// </summary>
        /// <param name="bundleProductId">Product GUID which needs to be removed</param>
        /// <returns>Returns true if the product is deleted from all the bundles</returns>
        public bool DeleteByBundleProductId(string bundleProductId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(bundleProductId);
            return Delete(y => y.BundledProductId == productGuid);
        }

        /// <summary>
        ///     Remove specific individual record from the table
        /// </summary>
        /// <param name="id">Unique identifier of the particular row in table</param>
        /// <returns>returns true if record deleted successfully</returns>
        public bool Delete(long id)
        {
            return Delete(y => y.Id == id);
        }

        /// <summary>
        ///     Change the bundled products order
        /// </summary>
        /// <param name="sortedBundledProductIds">List of the ids for unique  identification of each row</param>
        /// <returns>Returns true if the resort done successfully</returns>
        public bool ResortBundledProducts(List<long> sortedBundledProductIds)
        {
            if (sortedBundledProductIds != null)
            {
                for (var i = 0; i < sortedBundledProductIds.Count; i++)
                {
                    UpdateSortOrderForProduct(sortedBundledProductIds[i], i + 1);
                }
            }
            return true;
        }

        /// <summary>
        ///     Clone products in one bundle product to another bundle product
        /// </summary>
        /// <param name="productId">Source Bundle product GUID</param>
        /// <param name="newProductId">Destination bundle product GUID</param>
        public void CloneForProduct(string productId, string newProductId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var newProductGuid = DataTypeHelper.BvinToGuid(newProductId);
            using (var s = CreateStrategy())
            {
                var items = s.GetQuery()
                    .Where(bp => bp.ProductId == productGuid).ToList();

                foreach (var item in items)
                {
                    s.Detach(item);
                    item.ProductId = newProductGuid;
                    s.Add(item);
                }

                s.SubmitChanges();
            }
        }

        #region Implementation

        /// <summary>
        ///     Copy records from database to the model class.
        /// </summary>
        /// <param name="data">Instance of the hcc_BundledProducts which represents single row in table.</param>
        /// <param name="model">Model class for Bundled product which is used on the view.</param>
        protected override void CopyDataToModel(hcc_BundledProducts data, BundledProduct model)
        {
            model.Id = data.Id;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
            model.BundledProductId = DataTypeHelper.GuidToBvin(data.BundledProductId);
            model.Quantity = data.Quantity;
            model.SortOrder = data.SortOrder;
            //model.SelectionData = data.SelectionData;
        }

        /// <summary>
        ///     Copy model data to the database instance.
        /// </summary>
        /// <param name="data">Instance of the hcc_BundledProducts which represents single row in table.</param>
        /// <param name="model">View model instance.</param>
        protected override void CopyModelToData(hcc_BundledProducts data, BundledProduct model)
        {
            data.Id = model.Id;
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
            data.BundledProductId = DataTypeHelper.BvinToGuid(model.BundledProductId);
            data.Quantity = model.Quantity;
            data.SortOrder = model.SortOrder;
            //data.SelectionData = model.SelectionData;
        }

        #endregion

        #region

        /// <summary>
        ///     Find the maximum sort order number for products in any bundled product
        /// </summary>
        /// <param name="productId">Product GUID</param>
        /// <returns>Returns the max sort order number</returns>
        private int FindMaxSort(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            using (var s = CreateReadStrategy())
            {
                var maxSortOrder = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.ProductId == productGuid)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        /// <summary>
        ///     Update sort order number for given unique record identifier
        /// </summary>
        /// <param name="bundledProductId">Unique row identifier in the table</param>
        /// <param name="newSortOrder">Sort order needs to be set in database</param>
        /// <returns>Returns true if the record has been updated successfully</returns>
        private bool UpdateSortOrderForProduct(long bundledProductId, int newSortOrder)
        {
            var item = Find(bundledProductId);
            if (item == null)
                return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        #endregion
    }
}