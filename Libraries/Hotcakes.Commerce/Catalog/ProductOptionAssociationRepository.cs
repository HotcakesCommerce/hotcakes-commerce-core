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

using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    public class ProductOptionAssociationRepository : HccSimpleRepoBase<hcc_ProductXOption, ProductOptionAssociation>
    {
        public ProductOptionAssociationRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override void CopyDataToModel(hcc_ProductXOption data, ProductOptionAssociation model)
        {
            model.Id = data.Id;
            model.OptionBvin = DataTypeHelper.GuidToBvin(data.OptionBvin);
            model.ProductBvin = DataTypeHelper.GuidToBvin(data.ProductBvin);
            model.SortOrder = data.SortOrder;
            model.StoreId = data.StoreId;
        }

        protected override void CopyModelToData(hcc_ProductXOption data, ProductOptionAssociation model)
        {
            data.Id = model.Id;
            data.OptionBvin = DataTypeHelper.BvinToGuid(model.OptionBvin);
            data.ProductBvin = DataTypeHelper.BvinToGuid(model.ProductBvin);
            data.SortOrder = model.SortOrder;
            data.StoreId = model.StoreId;
        }

        public ProductOptionAssociation Find(long id)
        {
            return FindFirstPoco(y => y.Id == id);
        }

        public ProductOptionAssociation FindByProductAndOption(string productId, string optionId)
        {
            return FindByProductAndOption(productId, optionId, Context.CurrentStore.Id);
        }

        public ProductOptionAssociation FindByProductAndOption(string productId, string optionId, long storeId)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var optionGuid = DataTypeHelper.BvinToGuid(optionId);
            using (var s = CreateReadStrategy())
            {
                var data = s.GetQuery().AsNoTracking().Where(y => y.ProductBvin == productGuid)
                    .Where(y => y.OptionBvin == optionGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder).FirstOrDefault();

                return FirstPoco(data);
            }
        }

        public override bool Create(ProductOptionAssociation item)
        {
            item.StoreId = Context.CurrentStore.Id;
            item.SortOrder = FindMaxSort(item.ProductBvin) + 1;
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
                    .Where(y => y.ProductBvin == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .Max(y => (int?) y.SortOrder);

                return maxSortOrder ?? 0;
            }
        }

        public bool Update(ProductOptionAssociation c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            return Update(c, y => y.Id == c.Id && y.StoreId == Context.CurrentStore.Id);
        }

        public List<ProductOptionAssociation> FindAll()
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            });
        }

        public List<ProductOptionAssociation> FindAllForAllStores()
        {
            return FindAllPagedForAllStores(1, int.MaxValue);
        }

        public new List<ProductOptionAssociation> FindAllPaged(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q =>
            {
                return q.Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        public List<ProductOptionAssociation> FindAllPagedForAllStores(int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            return FindListPoco(q => { return q.OrderBy(y => y.SortOrder); }, pageNumber, pageSize);
        }

        public List<ProductOptionAssociation> FindForProducts(List<string> productIds, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuids = productIds.Select(pi => DataTypeHelper.BvinToGuid(pi)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => productGuids.Contains(y.ProductBvin))
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        public List<ProductOptionAssociation> FindForProduct(string productId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductBvin == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        public List<ProductOptionAssociation> FindForOption(string optionId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var optionGuid = DataTypeHelper.BvinToGuid(optionId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.OptionBvin == optionGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.SortOrder);
            }, pageNumber, pageSize);
        }

        public bool DeleteAllForProduct(string productId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return Delete(y => y.ProductBvin == productGuid && y.StoreId == storeId);
        }

        public bool DeleteAllForOption(string optionId)
        {
            var storeId = Context.CurrentStore.Id;
            var optionGuid = DataTypeHelper.BvinToGuid(optionId);
            return Delete(y => y.OptionBvin == optionGuid && y.StoreId == storeId);
        }

        public bool AddOptionToProduct(string productId, string optionId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var optionGuid = DataTypeHelper.BvinToGuid(optionId);
            using (var s = CreateStrategy())
            {
                var exists = s.GetQuery()
                    .Where(y => y.ProductBvin == productGuid)
                    .Where(y => y.OptionBvin == optionGuid)
                    .Where(y => y.StoreId == storeId)
                    .SingleOrDefault();

                if (exists == null)
                {
                    var x = new ProductOptionAssociation();
                    x.ProductBvin = productId;
                    x.OptionBvin = optionId;
                    return Create(x);
                }

                return true;
            }
        }

        public bool RemoveOptionFromProduct(string productId, string optionId)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            var optionGuid = DataTypeHelper.BvinToGuid(optionId);
            return Delete(y => y.ProductBvin == productGuid && y.OptionBvin == optionGuid);
        }

        private bool UpdateSortOrderForOption(string productId, string optionId, int newSortOrder)
        {
            var c1 = FindByProductAndOption(productId, optionId);
            if (c1 == null) return false;
            c1.SortOrder = newSortOrder;
            return Update(c1);
        }

        public bool ResortOptionsForProduct(string productId, List<string> sortedIds)
        {
            if (sortedIds != null)
            {
                for (var i = 1; i <= sortedIds.Count; i++)
                {
                    UpdateSortOrderForOption(productId, sortedIds[i - 1], i);
                }
            }
            return true;
        }
    }
}