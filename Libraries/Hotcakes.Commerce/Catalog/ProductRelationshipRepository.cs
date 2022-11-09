#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020 Upendo Ventures, LLC
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
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductRelationshipRepository : HccSimpleRepoBase<hcc_ProductRelationship, ProductRelationship>
    {
        public ProductRelationshipRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_ProductRelationship data, ProductRelationship model)
        {
            model.Id = data.Id;
            model.StoreId = data.StoreId;
            model.IsSubstitute = data.IsSubstitute;
            model.MarketingDescription = data.MarketingDescription;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
            model.RelatedProductId = DataTypeHelper.GuidToBvin(data.RelatedProductId);
            model.SortOrder = data.SortOrder;
        }

        protected override void CopyModelToData(hcc_ProductRelationship data, ProductRelationship model)
        {
            data.Id = model.Id;
            data.StoreId = model.StoreId;
            data.IsSubstitute = model.IsSubstitute;
            data.MarketingDescription = model.MarketingDescription;
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
            data.RelatedProductId = DataTypeHelper.BvinToGuid(model.RelatedProductId);
            data.SortOrder = model.SortOrder;
        }

        public ProductRelationship Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public ProductRelationship FindByProductAndRelated(string productId, string relatedId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var relatedGuid = DataTypeHelper.BvinToGuid(relatedId);
            return FindFirstPoco(y => y.ProductId == productGuid && y.RelatedProductId == relatedGuid);
        }

        public override bool Create(ProductRelationship item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.SortOrder = FindMaxSort(item.ProductId) + 1;
            return base.Create(item);
        }

        private int FindMaxSort(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            using (var s = CreateReadStrategy())
            {
                var maxSortOrder = s.GetQuery()
                    .AsNoTracking()
                    .Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        public bool Update(ProductRelationship c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id);
        }

        public List<ProductRelationship> FindForProduct(string productId)
        {
            return FindForProduct(productId, Context.CurrentStore.Id);
        }

        public List<ProductRelationship> FindForProduct(string productId, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            });
        }

        public bool DeleteAllForProduct(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductId == productGuid && y.StoreId == storeId);
        }

        public bool RelateProducts(string productId, string relatedProductId, bool isSubstitute)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var relatedProductGuid = DataTypeHelper.BvinToGuid(relatedProductId);
            using (var s = CreateStrategy())
            {
                var exists = s.GetQuery()
                    .Where(y => y.RelatedProductId == relatedProductGuid)
                    .Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .FirstOrDefault();

                if (exists == null)
                {
                    var x = new ProductRelationship();
                    x.ProductId = productId;
                    x.RelatedProductId = relatedProductId;
                    x.IsSubstitute = isSubstitute;
                    return Create(x);
                }
            }
            return true;
        }

        public bool UnrelateProducts(string productId, string relatedProductId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var relatedProductGuid = DataTypeHelper.BvinToGuid(relatedProductId);
            return
                Delete(
                    y => y.RelatedProductId == relatedProductGuid && y.ProductId == productGuid && y.StoreId == storeId);
        }

        public bool Delete(long relationshipId)
        {
            return Delete(y => y.Id == relationshipId);
        }

        private bool UpdateSortOrderForProduct(string productId, string relatedId, int newSortOrder)
        {
            var item = FindByProductAndRelated(productId, relatedId);
            if (item == null) return false;
            item.SortOrder = newSortOrder;
            return Update(item);
        }

        public bool ResortRelationships(string productId, List<string> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForProduct(productId, sortedIds[i - 1], i);
                }
            }
            return true;
        }
    }
}