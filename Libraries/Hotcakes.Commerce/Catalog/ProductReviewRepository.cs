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
using System.Collections.Generic;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductReviewRepository : HccSimpleRepoBase<hcc_ProductReview, ProductReview>
    {
        public ProductReviewRepository(HccRequestContext context)
            : base(context)
        {
        }

        #region Obsolete

        [Obsolete("Obsolete in 1.8.0. Use Factory.CreateRepo instead")]
        public ProductReviewRepository(HccRequestContext context, bool isForMemoryOnly)
            : this(context)
        {
        }

        #endregion

        protected override Func<hcc_ProductReview, bool> MatchItems(ProductReview item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return pr => pr.bvin == guid;
        }

        protected override Func<hcc_ProductReview, bool> NotMatchItems(List<ProductReview> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return pr => !itemGuids.Contains(pr.bvin);
        }

        protected override void CopyDataToModel(hcc_ProductReview data, ProductReview model)
        {
            model.StoreId = data.StoreId;
            model.Approved = data.Approved == 1 ? true : false;
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.Description = data.Description;
            model.Karma = data.Karma;
            model.LastUpdated = data.lastUpdated;
            model.ProductBvin = DataTypeHelper.GuidToBvin(data.ProductId);
            model.Rating = (ProductReviewRating) data.Rating;
            model.ReviewDateUtc = data.ReviewDate;
            model.UserID = data.UserID;
        }

        protected override void CopyModelToData(hcc_ProductReview data, ProductReview model)
        {
            data.StoreId = model.StoreId;
            data.Approved = model.Approved ? 1 : 0;
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Description = model.Description;
            data.Karma = model.Karma;
            data.lastUpdated = model.LastUpdated;
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductBvin);
            data.Rating = (int) model.Rating;
            data.ReviewDate = model.ReviewDateUtc;
            data.UserID = model.UserID;
        }

        public ProductReview Find(string bvin)
        {
            var result = FindForAllStores(bvin);
            if (result != null)
            {
                if (result.StoreId == Context.CurrentStore.Id)
                {
                    return result;
                }
            }
            return null;
        }

        public ProductReview FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        public override bool Create(ProductReview item)
        {
            item.LastUpdated = DateTime.UtcNow;
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(ProductReview c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }

            c.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid && y.StoreId == Context.CurrentStore.Id);
        }

        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        public bool DeleteForProductId(string productId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductId == productGuid);
        }

        public List<ProductReview> FindByProductIds(List<string> productIds)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuids = productIds.Select(id => DataTypeHelper.BvinToGuid(id)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => productGuids.Contains(y.ProductId))
                    .Where(y => y.StoreId == storeId)
                    .OrderByDescending(y => y.Karma)
                    .ThenByDescending(y => y.ReviewDate);
            });
        }

        public List<ProductReview> FindByProductId(string productId)
        {
            return FindByProductIdPaged(productId, 1, int.MaxValue);
        }

        public List<ProductReview> FindByProductIdPaged(string productId, int pageNumber, int pageSize)
        {
            var guid = DataTypeHelper.BvinToNullableGuid(productId);
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == guid)
                    .Where(y => y.StoreId == storeId)
                    .OrderByDescending(y => y.Karma)
                    .ThenByDescending(y => y.ReviewDate);
            }, pageNumber, pageSize);
        }

        public bool DeleteByCustomerID(string CustomerId)
        {
            return Delete(x => x.UserID == CustomerId);
        }

        public List<ProductReview> FindByCustomerID(string CustomerID)
        {
            // var guid = DataTypeHelper.BvinToNullableGuid(CustomerID);
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.UserID == CustomerID)
                    .Where(y => y.StoreId == storeId);
            }, 1, 1000);
        }

        public void MergeList(string productBvin, List<ProductReview> subitems)
        {
            var storeId = Context.CurrentStore.Id;
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.ProductBvin = productBvin;
                item.StoreId = storeId;
                item.LastUpdated = DateTime.UtcNow;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            MergeList(subitems, pr => pr.ProductId == productGuid);
        }

        public List<ProductReview> FindNotApproved(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.Approved == 0)
                    .Where(y => y.StoreId == storeId)
                    .OrderByDescending(y => y.ReviewDate);
            }, pageNumber, pageSize);
        }

        public bool UpdateKarma(string reviewBvin, int karmaModifier)
        {
            var item = Find(reviewBvin);
            if (item == null) return false;
            item.Karma += karmaModifier;
            return Update(item);
        }
    }
}