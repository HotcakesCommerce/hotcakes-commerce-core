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
using System.Linq.Expressions;
using Hotcakes.Commerce.Catalog;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Commerce.Social;

namespace Hotcakes.Commerce.Dnn
{
    [Serializable]
    public class DnnProductReviewRepository : ProductReviewRepository
    {
        private readonly ISocialService _socialService;

        #region Constructor

        public DnnProductReviewRepository(HccRequestContext c)
            : base(c)
        {
            _socialService = Factory.CreateSocialService(c);
        }

        #endregion
        
        #region Public methods

        public override bool Create(ProductReview item)
        {
            var res = base.Create(item);
            OnCreated(item);
            return res;
        }

        protected override bool Update(ProductReview item, Expression<Func<hcc_ProductReview, bool>> predicate,
            bool mergeSubItems = true)
        {
            var res = base.Update(item, predicate, mergeSubItems);
            OnUpdated(item);
            return res;
        }

        #endregion

        #region Implementation

        private void OnCreated(ProductReview review)
        {
            var productRepository = Factory.CreateRepo<ProductRepository>(Context);
            var product = productRepository.FindWithCache(review.ProductBvin);
            _socialService.UpdateJournalRecord(review, product);
        }

        private void OnUpdated(ProductReview review)
        {
            var productRepository = Factory.CreateRepo<ProductRepository>(Context);
            var product = productRepository.FindWithCache(review.ProductBvin);
            _socialService.UpdateJournalRecord(review, product);
        }

        #endregion
    }
}