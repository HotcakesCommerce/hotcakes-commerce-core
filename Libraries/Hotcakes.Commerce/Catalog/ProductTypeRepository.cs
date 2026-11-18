#region License

// Distributed under the MIT License
// ============================================================
// Copyright (c) 2019 Hotcakes Commerce, LLC
// Copyright (c) 2020-present Upendo Ventures, LLC
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
using System.Linq.Expressions;
using Hotcakes.Commerce.Data;
using Hotcakes.Commerce.Data.EF;
using Hotcakes.Web.Data;

namespace Hotcakes.Commerce.Catalog
{
    /// <summary>Provides the catalog ProductTypeRepository API.</summary>
    public class ProductTypeRepository :
        HccLocalizationRepoBase<hcc_ProductType, hcc_ProductTypeTranslation, ProductType, Guid>
    {
        public ProductTypeRepository(HccRequestContext c)
            : base(c)
        {
        }

        /// <summary>Gets or sets the Guid value.</summary>
        protected override Expression<Func<hcc_ProductType, Guid>> ItemKeyExp
        {
            get { return pt => pt.bvin; }
        }

        /// <summary>Gets or sets the Guid value.</summary>
        protected override Expression<Func<hcc_ProductTypeTranslation, Guid>> ItemTranslationKeyExp
        {
            get { return ptt => ptt.ProductTypeId; }
        }

        /// <summary>Gets or sets the CopyItemToModel value.</summary>
        /// <param name="data">Input used by this operation.</param>
        /// <param name="model">Input used by this operation.</param>
        protected override void CopyItemToModel(hcc_ProductType data, ProductType model)
        {
            model.Bvin = DataTypeHelper.GuidToBvin(data.bvin);
            model.IsPermanent = data.IsPermanent;
            model.LastUpdated = data.LastUpdated;
            model.StoreId = data.StoreId;
            model.TemplateName = data.TemplateName;
        }

        /// <summary>Gets or sets the CopyTransToModel value.</summary>
        /// <param name="data">Input used by this operation.</param>
        /// <param name="model">Input used by this operation.</param>
        protected override void CopyTransToModel(hcc_ProductTypeTranslation data, ProductType model)
        {
            model.ProductTypeName = data.ProductTypeName;
        }

        /// <summary>Gets or sets the CopyModelToItem value.</summary>
        protected override void CopyModelToItem(JoinedItem<hcc_ProductType, hcc_ProductTypeTranslation> data,
            ProductType model)
        {
            data.Item.bvin = DataTypeHelper.BvinToGuid(model.Bvin);
            data.Item.IsPermanent = model.IsPermanent;
            data.Item.LastUpdated = model.LastUpdated;
            data.Item.StoreId = model.StoreId;
            data.Item.TemplateName = model.TemplateName;
        }

        /// <summary>Gets or sets the CopyModelToTrans value.</summary>
        protected override void CopyModelToTrans(JoinedItem<hcc_ProductType, hcc_ProductTypeTranslation> data,
            ProductType model)
        {
            data.ItemTranslation.ProductTypeId = data.Item.bvin;

            data.ItemTranslation.ProductTypeName = model.ProductTypeName;
        }

        /// <summary>Gets or sets the Find value.</summary>
        /// <param name="bvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public ProductType Find(string bvin)
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

        /// <summary>Gets or sets the FindForAllStores value.</summary>
        /// <param name="bvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public ProductType FindForAllStores(string bvin)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return FindFirstPoco(y => y.Item.bvin == guid);
        }

        /// <summary>Gets or sets the FindNameForType value.</summary>
        /// <param name="bvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public string FindNameForType(string bvin)
        {
            var item = Find(bvin);
            if (item != null)
            {
                return item.ProductTypeName;
            }
            return string.Empty;
        }

        /// <summary>Gets or sets the CreateAsNew value.</summary>
        /// <param name="item">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool CreateAsNew(ProductType item)
        {
            if (item != null)
            {
                item.Bvin = string.Empty;
            }
            return Create(item);
        }

        /// <summary>Gets or sets the Create value.</summary>
        /// <param name="item">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public override bool Create(ProductType item)
        {
            item.LastUpdated = DateTime.UtcNow;
            if (item.Bvin == string.Empty)
            {
                item.Bvin = Guid.NewGuid().ToString();
            }
            item.LastUpdated = DateTime.UtcNow;
            item.StoreId = Context.CurrentStore.Id;
            return base.Create(item);
        }

        /// <summary>Gets or sets the Update value.</summary>
        /// <param name="c">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool Update(ProductType c)
        {
            if (c.StoreId != Context.CurrentStore.Id)
            {
                return false;
            }
            c.LastUpdated = DateTime.UtcNow;
            var guid = DataTypeHelper.BvinToGuid(c.Bvin);
            return Update(c, y => y.bvin == guid);
        }

        /// <summary>Gets or sets the Delete value.</summary>
        /// <param name="bvin">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public bool Delete(string bvin)
        {
            var storeId = Context.CurrentStore.Id;
            return DeleteForStore(bvin, storeId);
        }

        internal bool DeleteForStore(string bvin, long storeId)
        {
            var guid = DataTypeHelper.BvinToGuid(bvin);
            return Delete(y => y.bvin == guid && y.StoreId == storeId);
        }

        /// <summary>Gets or sets the FindAll value.</summary>
        /// <returns>The result of this operation.</returns>
        public List<ProductType> FindAll()
        {
            return FindAllForStore(Context.CurrentStore.Id);
        }

        /// <summary>Gets or sets the FindAllForStore value.</summary>
        /// <param name="storeId">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public List<ProductType> FindAllForStore(long storeId)
        {
            return
                FindListPoco(
                    q =>
                    {
                        return q.Where(y => y.Item.StoreId == storeId).OrderBy(y => y.ItemTranslation.ProductTypeName);
                    });
        }

        /// <summary>Gets or sets the FindByName value.</summary>
        /// <param name="name">Input used by this operation.</param>
        /// <returns>The result of this operation.</returns>
        public List<ProductType> FindByName(string name)
        {
            var storeId = Context.CurrentStore.Id;
            return
                FindListPoco(
                    q =>
                    {
                        return
                            q.Where(y => y.Item.StoreId == storeId)
                                .Where(y => y.ItemTranslation.ProductTypeName == name);
                    });
        }


        internal void DestroyAllForStore(long storeId)
        {
            Delete(y => y.StoreId == storeId);
        }
    }
}
