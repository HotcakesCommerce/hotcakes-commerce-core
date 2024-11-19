#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-2023 Upendo Ventures, LLC
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
using Hotcakes.Web;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    public class VariantRepository : HccSimpleRepoBase<hcc_Variants, Variant>
    {
        public VariantRepository(HccRequestContext c)
            : base(c)
        {
        }

        protected override Func<hcc_Variants, bool> MatchItems(Variant item)
        {
            var guid = DataTypeHelper.BvinToGuid(item.Bvin);
            return v => v.bvin == guid;
        }

        protected override Func<hcc_Variants, bool> NotMatchItems(List<Variant> items)
        {
            var itemGuids = items.Select(i => DataTypeHelper.BvinToGuid(i.Bvin)).ToList();
            return v => !itemGuids.Contains(v.bvin);
        }

        protected override void CopyDataToModel(hcc_Variants data, Variant model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.Price = data.Price;
            model.ProductId = DataTypeHelper.GuidToBvin(data.ProductId);
            var selectionData = Json.ObjectFromJson<List<OptionSelection>>(data.SelectionData);
            if (selectionData != null)
            {
                model.Selections.Clear();
                model.Selections.AddRange(selectionData);
            }
            model.Sku = data.Sku ?? string.Empty;
            model.StoreId = data.StoreId;
            model.CustomProperty = data.CustomProperty;
        }

        protected override void CopyModelToData(hcc_Variants data, Variant model)
        {
            data.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Price = model.Price;
            data.ProductId = DataTypeHelper.BvinToGuid(model.ProductId);
            data.SelectionData = Json.ObjectToJson(model.Selections);
            data.Sku = model.Sku;
            data.StoreId = model.StoreId;
            data.CustomProperty = model.CustomProperty;
        }


        public Variant Find(string bvin)
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

        public Variant FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.bvin == guid);
        }

        public override bool Create(Variant item)
        {
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        public bool Update(Variant c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        public bool Delete(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid);
        }

        public bool IsSkuExist(string sku, Guid? excludeProductId = null)
        {
            var storeId = Context.CurrentStore.Id;
            sku = sku.ToLower();
            using (var s = CreateReadStrategy())
            {
                var q = excludeProductId.HasValue
                    ? s.GetQuery().Where(i => i.ProductId != excludeProductId)
                    : s.GetQuery();
                return q.Where(y => y.StoreId == storeId).Any(i => i.Sku.ToLower() == sku);
            }
        }

        public List<Variant> FindByProductIds(List<string> productIds)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuids = productIds.Select(id => DataTypeHelper.BvinToGuid(id)).ToList();
            return FindListPoco(q =>
            {
                return q.Where(y => productGuids.Contains(y.ProductId))
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.ProductId);
            });
        }

        public List<Variant> FindByProductId(string productId)
        {
            return FindByProductIdPaged(productId, 1, int.MaxValue);
        }

        public List<Variant> FindByProductIdPaged(string productId, int pageNumber, int pageSize)
        {
            var storeId = Context.CurrentStore.Id;
            var productGuid = DataTypeHelper.BvinToGuid(productId);
            return FindListPoco(q =>
            {
                return q.Where(y => y.ProductId == productGuid)
                    .Where(y => y.StoreId == storeId)
                    .OrderBy(y => y.ProductId);
            }, pageNumber, pageSize);
        }

        public bool DeleteForProductId(string productBvin)
        {
            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            return Delete(y => y.ProductId == productGuid);
        }

        public void MergeList(string productBvin, List<Variant> subitems)
        {
            var storeId = Context.CurrentStore.Id;
            // Set Base Key Field
            foreach (var item in subitems)
            {
                item.ProductId = productBvin;
                item.StoreId = storeId;

                if (string.IsNullOrEmpty(item.Bvin))
                    item.Bvin = Guid.NewGuid().ToString();
            }

            var productGuid = DataTypeHelper.BvinToGuid(productBvin);
            MergeList(subitems, v => v.ProductId == productGuid);
        }
    }
}
